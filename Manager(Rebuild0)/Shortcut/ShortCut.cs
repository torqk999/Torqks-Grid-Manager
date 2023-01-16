using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";
        const string CustomSig = "[CPY]";

        const float DefFontSize = .5f;
        const int DefSigCount = 2;
        readonly int[] ScreenRatio = { 25, 17 };
        const int InvSearchCap = 10;
        const int ItemSearchCap = 10;
        const int ProdSearchCap = 10;
        const int ItemMoveCap = 3;
        const int SetupCap = 20;
        const float CleanPercent = .8f;
        const float PowerThreshold = .2f;

        /// WARNING!! DO NOT GO FURTHER USER!! ///

        /// LOGIC
        IMyTextSurface mySurface;

        public readonly char[] EchoLoop = new char[]
{
            '%',
            '$',
            '#',
            '&'
};
        const char Split = '^';
        const string Seperator = "^";
        const int EchoMax = 4;
        const int ClockCycle = 3;

        SearchCounter Counter = new SearchCounter(new int[] { ItemMoveCap, ItemSearchCap });
        StringBuilder Debug = new StringBuilder();
        bool FAIL = false;

        int SetupQueIndex = 0;
        int DisplayQueIndex = 0;
        int InvQueIndex = 0;
        int ProdQueIndex = 0;

        int InvSearchIndex = 0;
        int ProdSearchIndex = 0;

        int InventoryClock = 0;
        int ProdClock = 1;
        int DisplayClock = 2;

        bool bPowerSetupComplete = false;
        bool bSetupComplete = false;
        bool bTallyCycleComplete = false;

        bool bSortRunning = true;
        bool bTallyRunning = true;
        bool bProdRunning = true;
        bool bDisplayRunning = true;
        bool bPowerRunning = true;

        bool bUpdated = false;
        bool bShowProdBuilding = false;
        bool bPowerCharged = false;

        int EchoCount = 0;
        int ProdCharBuffer = 0;

        List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();
        List<IMyTextPanel> PanelBlocks = new List<IMyTextPanel>();

        List<IMyProductionBlock> ProdBlocks = new List<IMyProductionBlock>();
        List<IMyCargoContainer> CargoBlocks = new List<IMyCargoContainer>();
        List<IMyRefinery> RefineryBlocks = new List<IMyRefinery>();

        List<IMyPowerProducer> PowerBlocks = new List<IMyPowerProducer>();
        List<IMyGasTank> TankBlocks = new List<IMyGasTank>();
        List<IMyBatteryBlock> BatteryBlocks = new List<IMyBatteryBlock>();
        List<IMyGasGenerator> GeneratorBlocks = new List<IMyGasGenerator>();
        List<IMyOxygenFarm> OxyFarmBlocks = new List<IMyOxygenFarm>();

        List<_Block> Blocks = new List<_Block>();
        List<_Display> Displays = new List<_Display>();
        List<_Resource> Resources = new List<_Resource>();
        List<_Inventory> Inventories = new List<_Inventory>();

        List<_Cargo> Cargos = new List<_Cargo>();
        List<_Producer> Producers = new List<_Producer>();
        List<_Refinery> Refineries = new List<_Refinery>();

        List<_Production> Productions = new List<_Production>();
        List<_Block> PowerConsumers = new List<_Block>();
        _Resource PowerMonitor;

        public enum Count
        {
            MOVED,
            ITEMS
        }
        public enum _Target
        {
            DEFAULT,
            BLOCK,
            GROUP,
            GRID_GROUPS,
            GRID_BLOCKS,
            ALL_GROUPS,
            ALL_BLOCKS
        }
        public enum _ResType
        {
            POWERGEN,
            GASTANK,
            BATTERY,
            GASGEN,
            OXYFARM
        }
        public enum _ScreenMode
        {
            DEFAULT,
            STATUS,
            INVENTORY,
            RESOURCE,
            PRODUCTION,
            TALLY
        }
        public enum _Notation
        {
            DEFAULT,
            SCIENTIFIC,
            SIMPLIFIED,
            PERCENT
        }

        public class SearchCounter
        {
            public int[] Max;
            public int[] Index;

            public SearchCounter(int[] init)
            {
                Max = init;
                Index = new int[Max.Length];
            }
            public bool Increment(Count count)
            {
                return Increment((int)count);
            }
            public bool Increment(int dim)
            {
                Index[dim]++;
                return Check(dim);
            }

            public bool Check(Count count)
            {
                return Check((int)count);
            }
            public bool Check(int dim)
            {
                if (Index[dim] >= Max[dim])
                    return true;
                return false;
            }

            public void Reset()
            {
                for (int i = 0; i < Index.Length; i++)
                    Index[i] = 0;
            }

            public void Reset(int i)
            {
                Index[i] = 0;
            }
            public void Reset(Count count)
            {
                Reset((int)count);
            }
        }
        public class Tally
        {
            public _Inventory Inventory;
            public MyItemType ItemType;
            public MyFixedPoint CurrentAmount;
            public MyFixedPoint OldAmount;

            public Tally(_Inventory inventory, MyInventoryItem item)
            {
                Inventory = inventory;
                ItemType = item.Type;
            }

            public bool Refresh(out MyFixedPoint change)
            {
                OldAmount = CurrentAmount;
                bool success;

                if (Inventory == null)
                {
                    CurrentAmount = 0;
                    success = false;
                }

                else
                {
                    MyInventoryItem? check = PullInventory(Inventory, false).FindItem(ItemType);
                    if (check == null)
                    {
                        CurrentAmount = 0;
                        success = false;
                    }
                    else
                    {
                        MyInventoryItem item = (MyInventoryItem)check;
                        CurrentAmount = item.Amount;
                        success = true;
                    }
                }

                change = CurrentAmount - OldAmount;
                return success;
            }
        }

        public class _Block
        {
            public long BlockID;
            public int Priority;
            public string CustomName;
            public bool bDetatchable;

            public IMyTerminalBlock TermBlock;

            public _Block(IMyTerminalBlock targetBlock, string signature)
            {
                TermBlock = targetBlock;
                CustomName = TermBlock.CustomName.Replace(signature, "");
                BlockID = targetBlock.EntityId;
                Priority = -1;
            }
        }
        public class _DisplayModule
        {
            public int SigCount;
            public string TargetName;

            public _ScreenMode Mode;
            public _Notation Notation;
            public _Target TargetType;

            public _FilterProfile FilterProfile;
            public _Block TargetBlock;
            public IMyCubeGrid TargetGrid;
            public IMyBlockGroup TargetGroup;

            public _DisplayModule()
            {
                SigCount = DefSigCount;
                TargetName = "No Target";

                Mode = _ScreenMode.DEFAULT;
                FilterProfile = new _FilterProfile(false, false);
                Notation = _Notation.DEFAULT;
                TargetType = _Target.DEFAULT;
            }
        }
        public class _Display : _Block
        {
            public IMyTextPanel Panel;
            public string oldData;
            public float oldFontSize;

            public StringBuilder rawOutput;
            public StringBuilder fOutput;

            public int OutputIndex;
            public int Scroll;
            public int Delay;
            public int Timer;

            public _DisplayModule Module;

            public void RebootScreen(float fontSize)
            {
                if (Panel == null)
                    return;

                Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                Panel.Font = "Monospace";
                Panel.FontSize = fontSize;
            }

            public _Display(IMyTerminalBlock termBlock, string signature, float fontSize = 1) : base(termBlock, signature)
            {
                Panel = (IMyTextPanel)TermBlock;
                RebootScreen(fontSize);
                oldData = Panel.CustomData;
                oldFontSize = Panel.FontSize;
                rawOutput = new StringBuilder();
                fOutput = new StringBuilder();
                OutputIndex = 0;
                Scroll = 1;
                Delay = 10;
                Timer = 0;

                Module = new _DisplayModule();
            }
        }
        public struct FilterMeta
        {
            public bool IN_BOUND;
            public bool OUT_BOUND;
            public MyFixedPoint Target;

            public FilterMeta(bool In = true, bool Out = true, MyFixedPoint target = new MyFixedPoint())
            {
                IN_BOUND = In;
                OUT_BOUND = Out;
                Target = target;
            }
        }
        public class _Filter
        {
            public string ItemType;
            public string ItemSubType;
            public FilterMeta Meta;

            void GenerateFilters(string combo)
            {
                ItemType = "null";
                ItemSubType = "null";

                try
                {
                    string[] strings = combo.Split(':');
                    ItemType = strings[0];
                    ItemSubType = strings[1];

                    ItemType = ItemType == "any" || ItemType == "" ? "any" : ItemType;
                    ItemSubType = ItemSubType == "any" || ItemSubType == "" ? "any" : ItemSubType;
                }
                catch { }
            }

            void GenerateFilters(MyDefinitionId id) // Production
            {
                ItemType = "Component";
                ItemSubType = id.SubtypeName.ToString();
            }

            void GenerateFilters(MyItemType type)
            {
                ItemType = type.TypeId;
                ItemSubType = type.SubtypeId;
            }

            public _Filter(FilterMeta meta, string combo)
            {
                Meta = meta;
                GenerateFilters(combo);
            }
            public _Filter(FilterMeta meta, MyItemType type)
            {
                Meta = meta;
                GenerateFilters(type);
            }
            public _Filter(FilterMeta meta, MyDefinitionId id)
            {
                Meta = meta;
                GenerateFilters(id);
            }
        }
        public class _FilterProfile
        {
            public List<_Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;

            public _FilterProfile(bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false)
            {
                Filters = new List<_Filter>();
                DEFAULT_OUT = defOut;
                DEFAULT_IN = defIn;
                FILL = defFill;
                EMPTY = defEmpty;
            }
        }
        public class _Inventory : _Block
        {
            public _FilterProfile FilterProfile;
            public int CallBackIndex;
            public _Inventory(IMyTerminalBlock termBlock, _FilterProfile profile, string signature, bool active = true) : base(termBlock, signature)
            {
                FilterProfile = profile;
                CallBackIndex = -1;
            }
        }
        public class _Cargo : _Inventory
        {
            public _Cargo(IMyCargoContainer cargoBlock, _FilterProfile profile, string signature) : base(cargoBlock, profile, signature)
            {

            }
        }
        public class _Producer : _Inventory
        {
            public IMyProductionBlock ProdBlock;

            public bool CLEAN;
            public _Producer(IMyProductionBlock prodBlock, _FilterProfile profile, string signature) : base((IMyTerminalBlock)prodBlock, profile, signature)
            {
                ProdBlock = prodBlock;
                CLEAN = true;
            }
        }
        public class _Refinery : _Inventory
        {
            public IMyRefinery RefineBlock;
            public bool AutoRefine;

            public _Refinery(IMyRefinery refineBlock, _FilterProfile profile, string signature, bool auto = false) : base(refineBlock, profile, signature)
            {
                RefineBlock = refineBlock;
                AutoRefine = auto;
                RefineBlock.UseConveyorSystem = AutoRefine;
            }
        }
        public class _Production
        {
            public MyDefinitionId Def;
            public _Filter Filter;
            public string ProdIdString;
            public MyFixedPoint Current;
            public List<Tally> Tallies;

            public _Production(MyDefinitionId def, string prodIdString, int target = 0)
            {
                ProdIdString = prodIdString;
                Def = def;
                Tallies = new List<Tally>();
                FilterMeta meta = new FilterMeta(true, true, target);
                Filter = new _Filter(meta, Def);
            }

            public void TallyUpdate(_Inventory sourceInventory, ref SearchCounter counter)
            {
                if (counter == null)
                    return;
                IMyInventory inventory = PullInventory(sourceInventory, false);
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                inventory.GetItems(items);

                foreach (MyInventoryItem item in items)
                {
                    if (counter.Increment(Count.ITEMS))
                        break;

                    if (!FilterCompare(Filter, item))
                        continue;

                    Tally sourceTally = Tallies.Find(x => x.Inventory == sourceInventory);
                    if (sourceTally == null)
                    {
                        sourceTally = new Tally(sourceInventory, item);
                        Tallies.Add(sourceTally);
                    }

                    MyFixedPoint change = 0;
                    if (!sourceTally.Refresh(out change))
                    {
                        Tallies.Remove(sourceTally);
                    }

                    Current += change;
                }
            }
        }
        public class _Resource : _Block
        {
            public _ResType Type;
            public bool bIsValue;

            public _Resource(IMyTerminalBlock termBlock, string signature, _ResType type, bool isValue = true) : base(termBlock, signature)
            {
                Type = type;
                bIsValue = isValue;
            }
        }

        /// Helpers
        static Dictionary<string, MyFixedPoint> ItemListBuilder(Dictionary<string, MyFixedPoint> dictionary, List<MyInventoryItem> items, _FilterProfile profile = null)
        {
            foreach (MyInventoryItem nextItem in items)
            {
                MyFixedPoint target;
                if (profile != null && !ProfileCompare(profile, nextItem, out target))
                    continue;

                string itemName = nextItem.Type.ToString().Split('/')[1];
                string itemAmount = nextItem.Amount.ToString();

                if (nextItem.Type.TypeId.Split('_')[1] == "Ore" || nextItem.Type.TypeId.Split('_')[1] == "Ingot")   // Distinguish between Ore and Ingot Types
                    itemName = nextItem.Type.TypeId.Split('_')[1] + ":" + nextItem.Type.SubtypeId;

                if (nextItem.Type.TypeId.Split('_')[1] == "Ingot" && nextItem.Type.SubtypeId == "Stone")
                    itemName = "Gravel";

                if (!dictionary.ContainsKey(itemName))  // Summate like stacks into same dictionary value
                    dictionary[itemName] = nextItem.Amount;
                else
                    dictionary[itemName] += nextItem.Amount;
            }

            return dictionary;
        }
        static IMyInventory PullInventory(_Inventory inventory, bool input = true)
        {
            if (inventory is _Cargo)
            {
                return ((IMyCargoContainer)inventory.TermBlock).GetInventory();
            }
            else
            {
                return (input) ? ((IMyProductionBlock)inventory.TermBlock).InputInventory : ((IMyProductionBlock)inventory.TermBlock).OutputInventory;
            }
        }

        static string NotationBundler(float value, int sigCount)
        {
            int sci = 0;

            while (value > 10 || value < 1)
            {
                if (value > 10)
                {
                    value /= 10;
                    sci++;
                }
                if (value < 1)
                {
                    value *= 10;
                    sci--;
                }
            }

            string output = value.ToString($"n{sigCount}");
            output += "e" + sci;
            return output;
        }
        static string SimpleBundler(float value, int sigCount)
        {
            string simp = "  ";

            if (value > 1000000000)
            {
                value /= 1000000000;
                simp = " B";
            }
            else if (value > 1000000)
            {
                value /= 1000000;
                simp = " M";
            }
            else if (value > 1000)
            {
                value /= 1000;
                simp = " K";
            }

            string output = (value % (int)value > 0) ? value.ToString($"n{sigCount}") : value.ToString().PadRight(value.ToString().Length + sigCount + 1);
            output += simp;
            return output;
        }
        static string ParseItemTotal(KeyValuePair<string, MyFixedPoint> item, _DisplayModule module)
        {
            string nextOut = "$" + Seperator + item.Key + Seperator;

            switch (module.Notation)
            {
                case _Notation.PERCENT: // has no use here
                case _Notation.DEFAULT:
                    //nextOut += item.Value.ToString();
                    nextOut += ((int)item.Value).ToString();    // decimaless def
                    break;

                case _Notation.SCIENTIFIC:
                    nextOut += NotationBundler((float)item.Value, module.SigCount);
                    break;

                case _Notation.SIMPLIFIED:
                    nextOut += SimpleBundler((float)item.Value, module.SigCount);
                    break;
            }

            return nextOut;
        }

        static int MonoSpaceChars(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
        }
        static int MonoSpaceLines(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
        }

        /// Comparisons
        static bool Contains(string source, string target)
        {
            if (target == null)
                return true;

            /*if (source == null && target == null)
                return true;

            if (source == null || target == null)
                return false;*/

            if (source.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
        static bool FilterCompare(_Filter A, MyInventoryItem B)
        {
            return FilterCompare(
                A.ItemType, A.ItemSubType,
                B.Type.TypeId, B.Type.SubtypeId);
        }
        static bool FilterCompare(StringBuilder debug, _Filter A, MyProductionItem B)
        {
            return FilterCompare(
                A.ItemType, A.ItemSubType,
                "Component", // >: |
                B.BlueprintId.SubtypeId.ToString());
        }
        static bool FilterCompare(string a, string A, string b, string B)
        {
            if (a != "any" && !Contains(b, a))
                return false;

            if (A != "any" && !Contains(A, B))
                return false;

            return true;
        }
        static bool ProfileCompare(_FilterProfile profile, MyInventoryItem item, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;
            bool match = false;

            if (profile.Filters.Count == 0)
            {
                if (dirIn && profile.DEFAULT_IN)
                    return true;

                if (!dirIn && profile.DEFAULT_OUT)
                    return true;
            }

            foreach (_Filter filter in profile.Filters)
            {
                if (!FilterCompare(filter, item))
                    continue;

                match = true;

                if (!filter.Meta.IN_BOUND &&
                    dirIn)
                    return false;

                if (!filter.Meta.OUT_BOUND &&
                    !dirIn)
                    return false;

                target = (filter.Meta.Target == 0) ? target : filter.Meta.Target;
            }

            return dirIn ? match : true;
        }

        /// Initializers
        void BlockDetection()
        {
            BlockGroups.Clear();
            PanelBlocks.Clear();
            ProdBlocks.Clear();
            CargoBlocks.Clear();

            PowerBlocks.Clear();
            TankBlocks.Clear();
            BatteryBlocks.Clear();
            GeneratorBlocks.Clear();
            OxyFarmBlocks.Clear();

            GridTerminalSystem.GetBlockGroups(BlockGroups);

            GridTerminalSystem.GetBlocksOfType(PanelBlocks);
            GridTerminalSystem.GetBlocksOfType(CargoBlocks);
            GridTerminalSystem.GetBlocksOfType(ProdBlocks);
            GridTerminalSystem.GetBlocksOfType(RefineryBlocks);
            GridTerminalSystem.GetBlocksOfType(PowerBlocks);
            GridTerminalSystem.GetBlocksOfType(TankBlocks);
            GridTerminalSystem.GetBlocksOfType(BatteryBlocks);
            GridTerminalSystem.GetBlocksOfType(GeneratorBlocks);
            GridTerminalSystem.GetBlocksOfType(OxyFarmBlocks);

            ProdBlocks = ProdBlocks.Except(RefineryBlocks).ToList();
            PowerBlocks = PowerBlocks.Except(BatteryBlocks).ToList();
        }
        void ClearClassLists()
        {
            Blocks.Clear();
            Displays.Clear();
            Resources.Clear();
            Inventories.Clear();

            Cargos.Clear();
            Producers.Clear();
            Refineries.Clear();
            //Rotations.Clear();

            Productions.Clear();
            PowerConsumers.Clear();
        }

        /// Display
        void DisplayUpdate(_Display display)
        {
            if (!CheckBlockExists(display))
                return;

            RawStringBuilder(display);
            FormattedStringBuilder(display);
        }
        void DisplaySetup(_Display display)
        {
            display.Module.FilterProfile.Filters.Clear();

            string[] data = display.Panel.CustomData.Split('\n');

            foreach (string nextline in data)
            {
                char check = (nextline.Length > 0) ? nextline[0] : '/';

                string[] lineblocks = nextline.Split(' ');

                switch (check)
                {
                    case '/': // Comment Section (ignored)
                        break;

                    case '*': // Mode
                        if (Contains(nextline, "stat"))
                            display.Module.Mode = _ScreenMode.STATUS;
                        if (Contains(nextline, "inv"))
                            display.Module.Mode = _ScreenMode.INVENTORY;
                        if (Contains(nextline, "prod"))
                            display.Module.Mode = _ScreenMode.PRODUCTION;
                        if (Contains(nextline, "res"))
                            display.Module.Mode = _ScreenMode.RESOURCE;
                        if (Contains(nextline, "tally"))
                            display.Module.Mode = _ScreenMode.TALLY;
                        break;

                    case '@': // Target
                        if (Contains(nextline, "block"))
                        {
                            _Block block = Blocks.Find(x => x.CustomName.Contains(lineblocks[1]));

                            if (block != null)
                            {

                                display.Module.TargetType = _Target.BLOCK;
                                display.Module.TargetBlock = block;
                                display.Module.TargetName = block.CustomName;
                            }
                            else
                            {
                                display.Module.TargetType = _Target.DEFAULT;
                                display.Module.TargetName = "Block not found!";
                            }
                            break;
                        }
                        if (Contains(nextline, "group"))
                        {
                            IMyBlockGroup targetGroup = BlockGroups.Find(x => x.Name.Contains(lineblocks[1]));
                            if (targetGroup != null)
                            {
                                display.Module.TargetType = _Target.GROUP;
                                display.Module.TargetGroup = targetGroup;
                                display.Module.TargetName = targetGroup.Name;
                            }
                            else
                            {
                                display.Module.TargetType = _Target.DEFAULT;
                                display.Module.TargetName = "Group not found!";
                            }
                            break;
                        }
                        break;

                    case '&':   // Option
                        if (Contains(nextline, "scroll"))
                        {
                            int newDelay = Convert.ToInt32(lineblocks[1]);
                            if (newDelay > 0)
                                display.Delay = newDelay;
                            else
                                display.Delay = 10;
                        }
                        break;

                    case '#':   // Notation
                        if (Contains(nextline, "def"))
                            display.Module.Notation = _Notation.DEFAULT;
                        if (Contains(nextline, "simp"))
                            display.Module.Notation = _Notation.SIMPLIFIED;
                        if (Contains(nextline, "sci"))
                            display.Module.Notation = _Notation.SCIENTIFIC;
                        if (Contains(nextline, "%"))
                            display.Module.Notation = _Notation.PERCENT;
                        break;
                }

                FilterSetup(nextline, display.Module.FilterProfile);
            }
        }
        void DisplayRefresh(_Display display)
        {
            display.Panel.WriteText("", false);
            int Offset = (display.Module.TargetType == _Target.GROUP && display.Module.Mode == _ScreenMode.INVENTORY) ? 4 : 2; // Header Work around
            int lineCount = MonoSpaceLines(ScreenRatio[1], display.Panel) - Offset;

            bool tick = false;
            display.Timer++;
            if (display.Timer >= display.Delay)
            {
                display.Timer = 0;
                tick = true;
            }

            string[] lines = display.fOutput.ToString().Split('\n');

            if (lines.Length > MonoSpaceLines(ScreenRatio[1], display.Panel)) // Requires Scrolling
            {
                List<string> formattedSection = new List<string>();
                if (display.OutputIndex < Offset || display.OutputIndex > (lines.Length - lineCount)) // Index Reset Failsafe
                    display.OutputIndex = Offset;

                for (int i = 0; i < Offset; i++)
                    formattedSection.Add(lines[i]);

                for (int i = display.OutputIndex; i < (display.OutputIndex + lineCount); i++)
                    formattedSection.Add(lines[i]);

                if (display.OutputIndex == lines.Length - lineCount)
                    display.Scroll = -1;
                if (display.OutputIndex == Offset)
                    display.Scroll = 1;

                if (tick)
                    display.OutputIndex += display.Scroll;

                foreach (string nextString in formattedSection)
                    display.Panel.WriteText(nextString + "\n", true);
            }

            else // Static
            {
                foreach (string nextString in lines)
                    display.Panel.WriteText(nextString + "\n", true);
            }
        }
        void RawStringBuilder(_Display display)
        {
            display.rawOutput.Clear();

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    display.rawOutput.Append($"={Seperator}[Default]\n");
                    break;

                case _ScreenMode.INVENTORY:
                    display.rawOutput.Append($"={Seperator}[Inventory]\n");
                    break;

                case _ScreenMode.RESOURCE:
                    display.rawOutput.Append($"={Seperator}[Resource]\n");
                    break;

                case _ScreenMode.STATUS:
                    display.rawOutput.Append($"={Seperator}[Status]\n");
                    break;

                case _ScreenMode.PRODUCTION:
                    display.rawOutput.Append($"={Seperator}[Production]\n");
                    ProductionBuilder(display.rawOutput);
                    break;

                case _ScreenMode.TALLY:
                    display.rawOutput.Append($"={Seperator}[Tally]\n");
                    ItemTallyBuilder(display);
                    break;
            }

            display.rawOutput.Append("#\n");

            if (display.Module.Mode > (_ScreenMode)3)
                return; // No Target for tally systems

            switch (display.Module.TargetType)
            {
                case _Target.DEFAULT:
                    display.rawOutput.Append("=" + "/" + display.Module.TargetName);
                    break;

                case _Target.BLOCK:
                    if (display.Module.Mode == _ScreenMode.RESOURCE || display.Module.Mode == _ScreenMode.STATUS)
                        TableHeaderBuilder(display);
                    RawBlockBuilder(display, display.Module.TargetBlock);
                    break;

                case _Target.GROUP:
                    RawGroupBuilder(display, display.Module.TargetGroup);
                    break;
            }

        }
        void FormattedStringBuilder(_Display display)
        {
            display.fOutput.Clear();

            int chars = MonoSpaceChars(ScreenRatio[0], display.Panel);
            string[] strings = display.rawOutput.ToString().Split('\n');

            foreach (string nextString in strings) // NEEDS UPDATEING!!
            {
                string[] blocks = nextString.Split(Split);
                string formattedString = string.Empty;
                int remains = 0;

                switch (blocks[0])
                {
                    case "#": // Empty Line
                        break;

                    case "!": // Warning
                        formattedString = blocks[1];
                        break;

                    case "^": // Table-Header
                        if (blocks.Length == 3)
                        {
                            remains = chars - blocks[2].Length;
                            if (remains > 0)
                            {
                                if (remains < blocks[1].Length)
                                    formattedString += blocks[1].Substring(0, remains);
                                else
                                    formattedString += blocks[1];
                            }
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                formattedString += "-";
                            formattedString += blocks[2];
                        }
                        else
                        {
                            remains = chars - blocks[1].Length;
                            formattedString += "[" + blocks[1] + "]";
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                formattedString += "-";
                        }

                        break;

                    case "=": // Header
                        if (chars <= blocks[1].Length) // Can header fit side dressings?
                        {
                            formattedString = blocks[1];
                        }
                        else // Apply Header Dressings
                        {
                            remains = chars - blocks[1].Length;
                            if (remains % 2 == 1)
                            {
                                blocks[1] += "=";
                                remains -= 1;
                            }
                            for (int i = 0; i < remains / 2; i++)
                                formattedString += "=";
                            formattedString += blocks[1];
                            for (int i = 0; i < remains / 2; i++)
                                formattedString += "=";
                        }

                        break;

                    case "$": // Inventory
                        if (chars < (blocks[1].Length + blocks[2].Length)) // Can Listing fit on one line?
                        {
                            formattedString = $"{blocks[1]}\n{blocks[2]}";
                        }
                        else
                        {
                            formattedString += blocks[1];
                            for (int i = 0; i < (chars - (blocks[1].Length + blocks[2].Length)); i++)
                                formattedString += "-";
                            formattedString += blocks[2];
                        }
                        break;

                    case "%": // Resource
                        if (!blocks[2].Contains("%"))
                            blocks[2] += "|" + blocks[3];
                        if (chars < (blocks[1].Length + blocks[2].Length)) // Can Listing fit on one line?
                        {
                            formattedString = $"{blocks[1]}\n{blocks[2]}";
                        }
                        else
                        {
                            formattedString += blocks[1];
                            for (int i = 0; i < (chars - (blocks[1].Length + blocks[2].Length)); i++)
                                formattedString += "-";
                            formattedString += blocks[2];
                        }
                        break;

                    case "*": // Status
                              // remaining chars = total line chars - (colored blocks + 2 to correct for colored blocks spacing)
                        remains = chars - (blocks[2].Length + 2);
                        if (remains > 0)
                        {
                            if (remains < blocks[1].Length)
                                formattedString += blocks[1].Substring(0, remains);
                            else
                                formattedString += blocks[1];
                        }
                        for (int i = 0; i < (remains - blocks[1].Length); i++)
                            formattedString += "-";
                        formattedString += blocks[2];
                        break;

                    case "@": // Production
                        if (!bShowProdBuilding)
                        {
                            if (chars < (blocks[1].Length + blocks[3].Length + blocks[4].Length + 4)) // Can Listing fit on one line?
                            {
                                formattedString = blocks[1] + "\nCurrent: " + blocks[3] + "\nTarget : " + blocks[4] + "\n";
                            }
                            else
                            {
                                formattedString += blocks[1];
                                for (int i = 0; i < (chars - (blocks[1].Length + blocks[3].Length + blocks[4].Length + 1)); i++)
                                    formattedString += "-";

                                formattedString += blocks[3] + "/" + blocks[4] + "\n";
                            }
                        }

                        else
                        {
                            if (chars < (blocks[1].Length + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)) // Can Listing fit on one line?
                            {
                                formattedString =
                                    blocks[1] +
                                    "\n" + blocks[2] +
                                    "\nCurrent: " + blocks[3] +
                                    "\nTarget : " + blocks[4] + "\n";
                            }
                            else
                            {
                                formattedString += blocks[1];

                                for (int i = 0; i < ProdCharBuffer - blocks[1].Length; i++)
                                    formattedString += " ";
                                formattedString += " | " + blocks[2];
                                for (int i = 0; i < (chars - (ProdCharBuffer + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)); i++)
                                    formattedString += "-";

                                formattedString += blocks[3] + "/" + blocks[4];
                            }
                        }

                        break;
                }

                display.fOutput.Append(formattedString + "\n");
            }
        }

        /// String Builders
        void RawGroupBuilder(_Display display, IMyBlockGroup targetGroup)
        {
            //List<string> output = new List<string>();

            display.rawOutput.Append("=" + Seperator + "(" + display.Module.TargetName + ")\n");
            display.rawOutput.Append("#\n");

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    break;

                case _ScreenMode.INVENTORY:
                    break;

                case _ScreenMode.RESOURCE:
                    TableHeaderBuilder(display);
                    break;

                case _ScreenMode.STATUS:
                    TableHeaderBuilder(display);
                    break;
            }

            List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
            display.Module.TargetGroup.GetBlocks(groupList);

            foreach (IMyTerminalBlock nextTermBlock in groupList)
            {
                if (Blocks.FindIndex(x => x.TermBlock == nextTermBlock) >= 0)
                    RawBlockBuilder(display, Blocks.Find(x => x.TermBlock == nextTermBlock));
                else
                    display.rawOutput.Append("!" + "/" + "Block data class not found! Signature missing from block in group!\n");
            }
        }
        void RawBlockBuilder(_Display display, _Block target)
        {
            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:

                    break;

                case _ScreenMode.INVENTORY:
                    BlockInventoryBuilder(display, target);
                    break;

                case _ScreenMode.RESOURCE:
                    try
                    {
                        _Resource resource = (_Resource)target;
                        BlockResourceBuilder(display, resource);
                    }
                    catch
                    {
                        display.rawOutput.Append("!" + Seperator + "Not a resource!...\n");
                    }
                    break;

                    /*
                case _ScreenMode.STATUS:
                    output.Add(BlockStatusBuilder(target));
                    break;
                    */
            }
        }
        void BlockInventoryBuilder(_Display display, _Block targetBlock)
        {
            //List<string> output = new List<string>();
            display.rawOutput.Append("=" + Seperator + targetBlock.CustomName + "\n");

            if (targetBlock is _Cargo)
                InventoryBuilder(display, PullInventory((_Cargo)targetBlock));

            else if (targetBlock is _Inventory)
            {
                display.rawOutput.Append("=" + Seperator + "|Input|\n");
                InventoryBuilder(display, ((IMyProductionBlock)((_Inventory)targetBlock).TermBlock).InputInventory);
                display.rawOutput.Append("#\n");
                display.rawOutput.Append("=" + Seperator + "|Output|\n");
                InventoryBuilder(display, ((IMyProductionBlock)((_Inventory)targetBlock).TermBlock).OutputInventory);
            }

            else
                display.rawOutput.Append("!" + Seperator + "Invalid Block Type!\n");

            display.rawOutput.Append("#\n");

        }

        void InventoryBuilder(_Display display, IMyInventory targetInventory)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();

            targetInventory.GetItems(items);
            if (items.Count == 0)
            {
                display.rawOutput.Append("!" + Seperator + "Empty!\n");
                return;
            }


            itemTotals = ItemListBuilder(itemTotals, items);

            foreach (var next in itemTotals)
                display.rawOutput.Append(ParseItemTotal(next, display.Module) + "\n");
        }
        void ProductionBuilder(StringBuilder builder)
        {
            builder.Append("#");

            foreach (_Production prod in Productions)
            {
                builder.Append("\n@" + Seperator);
                string nextDef = prod.Filter.ItemSubType;
                builder.Append(nextDef + Seperator);
                ProdCharBuffer = (ProdCharBuffer > nextDef.Length) ? ProdCharBuffer : nextDef.Length;

                builder.Append(
                    prod.ProdIdString + Seperator +
                    prod.Current + Seperator +
                    prod.Filter.Meta.Target);
            }
        }
        void ItemTallyBuilder(_Display display)
        {
            //List<string> output = new List<string>();
            display.rawOutput.Append("#" + Seperator + "\n");

            if (display.Module.FilterProfile.Filters.Count == 0)
            {
                display.rawOutput.Append("!" + Seperator + "No Filter!\n");
                return;
            }

            Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            _Inventory targetInventory;

            switch (display.Module.TargetType)
            {
                case _Target.BLOCK:
                    targetInventory = Inventories.Find(x => x == (_Inventory)display.Module.TargetBlock);
                    PullInventory(targetInventory).GetItems(items);
                    ItemListBuilder(itemTotals, items, display.Module.FilterProfile);
                    break;

                case _Target.GROUP:

                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    display.Module.TargetGroup.GetBlocks(blocks);
                    foreach (IMyTerminalBlock block in blocks)
                    {
                        targetInventory = Inventories.Find(x => x.TermBlock == block);
                        if (targetInventory == null)
                            continue;
                        items.Clear();
                        PullInventory(targetInventory).GetItems(items);
                        ItemListBuilder(itemTotals, items, display.Module.FilterProfile);
                    }
                    break;
            }

            foreach (var next in itemTotals)
                display.rawOutput.Append(ParseItemTotal(next, display.Module) + "\n");
        }
        void TableHeaderBuilder(_Display display)
        {
            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    break;

                case _ScreenMode.INVENTORY:
                    break;

                case _ScreenMode.RESOURCE:
                    display.rawOutput.Append("^" + Seperator + "[Source]" + Seperator + "Val|Uni\n");
                    break;

                case _ScreenMode.STATUS:
                    display.rawOutput.Append("^" + Seperator + "[Target]" + Seperator + "|E  P|I  H|\n");
                    break;
            }
        }
        void BlockResourceBuilder(_Display display, _Resource targetBlock)
        {
            display.rawOutput.Append("%" + Seperator + targetBlock.CustomName + Seperator);

            string value = string.Empty;
            int percent = 0;
            string unit = "n/a";

            switch (display.Module.Notation)
            {
                case _Notation.DEFAULT:
                case _Notation.SCIENTIFIC:
                case _Notation.SIMPLIFIED:

                    switch (targetBlock.Type)
                    {
                        case _ResType.BATTERY:
                            IMyBatteryBlock batBlock = (IMyBatteryBlock)targetBlock.TermBlock;
                            value = batBlock.CurrentStoredPower + "/" + batBlock.MaxStoredPower;
                            unit = batBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Stored power")).Split(' ')[3];
                            break;

                        case _ResType.POWERGEN:
                            IMyPowerProducer powBlock = (IMyPowerProducer)targetBlock.TermBlock;
                            value = powBlock.CurrentOutput + "/" + powBlock.MaxOutput;
                            unit = powBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Current Output")).Split(' ')[3];
                            break;

                        case _ResType.GASTANK:
                            IMyGasTank gasTank = (IMyGasTank)targetBlock.TermBlock;
                            value = gasTank.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Filled")).Split(' ')[2];
                            value = value.Substring(1, value.Length - 2);
                            value = value.Replace("L", "");
                            unit = " L ";
                            break;

                        case _ResType.GASGEN:
                            IMyGasGenerator gasGen = (IMyGasGenerator)targetBlock.TermBlock;
                            value = (gasGen.IsWorking) ? "Running" : "NotRunning";
                            unit = "I/O";
                            break;

                        case _ResType.OXYFARM:
                            IMyOxygenFarm oxyFarm = (IMyOxygenFarm)targetBlock.TermBlock;
                            value = (oxyFarm.IsWorking) ? "Running" : "NotRunning";
                            unit = "I/O";
                            break;
                    }
                    break;

                case _Notation.PERCENT:
                    switch (targetBlock.Type)
                    {
                        case _ResType.BATTERY:
                            IMyBatteryBlock batBlock = (IMyBatteryBlock)targetBlock.TermBlock;
                            percent = Convert.ToInt32((batBlock.CurrentStoredPower / batBlock.MaxStoredPower) * 100f);
                            break;

                        case _ResType.POWERGEN:
                            IMyPowerProducer powBlock = (IMyPowerProducer)targetBlock.TermBlock;
                            percent = (int)((powBlock.CurrentOutput / powBlock.MaxOutput) * 100);
                            break;

                        case _ResType.GASTANK:
                            IMyGasTank gasTank = (IMyGasTank)targetBlock.TermBlock;
                            percent = (int)((gasTank.FilledRatio) * 100);
                            break;

                        case _ResType.GASGEN:
                            IMyGasGenerator gasGen = (IMyGasGenerator)targetBlock.TermBlock;
                            if (gasGen.IsWorking)
                                percent = 100;
                            break;

                        case _ResType.OXYFARM:
                            IMyOxygenFarm oxyFarm = (IMyOxygenFarm)targetBlock.TermBlock;
                            if (oxyFarm.IsWorking)
                                percent = 100;
                            break;
                    }
                    break;
            }


            display.rawOutput.Append(((display.Module.Notation == _Notation.PERCENT) ? percent + "| % " : (value + Seperator + unit)) + "\n");
        }

        /// Inventory
        void FilterSetup(string nextline, _FilterProfile profile)
        {
            /// FILTER CHANGE ///

            string[] lineblocks = nextline.Split(' ');  // Break each line into blocks

            if (lineblocks.Length < 2)  // There must be more than one block to have filter candidate and desired update
                return;

            string itemID = "null"; // Default target, don't update any filters
            bool bDefault = false;
            bool bIn = true;
            bool bOut = true;
            MyFixedPoint target = 0;

            if (lineblocks[0].Contains(":")) // Filter insignia
            {
                itemID = lineblocks[0];
                Echo("Filter");
            }

            if (lineblocks[0].Contains("!")) // Default insignia
            {
                bDefault = true;
                Echo("Default");
            }

            for (int i = 1; i < lineblocks.Length; i++) // iterate through the remaining blocks
            {
                switch (lineblocks[i][0])
                {
                    case '#':   // set a new target value
                        target = (MyFixedPoint)float.Parse(lineblocks[i].Remove(0, 1));
                        Echo("Value");
                        break;

                    case '+':
                        bIn = (Contains(lineblocks[i], "in")) ? true : bIn;
                        bOut = (Contains(lineblocks[i], "out")) ? true : bOut;
                        Echo("WhiteList");
                        break;

                    case '-':
                        bIn = (Contains(lineblocks[i], "in")) ? false : bIn;
                        bOut = (Contains(lineblocks[i], "out")) ? false : bOut;
                        Echo("BlackList");
                        break;
                }
            }

            if (bDefault)
            {
                profile.DEFAULT_IN = bIn;
                profile.DEFAULT_OUT = bOut;
            }

            if (itemID != "null")
            {
                FilterMeta meta = new FilterMeta(bIn, bOut, target);
                profile.Filters.Add(new _Filter(meta, itemID));
            }
        }
        void RefinerySetup(_Refinery refinery)
        {
            string[] data = refinery.TermBlock.CustomData.Split('\n');

            foreach (string nextline in data)                                                               // Iterate each line
            {
                if (nextline.Length == 0)                                                                   // Line must contain information
                    continue;

                /// OPTION CHANGE ///

                if (nextline[0] == '&')
                {
                    if (Contains(nextline, "auto"))
                    {
                        if (nextline.Contains("-"))
                            refinery.AutoRefine = false;

                        else
                            refinery.AutoRefine = true;
                    }
                }
            }

            refinery.RefineBlock.UseConveyorSystem = refinery.AutoRefine;

            InventorySetup(refinery);
        }
        void InventorySetup(_Inventory inventory)
        {
            inventory.FilterProfile.Filters.Clear();
            inventory.FilterProfile.DEFAULT_IN = true;
            inventory.FilterProfile.DEFAULT_OUT = true;

            string[] data = inventory.TermBlock.CustomData.Split('\n');                                     // Break customData into lines

            foreach (string nextline in data)                                                               // Iterate each line
            {
                if (nextline.Length == 0)                                                                   // Line must contain information
                    continue;

                /// OPTION CHANGE ///

                if (nextline[0] == '&')
                {
                    if (Contains(nextline, "empty"))
                    {
                        if (nextline.Contains("-"))
                            inventory.FilterProfile.EMPTY = false;
                        else
                            inventory.FilterProfile.EMPTY = true;
                    }
                    if (Contains(nextline, "fill"))
                    {
                        if (nextline.Contains("-"))
                            inventory.FilterProfile.FILL = false;
                        else
                            inventory.FilterProfile.FILL = true;
                    }
                    continue;
                }

                /// FILTER CHANGE ///

                FilterSetup(nextline, inventory.FilterProfile);
            }

            inventory.FilterProfile.Filters.RemoveAll(x => x.ItemType == "any" && x.ItemSubType == "any");  // Redundant. Refer to inventory default mode
        }

        void TallyUpdates(_Inventory inventory)
        {
            Counter.Reset();

            for (int i = ProdSearchIndex; i < (ProdSearchIndex + ProdSearchCap) && i < Productions.Count(); i++)
            {
                Productions[i].TallyUpdate(inventory, ref Counter);
            }
        }
        void SortUpdate(_Inventory inventory)
        {
            if (inventory.FilterProfile.EMPTY ||
                CheckProducerInputClog(inventory)) // Assembler anti-clog
                InventoryEmpty(inventory);

            if (inventory.FilterProfile.FILL)
                InventoryFill(inventory);

            //RotateInventory(inventory);
        }
        void InventoryEmpty(_Inventory inventory)
        {
            IMyInventory sourceInventory = PullInventory(inventory, false);
            List<MyInventoryItem> sourceItems = new List<MyInventoryItem>();
            sourceInventory.GetItems(sourceItems);
            MyFixedPoint target;

            /*
            Items moved
            Items searched
            Inv searched
             */

            Counter.Reset();

            foreach (MyInventoryItem nextItem in sourceItems)
            {
                if (Counter.Check(Count.MOVED)) // Total Items moved
                    break;

                if (Counter.Increment(Count.ITEMS)) // Total Items searched
                    break;

                if (ProfileCompare(inventory.FilterProfile, nextItem, out target, false))
                    continue;

                for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < Inventories.Count(); i++)
                {
                    if (Counter.Check(Count.MOVED)) // Total Items moved
                        break;
                    EmptyToCandidate(inventory, Inventories[i], nextItem);
                }
            }
        }
        void InventoryFill(_Inventory inventory)
        {
            if (inventory is _Refinery &&
                ((_Refinery)inventory).AutoRefine)
                return; // using vanilla system

            if (inventory.FilterProfile.Filters.Count() <= 0)
                return; // No Filters to pull


            /*
            Items moved
            Inv searched
            Items searched
             */

            Counter.Reset();

            if (inventory.CallBackIndex != -1 &&
                !FillFromCandidate(inventory, Inventories[inventory.CallBackIndex]))
                inventory.CallBackIndex = -1;

            for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < Inventories.Count(); i++)
            {
                if (Counter.Check(Count.MOVED))
                    return;

                if (FillFromCandidate(inventory, Inventories[i]))
                    break;
            }
        }
        void InventoryRotate(_Inventory inventory)
        {
            IMyInventory source = PullInventory(inventory);
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            source.GetItems(items);
            int count = items.Count();
            if (count < ItemSearchCap)
                return;

            for (int i = 0; i < ItemSearchCap && i < count; i++)
                source.TransferItemTo(source, 0, count);
        }

        bool CheckProducerInputClog(_Inventory inventory)
        {
            if (!(inventory is _Producer))
                return false;

            _Producer producer = (_Producer)inventory;

            return (float)producer.ProdBlock.InputInventory.CurrentVolume / (float)producer.ProdBlock.InputInventory.MaxVolume > CleanPercent;
        }
        bool CheckInventoryLink(_Inventory outbound, _Inventory inbound)
        {
            if (outbound == inbound)
                return false;

            if (!PullInventory(outbound, false).IsConnectedTo(PullInventory(inbound)))
                return false;

            if (PullInventory(inbound).IsFull)
                return false;

            return true;
        }
        void EmptyToCandidate(_Inventory source, _Inventory target, MyInventoryItem currentItem)
        {
            if (!CheckInventoryLink(source, target))
                return;

            IMyInventory targetInventory = PullInventory(target);
            IMyInventory sourceInventory = PullInventory(source, false);

            MyFixedPoint value;
            if (!ProfileCompare(source.FilterProfile, currentItem, out value))
                return;

            if (value != 0)
            {
                MyItemType itemType = currentItem.Type;
                MyFixedPoint sourceCurrentAmount = 0;

                MyInventoryItem? sourceCheck = sourceInventory.FindItem(itemType);
                if (sourceCheck != null)
                {
                    MyInventoryItem sourceItem = (MyInventoryItem)sourceCheck;
                    sourceCurrentAmount = sourceItem.Amount;
                }

                if (value > sourceCurrentAmount)
                {
                    sourceInventory.TransferItemTo(targetInventory, currentItem, value - sourceCurrentAmount);
                    Counter.Increment(Count.MOVED);
                }
            }
            else
            {
                sourceInventory.TransferItemTo(targetInventory, currentItem);
                Counter.Increment(Count.MOVED);
            }
        }
        bool FillFromCandidate(_Inventory source, _Inventory target)
        {
            if (!CheckInventoryLink(target, source))
                return false;

            IMyInventory sourceInventory = PullInventory(source);
            IMyInventory targetInventory = PullInventory(target, false);

            // Populate items
            List<MyInventoryItem> targetItems = new List<MyInventoryItem>();
            targetInventory.GetItems(targetItems);
            MyFixedPoint targetAmount = 0;
            bool callback = false;

            foreach (MyInventoryItem nextItem in targetItems)
            {
                if (sourceInventory.IsFull)
                    break;

                if (Counter.Increment(Count.ITEMS))
                    break;

                MyFixedPoint value = 0;

                if (!(source is _Refinery) &&                                           // Refineries get override privledges
                    !ProfileCompare(target.FilterProfile, nextItem, out value, false))   // Check aloud to leave
                    continue;

                if (!ProfileCompare(source.FilterProfile, nextItem, out value))          // Check if it fits the pull request
                    continue;


                if (value != 0)
                {
                    MyItemType itemType = nextItem.Type;
                    MyFixedPoint sourceCurrentAmount = 0;

                    MyInventoryItem? sourceCheck = sourceInventory.FindItem(itemType);
                    if (sourceCheck != null)
                    {
                        MyInventoryItem sourceItem = (MyInventoryItem)sourceCheck;
                        sourceCurrentAmount = sourceItem.Amount;
                    }

                    if (value > sourceCurrentAmount)
                    {
                        targetInventory.TransferItemTo(sourceInventory, nextItem, value - sourceCurrentAmount);
                        source.CallBackIndex = Inventories.FindIndex(x => x == target);
                        callback = true;
                        if (Counter.Increment(Count.MOVED))
                            break;
                    }
                }

                else
                {
                    targetInventory.TransferItemTo(sourceInventory, nextItem);
                    source.CallBackIndex = Inventories.FindIndex(x => x == target);
                    callback = true;
                    if (Counter.Increment(Count.MOVED))
                        break;
                }
            }

            Counter.Reset(Count.ITEMS);
            return callback;
        }

        /// Production
        string GenerateRecipes()
        {
            Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> recipeList = new Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId>();

            List<MyProductionItem> nextList = new List<MyProductionItem>();
            string finalList = string.Empty;

            foreach (_Producer producer in Producers)
            {
                producer.ProdBlock.GetQueue(nextList);

                foreach (MyProductionItem item in nextList)
                    recipeList[item] = producer.ProdBlock.BlockDefinition;
            }

            foreach (KeyValuePair<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> pair in recipeList)
                finalList += pair.Key.BlueprintId + ":" + pair.Value.SubtypeId.ToString() + ":" + pair.Key.Amount + "\n";

            return finalList;
        }
        void LoadRecipes()
        {
            Me.CustomData = Storage;
        }
        void ClearQue(int level)
        {
            switch (level)
            {
                case 0:
                    foreach (IMyProductionBlock block in ProdBlocks)
                        block.ClearQueue();
                    break;

                case 1:
                    foreach (_Producer producer in Producers)
                    {
                        Echo("Clearing...");
                        if (producer.ProdBlock != null)
                            producer.ProdBlock.ClearQueue();
                        Echo("Cleared!");
                    }

                    break;
            }
        }
        void ProductionSetup()
        {
            Productions.Clear();

            string[] dataTriples = Me.CustomData.Split('\n');

            foreach (string line in dataTriples)
            {
                string[] set = line.Split(':');

                if (set.Length != 3)
                    continue;

                MyDefinitionId nextId;
                try
                { nextId = MyDefinitionId.Parse(set[0]); }
                catch
                { continue; }

                string prodId = set[1];

                int target;
                try
                { target = Int32.Parse(set[2]); }
                catch
                { target = 0; }

                _Production nextProd = new _Production(nextId, prodId, target);
                Productions.Add(nextProd);
            }
        }
        void ProductionUpdate(_Production prod)
        {
            if (prod.Current >= prod.Filter.Meta.Target)
            {
                // Add excess que removal logic here later
                return;
            }

            List<_Producer> candidates = Producers.FindAll(x => x.ProdBlock.BlockDefinition.SubtypeId.ToString() == prod.ProdIdString);
            List<MyProductionItem> existingQues = new List<MyProductionItem>();

            Debug.Append($"ProdUpdate: {prod.Def}\n");

            foreach (_Producer producer in candidates)
            {
                if (!CheckBlockExists(producer))
                    continue;

                List<MyProductionItem> nextList = new List<MyProductionItem>();
                producer.ProdBlock.GetQueue(nextList);

                for (int i = nextList.Count - 1; i > -1; i--)
                    if (nextList[i].Amount < 1)
                    {
                        producer.ProdBlock.RemoveQueueItem(i, 1f);
                        nextList.RemoveAt(i);
                    }

                existingQues.AddRange(nextList.FindAll(x => FilterCompare(Debug, prod.Filter, x)));
            }

            Debug.Append($"existingQueCount: {existingQues.Count}\n");

            MyFixedPoint existingQueAmount = 0;
            foreach (MyProductionItem item in existingQues)
                existingQueAmount += item.Amount;

            MyFixedPoint qeueTotal = prod.Filter.Meta.Target - (prod.Current + existingQueAmount);

            Debug.Append(
                $"Current: {prod.Current}\n" +
                $"InQue: {existingQueAmount}\n" +
                $"Target: {prod.Filter.Meta.Target}\n" +
                $"Final: {qeueTotal}\n");

            if (qeueTotal <= 0)
                return;

            MyFixedPoint qeueIndividual = (qeueTotal * ((float)1 / candidates.Count));  // Divide into equal portions
            qeueIndividual = (qeueIndividual < 1) ? 1 : qeueIndividual;                 // Always make atleast 1
            qeueIndividual = (int)qeueIndividual;                                       // Removal decimal place

            foreach (_Producer producer in candidates)                                  // Distribute
                producer.ProdBlock.AddQueueItem(prod.Def, qeueIndividual);
        }

        /// Power
        bool PowerSetup(_Resource candidateMonitor)
        {
            foreach (_Block block in PowerConsumers)
                block.Priority = -1;

            if (candidateMonitor.Type != _ResType.BATTERY)
                return false;

            string[] data = candidateMonitor.TermBlock.CustomData.Split('\n');
            int index = 0;

            bool[] checks = new bool[4]; // rough inset

            foreach (string nextline in data)
            {
                if (Contains(nextline, "prod") && !checks[0])
                {
                    index = PowerPrioritySet(Producers.Cast<_Block>().ToList(), index);
                    checks[0] = true;
                }

                if (Contains(nextline, "ref") && !checks[1])
                {
                    index = PowerPrioritySet(Refineries.Cast<_Block>().ToList(), index);
                    checks[1] = true;
                }

                if (Contains(nextline, "farm") && !checks[2])
                {
                    List<_Resource> farms = new List<_Resource>();
                    farms.AddRange(Resources.FindAll(x => x.Type == _ResType.OXYFARM));
                    index = PowerPrioritySet(farms.Cast<_Block>().ToList(), index);
                    checks[2] = true;
                }

                if (Contains(nextline, "gen") && !checks[3])
                {
                    List<_Resource> gens = new List<_Resource>();
                    gens.AddRange(Resources.FindAll(x => x.Type == _ResType.GASGEN));
                    index = PowerPrioritySet(gens.Cast<_Block>().ToList(), index);
                    checks[3] = true;
                }
            }

            if (index == 0)
                return false;

            PowerPrioritySet(PowerConsumers, index);

            PowerConsumers = PowerConsumers.OrderBy(x => x.Priority).ToList(); // << Maybe?

            PowerMonitor = candidateMonitor;

            return true;
        }
        void PowerUpdate()
        {
            if (PowerMonitor == null)
                return;

            IMyBatteryBlock battery = (IMyBatteryBlock)PowerMonitor.TermBlock;

            if (!bPowerCharged &&
                battery.CurrentStoredPower / battery.MaxStoredPower >= (1 - PowerThreshold))
                PowerAdjust(true);

            if (battery.CurrentStoredPower / battery.MaxStoredPower <= PowerThreshold)
                PowerAdjust(false);
        }
        int PowerPrioritySet(List<_Block> blocks, int start)
        {
            int index = 0;

            foreach (_Block block in blocks)
            {
                if (block.Priority == -1)
                {
                    block.Priority = index + start;
                    index++;
                }
            }

            return index + start;
        }
        void PowerAdjust(bool bAdjustUp = true)
        {
            if (bAdjustUp)
                foreach (_Block block in PowerConsumers)
                    ((IMyFunctionalBlock)block.TermBlock).Enabled = true;

            else
                for (int i = PowerConsumers.Count - 1; i > -1; i--)
                {
                    if (((IMyFunctionalBlock)PowerConsumers[i].TermBlock).Enabled)
                    {
                        ((IMyFunctionalBlock)PowerConsumers[i].TermBlock).Enabled = false;
                        break;
                    }

                }
        }

        /// Main
        void RunArguments(string argument)
        {
            switch (argument)
            {
                case "DETECT":
                    BlockDetection();
                    ClearClassLists();
                    bSetupComplete = false;
                    bUpdated = false;
                    break;

                case "BUILD":
                    ClearClassLists();
                    bSetupComplete = false;
                    bUpdated = false;
                    break;

                case "UPDATE":
                    bUpdated = false;
                    break;

                case "RETAG":
                    ReTagBlocks();
                    break;

                case "REFILTER":
                    ReFilterBlocks();
                    break;

                case "CLEARTAGS":
                    ClearAllBlockTags();
                    break;

                case "GENERATE":
                    Me.CustomData = GenerateRecipes();
                    break;

                case "CLEARQUE":
                    ClearQue(1);
                    break;

                case "SAVE":
                    Save();
                    break;

                case "LOAD":
                    LoadRecipes();
                    break;

                case "INV":
                    bSortRunning = !bSortRunning;
                    break;

                case "TALLY":
                    bTallyRunning = !bTallyRunning;
                    break;

                case "DIS":
                    bDisplayRunning = !bDisplayRunning;
                    break;

                case "PROD":
                    bProdRunning = !bProdRunning;
                    break;

                case "POWER":
                    bPowerRunning = !bPowerRunning;
                    break;
            }
        }
        string ProgEcho()
        {
            string echoOutput = string.Empty;
            int count = -1;
            if (Productions != null &&
                ProdQueIndex > -1 &&
                ProdQueIndex < Productions.Count)
                count = Productions[ProdQueIndex].Tallies.Count;

            echoOutput += $"{EchoLoop[EchoCount]} Torqk's Grid Manager {EchoLoop[EchoCount]}" +
                            "\n====================" +
                            $"\nTallyCount: {count}" +
                            $"\nClock Indices: {InventoryClock} : {ProdClock} : {DisplayClock}" +
                            $"\nOperation Indices: {InvQueIndex} : {ProdQueIndex} : {DisplayQueIndex}" +
                            $"\nSorting   : {(bSortRunning ? "Online" : "Offline")}" +
                            $"\nTally : {(bTallyRunning ? "Online" : "Offline")}" +
                            $"\nProduction : {(bProdRunning ? "Online" : "Offline")}" +
                            $"\nDisplay      : {(bDisplayRunning ? "Online" : "Offline")}" +
                            "\n====================" +
                            $"\nSearchIndex: {InvSearchIndex}" +
                            $"\nProdSearchIndex: {ProdSearchIndex}";

            echoOutput += (bSetupComplete) ? $"\nInvCount: {Inventories.Count}" : $"\nSetupIndex: {SetupQueIndex}";

            EchoCount++;

            if (EchoCount >= EchoMax)
                EchoCount = 0;

            return echoOutput;
        }

        bool CheckBlockExists(_Block block)
        {
            IMyTerminalBlock maybeMe = GridTerminalSystem.GetBlockWithId(block.BlockID);
            if (maybeMe != null &&                          // Exists?
                maybeMe.CustomName.Contains(Signature))    // Signature?
                return true;

            if (!block.bDetatchable)
            {
                if (block is _Inventory)
                    Inventories.RemoveAt(Inventories.FindIndex(x => x == (_Inventory)block));

                if (block is _Display)
                    Displays.RemoveAt(Displays.FindIndex(x => x == (_Display)block));

                if (block is _Resource)
                    Resources.RemoveAt(Resources.FindIndex(x => x == (_Resource)block));

                Blocks.RemoveAt(Blocks.FindIndex(x => x == block));
            }

            return false;
        }
        bool CheckCandidate(IMyTerminalBlock block)
        {
            if (block == null)
                return false;
            return (Blocks.FindIndex(x => x.BlockID == block.EntityId) < 0 && block.CustomName.Contains(Signature));
        }
        bool PullSignedTerminalBlocks(List<IMyTerminalBlock> blocks)
        {
            blocks.Clear();
            List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
            GridTerminalSystem.GetBlockGroups(groups);
            IMyBlockGroup group = groups.Find(x => Contains(x.Name, Signature));
            if (group == null)
                return false;

            group.GetBlocks(blocks);
            return true;
        }

        void ReTagBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullSignedTerminalBlocks(blocks))
                return;

            foreach (IMyTerminalBlock block in blocks)
                if (!block.CustomName.Contains(Signature))
                    block.CustomName += Signature;
        }
        void ReFilterBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullSignedTerminalBlocks(blocks))
                return;

            string masterCustomData = string.Empty;

            foreach (IMyTerminalBlock block in blocks)
                if (block.CustomData.Contains(CustomSig))
                {
                    masterCustomData = block.CustomData.Replace(CustomSig, "");
                    break;
                }

            foreach (IMyTerminalBlock block in blocks)
                block.CustomData = masterCustomData;
        }
        void ClearAllBlockTags()
        {
            List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(allBlocks);

            foreach (IMyTerminalBlock block in allBlocks)
                if (block.CustomName.Contains(Signature))
                    block.CustomName = block.CustomName.Replace(Signature, string.Empty);
        }

        bool UpdateSettings()
        {
            PowerMonitor = null;

            foreach (_Resource resource in Resources)
            {
                if (PowerSetup(resource))
                {
                    bPowerSetupComplete = true;
                    break;
                }
            }

            if (!bPowerSetupComplete)
                PowerPrioritySet(Blocks, 0);

            foreach (_Cargo nextCargo in Cargos)
                InventorySetup(nextCargo);

            foreach (_Refinery nextRefine in Refineries)
                RefinerySetup(nextRefine);

            foreach (_Display display in Displays)
                DisplaySetup(display);

            ProductionSetup();

            return true;
        }
        bool BlockListSetup()
        {
            bool setup = true;

            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < BatteryBlocks.Count(); i++) // Batteries first
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(BatteryBlocks[i]))
                {
                    _Resource newRes = new _Resource(BatteryBlocks[i], Signature, _ResType.BATTERY);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < CargoBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(CargoBlocks[i]))
                {
                    _Cargo newCargo = new _Cargo(CargoBlocks[i], new _FilterProfile(), Signature);
                    Blocks.Add(newCargo);
                    Inventories.Add(newCargo);
                    Cargos.Add(newCargo);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < ProdBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(ProdBlocks[i]))
                {
                    _Producer newProd = new _Producer(ProdBlocks[i], new _FilterProfile(true, true, false, true), Signature);
                    Blocks.Add(newProd);
                    PowerConsumers.Add(newProd);
                    Inventories.Add(newProd);
                    Producers.Add(newProd);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < RefineryBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(RefineryBlocks[i]))
                {
                    _Refinery newRefine = new _Refinery(RefineryBlocks[i], new _FilterProfile(false, true, true, true), Signature);
                    Blocks.Add(newRefine);
                    PowerConsumers.Add(newRefine);
                    Inventories.Add(newRefine);
                    Refineries.Add(newRefine);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < PanelBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PanelBlocks[i]))
                {
                    _Display newPanel = new _Display(PanelBlocks[i], Signature, DefFontSize);
                    Blocks.Add(newPanel);
                    Displays.Add(newPanel);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < PowerBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PowerBlocks[i]))
                {
                    _Resource newRes = new _Resource(PowerBlocks[i], Signature, _ResType.POWERGEN);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < TankBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(TankBlocks[i]))
                {
                    _Resource newRes = new _Resource(TankBlocks[i], Signature, _ResType.GASTANK);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < GeneratorBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(GeneratorBlocks[i]))
                {
                    _Resource newRes = new _Resource(GeneratorBlocks[i], Signature, _ResType.GASGEN);
                    Blocks.Add(newRes);
                    PowerConsumers.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < OxyFarmBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(OxyFarmBlocks[i]))
                {
                    _Resource newRes = new _Resource(OxyFarmBlocks[i], Signature, _ResType.OXYFARM);
                    Blocks.Add(newRes);
                    PowerConsumers.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            SetupQueIndex += SetupCap;
            return setup;
        }
        void BlockListUpdate()
        {
            // Persistent Updates
            if (bPowerRunning)
                PowerUpdate();

            if (bDisplayRunning)
                foreach (_Display display in Displays)
                    DisplayRefresh(display);

            // Inventory Updates
            if (InventoryClock == ClockCycle &&
                Inventories.Count > 0)
            {
                if (CheckBlockExists(Inventories[InvQueIndex]))
                {
                    if (InvSearchIndex == 0)
                        InventoryRotate(Inventories[InvQueIndex]);

                    if (bSortRunning)
                        SortUpdate(Inventories[InvQueIndex]);

                    if (bTallyRunning)
                        TallyUpdates(Inventories[InvQueIndex]);
                }

                InvSearchIndex += InvSearchCap;
                if (InvSearchIndex >= Inventories.Count)
                {
                    bTallyCycleComplete = true;
                    InvSearchIndex = 0;
                    InvQueIndex++;
                    InvQueIndex = (InvQueIndex >= Inventories.Count) ? 0 : InvQueIndex;
                }
            }

            // Production Updates
            if (bTallyCycleComplete &&
                bProdRunning &&
                ProdClock == ClockCycle &&
                Productions.Count > 0)
            {
                ProductionUpdate(Productions[ProdQueIndex]);

                ProdQueIndex++;
                ProdQueIndex = (ProdQueIndex >= Productions.Count) ? 0 : ProdQueIndex;
            }

            // Display Updates
            if (bDisplayRunning &&
                DisplayClock == ClockCycle &&
                Displays.Count > 0)
            {
                DisplayUpdate(Displays[DisplayQueIndex]);

                DisplayQueIndex++;
                DisplayQueIndex = (DisplayQueIndex >= Displays.Count) ? 0 : DisplayQueIndex;
            }

            // Clock Updates
            DisplayClock = (DisplayClock == ClockCycle) ? 0 : (DisplayClock + 1);
            InventoryClock = (InventoryClock == ClockCycle) ? 0 : (InventoryClock + 1);
            ProdClock = (ProdClock == ClockCycle) ? 0 : (ProdClock + 1);
        }

        public Program()
        {
            mySurface = Me.GetSurface(0);
            mySurface.ContentType = ContentType.TEXT_AND_IMAGE;
            mySurface.WriteText("", false);

            LoadRecipes();
            BlockDetection();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            string output = ProgEcho();
            //Echo(output);
            mySurface.WriteText(output);

            RunArguments(argument);

            if (FAIL)
                return;

            if (!bSetupComplete)
            {
                bSetupComplete = BlockListSetup();
                return;
            }

            if (!bUpdated)
            {
                bUpdated = UpdateSettings();
                return;
            }

            if (bSetupComplete && bUpdated)
            {
                try
                {
                    BlockListUpdate();
                }
                catch
                {
                    //Debug.Append("FAIL-POINT!\n");
                    FAIL = true;
                }
            }

            //mySurface.WriteText(Debug);
            Debug.Clear();
        }
        public void Save()
        {
            Storage = Me.CustomData;
        }
        #endregion
    }
}

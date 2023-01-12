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
        #region MENTIONS

        /*KILL'TRILL'N'SPILL
         * Noodle
         * DeathFather
         * Subbob
         * Cmd_bobcat
         * MalWare
         */

        #endregion

        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";

        const float DefFontSize = .5f;
        const int DefSigCount = 2;
        readonly int[] ScreenRatio = { 25, 17 };
        const int InvSearchCap = 10;
        const int ItemSearchCap = 10;
        const int ItemMoveCap = 3;
        const int SetupCap = 20;
        const float CleanPercent = .8f;
        const float PowerThreshold = .2f;

        /// WARNING!! DO NOT GO FURTHER USER!! ///

        /// LOGIC
        IMyTextSurface debug;

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
        const int ClockCycle = 5;

        int SetupIndex = 0;
        int InvSearchIndex = 0;
        int InvQueIndex = 0;
        int ProdQueIndex = 0;
        //int CleanQueIndex = 0;
        int DisplayQueIndex = 0;

        int InvClock = 0;
        int ProdClock = 1;
        //int CleanClock = 2;
        int DisplayClock = 3;
        int RotationClock = 4;

        bool bPowerSetupComplete = false;
        bool bSetupComplete = false;
        bool bUpdated = false;
        bool bRotate = false;
        bool bInventoryRunning = true;
        bool bProductionRunning = true;
        bool bDisplayRunning = true;
        bool bPowerRunning = true;
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
        List<_Inventory> Rotations = new List<_Inventory>();

        List<_Production> Productions = new List<_Production>();
        List<_Block> PowerConsumers = new List<_Block>();
        _Resource PowerMonitor;

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

                change = OldAmount - CurrentAmount;
                return success;
            }
        }

        public class _Block
        {
            public long BlockID;
            public int Priority;
            public string CustomName;

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
            public List<string> rawOutput;
            public List<string> fOutput;

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
                rawOutput = new List<string>();
                fOutput = new List<string>();
                OutputIndex = 0;
                Scroll = 1;
                Delay = 10;
                Timer = 0;

                Module = new _DisplayModule();
            }
        }
        public class _Filter
        {
            public string ItemType;
            public string ItemSubType;

            public MyFixedPoint Target;

            public bool IN_BOUND;
            public bool OUT_BOUND;
            public _Filter(MyFixedPoint target, string itemType = "any:any", bool inBound = true, bool outBound = true)
            {
                string[] types = itemType.Split(':');

                ItemType = (types[0] == "" || types[0] == "any") ? "any" : types[0];

                try
                { ItemSubType = (types[1] == "" || types[1] == "any") ? "any" : types[1]; }
                catch
                { ItemSubType = "any"; }

                Target = target; // 0 == none
                                 //Priority = 0;
                IN_BOUND = inBound;
                OUT_BOUND = outBound;
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
            public string ProdIdString;
            public MyFixedPoint Target;
            public MyFixedPoint Current;
            public List<Tally> Totals;

            public _Production(MyDefinitionId def, string prodIdString, int target = 0)
            {
                Def = def;
                ProdIdString = prodIdString;
                Target = target;
                Totals = new List<Tally>();
            }

            public void TallyUpdate(_Inventory sourceInventory, MyInventoryItem tallyItem)
            {

                if (!ItemDefinitionCompare(Def, tallyItem))
                    return;

                Tally sourceTally = Totals.Find(x => x.Inventory == sourceInventory);
                if (sourceTally == null)
                {
                    sourceTally = new Tally(sourceInventory, tallyItem);
                    Totals.Add(sourceTally);
                }

                MyFixedPoint change = 0;
                if (!sourceTally.Refresh(out change))
                {
                    Totals.Remove(sourceTally);
                }

                Current += change;

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
        static bool ContainsCIS(string source, string target)// Case In-Sensitive
        {
            if (source.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
        static string[] ItemTypeCrop(MyInventoryItem targetItem)
        {
            string[] output = new string[2];

            output[0] = targetItem.Type.TypeId.Split('_')[1];
            output[1] = targetItem.Type.SubtypeId;

            if (output[0].Contains("Comp") && targetItem.Type.SubtypeId != "ZoneChip")
                output[0] = "Comp";
            else if (output[0].Contains("Ammo"))
                output[0] = "Ammo";
            else if (output[0].Contains("Gun"))
                output[0] = "Hand";
            else if (output[0].Contains("Container"))
                output[0] = "Bottle";
            else if (output[0] != "Ore" && output[0] != "Ingot")
                output[0] = "Other";

            if (output[0].Contains("Ingot") && output[1].Contains("Stone"))// Gravel Corner-case
                output[1] = "Gravel";

            return output;
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
        static bool FilterCompare(_FilterProfile profile, MyInventoryItem item, out MyFixedPoint target, bool dirIn = true)
        {
            string[] croppedItem = ItemTypeCrop(item);
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
                if (filter.ItemType != "any" && !ContainsCIS(croppedItem[0], filter.ItemType))
                    continue;

                if (filter.ItemSubType != "any" && !ContainsCIS(croppedItem[1], filter.ItemSubType))
                    continue;

                match = true;

                if (!filter.IN_BOUND &&
                    dirIn)
                    return false;

                if (!filter.OUT_BOUND &&
                    !dirIn)
                    return false;

                target = (filter.Target == 0) ? target : filter.Target;
            }

            return dirIn ? match : true;
        }
        static string ProductionIdCrop(string blueprintId)
        {
            return blueprintId.Split('/')[1];
        }
        static bool ItemDefinitionCompare(MyDefinitionId def, MyInventoryItem item)
        {
            string compare = def.ToString().Split('/')[1];
            string itemDef = item.Type.ToString().Split('/')[1];

            /////////////////////////////////////////////////
            // Mod specific hack, requires better autonomy //
            if (compare[0] == 'P' && compare[1] == 'O')
                compare = compare.Remove(0, 2);
            compare = compare.Replace("Component", "");
            compare = compare.Replace("PolymerTo", "");
            /////////////////////////////////////////////////

            return compare == itemDef;

            //targets = allItems.FindAll(x => compare == x.Type.ToString().Split('/')[1]);
        }

        int MonoSpaceChars(IMyTextPanel panel)
        {
            return (int)(ScreenRatio[0] / panel.FontSize);
        }
        int MonoSpaceLines(IMyTextPanel panel)
        {
            return (int)(ScreenRatio[1] / panel.FontSize);
        }
        string NotationBundler2(float value, int sigCount)
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
        string SimpleBundler(float value, int sigCount)
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
        string ParseItemTotal(KeyValuePair<string, MyFixedPoint> item, _DisplayModule module)
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
                    nextOut += NotationBundler2((float)item.Value, module.SigCount);
                    break;

                case _Notation.SIMPLIFIED:
                    nextOut += SimpleBundler((float)item.Value, module.SigCount);
                    break;
            }

            return nextOut;
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
            Rotations.Clear();

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
                        if (ContainsCIS(nextline, "stat"))
                            display.Module.Mode = _ScreenMode.STATUS;
                        if (ContainsCIS(nextline, "inv"))
                            display.Module.Mode = _ScreenMode.INVENTORY;
                        if (ContainsCIS(nextline, "prod"))
                            display.Module.Mode = _ScreenMode.PRODUCTION;
                        if (ContainsCIS(nextline, "res"))
                            display.Module.Mode = _ScreenMode.RESOURCE;
                        if (ContainsCIS(nextline, "tally"))
                            display.Module.Mode = _ScreenMode.TALLY;
                        break;

                    case '@': // Target
                        if (ContainsCIS(nextline, "block"))
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
                        if (ContainsCIS(nextline, "group"))
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
                        if (ContainsCIS(nextline, "scroll"))
                        {
                            int newDelay = Convert.ToInt32(lineblocks[1]);
                            if (newDelay > 0)
                                display.Delay = newDelay;
                            else
                                display.Delay = 10;
                        }
                        break;

                    case '#':   // Notation
                        if (ContainsCIS(nextline, "def"))
                            display.Module.Notation = _Notation.DEFAULT;
                        if (ContainsCIS(nextline, "simp"))
                            display.Module.Notation = _Notation.SIMPLIFIED;
                        if (ContainsCIS(nextline, "sci"))
                            display.Module.Notation = _Notation.SCIENTIFIC;
                        if (ContainsCIS(nextline, "%"))
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
            int lines = MonoSpaceLines(display.Panel) - Offset;

            bool tick = false;
            display.Timer++;
            if (display.Timer >= display.Delay)
            {
                display.Timer = 0;
                tick = true;
            }

            if (display.fOutput.Count > MonoSpaceLines(display.Panel)) // Requires Scrolling
            {
                List<string> formattedSection = new List<string>();
                if (display.OutputIndex < Offset || display.OutputIndex > (display.fOutput.Count - lines)) // Index Reset Failsafe
                    display.OutputIndex = Offset;

                for (int i = 0; i < Offset; i++)
                    formattedSection.Add(display.fOutput[i]);

                for (int i = display.OutputIndex; i < (display.OutputIndex + lines); i++)
                    formattedSection.Add(display.fOutput[i]);

                if (display.OutputIndex == display.fOutput.Count - lines)
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
                foreach (string nextString in display.fOutput)
                    display.Panel.WriteText(nextString + "\n", true);
            }
        }
        void RawStringBuilder(_Display display)
        {
            display.rawOutput.Clear();

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    display.rawOutput.Add("=" + Seperator + "[Default]");
                    break;

                case _ScreenMode.INVENTORY:
                    display.rawOutput.Add("=" + Seperator + "[Inventory]");
                    break;

                case _ScreenMode.RESOURCE:
                    display.rawOutput.Add("=" + Seperator + "[Resource]");
                    break;

                case _ScreenMode.STATUS:
                    display.rawOutput.Add("=" + Seperator + "[Status]");
                    break;

                case _ScreenMode.PRODUCTION:
                    display.rawOutput.Add("=" + Seperator + "[Production]");
                    display.rawOutput.AddRange(ProductionBuilder());
                    break;

                case _ScreenMode.TALLY:
                    display.rawOutput.Add("=" + Seperator + "[Tally]");
                    display.rawOutput.AddRange(ItemTallyBuilder(display.Module));
                    break;
            }

            display.rawOutput.Add("#");

            if (display.Module.Mode > (_ScreenMode)3)
                return; // No Target for tally systems

            switch (display.Module.TargetType)
            {
                case _Target.DEFAULT:
                    display.rawOutput.Add("=" + "/" + display.Module.TargetName);
                    break;

                case _Target.BLOCK:
                    if (display.Module.Mode == _ScreenMode.RESOURCE || display.Module.Mode == _ScreenMode.STATUS)
                        display.rawOutput.Add(TableHeaderBuilder(display));
                    display.rawOutput.AddRange(RawBlockBuilder(display, display.Module.TargetBlock));
                    break;

                case _Target.GROUP:
                    display.rawOutput.AddRange(RawGroupBuilder(display, display.Module.TargetGroup));
                    break;
            }

        }
        void FormattedStringBuilder(_Display display)
        {
            display.fOutput.Clear();

            int chars = MonoSpaceChars(display.Panel);

            foreach (string nextString in display.rawOutput) // NEEDS UPDATEING!!
            {
                string[] blocks = nextString.Split(Split);
                string newLine = string.Empty;
                int remains = 0;

                switch (blocks[0])
                {
                    case "#": // Empty Line
                        display.fOutput.Add(newLine);
                        break;

                    case "!": // Warning
                        display.fOutput.Add(blocks[1]);
                        break;

                    case "^": // Table-Header
                        if (blocks.Length == 3)
                        {
                            remains = chars - blocks[2].Length;
                            if (remains > 0)
                            {
                                if (remains < blocks[1].Length)
                                    newLine += blocks[1].Substring(0, remains);
                                else
                                    newLine += blocks[1];
                            }
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                newLine += "-";
                            newLine += blocks[2];
                        }
                        else
                        {
                            remains = chars - blocks[1].Length;
                            newLine += "[" + blocks[1] + "]";
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                newLine += "-";
                        }
                        display.fOutput.Add(newLine);
                        break;

                    case "=": // Header
                        if (chars <= blocks[1].Length) // Can header fit side dressings?
                        {
                            newLine = blocks[1];
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
                                newLine += "=";
                            newLine += blocks[1];
                            for (int i = 0; i < remains / 2; i++)
                                newLine += "=";
                        }
                        display.fOutput.Add(newLine);
                        break;

                    case "$": // Inventory
                        if (chars < (blocks[1].Length + blocks[2].Length)) // Can Listing fit on one line?
                        {
                            display.fOutput.Add(blocks[1]);
                            display.fOutput.Add(blocks[2]);
                        }
                        else
                        {
                            newLine += blocks[1];
                            for (int i = 0; i < (chars - (blocks[1].Length + blocks[2].Length)); i++)
                                newLine += "-";
                            newLine += blocks[2];
                            display.fOutput.Add(newLine);
                        }
                        break;

                    case "%": // Resource
                        if (!blocks[2].Contains("%"))
                            blocks[2] += "|" + blocks[3];
                        if (chars < (blocks[1].Length + blocks[2].Length)) // Can Listing fit on one line?
                        {
                            display.fOutput.Add(blocks[1]);
                            display.fOutput.Add(blocks[2]);
                        }
                        else
                        {
                            newLine += blocks[1];
                            for (int i = 0; i < (chars - (blocks[1].Length + blocks[2].Length)); i++)
                                newLine += "-";
                            newLine += blocks[2];
                            display.fOutput.Add(newLine);
                        }
                        break;

                    case "*": // Status
                              // remaining chars = total line chars - (colored blocks + 2 to correct for colored blocks spacing)
                        remains = chars - (blocks[2].Length + 2);
                        if (remains > 0)
                        {
                            if (remains < blocks[1].Length)
                                newLine += blocks[1].Substring(0, remains);
                            else
                                newLine += blocks[1];
                        }
                        for (int i = 0; i < (remains - blocks[1].Length); i++)
                            newLine += "-";
                        newLine += blocks[2];

                        display.fOutput.Add(newLine);

                        break;

                    case "@": // Production
                        if (!bShowProdBuilding)
                        {
                            if (chars < (blocks[1].Length + blocks[3].Length + blocks[4].Length + 4)) // Can Listing fit on one line?
                            {
                                display.fOutput.Add(blocks[1]);
                                display.fOutput.Add("Current: " + blocks[3]);
                                display.fOutput.Add("Target : " + blocks[4]);
                            }
                            else
                            {
                                newLine += blocks[1];
                                for (int i = 0; i < (chars - (blocks[1].Length + blocks[3].Length + blocks[4].Length + 1)); i++)
                                    newLine += "-";

                                newLine += blocks[3] + "/" + blocks[4];
                                display.fOutput.Add(newLine);
                            }
                        }

                        else
                        {
                            if (chars < (blocks[1].Length + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)) // Can Listing fit on one line?
                            {
                                display.fOutput.Add(blocks[1]);
                                display.fOutput.Add(blocks[2]);
                                display.fOutput.Add("Current: " + blocks[3]);
                                display.fOutput.Add("Target : " + blocks[4]);
                            }
                            else
                            {
                                newLine += blocks[1];

                                for (int i = 0; i < ProdCharBuffer - blocks[1].Length; i++)
                                    newLine += " ";
                                newLine += " | " + blocks[2];
                                for (int i = 0; i < (chars - (ProdCharBuffer + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)); i++)
                                    newLine += "-";

                                newLine += blocks[3] + "/" + blocks[4];
                                display.fOutput.Add(newLine);
                            }
                        }

                        break;
                }
            }
        }

        /// String Builders
        List<string> RawGroupBuilder(_Display display, IMyBlockGroup targetGroup)
        {
            List<string> output = new List<string>();

            output.Add("=" + Seperator + "(" + display.Module.TargetName + ")");
            output.Add("#");

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    break;

                case _ScreenMode.INVENTORY:
                    break;

                case _ScreenMode.RESOURCE:
                    output.Add(TableHeaderBuilder(display));
                    break;

                case _ScreenMode.STATUS:
                    output.Add(TableHeaderBuilder(display));
                    break;
            }

            List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
            display.Module.TargetGroup.GetBlocks(groupList);

            foreach (IMyTerminalBlock nextTermBlock in groupList)
            {
                if (Blocks.FindIndex(x => x.TermBlock == nextTermBlock) >= 0)
                    output.AddRange(RawBlockBuilder(display, Blocks.Find(x => x.TermBlock == nextTermBlock)));
                else
                    output.Add("!" + "/" + "Block data class not found! Signature missing from block in group!");
            }

            return output;
        }
        List<string> RawBlockBuilder(_Display display, _Block target)
        {
            List<string> output = new List<string>();

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:

                    break;

                case _ScreenMode.INVENTORY:
                    output.AddRange(BlockInventoryBuilder(target, display.Module));
                    break;

                case _ScreenMode.RESOURCE:
                    try
                    {
                        _Resource resource = (_Resource)target;
                        output.Add(BlockResourceBuilder(resource, display.Module.Notation));
                    }
                    catch
                    {
                        output.Add("!" + Seperator + "Not a resource!...");
                    }
                    break;

                    /*
                case _ScreenMode.STATUS:
                    output.Add(BlockStatusBuilder(target));
                    break;
                    */
            }

            return output;
        }
        List<string> BlockInventoryBuilder(_Block Block, _DisplayModule module)
        {
            List<string> output = new List<string>();
            output.Add("=" + Seperator + Block.CustomName);

            if (Block is _Cargo)
                output.AddRange(InventoryBuilder(PullInventory((_Cargo)Block), module));

            else if (Block is _Inventory)
            {
                output.Add("=" + Seperator + "|Input|");
                output.AddRange(InventoryBuilder(((IMyProductionBlock)((_Inventory)Block).TermBlock).InputInventory, module));
                output.Add("#");
                output.Add("=" + Seperator + "|Output|");
                output.AddRange(InventoryBuilder(((IMyProductionBlock)((_Inventory)Block).TermBlock).OutputInventory, module));
            }

            else
                output.Add("!" + Seperator + "Invalid Block Type!");
            output.Add("#");

            return output;
        }
        Dictionary<string, MyFixedPoint> ItemListBuilder(Dictionary<string, MyFixedPoint> dictionary, List<MyInventoryItem> items, _FilterProfile profile = null)
        {
            foreach (MyInventoryItem nextItem in items)
            {
                MyFixedPoint target;
                if (profile != null && !FilterCompare(profile, nextItem, out target))
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
        List<string> InventoryBuilder(IMyInventory targetInventory, _DisplayModule module)
        {
            List<string> output = new List<string>();
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();

            targetInventory.GetItems(items);
            if (items.Count == 0)
                output.Add("!" + Seperator + "Empty!");

            itemTotals = ItemListBuilder(itemTotals, items);

            foreach (var next in itemTotals)
                output.Add(ParseItemTotal(next, module));

            return output;
        }
        List<string> ProductionBuilder()
        {
            List<string> output = new List<string>();

            output.Add("#" + Seperator);

            foreach (_Production prod in Productions)
            {
                string line = "@" + Seperator;
                string nextDef = ProductionIdCrop(prod.Def.ToString());
                ProdCharBuffer = (ProdCharBuffer > nextDef.Length) ? ProdCharBuffer : nextDef.Length;
                line += nextDef + Seperator;
                line += prod.ProdIdString + Seperator;
                line += prod.Current + Seperator + prod.Target;
                output.Add(line);
            }

            return output;
        }
        List<string> ItemTallyBuilder(_DisplayModule module)
        {
            List<string> output = new List<string>();

            output.Add("#" + Seperator);

            if (module.FilterProfile.Filters.Count == 0)
            {
                output.Add("!" + Seperator + "No Filter!");
                return output;
            }

            Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            _Inventory targetInventory;

            switch (module.TargetType)
            {
                case _Target.BLOCK:
                    targetInventory = Inventories.Find(x => x == (_Inventory)module.TargetBlock);
                    PullInventory(targetInventory).GetItems(items);
                    ItemListBuilder(itemTotals, items, module.FilterProfile);
                    break;

                case _Target.GROUP:

                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    module.TargetGroup.GetBlocks(blocks);
                    foreach (IMyTerminalBlock block in blocks)
                    {
                        targetInventory = Inventories.Find(x => x.TermBlock == block);
                        if (targetInventory == null)
                            continue;
                        items.Clear();
                        PullInventory(targetInventory).GetItems(items);
                        ItemListBuilder(itemTotals, items, module.FilterProfile);
                    }
                    break;
            }

            foreach (var next in itemTotals)
                output.Add(ParseItemTotal(next, module));

            return output;
        }
        string TableHeaderBuilder(_Display display)
        {
            string output = string.Empty;

            switch (display.Module.Mode)
            {
                case _ScreenMode.DEFAULT:
                    break;

                case _ScreenMode.INVENTORY:
                    break;

                case _ScreenMode.RESOURCE:
                    output = ("^" + Seperator + "[Source]" + Seperator + "Val|Uni");
                    break;

                case _ScreenMode.STATUS:
                    output = ("^" + Seperator + "[Target]" + Seperator + "|E  P|I  H|");
                    break;
            }

            return output;
        }
        string BlockResourceBuilder(_Resource Block, _Notation notation)
        {
            string output = "%" + Seperator + Block.CustomName + Seperator;

            string value = string.Empty;
            int percent = 0;
            string unit = "n/a";

            switch (notation)
            {
                case _Notation.DEFAULT:
                case _Notation.SCIENTIFIC:
                case _Notation.SIMPLIFIED:

                    switch (Block.Type)
                    {
                        case _ResType.BATTERY:
                            IMyBatteryBlock batBlock = (IMyBatteryBlock)Block.TermBlock;
                            value = batBlock.CurrentStoredPower + "/" + batBlock.MaxStoredPower;
                            unit = batBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Stored power")).Split(' ')[3];
                            break;

                        case _ResType.POWERGEN:
                            IMyPowerProducer powBlock = (IMyPowerProducer)Block.TermBlock;
                            value = powBlock.CurrentOutput + "/" + powBlock.MaxOutput;
                            unit = powBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Current Output")).Split(' ')[3];
                            break;

                        case _ResType.GASTANK:
                            IMyGasTank gasTank = (IMyGasTank)Block.TermBlock;
                            value = gasTank.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Filled")).Split(' ')[2];
                            value = value.Substring(1, value.Length - 2);
                            value = value.Replace("L", "");
                            unit = " L ";
                            break;

                        case _ResType.GASGEN:
                            IMyGasGenerator gasGen = (IMyGasGenerator)Block.TermBlock;
                            value = (gasGen.IsWorking) ? "Running" : "NotRunning";
                            unit = "I/O";
                            break;

                        case _ResType.OXYFARM:
                            IMyOxygenFarm oxyFarm = (IMyOxygenFarm)Block.TermBlock;
                            value = (oxyFarm.IsWorking) ? "Running" : "NotRunning";
                            unit = "I/O";
                            break;
                    }
                    break;

                case _Notation.PERCENT:
                    switch (Block.Type)
                    {
                        case _ResType.BATTERY:
                            IMyBatteryBlock batBlock = (IMyBatteryBlock)Block.TermBlock;
                            percent = Convert.ToInt32((batBlock.CurrentStoredPower / batBlock.MaxStoredPower) * 100f);
                            break;

                        case _ResType.POWERGEN:
                            IMyPowerProducer powBlock = (IMyPowerProducer)Block.TermBlock;
                            percent = (int)((powBlock.CurrentOutput / powBlock.MaxOutput) * 100);
                            break;

                        case _ResType.GASTANK:
                            IMyGasTank gasTank = (IMyGasTank)Block.TermBlock;
                            percent = (int)((gasTank.FilledRatio) * 100);
                            break;

                        case _ResType.GASGEN:
                            IMyGasGenerator gasGen = (IMyGasGenerator)Block.TermBlock;
                            if (gasGen.IsWorking)
                                percent = 100;
                            break;

                        case _ResType.OXYFARM:
                            IMyOxygenFarm oxyFarm = (IMyOxygenFarm)Block.TermBlock;
                            if (oxyFarm.IsWorking)
                                percent = 100;
                            break;
                    }
                    break;
            }

            output += (notation == _Notation.PERCENT) ? percent + "| % " : (value + Seperator + unit);

            return output;
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
                itemID = lineblocks[0];

            if (lineblocks[0].Contains("!")) // Default insignia
                bDefault = true;

            for (int i = 1; i < lineblocks.Length; i++) // iterate through the remaining blocks
            {
                switch (lineblocks[i][0])
                {
                    case '#':   // set a new target value
                        target = (MyFixedPoint)float.Parse(lineblocks[i].Remove(0, 1));
                        break;

                    case '+':
                        bIn = (ContainsCIS(lineblocks[i], "in")) ? true : bIn;
                        bOut = (ContainsCIS(lineblocks[i], "out")) ? true : bOut;
                        break;

                    case '-':
                        bIn = (ContainsCIS(lineblocks[i], "in")) ? false : bIn;
                        bOut = (ContainsCIS(lineblocks[i], "out")) ? false : bOut;
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
                profile.Filters.Add(new _Filter(target, itemID, bIn, bOut));
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
                    if (ContainsCIS(nextline, "auto"))
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
                    if (ContainsCIS(nextline, "empty"))
                    {
                        if (nextline.Contains("-"))
                            inventory.FilterProfile.EMPTY = false;
                        else
                            inventory.FilterProfile.EMPTY = true;
                    }
                    if (ContainsCIS(nextline, "fill"))
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
        void InventoryUpdate(_Inventory inventory)
        {
            if (!CheckBlockExists(inventory))
                return;

            if (inventory.FilterProfile.EMPTY ||
                CheckProducerInputClog(inventory)) // Assembler anti-clog
                InventoryEmpty(inventory);

            if (inventory.FilterProfile.FILL)
                InventoryFill(inventory);
        }
        void InventoryEmpty(_Inventory inventory)
        {
            IMyInventory sourceInventory = PullInventory(inventory, false);
            List<MyInventoryItem> sourceItems = new List<MyInventoryItem>();
            sourceInventory.GetItems(sourceItems);

            int SearchCount = 0;

            foreach (MyInventoryItem nextItem in sourceItems)
            {
                if (SearchCount >= ItemSearchCap)
                {
                    //if (Rotations.FindIndex(x => x == inventory) == -1)
                    if (Rotations.Find(x => x == inventory) == null)
                        Rotations.Add(inventory);

                    break;
                }

                SearchCount++;

                MyFixedPoint target;

                for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < Inventories.Count(); i++)
                {
                    if (FilterCompare(inventory.FilterProfile, nextItem, out target, false))
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

            if (inventory.CallBackIndex != -1 &&
                !FillFromCandidate(inventory, Inventories[inventory.CallBackIndex]))
                inventory.CallBackIndex = -1;

            for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < Inventories.Count(); i++)
            {
                FillFromCandidate(inventory, Inventories[i]);
            }
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
        //void EmptyToCandidate(IMyInventory sourceInventory, MyInventoryItem nextItem)
        void EmptyToCandidate(_Inventory source, _Inventory target, MyInventoryItem nextItem)
        {
            int itemCount = 0; // Multi-Proc breaking count
            int invCount = 0;

            for (int i = InvSearchIndex; i < InvSearchIndex + InvSearchCap && i < Inventories.Count(); i++)
            {
                if (itemCount == ItemMoveCap)
                    break;

                if (invCount == InvSearchCap)
                    break;

                if (!CheckInventoryLink(source, target))
                    continue;

                invCount++;

                IMyInventory targetInventory = PullInventory(target);
                IMyInventory sourceInventory = PullInventory(source, false);

                MyFixedPoint value;
                if (!FilterCompare(Inventories[i].FilterProfile, nextItem, out value))
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
                        sourceInventory.TransferItemTo(targetInventory, nextItem, value - sourceCurrentAmount);
                        itemCount++;
                    }
                }
                else
                {
                    sourceInventory.TransferItemTo(targetInventory, nextItem);
                    itemCount++;
                }
            }
        }
        bool FillFromCandidate(_Inventory source, _Inventory target)
        {
            if (!CheckInventoryLink(target, source))
                return false;

            IMyInventory sourceInventory = PullInventory(source);
            IMyInventory targetInventory = PullInventory(target, false);

            int ItemCount = 0; // Multi-Proc breaking counts
            int SearchCount = 0;

            // Populate items
            List<MyInventoryItem> targetItems = new List<MyInventoryItem>();
            targetInventory.GetItems(targetItems);
            MyFixedPoint targetAmount = 0;

            foreach (MyInventoryItem nextItem in targetItems)
            {
                if (sourceInventory.IsFull)
                    break;

                if (ItemCount >= ItemMoveCap)
                    break;

                if (SearchCount >= ItemSearchCap)
                {
                    if (Rotations.Find(x => x == target) == null)
                        Rotations.Add(target);

                    break;
                }

                SearchCount++;

                MyFixedPoint value = 0;

                if (!(source is _Refinery) &&                                           // Refineries get override privledges
                    !FilterCompare(target.FilterProfile, nextItem, out value, false))   // Check aloud to leave
                    continue;

                if (!FilterCompare(source.FilterProfile, nextItem, out value))          // Check if it fits the pull request
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
                        ItemCount++;
                        source.CallBackIndex = Inventories.FindIndex(x => x == target);
                    }
                }

                else
                {
                    targetInventory.TransferItemTo(sourceInventory, nextItem);
                    ItemCount++;
                    source.CallBackIndex = Inventories.FindIndex(x => x == target);
                }
            }

            return ItemCount > 0;
        }
        /*void TallyUpdate(_Inventory sourceInventory, MyInventoryItem tallyItem)
        {
            foreach(_Production production in Productions)
            {
                if (!ItemDefinitionCompare(production.Def, tallyItem))
                    continue;

                Tally sourceTally = production.Totals.Find(x => x.Inventory == sourceInventory);
                if (sourceTally == null)
                {
                    sourceTally = new Tally(sourceInventory, tallyItem);
                    production.Totals.Add(sourceTally);
                }

                MyFixedPoint change = 0;
                if (!sourceTally.Refresh(out change))
                {
                    production.Totals.Remove(sourceTally);
                }

                production.Current += change;
            }
        }*/
        void RotateInventory(_Inventory inventory)
        {
            IMyInventory source = PullInventory(inventory);
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            source.GetItems(items);
            int count = items.Count();

            for (int i = 0; i < ItemSearchCap && i < count; i++)
                source.TransferItemTo(source, 0, count);
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
                finalList += pair.Key.BlueprintId + ":" + pair.Value.SubtypeId.ToString() + ":" + "userVal" + "\n";

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
                        producer.ProdBlock.ClearQueue();
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

        void ProductionTally(_Production prod)
        {
            prod.Current = 0;

            List<MyInventoryItem> allItems = new List<MyInventoryItem>();
            List<MyInventoryItem> targets = new List<MyInventoryItem>();

            int count = 0;
            foreach (_Inventory inventory in Inventories)
            {
                PullInventory(inventory, false).GetItems(allItems);
                count++;
            }
            /*
            string modDef = prod.Def.ToString().Split('/')[1];

            /////////////////////////////////////////////////
            // Mod specific hack, requires better autonomy //
            if (modDef[0] == 'P' && modDef[1] == 'O')
                modDef = modDef.Remove(0, 2);
            modDef = modDef.Replace("Component", "");
            modDef = modDef.Replace("PolymerTo", "");
            /////////////////////////////////////////////////
            */

            targets = allItems.FindAll(x => ItemDefinitionCompare(prod.Def, x));

            foreach (MyInventoryItem item in targets)
                prod.Current += item.Amount;

        }
        void ProductionUpdate(_Production prod)
        {
            //ProductionTally(prod);

            //prod.TallyUpdate();

            if (prod.Current >= prod.Target)
                return;

            List<_Producer> candidates = Producers.FindAll(x => x.ProdBlock.BlockDefinition.SubtypeId.ToString() == prod.ProdIdString);

            List<MyProductionItem> existingQues = new List<MyProductionItem>();

            foreach (_Producer producer in candidates)
            {
                if (!CheckBlockExists(producer))
                    continue;

                List<MyProductionItem> nextList = new List<MyProductionItem>();
                producer.ProdBlock.GetQueue(nextList);

                //for (int i = 0; i < nextList.Count; i++)
                for (int i = nextList.Count - 1; i > -1; i--)
                    if (nextList[i].Amount < 1)
                    {
                        producer.ProdBlock.RemoveQueueItem(i, 1f);
                        nextList.RemoveAt(i);
                    }

                //producer.ProdBlock.GetQueue(nextList);
                existingQues.AddRange(nextList.FindAll(x => x.BlueprintId == prod.Def));
            }

            MyFixedPoint existingTotal = 0;
            foreach (MyProductionItem item in existingQues)
                existingTotal += item.Amount;

            MyFixedPoint qeueTotal = prod.Target - (prod.Current + existingTotal);

            if (qeueTotal <= 0)
                return;

            MyFixedPoint qeueIndividual = (qeueTotal * ((float)1 / candidates.Count));  // Divide into equal portions
            qeueIndividual = (qeueIndividual < 1) ? 1 : qeueIndividual;                 // Always make atleast 1
            qeueIndividual = (int)qeueIndividual;                                       // Removal decimal place

            foreach (_Producer producer in candidates)                                  // Distribute
                producer.ProdBlock.AddQueueItem(prod.Def, qeueIndividual);
        }
        /*void ProducerClean(_Producer producer, _Inventory dump)
        {
            if (!producer.CLEAN)
                return;

            // Instantiate source Inventory  
            List<MyInventoryItem> sourceItems = new List<MyInventoryItem>();
            IMyInventory sourceInventory = PullInventory(producer);

            // Input Inventory
            if ((float)producer.ProdBlock.InputInventory.CurrentVolume / (float)producer.ProdBlock.InputInventory.MaxVolume > CleanPercent)
            {
                sourceInventory = producer.ProdBlock.InputInventory;
                sourceInventory.GetItems(sourceItems);
            }

            // Iterate Items and compare to potential filters
            foreach (MyInventoryItem nextItem in sourceItems)
            {
                MyItemType sourceType = nextItem.Type;
                EmptyToCandidate(sourceInventory, Inventories[] nextItem);
            }
        }*/
        bool CheckProducerInputClog(_Inventory inventory)
        {
            if (!(inventory is _Producer))
                return false;

            _Producer producer = (_Producer)inventory;

            return (float)producer.ProdBlock.InputInventory.CurrentVolume / (float)producer.ProdBlock.InputInventory.MaxVolume > CleanPercent;
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
                if (ContainsCIS(nextline, "prod") && !checks[0])
                {
                    index = PowerPrioritySet(Producers.Cast<_Block>().ToList(), index);
                    checks[0] = true;
                }

                if (ContainsCIS(nextline, "ref") && !checks[1])
                {
                    index = PowerPrioritySet(Refineries.Cast<_Block>().ToList(), index);
                    checks[1] = true;
                }

                if (ContainsCIS(nextline, "farm") && !checks[2])
                {
                    List<_Resource> farms = new List<_Resource>();
                    farms.AddRange(Resources.FindAll(x => x.Type == _ResType.OXYFARM));
                    index = PowerPrioritySet(farms.Cast<_Block>().ToList(), index);
                    checks[2] = true;
                }

                if (ContainsCIS(nextline, "gen") && !checks[3])
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
        string ProgEcho()
        {
            string echoOutput = string.Empty;

            echoOutput += $"{EchoLoop[EchoCount]} Torqk's Grid Manager {EchoLoop[EchoCount]}" +
                            "\n====================" +
                            $"\nInventory   : {(bInventoryRunning ? "Online" : "Offline")} : {InvQueIndex}" +
                            $"\nProduction : {(bProductionRunning ? "Online" : "Offline")} : {ProdQueIndex}" +
                            $"\nDisplay      : {(bDisplayRunning ? "Online" : "Offline")} : {DisplayQueIndex}" +
                            "\n====================" +
                            $"\nRotations: {Rotations.Count()}" +
                            $"\nSearchIndex: {InvSearchIndex}";

            echoOutput += (bSetupComplete) ? $"\nInvCount: {Inventories.Count}" : $"\nSetupIndex: {SetupIndex}";

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


            if (block is _Inventory)
                Inventories.RemoveAt(Inventories.FindIndex(x => x == (_Inventory)block));

            if (block is _Display)
                Displays.RemoveAt(Displays.FindIndex(x => x == (_Display)block));

            if (block is _Resource)
                Resources.RemoveAt(Resources.FindIndex(x => x == (_Resource)block));

            Blocks.RemoveAt(Blocks.FindIndex(x => x == block));

            return false;
        }
        bool CheckCandidate(IMyTerminalBlock block)
        {
            if (block == null)
                return false;
            return (Blocks.FindIndex(x => x.BlockID == block.EntityId) < 0 && block.CustomName.Contains(Signature));
        }
        void ReTagBlocks()
        {
            List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
            GridTerminalSystem.GetBlockGroups(groups);
            IMyBlockGroup group = groups.Find(x => ContainsCIS(x.Name, Signature));
            //IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(Signature);
            if (group == null)
                return;
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            group.GetBlocks(blocks);
            foreach (IMyTerminalBlock block in blocks)
                if (!block.CustomName.Contains(Signature))
                    block.CustomName += Signature;
        }
        void ClearAllBlockTags()
        {
            List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(allBlocks);

            foreach (IMyTerminalBlock block in allBlocks)
                if (block.CustomName.Contains(Signature))
                    block.CustomName = block.CustomName.Replace(Signature, string.Empty);
        }
        bool BlockListSetup()
        {
            bool setup = true;

            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < BatteryBlocks.Count(); i++) // Batteries first
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(BatteryBlocks[i]))
                {
                    _Resource newRes = new _Resource(BatteryBlocks[i], Signature, _ResType.BATTERY);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < CargoBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(CargoBlocks[i]))
                {
                    _Cargo newCargo = new _Cargo(CargoBlocks[i], new _FilterProfile(), Signature);
                    Blocks.Add(newCargo);
                    Inventories.Add(newCargo);
                    Cargos.Add(newCargo);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < ProdBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
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
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < RefineryBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
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
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < PanelBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PanelBlocks[i]))
                {
                    _Display newPanel = new _Display(PanelBlocks[i], Signature, DefFontSize);
                    Blocks.Add(newPanel);
                    Displays.Add(newPanel);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < PowerBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PowerBlocks[i]))
                {
                    _Resource newRes = new _Resource(PowerBlocks[i], Signature, _ResType.POWERGEN);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < TankBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(TankBlocks[i]))
                {
                    _Resource newRes = new _Resource(TankBlocks[i], Signature, _ResType.GASTANK);
                    Blocks.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < GeneratorBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(GeneratorBlocks[i]))
                {
                    _Resource newRes = new _Resource(GeneratorBlocks[i], Signature, _ResType.GASGEN);
                    Blocks.Add(newRes);
                    PowerConsumers.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            for (int i = SetupIndex; i < SetupIndex + SetupCap && i < OxyFarmBlocks.Count(); i++)
            {
                if (i == SetupIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(OxyFarmBlocks[i]))
                {
                    _Resource newRes = new _Resource(OxyFarmBlocks[i], Signature, _ResType.OXYFARM);
                    Blocks.Add(newRes);
                    PowerConsumers.Add(newRes);
                    Resources.Add(newRes);
                }
            }
            SetupIndex += SetupCap;
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

            // Controlled Updates
            if (bInventoryRunning &&
                InvClock == ClockCycle &&
                Inventories.Count > 0)
            {
                InventoryUpdate(Inventories[InvQueIndex]);

                InvQueIndex++;
                InvQueIndex = (InvQueIndex >= Inventories.Count) ? 0 : InvQueIndex;
                bRotate = InvQueIndex == 0;
                InvSearchIndex = (InvQueIndex == 0) ? InvSearchIndex + InvSearchCap : InvSearchIndex;
                InvSearchIndex = (InvSearchIndex >= Inventories.Count) ? 0 : InvSearchIndex;
            }

            if (bProductionRunning)
            {
                if (ProdClock == ClockCycle && Productions.Count > 0)
                {
                    ProductionUpdate(Productions[ProdQueIndex]);

                    ProdQueIndex++;
                    ProdQueIndex = (ProdQueIndex >= Productions.Count) ? 0 : ProdQueIndex;
                }

                /*if (CleanClock == ClockCycle && Producers.Count > 0)
                {
                    ProducerClean(Producers[CleanQueIndex], Inventories);

                    CleanQueIndex++;
                    CleanQueIndex = (CleanQueIndex >= Producers.Count) ? 0 : CleanQueIndex;
                }*/
            }

            if (bDisplayRunning &&
                DisplayClock == ClockCycle &&
                Displays.Count > 0)
            {
                DisplayUpdate(Displays[DisplayQueIndex]);

                DisplayQueIndex++;
                DisplayQueIndex = (DisplayQueIndex >= Displays.Count) ? 0 : DisplayQueIndex;
            }

            if (bRotate &&
                RotationClock == ClockCycle &&
                Rotations.Count > 0)
            {
                foreach (_Inventory inventory in Rotations)
                    RotateInventory(inventory);

                Rotations.Clear();
                bRotate = false;
            }

            // Clock Updates
            InvClock = (InvClock == ClockCycle) ? 0 : (InvClock + 1);
            ProdClock = (ProdClock == ClockCycle) ? 0 : (ProdClock + 1);
            //CleanClock = (CleanClock == ClockCycle) ? 0 : (CleanClock + 1);
            DisplayClock = (DisplayClock == ClockCycle) ? 0 : (DisplayClock + 1);
            RotationClock = (RotationClock == ClockCycle) ? 0 : (RotationClock + 1);
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
        public Program()
        {
            debug = Me.GetSurface(0);
            debug.ContentType = ContentType.TEXT_AND_IMAGE;
            debug.WriteText("", false);

            LoadRecipes();
            BlockDetection();

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            string output = ProgEcho();
            Echo(output);
            debug.WriteText(output);

            if (!bSetupComplete)
                bSetupComplete = BlockListSetup();

            if (bSetupComplete && !bUpdated)
                bUpdated = UpdateSettings();

            if (bSetupComplete && bUpdated)
                BlockListUpdate();

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
                    bInventoryRunning = !bInventoryRunning;
                    break;

                case "DIS":
                    bDisplayRunning = !bDisplayRunning;
                    break;

                case "PROD":
                    bProductionRunning = !bProductionRunning;
                    break;

                case "POWER":
                    bPowerRunning = !bPowerRunning;
                    break;
            }
        }
        public void Save()
        {
            Storage = Me.CustomData;
        }
        #endregion
    }
}

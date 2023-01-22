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
        static readonly int[] DefScreenRatio = { 25, 17 };
        const int InvSearchCap = 10;
        const int ItemSearchCap = 10;
        const int ProdSearchCap = 10;
        const int ItemMoveCap = 3;
        const int SetupCap = 20;
        const float CleanPercent = .8f;
        const float PowerThreshold = .2f;

        /// WARNING!! DO NOT GO FURTHER USER!! ///

        /// LOGIC

        RootMeta ROOT;
        int ROOT_INDEX = 0;
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

        List<block> Blocks = new List<block>();
        List<Display> Displays = new List<Display>();
        List<Resource> Resources = new List<Resource>();
        List<Inventory> Inventories = new List<Inventory>();

        List<Cargo> Cargos = new List<Cargo>();
        List<Producer> Producers = new List<Producer>();
        List<Refinery> Refineries = new List<Refinery>();

        List<Production> Productions = new List<Production>();
        List<block> PowerConsumers = new List<block>();
        Resource PowerMonitor;

        
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
            NONE,
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

        public struct RootMeta
        {
            public string Signature;
            public int ID;
            public Program Program;

            public RootMeta(string signature, Program program)
            {
                Signature = signature;
                Program = program;
                ID = -1;
            }
        }
        public struct BlockMeta
        {
            public RootMeta Root;
            public int Priority;
            public bool Detatchable;
            public IMyTerminalBlock Block;

            public BlockMeta(RootMeta root, IMyTerminalBlock block = null, bool bDetachable = false, int priority = -1)
            {
                Priority = priority;
                Detatchable = bDetachable;
                Root = root;
                Block = block;
            }
        }
        public struct FilterMeta
        {
            public bool IN_BOUND;
            public bool OUT_BOUND;
            public MyFixedPoint Target;
            public RootMeta Root;
            public string[] ItemID;

            public FilterMeta(
                RootMeta meta,
                bool In, bool Out,
                string[] id,
                MyFixedPoint target)
            {
                Root = meta;
                IN_BOUND = In;
                OUT_BOUND = Out;
                Target = target;
                ItemID = id;
            }

            public FilterMeta(
                RootMeta meta,
                bool In, bool Out,
                string id,
                MyFixedPoint target)
            {
                Root = meta;
                IN_BOUND = In;
                OUT_BOUND = Out;
                Target = target;
                ItemID = null;
                GenerateFilters(id, ref ItemID);
            }
        }
        public struct ProdMeta
        {
            public MyDefinitionId Def;
            public string ProducerType;
            public int Target;
            public FilterMeta Filter;

            public ProdMeta(RootMeta root, MyDefinitionId def, string prodIdString, int target = 0)
            {
                Def = def;
                ProducerType = prodIdString;
                Target = target;
                string[] id = new string[2];
                GenerateFilters(def, ref id);
                Filter = new FilterMeta(root, true, true, id, target);
            }
        }
        public struct DisplayMeta
        {
            public int SigCount;
            public string TargetName;

            public _ScreenMode Mode;
            public _Notation Notation;
            public _Target TargetType;

            //public Profile FilterProfile;
            public block TargetBlock;
            public IMyCubeGrid TargetGrid;
            public IMyBlockGroup TargetGroup;

            public DisplayMeta(RootMeta meta)
            {
                SigCount = DefSigCount;
                TargetName = "No Target";

                Mode = _ScreenMode.DEFAULT;
                Notation = _Notation.DEFAULT;
                TargetType = _Target.DEFAULT;

                //FilterProfile = new Profile(false, false);
                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }

        public class Root
        {
            public RootMeta rMeta;
            public StringBuilder DebugBuilder;

            public virtual void Setup()
            {

            }
            public virtual void Update()
            {
                DebugBuilder.Clear();
            }
            public virtual void RemoveMe()
            {

            }
            public Root(RootMeta meta)
            {
                DebugBuilder = new StringBuilder();
                rMeta = meta;
                rMeta.Program.RequestIndex();
            }
        }
        public class Filter
        {
            public string[] ItemID = new string[2];
            public FilterMeta Meta;

            
            void FilterSetup(string dataString)
            {
                /// FILTER CHANGE ///

                string[] lineblocks = dataString.Split(' ');  // Break each line into blocks

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
                }

                if (lineblocks[0].Contains("!")) // Default insignia
                {
                    bDefault = true;
                }

                for (int i = 1; i < lineblocks.Length; i++) // iterate through the remaining blocks
                {
                    switch (lineblocks[i][0])
                    {
                        case '#':   // set a new target value
                            target = (MyFixedPoint)float.Parse(lineblocks[i].Remove(0, 1));
                            break;

                        case '+':
                            bIn = (Contains(lineblocks[i], "in")) ? true : bIn;
                            bOut = (Contains(lineblocks[i], "out")) ? true : bOut;
                            break;

                        case '-':
                            bIn = (Contains(lineblocks[i], "in")) ? false : bIn;
                            bOut = (Contains(lineblocks[i], "out")) ? false : bOut;
                            break;
                    }
                }

                if (bDefault)
                {
                    Meta.IN_BOUND = bIn;
                    Meta.OUT_BOUND = bOut;
                }

                if (itemID != "null")
                {
                    GenerateFilters(itemID, ref ItemID);
                    //FilterMeta meta = new FilterMeta(Meta.Parent, Meta.Root, bIn, bOut,  target);
                    //profile.Filters.Add(new Filter(meta, itemID));
                }
            }

            public Filter(FilterMeta meta, string combo)
            {
                Meta = meta;
                GenerateFilters(combo, ref ItemID);
            }
            public Filter(FilterMeta meta, MyItemType type)
            {
                Meta = meta;
                GenerateFilters(type, ref ItemID);
            }
            public Filter(FilterMeta meta, MyDefinitionId id)
            {
                Meta = meta;
                GenerateFilters(id, ref ItemID);
            }
        }
        public class Profile
        {
            public RootMeta Meta;
            public List<Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;

            public Profile(RootMeta meta, bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false)
            {
                Meta = meta;
                Filters = new List<Filter>();
                DEFAULT_OUT = defOut;
                DEFAULT_IN = defIn;
                FILL = defFill;
                EMPTY = defEmpty;
            }
            public bool Setup(string customData)
            {
                Filters.Clear();
                DEFAULT_IN = true;
                DEFAULT_OUT = true;

                string[] data = customData.Split('\n'); //inventory.Block.CustomData.Split('\n');                                     // Break customData into lines

                foreach (string nextline in data)                                                               // Iterate each line
                {
                    if (nextline.Length == 0)                                                                   // Line must contain information
                        continue;

                    /// OPTION CHANGE ///

                    if (nextline[0] == '&')
                    {
                        if (Contains(nextline, "empty"))
                            EMPTY = !nextline.Contains("-");
                        if (Contains(nextline, "fill"))
                            FILL = !nextline.Contains("-");
                        continue;
                    }

                    /// FILTER CHANGE ///
                    Append(nextline);
                }

                Filters.RemoveAll(x => x.ItemID[0] == "any" && x.ItemID[1] == "any");  // Redundant. Refer to inventory default mode

                return true;
            }
            public void Append(string nextline)
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
                }

                if (lineblocks[0].Contains("!")) // Default insignia
                {
                    bDefault = true;
                }

                for (int i = 1; i < lineblocks.Length; i++) // iterate through the remaining blocks
                {
                    switch (lineblocks[i][0])
                    {
                        case '#':   // set a new target value
                            target = (MyFixedPoint)float.Parse(lineblocks[i].Remove(0, 1));
                            break;

                        case '+':
                            bIn = (Contains(lineblocks[i], "in")) ? true : bIn;
                            bOut = (Contains(lineblocks[i], "out")) ? true : bOut;
                            break;

                        case '-':
                            bIn = (Contains(lineblocks[i], "in")) ? false : bIn;
                            bOut = (Contains(lineblocks[i], "out")) ? false : bOut;
                            break;
                    }
                }

                if (bDefault)
                {
                    DEFAULT_IN = bIn;
                    DEFAULT_OUT = bOut;
                }

                if (itemID != "null")
                {
                    FilterMeta meta = new FilterMeta(Meta, bIn, bOut, , target);
                    Filters.Add(new Filter(meta, itemID));
                }

            }
        }
        public class Tally : Root
        {
            public Inventory Inventory;
            public MyItemType ItemType;
            public MyFixedPoint CurrentAmount;
            public MyFixedPoint OldAmount;
            public Tally(RootMeta meta, Inventory inventory, MyInventoryItem item) : base(meta)
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
        public class Production : Root
        {
            public MyDefinitionId Def;
            public string ProducerType;

            public Filter Filter;
            public MyFixedPoint Current;
            public List<Tally> Tallies;

            public Production(ProdMeta meta, RootMeta root) : base(root)
            {
                Def = meta.Def;
                ProducerType = meta.ProducerType;
                Filter = new Filter(meta.Filter, Def);
                Tallies = new List<Tally>();
            }

            public void TallyUpdate(Inventory sourceInventory, ref SearchCounter counter)
            {
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

                        sourceTally = new Tally(rMeta, sourceInventory, item);
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

            public override void Update()
            {
                if (Current >= Filter.Meta.Target)
                {
                    // Add excess que removal logic here later
                    return;
                }

                List<Producer> candidates = rMeta.Program.Producers;
                List<MyProductionItem> existingQues = new List<MyProductionItem>();

                foreach (Producer producer in candidates)
                {
                    if (!producer.CheckBlockExists())
                        continue;

                    List<MyProductionItem> nextList = new List<MyProductionItem>();
                    producer.ProdBlock.GetQueue(nextList);

                    for (int i = nextList.Count - 1; i > -1; i--)
                        if (nextList[i].Amount < 1)
                        {
                            producer.ProdBlock.RemoveQueueItem(i, 1f);
                            nextList.RemoveAt(i);
                        }

                    existingQues.AddRange(nextList.FindAll(x => FilterCompare(Filter, x)));
                }

                MyFixedPoint existingQueAmount = 0;
                foreach (MyProductionItem item in existingQues)
                    existingQueAmount += item.Amount;

                MyFixedPoint projectedTotal = Current + existingQueAmount;
                MyFixedPoint projectedOverage = Filter.Meta.Target - projectedTotal;

                if (projectedOverage >= 0)
                {
                    MyFixedPoint remove = new MyFixedPoint();

                    foreach (Producer producer in candidates)
                    {
                        existingQues.Clear();
                        producer.ProdBlock.GetQueue(existingQues);
                        for (int i = 0; i < existingQues.Count; i++)
                        {
                            if (!FilterCompare(Filter, existingQues[i]))
                                continue;

                            remove = projectedOverage > existingQues[i].Amount ? existingQues[i].Amount : projectedOverage;
                            producer.ProdBlock.RemoveQueueItem(i, remove);
                        }
                    }
                }
                else
                {
                    MyFixedPoint qeueIndividual = (projectedOverage * ((float)1 / candidates.Count));  // Divide into equal portions
                    qeueIndividual = (qeueIndividual < 1) ? 1 : qeueIndividual;                 // Always make atleast 1
                    qeueIndividual = (int)qeueIndividual;                                       // Removal decimal place

                    foreach (Producer producer in candidates)                                  // Distribute
                        producer.ProdBlock.AddQueueItem(Def, qeueIndividual);
                }
            }
        }
        public class block : Root
        {
            public long BlockID;
            public string CustomName;

            public int Priority;
            public bool bDetatchable;
            public IMyTerminalBlock TermBlock;
            public Profile FilterProfile;

            public block(BlockMeta bMeta) : base(bMeta.Root)
            {
                TermBlock = bMeta.Block;
                CustomName = TermBlock.CustomName.Replace(bMeta.Root.Signature, "");
                BlockID = TermBlock.EntityId;
                Priority = -1;
            }
            public bool CheckBlockExists()
            {
                IMyTerminalBlock maybeMe = rMeta.Program.GridTerminalSystem.GetBlockWithId(TermBlock.EntityId); // BlockID?
                if (maybeMe != null &&                          // Exists?
                    maybeMe.CustomName.Contains(Signature))    // Signature?
                    return true;

                if (!bDetatchable)
                    RemoveMe();

                return false;
            }

            public override void RemoveMe()
            {

            }
        }
        public class Display : block
        {
            public IMyTextPanel Panel;
            public string oldData;
            public float oldFontSize;

            public StringBuilder rawOutput;
            public StringBuilder fOutput;
            public string[][] Buffer = new string[2][];

            public int[] ScreenRatio = new int[2]; // chars, lines
            public int OutputIndex;
            public int Scroll;
            public int Delay;
            public int Timer;

            public DisplayMeta dMeta;

            public Display(BlockMeta bMeta, float fontSize = 1) : base(bMeta)
            {
                Panel = (IMyTextPanel)bMeta.Block;
                RebootScreen(fontSize);
                oldData = Panel.CustomData;
                oldFontSize = Panel.FontSize;
                rawOutput = new StringBuilder();
                fOutput = new StringBuilder();
                OutputIndex = 0;
                Scroll = 1;
                Delay = 10;
                Timer = 0;

                dMeta = new DisplayMeta(bMeta.Root);
            }
            public override void Setup()
            {
                FilterProfile.Filters.Clear();

                Buffer[0] = Panel.CustomData.Split('\n');

                for(int i = 0; i < Buffer[0].Length; i++)
                //foreach(string nextline in Buffer[0])
                {
                    string nextline = Buffer[0][i];

                    char check = (nextline.Length > 0) ? nextline[0] : '/';

                    Buffer[1] = nextline.Split(' ');

                    switch (check)
                    {
                        case '/': // Comment Section (ignored)
                            break;

                        case '*': // Mode
                            if (Contains(nextline, "stat"))
                                dMeta.Mode = _ScreenMode.STATUS;
                            if (Contains(nextline, "inv"))
                                dMeta.Mode = _ScreenMode.INVENTORY;
                            if (Contains(nextline, "prod"))
                                dMeta.Mode = _ScreenMode.PRODUCTION;
                            if (Contains(nextline, "res"))
                                dMeta.Mode = _ScreenMode.RESOURCE;
                            if (Contains(nextline, "tally"))
                                dMeta.Mode = _ScreenMode.TALLY;
                            break;

                        case '@': // Target
                            if (Contains(nextline, "block"))
                            {
                                //Operation
                                block block = rMeta.Program.Blocks.Find(x => x.TermBlock.CustomName.Contains(Buffer[1][1]));

                                if (block != null)
                                {

                                    dMeta.TargetType = _Target.BLOCK;
                                    dMeta.TargetBlock = block;
                                    dMeta.TargetName = block.CustomName;
                                }
                                else
                                {
                                    dMeta.TargetType = _Target.DEFAULT;
                                    dMeta.TargetName = "Block not found!";
                                }
                                break;
                            }
                            if (Contains(nextline, "group"))
                            {
                                IMyBlockGroup targetGroup = rMeta.Program.BlockGroups.Find(x => x.Name.Contains(Buffer[1][1]));
                                if (targetGroup != null)
                                {
                                    dMeta.TargetType = _Target.GROUP;
                                    dMeta.TargetGroup = targetGroup;
                                    dMeta.TargetName = targetGroup.Name;
                                }
                                else
                                {
                                    dMeta.TargetType = _Target.DEFAULT;
                                    dMeta.TargetName = "Group not found!";
                                }
                                break;
                            }
                            break;

                        case '&':   // Option
                            if (Contains(nextline, "scroll"))
                            {
                                int newDelay = Convert.ToInt32(Buffer[1][1]);
                                Delay = newDelay > 0 ? newDelay : 10;
                            }
                            break;

                        case '#':   // Notation
                            if (Contains(nextline, "def"))
                                dMeta.Notation = _Notation.DEFAULT;
                            if (Contains(nextline, "simp"))
                                dMeta.Notation = _Notation.SIMPLIFIED;
                            if (Contains(nextline, "sci"))
                                dMeta.Notation = _Notation.SCIENTIFIC;
                            if (Contains(nextline, "%"))
                                dMeta.Notation = _Notation.PERCENT;
                            break;

                        default:
                            FilterProfile.Append(nextline);
                            break;
                    }
                }
            }
            public void RebootScreen(float fontSize, int[] ratio = null)
            {
                if (Panel == null)
                    return;

                Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                Panel.Font = "Monospace";
                Panel.FontSize = fontSize;
                ScreenRatio = ratio == null ? DefScreenRatio : ratio;
            }
            public void DisplayUpdate()
            {
                //if (!CheckBlockExists(display))
                //    return;

                RawStringBuilder();
                FormattedStringBuilder();
            }
            public void DisplayRefresh()
            {
                if (!CheckBlockExists())
                    return;

                Panel.WriteText("", false);
                int Offset = (dMeta.TargetType == _Target.GROUP && dMeta.Mode == _ScreenMode.INVENTORY) ? 4 : 2; // Header Work around
                int lineCount = MonoSpaceLines(ScreenRatio[1], Panel) - Offset;

                bool tick = false;
                Timer++;
                if (Timer >= Delay)
                {
                    Timer = 0;
                    tick = true;
                }

                string[] lines = fOutput.ToString().Split('\n');

                if (lines.Length > MonoSpaceLines(ScreenRatio[1], Panel)) // Requires Scrolling
                {
                    List<string> formattedSection = new List<string>();
                    if (OutputIndex < Offset || OutputIndex > (lines.Length - lineCount)) // Index Reset Failsafe
                        OutputIndex = Offset;

                    for (int i = 0; i < Offset; i++)
                        formattedSection.Add(lines[i]);

                    for (int i = OutputIndex; i < (OutputIndex + lineCount); i++)
                        formattedSection.Add(lines[i]);

                    if (OutputIndex == lines.Length - lineCount)
                        Scroll = -1;
                    if (OutputIndex == Offset)
                        Scroll = 1;

                    if (tick)
                        OutputIndex += Scroll;

                    foreach (string nextString in formattedSection)
                        Panel.WriteText(nextString + "\n", true);
                }

                else // Static
                {
                    foreach (string nextString in lines)
                        Panel.WriteText(nextString + "\n", true);
                }
            }
            void RawStringBuilder()
            {
                rawOutput.Clear();

                switch (dMeta.Mode)
                {
                    case _ScreenMode.DEFAULT:
                        rawOutput.Append($"={Seperator}[Default]\n");
                        break;

                    case _ScreenMode.INVENTORY:
                        rawOutput.Append($"={Seperator}[Inventory]\n");
                        break;

                    case _ScreenMode.RESOURCE:
                        rawOutput.Append($"={Seperator}[Resource]\n");
                        break;

                    case _ScreenMode.STATUS:
                        rawOutput.Append($"={Seperator}[Status]\n");
                        break;

                    case _ScreenMode.PRODUCTION:
                        rawOutput.Append($"={Seperator}[Production]\n");
                        ProductionBuilder();
                        break;

                    case _ScreenMode.TALLY:
                        rawOutput.Append($"={Seperator}[Tally]\n");
                        ItemTallyBuilder();
                        break;
                }

                rawOutput.Append("#\n");

                if (dMeta.Mode > (_ScreenMode)3)
                    return; // No Target for tally systems

                switch (dMeta.TargetType)
                {
                    case _Target.DEFAULT:
                        rawOutput.Append("=" + "/" + dMeta.TargetName);
                        break;

                    case _Target.BLOCK:
                        if (dMeta.Mode == _ScreenMode.RESOURCE || dMeta.Mode == _ScreenMode.STATUS)
                            TableHeaderBuilder();
                        RawBlockBuilder(dMeta.TargetBlock);
                        break;

                    case _Target.GROUP:
                        RawGroupBuilder(dMeta.TargetGroup);
                        break;
                }

            }
            void FormattedStringBuilder()
            {
                fOutput.Clear();

                int chars = MonoSpaceChars(DefScreenRatio[0], Panel);
                string[] strings = rawOutput.ToString().Split('\n');

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
                            if (!rMeta.Program.bShowProdBuilding)
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

                                    for (int i = 0; i < rMeta.Program.ProdCharBuffer - blocks[1].Length; i++)
                                        formattedString += " ";
                                    formattedString += " | " + blocks[2];
                                    for (int i = 0; i < (chars - (rMeta.Program.ProdCharBuffer + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)); i++)
                                        formattedString += "-";

                                    formattedString += blocks[3] + "/" + blocks[4];
                                }
                            }

                            break;
                    }

                    fOutput.Append(formattedString + "\n");
                }
            }

            /// String Builders
            void RawGroupBuilder(IMyBlockGroup targetGroup)
            {
                rawOutput.Append("=" + Seperator + "(" + dMeta.TargetName + ")\n");
                rawOutput.Append("#\n");

                TableHeaderBuilder();

                List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
                dMeta.TargetGroup.GetBlocks(groupList);

                foreach (IMyTerminalBlock nextTermBlock in groupList)
                {
                    block next = rMeta.Program.Blocks.Find(x => x.TermBlock == nextTermBlock);
                    if (next != null)
                        RawBlockBuilder(next);
                    else
                        rawOutput.Append("!" + "/" + "Block data class not found! Signature missing from block in group!\n");
                }
            }
            void RawBlockBuilder(block target)
            {
                switch (dMeta.Mode)
                {
                    case _ScreenMode.DEFAULT:

                        break;

                    case _ScreenMode.INVENTORY:
                        BlockInventoryBuilder(target);
                        break;

                    case _ScreenMode.RESOURCE:
                        try
                        {
                            Resource resource = (Resource)target;
                            BlockResourceBuilder(resource);
                        }
                        catch
                        {
                            rawOutput.Append("!" + Seperator + "Not a resource!...\n");
                        }
                        break;

                        /*
                    case _ScreenMode.STATUS:
                        output.Add(BlockStatusBuilder(target));
                        break;
                        */
                }
            }
            void BlockInventoryBuilder(block target)
            {
                rawOutput.Append("=" + Seperator + target.CustomName + "\n");

                if (target is Cargo)
                    InventoryBuilder(PullInventory((Cargo)target));

                else if (target is Inventory)
                {
                    rawOutput.Append("=" + Seperator + "|Input|\n");
                    InventoryBuilder(((IMyProductionBlock)((Inventory)target).TermBlock).InputInventory);
                    rawOutput.Append("#\n");
                    rawOutput.Append("=" + Seperator + "|Output|\n");
                    InventoryBuilder(((IMyProductionBlock)((Inventory)target).TermBlock).OutputInventory);
                }

                else
                    rawOutput.Append("!" + Seperator + "Invalid Block Type!\n");

                rawOutput.Append("#\n");
            }

            void InventoryBuilder(IMyInventory targetInventory)
            {
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();

                targetInventory.GetItems(items);
                if (items.Count == 0)
                {
                    rawOutput.Append("!" + Seperator + "Empty!\n");
                    return;
                }

                itemTotals = ItemListBuilder(itemTotals, items);

                foreach (var next in itemTotals)
                    rawOutput.Append(ParseItemTotal(next, dMeta) + "\n");
            }
            void ProductionBuilder()
            {
                rawOutput.Append("#");

                foreach (Production prod in rMeta.Program.Productions)
                {
                    rawOutput.Append("\n@" + Seperator);
                    string nextDef = prod.Filter.ItemID[1];
                    rawOutput.Append(nextDef + Seperator);
                    rMeta.Program.ProdCharBuffer = (rMeta.Program.ProdCharBuffer > nextDef.Length) ? rMeta.Program.ProdCharBuffer : nextDef.Length;

                    rawOutput.Append(
                        prod.ProducerType + Seperator +
                        prod.Current + Seperator +
                        prod.Filter.Meta.Target);
                }
            }
            void ItemTallyBuilder()
            {
                rawOutput.Append("#" + Seperator + "\n");

                if (FilterProfile.Filters.Count == 0)
                {
                    rawOutput.Append("!" + Seperator + "No Filter!\n");
                    return;
                }

                Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                //Inventory targetInventory;

                switch (dMeta.TargetType)
                {
                    case _Target.BLOCK:
                        PullInventory((Inventory)dMeta.TargetBlock).GetItems(items);
                        ItemListBuilder(itemTotals, items, FilterProfile);
                        break;

                    case _Target.GROUP:

                        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                        dMeta.TargetGroup.GetBlocks(blocks);
                        foreach (IMyTerminalBlock block in blocks)
                        {
                            Inventory targetInventory = rMeta.Program.Inventories.Find(x => x.TermBlock == block);
                            if (targetInventory == null)
                                continue;
                            items.Clear();
                            PullInventory(targetInventory).GetItems(items);
                            ItemListBuilder(itemTotals, items, FilterProfile);
                        }
                        break;
                }

                foreach (var next in itemTotals)
                    rawOutput.Append(ParseItemTotal(next, dMeta) + "\n");
            }
            void TableHeaderBuilder()
            {
                switch (dMeta.Mode)
                {
                    case _ScreenMode.DEFAULT:
                        break;

                    case _ScreenMode.INVENTORY:
                        break;

                    case _ScreenMode.RESOURCE:
                        rawOutput.Append("^" + Seperator + "[Source]" + Seperator + "Val|Uni\n");
                        break;

                    case _ScreenMode.STATUS:
                        rawOutput.Append("^" + Seperator + "[Target]" + Seperator + "|E  P|I  H|\n");
                        break;
                }
            }
            void BlockResourceBuilder(Resource targetBlock)
            {
                rawOutput.Append("%" + Seperator + targetBlock.CustomName + Seperator);

                string value = string.Empty;
                int percent = 0;
                string unit = "n/a";

                switch (dMeta.Notation)
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


                rawOutput.Append(((dMeta.Notation == _Notation.PERCENT) ? percent + "| % " : (value + Seperator + unit)) + "\n");
            }
        }
        public class Inventory : block
        {
            public int CallBackIndex;
            public Inventory(BlockMeta meta, bool active = true) : base(meta)
            {
                CallBackIndex = -1;
            }

            public override void Setup()
            {
                FilterProfile.Setup(TermBlock.CustomData);
            }

            void TallyUpdates()
            {
                rMeta.Program.Counter.Reset();

                for (int i = rMeta.Program.ProdSearchIndex; i < (rMeta.Program.ProdSearchIndex + ProdSearchCap) && i < rMeta.Program.Productions.Count(); i++)
                {
                    rMeta.Program.Productions[i].TallyUpdate(this, ref rMeta.Program.Counter);
                }
            }
            public void Sort()
            {
                if (FilterProfile.EMPTY ||
                    InventoryEmptyCheck()) // Assembler anti-clog
                    Empty();

                if (FilterProfile.FILL &&
                    InventoryFillCheck())
                    Fill();

                //RotateInventory(inventory);
            }
            public virtual bool InventoryEmptyCheck()
            {
                return false;
            }
            public virtual bool InventoryFillCheck()
            {
                return true;
            }
            public void Empty()
            {
                IMyInventory sourceInventory = PullInventory(this, false);
                List<MyInventoryItem> sourceItems = new List<MyInventoryItem>();
                sourceInventory.GetItems(sourceItems);
                MyFixedPoint target;

                /*
                Items moved
                Items searched
                Inv searched
                 */

                rMeta.Program.Counter.Reset();

                foreach (MyInventoryItem nextItem in sourceItems)
                {
                    if (rMeta.Program.Counter.Check(Count.MOVED)) // Total Items moved
                        break;

                    if (rMeta.Program.Counter.Increment(Count.ITEMS)) // Total Items searched
                        break;

                    if (ProfileCompare(FilterProfile, nextItem, out target, false))
                        continue;

                    for (int i = rMeta.Program.InvSearchIndex; i < (rMeta.Program.InvSearchIndex + InvSearchCap) && i < rMeta.Program.Inventories.Count(); i++)
                    {
                        if (rMeta.Program.Counter.Check(Count.MOVED)) // Total Items moved
                            break;
                        EmptyToCandidate(this, rMeta.Program.Inventories[i], nextItem);
                    }
                }
            }
            public void Fill()
            {


                if (FilterProfile.Filters.Count() <= 0)
                    return; // No Filters to pull


                /*
                Items moved
                Inv searched
                Items searched
                 */

                rMeta.Program.Counter.Reset();

                if (CallBackIndex != -1 &&
                    !FillFromCandidate(this, rMeta.Program.Inventories[this.CallBackIndex]))
                    CallBackIndex = -1;

                for (int i = rMeta.Program.InvSearchIndex; i < (rMeta.Program.InvSearchIndex + InvSearchCap) && i < rMeta.Program.Inventories.Count(); i++)
                {
                    if (rMeta.Program.Counter.Check(Count.MOVED))
                        return;

                    if (FillFromCandidate(this, rMeta.Program.Inventories[i]))
                        break;
                }
            }
            public void Rotate()
            {
                IMyInventory source = PullInventory(this);
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                source.GetItems(items);
                int count = items.Count();
                if (count < ItemSearchCap)
                    return;

                for (int i = 0; i < ItemSearchCap && i < count; i++)
                    source.TransferItemTo(source, 0, count);
            }

            
            bool CheckInventoryLink(Inventory outbound, Inventory inbound)
            {
                if (outbound == inbound)
                    return false;

                if (!PullInventory(outbound, false).IsConnectedTo(PullInventory(inbound)))
                    return false;

                if (PullInventory(inbound).IsFull)
                    return false;

                return true;
            }
            void EmptyToCandidate(Inventory source, Inventory target, MyInventoryItem currentItem)
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
                        rMeta.Program.Counter.Increment(Count.MOVED);
                    }
                }
                else
                {
                    sourceInventory.TransferItemTo(targetInventory, currentItem);
                    rMeta.Program.Counter.Increment(Count.MOVED);
                }
            }
            bool FillFromCandidate(Inventory source, Inventory target)
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

                    if (rMeta.Program.Counter.Increment(Count.ITEMS))
                        break;

                    MyFixedPoint value = 0;

                    if (!(source is Refinery) &&                                           // Refineries get override privledges
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
                            source.CallBackIndex = rMeta.Program.Inventories.FindIndex(x => x == target);
                            callback = true;
                            if (rMeta.Program.Counter.Increment(Count.MOVED))
                                break;
                        }
                    }

                    else
                    {
                        targetInventory.TransferItemTo(sourceInventory, nextItem);
                        source.CallBackIndex = rMeta.Program.Inventories.FindIndex(x => x == target);
                        callback = true;
                        if (rMeta.Program.Counter.Increment(Count.MOVED))
                            break;
                    }
                }

                rMeta.Program.Counter.Reset(Count.ITEMS);
                return callback;
            }
        }
        public class Cargo : Inventory
        {
            public Cargo(BlockMeta meta) : base(meta)
            {
                
            }
        }
        public class Producer : Inventory
        {
            public IMyProductionBlock ProdBlock;

            public bool CLEAN;
            public Producer(BlockMeta meta) : base(meta)
            {
                ProdBlock = (IMyProductionBlock)meta.Block;
                CLEAN = true;
            }

            public override bool InventoryEmptyCheck()
            {
                return (float)ProdBlock.InputInventory.CurrentVolume / (float)ProdBlock.InputInventory.MaxVolume > CleanPercent;
            }
        }
        public class Refinery : Inventory
        {
            public IMyRefinery RefineBlock;
            public bool AutoRefine;

            public Refinery(BlockMeta meta, bool auto = false) : base(meta)
            {
                RefineBlock = (IMyRefinery)meta.Block;
                AutoRefine = auto;
                RefineBlock.UseConveyorSystem = AutoRefine;
            }

            public override void Setup()
            {
                string[] data = TermBlock.CustomData.Split('\n');

                foreach (string nextline in data)                                                               // Iterate each line
                {
                    if (nextline.Length == 0)                                                                   // Line must contain information
                        continue;

                    /// OPTION CHANGE ///

                    if (nextline[0] == '&')
                    {
                        if (Contains(nextline, "auto"))
                        {
                            AutoRefine = !nextline.Contains("-");
                        }
                    }
                }

                RefineBlock.UseConveyorSystem = AutoRefine;
            }

            public override bool InventoryFillCheck()
            {
                return !AutoRefine;
            }
        }
        public class Resource : block
        {
            public _ResType Type;
            public bool bIsValue;

            public Resource(BlockMeta meta, _ResType type, bool isValue = true) : base(meta)
            {
                Type = type;
                bIsValue = isValue;
            }
        }


        /// Helpers
        static Dictionary<string, MyFixedPoint> ItemListBuilder(Dictionary<string, MyFixedPoint> dictionary, List<MyInventoryItem> items, Profile profile = null)
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
        static IMyInventory PullInventory(Inventory inventory, bool input = true)
        {
            if (inventory is Cargo)
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
        static string ParseItemTotal(KeyValuePair<string, MyFixedPoint> item, DisplayMeta module)
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
        static bool FilterCompare(Filter A, MyInventoryItem B)
        {
            return FilterCompare(
                A.ItemID[0], A.ItemID[1],
                B.Type.TypeId, B.Type.SubtypeId);
        }
        static bool FilterCompare(Filter A, MyProductionItem B)
        {
            return FilterCompare(
                A.ItemID[0], A.ItemID[1],
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
        static bool ProfileCompare(Profile profile, MyInventoryItem item, out MyFixedPoint target, bool dirIn = true)
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

            foreach (Filter filter in profile.Filters)
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

        static void GenerateFilters(string combo, ref string[] id)
        {
            id[0] = "null";
            id[1] = "null";

            try
            {
                string[] strings = combo.Split(':');
                id[0] = strings[0];
                id[1] = strings[1];

                id[0] = id[0] == "any" || id[0] == "" ? "any" : id[0];
                id[1] = id[1] == "any" || id[1] == "" ? "any" : id[1];
            }
            catch { }
        }
        static void GenerateFilters(MyDefinitionId def, ref string[] id) // Production
        {
            id[0] = "Component";
            id[1] = def.SubtypeName.ToString();
        }
        static void GenerateFilters(MyItemType type, ref string[] id)
        {
            id[0] = type.TypeId;
            id[1] = type.SubtypeId;
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

        /// Inventory
        
        
        

        /// Production
        string GenerateRecipes()
        {
            Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> recipeList = new Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId>();

            List<MyProductionItem> nextList = new List<MyProductionItem>();
            string finalList = string.Empty;

            foreach (Producer producer in Producers)
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
                    foreach (Producer producer in Producers)
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

                ProdMeta prod = new ProdMeta(ROOT, nextId, prodId, target);
                Production nextProd = new Production(prod, ROOT);
                Productions.Add(nextProd);
            }
        }
        void ProductionUpdate(Production prod)
        {
            if (prod.Current >= prod.Filter.Meta.Target)
            {
                // Add excess que removal logic here later
                return;
            }

            List<Producer> candidates = Producers.FindAll(x => x.ProdBlock.BlockDefinition.SubtypeId.ToString() == prod.ProducerType);
            List<MyProductionItem> existingQues = new List<MyProductionItem>();

            Debug.Append($"ProdUpdate: {prod.Def}\n");

            foreach (Producer producer in candidates)
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

                existingQues.AddRange(nextList.FindAll(x => FilterCompare(prod.Filter, x)));
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

            foreach (Producer producer in candidates)                                  // Distribute
                producer.ProdBlock.AddQueueItem(prod.Def, qeueIndividual);
        }

        /// Power
        bool PowerSetup(Resource candidateMonitor)
        {
            foreach (block block in PowerConsumers)
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
                    index = PowerPrioritySet(Producers.Cast<block>().ToList(), index);
                    checks[0] = true;
                }

                if (Contains(nextline, "ref") && !checks[1])
                {
                    index = PowerPrioritySet(Refineries.Cast<block>().ToList(), index);
                    checks[1] = true;
                }

                if (Contains(nextline, "farm") && !checks[2])
                {
                    List<Resource> farms = new List<Resource>();
                    farms.AddRange(Resources.FindAll(x => x.Type == _ResType.OXYFARM));
                    index = PowerPrioritySet(farms.Cast<block>().ToList(), index);
                    checks[2] = true;
                }

                if (Contains(nextline, "gen") && !checks[3])
                {
                    List<Resource> gens = new List<Resource>();
                    gens.AddRange(Resources.FindAll(x => x.Type == _ResType.GASGEN));
                    index = PowerPrioritySet(gens.Cast<block>().ToList(), index);
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
        int PowerPrioritySet(List<block> blocks, int start)
        {
            int index = 0;

            foreach (block block in blocks)
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
                foreach (block block in PowerConsumers)
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

        bool CheckBlockExists(block block)
        {
            IMyTerminalBlock maybeMe = GridTerminalSystem.GetBlockWithId(block.BlockID);
            if (maybeMe != null &&                          // Exists?
                maybeMe.CustomName.Contains(Signature))    // Signature?
                return true;

            if (!block.bDetatchable)
            {
                if (block is Inventory)
                    Inventories.RemoveAt(Inventories.FindIndex(x => x == (Inventory)block));

                if (block is Display)
                    Displays.RemoveAt(Displays.FindIndex(x => x == (Display)block));

                if (block is Resource)
                    Resources.RemoveAt(Resources.FindIndex(x => x == (Resource)block));

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

            /*foreach (Resource resource in Resources)
            {
                if (PowerSetup(resource))
                {
                    bPowerSetupComplete = true;
                    break;
                }
            }

            if (!bPowerSetupComplete)
                PowerPrioritySet(Blocks, 0);*/

            //foreach

            foreach (Cargo nextCargo in Cargos)
                nextCargo.Setup();

            foreach (Refinery nextRefine in Refineries)
                nextRefine.Setup();

            foreach (Display display in Displays)
                display.Setup();

            ProductionSetup();

            return true;
        }


        bool BlockListBuilder()
        {
            bool setup = true;
            block newBlock;

            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < BatteryBlocks.Count(); i++) // Batteries first
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(BatteryBlocks[i]))
                {
                    newBlock = new Resource(new BlockMeta(ROOT, BatteryBlocks[i]),_ResType.BATTERY);
                    Resources.Add((Resource)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < CargoBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(CargoBlocks[i]))
                {
                    newBlock = new Cargo(new BlockMeta(ROOT, CargoBlocks[i]));
                    Inventories.Add((Inventory)newBlock);
                    Cargos.Add((Cargo)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < ProdBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(ProdBlocks[i]))
                {
                    newBlock = new Producer(new BlockMeta(ROOT, ProdBlocks[i]));
                    PowerConsumers.Add(newBlock);
                    Inventories.Add((Inventory)newBlock);
                    Producers.Add((Producer)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < RefineryBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(RefineryBlocks[i]))
                {
                    newBlock = new Refinery(new BlockMeta(ROOT, RefineryBlocks[i]));
                    PowerConsumers.Add(newBlock);
                    Inventories.Add((Inventory)newBlock);
                    Refineries.Add((Refinery)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < PanelBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PanelBlocks[i]))
                {
                    newBlock = new Display(new BlockMeta(ROOT, PanelBlocks[i]));
                    PowerConsumers.Add(newBlock);
                    Displays.Add((Display)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < PowerBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(PowerBlocks[i]))
                {
                    newBlock = new Resource(new BlockMeta(ROOT, PowerBlocks[i]), _ResType.POWERGEN);
                    Resources.Add((Resource)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < TankBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(TankBlocks[i]))
                {
                    newBlock = new Resource(new BlockMeta(ROOT, TankBlocks[i]), _ResType.GASTANK);
                    Resources.Add((Resource)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < GeneratorBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(GeneratorBlocks[i]))
                {
                    newBlock = new Resource(new BlockMeta(ROOT, GeneratorBlocks[i]), _ResType.GASGEN);
                    PowerConsumers.Add(newBlock);
                    Resources.Add((Resource)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }
            for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < OxyFarmBlocks.Count(); i++)
            {
                if (i == SetupQueIndex + SetupCap - 1)
                    setup = false;

                if (CheckCandidate(OxyFarmBlocks[i]))
                {
                    newBlock = new Resource(new BlockMeta(ROOT, OxyFarmBlocks[i]), _ResType.OXYFARM);
                    PowerConsumers.Add(newBlock);
                    Resources.Add((Resource)newBlock);

                    Blocks.Add(newBlock);
                    newBlock.Setup();
                }
            }

            SetupQueIndex += SetupCap;
            return setup;
        }
        public enum BlockType
        {
            DISPLAY,
            CARGO,
            PRODUCER,
            REFINERY,
            POW_CON,
            RESOURCE
        }

        static void BuildBlock<T>(T termBlock) where T : class
        {
            if (typeof(T) == typeof(IMyRefinery))
            {

            }

            if (typeof(T) == typeof(IMyAssembler))
            {

            }

            if (typeof(T) == typeof(IMyCargoContainer))
            {

            }

            /*if (typeof(T) == typeof(IMyInventoryOwner))
            {

            }*/

            if (typeof(T) == typeof(IMyTextPanel))
            {

            }

            if (typeof(T) == typeof(IMyTerminalBlock))
            {

            }
        }

        void BlockListUpdate()
        {
            // Persistent Updates
            if (bPowerRunning)
                PowerUpdate();

            if (bDisplayRunning)
                foreach (Display display in Displays)
                    display.DisplayRefresh();

            // Inventory Updates
            if (InventoryClock == ClockCycle &&
                Inventories.Count > 0)
            {
                if (CheckBlockExists(Inventories[InvQueIndex]))
                {
                    if (InvSearchIndex == 0)
                        Inventories[InvQueIndex].Rotate();

                    if (bSortRunning)
                        Inventories[InvQueIndex].Sort();

                    if (bTallyRunning)
                        Inventories[InvQueIndex].Tally;
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
                Displays[DisplayQueIndex].DisplayUpdate();

                DisplayQueIndex++;
                DisplayQueIndex = (DisplayQueIndex >= Displays.Count) ? 0 : DisplayQueIndex;
            }

            // Clock Updates
            DisplayClock = (DisplayClock == ClockCycle) ? 0 : (DisplayClock + 1);
            InventoryClock = (InventoryClock == ClockCycle) ? 0 : (InventoryClock + 1);
            ProdClock = (ProdClock == ClockCycle) ? 0 : (ProdClock + 1);
        }
        public int RequestIndex()
        {
            ROOT_INDEX++;
            return ROOT_INDEX - 1;
        }

        public Program()
        {
            mySurface = Me.GetSurface(0);
            mySurface.ContentType = ContentType.TEXT_AND_IMAGE;
            mySurface.WriteText("", false);

            ROOT = new RootMeta(Signature, this);

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
                bSetupComplete = BlockListBuilder();
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

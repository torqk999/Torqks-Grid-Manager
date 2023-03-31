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
    #region POWAAA

    /// Power
    /*bool PowerSetup(Resource candidateMonitor)
    {
        foreach (block block in Blocks.Roots)
            block.Priority = -1;

        if (candidateMonitor.Type != ResType.BATTERY)
            return false;

        string[] data = candidateMonitor.TermBlock.CustomData.Split('\n');
        int index = 0;

        bool[] checks = new bool[4]; // rough inset
        /*
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
                farms.AddRange(Resources.FindAll(x => x.Type == ResType.OXYFARM));
                index = PowerPrioritySet(farms.Cast<block>().ToList(), index);
                checks[2] = true;
            }

            if (Contains(nextline, "gen") && !checks[3])
            {
                List<Resource> gens = new List<Resource>();
                gens.AddRange(Resources.FindAll(x => x.Type == ResType.GASGEN));
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

     void InvBuilder(block target)
    {
        if (target is Producer)
        {

            0 = InputHeader
            x = InputCount
            x+1 = OutputHeader


    List<MyInventoryItem> inItems = new List<MyInventoryItem>();
    List<MyInventoryItem> outItems = new List<MyInventoryItem>();

    ((Producer) target).ProdBlock.InputInventory.GetItems(inItems);
            ((Producer) target).ProdBlock.OutputInventory.GetItems(outItems);

            OutputCount = inItems.Count + outItems.Count;

            InventoryListBuilder(inItems, OutputIndex, OutputCount - 1, "|Input|");
    InventoryListBuilder(inItems, OutputIndex - (inItems.Count + 1), OutputCount - 2, "|Output|");
        }

        else if (target is Inventory)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
IMyInventoryOwner owner = ((Inventory)target).Owner;
IMyInventory inventory = owner.GetInventory(0);
inventory.GetItems(items);
            OutputCount = items.Count;

            InventoryListBuilder(items, OutputIndex, OutputCount);
}

        else
AppendLine(StrForm.WARNING, "Invalid Block Type!\n");

AppendLine(StrForm.EMPTY);
    }
    void InventoryListBuilder(List<MyInventoryItem> items, int startIndex, int lineCap, string header = null)
{
if (items.Count < 1)
{
    AppendLine(StrForm.WARNING, "Inventory Empty!");
    return;
}

int count = items.Count;
count -= header == null || startIndex != 0 ? 0 : 1;
startIndex -= header != null && startIndex != 0 ? 1 : 0;

if (header != null)
    AppendLine(StrForm.HEADER, header);

for (int i = startIndex; i < count && (i - startIndex) < lineCap; i++)
    AppendLine(StrForm.INVENTORY, RawListItem(Meta, 0, items[i].Amount, items[i].Type, Program.LIT_DEFS));
}


*/

    #endregion

    #region MOTH-BALLED
    /*
    public void TrashBin(Root root)
    {
        root.RemoveMe();
        if (!(root is block))
            return;

        block block = (block)root;

        if (block is Inventory)
            Inventories.Remove((Inventory)block);

        if (block is Resource)
            Resources.Remove((Resource)block);

        Blocks.Remove(block);
    }
    static bool FilterCompare(Filter A, MyItemType B)
    {
        return FilterCompare(
            A.ItemID[0], A.ItemID[1],
            B.TypeId.Replace("MyObjectBuilder_", ""),
            B.SubtypeId);
    }
    */
    /*static IMyInventory PullInventory(Inventory inventory, bool input = true)
        {
            if (inventory is Producer)
            {
                IMyProductionBlock prodBlock = ((Producer)inventory).ProdBlock;
                return input ? prodBlock.InputInventory : prodBlock.OutputInventory;
            }
            if (inventory != null)
            {
                return inventory.Owner.GetInventory(0);
            }
            return null;
        }*/
    #endregion

    partial class Program : MyGridProgram
    {
        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";
        const string CustomSig = "[CPY]";

        const float CLEAN_MIN = .8f;
        const float FULL_MIN = 0.95f;
        const float EMPTY_MAX = 0.05f;

        const int DefScrollDelay = 1;
        const int DefSigCount = 2;
        const int SEARCH_CAP = 30;
        const int BUILD_CAP = 20;
        static int[] MONO_RATIO = {20, 30};

        static UpdateFrequency INIT_FREQ = UpdateFrequency.Update1;
        //static readonly int[] DefScreenRatio = { 25, 15 };
        /// WARNING!! DO NOT GO FURTHER USER!! ///

        #region DEFAULTS

        const string RefineryDefault =
            "! -in +out\n" +
            "ore: +in -out\n" +
            "&reside\n" +
            "&convey\n" +
            "&fill\n" +
            "&empty";

        #endregion

        #region LOGIC 
        UpdateFrequency RUN_FREQ;

        bool LIT_DEFS = false;
        bool ShowProdBuilding = false;

        bool DETECTED;
        bool BUILT;
        bool SETUP;
        bool SCANNED;
        bool SWEPT;

        static readonly char[] EchoLoop =
        {
            '%',
            '$',
            '#',
            '&'
        };
        const char Split = '/';
        const string Seperator = "/";
        const int EchoMax = 4;

        string[] InputBuffer = new string[2];
        StringBuilder Debug = new StringBuilder();
        StringBuilder EchoBuilder = new StringBuilder();
        IMyTextSurface mySurface;
        RootMeta ROOT;

        int ROOT_INDEX = 0;
        int SEARCH_COUNT = 0;
        int EchoCount = 0;
        int CurrentOp = -1;
        int AuxIndex = 0;

        #endregion

        #region Engine
        List<IMyTerminalBlock> DetectedBlocks = new List<IMyTerminalBlock>();
        List<IMyBlockGroup> DetectedGroups = new List<IMyBlockGroup>();

        List<Production> Productions = new List<Production>();
        List<block> Blocks = new List<block>();
        List<Display> Displays = new List<Display>();
        List<Assembler> Assemblers = new List<Assembler>();
        List<Resource> Resources = new List<Resource>();
        List<Inventory> Inventories = new List<Inventory>();
        List<TallyItemType> AllItemTypes = new List<TallyItemType>();

        List<Inventory> Requesters = new List<Inventory>();
        List<Slot> PumpRequests = new List<Slot>();

        DisplayManager DisplayMan;
        TallyScanner Scanner;
        TallySorter Sorter;
        TallyLinker Linker;
        TallyPumper Pumper;
        ItemBrowser itemBrowser;
        InventoryBrowser inventoryBrowser;
        AssemblyCleaner Cleaner;
        ProductionManager Producing;
        PowerManager Powering;

        List<Op> AllOps;
        List<Op> InactiveOps = new List<Op>();
        List<Op> ActiveOps = new List<Op>();

        void SetupOps()
        {
            DisplayMan = new DisplayManager(this);
            Scanner = new TallyScanner(this);
            Sorter = new TallySorter(this);
            Pumper = new TallyPumper(this);
            Linker = new TallyLinker(this);
            itemBrowser = new ItemBrowser(this);
            inventoryBrowser = new InventoryBrowser(this);
            Cleaner = new AssemblyCleaner(this, false);
            Producing = new ProductionManager(this, false);
            Powering = new PowerManager(this, false);

            AllOps = new List<Op>()
            {
                DisplayMan,
                Scanner,
                Sorter,
                Linker,
                Pumper,
                itemBrowser,
                inventoryBrowser,
                Cleaner,
                Producing,
                Powering,
            };

            InactiveOps.AddRange(AllOps);
        }
        void RunOperations()
        {
            for (int i = InactiveOps.Count - 1; i > -1; i--)
                if (InactiveOps[i].Active && InactiveOps[i].HasWork())
                {
                    ActiveOps.Add(InactiveOps[i]);
                    InactiveOps.Remove(InactiveOps[i]);
                }

            for (int i = ActiveOps.Count - 1; i > -1; i--)
                if (!ActiveOps[i].Active || !ActiveOps[i].HasWork())
                {
                    InactiveOps.Add(ActiveOps[i]);
                    ActiveOps.Remove(ActiveOps[i]);
                }

            if (SCANNED && !Sorter.HasWork())
                SWEPT = true;

            if (!NextCurrentOp())
                return;

            ActiveOps[CurrentOp].Run();
        }
        bool NextCurrentOp()
        {
            if (ActiveOps == null ||
                ActiveOps.Count < 1)
                return false;
            CurrentOp = CurrentOp + 1 >= ActiveOps.Count ? 0 : CurrentOp + 1;
            return true;
        }
        public bool CallCounter()
        {
            SEARCH_COUNT++;
            bool capped = SEARCH_COUNT >= SEARCH_CAP;
            SEARCH_COUNT = capped ? 0 : SEARCH_COUNT;
            return capped;
        }

        #endregion

        #region ENUMS
        public enum StrForm
        {
            EMPTY,
            WARNING,
            TABLE,
            HEADER,
            SUB_HEADER,
            FOOTER,
            INVENTORY,
            RESOURCE,
            STATUS,
            FILTER,
            LINK,
            PRODUCTION
        }
        public enum ScreenMode
        {
            DEFAULT,
            STATUS,
            INVENTORY,
            RESOURCE,
            PRODUCTION,
            SORT,
            TALLY,
            LINK
        }
        public enum Notation
        {
            DEFAULT,
            SCIENTIFIC,
            SIMPLIFIED,
            PERCENT
        }

        public enum TallyGroup
        {
            ALL,
            REQUEST,
            STORAGE,
            LINKABLE,
            AVAILABLE,
            LOCKED,
        }
        public enum TargetType
        {
            DEFAULT,
            BLOCK,
            GROUP,
            GRID_GROUPS,
            GRID_BLOCKS,
            ALL_GROUPS,
            ALL_BLOCKS
        }
        public enum ResType
        {
            NONE,
            POWERGEN,
            GASTANK,
            BATTERY,
            GASGEN,
            OXYFARM
        }
        public enum JobType
        {
            WORK,
            FIND,
            MATCH
        }
        public enum WorkType
        {
            NONE,
            SCAN,
            CHECK,
            SORT,
            LINK,
            PUMP,
            BROWSE
        }
        public enum WorkResult
        {
            ERROR = -3,
            DEAD_SLOT = -2,
            OVERHEAT = -1,

            //NONE = 0,
            NONE_CONTINUE = 0,
            COMPLETE = 1,

            //MATCH_NONE = 0,
            //MATCH_TYPE = 2,
            //MATCH_SUB = 3,
            MATCH_SLOT = 2,

            //FIND_NONE = 0,
            FIND_TYPE = 3,
            FIND_SUB = 4,
            FIND_SLOT = 5,
        }
        #endregion

        #region META
        public class Root
        {
            public string Signature;
            public int RootID;
            public Program Program;
            public Root(RootMeta meta)
            {
                Signature = meta.Signature;
                Program = meta.Program;
                RootID = Program.RequestIndex();
            }
            public virtual void Setup()
            {
                Program.Echo("Root Setup");
            }
            
            public void Dlog(string nextLine)
            {
                Program.Debug.Append($"{nextLine}\n");
            }
        }
        public struct RootMeta
        {
            public string Signature;
            public Program Program;

            public RootMeta(string signature, Program program)
            {
                Signature = signature;
                Program = program;
            }
        }
        public struct BlockMeta
        {
            public RootMeta Root;
            public IMyTerminalBlock Block;
            public bool Detatchable;
            public bool ConsumesPower;
            public int Priority;

            public BlockMeta(RootMeta root, IMyTerminalBlock block = null, bool consumes = false, bool detachable = false, int priority = -1)
            {
                Root = root;
                Block = block;
                ConsumesPower = consumes;
                Detatchable = detachable;
                Priority = priority;
            }
        }
        public struct ProdMeta
        {
            public MyDefinitionId Def;
            public string AssemblerType;
            public Filter Filter;
            public RootMeta Root;

            public ProdMeta(RootMeta root, MyDefinitionId def, string prodIdString, int target = 0)
            {
                Def = def;
                AssemblerType = prodIdString;
                Filter = new Filter(root, def, target);
                Root = root;
            }
        }
        public struct DisplayMeta
        {
            public int SigCount;
            public string TargetName;

            public ScreenMode Mode;
            public Notation Notation;
            public TargetType TargetType;

            public block TargetBlock;
            public IMyCubeGrid TargetGrid;
            public IMyBlockGroup TargetGroup;

            public DisplayMeta(int sigCount)
            {
                SigCount = sigCount;
                TargetName = "No Target";

                Mode = ScreenMode.DEFAULT;
                Notation = Notation.DEFAULT;
                TargetType = TargetType.DEFAULT;

                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }
        public struct JobMeta
        {
            public Root Requester;
            public string TypeCompare;
            public string SubCompare;
            public MyItemType? TypeMatch;
            public Slot SlotMatch;

            public int SearchCount;
            public int WIxA;
            public int WIxB;

            public JobType JobType;
            public WorkType WorkType;
            public WorkResult Success;
            public WorkResult Fail;
            public TallyGroup TargetGroup;

            public JobMeta(
                JobType job = JobType.WORK,
                WorkType work = WorkType.NONE,
                WorkResult success = WorkResult.COMPLETE,
                WorkResult fail = WorkResult.ERROR,
                TallyGroup target = TallyGroup.ALL)
            {
                JobType = job;
                WorkType = work;
                Success = success;
                Fail = fail;
                TargetGroup = target;

                Requester = null;
                TypeCompare = null;
                SubCompare = null;
                TypeMatch = null;
                SlotMatch = null;

                WIxA = 0;
                WIxB = 0;
                SearchCount = 0;
            }

            public void SlotCompare(Slot slot)
            {
                if (slot == null)
                    return;

                Requester = slot;
                TypeCompare = slot.SnapShot.Type.TypeId;
                SubCompare = slot.SnapShot.Type.SubtypeId;
            }

            public void CopyJobMeta(JobMeta source)
            {
                TargetGroup = source.TargetGroup;
                Requester = source.SlotMatch == null ? source.Requester : source.SlotMatch;
                TypeCompare = source.TypeCompare;
                SubCompare = source.SubCompare;
                //SlotMatch = source.SlotMatch;
            }
        }
        #endregion

        #region VIRTUALS
        public class Slot : Root
        {
            public int Index;

            public Inventory Inventory;
            public TallyItemSub Profile;
            public Filter Filter;

            public Slot InLink;
            public Slot OutLink;

            public MyInventoryItem SnapShot;

            public bool SortQueued; // Called by Scanner
            public bool LinkQueued; // Called by Pumper
            public bool BrowseQueued; // Called by Linker

            public Slot(RootMeta meta, Inventory inventory, MyInventoryItem literal, int index) : base(meta)
            {
                Inventory = inventory;
                Index = index;
                SnapShot = literal;
                SlotFilter();
            }
            void SlotFilter()
            {
                if (CheckDead())
                    return;

                MyFixedPoint inTarget, outTarget;

                bool inAllowed = ProfileCompare(Inventory.Profile, SnapShot.Type, out inTarget);
                bool outAllowed = ProfileCompare(Inventory.Profile, SnapShot.Type, out outTarget, false);
                bool wasPumping = Filter == null ? false : Filter.IsPumping();

                Filter = new Filter(Program.ROOT, SnapShot.Type,
                    outTarget < inTarget ? outTarget : inTarget,  // Prioritize smaller value
                    inAllowed ? (Inventory.Profile.FILL ? 1 : 0) : -1,
                    outAllowed ? (Inventory.Profile.EMPTY ? 1 : 0) : -1);

                if (!wasPumping && Filter.IsPumping())
                    Program.PumpRequests.Add(this);

                if (wasPumping && !Filter.IsPumping())
                    Program.PumpRequests.Remove(this);
            }
            bool ProfileUpdate()
            {
                if (Profile != null)
                {
                    MyFixedPoint delta;

                    if (!CheckSlot(out delta))
                    {
                        UnsyncFromProfile(delta);
                        SlotFilter();
                        return false;
                    }

                    else
                    {
                        Profile.Update(delta);
                        return true;
                    }
                }
                return false;
            }

            public void Update()
            {
                if (SortQueued)
                    return;

                if (!ProfileUpdate())
                    QueueSort();
            }
            void QueueSort()
            {
                Program.Sorter.Queue.Add(this);
                SortQueued = true;
            }
            public bool Pump()
            {
                if (CheckBroken())
                    return false;

                bool satisfied = true;

                if (Filter.IN == 1)
                {
                    if (InLink == null)
                    {
                        satisfied = false;
                    }
                    else if (InLink.CheckBroken())
                    {
                        InLink = null;
                        satisfied = false;
                    }
                    else if (!TallyTransfer(InLink, this))
                    {
                        satisfied = false;
                    }
                    else
                    {

                    }
                }

                if (Filter.OUT == 1)
                {
                    if (OutLink == null)
                    {
                        satisfied = false;
                    }

                    else if (OutLink.Inventory == null)//.DEAD)
                    {
                        OutLink = null;
                        satisfied = false;
                    }

                    else if (!TallyTransfer(this, OutLink))
                    {
                        satisfied = false;
                    }
                    else
                    {

                    }
                }

                return satisfied;
            }
            public bool CheckSlot(out MyFixedPoint delta)
            {
                delta = SnapShot.Amount;
                if (Inventory == null)
                {

                    return false;
                }

                MyInventoryItem? check = Inventory.PullIndex(Index);
                if (!check.HasValue)
                {

                    return false;
                }

                if (check.Value.Type != SnapShot.Type)
                {

                    SnapShot = check.Value;
                    return false;
                }

                delta = check.Value.Amount - SnapShot.Amount;
                SnapShot = check.Value;
                return true;
            }
            public bool CheckUnLinked()
            {
                if (Filter.IN == 1 && InLink == null)
                    return true;

                if (Filter.OUT == 1 && OutLink == null)
                    return true;

                return false;
            }
            public bool CheckDead()
            {
                return Inventory == null;
            }
            public bool CheckBroken()
            {
                return Inventory == null || Profile == null;
            }
            public void SyncTallyToProfile(TallyItemSub profile)
            {
                if (profile == null)
                    return;

                Profile = profile;

                Profile.Tallies[(int)TallyGroup.ALL].Add(this);

                if (Filter.IN == -1 && Filter.OUT == -1)
                    Profile.Tallies[(int)TallyGroup.LOCKED].Add(this);
                if (Filter.IN == 0 && Filter.OUT == 0)
                    Profile.Tallies[(int)TallyGroup.STORAGE].Add(this);
                if (Filter.IN == 1 || Filter.OUT == 1)
                {
                    if (Inventory is Producer)
                        Profile.Tallies[(int)TallyGroup.LINKABLE].Add(this);
                    else
                        Profile.Tallies[(int)TallyGroup.REQUEST].Add(this);
                }

                if (Filter.IN > -1 || Filter.OUT > -1)
                    Profile.Tallies[(int)TallyGroup.AVAILABLE].Add(this);
            }
            private void UnsyncFromProfile(MyFixedPoint remaining)
            {
                if (Profile == null)
                    return;

                Profile.Tallies[(int)TallyGroup.ALL].Remove(this);

                if (Filter.IN == -1 && Filter.OUT == -1)
                    Profile.Tallies[(int)TallyGroup.LOCKED].Remove(this);
                if (Filter.IN == 0 && Filter.OUT == 0)
                    Profile.Tallies[(int)TallyGroup.STORAGE].Remove(this);
                if (Filter.IN == 1 || Filter.OUT == 1)
                    Profile.Tallies[(int)TallyGroup.REQUEST].Remove(this);
                if (Filter.IN > -1 || Filter.OUT > -1)
                    Profile.Tallies[(int)TallyGroup.AVAILABLE].Remove(this);

                Profile.Update(-remaining);
                Profile = null;
            }

            public void Kill()
            {
                UnsyncFromProfile(SnapShot.Amount);
                Inventory.Slots.Remove(this);
                Inventory = null;
            }
        }
        public class Filter : Root
        {
            public string[] ItemID = new string[2];
            public MyFixedPoint Target;
            public int IN;
            public int OUT;

            public bool IsPumping()
            {
                return IN == 1 || OUT == 1;
            }
            Filter(RootMeta meta, MyFixedPoint? target = null, int @in = 0, int @out = 0) : base(meta)
            {
                Target = target.HasValue ? target.Value : 0;
                IN = @in;
                OUT = @out;
            }
            public Filter(RootMeta meta, string combo, MyFixedPoint? target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
            {
                GenerateFilters(combo, ref ItemID);
            }
            public Filter(RootMeta meta, MyItemType type, MyFixedPoint? target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
            {
                GenerateFilters(type, ref ItemID);
            }
            public Filter(RootMeta meta, MyDefinitionId id, MyFixedPoint? target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
            {
                GenerateFilters(id, ref ItemID);
            }
        }
        public class FilterProfile : Root
        {
            public List<Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;
            public bool RESIDE;
            public bool CLEAN;
            public bool ACTIVE_CONVEYOR;

            public FilterProfile(RootMeta meta, bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false) : base(meta)
            {
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

                string[] data = customData.Split('\n');

                foreach (string nextline in data)
                {
                    if (nextline.Length == 0)
                        continue;

                    /// OPTION CHANGE ///

                    if (nextline[0] == '&')
                    {
                        if (Contains(nextline, "empty"))
                            EMPTY = !nextline.Contains("-");

                        if (Contains(nextline, "fill"))
                            FILL = !nextline.Contains("-");

                        if (Contains(nextline, "res"))
                            RESIDE = !nextline.Contains("-");

                        if (Contains(nextline, "clean"))
                            CLEAN = !nextline.Contains("-");

                        if (Contains(nextline, "convey"))
                            ACTIVE_CONVEYOR = !nextline.Contains("-");

                        continue;
                    }

                    /// FILTER CHANGE ///
                    AppendFilter(nextline);
                }

                Filters.RemoveAll(x => x.ItemID[0] == "any" && x.ItemID[1] == "any");  // Redundant. Refer to inventory default mode

                return true;
            }
            public void AppendFilter(string nextline)
            {
                /// FILTER CHANGE ///

                string[] lineblocks = nextline.Split(' ');  // Break each line into blocks

                if (lineblocks.Length < 2)  // There must be more than one block to have filter candidate and desired update
                    return;

                string itemID = "null"; // Default target, don't update any filters
                bool bDefault = false;
                bool bIn = true;
                bool bOut = true;
                MyFixedPoint? target = null;

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
                    Filters.Add(new Filter(Program.ROOT, itemID, target, bIn ? FILL ? 1 : 0 : -1, bOut ? EMPTY ? 1 : 0 : -1));
                }

            }
        }
        public class TallyItemType : Root
        {
            public List<TallyItemSub> SubTypes = new List<TallyItemSub>();
            public string TypeId;

            public TallyItemType(RootMeta meta, TallyItemSub first) : base(meta)
            {
                SubTypes.Add(first);
                TypeId = first.Type.TypeId;
            }
        }
        public class TallyItemSub : Root
        {
            public MyFixedPoint? TargetGoal;
            public MyFixedPoint CurrentTotal;
            public MyFixedPoint HighestReached;
            public MyItemType Type;

            public List<Slot>[] Tallies = new List<Slot>[Enum.GetNames(typeof(TallyGroup)).Length];
            public TallyItemSub(RootMeta meta, Slot first) : base(meta)
            {
                for (int i = 0; i < Tallies.Length; i++)
                    Tallies[i] = new List<Slot>();

                if (first == null)
                    return;

                Type = first.SnapShot.Type;
                Append(first);
            }

            public Slot GetSlot(TallyGroup group, int index)
            {
                try { return Tallies[(int)group][index]; }
                catch { return null; }
            }

            public void Update(MyFixedPoint change)
            {
                CurrentTotal += change;
                CurrentTotal = CurrentTotal > 0 ? CurrentTotal : 0;
                HighestReached = CurrentTotal > HighestReached ? CurrentTotal : HighestReached;
            }
            public void Append(Slot newTally)
            {
                newTally.SyncTallyToProfile(this);
                Update(newTally.SnapShot.Amount);
            }
        }
        public class Production : Root
        {
            public MyFixedPoint Current;
            public MyDefinitionId Def;
            public string AssemblerType;
            public Filter Filter;

            public Production(ProdMeta meta) : base(meta.Root)
            {
                Current = 0;
                Def = meta.Def;
                AssemblerType = meta.AssemblerType;
                Filter = meta.Filter;
            }

            public bool Update()
            {

                // OVERHAUL PRODUCTION! REMOVE FOREACH LOOPS! //

                if (Current >= Filter.Target)
                {
                    // Add excess que removal logic here later
                    return true;
                }

                List<Assembler> candidates = new List<Assembler>();
                candidates.AddRange(Program.Assemblers);
                List<MyProductionItem> existingQues = new List<MyProductionItem>();

                foreach (Assembler producer in candidates)
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
                MyFixedPoint projectedOverage = Filter.Target - projectedTotal;

                if (projectedOverage >= 0)
                {
                    MyFixedPoint remove = new MyFixedPoint();

                    foreach (Assembler producer in candidates)
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

                    foreach (Assembler producer in candidates)                                  // Distribute
                        producer.ProdBlock.AddQueueItem(Def, qeueIndividual);
                }
                return true;
            }
        }

        public class Work : Root
        {
            public Op MyOp;
            public Work Chain;
            public JobMeta Job;
            public MyFixedPoint DestinationTarget;
            public string Name;
            public int SIx = 0;

            public Work(RootMeta meta, Op op) : base(meta)
            {
                MyOp = op;
                //ACTIVE = true;
            }
            public WorkResult WorkLoad()
            {
                Dlog($"Performing: {Name} {Job.JobType}");

                MyOp.CurrentWork = this;

                return IterateScope();
            }

            public virtual void SetSearchCount()
            {

            }
            public virtual bool Compare()
            {
                return false;
            }
            public virtual bool Match() { return false; }
            public virtual bool DoWork()
            {
                return false; // Task/Search Proced
            }
            public virtual WorkResult ChainCall()
            {
                return WorkResult.NONE_CONTINUE;
            }

            WorkResult IterateScope(bool forward = true)
            {
                WorkResult result = WorkResult.NONE_CONTINUE;
                for (int i = SIx; i < Job.SearchCount; i++)
                {
                    SIx = i;
                    result = forward ? Peek(i) : Peek(Job.SearchCount - (i + 1));

                    if (result == WorkResult.OVERHEAT)
                    {
                        break;
                    }

                    if ((Job.JobType == JobType.FIND || Job.JobType == JobType.MATCH) && result > 0)
                    {
                        break;
                    }

                    if (Job.JobType == JobType.WORK && result == WorkResult.COMPLETE)
                    {
                        break;
                    }
                }

                if (Job.JobType == JobType.MATCH && Chain != null)
                {
                    //SIx = 0; // Special Condition?

                    //if (result == WorkResult.NONE_CONTINUE && Chain != null)

                    //if (result > 0)
                    //{
                        return ChainCall();
                    //}
                }

                Dlog($"Peek Result: {result}");
                return result;
            }
            WorkResult Peek(int i)
            {
                Dlog("Peeking...");

                if (CheckOp())
                    return WorkResult.OVERHEAT;

                if (DeadSlot())
                    return WorkResult.DEAD_SLOT;

                switch (Job.JobType)
                {
                    case JobType.FIND:
                        Dlog("Finding...");
                        if (Compare())
                        {
                            Dlog("Found!");
                            if (Chain != null)
                            {
                                Dlog("ChainCall!");
                                WorkResult chainResult = ChainCall();
                                if (chainResult != WorkResult.NONE_CONTINUE)
                                {
                                    if (Chain.Job.JobType == JobType.MATCH && chainResult == WorkResult.MATCH_SLOT)
                                    {
                                        Dlog($"Chain Match! Keep searching...");
                                        return Job.Fail; // Special condition?
                                    }

                                    Dlog($"ChainResult: {chainResult}");
                                    return chainResult;
                                }
                            }

                            return Job.Success;
                        }
                        return Job.Fail;

                    case JobType.MATCH:
                        Dlog("Matching...");
                        return Match() ? Job.Success : Job.Fail;

                    case JobType.WORK:
                        Dlog("Doing work...");
                        return DoWork() ? Job.Success : Job.Fail;

                    default:
                        return WorkResult.ERROR;
                }
            }
            bool DeadSlot()
            {
                return Job.Requester is Slot && ((Slot)Job.Requester).Inventory == null;
            }
            bool CheckOp()
            {
                bool maxed = Program.CallCounter();
                Dlog($"Calls Maxed: {maxed}");
                return maxed;
            }
        }
        public class InvWork : Work
        {
            List<Inventory> InventoryList;
            public InvWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "INVENTORY";
            }

            public override void SetSearchCount()
            {
                InventoryList = null;

                switch (Job.WorkType)
                {
                    case WorkType.BROWSE:
                        InventoryList = Program.Inventories;
                        break;
                }

                if (InventoryList == null)
                {
                    Dlog("Null InventoryList!");
                    Job.SearchCount = 0;
                    return;
                }

                Job.SearchCount = InventoryList.Count;
            }
            public override bool Compare()
            {
                Dlog($"Checking inventory: {InventoryList[SIx].CustomName}");

                return Job.Requester is Slot && ProfileCompare(InventoryList[SIx], (Slot)Job.Requester, out DestinationTarget);
            }
            /*public override bool DoWork()
            {
                return base.DoWork();
            }*/
            /*public override WorkResult ChainCall()
            {
                return base.ChainCall();
            }*/

            public Inventory Current()
            {
                return InventoryList == null ? null : InventoryList[SIx];
            }
        }
        public class ProdWork : Work
        {
            public ProdWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "PRODUCTION";
            }
        }
        public class TypeWork : Work
        {
            public TypeWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "TYPE";
            }

            public override void SetSearchCount()
            {
                Job.SearchCount = Program.AllItemTypes.Count;
            }
            public override bool Compare()
            {
                TallyItemType type = Program.AllItemTypes[SIx];
                Dlog($"Comparing type: {Job.TypeCompare} || {type.TypeId}");

                return (Job.TypeCompare != null && (Job.TypeCompare == "any" || type.TypeId == Job.TypeCompare)) ||
                    (Job.Requester is Inventory && ProfileCompare(((Inventory)Job.Requester).Profile, type.TypeId, out DestinationTarget, true));
            }
            /*public override bool DoWork()
            {
                return true; // ?
            }*/
            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                Chain.Job.WIxA = SIx;
                Chain.SetSearchCount();

                //Dlog($"raw compares: {Chain.Job.TypeCompare != null}/{Chain.Job.SubCompare != null} ");

                //Dlog($"Sub searchCount: {Chain.Job.SearchCount}");

                return Chain.WorkLoad();// WorkLoad(ChainJob);
            }
        }
        public class SubWork : Work
        {

            public SubWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SUBTYPE";
            }

            TallyItemSub Current()
            {
                return Program.AllItemTypes[Job.WIxA].SubTypes[SIx];
            }
            public override void SetSearchCount()
            {
                Job.SearchCount = Program.AllItemTypes[Job.WIxA].SubTypes.Count;
            }
            public override bool Compare()
            {
                TallyItemSub sub = Program.AllItemTypes[Job.WIxA].SubTypes[SIx];
                Dlog($"Comparing sub: {Job.SubCompare} || {sub.Type.SubtypeId}");

                return (Job.SubCompare != null && (Job.SubCompare == "any" || sub.Type.SubtypeId == Job.SubCompare)) ||
                    (Job.Requester is Inventory && ProfileCompare(((Inventory)Job.Requester).Profile, out DestinationTarget, sub.Type.SubtypeId, true));
            }
            /*public override bool DoWork()
            {
                return true; // ?
            }*/
            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);

                if (Chain.Job.JobType == JobType.MATCH)
                    Chain.Job.TypeMatch = Current().Type;

                Chain.Job.WIxA = Job.WIxA;
                Chain.Job.WIxB = SIx;
                Chain.SetSearchCount();

                Dlog($"raw compares: {Chain.Job.TypeCompare != null}/{Chain.Job.SubCompare != null}");

                return Chain.WorkLoad();
            }
        }
        public class SlotWork : Work
        {
            public List<Slot> SlotList;
            public SlotWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SLOT";
            }

            public override void SetSearchCount()
            {
                SlotList = null;
                Inventory inv = null;

                switch (Job.WorkType)
                {
                    case WorkType.CHECK:
                    case WorkType.SCAN:
                        if (Job.Requester is Inventory)
                        {
                            inv = (Inventory)Job.Requester;
                            SlotList = inv.Slots;
                        }
                        break;

                    case WorkType.LINK:
                        if (Job.Requester is Slot && ((Slot)Job.Requester).Profile != null)
                        {
                            SlotList = ((Slot)Job.Requester).Profile.Tallies[(int)Job.TargetGroup];
                        }
                        break;

                    case WorkType.PUMP:
                        SlotList = Program.PumpRequests;
                        break;

                    case WorkType.BROWSE:
                        SlotList = Program.AllItemTypes[Job.WIxA].SubTypes[Job.WIxB].Tallies[(int)Job.TargetGroup];
                        break;
                }

                if (SlotList == null)
                {
                    Dlog("Null SlotList!");
                    Job.SearchCount = 0;
                    return;
                }

                if (inv != null)
                    Job.SearchCount = inv.Slots.Count > inv.ItemCount() ? inv.Slots.Count : inv.ItemCount();

                else
                    Job.SearchCount = SlotList.Count;

                Dlog($"SearchCount: {Job.SearchCount}");
            }
            public override bool Compare()
            {
                Dlog($"Checking slot: {SlotList[SIx].SnapShot.Type.SubtypeId}");

                return (

                    /*(Job.SlotMatch != null && SlotCompare((Job.SlotMatch

                    ||*/

                    (Job.Requester is Inventory && ProfileCompare((Inventory)Job.Requester, SlotList[SIx], out DestinationTarget))

                    ||

                    (Job.Requester is Slot && SlotCompare((Slot)Job.Requester, SlotList[SIx]))

                    );
            }
            public override bool Match()
            {
                Dlog($"Matching slot: {SlotList[SIx].SnapShot.Type.SubtypeId}");

                Job.SlotMatch = Job.TypeMatch == SlotList[SIx].SnapShot.Type ? SlotList[SIx] : null;
                return Job.SlotMatch != null;
            }

            public override bool DoWork()
            {
                if (SlotList == null)
                    return true;

                switch (Job.WorkType)
                {
                    case WorkType.SCAN:
                        return SlotScan();

                    case WorkType.PUMP:
                        return SlotPump();

                    case WorkType.LINK:
                        return SlotLink();

                    //case WorkType.PULL:
                    //return SlotPull();

                    default:
                        return false;
                }
            }
            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                Chain.Job.WIxA = Job.WIxA;
                Chain.Job.WIxB = Job.WIxB; // Unique operation chain, may need re-factoring
                Chain.SetSearchCount();

                Dlog("SlotWork ChainCall to SlotWork");

                return Chain.WorkLoad();
            }


            public Slot Current()
            {
                return SlotList == null ? null : SlotList[SIx];
            }
            bool SlotLink()
            {
                Slot queued = (Slot)Job.Requester;
                Slot match = SlotList[SIx];

                if (queued == null)
                {
                    Dlog("null Queued!");
                    return true;
                }

                if (queued.Profile == null)
                {
                    Dlog("null Queued Profile!");
                    return true;
                }

                if (match == null)
                {
                    Dlog("null Match!");
                    return false;
                }

                if (queued == match)
                {
                    Dlog("Looking at self!");
                    return false;
                }

                //if ()

                Dlog($"Queued Tally for linking: {queued.SnapShot.Type.SubtypeId}");

                if (queued.Filter.IN == 1 &&
                    queued.InLink == null &&
                    match.Filter.OUT > -1 &&
                    (match.Inventory.Profile.RESIDE ||
                        !match.Inventory.IsEmpty()))
                {
                    Dlog($"In Link Made!");

                    queued.InLink = match;
                    match.OutLink = queued;
                }

                if (queued.Filter.OUT == 1 &&
                    queued.OutLink == null &&
                    match.Filter.IN > -1 &&
                    (match.Inventory.Profile.RESIDE ||
                        !match.Inventory.IsFull()))
                {
                    Dlog($"Out Link Made!");

                    queued.OutLink = match;
                    match.InLink = queued;
                }

                return !queued.CheckUnLinked();
            }
            bool SlotScan()
            {
                Inventory inv = (Inventory)Job.Requester;

                if (SIx >= inv.Slots.Count) // Not enough slots
                {
                    MyInventoryItem? check = inv.PullIndex(SIx);
                    if (!check.HasValue)
                    {
                        Dlog("Bad item lookup!");
                        return true;
                    }

                    Dlog("Adding slot");
                    Slot sloot = new Slot(Program.ROOT, inv, (MyInventoryItem)check, SIx);

                    //if (sloot.Filter.IN == 1 || sloot.Filter.OUT == 1)
                    //    Program.PumpRequests.Add(sloot);

                    inv.Slots.Add(sloot);
                }

                if (SIx >= inv.ItemCount()) // Too many slots
                {
                    Dlog("Killing slot");
                    inv.Slots[inv.Slots.Count - 1].Kill();
                }

                else
                {
                    Dlog("Updating slot");
                    inv.Slots[SIx].Update();
                }

                Dlog("Slot Scanned!");
                return false; // Keep Working!
            }
            bool SlotPump()
            {
                if (SlotList[SIx].Inventory == null)
                {
                    Dlog("Dead Slot, removing from pump requesters...");

                    SlotList.RemoveAt(SIx);
                    Job.SearchCount = SlotList.Count;
                    SIx--;
                    return false;
                }

                Dlog("Pumping...");

                if (!SlotList[SIx].Pump() && !SlotList[SIx].LinkQueued)
                {
                    Dlog("Requesting Links!");
                    SlotList[SIx].LinkQueued = true;
                    Program.Linker.Queue.Add(SlotList[SIx]);
                }

                return false; // Keep Working!
            }
        }


        public class Op : Root
        {
            public string Name;
            public bool Active;
            //public bool Swept;
            public Work CurrentWork;

            public int
                WorkIndex,
                WorkCount,
                WorkTotal;

            public Op(Program prog, bool active) : base(prog.ROOT)
            {
                Active = active;
                //Swept = false;
            }

            public void Toggle()
            {
                Active = !Active;
            }
            public virtual void Run() { Dlog($"Running:{Name}"); }
            public virtual bool HasWork() { return false; }

            public bool Next()
            {
                WorkIndex++;
                WorkTotal++;
                bool result = WorkIndex >= WorkCount;
                WorkIndex = result ? 0 : WorkIndex;
                return result;
            }
            public void Reset()
            {
                WorkIndex = 0;
                WorkCount = 0;
                WorkTotal = 0;
            }
        }

        public class QueueOp : Op
        {
            public List<Slot> Queue = new List<Slot>();

            public QueueOp(Program prog, bool active) : base(prog, active)
            {
            }

            public override bool HasWork()
            {
                WorkCount = Queue.Count;
                return WorkCount > 0 ;
            }
        }
        public class TallySorter : QueueOp
        {
            public TypeWork TypeMatch;
            public SubWork SubMatch;
            public TallySorter(Program prog, bool active = true) : base(prog, active)
            {
                Name = "SORT";

                TypeMatch = new TypeWork(prog.ROOT, this);
                TypeMatch.Job = new JobMeta(JobType.FIND, WorkType.SORT, WorkResult.FIND_TYPE, WorkResult.NONE_CONTINUE);

                SubMatch = new SubWork(prog.ROOT, this);
                SubMatch.Job = new JobMeta(JobType.FIND, WorkType.SORT, WorkResult.FIND_SUB, WorkResult.NONE_CONTINUE);
                TypeMatch.Chain = SubMatch;
            }

            public override void Run()
            {
                base.Run();
                Dlog($"Sorts Remaining: {WorkCount}");

                TypeMatch.Job.SlotCompare(Queue[0]);
                TypeMatch.SetSearchCount();

                if (Sort())
                {
                    TypeMatch.SIx = 0;
                    SubMatch.SIx = 0;
                    Queue[0].SortQueued = false;
                    Queue.RemoveAt(0);
                }
            }
            bool Sort()
            {
                Dlog($"Processing top of sort queue: {Queue[0].SnapShot.Type}");
                TallyItemType newType;
                TallyItemSub newProf;

                switch (TypeMatch.WorkLoad())
                {
                    case WorkResult.DEAD_SLOT:
                        Dlog("DEAD!");
                        return true;

                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE:
                        Dlog("Not found!\nNew Type/Sub/Slot");
                        newProf = new TallyItemSub(Program.ROOT, Queue[0]);
                        newType = new TallyItemType(Program.ROOT, newProf);
                        Program.AllItemTypes.Add(newType);
                        return true;

                    case WorkResult.FIND_TYPE:
                        Dlog("Type Found!\nNew Sub/Slot");
                        newProf = new TallyItemSub(Program.ROOT, Queue[0]);
                        Program.AllItemTypes[TypeMatch.SIx].SubTypes.Add(newProf);
                        return true;

                    case WorkResult.FIND_SUB:
                        Dlog("Sub Type Found!\nNew Slot");
                        Program.AllItemTypes[TypeMatch.SIx].SubTypes[SubMatch.SIx].Append(Queue[0]);
                        return true;

                    default:
                        return true;
                }
            }
        }
        public class TallyLinker : QueueOp
        {
            public SlotWork SlotLink;

            public TallyLinker(Program prog, bool active = true) : base(prog, active)
            {
                Name = "LINK";

                SlotLink = new SlotWork(prog.ROOT, this);
                SlotLink.Job = new JobMeta(JobType.WORK, WorkType.LINK, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE, TallyGroup.LINKABLE);
            }
            public override void Run()
            {
                base.Run();

                SlotLink.Job.Requester = Queue[0];
                SlotLink.SetSearchCount();

                if (Link())
                {
                    Queue[0].LinkQueued = false;
                    Queue.RemoveAt(0);
                }
            }
            bool Link()
            {
                Dlog($"Linking TallySlot: {Queue[0].SnapShot.Type}");

                switch (SlotLink.WorkLoad())
                {
                    case WorkResult.DEAD_SLOT:
                        Dlog("DEAD!");
                        return true;

                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE: // Didn't find any links, send to inventory browser

                        if (Queue[0].OutLink == null && Queue[0].Filter.OUT == 1 && !Queue[0].BrowseQueued)
                        {
                            Dlog("Adding to browse queue...");
                            Program.inventoryBrowser.Queue.Add(Queue[0]);
                            Queue[0].BrowseQueued = true;
                        }

                        return true;

                    case WorkResult.COMPLETE:
                        Dlog("Links fullfilled!");
                        return true;

                    default: // Clear bad event
                        return true;
                }
            }

            /*
            bool DumpUnwanted()
            {
                Dlog("Dumping...");

                Slot queued = Queue[0];
                SearchCount[1] = Program.Inventories.Count;
                SearchIndex[1] = SearchIndex[1] < SearchCount[1] ? SearchIndex[1] : SearchCount[1] - 1;

                for (int i = SearchIndex[1]; i > SearchIndex[1] - SR[1] && i > -1; i--)
                {
                    Dlog($"Checking Inventory: {Program.Inventories[i].CustomName}");

                    if (queued.Inventory == Program.Inventories[i]) // Skip own inventory
                        continue;

                    Inventory inventory = Program.Inventories[i];
                    MyFixedPoint target = 0;

                    if (queued.OUT == 1 &&
                        queued.Inventory.EmptyCheck() &&
                        ProfileCompare(inventory.Profile, queued.NEW.Type, out target) &&
                        ForceTransfer(inventory, queued.Inventory, queued.NEW,
                        AllowableReturn(target, queued.MyTarget, queued)))
                    {
                        SearchIndex[1] = SearchCount[1] - 1;
                        return true;
                    }
                }

                if (Reverse(1))
                    return true; // found nothing

                return false;
            }*/

        }
        public class InventoryBrowser : QueueOp
        {

            public InvWork InventoryCheck;
            public InventoryBrowser(Program prog, bool active = true) : base(prog, active)
            {
                InventoryCheck = new InvWork(prog.ROOT, this);
                InventoryCheck.Job = new JobMeta(JobType.FIND, WorkType.BROWSE, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE);

                Name = "INVENTORY BROWSE";
            }

            public override bool HasWork()
            {
                return base.HasWork() && Program.SWEPT;
            }

            public override void Run()
            {
                base.Run();

                InventoryCheck.Job.Requester = Queue[0];
                InventoryCheck.SetSearchCount();

                if (Browse())
                {
                    Queue[0].BrowseQueued = false;
                    Queue.RemoveAt(0);
                }
            }

            public bool Browse()
            {
                Slot browser = Queue[0];

                switch (InventoryCheck.WorkLoad())
                {
                    case WorkResult.DEAD_SLOT:
                        Dlog("DEAD!");
                        return true;

                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.COMPLETE:
                        Dlog("Eligable Inventory Found!");
                        Inventory target = InventoryCheck.Current();
                        target.Pulled = ForceTransfer(target, InventoryCheck.DestinationTarget, browser);
                        Dlog($"Transfer success: {target.Pulled}");
                        return true;

                    default:
                        return true;
                }
            }
        }

        public class DisplayManager : Op
        {
            public DisplayManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "DISPLAY";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Displays.Count;
                return WorkCount > 0;
            }

            public override void Run()
            {
                base.Run();

                if (Program.Displays[WorkIndex].Update())
                    Next();
            }
        }
        public class TallyScanner : Op
        {
            public SlotWork TallyScope;
            public TallyScanner(Program prog, bool active = true) : base(prog, active)
            {
                Name = "SCAN";

                TallyScope = new SlotWork(prog.ROOT, this);
                TallyScope.Job = new JobMeta(JobType.WORK, WorkType.SCAN, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE);
            }

            public override bool HasWork()
            {
                WorkCount = Program.Inventories.Count;
                return WorkCount > 0;
            }
            public override void Run()
            {
                base.Run();

                Inventory working = Program.Inventories[WorkIndex];
                TallyScope.Job.Requester = working;
                TallyScope.SetSearchCount();

                if (Scan())
                {
                    working.Pulled = false;
                    TallyScope.SIx = 0;
                    if (Next())
                        Program.SCANNED = true;
                }
            }

            bool Scan()
            {
                Dlog($"Scanning Inventory: {Program.Inventories[WorkIndex].CustomName}");

                switch (TallyScope.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.COMPLETE:
                        //case 1:
                        Dlog("Scan Complete!");
                        return true;

                    default:
                        return true; // Clear bad scan
                }
            }
        }
        public class TallyPumper : Op
        {
            public SlotWork SlotPump;

            public TallyPumper(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PUMP";

                SlotPump = new SlotWork(prog.ROOT, this);
                SlotPump.Job = new JobMeta(JobType.WORK, WorkType.PUMP, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE);
            }

            public override bool HasWork()
            {
                WorkCount = Program.PumpRequests.Count;
                return Program.PumpRequests.Count > 0;
            }
            public override void Run()
            {
                base.Run();

                SlotPump.SetSearchCount();

                if (Pumping())
                {
                    SlotPump.SIx = 0;
                }
            }
            bool Pumping()
            {
                switch (SlotPump.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.COMPLETE:
                        Dlog("Pump workload complete!");
                        return true;

                    default:
                        return true;
                }
            }
        }
        public class ItemBrowser : Op
        {
            public TypeWork TypeFind;
            public SubWork SubFind;
            public SlotWork SlotMatch;
            public SlotWork SlotFind;

            public ItemBrowser(Program prog, bool active = true) : base(prog, active)
            {
                TypeFind = new TypeWork(prog.ROOT, this);
                TypeFind.Job = new JobMeta(JobType.FIND, WorkType.NONE, WorkResult.FIND_TYPE, WorkResult.NONE_CONTINUE);

                SubFind = new SubWork(prog.ROOT, this);
                SubFind.Job = new JobMeta(JobType.FIND, WorkType.NONE, WorkResult.FIND_SUB, WorkResult.NONE_CONTINUE);
                TypeFind.Chain = SubFind;

                SlotMatch = new SlotWork(prog.ROOT, this);
                SlotMatch.Job = new JobMeta(JobType.MATCH, WorkType.CHECK, WorkResult.MATCH_SLOT, WorkResult.NONE_CONTINUE);
                SubFind.Chain = SlotMatch;

                SlotFind = new SlotWork(prog.ROOT, this);
                SlotFind.Job = new JobMeta(JobType.FIND, WorkType.BROWSE, WorkResult.FIND_SLOT, WorkResult.NONE_CONTINUE, TallyGroup.AVAILABLE);
                SlotMatch.Chain = SlotFind;

                Name = "ITEM BROWSE";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Requesters.Count;
                return WorkCount > 0;
            }

            public override void Run()
            {
                base.Run();

                Dlog($"{Program.Requesters[WorkIndex].CustomName} is Browsing...");
                TypeFind.Job.Requester = Program.Requesters[WorkIndex];
                TypeFind.SetSearchCount();

                if (Browse())
                {
                    TypeFind.SIx = 0;
                    SubFind.SIx = 0;
                    SlotMatch.SIx = 0;
                    SlotFind.SIx = 0;
                    Next();
                }
            }

            bool Browse()
            {
                Inventory browser = Program.Requesters[WorkIndex];
                if (browser.Pulled)
                {
                    Dlog("Needs re-scan!");
                    return true;
                }

                switch (TypeFind.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE:
                        Dlog("Nothing Found!");
                        return true;

                    case WorkResult.FIND_TYPE:
                        Dlog("Type-only Found!");
                        return true;

                    case WorkResult.FIND_SUB:
                        Dlog("Sub-only Found!");
                        return true;

                    case WorkResult.MATCH_SLOT:
                        Dlog("Slot-match-only present!");
                        return true;

                    case WorkResult.FIND_SLOT:
                        Dlog("Eligable Slot Found!");
                        browser.Pulled =
                            SlotFind.Job.Requester == browser ? ForceTransfer(browser, SlotFind.DestinationTarget, SlotFind.Current()) :
                            (SlotFind.Job.Requester is Slot) ? TallyTransfer((Slot)SlotFind.Job.Requester, SlotFind.Current()) : false;
                        Dlog($"Transfer success: {browser.Pulled}");
                        return true;

                    default:
                        return true;
                }
            }
        }
        /*public class SlotBrowser : Op
        {

        }*/
        public class AssemblyCleaner : Op
        {
            public AssemblyCleaner(Program prog, bool active) : base(prog, active)
            {
                Name = "CLEAN";
            }
        }
        public class ProductionManager : Op
        {
            public ProdWork ProdCheck;
            public ProductionManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PRODUCE";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Productions.Count;
                //WorkCount[1] = Program.Assemblers.Count;
                return WorkCount > 0;//&& WorkCount[1] > 0;
            }

            public override void Run()
            {
                base.Run();
            }
        }
        public class PowerManager : Op
        {
            public PowerManager(Program prog, bool active) : base(prog, active)
            {
                Name = "POWER";
            }
        }
        #endregion

        #region BLOCK WRAPPERS
        public class block : Root
        {
            public long BlockID;
            public string LastData;
            public string CustomName;

            public int Priority;

            public bool Detatchable;
            public bool ConsumesPower;
            public IMyTerminalBlock TermBlock;
            public FilterProfile Profile;

            public block(BlockMeta bMeta) : base(bMeta.Root)
            {
                TermBlock = bMeta.Block;
                LastData = TermBlock.CustomData;
                Detatchable = bMeta.Detatchable;
                ConsumesPower = bMeta.ConsumesPower;
                CustomName = TermBlock.CustomName.Replace(bMeta.Root.Signature, "");
                BlockID = TermBlock.EntityId;
                Priority = -1;
                Profile = new FilterProfile(bMeta.Root);
            }

            public bool CheckBlockExists()
            {
                IMyTerminalBlock maybeMe = Program.GridTerminalSystem.GetBlockWithId(TermBlock.EntityId); // BlockID?
                if (maybeMe != null &&                          // Exists?
                    maybeMe.CustomName.Contains(Signature))    // Signature?
                    return true;

                if (!Detatchable)
                    RemoveMe();

                return false;
            }
            void RemoveMe()
            {

            }
            public override void Setup()
            {
                Program.Echo("Block Setup");
                base.Setup();
                Profile.Setup(TermBlock.CustomData);
            }
            public void DefaultData(string defData)
            {
                TermBlock.CustomData = TermBlock.CustomData == string.Empty ? defData : TermBlock.CustomData;
            }
        }
        public class Display : block
        {
            public IMyTextPanel Panel;
            public StringBuilder FormattedBuilder = new StringBuilder();
            public List<string> StringList = new List<string>();

            public string[][] Buffer = new string[2][];

            int CharCount;
            int LineCount;

            int ProdCharBuffer = 0;
            int BodySize;
            int BodyCount;
            int HeaderCount;
            int FooterCount;
            int ScrollIndex;
            int ScrollDirection;
            int Delay;
            int Timer;

            public bool AutoScroll;
            public DisplayMeta Meta;

            public Display(BlockMeta bMeta /*, int[] ratio*/) : base(bMeta)
            {
                Panel = (IMyTextPanel)bMeta.Block;
                RebootScreen();

                ScrollIndex = 0;
                ScrollDirection = 1;
                Delay = DefScrollDelay;
                Timer = 0;
                AutoScroll = true;

                Meta = new DisplayMeta(DefSigCount);
            }
            public override void Setup()
            {
                Program.Echo("Display Setup");
                base.Setup();

                Program.Echo($"Panel:{Panel != null}");

                Buffer[0] = Panel.CustomData.Split('\n'); // [0][x] Entries

                for (int i = 0; i < Buffer[0].Length; i++)
                {
                    string nextline = Buffer[0][i]; // [0][i] Entry

                    char opCode = (nextline.Length > 0) ? nextline[0] : '/'; // [1][0] OpCode

                    Buffer[1] = nextline.Split(' '); // [1][x] Blocks

                    try
                    {
                        switch (opCode)
                        {
                            case '/': // Comment Section (ignored)
                                break;

                            case '*': // Mode
                                if (Contains(nextline, "stat"))
                                    Meta.Mode = ScreenMode.STATUS;
                                if (Contains(nextline, "inv"))
                                    Meta.Mode = ScreenMode.INVENTORY;
                                if (Contains(nextline, "prod"))
                                    Meta.Mode = ScreenMode.PRODUCTION;
                                if (Contains(nextline, "res"))
                                    Meta.Mode = ScreenMode.RESOURCE;
                                if (Contains(nextline, "tally"))
                                    Meta.Mode = ScreenMode.TALLY;
                                if (Contains(nextline, "sort"))
                                    Meta.Mode = ScreenMode.SORT;
                                if (Contains(nextline, "link"))
                                    Meta.Mode = ScreenMode.LINK;
                                break;

                            case '@': // Target
                                if (Contains(nextline, "block"))
                                {
                                    //Operation
                                    block block = Program.Blocks.Find(x => x.TermBlock.CustomName.Contains(Buffer[1][1]));

                                    if (block != null)
                                    {

                                        Meta.TargetType = TargetType.BLOCK;
                                        Meta.TargetBlock = block;
                                        Meta.TargetName = block.CustomName;
                                    }
                                    else
                                    {
                                        Meta.TargetType = TargetType.DEFAULT;
                                        Meta.TargetName = "Block not found!";
                                    }
                                    break;
                                }
                                if (Contains(nextline, "group"))
                                {
                                    IMyBlockGroup targetGroup = Program.DetectedGroups.Find(x => x.Name.Contains(Buffer[1][1]));
                                    if (targetGroup != null)
                                    {
                                        Meta.TargetType = TargetType.GROUP;
                                        Meta.TargetGroup = targetGroup;
                                        Meta.TargetName = targetGroup.Name;
                                    }
                                    else
                                    {
                                        Meta.TargetType = TargetType.DEFAULT;
                                        Meta.TargetName = "Group not found!";
                                    }
                                    break;
                                }
                                break;

                            case '&':   // Option
                                if (Contains(nextline, "scroll"))
                                {
                                    int newDelay = Convert.ToInt32(Buffer[1][1]);
                                    Delay = newDelay > 0 ? newDelay : DefScrollDelay;
                                }
                                if (Contains(nextline, "auto"))
                                {
                                    AutoScroll = !nextline.Contains("-");
                                }
                                if (Contains(nextline, "f_size"))
                                {
                                    Panel.FontSize = Convert.ToInt32(Buffer[1][1]);
                                }
                                if (Contains(nextline, "f_col"))
                                {
                                    Color newColor = new Color(
                                        Convert.ToInt32(Buffer[1][1]),
                                        Convert.ToInt32(Buffer[1][2]),
                                        Convert.ToInt32(Buffer[1][3]));

                                    Panel.FontColor = newColor;
                                }
                                if (Contains(nextline, "b_col"))
                                {
                                    Color newColor = new Color(
                                        Convert.ToInt32(Buffer[1][1]),
                                        Convert.ToInt32(Buffer[1][2]),
                                        Convert.ToInt32(Buffer[1][3]));

                                    Panel.BackgroundColor = newColor;
                                }
                                break;

                            case '#':   // Notation
                                if (Contains(nextline, "def"))
                                    Meta.Notation = Notation.DEFAULT;
                                if (Contains(nextline, "simp"))
                                    Meta.Notation = Notation.SIMPLIFIED;
                                if (Contains(nextline, "sci"))
                                    Meta.Notation = Notation.SCIENTIFIC;
                                if (Contains(nextline, "%"))
                                    Meta.Notation = Notation.PERCENT;
                                break;

                            default:
                                Profile.AppendFilter(nextline);
                                break;
                        }
                    }
                    catch { }
                }
            }
            public void RebootScreen(/*int[] ratio*/)
            {
                /*if (Panel == null ||
                    ratio == null ||
                    ratio.Length < 2)
                    return;*/

                Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                Panel.Font = "Monospace";
                //ScreenRatio = ratio == null ? DefScreenRatio : ratio;
            }
            public bool Update()
            {
                Dlog($"Updating: {CustomName}");
                if (!CheckBlockExists())
                    return false;
                Dlog("Screen exists!");

                StringBuilder();
                StringWriter();

                if (AutoScroll)
                {
                    Timer++;
                    if (Timer >= Delay)
                    {
                        Timer = 0;
                        ScrollDirection = ScrollIndex >= BodyCount - BodySize ? -1 : ScrollIndex <= HeaderCount ? 1 : ScrollDirection;
                        Scroll(ScrollDirection);
                    }
                }

                return true;
            }
            void AppendLine(StrForm format, string rawInput = null)
            {
                int lineCount;
                AppendFormattedString(format, rawInput, out lineCount);

                switch (format)
                {
                    case StrForm.HEADER:
                        HeaderCount += lineCount;
                        break;

                    case StrForm.FOOTER:
                        FooterCount += lineCount;
                        break;

                    default:
                        BodyCount += lineCount;
                        break;
                }
            }
            /*void Scroll()
            {
                


                if (ScrollIndex >= BodyCount - BodySize)
                {
                    ScrollDirection = -1;
                }


                if (ScrollIndex <= HeaderCount)
                {
                    ScrollDirection = 1;
                }

                Scroll(ScrollDirection);
            }*/
            public void Scroll(int dir)
            {
                ScrollIndex += dir;
                ScrollIndex = ScrollIndex < 0 ? 0 : ScrollIndex > (BodyCount - BodySize) ? (BodyCount - BodySize) /*- 1*/ : ScrollIndex;
            }
            void StringBuilder()
            {
                CharCount = MonoSpaceChars(Panel);
                LineCount = MonoSpaceLines(Panel);

                BodyCount = 0;
                HeaderCount = 0;
                FooterCount = 0;
                StringList.Clear();

                try
                {
                    switch (Meta.Mode)
                    {
                        case ScreenMode.DEFAULT:
                            AppendLine(StrForm.HEADER, "[Default]");
                            break;

                        case ScreenMode.INVENTORY:
                            AppendLine(StrForm.HEADER, "[Inventory]");
                            break;

                        case ScreenMode.RESOURCE:
                            AppendLine(StrForm.HEADER, "[Resource]");
                            break;

                        case ScreenMode.STATUS:
                            AppendLine(StrForm.HEADER, "[Status]");
                            break;

                        case ScreenMode.SORT:
                            AppendLine(StrForm.HEADER, "[Filters]");
                            break;

                        case ScreenMode.PRODUCTION:
                            AppendLine(StrForm.HEADER, "[Production]");
                            ProductionBuilder();
                            break;

                        case ScreenMode.TALLY:
                            AppendLine(StrForm.HEADER, "[Tally]");
                            TallyBuilder();
                            //AppendLine(StrForm.FOOTER, "[TGM version 3.0]");
                            break;

                        case ScreenMode.LINK:
                            AppendLine(StrForm.HEADER, $"[Links ({Program.PumpRequests.Count})]");
                            LinkBuilder();
                            break;
                    }

                    switch (Meta.TargetType)
                    {
                        case TargetType.DEFAULT:
                            //AppendLine(StrForm.HEADER, $"( {Meta.TargetName} )");
                            break;

                        case TargetType.BLOCK:
                            if (Meta.Mode == ScreenMode.RESOURCE || Meta.Mode == ScreenMode.STATUS)
                                TableHeaderBuilder();
                            RawBlockBuilder(Meta.TargetBlock);
                            break;

                        case TargetType.GROUP:
                            RawGroupBuilder(Meta.TargetGroup);
                            break;
                    }

                    AppendLine(StrForm.FOOTER, "[TGM version 3.0]");
                }
                catch
                {
                    AppendLine(StrForm.WARNING, "String Builder Error...");
                    Dlog("STRING BUILDER FAIL-POINT!");
                }
            }

            void StringWriter()
            {
                try
                {
                    Panel.WriteText("");

                    for (int h = 0; h < HeaderCount; h++)
                        LINE(StringList[h]);

                    BodySize = LineCount - (HeaderCount + FooterCount);

                    for (int b = 0; b < BodyCount && b < BodySize; b++)
                        LINE(StringList[HeaderCount + ScrollIndex + b]);

                    for (int f = 0; f < FooterCount; f++)
                        LINE(StringList[f + (HeaderCount + BodyCount)]);
                }
                catch
                {
                    Dlog("WRITER FAIL-POINT!");
                }

            }

            void AppendFormattedString(StrForm format, string rawInput, out int lineCount)
            {
                FormattedBuilder.Clear();

                lineCount = 1;
                rawInput = rawInput != null ? rawInput : "";
                string[] blocks = rawInput.Split(Split);
                int remains = 0;

                switch (format)
                {
                    case StrForm.EMPTY:
                        break;

                    case StrForm.WARNING:
                        FAP(blocks[0]);
                        break;

                    case StrForm.TABLE:
                        if (blocks.Length == 2)
                        {
                            remains = CharCount - blocks[1].Length;
                            if (remains > 0)
                            {
                                if (remains < blocks[0].Length)
                                    FAP(blocks[0].Substring(0, remains));
                                else
                                    FAP(blocks[0]);
                            }
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                FAP("-");
                            FAP(blocks[2]);
                        }
                        else
                        {
                            remains = CharCount - blocks[1].Length;
                            FAP("[" + blocks[1] + "]");

                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                FAP("-");
                        }
                        break;

                    case StrForm.HEADER:
                    case StrForm.SUB_HEADER:
                    case StrForm.FOOTER:
                        if (CharCount <= blocks[0].Length) // Can header fit side dressings?
                        {
                            FormattedBuilder.Append(blocks[0]);
                        }
                        else // Apply Header Dressings
                        {
                            remains = CharCount - blocks[0].Length;

                            if (remains % 2 == 1)
                            {
                                blocks[0] += "=";
                                remains -= 1;
                            }

                            for (int i = 0; i < remains / 2; i++)
                                FAP("=");

                            FAP(blocks[0]);

                            for (int i = 0; i < remains / 2; i++)
                                FAP("=");

                            if (format == StrForm.HEADER)
                            {
                                StringList.Add(FormattedBuilder.ToString());
                                StringList.Add(" ");
                                lineCount = 2;
                                return;
                            }

                            if (format == StrForm.FOOTER)
                            {
                                StringList.Add(" ");
                                StringList.Add(FormattedBuilder.ToString());
                                lineCount = 2;
                                return;
                            }
                        }
                        break;

                    case StrForm.INVENTORY:
                        if (CharCount < (blocks[0].Length + blocks[1].Length)) // Can Listing fit on one line?
                        {
                            StringList.Add(blocks[0]);
                            StringList.Add(blocks[1]);
                            lineCount = 2;
                            return;
                        }
                        else
                        {
                            FAP(blocks[0]);

                            for (int i = 0; i < (CharCount - (blocks[0].Length + blocks[1].Length)); i++)
                                FAP("-");

                            FAP(blocks[1]);
                        }
                        break;

                    case StrForm.RESOURCE:
                        if (!blocks[1].Contains("%"))
                            blocks[1] += "|" + blocks[2];
                        if (CharCount < (blocks[0].Length + blocks[1].Length)) // Can Listing fit on one line?
                        {
                            StringList.Add(blocks[0]);
                            StringList.Add(blocks[1]);
                            lineCount = 2;
                            return;
                        }
                        else
                        {
                            FAP(blocks[0]);
                            for (int i = 0; i < (CharCount - (blocks[0].Length + blocks[1].Length)); i++)
                                FAP("-");
                            FAP(blocks[1]);
                        }
                        break;

                    case StrForm.STATUS:
                        // remaining chars = total line chars - (colored blocks + 2 to correct for colored blocks spacing)
                        remains = CharCount - (blocks[0].Length + blocks[1].Length + 2);
                        if (remains > 0)
                        {
                            if (remains < blocks[0].Length)
                                FAP(blocks[0].Substring(0, remains));
                            else
                                FAP(blocks[0]);
                        }

                        for (int i = 0; i < (remains - blocks[0].Length); i++)
                            FAP("-");

                        FAP(blocks[1]);
                        break;

                    case StrForm.PRODUCTION:
                        if (!Program.ShowProdBuilding)
                        {
                            if (CharCount < (blocks[0].Length + blocks[2].Length + blocks[3].Length // Can Listing fit on one line?
                                + 4)) // Additional chars
                            {
                                StringList.Add(blocks[0]);
                                StringList.Add(blocks[2]);
                                StringList.Add(blocks[3]);
                                lineCount = 3;
                                return;
                            }
                            else
                            {
                                FAP(blocks[0]);

                                for (int i = 0; i < (CharCount - (blocks[0].Length + blocks[2].Length + blocks[3].Length + 1)); i++)
                                    FAP("-");

                                FAP($"{blocks[2]}/{blocks[3]}");
                            }
                        }
                        else
                        {
                            if (CharCount < (blocks[0].Length + blocks[1].Length + blocks[2].Length + blocks[3].Length // Can Listing fit on one line?
                                + 4)) // Additional chars
                            {
                                StringList.Add(blocks[0]);
                                StringList.Add(blocks[1]);
                                StringList.Add(blocks[2]);
                                StringList.Add(blocks[3]);
                                lineCount = 4;
                                return;
                            }
                            else
                            {
                                FAP(blocks[0]);

                                for (int i = 0; i < ProdCharBuffer - blocks[0].Length; i++)
                                    FAP(" ");

                                FAP($" | {blocks[1]}");

                                for (int i = 0; i < (CharCount - (ProdCharBuffer + blocks[1].Length + blocks[2].Length + blocks[3].Length + 4)); i++)
                                    FAP("-");

                                FAP($"{blocks[2]}/{blocks[3]}");
                            }
                        }
                        break;

                    case StrForm.FILTER:
                        if (blocks.Length == 1)
                        {
                            FAP(blocks[0]);
                            break;
                        }
                        remains = CharCount - (blocks[0].Length + blocks[1].Length + blocks[2].Length + blocks[3].Length + 1);
                        if (remains < 0)
                        {
                            StringList.Add("Filter:");
                            StringList.Add(blocks[0]);
                            StringList.Add(blocks[1]);
                            StringList.Add(blocks[2]);
                            StringList.Add(blocks[3]);
                            lineCount = 5;
                        }
                        else
                        {
                            FAP($"{blocks[0]}:{blocks[1]}");
                            for (int i = 0; i < remains; i++)
                                FAP("-");
                            FAP($"{blocks[2]}:{blocks[3]}");
                        }
                        break;

                    case StrForm.LINK:

                        remains = CharCount - (blocks[0].Length + blocks[1].Length + blocks[2].Length);
                        if (remains % 2 == 1)
                        {
                            blocks[1] += " ";
                            remains -= 1;
                        }
                        remains /= 2;

                        FAP(blocks[0]);
                        for (int i = 0; i < remains; i++)
                            FAP(" ");
                        FAP(blocks[1]);
                        for (int i = 0; i < remains; i++)
                            FAP(" ");
                        FAP(blocks[2]);

                        break;
                }

                //FAP("\n");
                StringList.Add(FormattedBuilder.ToString());

            }
            void FAP(string input)
            {
                FormattedBuilder.Append(input);
            }
            void LINE(string input)
            {
                Panel.WriteText($"{input}\n", true);
            }

            /// String Builders
            void RawGroupBuilder(IMyBlockGroup targetGroup)
            {
                AppendLine(StrForm.HEADER, $"( {Meta.TargetName} )");
                AppendLine(StrForm.EMPTY);

                TableHeaderBuilder();

                List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
                Meta.TargetGroup.GetBlocks(groupList);

                foreach (IMyTerminalBlock nextTermBlock in groupList)
                {
                    block next = Program.Blocks.Find(x => x.TermBlock == nextTermBlock);
                    if (next != null)
                        RawBlockBuilder(next);
                    else
                        AppendLine(StrForm.WARNING, "Block data class not found! Signature missing from block in group!");
                }
            }
            void RawBlockBuilder(block target)
            {
                AppendLine(StrForm.HEADER, $"( {target.CustomName} )");

                switch (Meta.Mode)
                {
                    case ScreenMode.LINK:
                        LinkBuilder(target);
                        break;

                    case ScreenMode.SORT:
                        SortBuilder(target);
                        break;

                    case ScreenMode.RESOURCE:
                        ResBuilder(target);
                        break;
                }
            }
            void LinkBuilder(block target)
            {
                if (!(target is Inventory))
                {
                    AppendLine(StrForm.WARNING, "Not Link-able!");
                    return;
                }
                Inventory inventory = (Inventory)target;
                if (inventory.Slots.Count < 0)
                {
                    AppendLine(StrForm.WARNING, "No Linked Tallies!");
                    return;
                }


            }

            void LinkBuilder()
            {
                foreach (Slot request in Program.PumpRequests)
                {
                    AppendLine(StrForm.LINK, RawLink(request, CharCount));
                }
            }

            void SortBuilder(block target)
            {
                if (target.Profile == null)
                {
                    AppendLine(StrForm.WARNING, "No FilterProfile!");
                    return;
                }

                AppendLine(StrForm.HEADER, $"DEF IN:{target.Profile.DEFAULT_IN} | DEF OUT:{target.Profile.DEFAULT_OUT}");
                AppendLine(StrForm.HEADER, $"FILL :{target.Profile.FILL      } | EMPTY :{target.Profile.EMPTY      }");

                foreach (Filter filter in target.Profile.Filters)
                    AppendLine(StrForm.FILTER, RawFilter(filter));
            }

            void ProductionBuilder()
            {
                AppendLine(StrForm.EMPTY);

                foreach (Production prod in Program.Productions)
                {
                    string nextDef = prod.Filter.ItemID[1];
                    ProdCharBuffer = (ProdCharBuffer > nextDef.Length) ? ProdCharBuffer : nextDef.Length;


                    AppendLine(StrForm.PRODUCTION,
                        nextDef + Seperator +
                        prod.AssemblerType + Seperator +
                        prod.Current + Seperator +
                        prod.Filter.Target);
                }
            }
            void TallyBuilder()
            {
                //AppendLine(StrForm.EMPTY);

                MyFixedPoint blah;
                foreach (TallyItemType type in Program.AllItemTypes)
                {
                    if (!ProfileCompare(Profile, type.TypeId, out blah))
                        continue;

                    AppendLine(StrForm.SUB_HEADER, $"[{type.TypeId.Replace("MyObjectBuilder_", "")}]");
                    foreach (TallyItemSub subType in type.SubTypes)
                    {
                        if (!ProfileCompare(Profile, subType.Type, out blah))
                            continue;

                        AppendLine(StrForm.INVENTORY, RawListItem(Meta, subType, Program.LIT_DEFS));
                    }

                    AppendLine(StrForm.EMPTY);
                }
            }

            void TableHeaderBuilder()
            {
                switch (Meta.Mode)
                {
                    case ScreenMode.DEFAULT:
                        break;

                    case ScreenMode.INVENTORY:
                        AppendLine(StrForm.TABLE, $"[Items]{Seperator}Val|Uni");
                        break;

                    case ScreenMode.RESOURCE:
                        AppendLine(StrForm.TABLE, $"[Source]{Seperator}Val|Uni");
                        break;

                    case ScreenMode.STATUS:
                        AppendLine(StrForm.TABLE, $"[Target]{Seperator}|E  P|I  H|");
                        break;
                }
            }
            void ResBuilder(block targetBlock)
            {
                if (!(targetBlock is Resource))
                {
                    AppendLine(StrForm.WARNING, "Not a Resource!");
                    return;
                }

                Resource resource = (Resource)targetBlock;

                string value = string.Empty;
                int percent = 0;
                string unit = "n/a";

                switch (Meta.Notation)
                {
                    case Notation.DEFAULT:
                    case Notation.SCIENTIFIC:
                    case Notation.SIMPLIFIED:

                        switch (resource.Type)
                        {
                            case ResType.BATTERY:
                                IMyBatteryBlock batBlock = (IMyBatteryBlock)resource.TermBlock;
                                value = batBlock.CurrentStoredPower + "/" + batBlock.MaxStoredPower;
                                unit = batBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Stored power")).Split(' ')[3];
                                break;

                            case ResType.POWERGEN:
                                IMyPowerProducer powBlock = (IMyPowerProducer)resource.TermBlock;
                                value = powBlock.CurrentOutput + "/" + powBlock.MaxOutput;
                                unit = powBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Current Output")).Split(' ')[3];
                                break;

                            case ResType.GASTANK:
                                IMyGasTank gasTank = (IMyGasTank)resource.TermBlock;
                                value = gasTank.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Filled")).Split(' ')[2];
                                value = value.Substring(1, value.Length - 2);
                                value = value.Replace("L", "");
                                unit = " L ";
                                break;

                            case ResType.GASGEN:
                                IMyGasGenerator gasGen = (IMyGasGenerator)resource.TermBlock;
                                value = (gasGen.IsWorking) ? "Running" : "NotRunning";
                                unit = "I/O";
                                break;

                            case ResType.OXYFARM:
                                IMyOxygenFarm oxyFarm = (IMyOxygenFarm)resource.TermBlock;
                                value = (oxyFarm.IsWorking) ? "Running" : "NotRunning";
                                unit = "I/O";
                                break;
                        }
                        break;

                    case Notation.PERCENT:
                        switch (resource.Type)
                        {
                            case ResType.BATTERY:
                                IMyBatteryBlock batBlock = (IMyBatteryBlock)resource.TermBlock;
                                percent = Convert.ToInt32((batBlock.CurrentStoredPower / batBlock.MaxStoredPower) * 100f);
                                break;

                            case ResType.POWERGEN:
                                IMyPowerProducer powBlock = (IMyPowerProducer)resource.TermBlock;
                                percent = (int)((powBlock.CurrentOutput / powBlock.MaxOutput) * 100);
                                break;

                            case ResType.GASTANK:
                                IMyGasTank gasTank = (IMyGasTank)resource.TermBlock;
                                percent = (int)((gasTank.FilledRatio) * 100);
                                break;

                            case ResType.GASGEN:
                                IMyGasGenerator gasGen = (IMyGasGenerator)resource.TermBlock;
                                if (gasGen.IsWorking)
                                    percent = 100;
                                break;

                            case ResType.OXYFARM:
                                IMyOxygenFarm oxyFarm = (IMyOxygenFarm)resource.TermBlock;
                                if (oxyFarm.IsWorking)
                                    percent = 100;
                                break;
                        }
                        break;
                }

                AppendLine(StrForm.RESOURCE, $"{resource.CustomName}{Seperator}{((Meta.Notation == Notation.PERCENT) ? percent + "| % " : (value + Seperator + unit))}");
            }
        }
        public class Inventory : block
        {
            public bool Pulled = false;
            public IMyInventoryOwner Owner;
            public List<Slot> Slots = new List<Slot>();

            public Inventory(BlockMeta meta) : base(meta)
            {
                Owner = (IMyInventoryOwner)meta.Block;
            }

            public override void Setup()
            {
                base.Setup();
                if (Profile.FILL)
                    Program.Requesters.Add(this);
                else
                    Program.Requesters.Remove(this);
            }



            public bool IsFull(bool input = true)
            {
                IMyInventory target = PullInventory(input);
                return (float)target.CurrentVolume / (float)target.MaxVolume > FULL_MIN;
            }
            public bool IsClogged()
            {
                IMyInventory target = PullInventory();
                return (float)target.CurrentVolume / (float)target.MaxVolume > CLEAN_MIN;
            }
            public bool IsEmpty(bool input = true)
            {
                IMyInventory target = PullInventory(input);
                return (float)target.CurrentVolume / (float)target.MaxVolume < EMPTY_MAX;
            }
            public bool EmptyCheck()
            {
                return Profile.CLEAN ? IsClogged() : true;
            }

            public virtual bool FillCheck()
            {
                return true;
            }
            public IMyInventory PullInventory(int index)
            {
                int blah;
                return PullInventory(index, out blah);
            }
            public IMyInventory PullInventory(int index, out int offset)
            {
                IMyInventory target;
                offset = 0;

                for (int i = 0; i < Owner.InventoryCount; i++)
                {
                    target = Owner.GetInventory(i);
                    if (target.ItemCount <= index - offset)
                    {
                        offset += target.ItemCount;
                        continue;
                    }
                    return target;
                }
                return null;
            }
            public IMyInventory PullInventory(bool input = true)
            {
                return input ? Owner.GetInventory(0) : Owner.InventoryCount > 1 ? Owner.GetInventory(1) : Owner.GetInventory(0);
            }
            public MyInventoryItem? PullIndex(int index)
            {
                int offset;
                IMyInventory target = PullInventory(index, out offset);

                if (target == null)
                    return null;

                return target.GetItemAt(index - offset);
            }
            public int ItemCount()
            {
                int total = 0;

                for (int i = 0; i < Owner.InventoryCount; i++)
                    total += Owner.GetInventory(i).ItemCount;

                return total;
            }

        }
        public class Connector : Inventory
        {
            IMyShipConnector ConnectBlock;
            public Connector(BlockMeta meta) : base(meta)
            {
                ConnectBlock = (IMyShipConnector)meta.Block;
            }
        }
        public class Cargo : Inventory
        {
            public Cargo(BlockMeta meta) : base(meta)
            {

            }

        }
        public class Reactor : Inventory
        {
            IMyReactor ReactorBlock;
            public Reactor(BlockMeta meta) : base(meta)
            {
                ReactorBlock = (IMyReactor)meta.Block;
            }
        }
        public class Producer : Inventory
        {
            public IMyProductionBlock ProdBlock;

            public Producer(BlockMeta meta) : base(meta)
            {
                ProdBlock = (IMyProductionBlock)meta.Block;
            }
        }
        public class Assembler : Producer
        {
            public IMyAssembler AssemblerBlock;

            public Assembler(BlockMeta meta) : base(meta)
            {
                AssemblerBlock = (IMyAssembler)meta.Block;
                AssemblerBlock.CooperativeMode = false;
                Profile.CLEAN = true;
                Profile.RESIDE = true;
            }
        }
        public class Refinery : Producer
        {
            public IMyRefinery RefineBlock;

            public Refinery(BlockMeta meta) : base(meta)
            {
                RefineBlock = (IMyRefinery)meta.Block;
                //Profile.AUTO_REFINE = true;
                //Profile.RESIDE = true;
                RefineBlock.UseConveyorSystem = Profile.ACTIVE_CONVEYOR;
            }

            public override bool FillCheck()
            {
                return !Profile.ACTIVE_CONVEYOR;
            }

            public override void Setup()
            {
                DefaultData(RefineryDefault);
                base.Setup();
            }
        }
        public class Resource : block
        {
            public ResType Type;
            public bool bIsValue;

            public Resource(BlockMeta meta, ResType type, bool isValue = true) : base(meta)
            {
                Type = type;
                bIsValue = isValue;
            }
        }
        #endregion

        #region Helpers
        static bool CheckCandidate<T>(IMyTerminalBlock block, List<T> list) where T : block
        {
            if (block == null)
                return false;
            return (list.FindIndex(x => x.BlockID == block.EntityId) < 0 && block.CustomName.Contains(Signature));
        }
        static bool PullTerminalBlocksFromSignedGroup(List<IMyTerminalBlock> blocks, List<IMyBlockGroup> groups)
        {
            blocks.Clear();
            IMyBlockGroup group = groups.Find(x => Contains(x.Name, Signature));
            if (group == null)
                return false;

            group.GetBlocks(blocks);
            return true;
        }
        static bool ForceTransfer(Inventory target, MyFixedPoint targetAllowed, Slot source)
        {
            MyFixedPoint? allowed = AllowableReturn(targetAllowed, source);

            return target.PullInventory().TransferItemFrom(source.Inventory.PullInventory(source.Index), source.Index, null, null, allowed);
        }
        static bool TallyTransfer(Slot source, Slot dest)
        {
            try
            {
                if (source.SnapShot.Type != dest.SnapShot.Type)
                {
                    dest.Dlog("Type mis-match!");
                    return false;
                }
                    
                if (dest.Inventory.IsFull())
                {
                    dest.Dlog("Too Full!");
                    return false;
                }

                if (source.Inventory.IsEmpty())
                {
                    source.Dlog("Too Empty!");
                    return false;
                }

                MyFixedPoint? amount = AllowableReturn(dest, source);

                return dest.Inventory.PullInventory(dest.Index).TransferItemFrom(source.Inventory.PullInventory(source.Index), source.Index, dest.Index, true, amount);
            }
            catch { return false; }
        }

        static MyFixedPoint MaximumReturn(MyFixedPoint IN, MyFixedPoint OUT)
        {
            return IN == 0 ? OUT : OUT == 0 ? IN : IN < OUT ? IN : OUT;
        }
        static MyFixedPoint? AllowableReturn(Slot dest, Slot moving)
        {
            return AllowableReturn(dest.Filter.Target > 0 ? dest.Filter.Target - dest.SnapShot.Amount : 0, moving);
        }
        static MyFixedPoint? AllowableReturn(MyFixedPoint destTarget, Slot moving)
        {
            MyFixedPoint allow = moving.Filter.Target > 0 ? moving.SnapShot.Amount - moving.Filter.Target : moving.Inventory.Profile.RESIDE ? moving.SnapShot.Amount - 1 : 0;
            MyFixedPoint? output = MaximumReturn(destTarget, allow);
            return output.Value != 0 ? output.Value < 0 ? 0 : output : null;
        }
        static ProdMeta? ReadRecipe(string recipe, RootMeta meta)
        {
            try
            {
                string[] set = recipe.Split(':');
                MyDefinitionId nextId = MyDefinitionId.Parse(set[0]);
                string prodId = set[1];
                int target = Int32.Parse(set[2]);
                return new ProdMeta(meta, nextId, prodId, target);
            }
            catch { return null; }
        }
        static int MonoSpaceChars(IMyTextPanel panel)
        {
            return (int)(/*ratio / panel.FontSize*/ panel.SurfaceSize.X / (panel.FontSize * MONO_RATIO[0]));
        }
        static int MonoSpaceLines(IMyTextPanel panel)
        {
            return (int)(/*ratio / panel.FontSize*/ panel.SurfaceSize.Y / (panel.FontSize * MONO_RATIO[1]));
        }
        #endregion

        #region Comparisons
        static bool ProfileCompare(Inventory destination, Slot slot, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;

            if (!destination.PullInventory().IsConnectedTo(slot.Inventory.PullInventory(false)))
            {
                slot.Dlog("No Connection!");
                return false;
            }

            return dirIn ? slot.Filter.IN > -1 && ProfileCompare(destination.Profile, slot.SnapShot.Type, out target) :
                           slot.Filter.OUT > -1 && ProfileCompare(destination.Profile, slot.SnapShot.Type, out target, false);
        }
        static bool ProfileCompare(FilterProfile profile, MyItemType type, out MyFixedPoint target, bool dirIn = true)
        {
            return ProfileCompare(profile, type.TypeId, type.SubtypeId, out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, string type, out MyFixedPoint target, bool dirIn = true)
        {
            return ProfileCompare(profile, type, "any", out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, out MyFixedPoint target, string sub, bool dirIn = true)
        {
            return ProfileCompare(profile, "any", sub, out target, dirIn);
        }

        static bool ProfileCompare(FilterProfile profile, string type, string sub, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;
            if (profile == null)
                return true;

            bool match = false;
            bool allow = true;
            bool auto = false;

            if (dirIn && profile.DEFAULT_IN)
                auto = true;

            if (!dirIn && profile.DEFAULT_OUT)
                auto = true;

            foreach (Filter filter in profile.Filters)
            {
                if (!FilterCompare(filter, type, sub))
                    continue;

                match = true;
                allow = true; // re-try

                if (filter.IN == -1 &&
                    dirIn)
                    allow = false;

                if (filter.OUT == -1 &&
                    !dirIn)
                    allow = false;

                target = (filter.Target == 0) ? target : filter.Target;
            }

            allow = match ? allow : auto;

            return allow;
        }

        static bool FilterCompare(Filter A, MyProductionItem B)
        {
            return FilterCompare(A,
                A.ItemID[0], A.ItemID[1],
                "Component", // >: |
                B.BlueprintId.SubtypeId.ToString());
        }
        static bool FilterCompare(Filter A, string typeB, string subB)
        {
            return FilterCompare(A,
                A.ItemID[0], A.ItemID[1],
                typeB, subB);
        }
        static bool FilterCompare(Root dbug, string AA, string A, string b, string B)
        {
            if (AA != "any" && b != "any" && !Contains(AA, b) && !Contains(b, AA))
            {

                return false;
            }

            if (A != "any" && B != "any" && !Contains(A, B) && !Contains(B, A))
            {

                return false;
            }

            return true;
        }
        static bool Contains(string source, string target)
        {
            if (target == null)
                return true;

            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) > -1;
        }
        static bool SlotCompare(Slot Req, Slot Can)
        {
            return
                Req != Can && Req.SnapShot.Type == Can.SnapShot.Type &&
                //A.Inventory == B.Inventory || //A.Inventory == null || B.Inventory == null || // Let them amalgamate???
                ((Req.Filter.IN == 1 && Req.InLink == null && Req.Inventory.PullInventory().IsConnectedTo(Can.Inventory.PullInventory(false))) ||
                (Req.Filter.OUT == 1 && Req.OutLink == null && Req.Inventory.PullInventory().IsConnectedTo(Can.Inventory.PullInventory(false))));
        }

        #endregion

        #region Filter builders
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
            id[1] = def.SubtypeName.ToString().Replace("Component", "");
        }
        static void GenerateFilters(MyItemType type, ref string[] id)
        {
            id[0] = type.TypeId.Replace("MyObjectBuilder_", "");
            id[1] = type.SubtypeId.Replace("Component", "");
        }
        #endregion

        #region String builders
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
        static string ParseItemTotal(MyFixedPoint current, MyFixedPoint? target, DisplayMeta module)
        {
            switch (module.Notation)
            {
                case Notation.PERCENT: // has no use here
                case Notation.DEFAULT:
                    return $"{current}{(target.HasValue? $"/{target.Value}" : "" )}";    // decimaless def

                case Notation.SCIENTIFIC:
                    return $"{NotationBundler((float)current, module.SigCount)}";

                case Notation.SIMPLIFIED:
                    return $"{SimpleBundler((float)current, module.SigCount)}";
            }

            return "??????";
        }
        static string ItemNameCrop(MyItemType type, bool lit)
        {
            string itemName = string.Empty;
            if (lit)
            {
                itemName = type.ToString().Replace("MyObjectBuilder_", "");
            }
            else
            {
                itemName = type.ToString().Split('/')[1];

                if (type.TypeId.Split('_')[1] == "Ore" || type.TypeId.Split('_')[1] == "Ingot")   // Distinguish between Ore and Ingot Types
                    itemName = type.TypeId.Split('_')[1] + ":" + type.SubtypeId;

                if (type.TypeId.Split('_')[1] == "Ingot" && type.SubtypeId == "Stone")
                    itemName = "Gravel";
            }
            return itemName;
        }
        static string RawListItem(DisplayMeta mod, TallyItemSub sub, bool lit)
        {
            return $"{ItemNameCrop(sub.Type, lit)}{Split}{ParseItemTotal(sub.CurrentTotal, sub.TargetGoal, mod)}";
        }
        static string RawFilter(Filter filter)
        {
            if (filter == null)
                return "null filter!";

            string output = $"{filter.ItemID[0]}{Split}{filter.ItemID[1]}{Split}";
            output += filter.Target == 0 ? $"No Target{Split}" : $"{filter.Target}{Split}";
            output += filter.IN == 1 ? ":+ IN:" : filter.IN == 0 ? ":= IN:" : ":- IN:";
            output += filter.OUT == 1 ? ":+OUT:" : filter.OUT == 0 ? ":=OUT:" : ":-OUT:";
            return output;
        }
        static string RawLink(Slot link, int length)
        {
            if (link == null)
                return "null slot!";

            /*string linkName = $"{link.Inventory.CustomName}[{link.Index}]";
            string output = $"{(link.InLink == null ? "No Input" : $"{link.InLink.Inventory.CustomName}[{link.InLink.Index}]")}";
            string outLink = $"{(link.OutLink == null ? "No Output" : $"{link.OutLink.Inventory.CustomName}[{link.OutLink.Index}]")}";
            int buffers = length - (linkName.Length + output.Length + outLink.Length);
            if (buffers % 2 > 0)
            {
                linkName += " ";
                buffers -= 1;
            }
            buffers /= 2;

            for (int i = 0; i < buffers; i++)
                output += " ";

            output += linkName;

            for (int i = 0; i < buffers; i++)
                output += " ";

            output += outLink;

            return output;*/

            return $"{(link.InLink == null ? "No Input" : $"{link.InLink.Inventory.CustomName}[{link.InLink.Index}]")}{Split}" +
                $"{link.Inventory.CustomName}[{link.Index}]{Split}" +
                $"{(link.OutLink == null ? "No Output" : $"{link.OutLink.Inventory.CustomName}[{link.OutLink.Index}]")}";
        }
        #endregion

        #region Initializers
        void BlockDetection()
        {
            DetectedGroups.Clear();
            DetectedBlocks.Clear();

            GridTerminalSystem.GetBlocks(DetectedBlocks);
            GridTerminalSystem.GetBlockGroups(DetectedGroups);

            DETECTED = true;
        }
        void ClearWraps()
        {
            Productions.Clear();
            Blocks.Clear();
            Displays.Clear();
            Assemblers.Clear();
            Resources.Clear();
            Inventories.Clear();
            AllItemTypes.Clear();

            Requesters.Clear();
            PumpRequests.Clear();
        }
        void RunSystemBuild()
        {
            if (!DETECTED)
            {
                ClearWraps();
                BlockDetection();
            }

            BUILT = false;
            block newBlock = null;

            for (int i = AuxIndex; i < AuxIndex + BUILD_CAP; i++)
            {
                if (i >= DetectedBlocks.Count)
                {
                    BUILT = true;
                    break;
                }

                if (!CheckCandidate(DetectedBlocks[i], Blocks))
                    continue;

                if (DetectedBlocks[i] is IMyShipConnector)
                {
                    newBlock = new Connector(new BlockMeta(ROOT, DetectedBlocks[i]));
                }

                if (DetectedBlocks[i] is IMyCargoContainer)
                {
                    newBlock = new Cargo(new BlockMeta(ROOT, DetectedBlocks[i]));
                }

                if (DetectedBlocks[i] is IMyAssembler)
                {
                    newBlock = new Assembler(new BlockMeta(ROOT, DetectedBlocks[i], true));
                }

                if (DetectedBlocks[i] is IMyRefinery)
                {
                    newBlock = new Refinery(new BlockMeta(ROOT, DetectedBlocks[i], true));
                }

                if (DetectedBlocks[i] is IMyReactor)
                {
                    newBlock = new Reactor(new BlockMeta(ROOT, DetectedBlocks[i]));
                }

                if (DetectedBlocks[i] is IMyInventoryOwner)
                    Inventories.Add((Inventory)newBlock);

                if (DetectedBlocks[i] is IMyTextPanel)
                {
                    newBlock = new Display(new BlockMeta(ROOT, DetectedBlocks[i])/*, DefScreenRatio*/);
                    Displays.Add((Display)newBlock);
                }

                if (DetectedBlocks[i] is IMyBatteryBlock)
                {
                    newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.BATTERY);
                    Resources.Add((Resource)newBlock);
                }

                if (DetectedBlocks[i] is IMyGasTank)
                {
                    newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASTANK);
                    Resources.Add((Resource)newBlock);
                }

                if (DetectedBlocks[i] is IMyGasGenerator)
                {
                    newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASGEN, true);
                    Resources.Add((Resource)newBlock);
                }

                if (DetectedBlocks[i] is IMyOxygenFarm)
                {
                    newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.OXYFARM, true);
                    Resources.Add((Resource)newBlock);
                }

                if (newBlock != null)
                {
                    newBlock.Setup();
                    Blocks.Add(newBlock);
                }

            }

            SETUP = BUILT;
            AuxIndex = BUILT ? 0 : AuxIndex + BUILD_CAP;
        }
        #endregion

        #region Run Arguments
        void AdjustSpeed(bool up = true)
        {
            if ((byte)RUN_FREQ > 0)
            {
                if (!up && (byte)RUN_FREQ < 4)
                    RUN_FREQ = (UpdateFrequency)((byte)RUN_FREQ << 1);

                if (up && (byte)RUN_FREQ > 1)
                    RUN_FREQ = (UpdateFrequency)((byte)RUN_FREQ >> 1);

                if (!up && RUN_FREQ == UpdateFrequency.Update100)
                    RUN_FREQ = UpdateFrequency.None;
            }

            if (up && RUN_FREQ == UpdateFrequency.None)
                RUN_FREQ = UpdateFrequency.Update100;

            Runtime.UpdateFrequency = RUN_FREQ;
        }
        string GenerateRecipes()
        {
            Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> recipeList = new Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId>();

            List<MyProductionItem> nextList = new List<MyProductionItem>();
            string finalList = string.Empty;

            foreach (Assembler assembler in Assemblers)
            {
                assembler.ProdBlock.GetQueue(nextList);

                foreach (MyProductionItem item in nextList)
                    recipeList[item] = assembler.ProdBlock.BlockDefinition;
            }

            foreach (KeyValuePair<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> pair in recipeList)
                finalList += pair.Key.BlueprintId + ":" + pair.Value.SubtypeId.ToString() + ":" + pair.Key.Amount + "\n";

            return finalList;
        }
        void ClearQue()
        {
            foreach (Assembler producer in Assemblers)
            {
                Echo("Clearing...");
                if (producer.AssemblerBlock != null)
                    producer.AssemblerBlock.ClearQueue();
                Echo("Cleared!");
            }
        }
        void ReTagBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullTerminalBlocksFromSignedGroup(blocks, DetectedGroups))
                return;

            foreach (IMyTerminalBlock block in blocks)
                if (!block.CustomName.Contains(Signature))
                    block.CustomName += Signature;
        }
        void ReNameBlocks(string name)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullTerminalBlocksFromSignedGroup(blocks, DetectedGroups))
                return;

            for (int i = 0; i < blocks.Count; i++)
            {
                //string lead = numbered ? $"{i}" : "";
                blocks[i].CustomName = $"{name} - {i}{Signature}";
            }
        }

        void ScrollScreen(string name, int direction)
        {
            Display target = Displays.Find(x => x.CustomName.Contains(name));
            if (target == null)
                return;

            target.Scroll(direction);
        }

        void ToggleOp(string op)
        {
            switch (op)
            {
                case "TALLY":
                    Scanner.Toggle();
                    Sorter.Toggle();
                    break;

                case "MOVE":
                    Pumper.Toggle();
                    Linker.Toggle();
                    itemBrowser.Toggle();
                    inventoryBrowser.Toggle();
                    break;

                case "DISPLAY":
                    DisplayMan.Toggle();
                    break;

                case "PRODUCE":
                    Producing.Toggle();
                    break;

                case "CLEAN":
                    Cleaner.Toggle();
                    break;

                case "POWER":
                    Powering.Toggle();
                    break;
            }
        }
        void ReFilterBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullTerminalBlocksFromSignedGroup(blocks, DetectedGroups))
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
        void BuildProductions(string recipes)
        {
            string[] recipeList = recipes.Split('\n');
            foreach (string recipe in recipeList)
            {
                ProdMeta? prodMeta = ReadRecipe(recipe, ROOT);
                if (prodMeta == null)
                    continue;
                Production newProd = new Production((ProdMeta)prodMeta);
                Productions.Add(newProd);
            }
        }
        #endregion

        #region RUNTIME
        void Debugging()
        {
            Debug.Append($"CallCount: {SEARCH_COUNT}");
            mySurface.WriteText(Debug);
            Debug.Clear();
        }
        void RunArguments(string argument)
        {
            if (argument == string.Empty ||
                argument == null)
                return;

            switch (argument)
            {
                case "FASTER":
                    AdjustSpeed();
                    break;

                case "SLOWER":
                    AdjustSpeed(false);
                    break;

                case "BUILD":
                    DETECTED = false;
                    BUILT = false;
                    break;

                case "SETUP":
                    SETUP = false;
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
                    ClearQue();
                    break;

                case "SAVE":
                    Save();
                    break;

                case "LOAD":
                    LoadStorage();
                    break;


            }

            // SPECIAL

            try
            {
                InputBuffer = argument.Split(':');
                switch (InputBuffer[0])
                {
                    case "RENAME":
                        ReNameBlocks(InputBuffer[1]);
                        break;

                    case "TOGGLE":
                        ToggleOp(InputBuffer[1]);
                        break;

                    case "UP":
                        ScrollScreen(InputBuffer[1], -1);
                        break;

                    case "DOWN":
                        ScrollScreen(InputBuffer[1], 1);
                        break;
                }
            }
            catch
            {

            }
        }

        

        void ProgEcho()
        {
            Echo(EchoBuilder.ToString());

            EchoCount++;

            if (EchoCount >= EchoMax)
                EchoCount = 0;

            EchoBuilder.Clear();

            try
            {
                EchoBuilder.Append(
                $"{EchoLoop[EchoCount]} Torqk's Grid Manager {EchoLoop[EchoCount]}\n" +
                 "====================\n" +
                $"Program Frquency: {RUN_FREQ}\n" +
                $"DETECTED: {DETECTED} | BUILT: {BUILT} | SETUP: {SETUP}\n" +
                $"Total Managed Blocks/Inventories: {Blocks.Count}/{Inventories.Count}\n" +
                $"Current: Name : Ix/Count/Total\n");

                for (int i = 0; i < ActiveOps.Count; i++)
                {
                    EchoBuilder.Append($"{(ActiveOps[CurrentOp] == ActiveOps[i] ? ">>" : "==")}" +
                        $"{ActiveOps[i].Name}:{ActiveOps[i].WorkIndex}/{ActiveOps[i].WorkCount}/{ActiveOps[i].WorkTotal}\n");
                    if (ActiveOps[i].CurrentWork != null)
                        EchoBuilder.Append(
                            $"{ActiveOps[i].CurrentWork.Name}: {ActiveOps[i].CurrentWork.SIx}/{ActiveOps[i].CurrentWork.Job.SearchCount}\n");
                }


                EchoBuilder.Append("====================\n");
            }
            catch { EchoBuilder.Append("FAIL - POINT!"); }
        }
        void LoadStorage()
        {
            Me.CustomData = Storage;
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
            SetupOps();
            LoadStorage();
            BuildProductions(Me.CustomData); // for now, customData of pb used for recipes only.

            RUN_FREQ = INIT_FREQ;
            Runtime.UpdateFrequency = RUN_FREQ;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            SEARCH_COUNT = 0;
            RunArguments(argument);
            ProgEcho();

            //if (FAIL)
            //return;

            if (!DETECTED)
            {
                BlockDetection();
                return;
            }

            if (!BUILT)
            {
                RunSystemBuild();
                return;
            }

            try
            {
                RunOperations();
                Debugging();
            }
            catch { Debug.Append("FAIL-POINT!"); }

        }
        public void Save()
        {
            Storage = Me.CustomData;
        }
        #endregion

        #endregion
    }
}

﻿using Sandbox.Game.EntityComponents;
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

    #endregion

    partial class Program : MyGridProgram
    {
        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";
        const string CustomSig = "[CPY]";

        const float CLEAN_MIN = .8f;
        const float FULL_MIN = 0.98f;
        const float EMPTY_MAX = 0.02f;

        const int DefSigCount = 2;

        const int SEARCH_CAP = 30;
        const int BUILD_CAP = 20;

        static UpdateFrequency INIT_FREQ = UpdateFrequency.None;
        static readonly int[] DefScreenRatio = { 25, 17 };
        /// WARNING!! DO NOT GO FURTHER USER!! ///

        #region LOGIC 
        UpdateFrequency RUN_FREQ;
        bool LIT_DEFS = false;
        bool ShowProdBuilding = false;
        int ROOT_INDEX = 0;
        RootMeta ROOT;
        IMyTextSurface mySurface;

        static readonly char[] EchoLoop =
        {
            '%',
            '$',
            '#',
            '&'
        };
        string[] InputBuffer = new string[2];
        const char Split = '/';
        const string Seperator = "/";
        const int EchoMax = 4;

        StringBuilder Debug = new StringBuilder();
        StringBuilder EchoBuilder = new StringBuilder();

        int ProdCharBuffer = 0;
        int AuxIndex = 0;
        int EchoCount = 0;
        int SEARCH_COUNT = 0;

        bool DETECTED;
        bool BUILT;
        bool SETUP;
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
        SlotBrowser slotBrowser;
        InventoryBrowser inventoryBrowser;
        ProductionManager Producing;

        List<Op> AllOps;
        List<Op> InactiveOps = new List<Op>();
        List<Op> ActiveOps = new List<Op>();
        public int CurrentOp = -1;

        void SetupOps()
        {
            DisplayMan = new DisplayManager(this);
            Scanner = new TallyScanner(this);
            Sorter = new TallySorter(this);
            Pumper = new TallyPumper(this);
            Linker = new TallyLinker(this);
            slotBrowser = new SlotBrowser(this, false);
            inventoryBrowser = new InventoryBrowser(this, false);
            Producing = new ProductionManager(this, false);

            AllOps = new List<Op>()
            {
                DisplayMan,
                Scanner,
                Sorter,
                Linker,
                Pumper,
                slotBrowser,
                inventoryBrowser,
                Producing,
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
            FIND,
            WORK,
            CHAIN
        }
        public enum WorkType
        {
            NONE,
            SORT,
            //CHAIN,
            LINK,
            SCAN,
            PUMP,
            BROWSE
        }
        #endregion

        /*public enum CompareType
        {
            DEFAULT,
            STR_TYPE,
            STR_SUB,
            SLOT
        }*/

        #region OBJECTS
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
        public struct Job
        {
            public Root Requester;
            public string rawType;
            public string rawSubType;
            public int SearchCount;
            public int WIxA;
            public int WIxB;
            public JobType JobType;
            public WorkType WorkType;
            public TallyGroup TargetGroup;

            public Job(JobType job = JobType.CHAIN, WorkType work = WorkType.NONE, TallyGroup target = TallyGroup.ALL)
            {
                JobType = job;
                WorkType = work;
                TargetGroup = target;

                Requester = null;
                rawType = null;
                rawSubType = null;
                WIxA = 0;
                WIxB = 0;
                SearchCount = 0;
            }

            public void SlotCompare(Slot slot)
            {
                if (slot == null)
                    return;

                Requester = slot;
                rawType = slot.NEW.Type.TypeId;
                rawSubType = slot.NEW.Type.SubtypeId;
            }

            public void CopyCompare(Job source)
            {
                rawType = source.rawType;
                rawSubType = source.rawSubType;
            }
        }

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
            public virtual void RemoveMe()
            {

            }
            public void Dlog(string nextLine)
            {
                Program.Debug.Append($"{nextLine}\n");
            }
        }
        public class Slot : Root
        {
            public Inventory Inventory;
            public TallyItemSub Profile;
            public Slot
                InLink,
                OutLink;

            public MyInventoryItem
                OLD,
                NEW;
            public MyFixedPoint
                MyTarget,
                InTarget,
                OutTarget;

            public bool
                ProfileQueued, // Called by Scanner
                LinkQueued, // Called by Pumper
                BrowseQueued, // Called by Linker
                DEAD;
            public int
                IN,
                OUT,
                Index;

            public Slot(RootMeta meta, Inventory inventory, int index) : base(meta)
            {
                NEW = inventory.Buffer[index];
                OLD = NEW;
                Inventory = inventory;
                Index = index;
                TallyFilter();
            }
            void TallyFilter()
            {
                if (Inventory == null || Inventory.Profile == null)
                    return;

                MyFixedPoint inTarget, outTarget;

                bool inAllowed = ProfileCompare(Inventory.Profile, NEW.Type, out inTarget);
                bool outAllowed = ProfileCompare(Inventory.Profile, NEW.Type, out outTarget, false);

                MyTarget = outTarget < inTarget ? outTarget : inTarget;
                IN = inAllowed ? (Inventory.Profile.FILL ? 1 : 0) : -1;
                OUT = outAllowed ? (Inventory.Profile.EMPTY ? 1 : 0) : -1;
            }
            bool ProfileUpdate()
            {
                if (Profile != null)
                {
                    Dlog($"SlotCheck: {NEW.Type} || {Profile.Type}");
                    if (NEW.Type != Profile.Type)
                    {
                        Dlog("Broken Slot, re-tally");
                        UnsyncFromProfile(OLD.Amount);
                        TallyFilter();
                        return false;
                    }
                    else
                    {
                        Dlog($"Update: {NEW.Amount - OLD.Amount}");
                        Profile.Update(NEW.Amount - OLD.Amount);
                        return true;
                    }
                }
                return false;
            }
            public void Update(MyInventoryItem newItem)
            {
                OLD = NEW;
                NEW = newItem;

                if (ProfileQueued)
                    return;

                if (!ProfileUpdate())
                    QueueSort();
            }
            void QueueSort()
            {
                Program.Sorter.Queue.Add(this);
                ProfileQueued = true;
            }
            public bool Pump()
            {
                if (Inventory == null)
                    return false;

                bool satisfied = true;

                if (IN == 1)
                {
                    if (InLink == null)
                    {
                        Dlog("No In Link!");
                        satisfied = false;
                    }
                    else if (InLink.DEAD)
                    {
                        Dlog("In Link Dead!");
                        InLink = null;
                        satisfied = false;
                    }
                    else if (!TallyTransfer(InLink, this, InTarget))
                    {
                        Dlog("In Transfer Failed!");
                        satisfied = false;
                    }
                    else
                        Dlog("In Transfer Success!");
                }


                else if (OUT == 1)
                {
                    if (OutLink == null)
                    {
                        Dlog("No Out Link!");
                        satisfied = false;
                    }

                    else if (OutLink.DEAD)
                    {
                        Dlog("Out Link Dead!");
                        OutLink = null;
                        satisfied = false;
                    }

                    else if (!TallyTransfer(this, OutLink, OutTarget))
                    {
                        Dlog("Out Transfer Failed!");
                        satisfied = false;
                    }
                    else
                        Dlog("Out Transfer Success!");
                }

                Dlog($"Work Completed: {satisfied}");
                return satisfied;
            }
            public bool CheckUnLinked()
            {
                if (IN == 1 && InLink == null)
                    return true;

                if (OUT == 1 && OutLink == null)
                    return true;

                return false;
            }
            public void SyncTallyToProfile(TallyItemSub profile)
            {
                if (profile == null)
                    return;

                Profile = profile;

                Profile.Tallies[(int)TallyGroup.ALL].Add(this);

                if (IN == -1 && OUT == -1)
                    Profile.Tallies[(int)TallyGroup.LOCKED].Add(this);
                if (IN == 0 && OUT == 0)
                    Profile.Tallies[(int)TallyGroup.STORAGE].Add(this);
                if (IN == 1 || OUT == 1)
                    Profile.Tallies[(int)TallyGroup.REQUEST].Add(this);
                if (OUT > -1)
                    Profile.Tallies[(int)TallyGroup.AVAILABLE].Add(this);
            }
            private void UnsyncFromProfile(MyFixedPoint remaining)
            {
                if (Profile == null)
                    return;

                Profile.Tallies[(int)TallyGroup.ALL].Remove(this);

                if (IN == -1 && OUT == -1)
                    Profile.Tallies[(int)TallyGroup.LOCKED].Remove(this);
                if (IN == 0 && OUT == 0)
                    Profile.Tallies[(int)TallyGroup.STORAGE].Remove(this);
                if (IN == 1 || OUT == 1)
                    Profile.Tallies[(int)TallyGroup.REQUEST].Remove(this);
                if (OUT > -1)
                    Profile.Tallies[(int)TallyGroup.AVAILABLE].Remove(this);

                Profile.Update(-remaining);
                Profile = null;
            }

            public void Kill()
            {
                UnsyncFromProfile(NEW.Amount);
                Inventory.Slots.Remove(this);
                Inventory = null;
                DEAD = true; // for links
            }
        }
        public class Filter : Root
        {
            public string[] ItemID = new string[2];
            public MyFixedPoint Target;
            public int IN;
            public int OUT;

            Filter(RootMeta meta, MyFixedPoint target, int @in, int @out) : base(meta)
            {
                Target = target;
                IN = @in; // Prioritize In
                OUT = @out;
            }
            public Filter(RootMeta meta, string combo, MyFixedPoint target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
            {
                GenerateFilters(combo, ref ItemID);
            }
            public Filter(RootMeta meta, MyItemType type, MyFixedPoint target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
            {
                GenerateFilters(type, ref ItemID);
            }
            public Filter(RootMeta meta, MyDefinitionId id, MyFixedPoint target, int IN = 0, int OUT = 0) : this(meta, target, IN, OUT)
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
            public bool AUTO_REFINE;

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
                        if (Contains(nextline, "auto"))
                            AUTO_REFINE = !nextline.Contains("-");

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
                Dlog("New TallyItemType!");
            }
        }
        public class TallyItemSub : Root
        {
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

                Type = first.NEW.Type;
                Append(first);
                Dlog("New TallyItemSub!");
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
                Update(newTally.NEW.Amount);
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
            public Job MyJob;
            public Job ChainJob;
            public string Name;
            public int SIx = 0;

            public Work(RootMeta meta, Op op) : base(meta)
            {
                MyOp = op;
                //ACTIVE = true;
            }
            public int Init()
            {
                return WorkLoad(MyJob);
            }
            public virtual int WorkLoad(Job job)
            {
                Dlog($"Performing: {Name}");
                
                MyJob = job;
                MyOp.CurrentWork = this; 

                if (Chain != null)
                    ChainJob.Requester = job.Requester;

                return 1; // Keep going, called via override
            }
            public virtual bool DoWork()
            {
                return false; // Task/Search Proced
            }
            public virtual bool Compare()
            {
                return false;
            }
            public virtual int ChainCall()
            {
                return Chain == null ? 0 : 1; // Break work operation
            }

            int Peek(int i)
            {
                Dlog("Peeking...");

                if (CheckOp(i))
                    return -1;

                switch (MyJob.JobType)
                {
                    case JobType.FIND:
                        if(MyJob.Requester is Slot &&
                            ((Slot)MyJob.Requester).DEAD)
                        {
                            return -2;
                        }

                        Dlog("Searching...");
                        if (Compare())
                        {
                            Dlog("Found!");
                            if (Chain != null)
                            {
                                Dlog("ChainCall!");
                                return ChainCall() + 1;
                            }
                            else
                                return 1;
                        }
                        return 0;

                    case JobType.WORK:
                        Dlog("Doing work...");
                        return DoWork() ? 1 : 0;

                    case JobType.CHAIN:
                        if (Chain == null)
                            return 0; // fail

                        Dlog("ChainCall!");
                        return ChainCall();

                    default:
                        return 0; // fail
                }
            }
            bool CheckOp(int i)
            {
                bool maxed = Program.CallCounter();
                Dlog($"Calls Maxed: {maxed}");
                return maxed;
            }

            public int IterateScope(bool forward = true)
            {
                int result = MyJob.JobType == JobType.WORK ? 1 : 0;
                for (int i = SIx; i < MyJob.SearchCount; i++)
                {
                    Dlog($"Processing:{i}/{MyJob.SearchCount}");
                    SIx = i;
                    result = forward? Peek(i) : Peek(MyJob.SearchCount - (i+1));
                    Dlog($"Peek Result:{result}");

                    if (result == -1) // Operation call max
                    {
                        Dlog("Call cap reached!");
                        return -1;
                    }

                    if (MyJob.JobType == JobType.FIND && result > 0)
                    {
                        Dlog("Broke search early!");
                        break;
                    }

                    if (MyJob.JobType == JobType.WORK && result == 1) // Job Complete
                    {
                        Dlog("Work complete!");
                        break;
                    }
                }

                Dlog($"Search Result:{result == 1}");
                return result; // Found nothing/something
            }
        }
        public class InventoryWork : Work
        {
            public InventoryWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "INVENTORY";
            }
        }
        public class TypeWork : Work
        {
            public TypeWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "TYPE";
            }
            public override int WorkLoad(Job job)
            {
                if (base.WorkLoad(job) != 1)
                    return 0;

                return IterateScope();
            }
            public override bool Compare()
            {
                Dlog($"Comparing type: {MyJob.rawType} || {Program.AllItemTypes[SIx].TypeId}");
                return MyJob.rawType == "any" || Program.AllItemTypes[SIx].TypeId == MyJob.rawType;
            }
            public override bool DoWork()
            {
                return true; // ?
            }
            public override int ChainCall()
            {
                if (0 == base.ChainCall())
                {
                    Dlog("No Chain present!");
                    return 0;
                }

                ChainJob.CopyCompare(MyJob);

                ChainJob.WIxA = SIx;
                ChainJob.SearchCount = Program.AllItemTypes[SIx].SubTypes.Count;

                return Chain.WorkLoad(ChainJob);
            }
        }
        public class SubWork : Work
        {
            public SubWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SUBTYPE";
            }
            public override int WorkLoad(Job job)
            {
                if (base.WorkLoad(job) != 1)
                    return 0;

                return IterateScope();
            }
            public override bool Compare()
            {
                Dlog($"Comparing sub: {MyJob.rawSubType} || {Program.AllItemTypes[MyJob.WIxA].SubTypes[SIx].Type.SubtypeId}");
                return MyJob.rawSubType == "any" || Program.AllItemTypes[MyJob.WIxA].SubTypes[SIx].Type.SubtypeId == MyJob.rawSubType;
            }
            public override bool DoWork()
            {
                return true; // ?
            }
            public override int ChainCall()
            {
                if (0 == base.ChainCall())
                {
                    Dlog("No Chain present!");
                    return 0;
                }

                ChainJob.CopyCompare(MyJob);

                ChainJob.WIxA = MyJob.WIxA;
                ChainJob.WIxB = SIx;
                ChainJob.SearchCount = Program.AllItemTypes[MyJob.WIxA].SubTypes[SIx].Tallies[(int)MyJob.TargetGroup].Count;

                return Chain.WorkLoad(ChainJob);
            }
        }
        public class SlotWork : Work
        {
            public List<Slot> SlotList;
            public SlotWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SLOT";
            }

            public override int WorkLoad(Job job)
            {
                if (base.WorkLoad(job) != 1)
                    return 0;

                SlotList = null;

                if (job.Requester == null)
                {
                    SlotList = Program.PumpRequests;
                    MyJob.SearchCount = SlotList.Count;
                    //SlotList = Program.AllItemTypes[job.WIxA].SubTypes[job.WIxB].Tallies[(int)job.TargetGroup];
                }
                    
                if (job.Requester is Slot &&
                    ((Slot)job.Requester).Profile != null)
                {
                    SlotList = ((Slot)job.Requester).Profile.Tallies[(int)job.TargetGroup];
                    MyJob.SearchCount = SlotList.Count;
                }
                    
                if (job.Requester is Inventory)
                {
                    Inventory inv = (Inventory)job.Requester;
                    SlotList = inv.Slots;
                    MyJob.SearchCount = inv.Slots.Count > inv.Buffer.Count ? inv.Slots.Count : inv.Buffer.Count;
                }

                return IterateScope();
            }
            public override bool Compare()
            {
                if (!(MyJob.Requester is Slot))
                    return false;

                return SlotCompare((Slot)MyJob.Requester, SlotList[SIx]); // Maybe???

            }
            public override bool DoWork()
            {
                if (SlotList == null)
                    return true;

                switch (MyJob.WorkType)
                {
                    case WorkType.SCAN:
                        return SlotScan();

                    case WorkType.PUMP:
                        return SlotPump();

                    case WorkType.LINK:
                        return SlotLink();

                    case WorkType.BROWSE:
                        return false; // <<<<<<<<<<<<<<<<<<<<

                    default:
                        return false;
                }
            }

            bool SlotLink()
            {
                Slot queued = (Slot)MyJob.Requester;
                if (queued == null)
                {
                    Dlog("null Queued!");
                    return true;
                }

                if (queued.DEAD)
                {
                    Dlog("DEAD Queued!");
                    return true;
                }

                if (queued.Profile == null)
                {
                    Dlog("null Queued Profile!");
                    return true;
                }

                Slot match = SlotList[SIx];//queued.Profile.GetSlot(MyJob.TargetGroup, SIx);
                if (match == null)
                {
                    Dlog("null Match!");
                    return false;
                }

                Dlog($"Queued Tally for linking: {queued.NEW.Type.SubtypeId}\n");

                if (queued.IN == 1 &&
                    queued.InLink == null &&
                    match.OUT > -1 &&
                    (match.Inventory.Profile.RESIDE ||
                        !match.Inventory.IsEmpty()))
                {
                    Dlog($"In Link Made!");

                    queued.InTarget = MaximumReturn(match.MyTarget, queued.MyTarget);
                    queued.InLink = match;
                }

                if (queued.OUT == 1 &&
                    queued.InLink == null &&
                    match.IN > -1 &&
                    (match.Inventory.Profile.RESIDE ||
                        !match.Inventory.IsFull()))
                {
                    Dlog($"Out Link Made!");

                    queued.OutTarget = MaximumReturn(queued.MyTarget, match.MyTarget);
                    queued.OutLink = match;
                }

                return !queued.CheckUnLinked();
            }
            bool SlotScan()
            {
                Inventory inv = (Inventory)MyJob.Requester;
                if (inv == null)
                {
                    Dlog("null Inventory!");
                    return false;
                }

                if (SIx >= inv.Slots.Count) // Not enough slots
                {
                    Dlog("Adding slot");
                    Slot sloot = new Slot(Program.ROOT, inv, SIx);
                    if (sloot.IN == 1 || sloot.OUT == 1)
                        Program.PumpRequests.Add(sloot);
                    inv.Slots.Add(sloot);
                }

                if (SIx >= inv.Buffer.Count) // Too many slots
                {
                    Dlog("Killing slot");
                    inv.Slots[inv.Slots.Count - 1].Kill();
                }

                else
                {
                    Dlog("Updating slot");
                    inv.Slots[SIx].Update(inv.Buffer[SIx]);
                }

                return false; // Keep Working!
            }
            bool SlotPump()
            {
                if (SlotList[SIx].DEAD)
                {
                    Dlog("Dead Slot, removing from pump requesters...");

                    SlotList.RemoveAt(SIx);
                    MyJob.SearchCount = SlotList.Count;
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
        
        /*public class PullSlot : Worker//<Slot>
        {
            public Inventory InBound;
            public Inventory OutBound;
            public TallyItemSub SubGroup;
            public MyFixedPoint TargetBuffer;
            public PullSlot(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "FORCE_PULL";
            }
            public override int WorkLoad(Job job)
            {
                if (!(job.Requester is Inventory))
                    return 0;

                InBound = (Inventory)job.Requester;
                OutBound = Program.Inventories[job.WIxA];

                return IterateScopeForward();
            }
            public override bool DoWork()
            {
                return ProfileCompare(InBound.Profile, OutBound.Slots[SIx], out TargetBuffer, false);
            }
        }*/

        public class Op : Root
        {
            public string Name;
            public bool Active;
            public Work CurrentWork;

            public int
                WorkIndex,
                WorkCount,
                WorkTotal;

            public Op(Program prog, bool active) : base(prog.ROOT)
            {
                Active = active;
            }

            public virtual void Toggle()
            {
                Active = !Active;
            }
            public virtual bool Run() { Program.Debug.Append($"Running:{Name}\n"); return true; }
            public virtual bool HasWork() { return false; }
            public virtual void Clear() { }
            public bool Next()
            {
                WorkIndex++;
                WorkTotal++;
                bool result = WorkIndex >= WorkCount;
                WorkIndex = result ? 0 : WorkIndex;
                return result;
            }
            public void Reset(int index)
            {
                WorkIndex = 0;
                WorkCount = 0;
                WorkTotal = 0;
            }
            // -1 = Search Capped, 0 = Scope Searched, 1 = Task Complete 
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
                return WorkCount > 0;
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
                TypeMatch.MyJob = new Job(JobType.FIND, WorkType.SORT);

                SubMatch = new SubWork(prog.ROOT, this);
                TypeMatch.Chain = SubMatch;
                TypeMatch.ChainJob = new Job(JobType.FIND, WorkType.SORT);
            }

            public override bool Run()
            {
                base.Run();
                Dlog($"Sorts Remaining: {WorkCount}");

                TypeMatch.MyJob.SlotCompare(Queue[0]);
                TypeMatch.MyJob.SearchCount = Program.AllItemTypes.Count;

                if (Sort())
                {
                    TypeMatch.SIx = 0;
                    SubMatch.SIx = 0;
                    Queue[0].ProfileQueued = false;
                    Queue.RemoveAt(0);
                }

                return true;
            }
            bool Sort()
            {
                Dlog($"Processing top of sort queue: {Queue[0].NEW.Type}");
                TallyItemSub newProf;
                switch (TypeMatch.Init())
                {
                    case -2:
                        Dlog("DEAD!");
                        return true;

                    case -1:
                        Dlog("CORE-TEMP-CRITICAL!");
                        return false;

                    case 0:
                        Dlog("Un-Matched!\nNew Type/Sub/Slot");
                        newProf = new TallyItemSub(Program.ROOT, Queue[0]);
                        TallyItemType newType = new TallyItemType(Program.ROOT, newProf);
                        Program.AllItemTypes.Add(newType);
                        return true;

                    case 1:
                        Dlog("Type Matched!\nNew Sub/Slot");
                        newProf = new TallyItemSub(Program.ROOT, Queue[0]);
                        Program.AllItemTypes[TypeMatch.SIx].SubTypes.Add(newProf);
                        return true;

                    case 2:
                        Dlog("Sub Type Matched!\nNew Slot");
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
                SlotLink.MyJob = new Job(JobType.WORK, WorkType.LINK, TallyGroup.AVAILABLE);
            }
            public override bool Run()
            {
                base.Run();

                if (ProcessQueue())
                {
                    Queue[0].LinkQueued = false;
                    Queue.RemoveAt(0);
                }

                return true;
            }
            bool ProcessQueue()
            {
                try
                {
                    Dlog($"Processing TallySlot: {Queue[0].NEW.Type}\n" +
                    $"Source: {Queue[0].Inventory.CustomName} [{Queue[0].Index}]");
                }
                catch
                {
                    Dlog("H'wut?");
                }

                if (Queue[0].DEAD)
                {
                    Dlog("Dead!");
                    return true;
                }

                if (!Queue[0].CheckUnLinked())
                {
                    Dlog("Not unlinked! gtfo!");
                    return true;
                }

                SlotLink.MyJob.Requester = Queue[0];

                switch (SlotLink.Init())
                {
                    case -1: // Overheat
                        return false;

                    case 0: // Didn't find any links, send to inventory browser?

                        if (Queue[0].CheckUnLinked() && !Queue[0].BrowseQueued)
                        {
                            Dlog("Adding to browse queue...");
                            Program.inventoryBrowser.Queue.Add(Queue[0]);
                            Queue[0].BrowseQueued = true;
                        }

                        return true;

                    case 1: // Found a match
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
            public InventoryWork InventoryBrowse;
            public InventoryBrowser(Program prog, bool active = true) : base(prog, active)
            {
            }
        }
        public class SlotBrowser : Op
        {
            public TypeWork TypeMatch;
            public SubWork SubMatch;
            public SlotWork SlotMatch;
            public SlotBrowser(Program prog, bool active = true) : base(prog, active)
            {
                TypeMatch = new TypeWork(prog.ROOT, this);
                SubMatch = new SubWork(prog.ROOT, this);
                SlotMatch = new SlotWork(prog.ROOT, this);
                Name = "BROWSE";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Requesters.Count;
                //SearchCount = Program.AllItemTypes.Count;
                return WorkCount > 0;// && SearchCount > 0;
            }

            public override bool Run()
            {
                base.Run();

                if (BrowseItem())
                    Next();

                return true;
            }

            bool BrowseItem()
            {
                return false;
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

            public override bool Run()
            {
                base.Run();
                if (Program.Displays[WorkIndex].Update())
                    Next();

                return true;
            }
        }
        public class TallyScanner : Op
        {
            public SlotWork TallyScope;
            public TallyScanner(Program prog, bool active = true) : base(prog, active)
            {
                Name = "SCAN";

                TallyScope = new SlotWork(prog.ROOT, this);
                TallyScope.MyJob = new Job(JobType.WORK, WorkType.SCAN);
            }

            public override bool HasWork()
            {
                WorkCount = Program.Inventories.Count;
                return WorkCount > 0;
            }
            public override bool Run()
            {
                base.Run();

                Inventory working = Program.Inventories[WorkIndex];
                TallyScope.MyJob.Requester = working;
                Dlog($"Inventory Buffered: {working.Buffered}");
                if (!working.Buffered)
                {
                    working.Buffered = true;
                    working.SnapShot();
                    TallyScope.MyJob.SearchCount = working.Buffer.Count > working.Slots.Count ? working.Buffer.Count : working.Slots.Count;
                    Dlog($"Buffering: {TallyScope.MyJob.SearchCount}");
                }

                Dlog($"ScanIndex: {TallyScope.SIx}");

                if (Scan())
                {
                    working.Buffered = false;
                    TallyScope.SIx = 0;
                    Next();
                }
                    
                return true;
            }

            bool Scan()
            {
                Dlog($"Scanning Inventory: {Program.Inventories[WorkIndex].CustomName}");
                
                switch(TallyScope.Init())
                {
                    case -1:
                        Dlog("CORE-TEMP-CRITICAL!");
                        return false;

                    case 0:
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
                SlotPump.MyJob = new Job(JobType.WORK, WorkType.PUMP);
            }

            public override bool HasWork()
            {
                WorkCount = Program.PumpRequests.Count;
                return Program.PumpRequests.Count > 0;
            }
            public override bool Run()
            {
                base.Run();

                if (Pumping())
                {
                    SlotPump.SIx = 0;
                }

                return true;
            }
            bool Pumping()
            {
                switch (SlotPump.Init())
                {
                    case -1:
                        Dlog("OVER-HEAT!");
                        return false;

                    case 0:
                    //case 1:
                        Dlog("Pump workload complete!");
                        return true;

                    default:
                        return true;
                }
            }
        }
        public class ProductionManager : Op
        {
            public ProductionManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PROD";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Productions.Count;
                //WorkCount[1] = Program.Assemblers.Count;
                return WorkCount > 0;//&& WorkCount[1] > 0;
            }

            public override bool Run()
            {
                base.Run();
                return base.Run();
            }
        }

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

            public override void Setup()
            {
                Program.Echo("Block Setup");
                base.Setup();
                Profile.Setup(TermBlock.CustomData);
            }
        }
        public class Display : block
        {
            public IMyTextPanel Panel;
            public StringBuilder Builder = new StringBuilder();

            public string[][] Buffer = new string[2][];
            public int[] ScreenRatio = new int[2]; // chars, lines
            public int[] ScreenCount = new int[2];

            //public int WorkingIndex;
            public int ScrollCount;
            public int HeaderCount;
            public int FooterCount;
            public int ScrollIndex;
            public int ScrollDirection;
            public int Delay;
            public int Timer;

            public bool AutoScroll;
            public DisplayMeta Meta;

            public Display(BlockMeta bMeta, int[] ratio) : base(bMeta)
            {
                Panel = (IMyTextPanel)bMeta.Block;
                RebootScreen(ratio);

                ScrollIndex = 0;
                ScrollDirection = 1;
                Delay = 1;
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
                                    Delay = newDelay > 0 ? newDelay : 10;
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
                                Profile.Append(nextline);
                                break;
                        }
                    }
                    catch { }
                }
            }
            public void RebootScreen(int[] ratio)
            {
                if (Panel == null ||
                    ratio == null ||
                    ratio.Length < 2)
                    return;

                Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                Panel.Font = "Monospace";
                ScreenRatio = ratio == null ? DefScreenRatio : ratio;
            }
            public bool Update()
            {
                Dlog($"Updating: {CustomName}");
                if (!CheckBlockExists())
                    return false;
                Dlog("Screen exists!");
                StringBuilder();
                Panel.WriteText(Builder);
                if (AutoScroll)
                {
                    Timer++;
                    if (Timer >= Delay)
                    {
                        Dlog($"Scrolling: {ScrollDirection}[{ScrollIndex}]");

                        Timer = 0;
                        Scroll();
                    }
                }
                else
                {

                }

                return true;
            }
            void AppendLine(StrForm format, string rawInput = null)
            {
                switch (format)
                {
                    case StrForm.HEADER:
                        HeaderCount++;
                        break;

                    default:
                        ScrollCount++;
                        //if (ScrollCount < ScrollIndex ||
                        //    ScrollCount >= ScrollIndex + ScreenCount[1] - HeaderCount)
                        //    return;
                        break;
                }
                Builder.Append(FormatString(format, rawInput));
            }
            void Scroll()
            {
                if (ScrollIndex == ScrollCount - ScreenCount[1])
                    ScrollDirection = -1;

                if (ScrollIndex == 0)
                    ScrollDirection = 1;

                Scroll(ScrollDirection);
            }
            void Scroll(int dir)
            {
                ScrollIndex += dir;

                ScrollIndex = ScrollIndex < 0 ? 0 : ScrollIndex >= ScreenCount[1] ? ScreenCount[1] - 1 : ScrollIndex;
            }
            void StringBuilder()
            {
                ScreenCount[0] = MonoSpaceChars(ScreenRatio[0], Panel);
                ScreenCount[1] = MonoSpaceLines(ScreenRatio[1], Panel);

                ScrollCount = 0;
                HeaderCount = 0;
                FooterCount = 0;
                Builder.Clear();

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
                            break;
                    }

                    //AppendLine(StrForm.EMPTY);

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
                }
                catch
                {
                    Builder.Append("FAIL-POINT!\n");
                }
                //RawCount = RawLines.Count;
            }

            string FormatString(StrForm format, string rawInput)
            {
                //ScreenCount[0] = MonoSpaceChars(ScreenRatio[0], Panel);

                rawInput = rawInput != null ? rawInput : "";
                string[] blocks = rawInput.Split(Split);
                string formattedString = string.Empty;
                int remains = 0;
                //StrForm format = (StrForm)int.Parse(blocks[0]);

                switch (format)
                {
                    case StrForm.EMPTY:
                        break;

                    case StrForm.WARNING:
                        formattedString = blocks[0];
                        break;

                    case StrForm.TABLE:
                        if (blocks.Length == 2)
                        {
                            remains = ScreenCount[0] - blocks[1].Length;
                            if (remains > 0)
                            {
                                if (remains < blocks[0].Length)
                                    formattedString += blocks[0].Substring(0, remains);
                                else
                                    formattedString += blocks[0];
                            }
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                formattedString += "-";
                            formattedString += blocks[2];
                        }
                        else
                        {
                            remains = ScreenCount[0] - blocks[1].Length;
                            formattedString += "[" + blocks[1] + "]";
                            for (int i = 0; i < (remains - blocks[1].Length); i++)
                                formattedString += "-";
                        }
                        break;

                    case StrForm.HEADER:
                    case StrForm.SUB_HEADER:
                    case StrForm.FOOTER:
                        if (ScreenCount[0] <= blocks[0].Length) // Can header fit side dressings?
                        {
                            formattedString = blocks[0];
                        }
                        else // Apply Header Dressings
                        {
                            remains = ScreenCount[0] - blocks[0].Length;

                            if (remains % 2 == 1)
                            {
                                blocks[0] += "=";
                                remains -= 1;
                            }

                            for (int i = 0; i < remains / 2; i++)
                                formattedString += "=";

                            formattedString += blocks[0];

                            for (int i = 0; i < remains / 2; i++)
                                formattedString += "=";

                            formattedString += "\n";
                        }
                        break;

                    case StrForm.INVENTORY:
                        if (ScreenCount[0] < (blocks[0].Length + blocks[1].Length)) // Can Listing fit on one line?
                        {
                            formattedString = $"{blocks[0]}\n{blocks[1]}";
                        }
                        else
                        {
                            formattedString += blocks[0];

                            for (int i = 0; i < (ScreenCount[0] - (blocks[0].Length + blocks[1].Length)); i++)
                                formattedString += "-";

                            formattedString += blocks[1];
                        }
                        break;

                    case StrForm.RESOURCE:
                        if (!blocks[1].Contains("%"))
                            blocks[1] += "|" + blocks[2];
                        if (ScreenCount[0] < (blocks[0].Length + blocks[1].Length)) // Can Listing fit on one line?
                        {
                            formattedString = $"{blocks[0]}\n{blocks[1]}";
                        }
                        else
                        {
                            formattedString += blocks[0];
                            for (int i = 0; i < (ScreenCount[0] - (blocks[0].Length + blocks[1].Length)); i++)
                                formattedString += "-";
                            formattedString += blocks[1];
                        }
                        break;

                    case StrForm.STATUS:
                        // remaining chars = total line chars - (colored blocks + 2 to correct for colored blocks spacing)
                        remains = ScreenCount[0] - (blocks[0].Length + blocks[1].Length + 2);
                        if (remains > 0)
                        {
                            if (remains < blocks[0].Length)
                                formattedString += blocks[0].Substring(0, remains);
                            else
                                formattedString += blocks[0];
                        }
                        for (int i = 0; i < (remains - blocks[0].Length); i++)
                            formattedString += "-";
                        formattedString += blocks[1];
                        break;

                    case StrForm.PRODUCTION:
                        if (!Program.ShowProdBuilding)
                        {
                            if (ScreenCount[0] < (blocks[0].Length + blocks[2].Length + blocks[3].Length // Can Listing fit on one line?
                                + 4)) // Additional chars
                            {
                                formattedString = blocks[0] + "\nCurrent: " + blocks[2] + "\nTarget : " + blocks[3] + "\n";
                            }
                            else
                            {
                                formattedString += blocks[0];
                                for (int i = 0; i < (ScreenCount[0] - (blocks[0].Length + blocks[2].Length + blocks[3].Length + 1)); i++)
                                    formattedString += "-";

                                formattedString += blocks[2] + "/" + blocks[3] + "\n";
                            }
                        }
                        else
                        {
                            if (ScreenCount[0] < (blocks[0].Length + blocks[1].Length + blocks[2].Length + blocks[3].Length + 4)) // Can Listing fit on one line?
                            {
                                formattedString =
                                    blocks[0] +
                                    "\n" + blocks[1] +
                                    "\nCurrent: " + blocks[2] +
                                    "\nTarget : " + blocks[3] + "\n";
                            }
                            else
                            {
                                formattedString += blocks[0];

                                for (int i = 0; i < Program.ProdCharBuffer - blocks[0].Length; i++)
                                    formattedString += " ";
                                formattedString += " | " + blocks[1];
                                for (int i = 0; i < (ScreenCount[0] - (Program.ProdCharBuffer + blocks[1].Length + blocks[2].Length + blocks[3].Length + 4)); i++)
                                    formattedString += "-";

                                formattedString += blocks[2] + "/" + blocks[3];
                            }
                        }
                        break;

                    case StrForm.FILTER:
                        if (blocks.Length == 1)
                        {
                            formattedString += blocks[0];
                            break;
                        }
                        remains = ScreenCount[0] - (blocks[0].Length + blocks[1].Length + blocks[2].Length + blocks[3].Length + 1);
                        if (remains < 0)
                        {
                            formattedString += $"Filter:\n{blocks[0]}\n{blocks[1]}\n{blocks[2]}\n{blocks[3]}\n";
                        }
                        else
                        {
                            formattedString += $"{blocks[0]}:{blocks[1]}";
                            for (int i = 0; i < remains; i++)
                                formattedString += "-";
                            formattedString += $"{blocks[2]}{blocks[3]}";
                        }
                        break;
                }

                formattedString += "\n";
                return formattedString;
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
                    Program.ProdCharBuffer = (Program.ProdCharBuffer > nextDef.Length) ? Program.ProdCharBuffer : nextDef.Length;


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

                MyFixedPoint target;
                foreach (TallyItemType type in Program.AllItemTypes)
                {
                    if (!ProfileCompare(Profile, type.TypeId, out target))
                        continue;

                    AppendLine(StrForm.SUB_HEADER, $"[{type.TypeId.Replace("MyObjectBuilder_", "")}]");
                    foreach (TallyItemSub subType in type.SubTypes)
                    {
                        if (!ProfileCompare(Profile, subType.Type, out target))
                            continue;

                        AppendLine(StrForm.INVENTORY, RawListItem(Meta, target, subType.CurrentTotal, subType.Type, Program.LIT_DEFS));
                    }
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
            public bool Buffered = false;
            public IMyInventoryOwner Owner;
            public List<Slot> Slots = new List<Slot>();
            public List<MyInventoryItem> Buffer = new List<MyInventoryItem>();

            public Inventory(BlockMeta meta) : base(meta)
            {
                Owner = (IMyInventoryOwner)meta.Block;
            }

            public override void Setup()
            {
                base.Setup();
                if (Profile.FILL || Profile.EMPTY)
                    Program.Requesters.Add(this);
                else
                    Program.Requesters.Remove(this);
            }
            public virtual bool SnapShot()
            {
                Buffer.Clear();
                PullInventory(this).GetItems(Buffer);
                //Buffered = true;
                return Buffer.Count > 0;
            }


            public bool IsFull()
            {
                return (float)PullInventory(this).CurrentVolume / (float)PullInventory(this).MaxVolume > FULL_MIN;
            }
            public bool IsClogged()
            {
                return (float)PullInventory(this).CurrentVolume / (float)PullInventory(this).MaxVolume > CLEAN_MIN;
            }
            public bool IsEmpty()
            {
                return (float)PullInventory(this).CurrentVolume / (float)PullInventory(this).MaxVolume < EMPTY_MAX;
            }
            public bool EmptyCheck()
            {
                return Profile.CLEAN ? IsClogged() : true;
            }
            public virtual bool InventoryFillCheck()
            {
                return true;
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
            List<MyInventoryItem> SecondBuffer = new List<MyInventoryItem>();

            public Producer(BlockMeta meta) : base(meta)
            {
                ProdBlock = (IMyProductionBlock)meta.Block;
            }

            public override bool SnapShot()
            {
                Buffer.Clear();
                SecondBuffer.Clear();
                PullInventory(this).GetItems(Buffer);
                PullInventory(this, false).GetItems(SecondBuffer);
                Buffer.AddRange(SecondBuffer);
                //Buffered = true;
                return Buffer.Count > 0;
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
                Profile.AUTO_REFINE = true;
                Profile.RESIDE = true;
                RefineBlock.UseConveyorSystem = Profile.AUTO_REFINE;
            }

            public override bool InventoryFillCheck()
            {
                return !Profile.AUTO_REFINE;
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
        static bool PullSignedTerminalBlocks(List<IMyTerminalBlock> blocks, List<IMyBlockGroup> groups)
        {
            blocks.Clear();
            IMyBlockGroup group = groups.Find(x => Contains(x.Name, Signature));
            if (group == null)
                return false;

            group.GetBlocks(blocks);
            return true;
        }
        static bool ForceTransfer(Inventory target, Inventory source, MyInventoryItem item, MyFixedPoint amount)
        {
            if (amount != 0)
                return PullInventory(target).TransferItemFrom(PullInventory(source, false), item, amount);
            return PullInventory(target).TransferItemFrom(PullInventory(source, false), item);
        }
        static bool TallyTransfer(Slot source, Slot dest, MyFixedPoint target)
        {
            try
            {
                IMyInventory inBound = PullInventory(dest.Inventory);
                IMyInventory outBound = PullInventory(source.Inventory, false);
                MyFixedPoint available = source.Inventory.Profile.RESIDE ? source.NEW.Amount - 1 : source.NEW.Amount;
                MyFixedPoint amount = MaximumReturn(available, target);
                return inBound.TransferItemFrom(outBound, source.NEW, amount);
            }
            catch { return false; }
        }
        static IMyInventory PullInventory(Inventory inventory, bool input = true)
        {
            if (inventory is Producer)
            {
                return (input) ? ((Producer)inventory).ProdBlock.InputInventory : ((Producer)inventory).ProdBlock.OutputInventory;
            }
            if (inventory != null)
            {
                return inventory.Owner.GetInventory(0);
            }
            return null;
        }
        static MyFixedPoint MaximumReturn(MyFixedPoint IN, MyFixedPoint OUT)
        {
            return IN == 0 ? OUT : OUT == 0 ? IN : IN < OUT ? IN : OUT;
        }
        static MyFixedPoint AllowableReturn(MyFixedPoint IN, MyFixedPoint OUT, Slot moving)
        {
            MyFixedPoint DEST = MaximumReturn(IN, OUT);
            MyFixedPoint ITEM = moving.NEW.Amount;
            bool reside = moving.Inventory.Profile.RESIDE;
            return DEST == 0 || DEST >= ITEM ? reside ? ITEM - 1 : ITEM : DEST;
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
        static int MonoSpaceChars(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
        }
        static int MonoSpaceLines(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
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
        static string ParseItemTotal(MyFixedPoint item, DisplayMeta module)
        {
            switch (module.Notation)
            {
                case Notation.PERCENT: // has no use here
                case Notation.DEFAULT:
                    return $"{item}";    // decimaless def

                case Notation.SCIENTIFIC:
                    return $"{NotationBundler((float)item, module.SigCount)}";

                case Notation.SIMPLIFIED:
                    return $"{SimpleBundler((float)item, module.SigCount)}";
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
        static string RawListItem(DisplayMeta mod, MyFixedPoint target, MyFixedPoint current, MyItemType type, bool lit)
        {
            return $"{ItemNameCrop(type, lit)}{Split}{ParseItemTotal(current, mod)}";
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
        #endregion

        #region Comparisons
        static bool ProfileCompare(FilterProfile profile, Slot slot, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;
            return dirIn ? slot.IN > -1 && ProfileCompare(profile, slot.NEW.Type, out target, false) :
                           slot.OUT > -1 && ProfileCompare(profile, slot.NEW.Type, out target);
        }
        static bool ProfileCompare(FilterProfile profile, MyItemType type, out MyFixedPoint target, bool dirIn = true)
        {
            return ProfileCompare(profile, type.TypeId, type.SubtypeId, out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, string type, out MyFixedPoint target, bool dirIn = true)
        {
            return ProfileCompare(profile, type, "any", out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, string type, string sub, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;
            if (profile == null)
                return true;

            bool match = false;
            bool allow = false;
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
                allow = true;

                if (filter.IN == -1 &&
                    dirIn)
                    allow = false;

                if (filter.OUT == -1 &&
                    !dirIn)
                    allow = false;

                target = (filter.Target == 0) ? target : filter.Target;
            }

            bool result = match ? allow : auto;

            return result;
        }

        static bool FilterCompare(Filter A, MyProductionItem B)
        {
            return FilterCompare(
                A.ItemID[0], A.ItemID[1],
                "Component", // >: |
                B.BlueprintId.SubtypeId.ToString());
        }
        static bool FilterCompare(Filter A, string typeB, string subB)
        {
            return FilterCompare(
                A.ItemID[0], A.ItemID[1],
                typeB, subB);
        }
        static bool FilterCompare(string a, string A, string b, string B)
        {
            if (a != "any" && b != "any" && !Contains(a, b) && !Contains(b, a))
                return false;

            if (A != "any" && B != "any" && !Contains(A, B) && !Contains(B, A))
                return false;

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
                Req != Can && /*Inbound != null && Outbound != null ||*/ Req.NEW.Type == Can.NEW.Type &&
                //A.Inventory == B.Inventory || //A.Inventory == null || B.Inventory == null || // Let them amalgamate???
                ((Req.IN == 1 && Req.InLink == null && PullInventory(Req.Inventory).IsConnectedTo(PullInventory(Can.Inventory, false))) ||
                (Req.OUT == 1 && Req.OutLink == null && PullInventory(Req.Inventory, false).IsConnectedTo(PullInventory(Can.Inventory))));
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
            foreach (Op op in AllOps)
                op.Clear();
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
                    newBlock = new Display(new BlockMeta(ROOT, DetectedBlocks[i]), DefScreenRatio);
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
            if (!PullSignedTerminalBlocks(blocks, DetectedGroups))
                return;

            foreach (IMyTerminalBlock block in blocks)
                if (!block.CustomName.Contains(Signature))
                    block.CustomName += Signature;
        }
        void ReNameBlocks(string name, bool numbered = false)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullSignedTerminalBlocks(blocks, DetectedGroups))
                return;


            for (int i = 0; i < blocks.Count; i++)
            {
                string lead = numbered ? i.ToString() : "";
                blocks[i].CustomName = $"{lead}{name}";
            }

        }
        void ReFilterBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullSignedTerminalBlocks(blocks, DetectedGroups))
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
                ProdMeta? chk = ReadRecipe(recipe, ROOT);
                if (chk == null)
                    continue;
                ProdMeta meta = (ProdMeta)chk;
                Production newProd = new Production(meta);
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
                    LoadRecipes();
                    break;

                case "TALLY":
                    Sorter.Toggle();
                    Scanner.Toggle();
                    break;

                case "SORT":
                    Pumper.Toggle();
                    Linker.Toggle();
                    slotBrowser.Toggle();
                    break;

                case "DISPLAY":
                    DisplayMan.Toggle();
                    break;

                case "PRODUCE":
                    Producing.Toggle();
                    break;

                case "POWER":
                    //PowerDistribute.Toggle();
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
                            $"{ActiveOps[i].CurrentWork.Name}: {ActiveOps[i].CurrentWork.SIx}/{ActiveOps[i].CurrentWork.MyJob.SearchCount}\n");
                }


                EchoBuilder.Append("====================\n");
            }
            catch { EchoBuilder.Append("FAIL - POINT!"); }
        }
        void LoadRecipes()
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
            LoadRecipes();
            BuildProductions(Me.CustomData);

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

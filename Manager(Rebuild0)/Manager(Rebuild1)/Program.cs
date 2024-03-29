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
/*public void TrashBin(Root root)
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

 */
/*public struct ProdMeta
    {
        public string BluePrintDefintion;
        public string ItemDefinition;
        public int TargetGoal;
        public RootMeta Root;

        public ProdMeta(RootMeta root, string def, int target, string itemDef = null)
        {
            BluePrintDefintion = def;
            ItemDefinition = itemDef;
            TargetGoal = target;
            Root = root;
        }
    }*/
/*static ProdMeta? ReadRecipe(string recipe, RootMeta meta)
        {
            try
            {
                string[] set = recipe.Split(Split);
                string blueDef = set[0];
                string itemDef = set[1];
                int target = Int32.Parse(set[2]);
                return new ProdMeta(meta, blueDef, target, itemDef);
            }
            catch { return null; }
        }*/
/*public class Production : Root
        {
            //public MyDefinitionId Definition;
            public Compare Compare;
            public MyFixedPoint TargetGoal;
            public MyFixedPoint Current;
            //public MyItemType? Item;

            public Production(ProdMeta meta) : base(meta.Root)
            {
                Compare =  meta.ItemDefinition == null ? new Compare(meta.BluePrintDefintion) : new Compare(meta.ItemDefinition);
                TargetGoal = meta.TargetGoal;
                Current = 0;
                //try { Item = MyItemType.Parse(meta.ItemDefinition); } catch { Item = null; }
            }

            public bool Update()
            {

                // OVERHAUL PRODUCTION! REMOVE FOREACH LOOPS! //

                if (Current >= TargetGoal)
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

                    //existingQues.AddRange(nextList.FindAll(x => FilterCompare(Filter, x)));
                }

                MyFixedPoint existingQueAmount = 0;
                foreach (MyProductionItem item in existingQues)
                    existingQueAmount += item.Amount;

                MyFixedPoint projectedTotal = Current + existingQueAmount;
                MyFixedPoint projectedOverage = projectedTotal - TargetGoal;

                if (projectedOverage >= 0)
                {
                    MyFixedPoint remove = new MyFixedPoint();

                    foreach (Assembler producer in candidates)
                    {
                        existingQues.Clear();
                        producer.ProdBlock.GetQueue(existingQues);
                        for (int i = 0; i < existingQues.Count; i++)
                        {
                            //if (!FilterCompare(Filter, existingQues[i]))
                                //continue;

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

                    //foreach (Assembler producer in candidates)                                  // Distribute
                        //producer.ProdBlock.AddQueueItem(Def, qeueIndividual);
                }
                return true;
            }
        }*/
#endregion

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";
        const string CopySignature = "[CPY]";
        const string Terminate = "[TERMINATE]";

        const float PROD_BUFFER = 1000;
        const float CLEAN_MIN = .8f;
        const float FULL_MIN = 0.999999f;
        const float EMPTY_MAX = 0.000001f;

        const int DefScrollDelay = 1;
        const int DefSigCount = 0;
        const int OP_CAP_MAX = 50;
        const int OP_CAP_DEF = 30;
        const int OP_CAP_MIN = 10;
        const int FLIM_FLAM_DELAY = 50;

        const bool TAKE_OVER = false;

        static UpdateFrequency INIT_FREQ = UpdateFrequency.Update1;
        /// WARNING!! DO NOT GO FURTHER USER!! ///

        const string Version = "TGM v3.1";

        #region DEFAULTS

        static readonly string RefineryDefault =
            "! -in +out\n" +
            $"ore/ +in -out #{PROD_BUFFER}/\n" +
            "&fill\n" +
            "&empty\n" +
            "&conveyor-";

        static readonly string AssemblerDefault =
            "! -in +out\n" +
            $"ingot/ +in -out #{PROD_BUFFER}/\n" +
            $"comp/tech +in -out #100/\n" +
            "&fill\n" +
            "&empty\n" +
            "&conveyor-";

        static readonly string DisplayDefault =
            "*default\n" +
            "&f_color 100 100 100";

        #endregion

        #region LOGIC 
        UpdateFrequency RUN_FREQ;

        bool LIT_DEFS = false;

        bool DETECTED;
        bool BUILT;
        bool SETUP;
        bool BREAK;

        bool FLIM_FLAM;
        bool ONE_TIME = true;

        const string LEGACY_TYPE_PREFIX = "MyObjectBuilder_";
        //const string Seperator = "/";
        const char Split = '/';
        const char Bar = '=';
        const char A = '<';
        const char B = '~';
        const char C = '>';
        const char Empty = ' ';
        const char Full = '=';
        static readonly char[] EchoLoop =
        {
            '%',
            '$',
            '#',
            '&'
        };
        const int EchoMax = 4;
        static int[] MONO_RATIO = { 20, 30 };

        string[] InputBuffer;
        StringBuilder BugBuilder = new StringBuilder();
        StringBuilder Debug = new StringBuilder();
        StringBuilder EchoBuilder = new StringBuilder();
        IMyTextSurface mySurface;
        RootMeta ROOT;

        int ROOT_INDEX = 0;
        int OP_CAP = OP_CAP_DEF;
        int OP_COUNT = 0;
        int DISPLAY_SCROLL_TIMER = 0;
        int FLIM_FLAM_UPDATE = 0;

        int EchoCount = 0;
        int CurrentOp = -1;
        int AuxIndex = 0;

        #endregion

        #region Engine
        List<IMyTerminalBlock> DetectedBlocks = new List<IMyTerminalBlock>();
        List<IMyBlockGroup> DetectedGroups = new List<IMyBlockGroup>();

        List<block> Blocks = new List<block>();
        List<Display> Displays = new List<Display>();
        List<Assembler> Assemblers = new List<Assembler>();
        List<Resource> Resources = new List<Resource>();
        List<InvResource> InvResources = new List<InvResource>();
        List<Inventory> Inventories = new List<Inventory>();

        RootList<TallyItemType> AllItemTypes;
        RootList<TallyItemSub> AllItemSubs;
        RootList<TallyItemSub> Productions;
        RootList<Inventory> PullRequests;
        RootList<Slot> MoveRequests;
        RootList<Slot> PumpRequests;

        DisplayManager Writer;
        PageMaster Pager;
        TallyScanner Scanner;
        TallySorter Sorter;
        TallyMatcher Mover;
        TallyPumper Pumper;
        ItemBrowser Browser;
        ItemDumper Dumper;
        QuotaAssigner Quoting;
        ProductionManager Producing;
        PowerManager Powering;

        List<Op> AllOps;
        List<Op> InactiveOps = new List<Op>();
        List<Op> ActiveOps = new List<Op>();

        void SetupRootLists()
        {
            AllItemTypes = new RootList<TallyItemType>(RootListType.TYPE_ALL, this);
            AllItemSubs = new RootList<TallyItemSub>(RootListType.SUB_ALL, this);
            Productions = new RootList<TallyItemSub>(RootListType.SUB_PROD, this);
            PullRequests = new RootList<Inventory>(RootListType.INV_PULL, this);
            MoveRequests = new RootList<Slot>(RootListType.SLOT_MOVE, this);
            PumpRequests = new RootList<Slot>(RootListType.SLOT_PUMP, this);
        }
        void SetupOps()
        {
            Writer = new DisplayManager(this);
            Pager = new PageMaster(this);
            Scanner = new TallyScanner(this);
            Sorter = new TallySorter(this);
            Pumper = new TallyPumper(this);
            Mover = new TallyMatcher(this);
            Browser = new ItemBrowser(this);
            Dumper = new ItemDumper(this);
            Quoting = new QuotaAssigner(this);
            Producing = new ProductionManager(this);
            Powering = new PowerManager(this, false);

            AllOps = new List<Op>()
            {
                Writer,
                Pager,
                Powering,
                Producing,
                Quoting,
                Browser,
                Dumper,
                Pumper,
                Mover,
                Sorter,
                Scanner,
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
            OP_COUNT++;
            bool capped = OP_COUNT >= OP_CAP;
            OP_COUNT = capped ? 0 : OP_COUNT;
            return capped;
        }

        #endregion

        #region ENUMS
        public enum StringFormat
        {
            BODY,
            WARNING,
            BAR,
            TABLE,
            HEADER,
            SUB_HEADER,
            FOOTER,
            VALUE,
            RESOURCE,
            STATUS,
            FILTER,
            LINK,
            SLOT,
            PRODUCTION
        }
        public enum Notation
        {
            DEFAULT,
            SCIENTIFIC,
            SIMPLIFIED,
            PERCENT
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
            EXISTS
        }
        public enum WorkType
        {
            NONE,
            SCAN,
            EXISTS,
            SORT,
            MATCH,
            PUMP,
            BROWSE,
            QUOTE,
            PROD
        }
        public enum WorkResult
        {
            ERROR = -2,
            OVERHEAT = -1,

            NONE_CONTINUE = 0,
            COMPLETE = 1,

            INVENTORY_FOUND = 2,
            TYPE_FOUND = 3,
            SUB_FOUND = 4,
            SLOT_FOUND = 5,
            EXISTS = 6,
        }

        static public int ROOT_LIST_BUFFER_SIZE = Enum.GetValues(typeof(RootListType)).Length;
            //((int)Enum.GetValues(typeof(RootListType)).Cast<RootListType>().Max() -
            //(int)Enum.GetValues(typeof(RootListType)).Cast<RootListType>().Min()) + 1; // zero based indexing

        public enum RootListType
        {
            //DEFAULT = 0,

            SLOT_TALLY_ALL = 0,
            SLOT_QUEUE_SORT = 1,
            SLOT_QUEUE_DUMP = 2,
            SLOT_TALLY_REQUEST = 3,
            SLOT_TALLY_AVAILABLE = 4,
            SLOT_TALLY_LOCKED = 5,
            SLOT_INV_CONTAIN = 6,
            SLOT_INV_ALL = 7,
            SLOT_PUMP = 8,
            SLOT_MOVE = 9,

            SUB_PROD = 10,//0,
            SUB_ALL = 11,//1,
            SUB_TYPE = 12,//2,

            TYPE_ALL = 13,//0,

            ASS_PROD = 14,//0,

            INV_PULL = 15,//0,
        }
        #endregion

        #region META
        public class Root
        {
            public string Signature;
            public string Name;
            public char Char;
            public int RootID;
            public int[] ListIndices;

            public Program Program;
            public Root(RootMeta meta)
            {
                Signature = meta.Signature;
                Name = "NewRoot";
                Char = (char)0x0000;
                Program = meta.Program;
                RootID = Program.RequestIndex();
                ListIndices = new int[ROOT_LIST_BUFFER_SIZE];
                for (int i = 0; i < ListIndices.Length; i++)
                    ListIndices[i] = -1;
            }
            public virtual string MyName()
            {
                return Name;
            }
            public virtual char MyChar()
            {
                return Char;
            }
            public virtual MyFixedPoint CurrentAmount()
            {
                return 0;
            }
            public virtual MyFixedPoint? MyQuota()
            {
                return null;
            }
            public virtual void Setup()
            {
                Dlog("Root Setup");
            }
            public void Dlog(string nextLine)
            {
                Program.Debug.Append($"{nextLine}\n");
            }
            public void Fail()
            {
                Program.Fail();
            }
            public void Break(string input)
            {
                Program.Echo(input);
                Program.BREAK = true;
            }
        }
        public class RootList<R> : List<R> where R : Root
        {
            List<Work> Works = new List<Work>();
            List<Page> Pages = new List<Page>();
            public RootListType Type;
            public Program Program;

            public RootList(RootListType type, Program prog)
            {
                Type = type;
                Program = prog;
            }
            public void Dlog(string nextLine)
            {
                Program.Debug.Append($"{nextLine}\n");
            }

            public void Scope(Work work)
            {
                Works.Add(work);
            }
            public void Scope(Page page)
            {
                Pages.Add(page);
            }
            public void UnScope(Work work)
            {
                Works.Remove(work);
            }
            public void UnScope(Page page)
            {
                Pages.Remove(page);
            }
            void ResetPages()
            {
                for (int i = Pages.Count - 1; i > -1; i--)
                {
                    if (Pages[i].Dead)
                        Pages.RemoveAt(i);
                    else
                        Pages[i].Reset();
                }
            }

            public void Append(R root)
            {
                int listIndex = root.ListIndices[(int)Type];

                if (listIndex > -1 && listIndex < Count)
                {
                    Dlog("Index in range, already exists in list?");
                    return;
                }


                root.ListIndices[(int)Type] = Count;
                Add(root);

                ResetPages();


            }

            public bool Remove(int index)
            {
                if (index < 0 || index >= Count)
                {
                    return false;
                }

                Root removing = this[index]; // reference what we are removing
                this[index] = this[Count - 1]; // Overwrite with end of list
                this[index].ListIndices[(int)Type] = index; // Re-index replacement
                RemoveAt(Count - 1); // Remove copy of replacement
                removing.ListIndices[(int)Type] = -1; // cleanse what was removed


                foreach (Work work in Works)
                    work.Reset();

                ResetPages();

                return true;
            }
            new public void Remove(R root)
            {
                int listIndex = root.ListIndices[(int)Type];
                Remove(listIndex);
            }

            string RootIndices(Root root)
            {
                Program.BugBuilder.Clear();
                for (int i = 0; i < root.ListIndices.Length; i++)
                    Program.BugBuilder.Append($"{(RootListType)i}:{root.ListIndices[i]}\n");
                return Program.BugBuilder.ToString();
            }
            string RootIndices(int index)
            {
                return RootIndices(this[index]);
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
            public IMyTerminalBlock SubBlock;
            public bool Detatchable;
            public bool ConsumesPower;
            public int Priority;

            public BlockMeta(RootMeta root, IMyTerminalBlock block = null, IMyTerminalBlock sub = null, bool consumes = false, bool detachable = false, int priority = -1)
            {
                Root = root;
                Block = block;
                SubBlock = sub;
                ConsumesPower = consumes;
                Detatchable = detachable;
                Priority = priority;
            }
        }

        public struct DisplayMeta
        {
            public int SigCount;
            public Notation Notation;

            public string TargetName;
            public TargetType TargetType;

            public block TargetBlock;
            public IMyCubeGrid TargetGrid;
            public IMyBlockGroup TargetGroup;

            public DisplayMeta(int sigCount)
            {
                SigCount = sigCount;
                TargetName = "No Target";

                Notation = Notation.DEFAULT;
                TargetType = TargetType.DEFAULT;

                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }
        public struct StringMeta
        {
            public StringFormat Format;
            public Root Source;
            public string[] RawData;
            public char Char;


            public StringMeta(StringFormat form)
            {
                Format = form;
                Source = null;
                RawData = null;
                Char = (char)0x0000;
            }
            public StringMeta(StringFormat form, char single)
            {
                Format = form;
                Source = null;
                RawData = null;
                Char = single;
            }
            public StringMeta(StringFormat form, string input, bool paragraph = false)
            {
                Format = form;
                Source = null;
                RawData = input == null ? new string[1] : paragraph ? input.Split(' ') : new string[] { input };
                Char = (char)0x0000;
            }
            public StringMeta(StringFormat form, string[] data)
            {
                Format = form;
                Source = null;
                RawData = data;
                Char = (char)0x0000;
            }
            public StringMeta(StringFormat form, Root source)
            {
                Format = form;
                Source = source;
                RawData = null;
                Char = (char)0x0000;
            }
        }

        public struct JobMeta
        {
            public Root Requester;
            public string RawString;
            public Compare? Compare;
            public MyItemType? ItemTypeCompare;
            public MyFixedPoint? Quote;

            public int WIxA;
            public int WIxB;
            public bool Forward;

            public JobType JobType;
            public WorkType WorkType;
            public WorkResult Success;
            public WorkResult Fail;

            public JobMeta(
                JobType job = JobType.WORK,
                WorkType work = WorkType.NONE,
                WorkResult success = WorkResult.COMPLETE,
                WorkResult fail = WorkResult.NONE_CONTINUE,
                bool forward = true)
            {
                JobType = job;
                WorkType = work;
                Success = success;
                Fail = fail;

                Requester = null;
                Compare = null;
                RawString = null;
                ItemTypeCompare = null;
                Quote = null;

                WIxA = 0;
                WIxB = 0;
                Forward = forward;
            }

            public void SlotCompare(Slot slot)
            {
                if (slot == null)
                    return;

                Requester = slot;
                Compare = new Compare(slot.SnapShot.Type);
            }

            public void CopyJobMeta(JobMeta source)
            {
                //TargetGroup = source.TargetGroup;
                Requester = source.Requester;
                Compare = source.Compare;
                RawString = source.RawString;
                ItemTypeCompare = source.ItemTypeCompare;
                Quote = source.Quote;
            }
        }
        public struct Compare
        {
            public string type;
            public string sub;

            public Compare(string typeId, string subId)
            {
                type = typeId == null || typeId == "" ? "any" : typeId;
                sub = subId == null || typeId == "" ? "any" : subId;
            }

            public Compare(string combo)
            {
                type = "any";
                sub = "any";

                try
                {
                    string[] strings = combo.Split(Split);
                    type = strings[0];
                    sub = strings[1];

                    type = type == "" ? "any" : type;
                    sub = sub == "" ? "any" : sub;
                }
                catch { }
            }

            public Compare(MyDefinitionId defintion)
            {
                type = defintion.TypeId.ToString();
                sub = defintion.SubtypeId.ToString();
            }

            public string Type()
            {
                return type == null ? null : type.Replace(LEGACY_TYPE_PREFIX, "");
            }
        }
        public struct Flow
        {
            public MyFixedPoint? FILL, KEEP;
            public int IN, OUT;

            public Flow(MyFixedPoint? fILL, MyFixedPoint? eMPTY, int iN, int oUT)
            {
                FILL = fILL;
                KEEP = eMPTY;
                IN = iN;
                OUT = oUT;
            }

            public bool IsMoving()
            {
                return IN == 1 || OUT == 1;
            }
            public bool IsAvailable()
            {
                return IN > -1 || OUT > -1;
            }
            public bool IsLocked()
            {
                return IN == -1 && OUT == -1;
            }
        }
        #endregion

        #region VIRTUALS
        public class Page : Root
        {
            public Display MyDisplay;
            public bool ReBuild;
            public bool Dead;

            public List<StringMeta> Header = new List<StringMeta>();
            public List<StringMeta> Body = new List<StringMeta>();
            public List<StringMeta> Footer = new List<StringMeta>();

            public Page(RootMeta meta, Display display, string name = null) : base(meta)
            {
                MyDisplay = display;
                Name = name;
                //Mode = mode;
                BuildHeaders();
            }

            public void Reset()
            {
                ReBuild = true;
            }
            public void BuildHeaders()
            {
                Header.Clear();
                Footer.Clear();

                Header.Add(new StringMeta(StringFormat.HEADER, $"[{Name}]"));
                Header.Add(new StringMeta(StringFormat.HEADER));

                Footer.Add(new StringMeta(StringFormat.FOOTER));
                Footer.Add(new StringMeta(StringFormat.FOOTER, $"[{Version}]"));
            }

            public virtual void BuildBody() { }
            public virtual void TableHeaderBuilder()
            {
                /*switch (Mode)
                {
                    case ScreenMode.DEFAULT:
                        break;

                    case ScreenMode.INVENTORY:
                        AppendRawString(StringFormat.TABLE, $"[Items]{Seperator}Val|Target");
                        break;

                    case ScreenMode.RESOURCE:
                        AppendRawString(StringFormat.TABLE, $"[Source]{Seperator}Val|Uni");
                        break;

                    case ScreenMode.STATUS:
                        AppendRawString(StringFormat.TABLE, $"[Target]{Seperator}|E  P|I  H|");
                        break;
                }*/
            }
            void RawGroupBuilder(DisplayMeta meta/*IMyBlockGroup targetGroup*/)
            {
                AppendRawString(StringFormat.HEADER, $"( {meta.TargetName} )");

                TableHeaderBuilder();

                List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
                meta.TargetGroup.GetBlocks(groupList);

                foreach (IMyTerminalBlock nextTermBlock in groupList)
                {
                    block next = Program.Blocks.Find(x => x.TermBlock == nextTermBlock);
                    if (next != null)
                        RawBlockBuilder(next, false);
                    else
                        AppendRawString(StringFormat.WARNING, "Block data class not found! Signature missing from block in group!");
                    AppendRawString(StringFormat.BODY);
                }
            }
            public virtual void RawBlockBuilder(block target, bool single = true)
            {
                AppendLineSource(single ? StringFormat.HEADER : StringFormat.SUB_HEADER, target);

                /*switch (Meta.Mode)
                {
                    case ScreenMode.INVENTORY:
                        InventoryBuilder(target);
                        break;

                    case ScreenMode.PUSHING:
                        LinkBuilder(target);
                        break;

                    case ScreenMode.SORTING:
                        SortBuilder(target);
                        break;

                    case ScreenMode.RESOURCE:
                        ResBuilder(target);
                        break;
                }*/
            }

            public void AppendData(StringFormat form, string[] data)
            {
                AppendMeta(new StringMeta(form, data));
            }

            public void AppendRawString(StringFormat format, string input = null, bool paragraph = false)
            {
                AppendMeta(new StringMeta(format, input, paragraph));
            }

            public void AppendLineSource(StringFormat format, Root root)
            {
                AppendMeta(new StringMeta(format, root));
            }

            void AppendMeta(StringMeta meta)
            {
                switch (meta.Format)
                {
                    case StringFormat.HEADER:
                    case StringFormat.TABLE:
                        Header.Add(meta);
                        break;

                    case StringFormat.FOOTER:
                        Footer.Add(meta);
                        break;

                    default:
                        Body.Add(meta);
                        break;
                }
            }
        }

        /* void BuildPageBody()
            {
                BodyList.Clear();

                try
                {
                    switch (Meta.Mode)
                    {
                        case ScreenMode.DEFAULT:
                            break;

                        case ScreenMode.INVENTORY:
                            break;

                        case ScreenMode.RESOURCE:
                            break;

                        case ScreenMode.STATUS:
                            break;

                        case ScreenMode.SORTING:
                            break;

                        case ScreenMode.PRODUCTION:
                            ProductionBuilder();
                            break;

                        case ScreenMode.TALLY:
                            TallyBuilder();
                            break;

                        case ScreenMode.SLOTS:
                            SlotBuilder();
                            break;

                        case ScreenMode.PUSHING:
                            PushBuilder();
                            break;
                    }

                    switch (Meta.TargetType)
                    {
                        case TargetType.DEFAULT:
                            break;

                        case TargetType.BLOCK:
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
                    AppendRawString(StringForm.WARNING, "String Builder Error...");
                    Fail();
                }
            }
         

        void InventoryBuilder(block target)
            {
                if (!(target is Inventory))
                {
                    AppendRawString(StringForm.WARNING, "Not Inventory!");
                    return;
                }

                Inventory inventory = (Inventory)target;
                if (inventory.AllSlots.Count < 0)
                {
                    AppendRawString(StringForm.WARNING, "No Slots!");
                    return;
                }


                //AppendLine(StrForm.TABLE, $"[ ITEM ]{Split}[ COUNT/TARGET ]");

                foreach (Slot slot in inventory.AllSlots)
                    AppendLineSource(StringForm.INVENTORY, slot);
            }
            void LinkBuilder(block target)
            {
                if (!(target is Inventory))
                {
                    AppendRawString(StringForm.WARNING, "Not Link-able!");
                    return;
                }

                Inventory inventory = (Inventory)target;
                if (inventory.AllSlots.Count < 0)
                {
                    AppendRawString(StringForm.WARNING, "No Linked Tallies!");
                    return;
                }


            }
            void PushBuilder()
            {
                foreach (Slot request in Program.MoveRequests)
                {
                    AppendLineSource(StringForm.LINK, request);
                }
            }
            void SortBuilder(block target)
            {
                if (target.Profile == null)
                {
                    AppendRawString(StringForm.WARNING, "No FilterProfile!");
                    return;
                }

                AppendRawString(StringForm.SUB_HEADER, $"DEF IN:{target.Profile.DEFAULT_IN} | DEF OUT:{target.Profile.DEFAULT_OUT}");
                AppendRawString(StringForm.SUB_HEADER, $"FILL :{target.Profile.FILL      } | EMPTY :{target.Profile.EMPTY      }");

                foreach (Filter filter in target.Profile.Filters)
                    AppendLineSource(StringForm.FILTER, filter);
            }
            void ProductionBuilder()
            {
                AppendRawString(StringForm.BODY);

                foreach (TallyItemSub prod in Program.Productions)
                {
                    //string nextDef = prod.Type.SubtypeId;
                    //ProdCharBuffer = (ProdCharBuffer > nextDef.Length) ? ProdCharBuffer : nextDef.Length;


                    AppendLineSource(StringForm.PRODUCTION, prod);
                }
            }
            void SlotBuilder()
            {
                MyFixedPoint? blah;
                foreach (TallyItemType type in Program.AllItemTypes)
                {
                    if (!ProfileCompare(Profile, type.TypeId, out blah))
                        continue;

                    AppendLineSource(StringForm.SUB_HEADER, type);
                    foreach (TallyItemSub subType in type.SubTypes)
                    {
                        if (!ProfileCompare(Profile, subType.Type, out blah))
                            continue;

                        AppendLineSource(StringForm.INVENTORY, subType);
                        AppendRawString(StringForm.BAR);

                        foreach (Slot slot in subType.Tallies[(int)RootListType.SLOT_TALLY_ALL])
                            AppendLineSource(StringForm.INVENTORY, slot);

                        AppendRawString(StringForm.BAR);
                        AppendRawString(StringForm.BODY);
                    }

                    AppendRawString(StringForm.BODY);
                }
            }
            void ResBuilder(block targetBlock)
            {
                if (!(targetBlock is Resource) &&
                    !(targetBlock is InvResource))
                {
                    AppendRawString(StringForm.WARNING, "Not a Resource!");
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

                AppendLineSource(StringForm.RESOURCE, resource);
            }

         
         */
        public class DefaultPage : Page
        {
            public DefaultPage(RootMeta meta, Display display) : base(meta, display, "Default")
            {
            }

            public override void BuildBody()
            {
                Body.Clear();

                AppendRawString(StringFormat.BODY,
                    "This is a test paragraph for the body wrapper! Hello\nHello\nHello\nAm I working as intended?", true);
            }
        }
        public class TallyPage : Page
        {
            public TallyPage(RootMeta meta, Display display) : base(meta, display, "Tally")
            {
                Program.AllItemTypes.Scope(this);
            }

            public override void BuildBody()
            {
                Body.Clear();

                MyFixedPoint? blah;
                foreach (TallyItemType type in Program.AllItemTypes)
                {
                    type.SubTypes.UnScope(this);

                    if (!ProfileCompare(MyDisplay.Profile, type.TypeId, out blah))
                        continue;

                    type.SubTypes.Scope(this);

                    AppendLineSource(StringFormat.SUB_HEADER, type);
                    foreach (TallyItemSub subType in type.SubTypes)
                    {
                        if (!ProfileCompare(MyDisplay.Profile, subType.Type, out blah))
                            continue;

                        //Dlog("Filter pass!");

                        //Dlog($"MyDisplay.QUOTE: {MyDisplay.QUOTE}\n" +
                        //    $"subType.TargetQuota.HasValue: {subType.TargetQuota.HasValue}");

                        if (MyDisplay.QUOTE && !subType.TargetQuota.HasValue)
                            continue;

                        //Dlog("Quota pass!");

                        AppendLineSource(StringFormat.VALUE, subType);
                    }

                    AppendRawString(StringFormat.BODY);
                }
            }
        }


        public class Slot : Root
        {
            public int ItemIndex;
            public int InvIndex;

            public IMyInventory Container;

            public Inventory Inventory;
            public TallyItemSub Profile;
            public Slot InLink;
            public Slot OutLink;

            public Flow Flow;
            public MyInventoryItem SnapShot;

            public Slot(RootMeta meta, Inventory inventory, int inventoryIndex, int itemIndex) : base(meta)
            {
                ItemIndex = itemIndex;
                InvIndex = inventoryIndex;

                Inventory = inventory;
                Container = Inventory.Owner.GetInventory(InvIndex);
                SnapShot = Container.GetItemAt(itemIndex).Value;

                SlotFilter();
            }
            void SlotFilter()
            {

                if (Inventory == null)
                    return;

                MyFixedPoint? inTarget, outTarget;

                bool inAllowed = ProfileCompare(Inventory.Profile, SnapShot.Type, out inTarget);
                bool outAllowed = ProfileCompare(Inventory.Profile, SnapShot.Type, out outTarget, false);

                Flow = new Flow(inTarget, outTarget,
                    inAllowed ? (Inventory.Profile.FILL ? 1 : 0) : -1,
                    outAllowed ? (Inventory.Profile.EMPTY ? 1 : 0) : -1);


                if (Flow.IsMoving())
                {
                    Program.MoveRequests.Append(this);
                }

                else
                {
                    Program.MoveRequests.Remove(this);
                }



                if (IsLinkable() && Flow.IsMoving())
                    Program.PumpRequests.Append(this);
                else
                    Program.PumpRequests.Remove(this);

            }

            public override string MyName()
            {
                return $"{SnapShot.Type.SubtypeId}";
            }
            public string InventoryName()
            {
                return $"{((Inventory == null) ? "No inventory" : $"[{(InvIndex == 0 ? "Input" : InvIndex == 1 ? "Output" : InvIndex.ToString())} | {ItemIndex}] {Inventory.MyName()}")}";
            }
            public override MyFixedPoint CurrentAmount()
            {
                MyInventoryItem? item = PullItem();
                return item.HasValue ? item.Value.Amount : 0;
            }
            public void Update(int index)
            {
                ItemIndex = index;
                MyFixedPoint delta;

                if (!UpdateSlot(out delta))
                {
                    Unlink();
                    UnsyncFromProfile();
                    SlotFilter();
                }

                if (Profile != null)
                {
                    Profile.Update(delta);
                }
                else
                {
                    Program.Sorter.Queue.Append(this);
                }
            }
            public void Pump()
            {
                if (!Refresh())
                    return;

                if (Flow.IN == 1)
                {
                    if (InLink == null)
                    {

                    }
                    else if (!InLink.Refresh())
                    {
                        InLink = null;
                    }
                    else if (!TallyTransfer(InLink, this))
                    {

                    }
                    else
                    {

                    }
                }

                if (Flow.OUT == 1)
                {
                    if (OutLink == null)
                    {

                    }
                    else if (!OutLink.Refresh())
                    {
                        OutLink = null;
                    }
                    else if (!TallyTransfer(this, OutLink))
                    {

                    }
                    else
                    {

                    }
                }
            }
            public bool UpdateSlot(out MyFixedPoint delta)
            {
                MyInventoryItem? check;
                delta = SnapShot.Amount;

                bool checkResult = CheckSlot(out check);

                if (checkResult)
                    delta = check.Value.Amount - SnapShot.Amount;
                else
                    delta = -SnapShot.Amount;
                if(check.HasValue)
                    SnapShot = check.Value; // OverWrite

                return checkResult;
            }

            public MyInventoryItem? PullItem()
            {
                return Container == null ? null : Container.GetItemAt(ItemIndex);
            }
            public bool CheckSlot()
            {
                MyInventoryItem? empty;
                return CheckSlot(out empty);
            }
            public bool CheckSlot(out MyInventoryItem? check)
            {
                check = PullItem();
                return check.HasValue && check.Value.Type == SnapShot.Type;
            }

            public bool Refresh() // Only successful check updates the slot. Safe operational check
            {
                MyInventoryItem? check;
                bool result = CheckSlot(out check);
                return result;
            }
            /*public bool HasOverride()
            {
                return Inventory is Producer && Flow.IN == 1;
            }*/
            public bool IsLinkable()
            {
                return (Inventory is Producer && Inventory.IsManaged());// || IsStockpile();
            }
            public bool IsStockpile()
            {
                return Flow.IN == 1 && Flow.OUT == -1;
            }
            public bool IsLinked()
            {
                if (Flow.IN == 1 && InLink == null)
                {

                    return false;
                }

                if (Flow.OUT == 1 && OutLink == null)
                {
                    return false;
                }

                return true;
            }
            public bool IsSorted()
            {
                return Profile != null;
            }
            public bool IsBroken()
            {
                return !Refresh() || !IsSorted();
            }
            public bool IsFull()
            {
                return IsFilled() || Inventory.CheckFull(Container);
            }
            public bool IsFilled()
            {
                return /*Filter.IN == 1 && */Flow.FILL.HasValue && SnapShot.Amount >= Flow.FILL;
            }
            public bool IsEmpty()
            {
                return IsEmptied() || Inventory.CheckEmpty(Container);
            }
            public bool IsEmptied()
            {
                return /*Filter.OUT == 1 && */Flow.KEEP.HasValue && SnapShot.Amount <= Flow.KEEP;
            }
            public void Kill()
            {
                Unlink();
                UnsyncFromProfile();
                Inventory.ContainedSlots[InvIndex].RemoveAt(ItemIndex);

                Inventory.AllSlots.Remove(this);
                Inventory = null;
                Container = null;
            }
            public void SyncTallyToProfile(TallyItemSub profile)
            {
                if (profile == null)
                    return;

                Profile = profile;

                Profile.Tallies[(int)RootListType.SLOT_TALLY_ALL].Append(this);

                if (Flow.IsLocked())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_LOCKED].Append(this);
                //if (Flow.IN == 0 && Flow.OUT == 0)
                //    Profile.Tallies[(int)RootListType.STORAGE], this);
                //if (CheckLinkable())
                //    Profile.Tallies[(int)RootListType.LINKABLE], this);
                if (Flow.IsMoving())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_REQUEST].Append(this);
                if (Flow.IsAvailable())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_AVAILABLE].Append(this);
            }
            void UnsyncFromProfile()
            {
                if (Profile == null)
                    return;

                Profile.Tallies[(int)RootListType.SLOT_TALLY_ALL].Remove(this);

                if (Flow.IsLocked())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_LOCKED].Remove(this);
                //if (Flow.IN == 0 && Flow.OUT == 0)
                //    Profile.Tallies[(int)RootListType.STORAGE].Remove(this);
                //if (CheckLinkable())
                //    Profile.Tallies[(int)RootListType.LINKABLE].Remove(this);
                if (Flow.IsMoving())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_REQUEST].Remove(this);
                if (Flow.IsAvailable())
                    Profile.Tallies[(int)RootListType.SLOT_TALLY_AVAILABLE].Remove(this);

                Profile.Update(-SnapShot.Amount);
                Profile = null;
            }
            void Unlink()
            {
                if (Flow.IsMoving())
                {
                    Program.MoveRequests.Remove(this);
                }

                if (IsLinkable())
                {
                    Program.PumpRequests.Remove(this);
                }

                if (InLink != null)
                {
                    InLink.OutLink = null;
                    InLink = null;
                }

                if (OutLink != null)
                {
                    OutLink.InLink = null;
                    OutLink = null;
                }
            }
        }
        public class Filter : Root
        {
            public Compare Compare;
            public Flow Flow;

            public bool IsMoving()
            {
                return Flow.IsMoving();
            }
            Filter(RootMeta meta, Flow flow) : base(meta)
            {
                Flow = flow;
            }
            public Filter(RootMeta meta, string combo, Flow flow) : this(meta, flow)
            {
                Compare = new Compare(combo);
            }
            public Filter(RootMeta meta, MyDefinitionId definition, Flow flow) : this(meta, flow)
            {
                Compare = new Compare(definition);
            }
        }
        public class FilterProfile : Root
        {
            public List<Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool TRUE_FULL;
            public bool EMPTY;
            public bool CLEAN;
            public bool ACTIVE_CONVEYOR = true;
            //public bool QUOTE;

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

                try
                {
                    string[] data = customData.Split('\n');

                    foreach (string nextline in data)
                    {
                        if (nextline.Length == 0)
                            continue;

                        /// OPTION CHANGE ///

                        if (nextline == Terminate)
                            break;

                        if (nextline[0] == '&')
                        {
                            if (Contains(nextline, "empty"))
                                EMPTY = !nextline.Contains("-");

                            if (Contains(nextline, "fill"))
                                FILL = !nextline.Contains("-");

                            if (Contains(nextline, "full"))
                                TRUE_FULL = !nextline.Contains("-");

                            //if (Contains(nextline, "res"))
                            //    RESIDE = !nextline.Contains("-");

                            if (Contains(nextline, "clean"))
                                CLEAN = !nextline.Contains("-");

                            if (Contains(nextline, "convey"))
                                ACTIVE_CONVEYOR = !nextline.Contains("-");
                            


                            continue;
                        }

                        /// FILTER CHANGE ///
                        AppendFilter(nextline);
                    }
                }
                catch { Dlog("Profile Build Error!"); }

                Filters.RemoveAll(x => x.Compare.type == "any" && x.Compare.sub == "any");  // Redundant. Refer to inventory default mode

                return true;
            }
            public void AppendFilter(string nextline)
            {
                /// FILTER CHANGE ///

                string[] lineblocks = nextline.Split(' ');  // Break each line into blocks

                if (lineblocks.Length < 2)  // There must be more than one block to have filter candidate and desired update
                    return;

                string itemID = null; // Default target, don't update any filters
                bool bDefault = false;
                bool bIn = true;
                bool bOut = true;
                MyFixedPoint? inTarget = null;
                MyFixedPoint? outTarget = null;

                if (lineblocks[0].Contains(Split)) // Filter insignia
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
                            string[] targets = lineblocks[i].Remove(0, 1).Split(Split);
                            try
                            {
                                inTarget = (MyFixedPoint)float.Parse(targets[0]);
                                outTarget = (MyFixedPoint)float.Parse(targets[1]);
                            }
                            catch { }
                            //inTarget = (MyFixedPoint)float.Parse(lineblocks[i].Remove(0, 1));
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

                if (itemID != null)
                {
                    Flow flow = new Flow(inTarget, outTarget, bIn ? FILL ? 1 : 0 : -1, bOut ? EMPTY ? 1 : 0 : -1);
                    Filters.Add(new Filter(Program.ROOT, itemID, flow));
                }

            }
        }
        public class TallyItemType : Root
        {
            public RootList<TallyItemSub> SubTypes;// = new RootList<TallyItemSub>(RootListType.SUB_TYPE);
            public string TypeId;

            public TallyItemType(RootMeta meta, TallyItemSub first) : base(meta)
            {
                SubTypes = new RootList<TallyItemSub>(RootListType.SUB_TYPE, meta.Program);
                SubTypes.Append(first);
                TypeId = first.Type.TypeId;
                Name = Type();
                if (Name == "")
                    Name = "Miscellaneous";
            }

            public string Type()
            {
                return TypeId.Replace(LEGACY_TYPE_PREFIX, "").Replace("Physical", "").Replace("Object", "");
            }

            /*public override char MyChar()
            {
                switch(Name)
                {
                    case "Ore":
                        return '*';//rgb(200, 0, 0);//(char)0x25A9;

                    case "Ingot":
                        return '$';//rgb(0, 200, 0);//(char)0x25B1;

                    case "Component":
                        return '@';//rgb(0, 0, 200);//(char)0x25A3;

                    case "AmmoMagazine":
                        return 

                    default:
                        return Bar;
                }
            }*/
        }
        public class TallyItemSub : Root
        {
            public MyFixedPoint? TargetQuota;
            public MyFixedPoint CurrentTotal;
            public MyFixedPoint HighestReached;
            public MyItemType Type;

            public RootList<Slot>[] Tallies = new RootList<Slot>[ROOT_LIST_BUFFER_SIZE];
            public List<Assembler> Assemblers = new List<Assembler>();
            //public RootList<Assembler> Assemblers = new RootList<Assembler>(RootListType.ASS_PROD);
            public TallyItemSub(RootMeta meta, Slot first) : base(meta)
            {
                Tallies[(int)RootListType.SLOT_TALLY_ALL] = new RootList<Slot>(RootListType.SLOT_TALLY_ALL, meta.Program);
                Tallies[(int)RootListType.SLOT_TALLY_AVAILABLE] = new RootList<Slot>(RootListType.SLOT_TALLY_AVAILABLE, meta.Program);
                Tallies[(int)RootListType.SLOT_TALLY_REQUEST] = new RootList<Slot>(RootListType.SLOT_TALLY_REQUEST, meta.Program);
                Tallies[(int)RootListType.SLOT_TALLY_LOCKED] = new RootList<Slot>(RootListType.SLOT_TALLY_LOCKED, meta.Program);

                //for (int i = 0; i < Tallies.Length; i++)
                //Tallies[i] = new RootList<Slot>();

                if (first == null)
                    return;

                Type = first.SnapShot.Type;
                Append(first);
                Name = SubType();
                //Setup();
            }

            public Slot GetSlot(RootListType group, int index)
            {
                try { return Tallies[(int)group][index]; }
                catch { return null; }
            }

            public void SetQuota(MyFixedPoint? quote = null)
            {
                TargetQuota = quote;
            }
            public override MyFixedPoint? MyQuota()
            {
                return TargetQuota;
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
            public string SubType()
            {
                return Type.SubtypeId;
            }
            public override MyFixedPoint CurrentAmount()
            {
                return CurrentTotal;
            }
        }

        public class Work : Root
        {
            public Op MyOp;
            public Work Chain;
            public JobMeta Job;
            public MyFixedPoint? FixedPoint;

            public int IterationIndex;
            public int SearchIndex;
            public int SearchCount;

            public Work(RootMeta meta, Op op) : base(meta)
            {
                MyOp = op;
            }
            public WorkResult WorkLoad()
            {
                MyOp.CurrentWork = this;

                return IterateScope(Job.Forward);
            }

            public void Reset()
            {
                IterationIndex = 0;
                //SearchIndex = 0;
                if (Chain != null)
                    Chain.Reset();
            }
            public virtual void SetSearchCount()
            {

            }
            public virtual bool Compare()
            {
                return false;
            }
            public virtual bool DoWork()
            {
                return false; // Task/Search Proced
            }
            public virtual WorkResult ChainCall()
            {
                return WorkResult.NONE_CONTINUE;
            }

            public virtual WorkResult IterateScope(bool forward)
            {

                WorkResult myResult = WorkResult.NONE_CONTINUE;

                for (int i = IterationIndex; i < SearchCount; i++)
                {
                    IterationIndex = i;
                    SearchIndex = forward ? i : SearchCount - (i + 1);
                    myResult = IterateProc();

                    if (myResult == WorkResult.OVERHEAT)
                    {
                        Dlog("PAUSED AT:\n" +
                            $"Iteration:{IterationIndex} / Search:{SearchIndex} / Count:{SearchCount}");
                        return myResult;
                    }

                    if (Job.JobType == JobType.FIND && myResult > 0)
                    {
                        if (Chain != null)
                        {
                            WorkResult chainResult = ChainCall();
                            if (chainResult == WorkResult.OVERHEAT)
                                return chainResult;

                            if (Chain.Job.JobType == JobType.EXISTS &&
                                chainResult == WorkResult.EXISTS)
                                continue;

                            if (chainResult > 0)
                                return chainResult;
                        }
                        return myResult;
                    }

                    if (Job.JobType == JobType.EXISTS && myResult > 0)
                    {
                        IterationIndex = 0;
                        return myResult;
                    }

                    if (Job.JobType == JobType.WORK)
                    {
                        if (Chain != null)
                            myResult = ChainCall();

                        if (myResult > 0)
                            return myResult;
                    }
                }

                if (Job.JobType == JobType.EXISTS && Chain != null)
                {
                    IterationIndex = 0;
                    return ChainCall();
                }

                return myResult;
            }
            WorkResult IterateProc()
            {

                if (CheckOp())
                    return WorkResult.OVERHEAT;

                switch (Job.JobType)
                {
                    case JobType.FIND:
                        return Compare() ? Job.Success : Job.Fail;

                    case JobType.EXISTS:
                        return Compare() ? Job.Success : Job.Fail;

                    case JobType.WORK:
                        return DoWork() ? Job.Success : Job.Fail;

                    default:
                        return WorkResult.ERROR;
                }
            }
            bool CheckOp()
            {
                bool maxed = Program.CallCounter();
                return maxed;
            }
        }
        public class InventoryWork : Work
        {
            List<Inventory> InventoryList;
            public InventoryWork(RootMeta meta, Op op) : base(meta, op)
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
                    SearchCount = 0;
                    return;
                }

                SearchCount = InventoryList.Count;
            }
            public override bool Compare()
            {
                return Job.Requester is Slot && ProfileCompare(InventoryList[SearchIndex], (Slot)Job.Requester, out FixedPoint);
            }

            public Inventory Current()
            {
                return InventoryList == null ? null : InventoryList[SearchIndex];
            }
        }
        public class ContainerWork : Work
        {
            public ContainerWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "CONTAINER";
            }

            public override void SetSearchCount()
            {
                if (Job.Requester is Inventory)
                    SearchCount = ((Inventory)Job.Requester).Owner.InventoryCount;
                else
                {
                    SearchCount = 0;
                }

            }
            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                Chain.Job.WIxA = SearchIndex;
                Chain.SetSearchCount();

                return Chain.WorkLoad();
            }

            public IMyInventory Current()
            {
                return ((Inventory)Job.Requester).Owner.GetInventory(SearchIndex);
            }
        }
        public class AssemblerWork : Work
        {
            List<Assembler> AssemblerList;

            public AssemblerWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "ASSEMBLER";
            }

            public override void SetSearchCount()
            {
                AssemblerList = null;

                switch (Job.WorkType)
                {
                    case WorkType.QUOTE:
                        AssemblerList = Program.Assemblers;
                        break;

                    case WorkType.PROD:
                        AssemblerList = ((TallyItemSub)Job.Requester).Assemblers;
                        break;
                }

                SearchCount = AssemblerList == null ? 0 : AssemblerList.Count;
            }

            public Assembler Current()
            {
                return AssemblerList == null ? null : AssemblerList[SearchIndex];
            }

            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                Chain.Job.WIxA = SearchIndex;
                Chain.SetSearchCount();

                return Chain.WorkLoad();
            }

            public override bool DoWork()
            {
                switch (Job.WorkType)
                {
                    case WorkType.QUOTE:

                        Assembler candidate = Current();
                        if (candidate != null && CanAssemble())
                        {
                            ((TallyItemSub)Job.Requester).Assemblers.Add/*Append*/(candidate);
                        }
                            
                        return false; // keep working

                    //case WorkType.PROD:
                    //    Produce();
                    //    return false; // keep working

                    default:
                        return false;
                }
            }
            bool CanAssemble()
            {
                IMyAssembler assembler = Program.Assemblers[SearchIndex].AssemblerBlock;
                MyDefinitionId? recipe = Recipe(((TallyItemSub)Job.Requester).Type);
                return recipe.HasValue && assembler.CanUseBlueprint(recipe.Value);
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
                SearchCount = Program.AllItemTypes.Count;
            }
            public override bool Compare()
            {
                TallyItemType tallyType = Program.AllItemTypes[SearchIndex];
                Dlog($"Comparing type: {(Job.Requester is Inventory ? ((Inventory)Job.Requester).MyName() : Job.Compare.HasValue ? Job.Compare.Value.Type() : "NO COMPARE")} || {tallyType.TypeId}");

                return
                    (Job.Requester is Inventory && ProfileCompare(((Inventory)Job.Requester).Profile, tallyType.TypeId, out FixedPoint, true))

                    ||

                    (Job.Compare.HasValue && (Job.Compare.Value.type == "any" || tallyType.TypeId == Job.Compare.Value.type));
            }

            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                Chain.Job.WIxA = SearchIndex;
                Chain.SetSearchCount();

                return Chain.WorkLoad();
            }
        }
        public class SubWork : Work
        {
            RootList<TallyItemSub> SubList;

            public SubWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SUBTYPE";
            }

            TallyItemSub Current()
            {
                return SubList == null ? null : SubList[SearchIndex];
            }
            public override void SetSearchCount()
            {
                if (SubList != null)
                {
                    SubList.UnScope(this);
                    SubList = null;
                }


                switch (Job.WorkType)
                {
                    case WorkType.BROWSE:
                    case WorkType.SORT:
                        SubList = Program.AllItemTypes[Job.WIxA].SubTypes;
                        break;

                    case WorkType.QUOTE:
                        SubList = Program.AllItemSubs;
                        break;
                }

                if (SubList != null)
                {
                    SubList.Scope(this);
                }

                SearchCount = SubList == null ? 0 : SubList.Count;
            }
            public override bool Compare()
            {
                TallyItemSub sub = SubList[SearchIndex];

                return
                    (Job.Requester is Inventory && ProfileCompare(((Inventory)Job.Requester).Profile, out FixedPoint, sub.Type.SubtypeId, true))

                    ||

                    (Job.Compare.HasValue && (Job.Compare.Value.sub == "any" || sub.Type.SubtypeId == Job.Compare.Value.sub));
            }

            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);

                if (Chain.Job.JobType == JobType.EXISTS)
                {
                    Dlog($"Setting Chain Type Compare: {Current().Type}");
                    Chain.Job.ItemTypeCompare = Current().Type;
                }
                    

                Chain.Job.WIxA = Job.WIxA;
                Chain.Job.WIxB = SearchIndex;
                Chain.SetSearchCount();

                return Chain.WorkLoad();
            }
        }
        public class SlotWork : Work
        {
            public RootList<Slot> SlotList;
            public SlotWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "SLOT";
            }

            public override void SetSearchCount()
            {
                if (SlotList != null)
                {
                    SlotList.UnScope(this);
                    SlotList = null;
                }

                IMyInventory inv = null;

                switch (Job.WorkType)
                {
                    case WorkType.SCAN:
                        Inventory scan = ((Inventory)Job.Requester);
                        inv = scan.PullInventory(Job.WIxA);
                        SlotList = scan.ContainedSlots[Job.WIxA];
                        break;

                    case WorkType.EXISTS:
                        SlotList = ((Inventory)Job.Requester).AllSlots;
                        break;

                    case WorkType.MATCH:
                        if (Job.Requester is Slot && !((Slot)Job.Requester).IsBroken())
                        {
                            SlotList = ((Slot)Job.Requester).Profile.Tallies[(int)RootListType.SLOT_TALLY_AVAILABLE];
                        }
                        break;

                    case WorkType.PUMP:
                        SlotList = Program.PumpRequests;
                        break;

                    case WorkType.BROWSE:
                        SlotList = Program.AllItemTypes[Job.WIxA].SubTypes[Job.WIxB].Tallies[(int)RootListType.SLOT_TALLY_AVAILABLE];
                        break;
                }

                if (SlotList == null)
                {
                    Dlog("Null SlotList!");
                    SearchCount = 0;
                    return;
                }

                SlotList.Scope(this);

                if (inv != null)
                {
                    SearchCount = SlotList.Count > inv.ItemCount ? SlotList.Count : inv.ItemCount;
                }


                else
                    SearchCount = SlotList.Count;

            }
            public override bool Compare()
            {
                Slot current = Current();
                if (current == null)
                    return false;

                if (!current.Refresh())
                    return false;

                Dlog($"Checking slot: [{current.InventoryName()}:{current.SnapShot.Type.SubtypeId}]\n" +
                    $"Current ItemTypeCompare: {(Job.ItemTypeCompare.HasValue ? $"{Job.ItemTypeCompare.Value}" : "None")}");

                if (Job.Requester is Inventory)
                {
                    if (Job.JobType == JobType.EXISTS)
                        return TypeCompare(Job.ItemTypeCompare.Value, current);
                    else
                        return ProfileCompare((Inventory)Job.Requester, current, out FixedPoint);
                }
                    
                return Job.Requester is Slot && SlotCompare((Slot)Job.Requester, current, out FixedPoint);
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

                    case WorkType.MATCH:
                        return SlotMatch();

                    case WorkType.BROWSE:
                        return SlotBrowse();

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

                return Chain.WorkLoad();
            }


            public Slot Current()
            {
                return SlotList == null ? null : SlotList[SearchIndex];
            }

            bool SlotScan()
            {
                Inventory inv = (Inventory)Job.Requester;
                IMyInventory container = inv.PullInventory(Job.WIxA);

                if (SearchIndex >= SlotList.Count) // Not enough slots
                {
                    Dlog("Adding slot");
                    Slot newSlot = new Slot(Program.ROOT, inv, Job.WIxA, SearchIndex);
                    SlotList.Append(newSlot);
                    inv.AllSlots.Append(newSlot);
                }

                if (SearchIndex >= container.ItemCount) // Too many slots
                {
                    Dlog("Killing slot");
                    int last = SlotList.Count - 1;
                    SlotList[last].Kill();
                }

                else
                {
                    Dlog("Updating slot");
                    SlotList[SearchIndex].Update(SearchIndex);
                }

                Dlog("Slot Scanned!");
                return false; // Keep Working!
            }
            bool SlotMatch()
            {
                Slot requester = (Slot)Job.Requester;
                Slot candidate = Current();

                if (!requester.Refresh())
                {
                    Dlog("Broken slot, skipping...");
                    return true;
                }
                if (requester.IsLinkable()) // Moving handled by pumper, only search for links
                {
                    Dlog("Link-able!");
                    if (requester.IsLinked())
                    {
                        Dlog("Already linked!");
                        return true;
                    }
                    SlotLink();
                }
                else
                {
                    Dlog("Move-able!");

                    // Can move logic? Maybe too full/empty?

                    SlotMove();
                }
                return false; // Keep Working!
            }
            void SlotLink()
            {
                Slot queued = (Slot)Job.Requester;
                Slot match = Current();


                if (!match.IsStockpile())
                {
                    return;
                }

                if (!Compare())
                {
                    return;
                }


                if (queued.Inventory is Producer &&
                    match.Inventory is Producer)
                {
                    return;
                }

                if (queued.Flow.IN == 1 &&
                    queued.InLink == null)
                    //!match.CheckLinkable() &&
                    //(queued.HasOverride() ||
                    //match.Flow.OUT > -1))
                {
                    Dlog($"In Link Made!");

                    queued.InLink = match;
                    match.OutLink = queued;
                }

                if (queued.Flow.OUT == 1 &&
                    queued.OutLink == null)//&&
                    //!match.CheckLinkable() &&
                   // match.Flow.IN > -1 &&
                    //!match.IsLinkable())
                {
                    Dlog($"Out Link Made!");

                    queued.OutLink = match;
                    match.InLink = queued;
                }
            }
            void SlotMove()
            {
                Slot requester = (Slot)Job.Requester;
                Slot candidate = Current();

                if (candidate.IsLinkable())
                {
                    return;
                }

                if (!Compare())
                {
                    Dlog("Bad compare!");
                    return;
                }



                if (requester.Flow.IN == 1 && MoveCompare(candidate, requester))
                {
                    bool pullSuccess = TallyTransfer(candidate, requester);
                    Dlog($"Pull success: {pullSuccess}");
                    //if (requester.CheckFull())
                    //return true;
                }

                if (requester.Flow.OUT == 1 && MoveCompare(requester, candidate))
                {
                    bool pushSuccess = TallyTransfer(requester, candidate);
                    Dlog($"Push success: {pushSuccess}");
                    //if (requester.CheckEmpty())
                    //return true;
                }
            }
            bool SlotPump()
            {
                Current().Pump();
                return false; // Keep working!
            }
            bool SlotBrowse()
            {
                Inventory requester = (Inventory)Job.Requester;

                if (!Compare())
                {
                    Dlog("Bad Compare!");
                    return false; // Keep browsing!
                }

                return ForceTransfer(requester, FixedPoint, Current()); // Pull one unique item per cycle?


                //return true;
            }
        }
        public class ProductionWork : Work
        {
            List<MyProductionItem> ProdList = new List<MyProductionItem>();
            public ProductionWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "PRODUCTION";
            }

            public override void SetSearchCount()
            {
                GenerateQueuePortion();
                ProdList.Clear();
                ((TallyItemSub)Job.Requester).Assemblers[Job.WIxA].AssemblerBlock.GetQueue(ProdList);
                SearchCount = ProdList.Count;


            }

            void GenerateQueuePortion()
            {
                TallyItemSub sub = (TallyItemSub)Job.Requester;
                FixedPoint = sub.TargetQuota.HasValue ? (MyFixedPoint)(((float)sub.TargetQuota.Value - (float)sub.CurrentTotal) / sub.Assemblers.Count) : 0;
                FixedPoint = FixedPoint < 0 ? 0 : FixedPoint < 1 && FixedPoint > 0 ? 1 : (int)FixedPoint;
            }
            public override WorkResult IterateScope(bool forward)
            {
                WorkResult result = base.IterateScope(forward);
                if (SearchIndex == 0//mainResult == WorkResult.NONE_CONTINUE
                    && FixedPoint.HasValue
                    && FixedPoint.Value >= 1) // New queue needed
                {
                    TallyItemSub sub = (TallyItemSub)Job.Requester;
                    sub.Assemblers[Job.WIxA].AssemblerBlock.AddQueueItem(Recipe(sub.Type).Value, FixedPoint.Value);
                }
                return result;
            }

            public override bool DoWork()
            {
                Dlog($"Production Work Indices: {SearchIndex}/{SearchCount}");

                switch (Job.WorkType)
                {
                    case WorkType.PROD:
                        Produce();
                        Dlog($"SearchIndex: {SearchIndex} || FixedPoint: {(FixedPoint.HasValue ? FixedPoint.Value.ToString() : "None")}");
                        return false; // Keep working!
                        
                        //SearchIndex == 0 && !FixedPoint.HasValue; // All Queues checked and targetQueue updated!
                        //SearchIndex == 0 && FixedPoint.HasValue && FixedPoint.Value > 0; // New queue needed
                }
                return false;
            }

            void Produce()
            {
                TallyItemSub sub = (TallyItemSub)Job.Requester;
                if (sub.Type.SubtypeId != ProdList[SearchIndex].BlueprintId.SubtypeName)
                {
                    return;
                }

                if (FixedPoint == null)
                {
                    sub.Assemblers[Job.WIxA].AssemblerBlock.RemoveQueueItem(SearchIndex, ProdList[SearchIndex].Amount);
                    return;
                }

                MyFixedPoint correction = FixedPoint.Value - ProdList[SearchIndex].Amount; // first recipe found
                if (correction > 0)
                {
                    sub.Assemblers[Job.WIxA].AssemblerBlock.InsertQueueItem(SearchIndex, ProdList[SearchIndex].BlueprintId, correction);
                    FixedPoint = null;
                }
                else if (correction < 0)
                {
                    sub.Assemblers[Job.WIxA].AssemblerBlock.RemoveQueueItem(SearchIndex, -correction);
                    FixedPoint = null;
                }
                else
                {
                    FixedPoint = null;
                }
            }
        }
        public class StringWork : Work
        {
            public string[] PageBuffer;

            public StringWork(RootMeta meta, Op op) : base(meta, op)
            {
                Name = "STRING";
            }

            public override void SetSearchCount()
            {
                try
                {
                    switch (Job.WorkType)
                    {
                        case WorkType.QUOTE:
                            PageBuffer = Job.RawString.Split('\n');
                            SearchCount = PageBuffer.Length;
                            break;
                    }
                }
                catch { SearchCount = 0; }
            }
            public override bool Compare()
            {
                return

                    (Job.WorkType == WorkType.QUOTE && RawCompare(((TallyItemSub)Job.Requester).Type, PageBuffer[SearchIndex], out FixedPoint, this))

                    ;
            }

            public override WorkResult ChainCall()
            {
                Chain.Job.CopyJobMeta(Job);
                if (Chain.Job.WorkType == WorkType.QUOTE && Chain.Job.Requester is TallyItemSub)
                {
                    Program.Productions.Append((TallyItemSub)Chain.Job.Requester);
                }

                Chain.SetSearchCount();

                return Chain.WorkLoad();
            }
        }

        public class Op : Root
        {
            public bool Active;
            public bool Complete;
            public Work CurrentWork;

            public int
                WorkIndex,
                WorkCount,
                WorkTotal;

            public Op(Program prog, bool active) : base(prog.ROOT)
            {
                Active = active;
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
                if (result)
                    Complete = true;
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
            public RootList<Slot> Queue;// = new RootList<Slot>();

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
                Queue = new RootList<Slot>(RootListType.SLOT_QUEUE_SORT, prog);

                TypeMatch = new TypeWork(prog.ROOT, this);
                TypeMatch.Job = new JobMeta(JobType.FIND, WorkType.SORT, WorkResult.TYPE_FOUND);

                SubMatch = new SubWork(prog.ROOT, this);
                SubMatch.Job = new JobMeta(JobType.FIND, WorkType.SORT, WorkResult.SUB_FOUND);
                TypeMatch.Chain = SubMatch;
            }

            public override void Run()
            {
                base.Run();

                TypeMatch.Job.SlotCompare(Queue[0]);
                TypeMatch.SetSearchCount();

                if (!Queue[0].Refresh() || Sort())
                {
                    Queue.Remove(0);
                    TypeMatch.Reset();
                }
            }
            bool Sort()
            {
                TallyItemType newType;
                TallyItemSub newSubType;

                switch (TypeMatch.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE:
                        Dlog("Not found!\nNew Type/Sub/Slot");
                        newSubType = new TallyItemSub(Program.ROOT, Queue[0]);
                        newType = new TallyItemType(Program.ROOT, newSubType);
                        Program.AllItemTypes.Append(newType);
                        Program.AllItemSubs.Append(newSubType);
                        return true;

                    case WorkResult.TYPE_FOUND:
                        Dlog("Type Found!\nNew Sub/Slot");
                        newSubType = new TallyItemSub(Program.ROOT, Queue[0]);
                        Program.AllItemTypes[TypeMatch.SearchIndex].SubTypes.Append(newSubType);
                        Program.AllItemSubs.Append(newSubType);
                        return true;

                    case WorkResult.SUB_FOUND:
                        Dlog("Sub Type Found!\nNew Slot");
                        Program.AllItemTypes[TypeMatch.SearchIndex].SubTypes[SubMatch.SearchIndex].Append(Queue[0]);
                        return true;

                    default:
                        return true;
                }
            }
        }
        public class ItemDumper : QueueOp
        {
            public InventoryWork InventorySearch;
            public ItemDumper(Program prog, bool active = true) : base(prog, active)
            {
                Name = "DUMP";
                Queue = new RootList<Slot>(RootListType.SLOT_QUEUE_DUMP, prog);

                InventorySearch = new InventoryWork(prog.ROOT, this);
                InventorySearch.Job = new JobMeta(JobType.FIND, WorkType.BROWSE, WorkResult.INVENTORY_FOUND);
            }

            public override bool HasWork()
            {
                return base.HasWork() && Program.Scanner.Complete && !Program.Sorter.HasWork();
            }

            public override void Run()
            {
                base.Run();

                InventorySearch.Job.Requester = Queue[0];
                InventorySearch.SetSearchCount();

                if (!Queue[0].Refresh() || Browse())
                {
                    Queue.Remove(0);
                    InventorySearch.Reset();
                }
            }

            public bool Browse()
            {
                Slot browser = Queue[0];

                switch (InventorySearch.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.COMPLETE:
                        Dlog("Eligable Inventory Found!");
                        Inventory target = InventorySearch.Current();
                        target.Pulled = ForceTransfer(target, InventorySearch.FixedPoint, browser);
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
                Name = "WRITER";
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
        public class PageMaster : Op
        {
            public PageMaster(Program prog, bool active = true) : base(prog, active)
            {
                Name = "READER";
            }

            public override bool HasWork()
            {
                WorkCount = Program.Displays.Count;
                return WorkCount > 0;
            }

            public override void Run()
            {
                base.Run();

                Page working = Program.Displays[WorkIndex].MyPage;
                if (working != null)// &&
                    //(working.ReBuild || !Complete))
                {
                    working.BuildBody();
                    working.ReBuild = false;
                }
                Next();
            }
        }
        public class TallyScanner : Op
        {
            public ContainerWork ContainerScope;
            public SlotWork TallyScope;
            public TallyScanner(Program prog, bool active = true) : base(prog, active)
            {
                Name = "SCAN";

                ContainerScope = new ContainerWork(prog.ROOT, this);
                ContainerScope.Job = new JobMeta(JobType.WORK, WorkType.SCAN);

                TallyScope = new SlotWork(prog.ROOT, this);
                TallyScope.Job = new JobMeta(JobType.WORK, WorkType.SCAN);
                ContainerScope.Chain = TallyScope;
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
                ContainerScope.Job.Requester = working;
                ContainerScope.SetSearchCount();

                if (Scan())
                {
                    working.Pulled = false;
                    ContainerScope.Reset();
                    Next();
                }
            }

            bool Scan()
            {
                try { Dlog($"Scanning Inventory: {Program.Inventories[WorkIndex].MyName()}"); }
                catch { Dlog("Bad Inventory! Skipping..."); return true; }


                switch (ContainerScope.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        //Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.COMPLETE:
                        //Dlog("Scan Complete!");
                        return true;

                    default:
                        return true; // Clear bad scan
                }
            }
        }
        public class TallyMatcher : Op
        {
            public SlotWork SlotLink;

            public TallyMatcher(Program prog, bool active = true) : base(prog, active)
            {
                Name = "MATCH";

                SlotLink = new SlotWork(prog.ROOT, this);
                SlotLink.Job = new JobMeta(JobType.WORK, WorkType.MATCH, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE);
            }
            public override bool HasWork()
            {
                WorkCount = Program.MoveRequests.Count;
                return WorkCount > 0;
            }

            public override void Run()
            {
                base.Run();

                SlotLink.Job.Requester = Program.MoveRequests[WorkIndex];
                SlotLink.SetSearchCount();

                if (Match())
                {
                    SlotLink.Reset();
                    Next();
                }
            }
            bool Match()
            {
                Slot moving = Program.MoveRequests[WorkIndex];


                switch (SlotLink.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE: // Link(s) missing, or slots need fulfilling

                        if (moving.Flow.OUT == 1 && !moving.IsBroken())
                        {
                            Program.Dumper.Queue.Append(moving);
                        }

                        return true;

                    case WorkResult.COMPLETE:
                        Dlog("Moving Complete!");
                        return true;

                    default: // Clear bad event
                        return true;
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
                SlotPump.Job = new JobMeta(JobType.WORK, WorkType.PUMP);
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

                if (Pump())
                {
                    SlotPump.Reset();
                }
            }
            bool Pump()
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
            public SlotWork SlotBrowse;

            public ItemBrowser(Program prog, bool active = true) : base(prog, active)
            {
                TypeFind = new TypeWork(prog.ROOT, this);
                TypeFind.Job = new JobMeta(JobType.FIND, WorkType.BROWSE, WorkResult.TYPE_FOUND);

                SubFind = new SubWork(prog.ROOT, this);
                SubFind.Job = new JobMeta(JobType.FIND, WorkType.BROWSE, WorkResult.SUB_FOUND);
                TypeFind.Chain = SubFind;

                SlotMatch = new SlotWork(prog.ROOT, this);
                SlotMatch.Job = new JobMeta(JobType.EXISTS, WorkType.EXISTS, WorkResult.EXISTS);
                SubFind.Chain = SlotMatch;

                SlotBrowse = new SlotWork(prog.ROOT, this);
                SlotBrowse.Job = new JobMeta(JobType.WORK, WorkType.BROWSE, WorkResult.NONE_CONTINUE, WorkResult.NONE_CONTINUE); // Keep going
                SlotMatch.Chain = SlotBrowse;

                Name = "BROWSE";
            }

            public override bool HasWork()
            {
                WorkCount = Program.PullRequests.Count;
                return WorkCount > 0;
            }

            public override void Run()
            {
                base.Run();

                TypeFind.Job.Requester = Program.PullRequests[WorkIndex];
                TypeFind.SetSearchCount();

                if (Browse())
                {
                    TypeFind.Reset();
                    Next();
                }
            }

            bool Browse()
            {
                Inventory browser = Program.PullRequests[WorkIndex];
                if (browser.Pulled)
                    return true;
                

                switch (TypeFind.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.NONE_CONTINUE:
                        Dlog("Nothing Moved!");
                        return true;

                    case WorkResult.SLOT_FOUND:
                        Dlog("Slot Move Attempted!");
                        return true;

                    default:
                        return true;
                }
            }
        }

        /*public class AssemblyCleaner : Op
        {
            public AssemblyCleaner(Program prog, bool active) : base(prog, active)
            {
                Name = "CLEAN";
            }
        }*/
        public class QuotaAssigner : Op
        {
            public StringWork QuotaAssign;
            public AssemblerWork ProducerAssign;

            public QuotaAssigner(Program prog, bool active = true) : base(prog, active)
            {
                QuotaAssign = new StringWork(prog.ROOT, this);
                QuotaAssign.Job = new JobMeta(JobType.FIND, WorkType.QUOTE, WorkResult.SUB_FOUND);

                ProducerAssign = new AssemblerWork(prog.ROOT, this);
                ProducerAssign.Job = new JobMeta(JobType.WORK, WorkType.QUOTE);
                QuotaAssign.Chain = ProducerAssign;

                Name = "QUOTAS";
            }

            public override bool HasWork()
            {
                WorkCount = Program.AllItemSubs.Count;
                return WorkCount > 0 && !Complete && Program.Scanner.Complete && !Program.Sorter.HasWork();
            }

            public override void Run()
            {
                base.Run();

                TallyItemSub sub = Program.AllItemSubs[WorkIndex];
                QuotaAssign.Job.Requester = sub;
                sub.Assemblers.Clear();
                //Remove(Program.Productions, sub);
                QuotaAssign.Job.RawString = Program.Me.CustomData; // for now
                QuotaAssign.SetSearchCount();

                if (Assign())
                {
                    QuotaAssign.Reset();
                    bool oldComplete = Complete;
                    Next();
                    if (oldComplete != Complete) // Edge case where Quotas affect what shows up in pager (Fill bars!)
                    {
                        Program.Pager.Complete = false;
                        Program.Pager.WorkIndex = 0;
                    }

                }
            }

            bool Assign()
            {
                TallyItemSub item = Program.AllItemSubs[WorkIndex];

                switch (QuotaAssign.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    case WorkResult.SUB_FOUND:
                        Dlog("Quote found!");
                        item.SetQuota(QuotaAssign.FixedPoint);
                        return true;

                    case WorkResult.NONE_CONTINUE:
                        Dlog("Clear Existing Quote!");
                        item.SetQuota();
                        return true;

                    default:
                        return true;
                }
            }
        }
        public class ProductionManager : Op
        {
            public AssemblerWork ProdUpdate;
            public ProductionWork ProdCheck;

            public ProductionManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PRODUCE";

                ProdUpdate = new AssemblerWork(prog.ROOT, this);
                ProdUpdate.Job = new JobMeta(JobType.WORK, WorkType.PROD);

                ProdCheck = new ProductionWork(prog.ROOT, this);
                ProdCheck.Job = new JobMeta(JobType.WORK, WorkType.PROD, WorkResult.COMPLETE, WorkResult.NONE_CONTINUE, false);
                ProdUpdate.Chain = ProdCheck;
            }

            public override bool HasWork()
            {
                WorkCount = Program.Productions.Count;
                return WorkCount > 0 && !Program.Quoting.HasWork();
            }

            public override void Run()
            {
                base.Run();

                TallyItemSub prod = Program.Productions[WorkIndex];
                ProdUpdate.Job.Requester = prod;
                ProdUpdate.SetSearchCount();

                if (Produce())
                {
                    ProdUpdate.Reset();
                    Next();
                }
            }

            bool Produce()
            {
                TallyItemSub prod = Program.Productions[WorkIndex];

                switch (ProdUpdate.WorkLoad())
                {
                    case WorkResult.OVERHEAT:
                        Dlog("OVER-HEAT!");
                        return false;

                    //case WorkResult.COMPLETE:
                        //Dlog("Queue Updated!");
                        //prod.Assemblers[ProdUpdate.SearchIndex].AssemblerBlock.AddQueueItem(prod.Type, ProdCheck.FixedPoint.Value);
                        //return true;

                    //case WorkResult.COMPLETE:
                    case WorkResult.NONE_CONTINUE:
                        Dlog("Assemblers Updated!");
                        return true;

                    default:
                        return true;
                }
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
            //public string CustomName;

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
                //CustomName = TermBlock.CustomName.Replace(bMeta.Root.Signature, "");
                BlockID = TermBlock.EntityId;
                Priority = -1;
                Profile = new FilterProfile(bMeta.Root);
            }

            public override string MyName()
            {
                return TermBlock == null ? "nullBlock" : TermBlock.CustomName.Replace(Signature, string.Empty);
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
                base.Setup();
                Dlog("Block Setup");
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
            public Page MyPage;
            public StringBuilder PageBuilder = new StringBuilder();
            public StringBuilder FooterBuffer = new StringBuilder();

            float FontSizeCache;

            int CharCount;
            int LineCount;
            int HeaderCount;
            int BodyCount;
            int FooterCount;

            int BodySize;
            int ScrollIndex;
            int ScrollDirection;

            int Delay;
            int Timer;

            public bool AutoScroll;
            public bool QUOTE;
            public DisplayMeta Meta;

            public Display(BlockMeta bMeta) : base(bMeta)
            {
                Panel = (IMyTextPanel)bMeta.Block;
                FontSizeCache = Panel.FontSize;
                RebootScreen();

                ScrollIndex = 0;
                ScrollDirection = 1;
                Delay = DefScrollDelay;
                Timer = 0;
                AutoScroll = true;
                QUOTE = false;

                Meta = new DisplayMeta(DefSigCount);
            }

            public void MakePage(string nextline)
            {
                ClearPage();

                //if (Contains(nextline, "stat"))
                //    MakePage(ScreenMode.STATUS);
                //if (Contains(nextline, "inv"))
                //    MakePage(ScreenMode.INVENTORY);
                //if (Contains(nextline, "prod"))
                //    MakePage(ScreenMode.PRODUCTION);
                //if (Contains(nextline, "res"))
                //    MakePage(ScreenMode.RESOURCE);
                if (Contains(nextline, "tally"))
                    MyPage = new TallyPage(Program.ROOT, this);
                //if (Contains(nextline, "slot"))
                //    MakePage(ScreenMode.SLOTS);
                //if (Contains(nextline, "sort"))
                //    MakePage(ScreenMode.SORTING);
                //if (Contains(nextline, "push"))
                //    MakePage(ScreenMode.PUSHING);

                //if (MyPage != null)
                //    Program.Pages.Add(MyPage);
            }

            public void DefaultPage()
            {
                ClearPage();
                MyPage = new DefaultPage(Program.ROOT, this);
            }

            public void ClearPage()
            {
                if (MyPage == null)
                    return;

                MyPage.Dead = true;
                MyPage = null;
            }

            public override void Setup()
            {
                DefaultData(DisplayDefault);
                base.Setup();

                ClearPage();
                Dlog($"Panel:{Panel != null}");

                string[] LineBuffer = Panel.CustomData.Split('\n');

                for (int i = 0; i < LineBuffer.Length; i++)
                {
                    string nextline = LineBuffer[i];

                    char opCode = (nextline.Length > 0) ? nextline[0] : '/';

                    string[] BlockBuffer = nextline.Split(' ');

                    try
                    {
                        switch (opCode)
                        {
                            case '/': // Comment Section (ignored)
                                break;

                            case '*': // Mode
                                MakePage(nextline);
                                break;

                            case '@': // Target
                                if (Contains(nextline, "block"))
                                {
                                    //Operation
                                    block block = Program.Blocks.Find(x => x.TermBlock.CustomName.Contains(BlockBuffer[1]));

                                    if (block != null)
                                    {

                                        Meta.TargetType = TargetType.BLOCK;
                                        Meta.TargetBlock = block;
                                        Meta.TargetName = block.MyName();
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
                                    IMyBlockGroup targetGroup = Program.DetectedGroups.Find(x => x.Name.Contains(BlockBuffer[1]));
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
                                    int newDelay = Convert.ToInt32(BlockBuffer[1]);
                                    Delay = newDelay > 0 ? newDelay : DefScrollDelay;
                                }
                                if (Contains(nextline, "auto"))
                                {
                                    AutoScroll = !nextline.Contains("-");
                                }
                                if (Contains(nextline, "quot"))
                                {
                                    QUOTE = !nextline.Contains("-");
                                }
                                if (Contains(nextline, "sig"))
                                {
                                    Meta.SigCount = Convert.ToInt32(BlockBuffer[1]);
                                }
                                if (Contains(nextline, "f_size"))
                                {
                                    Panel.FontSize = Convert.ToInt32(BlockBuffer[1]);
                                }
                                if (Contains(nextline, "f_col"))
                                {
                                    Color newColor = new Color(
                                        Convert.ToInt32(BlockBuffer[1]),
                                        Convert.ToInt32(BlockBuffer[2]),
                                        Convert.ToInt32(BlockBuffer[3]));

                                    Panel.FontColor = newColor;
                                }
                                if (Contains(nextline, "b_col"))
                                {
                                    Color newColor = new Color(
                                        Convert.ToInt32(BlockBuffer[1]),
                                        Convert.ToInt32(BlockBuffer[2]),
                                        Convert.ToInt32(BlockBuffer[3]));

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

                if (MyPage == null)
                    DefaultPage();

                ResizeScreen();
            }

            public void RebootScreen()
            {
                Panel.Enabled = true;
                Panel.ContentType = ContentType.TEXT_AND_IMAGE;
                Panel.Font = "Monospace";
            }
            void ResizeScreen()
            {
                CharCount = MonoSpaceChars(Panel);
                LineCount = MonoSpaceLines(Panel);
                FontSizeCache = Panel.FontSize;
            }
            public bool Update()
            {
                Dlog($"Updating: {MyName()}");
                if (!CheckBlockExists())
                    return false;

                if (Panel.FontSize != FontSizeCache)
                {
                    ResizeScreen();
                    //Refresh = true;
                }

                Dlog("Screen exists!");

                WritePageToPanel();

                if (AutoScroll)
                {
                    if (Program.DISPLAY_SCROLL_TIMER >= Timer)
                    {
                        Timer += Delay;
                        ScrollDirection = ScrollIndex >= MyPage.Body.Count - BodySize ? -1 : ScrollIndex <= 0 ? 1 : ScrollDirection;
                        Scroll(ScrollDirection);
                    }
                }

                return true;
            }
            public void Scroll(int dir, int amount = 1)
            {
                int oldIndex = ScrollIndex;


                ScrollIndex += dir * amount;
                ScrollIndex =
                    ScrollIndex < 0 ? 0 :
                    //BodySize <= BodyCountCache ? 0 : 
                    MyPage.Body.Count <= BodySize ? 0 :
                    ScrollIndex > (MyPage.Body.Count - BodySize) ? (MyPage.Body.Count - BodySize) : ScrollIndex;

            }

            void WritePageToPanel()
            {

                HeaderCount = 0;
                BodyCount = 0;
                FooterCount = 0;

                PageBuilder.Clear();
                FooterBuffer.Clear();

                try
                {

                    for (int h = 0; h < MyPage.Header.Count; h++)
                        AppendFormattedString(MyPage.Header[h]);


                    for (int f = 0; f < MyPage.Footer.Count; f++)
                        AppendFormattedString(MyPage.Footer[f]);


                    BodySize = LineCount - (HeaderCount + FooterCount);



                    for (int b = 0; b < BodySize && BodyCount < BodySize; b++)
                    {

                        if (b < MyPage.Body.Count)
                        {
                            //Dlog("Bodyline...");
                            AppendFormattedString(MyPage.Body[b + ScrollIndex]);
                        }

                        else
                        {
                            Dlog("Emptyline...");
                            LINE();
                            BodyCount++;
                        }

                    }


                    PageBuilder.Append(FooterBuffer);
                    Panel.WriteText(PageBuilder);

                }
                catch
                {
                    Fail();
                }

            }
            void AppendFormattedString(StringMeta meta)
            {
                int linecount = 1;
                string[] data = meta.RawData;
                string name = null;
                string amount = null;
                int remains = 0;

                Dlog($"{meta.Format}-Line...");

                try
                {
                    switch (meta.Format)
                    {
                        case StringFormat.BODY:
                            Dlog("Adding paragraph...");
                            remains = CharCount;
                            //linecount = 1;
                            for (int i = 0; i < data.Length; i++)
                            {
                                if (data[i] == null)
                                    continue;

                                if (remains != CharCount && data[i].Length >= remains) // needs a new line
                                {
                                    LINE();
                                    linecount++;
                                    remains = CharCount;
                                    Dlog($"NewLine added for data {i}. LineCount: {linecount}");
                                }

                                if (!data[i].Contains('\n') && data[i].Length >= CharCount) // will only fit on this line by itself (hyphenate later)
                                {
                                    LINE($"{data[i]}");
                                    linecount++;
                                    Dlog($"data {i} will only fit by itself. LineCount: {linecount}");
                                    continue;
                                }

                                if (data[i].Contains('\n')) // Get size of last block
                                {
                                    string[] newLines = data[i].Split('\n');
                                    string lastLine = newLines[newLines.Length - 1];
                                    linecount += newLines.Length - 1; // Add number of 'splits'
                                    remains = CharCount - (lastLine.Length + 1); // New lineCount minus last word and one space
                                    Dlog($"data {i} had {newLines.Length} number of separate words. LineCount: {linecount}");
                                }
                                else
                                    remains -= (data[i].Length + 1); // Remainder of lineCount minus word and one space


                                FAP($"{data[i]}"); // Append the next block
                                if (i < data.Length - 1)
                                    FAP($" ");
                            }
                            LINE(); // End of paragraph
                            Dlog($"Final line count for paragraph: {linecount}");
                            //LINE();
                            //LINE(data[0]);
                            break;

                        case StringFormat.WARNING:
                            LINE(data[0]);
                            break;

                        case StringFormat.BAR:
                            for (int i = 0; i < CharCount; i++)
                                FAP((meta.Char == (char)0x0000 ? Bar : meta.Char));
                            LINE();
                            break;

                        case StringFormat.TABLE:
                            if (meta.RawData.Length != 2)
                            {
                                LINE(data[0]);
                                break;
                            }
                            remains = CharCount - (data[0].Length + data[1].Length);
                            if (remains > 0)
                            {
                                FAP(data[0]);

                                for (int i = 0; i < remains; i++)
                                    FAP(Bar);

                                LINE(data[1]);
                            }
                            else
                            {
                                LINE(data[0]);
                                LINE(data[1]);
                            }
                            break;

                        case StringFormat.HEADER:
                        case StringFormat.SUB_HEADER:
                        case StringFormat.FOOTER:

                            bool foot = meta.Format == StringFormat.FOOTER;

                            if (meta.Source != null)
                            {
                                name = $" {meta.Source.MyName()} ";
                            }
                            else if (meta.RawData != null && meta.RawData.Length > 0)
                            {
                                name = meta.RawData[0];
                            }
                            else
                            {
                                LINE(null, foot);
                                break;
                            }

                            //name = $" {name} ";

                            if (CharCount <= name.Length) // Can header fit side dressings?
                            {
                                LINE(name, foot);
                            }
                            else // Apply Header Dressings
                            {
                                remains = CharCount - name.Length;
                                bool myFlimFlam = Program.FLIM_FLAM;

                                //char unique = meta.Source == null ? Bar : meta.Source.MyChar();
                                //unique = unique == (char)0x0000 ? Bar : unique;


                                if (remains % 2 == 1)
                                {
                                    name += myFlimFlam ? B : C;
                                    remains -= 1;
                                }

                                for (int i = 0; i < remains / 2; i++)
                                {
                                    FAP(myFlimFlam ? A : B, foot);
                                    myFlimFlam = !myFlimFlam;
                                }

                                myFlimFlam = Program.FLIM_FLAM;
                                FAP(name, foot);

                                for (int i = 0; i < remains / 2; i++)
                                {
                                    FAP(myFlimFlam ? C : B, foot);
                                    myFlimFlam = !myFlimFlam;
                                }


                                LINE(null, foot);
                            }
                            break;

                        case StringFormat.VALUE:
                            if (meta.Source == null)
                            {
                                LINE("Bad Source...");
                                break;
                            }

                            name = meta.Source.MyName();
                            amount = ParseItemTotal(meta.Source.CurrentAmount(), meta.Source.MyQuota(), Meta);

                            if (CharCount < (name.Length + amount.Length)) // Can Listing fit on one line?
                            {
                                LINE(name);
                                LINE(amount);
                                linecount += 1;
                            }
                            else
                            {
                                FAP(name);

                                for (int i = 0; i < (CharCount - (name.Length + amount.Length)); i++)
                                    FAP(" ");

                                LINE(amount);
                            }

                            MyFixedPoint? sourceQuota = meta.Source.MyQuota();
                            if (!sourceQuota.HasValue)
                                break;

                            FILL(meta.Source.CurrentAmount(), sourceQuota.Value);
                            linecount += 1;

                            break;

                        case StringFormat.SLOT:

                            break;

                        /*case StrForm.RESOURCE:
                            if (!blocks[1].Contains("%"))
                                blocks[1] += "|" + blocks[2];
                            if (CharCount < (blocks[0].Length + blocks[1].Length)) // Can Listing fit on one line?
                            {
                                FormattedStringList.Add(blocks[0]);
                                FormattedStringList.Add(blocks[1]);
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
                                    FormattedStringList.Add(blocks[0]);
                                    FormattedStringList.Add(blocks[2]);
                                    FormattedStringList.Add(blocks[3]);
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
                            break;*/

                        case StringFormat.FILTER:
                            if (data.Length != 4)
                            {
                                FAP(data[0]);
                                break;
                            }
                            remains = CharCount - (data[0].Length + data[1].Length + data[2].Length + data[3].Length + 1);
                            if (remains < 0)
                            {
                                LINE("Filter:");
                                for (int i = 0; i < 4 && i < BodySize - (BodyCount + 1); i++)
                                {
                                    LINE(data[i]);
                                    linecount++;
                                }
                            }
                            else
                            {
                                FAP($"{data[0]}:{data[1]}");
                                for (int i = 0; i < remains; i++)
                                    FAP("-");
                                FAP($"{data[2]}:{data[3]}");
                            }
                            break;

                        case StringFormat.LINK:

                            if (data.Length != 3)
                            {
                                FAP(data[0]);
                                break;
                            }
                            remains = CharCount - (data[0].Length + data[1].Length + data[2].Length);
                            if (remains % 2 == 1)
                            {
                                data[1] += " ";
                                remains -= 1;
                            }
                            remains /= 2;

                            FAP(data[0]);
                            for (int i = 0; i < remains; i++)
                                FAP(" ");
                            FAP(data[1]);
                            for (int i = 0; i < remains; i++)
                                FAP(" ");
                            FAP(data[2]);

                            break;
                    }

                    if (meta.Format == StringFormat.FOOTER)
                        FooterCount += linecount;

                    else if (meta.Format == StringFormat.HEADER)
                        HeaderCount += linecount;

                    else
                        BodyCount += linecount;

                    Dlog($"Current Body Count: {BodyCount}");
                }
                catch
                {
                    Dlog("Bad read...");
                }
            }
            void FILL(MyFixedPoint current, MyFixedPoint quota, bool foot = false)
            {
                float value;
                string percent = $" {PercentBundler(current, quota, Meta.SigCount, out value)} ";
                if (CharCount < (percent.Length)) // Can Listing fit on one line?
                {
                    LINE(percent);
                }
                else
                {
                    int remains = CharCount - percent.Length;
                    //bool myFlimFlam = Program.FLIM_FLAM;

                    //char unique = meta.Source == null ? Bar : meta.Source.MyChar();
                    //unique = unique == (char)0x0000 ? Bar : unique;



                    if (remains % 2 == 1)
                    {
                        percent += Empty;
                        remains -= 1;
                    }
                    remains -= 2;
                    FAP('[', foot);

                    for (int i = 0; i < remains / 2; i++)
                    {
                        float filled = (float)i / remains;
                        Dlog($"first half of fill: {filled}");
                        FAP(filled * 100 <= value ? Full : Empty, foot);
                        //myFlimFlam = !myFlimFlam;
                    }

                    //myFlimFlam = Program.FLIM_FLAM;
                    FAP(percent, foot);

                    for (int i = 0; i < remains / 2; i++)
                    {
                        float filled = (i + ((float)remains / 2) + percent.Length) / remains;
                        Dlog($"second half of fill: {filled}");
                        FAP(filled * 100 <= value ? Full : Empty, foot);
                        //myFlimFlam = !myFlimFlam;
                    }

                    LINE("]", foot);
                }
            }
            void FAP(char input, bool footer = false)
            {
                if (!footer)
                    PageBuilder.Append(input);
                else
                    FooterBuffer.Append(input);
            }
            void FAP(string input, bool footer = false)
            {
                if (!footer)
                    PageBuilder.Append(input);
                else
                    FooterBuffer.Append(input);
            }
            void LINE(string input = null, bool footer = false)
            {
                if (!footer)
                    PageBuilder.Append($"{input}\n");
                else
                    FooterBuffer.Append($"{input}\n");
            }
        }
        public class Inventory : block
        {
            public bool Pulled = false;
            public IMyInventoryOwner Owner;
            public RootList<Slot> AllSlots;
            public RootList<Slot>[] ContainedSlots;

            public Inventory(BlockMeta meta) : base(meta)
            {
                Owner = (IMyInventoryOwner)meta.Block;
                AllSlots = new RootList<Slot>(RootListType.SLOT_INV_ALL, meta.Root.Program);
                ContainedSlots = new RootList<Slot>[Owner.InventoryCount];
                for (int i = 0; i < ContainedSlots.Length; i++)
                    ContainedSlots[i] = new RootList<Slot>(RootListType.SLOT_INV_CONTAIN, meta.Root.Program);
            }

            public override void Setup()
            {
                base.Setup();
                if (Profile.FILL) // Only append pullers, let the Pusher handle dumping
                    Program.PullRequests.Append(this);
                else
                    Program.PullRequests.Remove(this);
            }
            public bool CheckFull(IMyInventory target)
            {
                return target != null && ((Profile.TRUE_FULL && (float)target.CurrentVolume == (float)target.MaxVolume) || ((float)target.CurrentVolume / (float)target.MaxVolume > FULL_MIN));
            }
            public bool CheckEmpty(IMyInventory target)
            {
                float filled = (float)target.CurrentVolume / (float)target.MaxVolume;
                return filled < EMPTY_MAX;
            }
            public bool CheckClogged()
            {
                IMyInventory input = PullInventory();
                return (float)input.CurrentVolume / (float)input.MaxVolume > CLEAN_MIN;
            }
            public bool CheckOverride()
            {
                return this is Producer && Profile.FILL;
            }

            public bool EmptyCheck()
            {
                return Profile.CLEAN ? CheckClogged() : true;
            }
            public virtual bool IsManaged()
            {
                return true;
            }

            public IMyInventory PullInventory(bool input = true)
            {
                return PullInventory(input ? 0 : 1);
            }
            public IMyInventory PullInventory(int invIndex)
            {
                return Owner == null ? null : Owner.GetInventory(invIndex);
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

            public override bool IsManaged()
            {
                return !Profile.ACTIVE_CONVEYOR;
            }

            public override void Setup()
            {
                base.Setup();
                ProdBlock.UseConveyorSystem = Profile.ACTIVE_CONVEYOR;
            }
        }
        public class Assembler : Producer
        {
            public IMyAssembler AssemblerBlock;

            public Assembler(BlockMeta meta) : base(meta)
            {
                AssemblerBlock = (IMyAssembler)meta.Block;
                AssemblerBlock.CooperativeMode = false;
            }
            public override void Setup()
            {
                DefaultData(AssemblerDefault);
                base.Setup();
            }
        }
        public class Refinery : Producer
        {
            public IMyRefinery RefineBlock;

            public Refinery(BlockMeta meta) : base(meta)
            {
                RefineBlock = (IMyRefinery)meta.Block;
            }

            /*public override bool FillCheck()
            {
                return !Profile.ACTIVE_CONVEYOR;
            }*/

            public override void Setup()
            {
                DefaultData(RefineryDefault);
                base.Setup();
            }
        }
        public class InvResource : Inventory
        {
            public ResType Type;
            public bool bIsValue;

            public InvResource(BlockMeta meta, ResType type, bool isValue = true) : base(meta)
            {
                Type = type;
                bIsValue = isValue;
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

            return list.FindIndex(x => x.BlockID == block.EntityId) < 0 && (TAKE_OVER || block.CustomName.Contains(Signature));
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

        static bool ForceTransfer(Inventory target, MyFixedPoint? targetAllowed, Slot source)
        {
            MyFixedPoint? allowed = AllowableReturn(targetAllowed, source);
            if (allowed.HasValue && allowed.Value < 1)
                return false;

            return target.PullInventory().TransferItemFrom(source.Container, source.ItemIndex, null, null, allowed);
        }
        static bool TallyTransfer(Slot source, Slot dest)
        {
            try
            {
                source.Dlog($"*Transferring*\n" +
                    $"{source.InventoryName()} >>> {dest.InventoryName()}");

                if (dest.IsFull())
                {
                    dest.Dlog("Destination Too Full!");
                    return false;
                }

                if (source.IsEmpty())
                {
                    source.Dlog("Source Too Empty!");
                    return false;
                }

                MyFixedPoint? allowed = AllowableReturn(dest, source);
                if (allowed.HasValue && allowed.Value < 1)
                    return false;

                return dest.Container.TransferItemFrom(source.Container, source.ItemIndex, dest.ItemIndex, true, allowed);
            }
            catch { return false; }
        }

        static MyDefinitionId? Recipe(MyItemType type)
        {
            MyDefinitionId output;
            if (MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition", type.SubtypeId, out output))
                return output;
            return null;
        }
        static MyFixedPoint? MaximumReturn(MyFixedPoint? IN, MyFixedPoint? OUT)
        {
            return !IN.HasValue ? OUT : !OUT.HasValue ? IN : IN < OUT ? IN : OUT;
        }
        static MyFixedPoint? AllowableReturn(Slot dest, Slot moving)
        {
            return AllowableReturn(dest.Flow.FILL.HasValue ? dest.Flow.FILL - dest.SnapShot.Amount : null/*dest.SnapShot.Amount*/, moving);
        }
        static MyFixedPoint? AllowableReturn(MyFixedPoint? destTarget, Slot moving)
        {

            MyFixedPoint? allow = moving.Flow.KEEP.HasValue ? moving.CurrentAmount() - moving.Flow.KEEP.Value : moving.IsLinkable() ? (MyFixedPoint?)(moving.CurrentAmount() - 1) : null/*moving.SnapShot.Amount*/;

            MyFixedPoint? output = MaximumReturn(destTarget, allow);

            return output.HasValue ? output.Value < 0 ? 0 : output : null;
        }

        static int MonoSpaceChars(IMyTextPanel panel)
        {
            return (int)(panel.SurfaceSize.X / (panel.FontSize * MONO_RATIO[0]));
        }
        static int MonoSpaceLines(IMyTextPanel panel)
        {
            return (int)(panel.SurfaceSize.Y / (panel.FontSize * MONO_RATIO[1]));
        }


        #endregion

        #region Comparisons
        static bool ProfileCompare(Inventory destination, Slot source, out MyFixedPoint? target, bool dirIn = true)
        {
            target = 0;

            if (!destination.PullInventory().IsConnectedTo(source.Container))
            {
                return false;
            }

            return dirIn ? (source.Flow.OUT > -1 || destination.CheckOverride()) && ProfileCompare(destination.Profile, source.SnapShot.Type, out target) :

                           source.Flow.IN > -1 && ProfileCompare(destination.Profile, source.SnapShot.Type, out target, false);
        }
        static bool ProfileCompare(FilterProfile profile, MyItemType type, out MyFixedPoint? target, bool dirIn = true)
        {
            profile.Dlog("Item Compare");
            return ProfileCompare(profile, type.TypeId, type.SubtypeId, out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, string type, out MyFixedPoint? target, bool dirIn = true)
        {
            profile.Dlog("Type Compare");
            return ProfileCompare(profile, type, "any", out target, dirIn);
        }
        static bool ProfileCompare(FilterProfile profile, out MyFixedPoint? target, string sub, bool dirIn = true)
        {
            profile.Dlog("Sub Compare");
            return ProfileCompare(profile, "any", sub, out target, dirIn);
        }

        static bool ProfileCompare(FilterProfile profile, string type, string sub, out MyFixedPoint? target, bool dirIn = true)
        {
            target = null;
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

                if (dirIn)
                {
                    if (filter.Flow.IN == -1)
                        allow = false;
                    else
                        target = filter.Flow.FILL;
                }

                if (!dirIn)
                {
                    if (filter.Flow.OUT == -1)
                        allow = false;
                    else
                        target = filter.Flow.KEEP;
                }
            }

            allow = match ? allow : auto;

            profile.Dlog($"Full Compare Allow: {allow}");

            return allow;
        }
        static bool FilterCompare(Filter A, string typeB, string subB)
        {
            A.Dlog("Filter Compare");
            return FullCompare(
                A.Compare.type, A.Compare.sub,
                typeB, subB, A);
        }
        static bool FullCompare(string A, string a, string b, string B, Root dbug)
        {
            if (A != "any" && b != "any" && !Contains(A, b) && !Contains(b, A))
            {
                dbug.Dlog("Type mis-match!");
                return false;
            }

            if (a != "any" && B != "any" && !Contains(a, B) && !Contains(B, a))
            {
                dbug.Dlog("Sub mis-match!");
                return false;
            }

            dbug.Dlog("Full match!");
            return true;
        }
        static bool Contains(string source, string target)
        {
            if (source == null)
                return false;

            if (target == null)
                return true;

            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) > -1;
        }

        static bool SlotCompare(Slot Req, Slot Can, out MyFixedPoint? allowable)
        {
            allowable = AllowableReturn(Req, Can);

            return SlotCompare(Req, Can);
        }
        static bool SlotCompare(Slot Req, Slot Can)
        {
            Req.Dlog("Slot Compare!");

            return
                Req != Can && Can.Refresh() && /*Req.Refresh() &&*/ // Requester is expected to have refreshed already
                Req.SnapShot.Type == Can.SnapShot.Type &&           // Only type match, handle flow externally
                (Req.Container == Can.Container ||                  // Let them amalgamate???Let them amalgamate???
                 Req.Container.IsConnectedTo(Can.Container));
        }

        static bool MoveCompare(Slot output, Slot input)
        {
            return input.Flow.IN > -1 && output.Flow.OUT > -1;
        }
        static bool TypeCompare(MyItemType? type, Slot slot)
        {
            slot.Dlog("KeenType Compare!");
            return slot != null && type.HasValue && slot.SnapShot.Type == type.Value;
        }
        static bool TypeCompare(Compare compare, Slot slot)
        {
            slot.Dlog("TorqkType Compare!");

            return slot.CheckSlot() && FullCompare(compare.type, compare.sub, slot.SnapShot.Type.TypeId, slot.SnapShot.Type.SubtypeId, slot);
        }
        static bool TypeCompare(Compare compare, MyItemType type, Root dbug)
        {
            dbug.Dlog("Torqk-Keen Compare!");
            return FullCompare(compare.type, compare.sub, type.TypeId, type.SubtypeId, dbug);
        }

        static bool RawCompare(MyItemType type, string raw, out MyFixedPoint? quote, Root dbug)
        {
            dbug.Dlog("Raw Compare!");
            try
            {
                string[] data = raw.Split(' '); // blocks
                quote = (MyFixedPoint)float.Parse(data[1]);
                Compare compare = new Compare(data[0]);
                return TypeCompare(compare, type, dbug);
            }
            catch
            {
                quote = null;
                return false;
            }
        }

        #endregion

        #region String builders
        static string PercentBundler(MyFixedPoint current, MyFixedPoint? target, int sigCount, bool aligned = true)
        {
            float empty;
            return PercentBundler(current, target, sigCount, out empty, aligned);
        }
        static string PercentBundler(MyFixedPoint current, MyFixedPoint? target, int sigCount, out float percent, bool aligned = true)
        {
            if (!target.HasValue)
            {
                percent = 100;
                return "N/A";
            }


            percent = (float)current / (float)target.Value * 100;

            if (aligned)
                return $"%{Amount(percent, sigCount)}";

            return $"%{percent.ToString($"n{sigCount}")}";
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

            decimal decimalValue = (decimal)value;
            decimalValue = decimal.Round(decimalValue, 2);

            string output = Amount(value, sigCount);
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

            string output = Amount(value, sigCount);
            output += simp;
            return output;
        }
        static string ParseItemTotal(MyFixedPoint current, MyFixedPoint? target, DisplayMeta module)
        {
            return ParseItemTotal(current, target, module.Notation, module.SigCount);
        }
        static string ParseItemTotal(MyFixedPoint current, MyFixedPoint? target, Notation notation, int sigCount)
        {
            switch (notation)
            {
                case Notation.PERCENT: // has no use here
                    return PercentBundler(current, target, sigCount);

                case Notation.DEFAULT:
                    return Amount(current, sigCount);    // decimaless def

                case Notation.SCIENTIFIC:
                    return NotationBundler((float)current, sigCount);

                case Notation.SIMPLIFIED:
                    return SimpleBundler((float)current, sigCount);

                default:
                    return "Unknown Notation mode!";
            }
        }
        static string ItemNameCrop(MyItemType type, bool lit)
        {
            string itemName = string.Empty;
            if (lit)
            {
                itemName = $"{type.TypeId}|{type.SubtypeId}";//type.ToString();
            }
            else
            {
                string[] blocks = type.ToString().Split('/');

                if (blocks[0].Contains("Ingot") && type.SubtypeId.Contains("Stone")) // Gravel special case
                    itemName = "Gravel";

                else if (blocks[0].Contains("Ore") || blocks[0].Contains("Ingot"))   // Distinguish between Ore and Ingot Types
                    itemName = blocks[0].Split('_')[1] + ":" + type.SubtypeId;

                else
                    itemName = type.SubtypeId;
            }
            return itemName;
        }
        static string Type(MyItemType type)
        {
            return type.TypeId.Replace(LEGACY_TYPE_PREFIX, "");
        }
        static string Amount(MyFixedPoint amount, int sigCount)
        {
            return Amount((float)amount, sigCount);
        }
        static string Amount(float value, int sigCount)
        {
            //return amount.ToString($"n{sigCount}");
            return value % (int)value > 0 ? value.ToString($"n{sigCount}") : value.ToString($"n{0}").PadRight(value.ToString($"n{0}").Length + sigCount + (sigCount > 0 ? 1 : 0));
        }
        static string RawTallyItem(DisplayMeta mod, TallyItemSub sub, bool lit)
        {
            return $"{ItemNameCrop(sub.Type, lit)}{Split}{ParseItemTotal(sub.CurrentTotal, sub.TargetQuota, mod)}";
        }
        static string RawSlotItem(DisplayMeta mod, Slot slot, bool lit)
        {
            return $"{slot.InventoryName()/*ItemNameCrop(slot.SnapShot.Type, lit)*/}{Split}{ParseItemTotal(slot.SnapShot.Amount, slot.Flow.FILL, mod)}";
        }
        static string RawFilter(Filter filter)
        {
            if (filter == null)
                return "null filter!";

            string output = $"{filter.Compare.Type()}{Split}{filter.Compare.sub}{Split}";
            output += !filter.Flow.FILL.HasValue ? $"No Fill Target{Split}" : $"{filter.Flow.FILL}{Split}";
            output += !filter.Flow.KEEP.HasValue ? $"No Keep Target{Split}" : $"{filter.Flow.KEEP}{Split}";
            output += filter.Flow.IN == 1 ? ":+ IN:" : filter.Flow.IN == 0 ? ":= IN:" : ":- IN:";
            output += filter.Flow.OUT == 1 ? ":+OUT:" : filter.Flow.OUT == 0 ? ":=OUT:" : ":-OUT:";
            return output;
        }
        static string RawPush(Slot link, int length)
        {
            if (link == null)
                return "null slot!";

            return $"{(link.InLink == null ? "No Input" : link.InLink.InventoryName())}{Split}" +
                $"{link.InventoryName()}{Split}" +
                $"{(link.OutLink == null ? "No Output" : link.OutLink.InventoryName())}";
        }

        static char rgb(byte r, byte g, byte b)
        {
            return (char)(0xe100 + (r << 6) + (g << 3) + b);
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
            Debug.Append($"Detection Complete! [{DetectedBlocks.Count}]\n");
        }
        void ClearVirtuals()
        {
            Blocks.Clear();
            Displays.Clear();
            Assemblers.Clear();
            Resources.Clear();
            Inventories.Clear();
            AllItemTypes.Clear();

            Productions.Clear();
            PullRequests.Clear();
            PumpRequests.Clear();
            MoveRequests.Clear();
        }
        void RunSystemBuild()
        {
            if (!DETECTED)
            {
                ClearVirtuals();
                BlockDetection();
            }

            BUILT = false;
            block newBlock = null;

            for (int i = AuxIndex; i < AuxIndex + OP_CAP; i++)
            {
                if (i >= DetectedBlocks.Count)
                {
                    Debug.Append("Build Complete!\n");
                    BUILT = true;
                    break;
                }

                if (!CheckCandidate(DetectedBlocks[i], Blocks))
                {
                    Debug.Append("Skipping...\n");
                    continue;
                }


                Debug.Append($"[{i}]New Candidate...\n");

                if (DetectedBlocks[i] is IMyShipConnector)
                {
                    newBlock = new Connector(new BlockMeta(ROOT, DetectedBlocks[i]));
                    Debug.Append("Connector Built!\n");
                }

                if (DetectedBlocks[i] is IMyCargoContainer)
                {
                    newBlock = new Cargo(new BlockMeta(ROOT, DetectedBlocks[i]));
                    Debug.Append("Cargo Built!\n");
                }

                if (DetectedBlocks[i] is IMyAssembler)
                {
                    newBlock = new Assembler(new BlockMeta(ROOT, DetectedBlocks[i], null, true));
                    Assemblers.Add((Assembler)newBlock);
                    Debug.Append("Assembler Built!\n");
                }

                if (DetectedBlocks[i] is IMyRefinery)
                {
                    newBlock = new Refinery(new BlockMeta(ROOT, DetectedBlocks[i], null, true));
                    Debug.Append("Refinery Built!\n");
                }

                if (DetectedBlocks[i] is IMyReactor)
                {
                    newBlock = new Reactor(new BlockMeta(ROOT, DetectedBlocks[i]));
                    Debug.Append("Reactor Built!\n");
                }

                if (DetectedBlocks[i] is IMyTextPanel)
                {
                    newBlock = new Display(new BlockMeta(ROOT, DetectedBlocks[i]));
                    Displays.Add((Display)newBlock);
                    Debug.Append("TextPanel Built!\n");
                }

                if (DetectedBlocks[i] is IMyBatteryBlock)
                {
                    if (DetectedBlocks[i] is IMyInventoryOwner)
                    {
                        newBlock = new InvResource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.BATTERY);
                        InvResources.Add((InvResource)newBlock);
                    }
                    else
                    {
                        newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.BATTERY);
                        Resources.Add((Resource)newBlock);
                    }
                    Debug.Append("Battery Built!\n");
                }

                if (DetectedBlocks[i] is IMyGasTank)
                {
                    if (DetectedBlocks[i] is IMyInventoryOwner)
                    {
                        newBlock = new InvResource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASTANK);
                        InvResources.Add((InvResource)newBlock);
                    }
                    else
                    {
                        newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASTANK);
                        Resources.Add((Resource)newBlock);
                    }
                    Debug.Append("GasTank Built!\n");
                }

                if (DetectedBlocks[i] is IMyGasGenerator)
                {
                    if (DetectedBlocks[i] is IMyInventoryOwner)
                    {
                        newBlock = new InvResource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASGEN);
                        InvResources.Add((InvResource)newBlock);
                    }
                    else
                    {
                        newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.GASGEN);
                        Resources.Add((Resource)newBlock);
                    }
                    Debug.Append("GasGen Built!\n");
                }

                if (DetectedBlocks[i] is IMyOxygenFarm)
                {
                    if (DetectedBlocks[i] is IMyInventoryOwner)
                    {
                        newBlock = new InvResource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.OXYFARM);
                        InvResources.Add((InvResource)newBlock);
                    }
                    else
                    {
                        newBlock = new Resource(new BlockMeta(ROOT, DetectedBlocks[i]), ResType.OXYFARM);
                        Resources.Add((Resource)newBlock);
                    }
                    Debug.Append("OxyFarm Built!\n");
                }

                if (DetectedBlocks[i] is IMyInventoryOwner
                    && newBlock != null
                    && !(newBlock is InvResource))
                {
                    try
                    {
                        Inventory owner = (Inventory)newBlock;
                        if (owner != null) // HAAAAAAAAAAAATE!
                        {
                            Inventories.Add((Inventory)newBlock);
                            Debug.Append("Inventory Added!\n");
                        }

                    }
                    catch
                    { // Hatred ensues....
                    }
                }


                if (newBlock != null)
                {
                    if (TAKE_OVER)
                        ReTagBlock(DetectedBlocks[i]);


                    newBlock.Setup();
                    Blocks.Add(newBlock);
                    Debug.Append($"[{i}]{newBlock.MyName()} added to blocks!\n");
                }

            }

            SETUP = BUILT;
            AuxIndex = BUILT ? 0 : AuxIndex + OP_CAP;
        }
        #endregion

        #region Run Arguments
        void RunArguments(string argument)
        {
            if (argument == string.Empty ||
                argument == null)
                return;

            switch (argument)
            {
                case "FAST":
                    AdjustSpeed();
                    break;

                case "SLOW":
                    AdjustSpeed(false);
                    break;

                case "BIG":
                    AdjustVolume();
                    break;

                case "SMALL":
                    AdjustVolume(false);
                    break;

                case "BUILD":
                    DETECTED = false;
                    BUILT = false;
                    break;

                case "SETUP":
                    SETUP = false;
                    break;

                case "RESET":
                    BREAK = false;
                    break;

                case "QUOTE":
                    Productions.Clear();
                    Quoting.Complete = false;
                    break;

                case "RETAG":
                    ReTagBlocks();
                    break;

                case "CLONEDATA":
                    CloneBlocksCustomData();
                    break;

                case "CLEARTAGS":
                    ClearAllBlockTags();
                    break;

                case "CLEARDATA":
                    ClearData();
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

                case "LIT":
                    LIT_DEFS = !LIT_DEFS;
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
                        try
                        {
                            ScrollScreen(InputBuffer[2], -1, int.Parse(InputBuffer[1]));
                        }
                        catch
                        {
                            ScrollScreen(InputBuffer[1], -1);
                        }
                        break;

                    case "DOWN":
                        try
                        {
                            ScrollScreen(InputBuffer[2], 1, int.Parse(InputBuffer[1]));
                        }
                        catch
                        {
                            ScrollScreen(InputBuffer[1], 1);
                        }
                        break;
                }
            }
            catch
            {

            }
        }
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
        void AdjustVolume(bool up = true)
        {
            OP_CAP += up ? 1 : -1;
            OP_CAP = OP_CAP < OP_CAP_MIN ? OP_CAP_MIN : OP_CAP > OP_CAP_MAX ? OP_CAP_MAX : OP_CAP;
        }

        void ClearData()
        {
            foreach (block block in Blocks)
            {
                if (block.TermBlock != null)
                    block.TermBlock.CustomData = string.Empty;
            }
        }
        void ClearQue()
        {
            foreach (Assembler producer in Assemblers)
            {
                if (producer.AssemblerBlock != null)
                    producer.AssemblerBlock.ClearQueue();
            }
        }
        void ClearAllBlockTags()
        {
            List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(allBlocks);

            foreach (IMyTerminalBlock block in allBlocks)
                if (block.CustomName.Contains(Signature))
                    block.CustomName = block.CustomName.Replace(Signature, string.Empty);
        }

        void ReTagBlocks()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullTerminalBlocksFromSignedGroup(blocks, DetectedGroups))
                return;

            foreach (IMyTerminalBlock block in blocks)
                ReTagBlock(block);
        }
        void ReTagBlock(IMyTerminalBlock block)
        {
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
        void CloneBlocksCustomData()
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullTerminalBlocksFromSignedGroup(blocks, DetectedGroups))
                return;

            string masterCustomData = string.Empty;

            foreach (IMyTerminalBlock block in blocks)
                if (block.CustomData.Contains(CopySignature))
                {
                    masterCustomData = block.CustomData.Replace(CopySignature, "");
                    break;
                }

            foreach (IMyTerminalBlock block in blocks)
                block.CustomData = masterCustomData;
        }

        void ScrollScreen(string name, int direction, int amount = 1)
        {
            Display target = Displays.Find(x => x.MyName().Contains(name));
            if (target == null)
                return;

            target.Scroll(direction, amount);
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
                    Mover.Toggle();
                    Browser.Toggle();
                    Dumper.Toggle();
                    break;

                case "DISPLAY":
                    Writer.Toggle();
                    break;

                case "PRODUCE":
                    Quoting.Toggle();
                    Producing.Toggle();
                    break;

                //case "CLEAN":
                //Cleaner.Toggle();
                //break;

                case "POWER":
                    Powering.Toggle();
                    break;
            }
        }
        
        

        #endregion

        #region RUNTIME
        void Debugging()
        {
            Debug.Append($"CallCount: {OP_COUNT}");
            mySurface.WriteText(Debug);
            Debug.Clear();
        }
        void Fail()
        {
            Debug.Append("FAIL-POINT!\n");
        }
        
        void SynchronizedUpdates()
        {
            if (Writer.HasWork())
                DISPLAY_SCROLL_TIMER++;

            if (DISPLAY_SCROLL_TIMER >= FLIM_FLAM_UPDATE)
            {
                FLIM_FLAM_UPDATE += FLIM_FLAM_DELAY;
                FLIM_FLAM = !FLIM_FLAM;
            }
        }
        void ProgEcho()
        {
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
                            $"{ActiveOps[i].CurrentWork.Name}: {ActiveOps[i].CurrentWork.SearchIndex}/{ActiveOps[i].CurrentWork.SearchCount}\n");
                }


                EchoBuilder.Append("====================\n");
            }
            catch { EchoBuilder.Append("FAIL - POINT!"); }

            Echo(EchoBuilder.ToString());

            EchoCount++;

            if (EchoCount >= EchoMax)
                EchoCount = 0;

            EchoBuilder.Clear();
        }
        void LoadStorage()
        {
            //Me.CustomData = Storage;
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
            SetupRootLists();
            SetupOps();
            LoadStorage();

            RUN_FREQ = INIT_FREQ;
            Runtime.UpdateFrequency = RUN_FREQ;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            RunArguments(argument);

            if (BREAK)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }

            else if (!DETECTED)
            {
                BlockDetection();
            }

            else if (!BUILT)
            {
                try
                {
                    RunSystemBuild();
                }
                catch { Fail(); }
            }

            else if (!SETUP)
            {
                //try
                //{
                //    Setup() //????
                //}
            }

            else
            {
                try
                {
                    SynchronizedUpdates();
                    RunOperations();
                }
                catch { Fail(); }
            }

            if (!ONE_TIME && Scanner.Complete && !Sorter.HasWork() && !Quoting.HasWork())
            {
                ONE_TIME = true;
                RUN_FREQ = UpdateFrequency.None;
                Runtime.UpdateFrequency = RUN_FREQ;
            }

            ProgEcho();
            Debugging();

            OP_COUNT = 0;
        }
        public void Save()
        {
            //Storage = Me.CustomData;
        }
        #endregion

        #endregion
    }
}

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
        }*/

        #endregion

        #region MOTH-BALLED

        #endregion

        #region MAIN REGION

        /// USER CONSTANTS ///////////////////////
        const string Signature = "[TGM]";
        const string CustomSig = "[CPY]";

        const float CLEAN_MIN = .8f;
        const float FULL_MIN = 0.98f;
        const float EMPTY_MAX = 0.02f;
        
        const int DefSigCount = 2;

        const int InvSearchCap = 10;
        const int TallySearchCap = 5;
        const int BuildCap = 20;

        static readonly int[] DefScreenRatio = { 25, 17 };
        /// WARNING!! DO NOT GO FURTHER USER!! ///

        /// LOGIC

        UpdateFrequency RUN_FREQ = UpdateFrequency.Update1;
        bool LIT_DEFS = false;
        int ROOT_INDEX = 0;
        RootMeta ROOT;
        IMyTextSurface mySurface;

        public readonly char[] EchoLoop = new char[]
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

        bool FAIL = false;
        bool DETECTED = false;
        bool BUILT = false;
        bool SETUP = false;

        bool bShowProdBuilding = false;

        List<IMyTerminalBlock> DetectedBlocks = new List<IMyTerminalBlock>();
        List<IMyBlockGroup> DetectedGroups = new List<IMyBlockGroup>();

        // Engine
        List<Production> Productions = new List<Production>();
        List<block> Blocks = new List<block>();
        List<Display> Displays = new List<Display>();
        List<Assembler> Assemblers = new List<Assembler>();
        List<Resource> Resources = new List<Resource>();
        List<Inventory> Inventories = new List<Inventory>();
        List<TallyProfile> Profiles = new List<TallyProfile>();

        DisplayManager DisplayMan;
        TallyManager TallyOut;
        TallySorter TallyIn;
        TallyMatcher Matcher;
        TallyUpdater Pumper;
        ProductionManager Producing;

        List<Operation> AllOps;
        List<Operation> InactiveOps = new List<Operation>();
        List<Operation> ActiveOps = new List<Operation>();
        public int CurrentOp = -1;

        void SetupOps()
        {
            DisplayMan = new DisplayManager(this);
            TallyOut = new TallyManager(this);
            TallyIn = new TallySorter(this);
            Matcher = new TallyMatcher(this);
            Pumper = new TallyUpdater(this);
            Producing = new ProductionManager(this);

            AllOps = new List<Operation>()
            {
                DisplayMan,
                TallyOut,
                TallyIn,
                Matcher,
                Pumper,
                Producing,
            };

            InactiveOps.AddRange(AllOps);
        }
        void RunOperations()
        {

            for (int i = InactiveOps.Count - 1; i > -1; i--)
                if (InactiveOps[i].Active)
                {
                    ActiveOps.Add(InactiveOps[i]);
                    InactiveOps.Remove(InactiveOps[i]);
                }

            for (int i = ActiveOps.Count - 1; i > -1; i--)
                if (!ActiveOps[i].Active)
                {
                    InactiveOps.Add(ActiveOps[i]);
                    ActiveOps.Remove(ActiveOps[i]);
                }

            if (!NextOp())
                return;

            ActiveOps[CurrentOp].Run();
        }
        bool NextOp()
        {
            if (ActiveOps == null ||
                ActiveOps.Count < 1)
                return false;
            CurrentOp = CurrentOp + 1 >= ActiveOps.Count ? 0 : CurrentOp + 1;
            return true;
        }

        ///////////////////////
        public enum StrForm
        {
            EMPTY,
            WARNING,
            TABLE,
            HEADER,
            INVENTORY,
            RESOURCE,
            STATUS,
            FILTER,
            PRODUCTION
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
            LOCKED,
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
                Filter = new Filter(def, target);
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

            public DisplayMeta(RootMeta meta)
            {
                SigCount = DefSigCount;
                TargetName = "No Target";

                Mode = ScreenMode.DEFAULT;
                Notation = Notation.DEFAULT;
                TargetType = TargetType.DEFAULT;

                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }

        public class Operation
        {
            public string Name;
            public Program Program;
            public int Priority;
            public bool Active;

            public int[] SB = new int[3];
            public int[] SIx = new int[3];
            public int[] WIx = new int[3];
            public int[] SC = new int[3];
            public int[] WC = new int[3];
            public int[] WT = new int[3];

            public Operation(Program prog, bool active)
            {
                Program = prog;
                Active = active;
            }

            public virtual void Toggle()
            {
                Active = !Active;
            }
            public virtual bool Run() { return true; }
            public virtual void Clear() { }
            public bool Advance(int index)
            {
                SIx[index] += SB[index];
                bool result = SIx[index] >= SC[index];
                SIx[index] = result ? 0 : SIx[index];
                return result;
            }
            public bool Next(int index)
            {
                WIx[index]++;
                WT[index]++;
                bool result = WIx[index] >= WC[index];
                WIx[index] = result ? 0 : WIx[index];
                return result;
            }
            public void Reset(int index)
            {
                SB[index] = 0;
                SIx[index] = 0;
                WIx[index] = 0;
                SC[index] = 0;
                WC[index] = 0;
                WT[index] = 0;
            }
        }
        public class Tally
        {
            public TallyProfile Profile;
            public Filter Filter;
            public Inventory Inventory;
            public List<TallySlot> OccupiedSlots = new List<TallySlot>();

            public MyItemType Type;
            public MyFixedPoint Total;

            public Tally InLink;
            public Tally OutLink;

            public MyFixedPoint InTarget;
            public MyFixedPoint OutTarget;

            public bool ProfileQueued = false;
            public bool MatchQueued = false;

            public Tally(TallySlot first, Filter filter, Inventory inventory)
            {
                Inventory = inventory;
                Filter = filter;
                OccupiedSlots.Add(first);
                Total = first.NEW.Amount;
                Type = first.NEW.Type;
            }
            public void Update(MyFixedPoint change)
            {
                Total += change;

                if (Profile == null)
                {
                    if (!ProfileQueued)
                    {
                        Inventory.Program.TallyIn.TallyQueue.Add(this);
                        ProfileQueued = true;
                    }
                    return;
                }

                Profile.Update(change);
            }
            public void UnOccupy(TallySlot slot)
            {
                if (!OccupiedSlots.Remove(slot))
                    return;

                Update(-slot.NEW.Amount);

                if (OccupiedSlots.Count < 1)
                    Remove();
            }
            public bool CheckUnLinked()
            {
                if (Filter.IN == 1 && InLink == null)
                    return true;

                if (Filter.OUT == 1 && OutLink == null)
                    return true;

                return false;
            }
            public void SyncTallyToInventory(Inventory inventory = null)
            {
                if (Inventory != null)
                    Inventory.Tallies.Remove(this);
                Inventory = inventory;
                if (Inventory != null)
                    Inventory.Tallies.Add(this);
            }
            public void SyncTallyToProfile(TallyProfile profile = null)
            {
                if (Profile != null)
                {
                    Profile.Tallies[(int)TallyGroup.ALL].Remove(this);

                    if (Filter.IN == -1 && Filter.OUT == -1)
                        Profile.Tallies[(int)TallyGroup.LOCKED].Remove(this);
                    if (Filter.IN == 0 && Filter.OUT == 0)
                        Profile.Tallies[(int)TallyGroup.STORAGE].Remove(this);
                    if (Filter.IN == 1 || Filter.OUT == 1)
                        Profile.Tallies[(int)TallyGroup.REQUEST].Remove(this);
                }
                Profile = profile;

                if (Profile != null)
                {
                    Profile.Tallies[(int)TallyGroup.ALL].Add(this);

                    if (Filter.IN == -1 && Filter.OUT == -1)
                        Profile.Tallies[(int)TallyGroup.LOCKED].Add(this);
                    if (Filter.IN == 0 && Filter.OUT == 0)
                        Profile.Tallies[(int)TallyGroup.STORAGE].Add(this);
                    if (Filter.IN == 1 || Filter.OUT == 1)
                        Profile.Tallies[(int)TallyGroup.REQUEST].Add(this);
                }
            }

            public void Remove()
            {
                SyncTallyToProfile();
                SyncTallyToInventory();
            }
            public void Replace(Tally tally)
            {
                foreach (TallySlot slot in tally.OccupiedSlots)
                {
                    slot.Occupant = this;
                    OccupiedSlots.Add(slot);
                }

                Profile.Update(tally.Total);
                tally.Remove();
            }
            public bool Pump()
            {
                if (Inventory == null ||
                    OccupiedSlots.Count < 0)
                    return false;

                bool workComplete = true;

                if (Filter.IN == 1)
                {
                    if (InLink == null)
                    {
                        workComplete = false;
                    }
                    else if (!Transfer(Inventory.Program.Debug, InLink.OccupiedSlots[0], OccupiedSlots[0], InTarget))
                    {
                        workComplete = false;
                    }
                }


                else if (Filter.OUT == 1)
                {
                    if (OutLink == null)
                    {
                        workComplete = false;
                    }

                    else if (!Transfer(Inventory.Program.Debug, OccupiedSlots[0], OutLink.OccupiedSlots[0], OutTarget))
                    {
                        workComplete = false;
                    }
                }

                return workComplete;
            }
        }
        public class TallySlot
        {
            public MyInventoryItem OLD, NEW;
            public Inventory Inventory;
            public Tally Occupant;
            public int Index;

            public TallySlot(MyInventoryItem first, Inventory inventory, int index)
            {
                NEW = first;
                OLD = NEW;
                Inventory = inventory;
                Index = index;
                Occupant = null;
            }
            Tally FreshTally()
            {
                Filter newFilter = TallyFilter(Inventory.Profile, NEW);
                Tally newTally = new Tally(this, newFilter, Inventory);
                Inventory.Tallies.Add(newTally);
                return newTally;
            }
            public void Update(MyInventoryItem newItem)
            {
                OLD = NEW;
                NEW = newItem;

                if (Occupant == null) // Init
                {
                    Occupant = FreshTally();
                }
                else if (!Check(NEW)) // Desync Removal
                {
                    Occupant.UnOccupy(this);
                    Occupant = FreshTally();
                }
                else // Regular Update
                {
                    Occupant.Update(NEW.Amount - OLD.Amount);
                }
            }

            public void Clear()
            {
                if (Occupant != null)
                    Occupant.UnOccupy(this);
            }

            public bool Check(MyInventoryItem sample)
            {
                return OLD.Type == sample.Type;
            }
        }
        public class Filter
        {
            public string[] ItemID = new string[2];
            public MyFixedPoint Target;
            public int IN;
            public int OUT;

            Filter(MyFixedPoint target, int @in, int @out)
            {
                Target = target;
                IN = @in; // Prioritize In
                OUT = @out;
            }
            public Filter(string combo, MyFixedPoint target, int IN = 0, int OUT = 0) : this(target, IN, OUT)
            {
                GenerateFilters(combo, ref ItemID);
            }
            public Filter(MyItemType type, MyFixedPoint target, int IN = 0, int OUT = 0) : this(target, IN, OUT)
            {
                GenerateFilters(type, ref ItemID);
            }
            public Filter(MyDefinitionId id, MyFixedPoint target, int IN = 0, int OUT = 0) : this(target, IN, OUT)
            {
                GenerateFilters(id, ref ItemID);
            }
        }
        public class Root
        {
            public string Signature;
            public int RootID;
            public Program Program;
            public StringBuilder Debug;
            public Root(RootMeta meta)
            {
                Signature = meta.Signature;
                Program = meta.Program;
                Debug = Program.Debug;
                RootID = Program.RequestIndex();
            }
            public virtual void Setup()
            {
                Program.Echo("Root Setup");
            }
            public virtual void RemoveMe()
            {

            }
        }

        public class DisplayManager : Operation
        {
            public DisplayManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "DISPLAY";
            }

            public override bool Run()
            {
                WC[0] = Program.Displays.Count;

                if (WC[0] < 1)
                    return false;

                if (Program.Displays[WIx[0]].Update())
                    Next(0);

                Program.Debugging();

                return true;
            }
        }
        public class TallyManager : Operation
        {
            public TallyManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "TALLY_OUT";
                SB[0] = TallySearchCap;
            }

            public override bool Run()
            {
                WC[0] = Program.Inventories.Count;

                if (WC[0] < 1)
                    return false;

                Inventory working = Program.Inventories[WIx[0]];
                

                if(working.Tally(SIx[0], SB[0]))
                {
                    SIx[0] = 0;
                    Next(0);
                    return true;
                }

                SC[0] = working.Slots.Count > working.Buffer.Count ? working.Slots.Count : working.Buffer.Count;
                Program.Debug.Append($"SC[0]:{SC[0]}\n");

                if (Advance(0))
                    Next(0);

                return true;
            }
        }
        public class TallySorter : Operation
        {
            public List<Tally> TallyQueue = new List<Tally>();
            bool ProfileMatched = false;

            public TallySorter(Program prog, bool active = true) : base(prog, active)
            {
                Name = "TALLY_IN";
                SB[0] = TallySearchCap;
                SB[1] = TallySearchCap;
            }

            public override bool Run()
            {
                WC[0] = TallyQueue.Count;

                if (ProcessInbound())
                {
                    SIx[0] = 0;
                    SIx[1] = 0;
                    TallyQueue[0].ProfileQueued = false;
                    TallyQueue.RemoveAt(0);
                }
                return true;
            }

            bool ProcessInbound()
            {
                if (TallyQueue.Count < 1)
                    return false;

                SC[0] = Program.Profiles.Count;

                if (!ProfileMatched)
                    for (int i = SIx[0]; i < SIx[0] + SB[0]; i++)
                    {
                        if (i >= SC[0])
                        {
                            break;
                        }

                        if (Program.Profiles[i].Type == TallyQueue[0].Type)
                        {
                            SIx[0] = i;
                            SIx[1] = 0;
                            SC[1] = Program.Profiles[i].Tallies[(int)TallyGroup.ALL].Count;
                            ProfileMatched = true;
                            return false;
                        }
                    }

                else
                {
                    TallyProfile tallyProf = Program.Profiles[SIx[0]];
                    List<Tally> profTallyList = tallyProf.Tallies[(int)TallyGroup.ALL];
                    SC[1] = profTallyList.Count;

                    for (int i = SIx[1]; i < SIx[1] + SB[1]; i++)
                    {
                        if (i >= profTallyList.Count)
                        {
                            tallyProf.Append(TallyQueue[0]);
                            ProfileMatched = false;
                            return true;
                        }

                        if (profTallyList[i] == TallyQueue[0])
                        {
                            continue;
                        }

                        if (profTallyList[i].Inventory == TallyQueue[0].Inventory)
                        {
                            profTallyList[i].Replace(TallyQueue[0]);
                            ProfileMatched = false;
                            return true;
                        }
                    }

                    if (Advance(1))
                        ProfileMatched = false;
                }

                if (!Advance(0))
                    return false;

                TallyProfile newProf = new TallyProfile(Program.ROOT, TallyQueue[0]);
                Program.Profiles.Add(newProf);
                return true;
            }
        }
        public class TallyMatcher : Operation
        {
            public List<Tally> Queue = new List<Tally>();

            bool request = true;
            bool storage = true;
            bool inventory = true;

            public TallyMatcher(Program prog, bool active = true) : base(prog, active)
            {
                Name = "MATCH";
                WIx[0] = 0;
                SB[0] = TallySearchCap; // Tally
                SB[1] = InvSearchCap; // Inventory
            }

            public override bool Run()
            {
                WC[0] = Queue.Count;

                if (Queue.Count < 1)
                    return false;

                if (ProcessQueue())
                {
                    request = true;
                    storage = true;
                    inventory = true;
                    Queue[0].MatchQueued = false;
                    Queue.RemoveAt(0);
                }

                return true;
            }

            bool ProcessQueue()
            {
                if (!Queue[0].CheckUnLinked())
                    return true;

                if (request)
                {
                    return TallyLink(TallyGroup.REQUEST, out request);
                }

                if (storage)
                {
                    return TallyLink(TallyGroup.STORAGE, out storage);
                }

                if (inventory)
                {
                    return InventoryLink(false);
                }

                return true;
            }

            bool TallyLink(TallyGroup group, out bool state)
            {
                Tally queued = Queue[0];
                List<Tally> targetList = queued.Profile.Tallies[(int)group];
                SC[0] = targetList.Count;

                for (int i = SIx[0]; i < (SIx[0] + SB[0]); i++)
                {
                    if (i >= targetList.Count)
                    {
                        state = false;
                        SIx[0] = 0;
                        return false;
                    }

                    if (queued.Filter.IN == 1 &&
                        queued.InLink == null &&
                        targetList[i].Filter.OUT > -1 &&
                        (targetList[i].Inventory.Profile.RESIDE ||
                            !targetList[i].Inventory.IsEmpty()))
                    {
                        queued.InTarget = MaximumReturn(targetList[i], queued);
                        queued.InLink = targetList[i];
                    }

                    if (queued.Filter.OUT == 1 &&
                        queued.InLink == null &&
                        targetList[i].Filter.IN > -1 &&
                        (targetList[i].Inventory.Profile.RESIDE ||
                            !targetList[i].Inventory.IsFull()))
                    {
                        queued.OutTarget = MaximumReturn(queued, targetList[i]);
                        queued.OutLink = targetList[i];
                    }

                    if (!queued.CheckUnLinked())
                    {
                        state = true;
                        return true;
                    }
                }

                state = !Advance(0); // Nothing more to look for in state
                return false;
            }

            bool InventoryLink(bool pull = true)
            {
                Tally queued = Queue[0];

                if (queued.OccupiedSlots.Count < 1)
                    return false;

                for (int i = SIx[1]; i < SIx[1] + SB[1]; i++)
                {
                    if (i >= SC[1])
                    {
                        request = true;
                        storage = true;
                        SIx[1] = 0;
                        return true;
                    }

                    if (queued.Inventory == Program.Inventories[i]) // Skip own inventory
                        continue;

                    Inventory inventory = Program.Inventories[i];
                    MyFixedPoint target = 0;

                    if ((pull || queued.Inventory.EmptyCheck()) &&
                        ProfileCompare(inventory.Profile, queued.Type, out target, false) &&
                        ForceTransfer(queued.Inventory, inventory, queued.OccupiedSlots[0].NEW, target))
                    {
                        SIx[1] = 0;
                        return true;
                    }

                    if (!pull && ProfileCompare(inventory.Profile, queued.Type, out target)
                        && ForceTransfer(inventory, queued.Inventory, queued.OccupiedSlots[0].NEW, target))
                    {
                        SIx[1] = 0;
                        return true;
                    }
                }

                if (Advance(1))
                    return true; // found nothing

                return false;
            }

            bool ForceTransfer(Inventory target, Inventory source, MyInventoryItem item, MyFixedPoint amount)
            {
                if (amount != 0)
                    return PullInventory(target).TransferItemFrom(PullInventory(source, false), item, amount);
                return PullInventory(target).TransferItemFrom(PullInventory(source, false), item);
            }
        }
        public class TallyUpdater : Operation
        {
            public TallyUpdater(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PUMP";
                SB[0] = TallySearchCap;
            }

            public override bool Run()
            {
                WC[0] = Program.Profiles.Count;

                if (WC[0] < 1)
                    return false;

                if (Pumping())
                    Next(0);


                return true;
            }

            bool Pumping()
            {
                List<Tally> tallyList = Program.Profiles[WIx[0]].Tallies[(int)TallyGroup.REQUEST];
                SC[0] = tallyList.Count;

                for (int i = SIx[0]; i < SIx[0] + SB[0]; i++)
                {
                    if (i >= SC[0])
                    {
                        return true;
                    }

                    if (tallyList[i].CheckUnLinked())
                    {
                        if (!tallyList[i].MatchQueued)
                        {
                            Program.Matcher.Queue.Add(tallyList[i]);
                            tallyList[i].MatchQueued = true;
                        }
                    }

                    tallyList[i].Pump();
                }
                return Advance(0);
            }
        }
        public class ProductionManager : Operation
        {
            List<Production> Productions;
            List<TallyProfile> Profiles;
            List<Assembler> Assemblers;

            public ProductionManager(Program prog, bool active = true) : base(prog, active)
            {
                Name = "PROD";
                Productions = prog.Productions;
                Profiles = prog.Profiles;
                Assemblers = prog.Assemblers;
            }
        }

        public class FilterProfile
        {
            public List<Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;
            public bool RESIDE;
            public bool CLEAN;

            public FilterProfile(bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false)
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
                    Filters.Add(new Filter(itemID, target, bIn ? FILL ? 1 : 0 : -1, bOut ? EMPTY ? 1 : 0 : -1));
                }

            }
        }
        public class TallyProfile : Root
        {
            public MyFixedPoint CurrentTotal;
            public MyFixedPoint HighestReached;
            public MyItemType Type;

            public List<Tally>[] Tallies = new List<Tally>[Enum.GetNames(typeof(TallyGroup)).Length];

            public TallyProfile(RootMeta meta, Tally first) : base(meta)
            {
                for (int i = 0; i < Tallies.Length; i++)
                    Tallies[i] = new List<Tally>();

                if (first == null)
                    return;

                Type = first.Type;
                Append(first);
            }

            public void Update(MyFixedPoint change)
            {
                CurrentTotal += change;
                CurrentTotal = CurrentTotal > 0 ? CurrentTotal : 0;
                HighestReached = CurrentTotal > HighestReached ? CurrentTotal : HighestReached;
            }

            public void Append(Tally newTally)
            {
                newTally.SyncTallyToProfile(this);
                Update(newTally.Total);
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
                Profile = new FilterProfile();
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
            public StringBuilder Builder;
            public string[][] Buffer = new string[2][];
            public int[] ScreenRatio = new int[2]; // chars, lines
            public int[] ScreenCount = new int[2];

            public int OutputIndex;
            public int OutputCount;
            public int ScrollDirection;
            public int Delay;
            public int Timer;

            public DisplayMeta Meta;

            public Display(BlockMeta bMeta, int[] ratio) : base(bMeta)
            {
                Panel = (IMyTextPanel)bMeta.Block;
                RebootScreen(ratio);

                Builder = new StringBuilder();
                OutputIndex = 0;
                OutputCount = 0;
                ScrollDirection = 1;
                Delay = 1;
                Timer = 0;

                Meta = new DisplayMeta(bMeta.Root);
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
                if (!CheckBlockExists())
                    return false;

                Timer++;
                if (Timer >= Delay)
                {
                    Timer = 0;
                    StringBuilder();
                    Panel.WriteText(Builder);
                    Scroll();
                }

                return true;
            }
            void AppendLine(StrForm format, string rawInput = null)
            {
                Builder.Append(FormatString(format, rawInput));
            }
            void Scroll()
            {
                if (OutputCount < MonoSpaceLines(ScreenRatio[1], Panel)) // Requires Scrolling
                    return;

                if (OutputIndex < 0 || OutputIndex > (OutputCount - ScreenCount[1])) // Index Reset Failsafe
                    OutputIndex = 0;

                if (OutputIndex == OutputCount - ScreenCount[1])
                    ScrollDirection = -1;

                if (OutputIndex == 0)
                    ScrollDirection = 1;

                OutputIndex += ScrollDirection;
            }
            void StringBuilder()
            {
                ScreenCount[0] = MonoSpaceChars(ScreenRatio[0], Panel);
                ScreenCount[1] = MonoSpaceLines(ScreenRatio[1], Panel);

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
                            AppendLine(StrForm.HEADER, "[Tally]\n");
                            TallyBuilder();
                            break;
                    }

                    AppendLine(StrForm.EMPTY);

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
            }
            string FormatString(StrForm format, string rawInput)
            {
                ScreenCount[0] = MonoSpaceChars(ScreenRatio[0], Panel);

                rawInput = rawInput != null ? rawInput : "";
                string[] blocks = rawInput.Split(Split);
                string formattedString = string.Empty;
                int remains = 0;

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
                        if (!Program.bShowProdBuilding)
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

                    case ScreenMode.INVENTORY:
                        InvBuilder(target);
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
                if (inventory.Tallies.Count < 0)
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
            void InvBuilder(block target)
            {
                if (target is Producer)
                {
                    /*
                    0 = InputHeader
                    x = InputCount
                    x+1 = OutputHeader
                     */

                    List<MyInventoryItem> inItems = new List<MyInventoryItem>();
                    List<MyInventoryItem> outItems = new List<MyInventoryItem>();

                    ((Producer)target).ProdBlock.InputInventory.GetItems(inItems);
                    ((Producer)target).ProdBlock.OutputInventory.GetItems(outItems);

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
                AppendLine(StrForm.EMPTY);

                if (Profile.Filters.Count == 0)
                {
                    AppendLine(StrForm.WARNING, "No Filter!");
                    return;
                }

                MyFixedPoint target;
                foreach (TallyProfile candidate in Program.Profiles)
                {
                    if (!ProfileCompare(Profile, candidate.Type, out target))
                        continue;

                    AppendLine(StrForm.INVENTORY, RawListItem(Meta, target, candidate.CurrentTotal, candidate.Type, Program.LIT_DEFS));
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
            public IMyInventoryOwner Owner;
            public List<TallySlot> Slots = new List<TallySlot>();
            public List<MyInventoryItem> Buffer = new List<MyInventoryItem>();
            public List<Tally> Tallies = new List<Tally>();

            public Inventory(BlockMeta meta) : base(meta)
            {
                Owner = (IMyInventoryOwner)meta.Block;
            }

            public override void Setup()
            {
                base.Setup();
            }

            public virtual bool SnapShot()
            {
                PullInventory(this).GetItems(Buffer);
                return Buffer.Count > 0;
            }

            public bool Tally(int seek, int cap)
            {
                if (Buffer.Count < 1)
                    SnapShot();

                Debug.Append($"Tallying: {CustomName}\n" +
                    $"seek:{seek} cap:{cap} buffer:{Buffer.Count}\n");

                for (int i = seek; i < seek + cap; i++)
                {
                    if (i >= Buffer.Count) // List done
                    {
                        for (int j = 0; j < Slots.Count - Buffer.Count; j++) // Trim Excess
                        {
                            Slots[i].Clear();
                            Slots.RemoveAt(i);
                        }

                        Debug.Append($"buffer done...\n");
                        Buffer.Clear();
                        return true;
                    }

                    if (i >= Slots.Count) // New Item
                        Slots.Add(new TallySlot(Buffer[i], this, i));

                    Slots[i].Update(Buffer[i]);
                }

                Debug.Append($"loop done...\n");
                return false;
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
                return Profile.CLEAN ? IsClogged() : false;
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
            public bool AutoRefine;

            public Refinery(BlockMeta meta, bool auto = false) : base(meta)
            {
                RefineBlock = (IMyRefinery)meta.Block;
                AutoRefine = auto;
                Profile.RESIDE = true;
                RefineBlock.UseConveyorSystem = AutoRefine;
            }

            public override void Setup()
            {
                base.Setup();

                string[] data = TermBlock.CustomData.Split('\n');

                foreach (string nextline in data) // Iterate each line
                {
                    if (nextline.Length == 0)// Line must contain information
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
            public ResType Type;
            public bool bIsValue;

            public Resource(BlockMeta meta, ResType type, bool isValue = true) : base(meta)
            {
                Type = type;
                bIsValue = isValue;
            }
        }


        /// Helpers
        static bool Transfer(StringBuilder debug, TallySlot source, TallySlot dest, MyFixedPoint target)
        {
            try
            {
                IMyInventory inBound = PullInventory(dest.Inventory);
                IMyInventory outBound = PullInventory(source.Inventory, false);
                MyFixedPoint amount = target == 0 ? source.NEW.Amount : target < source.NEW.Amount ? target : source.NEW.Amount;
                debug.Append($"target:{target} souceAmount:{source.NEW.Amount} amount:{amount}\n");
                inBound.TransferItemFrom(outBound, source.Index, dest.Index, true, amount);
                return true;
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
        static MyFixedPoint MaximumReturn(Tally inbound, Tally outbound)
        {
            MyFixedPoint IN = inbound.Filter.Target;
            MyFixedPoint OUT = outbound.Filter.Target;

            return IN == 0 ? OUT : OUT == 0 ? IN : IN < OUT ? IN : OUT;
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

        static bool CheckInventoryLink(Inventory outbound, Inventory inbound)
        {
            if (outbound == inbound)
                return false;

            if (!PullInventory(outbound, false).IsConnectedTo(PullInventory(inbound)))
                return false;

            return true;
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
        static string RawListItem(DisplayMeta mod, MyFixedPoint target, MyFixedPoint current, MyItemType type, bool lit)
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
            return $"{itemName}{Split}{ParseItemTotal(current, mod)}";
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

        static int MonoSpaceChars(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
        }
        static int MonoSpaceLines(int ratio, IMyTextPanel panel)
        {
            return (int)(ratio / panel.FontSize);
        }

        /// Comparisons
        static bool ProfileCompare(FilterProfile profile, MyItemType type, out MyFixedPoint target, bool dirIn = true)
        {
            target = 0;
            bool match = false;
            bool allow = false;
            bool auto = false;

            if (dirIn && profile.DEFAULT_IN)
                auto = true;

            if (!dirIn && profile.DEFAULT_OUT)
                auto = true;

            foreach (Filter filter in profile.Filters)
            {
                if (!FilterCompare(filter, type))
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
        static bool FilterCompare(Filter A, MyItemType B)
        {
            return FilterCompare(
                A.ItemID[0], A.ItemID[1],
                B.TypeId.Replace("MyObjectBuilder_", ""),
                B.SubtypeId);
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
            if (a != "any" && !Contains(a, b) && !Contains(b, a))
                return false;

            if (A != "any" && !Contains(A, B) && !Contains(B, A))
                return false;

            return true;
        }
        static bool Contains(string source, string target)
        {
            if (target == null)
                return true;

            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) > -1;
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
            id[1] = def.SubtypeName.ToString().Replace("Component", "");
        }
        static void GenerateFilters(MyItemType type, ref string[] id)
        {
            id[0] = type.TypeId.Replace("MyObjectBuilder_", "");
            id[1] = type.SubtypeId.Replace("Component", "");
        }

        /// Constructors
        static Filter TallyFilter(FilterProfile profile, MyInventoryItem item)
        {
            MyFixedPoint inTarget, outTarget, finalTarget;

            bool inAllowed = ProfileCompare(profile, item.Type, out inTarget);
            bool outAllowed = ProfileCompare(profile, item.Type, out outTarget, false);

            finalTarget = outTarget < inTarget ? outTarget : inTarget;

            return new Filter(item.Type, finalTarget, inAllowed ? (profile.FILL ? 1 : 0) : -1, outAllowed ? (profile.EMPTY ? 1 : 0) : -1);
        }

        /// Initializers
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
            foreach (Operation op in AllOps)
                op.Clear();
        }

        /// Run Arguments
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
        void LoadRecipes()
        {
            Me.CustomData = Storage;
        }
        void ClearQue(int level)
        {
            switch (level)
            {
                case 0:
                    foreach (IMyProductionBlock block in DetectedBlocks)
                        block.ClearQueue();
                    break;

                case 1:
                    foreach (Assembler producer in Assemblers)
                    {
                        Echo("Clearing...");
                        if (producer.AssemblerBlock != null)
                            producer.AssemblerBlock.ClearQueue();
                        Echo("Cleared!");
                    }

                    break;
            }
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
        void ReNameBlocks(string name, bool numbered = false)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (!PullSignedTerminalBlocks(blocks))
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
        void RunSystemBuild()
        {
            if (!DETECTED)
            {
                ClearWraps();
                BlockDetection();
            }

            BUILT = false;
            block newBlock = null;

            for (int i = AuxIndex; i < AuxIndex + BuildCap; i++)
            {
                if (i >= DetectedBlocks.Count)
                {
                    BUILT = true;
                    break;
                }

                if (!CheckCandidate(DetectedBlocks[i]))
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
                    newBlock = new Refinery(new BlockMeta(ROOT, DetectedBlocks[i], true), true);
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
            AuxIndex = BUILT ? 0 : AuxIndex + BuildCap;
        }

        /// Main
        void Debugging()
        {
            mySurface.WriteText(Debug);
            Debug.Clear();

            Debug.Append(
                $"Total Pending Requests: {Matcher.Queue.Count}\n" +
                $"Pump WorkingIndices (IN/OUT): {Pumper.WIx[1]}/{Pumper.WIx[0]}\n" +
                $"======================================\n");
            foreach (TallyProfile profile in Profiles)
            {
                Debug.Append($"{profile.Type} : {profile.CurrentTotal}\n");
                /*foreach (Tally tally in profile.Tallies[(int)TallyGroup.ALL])
                {
                    Debug.Append($"->{tally.Inventory.CustomName}:{tally.Filter.IN}/{tally.Filter.OUT}\n");
                };*/
            }
            Debug.Append($" {Inventories[TallyOut.WIx[0]].CustomName}:\n");
            foreach (TallySlot slot in Inventories[TallyOut.WIx[0]].Slots)
            {
                Debug.Append($"{slot.NEW.Type}:{slot.NEW.Amount}\n");
            }
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
                    ClearQue(1);
                    break;

                case "SAVE":
                    Save();
                    break;

                case "LOAD":
                    LoadRecipes();
                    break;

                case "TALLY":
                    TallyIn.Toggle();
                    TallyOut.Toggle();
                    break;

                case "SORT":
                    Pumper.Toggle();
                    Matcher.Toggle();
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
            //mySurface.WriteText(EchoBuilder);
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
                $"DETECTED: {DETECTED} | BUILT: {BUILT} | SETUP: {SETUP} | FAIL: {FAIL}\n" +
                $"Total Managed Blocks/Inventories: {Blocks.Count}/{Inventories.Count}\n" +
                $"Current >= Name : WorkIx[0]/WorkCount[0]/WorkTotal[0]|SearchIx[0]/SearchCount[0]/WorkTotal[0]\n");

                for (int i = 0; i < AllOps.Count; i++)
                    EchoBuilder.Append($"{(ActiveOps[CurrentOp] == AllOps[i] ? ">>" : "==")}{AllOps[i].Name}:{AllOps[i].WIx[0]}/{AllOps[i].WC[0]}/{AllOps[i].WT[0]}|{AllOps[i].SIx[0]}/{AllOps[i].SC[0]}/{AllOps[i].WT[0]}\n");

                EchoBuilder.Append("====================\n");
            }
            catch { EchoBuilder.Append("FAIL - POINT!"); }
        }
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
        public int RequestIndex()
        {
            ROOT_INDEX++;
            return ROOT_INDEX - 1;
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

        public Program()
        {
            mySurface = Me.GetSurface(0);
            mySurface.ContentType = ContentType.TEXT_AND_IMAGE;
            mySurface.WriteText("", false);

            ROOT = new RootMeta(Signature, this);
            SetupOps();
            LoadRecipes();
            BuildProductions(Me.CustomData);
            BlockDetection();

            Runtime.UpdateFrequency = RUN_FREQ;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            //Debugging();
            RunArguments(argument);
            ProgEcho();
            //mySurface.WriteText(Debug);


            if (FAIL)
                return;

            if (!BUILT)
            {
                RunSystemBuild();
                return;
            }

            try
            {
                RunOperations();
            }
            catch { Debug.Append("FAIL-POINT!"); }

        }

        public void Save()
        {
            Storage = Me.CustomData;
        }
        #endregion
    }
}

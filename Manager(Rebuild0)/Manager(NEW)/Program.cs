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
    #region TEST
    /*public Program()
    {
        IMyTextSurface surface = Me.GetSurface(0);
        surface.ContentType = ContentType.TEXT_AND_IMAGE;
        surface.WriteText("");
        StringBuilder debug = new StringBuilder();

        try
        {
            List<IMyCargoContainer> allCargoes = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType(allCargoes);
            IMyCargoContainer inventoryBlock = allCargoes.Find(x => x.CustomName.Contains("BILL"));
            IMyInventory inventory = inventoryBlock.GetInventory(0);
            debug.Append("block found\n");
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items);
            debug.Append($"{items[0].Type}\n{items[0].Type.TypeId}\n{items[0].Type.SubtypeId}");
        }
        catch
        {
            debug.Append("FAIL!");
        }
        surface.WriteText(debug);
    }

    public void Main(string argument, UpdateType updateSource)
    {

    }*/
    #endregion

    partial class Program : MyGridProgram
    {
        #region MOTHBALL

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
        static string[] ItemTypeFormat(MyInventoryItem targetItem)
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
        static bool FilterCompare(Filter A, Filter B)
        {
            return FilterCompare(A.ItemType, A.ItemSubType, B.ItemType, B.ItemSubType);
        }
        /*void InventoryRotate(_Inventory inventory)
            {
                IMyInventory source = PullInventory(inventory);
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                source.GetItems(items);
                int count = items.Count();
                if (count < ItemSearchCap)
                    return;

                for (int i = 0; i < ItemSearchCap && i < count; i++)
                    source.TransferItemTo(source, 0, count);
            }*/
        /*
            
            public void Clear<T>() where T : Root
            {
                Compare.Match.Type = typeof(T);
                Compare.Ops[0] = CompareType;
                DelegateIterator<T>(Compare);
            }
            public bool AppendBlock(Operation meta)
            {

                //if (!CheckCandidate((meta.Current.Root).Block))
                //    return false;
                //Roots.Add(meta.Current);
                return false;
            }
            public bool Return<T>(BufferOp<T> meta) where T : Root
            {
                if (meta == null)
                    return false;

                //Compare.cType = typeof(T);
                //Compare.Ops[0] = CompareRoot;
                return DelegatePopulator<T>(meta);
            }
            
            public T Return<T>(IMyTerminalBlock block) where T : Root
            {
                if (block == null)
                    return null;

                Compare.Result = null;
                Compare.Match.Term = block;
                Compare.Ops[0] = CompareTerm;
                DelegateIterator<T>(Compare);
                return (T)Compare.Result;
            }
            public T Return<T>(string customName) where T : Root
            {
                if (customName == null)
                    return null;

                Compare.Result = null;
                Compare.Ops[0] = CompareString;
                DelegateIterator<T>(Compare);
                return (T)Compare.Result;
            }

            public Producer Return(string producerType)
            {
                if (producerType == null)
                    return null;

                Compare.Result = null;
                Compare.StringA = producerType;

                DelegateIterator<Producer>(Compare);
                return (Producer)Compare.Result;
            }
        public bool DelegatePopulator<T>(BufferOp<T> buffer) where T : Root
            {
                for (int i = 0; i < Roots.Count; i++)
                {
                    if (Roots[i].GetType() != typeof(T) ||
                        buffer == null)
                        continue;

                    buffer.Current.Root = Roots[i];
                    buffer.MemberIndex[buffer.FuncIndex,0] = i;

                    if (buffer.MATCH &&
                        !buffer.Funcs[buffer.FuncIndex](buffer))
                        continue;

                    buffer.Buffer.Add((T)buffer.Current.Root);

                    if (buffer.BREAK)
                        break;
                }
                return !buffer.TERMINATE;
            }
        /*public class BufferOp<T> : Operation where T : Root
        {
            public List<T> Buffer;

            public BufferOp(Data data) : base(data)
            {
                Buffer = new List<T>();
                Match.Type = typeof(T);
            }
        }*/
         
        

        #endregion

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
        const string CustomSig = "[CPY]";

        const float DefFontSize = .5f;
        const int DefSigCount = 2;
        static readonly int[] DefScreenRatio = { 25, 17 };

        const int MAX_OP_DEPTH = 5;
        const int CLOCK_MAX = 200;

        const float CleanPercent = .8f;
        const float PowerThreshold = .2f;

        /// WARNING!! DO NOT GO FURTHER USER!! ///

        /// LOGIC ////////////////////////
        
        Data DATA;
        IMyTextSurface mySurface;
        StringBuilder Debug = new StringBuilder();

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

        //////////////////////////////////

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
        public enum CompType
        {
            CUSTOM_NAME,
            PROD_BLUE,
            PROD_BUILD,
            TERM_BLOCK,
            ROOT_OBJ,
            FILTER
        }
        public enum ManagerState
        {
            IDLE,
            DETECT,
            SETUP,
            UPDATE,
            TALLY,
            SORT,
            PROD,
            DISPLAY
        }

        class Get
        {
            public virtual void BlockList(Program prog, List<IMyTerminalBlock> buffer)
            {

            }
        }
        class TG<T> : Get where T : class
        {
            public override void BlockList(Program prog, List<IMyTerminalBlock> buffer)
            {
                prog.GridTerminalSystem.GetBlocksOfType<T>(buffer);
            }
        }
        static readonly Get[] Getters =
        {
            new TG<IMyProductionBlock>(),
            new TG<IMyCargoContainer>(),
            new TG<IMyRefinery>(),

            new TG<IMyBatteryBlock>(),
            new TG<IMyPowerProducer>(),
            new TG<IMyGasGenerator>(),
            new TG<IMyOxygenFarm>(),
            new TG<IMyGasTank>()
            };
        public delegate bool RootDel(Operation op);

        public class Operation
        {
            public Data DATA;
            public RootDel[] Funcs = new RootDel[MAX_OP_DEPTH];
            public int[,] MemberIndex = new int[MAX_OP_DEPTH , 2];
            public int FuncIndex;

            public bool TERMINATE;
            public bool RESET;
            public bool BREAK;
            public bool MATCH;
            public CompType Compare;

            public Batch Match;
            public Batch Current;
            public Root Result;

            public List<Root> Collection = new List<Root>();
            public string[] StringsBuffer;

            public Operation(Data data, Batch match) : this(data)
            {
                Match = match;
            }
            public Operation(Data data)
            {
                DATA = data;
            }
            bool Refresh()
            {
                // Debugging
                // Check Remaining Clock Balance
                // Check Operation Terminate
                return !TERMINATE;
            }
            void Init()
            {

            }

            public bool Operate()
            {
                if (DATA == null)
                    return false;

                while (Refresh())
                    Funcs[FuncIndex](this);

                return !TERMINATE;
            }
        }
        public class Data
        {
            public StringBuilder ProductionBuilder = new StringBuilder();
            //public StringBuilder DebugBuilder = new StringBuilder();
            public bool bShowProdBuilding = false;
            public int ProdCharBuffer = 0;

            int EchoCount = 0;
            int ROOT_INDEX = -1;
            int OPERATION_TIME = 0;

            bool FAIL = false;
            bool bPowerSetupComplete = false;
            bool bTallyCycleComplete = false;
            bool bBlocksDetected = false;
            bool bSetupComplete = false;
            bool bUpdated = false;
            bool bPowerCharged = false;

            public RootMeta ROOT;
            public Resource PowerMonitor;
            public Pool Pool;
            public Operation Compare;

            public ManagerState State;
            public Operation[] MAIN_OPS = new Operation[Enum.GetValues(typeof(ManagerState)).Length];

            public List<block> PowerConsumers = new List<block>();
            public List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();
            public List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();

            public Data(RootMeta root)
            {
                ROOT = root;
                Compare = new Operation(this);
                Pool = new Pool(Compare);
            }
            void OpBuilder()
            {
                Operation idle = new Operation(this);
                MAIN_OPS[(int)ManagerState.IDLE] = idle;

                //////////////////////////////////////////

                Operation detect = new Operation(this);
                //detect.Funcs[0] = 

                MAIN_OPS[(int)ManagerState.DETECT] = detect;

                //////////////////////////////////////////
                
                
            }
            string ProgEcho()
            {
                string echoOutput = string.Empty;
                /*int count = -1;
                if (Productions != null &&
                    ProdQueIndex > -1 &&
                    ProdQueIndex < Productions.Count)
                    count = Productions[ProdQueIndex].Tallies.Count;

                echoOutput += $"{EchoLoop[EchoCount]} Torqk's Grid Manager {EchoLoop[EchoCount]}" +
                                "\n====================" +
                                //$"\nTallyCount: {count}"+
                                $"\nClock Indices: {InventoryClock} : {ProdClock} : {DisplayClock}" +
                                $"\nOperation Indices: {InvQueIndex} : {ProdQueIndex} : {DisplayQueIndex}" +
                                $"\nSorting   : {(bSortRunning ? "Online" : "Offline")}" +
                                $"\nTally : {(bTallyRunning ? "Online" : "Offline")}" +
                                $"\nProduction : {(bProdRunning ? "Online" : "Offline")}" +
                                $"\nDisplay      : {(bDisplayRunning ? "Online" : "Offline")}" +
                                "\n====================" +
                                $"\nSearchIndex: {InvSearchIndex}" +
                                $"\nProdSearchIndex: {ProdSearchIndex}";*/

                //echoOutput += (bSetupComplete) ? $"\nInvCount: {Inventories.Count}" : $"\nSetupIndex: {SetupQueIndex}";

                EchoCount++;

                if (EchoCount >= EchoMax)
                    EchoCount = 0;

                return echoOutput;
            }
            public int RequestIndex()
            {
                ROOT_INDEX++;
                return ROOT_INDEX;
            }
            public bool UpdateRoot(Operation op)
            {
                if (op.Current.Root == null)
                    return false;

                op.Current.Root.Update(op);
                return !op.TERMINATE;
            }
            public bool SetupRoot(Operation op)
            {
                return op.Current.Root.Setup(op);

   
                    foreach (Refinery nextRefine in Pool.Roots)
                        RefinerySetup(nextRefine);



                    ProductionSetup();

            }
            void RunClock()
            {
                switch (State)
                {
                    case ManagerState.IDLE:
                        break;

                    case ManagerState.DETECT:
                        break;

                    case ManagerState.SETUP:
                        break;
                }
            }
            public void ClockInitialize()
            {

            }
            public void Main()
            {
                if (FAIL)
                    return;

                if (!bBlocksDetected)
                {
                    bBlocksDetected = BlockDetection();
                    return;
                }
                if (!bSetupComplete)
                {
                    //bSetupComplete;
                    return;
                }
                if (!bUpdated)
                {
                    bUpdated = UpdateSettings();
                    return;
                }

                try
                {
                    RunClock();
                }
                catch
                {
                    //Debug.Append("FAIL-POINT!\n");
                    FAIL = true;
                }

            }
            public bool BlockDetection()
            {
                BlockGroups.Clear();
                ROOT.Program.GridTerminalSystem.GetBlockGroups(BlockGroups);

                for (int i = 0; i < Getters.Length; i++)
                    Getters[i].BlockList(ROOT.Program, Blocks);

                return Blocks.Count > 0;
            }
            /*public bool BlockListSetup()
            {
                for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < Blocks.Count(); i++)
                {
                    if (!Pool.CheckCandidate(Blocks[i]))
                        continue;

                    GenerateBlockWrapper(Pool, Blocks[i]);
                }
                return SetupQueIndex + SetupCap > Blocks.Count();
            }*/
            void ClearMetaObjects()
            {
                ProductionBuilder.Clear();
                Pool.Roots.Clear();
                PowerConsumers.Clear();
            }
            block GenerateBlockWrapper(IMyTerminalBlock block)
            {
                block output = null;
                BlockMeta meta = new BlockMeta(ROOT, block);

                if (block is IMyCargoContainer)
                {
                    output = new Cargo(meta);
                }
                if (block is IMyProductionBlock)
                {
                    output = new Producer(meta);
                    PowerConsumers.Add(output);
                }
                if (block is IMyRefinery)
                {
                    output = new Refinery(meta);
                    PowerConsumers.Add(output);
                }
                if (block is IMyTextPanel)
                {
                    output = new Display(meta, DefFontSize);
                    PowerConsumers.Add(output);
                }

                if (block is IMyBatteryBlock)
                {
                    output = new Resource(meta, _ResType.BATTERY);
                }
                if (block is IMyGasTank)
                {
                    output = new Resource(meta, _ResType.GASTANK);
                }
                if (block is IMyPowerProducer)
                {
                    output = new Resource(meta, _ResType.POWERGEN);
                }
                if (block is IMyGasGenerator)
                {
                    output = new Resource(meta, _ResType.GASGEN);
                    PowerConsumers.Add(output);
                }
                if (block is IMyOxygenFarm)
                {
                    output = new Resource(meta, _ResType.OXYFARM);
                    PowerConsumers.Add(output);
                }

                return output;
            }

            /// Setup
            void PowerSetup()
            {
                //if (PowerSetup(resource))
                //{
                //    bPowerSetupComplete = true;
                //    break;
                //}
                //PowerMonitor = null;
                //if (!bPowerSetupComplete)
                //    PowerPrioritySet(Base.Blocks, 0);
            }
            void RefinerySetup(Refinery refinery)
            {
                string[] data = refinery.Block.CustomData.Split('\n');

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

                //InventorySetup(refinery);
            }
            
            void ProductionSetup()
            {
                Pool.Roots.RemoveAll();

                string[] dataTriples = ProductionBuilder.ToString().Split('\n');

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

                    ProdMeta meta = new ProdMeta(nextId, prodId, target);
                    Production nextProd = new Production(meta, ROOT);
                    Pool.Roots.Add(nextProd);
                }
            }

            /// Inventory
            void Tally(Operation op)
            {
                /*for (int i = ProdSearchIndex; i < (ProdSearchIndex + ProdSearchCap) && i < DATA.Productions.Count(); i++)
                {
                    Productions[i].TallyUpdate(inventory, ref Counter);
                }*/
            }
            void TallyAmount(Operation op)
            {

            }
            void SortUpdate(Inventory inventory)
            {
                if (inventory.rMeta.FilterProfile.EMPTY ||
                    CheckProducerInputClog(inventory)) // Assembler anti-clog
                    InventoryEmpty(inventory);

                if (inventory.rMeta.FilterProfile.FILL)
                    InventoryFill(inventory);
            }
            void InventoryEmpty(Inventory inventory)
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

                foreach (MyInventoryItem nextItem in sourceItems)
                {
                    /*if (Counter.Check(Count.MOVED)) // Total Items moved
                        break;

                    if (Counter.Increment(Count.ITEMS)) // Total Items searched
                        break;*/

                    if (ProfileCompare(inventory.FilterProfile, nextItem, out target, false))
                        continue;

                    //BufferOp<Inventory> buffer = new BufferOp<Inventory>();

                    for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < DATA.Inventories.Count(); i++)
                    {
                        if (Counter.Check(Count.MOVED)) // Total Items moved
                            break;

                        EmptyToCandidate(inventory, Pool.Return<Inventory>(), nextItem);
                    }
                }
            }
            void InventoryFill(Inventory inventory)
            {
                if (inventory is Refinery &&
                    ((Refinery)inventory).AutoRefine)
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
                    !FillFromCandidate(inventory, DATA.Inventories[inventory.CallBackIndex]))
                    inventory.CallBackIndex = -1;

                for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < DATA.Inventories.Count(); i++)
                {
                    if (Counter.Check(Count.MOVED))
                        return;

                    if (FillFromCandidate(inventory, DATA.Inventories[i]))
                        break;
                }
            }


            bool CheckProducerInputClog(Inventory inventory)
            {
                if (!(inventory is Producer))
                    return false;

                Producer producer = (Producer)inventory;

                return (float)producer.ProdBlock.InputInventory.CurrentVolume / (float)producer.ProdBlock.InputInventory.MaxVolume > CleanPercent;
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
            //void EmptyToCandidates()
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
                        Counter.Increment(Count.MOVED);
                    }
                }
                else
                {
                    sourceInventory.TransferItemTo(targetInventory, currentItem);
                    Counter.Increment(Count.MOVED);
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

                    MyFixedPoint value = 0;

                    if (!(source is Refinery) &&                                           // Refineries get override privledges
                        !ProfileCompare(target.rMeta.FilterProfile, nextItem, out value, false))   // Check aloud to leave
                        continue;

                    if (!ProfileCompare(source.rMeta.FilterProfile, nextItem, out value))          // Check if it fits the pull request
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
                            source.CallBackIndex = DATA.Inventories.FindIndex(x => x == target);
                            callback = true;
                            if (Counter.Increment(Count.MOVED))
                                break;
                        }
                    }

                    else
                    {
                        targetInventory.TransferItemTo(sourceInventory, nextItem);
                        source.CallBackIndex = DATA.Inventories.FindIndex(x => x == target);
                        callback = true;
                        if (Counter.Increment(Count.MOVED))
                            break;
                    }
                }

                Counter.Reset(Count.ITEMS);
                return callback;
            }

            /// Power
            void PowerAdjust(bool bAdjustUp = true)
            {
                if (bAdjustUp)
                    foreach (block block in PowerConsumers)
                        ((IMyFunctionalBlock)block.Block).Enabled = true;

                else
                    for (int i = PowerConsumers.Count - 1; i > -1; i--)
                    {
                        if (((IMyFunctionalBlock)PowerConsumers[i].Block).Enabled)
                        {
                            ((IMyFunctionalBlock)PowerConsumers[i].Block).Enabled = false;
                            break;
                        }

                    }
            }
            bool PowerSetup(Resource candidateMonitor)
            {
                foreach (block block in PowerConsumers)
                    block.Priority = -1;

                if (candidateMonitor.Type != _ResType.BATTERY)
                    return false;

                string[] data = candidateMonitor.Block.CustomData.Split('\n');
                int index = 0;

                bool[] checks = new bool[4]; // rough inset
                Operation meta;

                foreach (string nextline in data)
                {

                    if (Contains(nextline, "prod") && !checks[0])
                    {
                        index = PowerPrioritySet((new BufferOp<Producer>()).Buffer, index);
                        checks[0] = true;
                    }

                    if (Contains(nextline, "ref") && !checks[1])
                    {
                        index = PowerPrioritySet((new BufferOp<Refinery>()).Buffer, index);
                        checks[1] = true;
                    }

                    /*if (Contains(nextline, "farm") && !checks[2])
                    {
                        List<_Resource> farms = new List<_Resource>();
                        farms.AddRange(DATA.Resources.FindAll(x => x.Type == _ResType.OXYFARM));
                        index = PowerPrioritySet(farms.Cast<_Block>().ToList(), index);
                        checks[2] = true;
                    }

                    if (Contains(nextline, "gen") && !checks[3])
                    {
                        List<_Resource> gens = new List<_Resource>();
                        gens.AddRange(DATA.Resources.FindAll(x => x.Type == _ResType.GASGEN));
                        index = PowerPrioritySet(gens.Cast<_Block>().ToList(), index);
                        checks[3] = true;
                    }*/
                }

                if (index == 0)
                    return false;

                PowerPrioritySet(PowerConsumers, index);

                PowerConsumers = PowerConsumers.OrderBy(x => x.Priority).ToList(); // << Maybe?

                PowerMonitor = candidateMonitor;

                return true;
            }
            int PowerPrioritySet<T>(List<T> consumers, int start) where T : block
            {
                int index = 0;

                foreach (T block in consumers)
                {
                    if (block.Priority == -1)
                    {
                        block.Priority = index + start;
                        index++;
                    }
                }

                return index + start;
            }
            void PowerUpdate()
            {
                if (PowerMonitor == null)
                    return;

                IMyBatteryBlock battery = (IMyBatteryBlock)PowerMonitor.Block;

                if (!bPowerCharged &&
                    battery.CurrentStoredPower / battery.MaxStoredPower >= (1 - PowerThreshold))
                    PowerAdjust(true);

                if (battery.CurrentStoredPower / battery.MaxStoredPower <= PowerThreshold)
                    PowerAdjust(false);

            }
        }
        public class Pool
        {
            public List<Root> Roots = new List<Root>();
            public Operation Compare;
            public Pool(Operation compare)
            {
                Compare = compare;
            }

            public T ReTurn<T, S>(S source)
            {

            }
            public bool DelegateIterator<T>(Operation meta) where T : Root
            {
                for (int i = 0; i < meta.MemberIndex[meta.FuncIndex,1]; i++)
                {
                    meta.MemberIndex[meta.FuncIndex, 0] = i;
                    SetCurrent(meta);
                    meta.Funcs[meta.FuncIndex](meta);
                    if (meta.BREAK)
                        break;
                }
                return !meta.TERMINATE;
            }
            
        }

        public struct Batch
        {
            public string String;
            public IMyTerminalBlock Term;
            public MyInventoryItem Item;
            public Type Type;
            public Root Root;
            public Filter Filter;
        }
        public struct RootMeta
        {
            public string Signature;
            public int RootID;
            public Data Data;
            public Profile FilterProfile;
            public Program Program;
            public IMyProgrammableBlock Me;

            public RootMeta(string signature, Data dataBase, Profile profile, Program program, IMyProgrammableBlock me, IMyTextSurface surface = null)
            {
                Data = dataBase;
                RootID = -1;
                if (Data != null)
                    RootID = Data.RequestIndex();
                Signature = signature;
                FilterProfile = profile;
                Program = program;
                Me = me;
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

            public FilterMeta(bool In = true, bool Out = true, MyFixedPoint target = new MyFixedPoint())
            {
                IN_BOUND = In;
                OUT_BOUND = Out;
                Target = target;
            }
        }
        public struct ProdMeta
        {
            public MyDefinitionId Def;
            public string ProducerType;
            public int Target;

            public ProdMeta(MyDefinitionId def, string prodIdString, int target = 0)
            {
                Def = def;
                ProducerType = prodIdString;
                Target = target;
            }
        }
        public struct DisplayMeta
        {
            public int SigCount;
            public string TargetName;

            public _ScreenMode Mode;
            public _Notation Notation;
            public _Target TargetType;

            public Profile FilterProfile;
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

                FilterProfile = new Profile(meta, false, false);
                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }

        public class Root
        {
            public RootMeta rMeta;
            public IMyTextSurface DebugSurface;
            public StringBuilder DebugBuilder;
            public virtual bool Setup(Operation op)
            {
                return true;
            }
            public virtual void DataSweep(Operation op)
            {

            }
            public virtual void Update(Operation op)
            {
                DebugBuilder.Clear();
            }
            public Root(RootMeta meta)
            {
                DebugBuilder = new StringBuilder();
                rMeta = meta;
                DebugSurface = rMeta.Me == null ? null : rMeta.Me.GetSurface(0);
            }
        }
        public class Filter
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

            public Filter(FilterMeta meta, string combo)
            {
                Meta = meta;
                GenerateFilters(combo);
            }
            public Filter(FilterMeta meta, MyItemType type)
            {
                Meta = meta;
                GenerateFilters(type);
            }
            public Filter(FilterMeta meta, MyDefinitionId id)
            {
                Meta = meta;
                GenerateFilters(id);
            }
        }
        public class Profile : Root
        {
            public List<Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;

            public Profile(RootMeta meta, bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false) : base(meta)
            {
                Filters = new List<Filter>();
                DEFAULT_OUT = defOut;
                DEFAULT_IN = defIn;
                FILL = defFill;
                EMPTY = defEmpty;
            }
            public override bool Setup(Operation op)
            {
                Filters.Clear();
                DEFAULT_IN = true;
                DEFAULT_OUT = true;

                string[] data = inventory.Block.CustomData.Split('\n');                                     // Break customData into lines

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

                Filters.RemoveAll(x => x.ItemType == "any" && x.ItemSubType == "any");  // Redundant. Refer to inventory default mode

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
                    FilterMeta meta = new FilterMeta(bIn, bOut, target);
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

                FilterMeta filter = new FilterMeta(true, true, meta.Target);
                Filter = new Filter(filter, Def);
                Tallies = new List<Tally>();
            }

            public void TallyUpdate(Inventory sourceInventory)
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
            public override void Update(Operation op)
            {
                if (Current >= Filter.Meta.Target)
                {
                    // Add excess que removal logic here later
                    return;
                }

                List<Producer> candidates = rMeta.Data.Pool.Return<Producer>();
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
            public IMyTerminalBlock Block;

            public block(BlockMeta bMeta) : base(bMeta.Root)
            {
                Block = bMeta.Block;
                CustomName = Block.CustomName.Replace(bMeta.Root.Signature, "");
                BlockID = Block.EntityId;
                Priority = -1;
            }
            public bool CheckBlockExists()
            {
                IMyTerminalBlock maybeMe = rMeta.Program.GridTerminalSystem.GetBlockWithId(Block.EntityId); // BlockID?
                if (maybeMe != null &&                          // Exists?
                    maybeMe.CustomName.Contains(Signature))    // Signature?
                    return true;

                if (!bDetatchable)
                    rMeta.Data.Pool.Roots.Remove(this);

                return false;
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

            public int[] WrapVectors = new int[2]; // chars, lines
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
            public override bool Setup(Operation op)
            {
                dMeta.FilterProfile.Filters.Clear();

                Buffer[0] = Panel.CustomData.Split('\n');

                foreach (string nextline in Buffer[0])
                {
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
                                block block = rMeta.Data.Pool.Return<block>(CustomName.Contains(lineblocks[1]));

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
                                IMyBlockGroup targetGroup = base.rMeta.Data.BlockGroups.Find(x => x.Name.Contains(lineblocks[1]));
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
                                int newDelay = Convert.ToInt32(lineblocks[1]);
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
                            dMeta.FilterProfile.Append(nextline);
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
                WrapVectors = ratio == null ? DefScreenRatio : ratio;
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
                int lineCount = MonoSpaceLines(DefScreenRatio[1], Panel) - Offset;

                bool tick = false;
                Timer++;
                if (Timer >= Delay)
                {
                    Timer = 0;
                    tick = true;
                }

                string[] lines = fOutput.ToString().Split('\n');

                if (lines.Length > MonoSpaceLines(DefScreenRatio[1], Panel)) // Requires Scrolling
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
                            if (!base.rMeta.Data.bShowProdBuilding)
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

                                    for (int i = 0; i < base.rMeta.Data.ProdCharBuffer - blocks[1].Length; i++)
                                        formattedString += " ";
                                    formattedString += " | " + blocks[2];
                                    for (int i = 0; i < (chars - (base.rMeta.Data.ProdCharBuffer + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)); i++)
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
                    block next = base.rMeta.Data.Pool.Return(nextTermBlock);
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
                    InventoryBuilder(((IMyProductionBlock)((Inventory)target).Block).InputInventory);
                    rawOutput.Append("#\n");
                    rawOutput.Append("=" + Seperator + "|Output|\n");
                    InventoryBuilder(((IMyProductionBlock)((Inventory)target).Block).OutputInventory);
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

                foreach (Production prod in base.rMeta.Data.Pool.Roots)
                {
                    rawOutput.Append("\n@" + Seperator);
                    string nextDef = prod.Filter.ItemSubType;
                    rawOutput.Append(nextDef + Seperator);
                    base.rMeta.Data.ProdCharBuffer = (base.rMeta.Data.ProdCharBuffer > nextDef.Length) ? base.rMeta.Data.ProdCharBuffer : nextDef.Length;

                    rawOutput.Append(
                        prod.ProducerType + Seperator +
                        prod.Current + Seperator +
                        prod.Filter.Meta.Target);
                }
            }
            void ItemTallyBuilder()
            {
                rawOutput.Append("#" + Seperator + "\n");

                if (dMeta.FilterProfile.Filters.Count == 0)
                {
                    rawOutput.Append("!" + Seperator + "No Filter!\n");
                    return;
                }

                Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                Inventory targetInventory;

                switch (dMeta.TargetType)
                {
                    case _Target.BLOCK:
                        //targetInventory = rMeta.Data.Pool.ReTurn<Tally,Sandbox>((object x) => x == (Inventory)Block);
                        PullInventory(targetInventory).GetItems(items);
                        ItemListBuilder(itemTotals, items, dMeta.FilterProfile);
                        break;

                    case _Target.GROUP:

                        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                        dMeta.TargetGroup.GetBlocks(blocks);
                        foreach (IMyTerminalBlock block in blocks)
                        {
                            targetInventory = base.rMeta.Data.Pool.Return<Inventory>(block);
                            if (targetInventory == null)
                                continue;
                            items.Clear();
                            PullInventory(targetInventory).GetItems(items);
                            ItemListBuilder(itemTotals, items, dMeta.FilterProfile);
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
                                IMyBatteryBlock batBlock = (IMyBatteryBlock)targetBlock.Block;
                                value = batBlock.CurrentStoredPower + "/" + batBlock.MaxStoredPower;
                                unit = batBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Stored power")).Split(' ')[3];
                                break;

                            case _ResType.POWERGEN:
                                IMyPowerProducer powBlock = (IMyPowerProducer)targetBlock.Block;
                                value = powBlock.CurrentOutput + "/" + powBlock.MaxOutput;
                                unit = powBlock.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Current Output")).Split(' ')[3];
                                break;

                            case _ResType.GASTANK:
                                IMyGasTank gasTank = (IMyGasTank)targetBlock.Block;
                                value = gasTank.DetailedInfo.Split('\n').ToList().Find(x => x.Contains("Filled")).Split(' ')[2];
                                value = value.Substring(1, value.Length - 2);
                                value = value.Replace("L", "");
                                unit = " L ";
                                break;

                            case _ResType.GASGEN:
                                IMyGasGenerator gasGen = (IMyGasGenerator)targetBlock.Block;
                                value = (gasGen.IsWorking) ? "Running" : "NotRunning";
                                unit = "I/O";
                                break;

                            case _ResType.OXYFARM:
                                IMyOxygenFarm oxyFarm = (IMyOxygenFarm)targetBlock.Block;
                                value = (oxyFarm.IsWorking) ? "Running" : "NotRunning";
                                unit = "I/O";
                                break;
                        }
                        break;

                    case _Notation.PERCENT:
                        switch (targetBlock.Type)
                        {
                            case _ResType.BATTERY:
                                IMyBatteryBlock batBlock = (IMyBatteryBlock)targetBlock.Block;
                                percent = Convert.ToInt32((batBlock.CurrentStoredPower / batBlock.MaxStoredPower) * 100f);
                                break;

                            case _ResType.POWERGEN:
                                IMyPowerProducer powBlock = (IMyPowerProducer)targetBlock.Block;
                                percent = (int)((powBlock.CurrentOutput / powBlock.MaxOutput) * 100);
                                break;

                            case _ResType.GASTANK:
                                IMyGasTank gasTank = (IMyGasTank)targetBlock.Block;
                                percent = (int)((gasTank.FilledRatio) * 100);
                                break;

                            case _ResType.GASGEN:
                                IMyGasGenerator gasGen = (IMyGasGenerator)targetBlock.Block;
                                if (gasGen.IsWorking)
                                    percent = 100;
                                break;

                            case _ResType.OXYFARM:
                                IMyOxygenFarm oxyFarm = (IMyOxygenFarm)targetBlock.Block;
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
                return ((IMyCargoContainer)inventory.Block).GetInventory();
            }
            else
            {
                return (input) ? ((IMyProductionBlock)inventory.Block).InputInventory : ((IMyProductionBlock)inventory.Block).OutputInventory;
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
        static string ParseItemTotal(KeyValuePair<string, MyFixedPoint> item, DisplayMeta meta)
        {
            string nextOut = "$" + Seperator + item.Key + Seperator;

            switch (meta.Notation)
            {
                case _Notation.PERCENT: // has no use here
                case _Notation.DEFAULT:
                    //nextOut += item.Value.ToString();
                    nextOut += ((int)item.Value).ToString();    // decimaless def
                    break;

                case _Notation.SCIENTIFIC:
                    nextOut += NotationBundler((float)item.Value, meta.SigCount);
                    break;

                case _Notation.SIMPLIFIED:
                    nextOut += SimpleBundler((float)item.Value, meta.SigCount);
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
                A.ItemType, A.ItemSubType,
                B.Type.TypeId, B.Type.SubtypeId);
        }
        static bool FilterCompare(Filter A, MyProductionItem B)
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
        static bool SetCurrent(Operation meta)
        {
            try
            {
                meta.Current.Root = meta.DATA.Pool.Roots[meta.MemberIndex[meta.FuncIndex,0]];
                meta.Current.Term = ((block)meta.Current.Root).Block;
                switch(meta.Compare)
                {
                    case CompType.CUSTOM_NAME:
                        //meta.Current.String = 
                        break;

                    case CompType.PROD_BLUE:
                        break;

                    case CompType.PROD_BUILD:
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        static bool CompareCurrent(Operation meta)
        {

            switch(meta.Compare)
            {
                case CompType.CUSTOM_NAME:
                    break;
            }
            return meta.Current.Root != null && meta.Match.Root == null;
        }
        static bool CompareRoot(Operation meta)
        {
            

            return meta.Current.Root == meta.Match.Root;
        }
        static bool CompareTerm(Operation meta)
        {
            if (meta.Current.Term == null ||
                meta.Match.Term == null)
                return false;

            return meta.Current.Term == meta.Match.Term;
        }
        static bool CompareType(Operation meta)
        {
            if (meta.Current.Root == null || meta.Match.Type == null)
                return false;

            if (meta.Current.GetType() == meta.Match.Type)
            {
                meta.Result = meta.Current.Root;
                return true;
            }
            return false;
        }
        static bool CompareString(Operation meta)
        {
            if (meta.Current.String == null || meta.Match.String == null)
                return false;

            return meta.Current.String == meta.Match.String;
        }
        /*static bool SetCompare(Operation meta)
        {
            //switch()
        }*/

        /// Production
        string GenerateRecipes()
        {
            Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> recipeList = new Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId>();

            List<MyProductionItem> nextList = new List<MyProductionItem>();
            string finalList = string.Empty;

            foreach (Producer producer in DATA.Pool.Roots)
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
                    //foreach (IMyProductionBlock block in Base.ProdBlocks)
                    //block.ClearQueue();
                    break;

                case 1:
                    foreach (Producer producer in DATA.Pool.Roots)
                    {
                        Echo("Clearing...");
                        if (producer.ProdBlock != null)
                            producer.ProdBlock.ClearQueue();
                        Echo("Cleared!");
                    }

                    break;
            }
        }

        /// Main
        void RunArguments(string argument)
        {
            switch (argument)
            {
                case "DETECT":
                    DATA.BlockDetection();
                    break;

                case "BUILD":
                    DATA.BlockListSetup();
                    break;

                case "UPDATE":
                    DATA.UpdateSettings();
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

                    /*case "INV":
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
                        break;*/
            }
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

        public Program()
        {
            mySurface = Me.GetSurface(0);
            mySurface.ContentType = ContentType.TEXT_AND_IMAGE;
            mySurface.WriteText("", false);

            RootMeta meta = new RootMeta();

            DATA = new Data(meta);

            LoadRecipes();
            //BlockDetection();

            //Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        public void Main(string argument, UpdateType updateSource)
        {
            string output = ProgEcho();
            Echo(output);
            mySurface.WriteText(output);

            RunArguments(argument);
            DATA.Main();

            Debug.Clear();
        }
        public void Save()
        {
            Storage = Me.CustomData;
        }
        #endregion
    }
}

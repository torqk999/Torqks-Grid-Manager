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
        static bool FilterCompare(_Filter A, _Filter B)
        {
            return FilterCompare(A.ItemType, A.ItemSubType, B.ItemType, B.ItemSubType);
        }

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
        Data DATA;
        RootMeta ROOT;
        SearchCounter Counter = new SearchCounter(new int[] { ItemMoveCap, ItemSearchCap });
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
        const int ClockMax = 3;

        int EchoCount = 0;

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

        public delegate bool RootCompDel(DelegateMeta meta);
        public class BufferMeta<T> : DelegateMeta where T : _Root
        {
            public List<T> Buffer;

            public BufferMeta(List<T> buffer)
            {
                Buffer = buffer;
                cType = typeof(T);
            }
        }
        public class DelegateMeta
        {
            public RootCompDel CompDel;
            public SearchCounter Counter;

            public int Index;
            public bool Compare;
            public bool FAIL;

            public _Root Current;
            public _Root Result;
            
            public Type cType;
            public _Root cRoot;
            public string cString;
            public IMyTerminalBlock cBlock;
        }
        public class Pool
        {
            public List<_Root> Roots = new List<_Root>();
            public DelegateMeta CompareMeta;

            public void Clear<T>() where T : _Root
            {
                CompareMeta.cType = typeof(T);
                CompareMeta.CompDel = CompareTerm;
                DelegateIterator<T>(CompareMeta);
            }
            public bool AppendBlock(DelegateMeta meta)
            {
                if (!CheckCandidate(meta.cBlock))
                    return false;
                Roots.Add(meta.Current);
                return true;
            }
            bool CheckCandidate(IMyTerminalBlock block)
            {
                if (block == null)
                    return false;
                return Return<_Block>(block) == null
                    && block.CustomName.Contains(Signature);
            }

            public bool Return<T>(BufferMeta<T> meta) where T : _Root
            {
                if (meta == null)
                    return false;

                CompareMeta.CompDel = CompareType;
                CompareMeta.cType = typeof(T);

                return DelegatePopulator<T>(meta);
            }
            public T Return<T>(IMyTerminalBlock block) where T : _Root
            {
                if (block == null)
                    return null;

                CompareMeta.Result = null;

                CompareMeta.CompDel = CompareTerm;
                CompareMeta.cBlock = block;
                
                DelegateIterator<T>(CompareMeta);
                return (T)CompareMeta.Result;
            }
            public _Producer Return(string producerType)
            {
                if (producerType == null)
                    return null;

                CompareMeta.Result = null;

                CompareMeta.CompDel = CompareProducer;
                CompareMeta.cString = producerType;
                
                DelegateIterator<_Producer>(CompareMeta);
                return (_Producer)CompareMeta.Result;
            }
            public bool DelegateIterator<T>(DelegateMeta meta) where T : _Root
            {
                for (int i = 0; i < Roots.Count; i++)
                {
                    if ((T)Roots[i] == null)
                        continue;

                    meta.Current = Roots[i];
                    meta.Index = i;

                    try
                    {
                        if (meta.CompDel(meta))
                            break;
                    }

                    catch { }
                }
                return meta.FAIL;
            }

            public bool DelegatePopulator<T>(DelegateMeta meta) where T : _Root
            {
                for (int i = 0; i < Roots.Count; i++)
                {
                    if ((T)Roots[i] == null)
                        continue;

                    meta.Current = Roots[i];

                }
                return meta.FAIL;
            }
        }
        static bool CompareRoot(DelegateMeta meta)
        {
            if (meta.Current == null || meta.cRoot == null)
                return false;

            if (meta.Current == meta.cRoot)
            {
                meta.Result = meta.Current;
                return true;
            }
            return false;
        }
        static bool CompareTerm(DelegateMeta meta)
        {
            if (meta.Current == null || !(meta.Current is _Block) || meta.cBlock == null)
                return false;

            if (((_Block)meta.Current).Block == meta.cBlock)
            {
                meta.Result = meta.Current;
                return true;
            }
            return false;
        }
        static bool CompareType(DelegateMeta meta)
        {
            if (meta.Current == null || meta.cType == null)
                return false;

            if (meta.Current.GetType() == meta.cType)
            {
                meta.Result = meta.Current;
                return true;
            }
            return false;
        }
        static bool CompareProducer(DelegateMeta meta)
        {
            if (meta.Current == null  || meta.cString == null || !(meta.Current is _Producer))
                return false;

            return ((_Producer)meta.Current).ProdBlock.BlockDefinition.SubtypeId == meta.cString
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
            public int RootID;
            public Data DataBase;
            public Program Program;
            public IMyProgrammableBlock Me;
            public IMyTextSurface DebugSurface;
            public StringBuilder DebugBuilder;

            public RootMeta(string signature, Data dataBase, Program program, IMyProgrammableBlock me, IMyTextSurface surface = null)
            {
                DataBase = dataBase;
                RootID = -1;
                if (DataBase != null)
                    RootID = DataBase.RequestIndex();
                Signature = signature;
                Program = program;
                Me = me;
                DebugSurface = surface;
                DebugBuilder = new StringBuilder();
            }
        }
        public struct BlockMeta
        {
            public RootMeta Root;
            public IMyTerminalBlock Block;

            public BlockMeta(RootMeta root, IMyTerminalBlock block = null)
            {
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

            public _FilterProfile FilterProfile;
            public _Block TargetBlock;
            public IMyCubeGrid TargetGrid;
            public IMyBlockGroup TargetGroup;

            public DisplayMeta(RootMeta meta)
            {
                SigCount = DefSigCount;
                TargetName = "No Target";

                Mode = _ScreenMode.DEFAULT;
                Notation = _Notation.DEFAULT;
                TargetType = _Target.DEFAULT;

                FilterProfile = new _FilterProfile(meta, false, false);
                TargetBlock = null;
                TargetGrid = null;
                TargetGroup = null;
            }
        }

        public class Data
        {
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
            public StringBuilder ProductionBuilder = new StringBuilder();
            public bool bShowProdBuilding = false;
            public int ProdCharBuffer = 0;

            public int SetupQueIndex = 0;
            
            public int ROOT_INDEX = -1;

            bool FAIL = false;
            bool bPowerSetupComplete = false;
            bool bSetupComplete = false;
            bool bTallyCycleComplete = false;
            bool bUpdated = false;
            bool bPowerCharged = false;

            public RootMeta ROOT;
            public _Resource PowerMonitor;
            public Pool Pool = new Pool();

            public List<_Block> PowerConsumers = new List<_Block>();
            public List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();
            public List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();

            public Data(RootMeta root)
            {
                ROOT = root;

                BlockDetection();
                ClockInitialize();
                AllBlocksSetup();
            }

            public int RequestIndex()
            {
                ROOT_INDEX++;
                return ROOT_INDEX;
            }
            bool PowerSetup(_Resource candidateMonitor)
            {
                foreach (_Block block in PowerConsumers)
                    block.Priority = -1;

                if (candidateMonitor.Type != _ResType.BATTERY)
                    return false;

                string[] data = candidateMonitor.Block.CustomData.Split('\n');
                int index = 0;

                bool[] checks = new bool[4]; // rough inset

                foreach (string nextline in data)
                {
                    if (Contains(nextline, "prod") && !checks[0])
                    {
                        index = PowerPrioritySet(Pool.Return<_Resource>()>, index);
                        checks[0] = true;
                    }

                    if (Contains(nextline, "ref") && !checks[1])
                    {
                        index = PowerPrioritySet(DATA.Refineries.Cast<_Block>().ToList(), index);
                        checks[1] = true;
                    }

                    if (Contains(nextline, "farm") && !checks[2])
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
                    }
                }

                if (index == 0)
                    return false;

                PowerPrioritySet(PowerConsumers, index);

                PowerConsumers = PowerConsumers.OrderBy(x => x.Priority).ToList(); // << Maybe?

                PowerMonitor = candidateMonitor;

                return true;
            }
            int PowerPrioritySet(List<_Block> consumers, int start)
            {
                int index = 0;

                foreach (_Block block in consumers)
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

            public void UpdateSettings()
            {
                PowerMonitor = null;

                foreach (_Resource resource in Pool.Roots)
                {
                    if (PowerSetup(resource))
                    {
                        bPowerSetupComplete = true;
                        break;
                    }
                }

                //if (!bPowerSetupComplete)
                //    PowerPrioritySet(Base.Blocks, 0);

                foreach (_Cargo nextCargo in Pool.Roots)
                    InventorySetup(nextCargo);

                foreach (_Refinery nextRefine in Pool.Roots)
                    RefinerySetup(nextRefine);

                foreach (_Display display in Pool.Roots)
                    display.Setup();

                ProductionSetup();
            }

            public void ClockInitialize()
            {

            }
            public void RunClock()
            {

            }
            public void BlockDetection()
            {
                BlockGroups.Clear();
                ROOT.Program.GridTerminalSystem.GetBlockGroups(BlockGroups);

                for (int i = 0; i < Getters.Length; i++)
                {
                    Getters[i].BlockList(ROOT.Program, Blocks);
                }

                /*
                ProdBlocks = ProdBlocks.Except(RefineryBlocks).ToList();
                PowerBlocks = PowerBlocks.Except(BatteryBlocks).ToList();*/
            }

            
            public bool BlockListSetup(RootMeta meta, List<IMyTerminalBlock> candidates)
            {



                for (int i = SetupQueIndex; i < SetupQueIndex + SetupCap && i < candidates.Count(); i++)
                {
                    if (!CheckCandidate(candidates[i]))
                        continue;

                    GenerateBlockWrapper(Pool, candidates[i]);
                }
                return SetupQueIndex + SetupCap > candidates.Count();
            }
            void ClearMetaObjects()
            {
                ProductionBuilder.Clear();
                Pool.Roots.Clear();
                PowerConsumers.Clear();
            }

            _Block GenerateBlockWrapper(Pool pool, IMyTerminalBlock block)
            {
                _Block output = null;
                BlockMeta meta = new BlockMeta(ROOT, block);

                if (block is IMyCargoContainer)
                {
                    output = new _Cargo(meta);
                }
                if (block is IMyProductionBlock)
                {
                    output = new _Producer(meta);
                    PowerConsumers.Add(output);
                }
                if (block is IMyRefinery)
                {
                    output = new _Refinery(meta);
                    PowerConsumers.Add(output);
                }
                if (block is IMyTextPanel)
                {
                    output = new _Display(meta, DefFontSize);
                    PowerConsumers.Add(output);
                }

                if (block is IMyBatteryBlock)
                {
                    output = new _Resource(meta, _ResType.BATTERY);
                }
                if (block is IMyGasTank)
                {
                    output = new _Resource(meta, _ResType.GASTANK);
                }
                if (block is IMyPowerProducer)
                {
                    output = new _Resource(meta, _ResType.POWERGEN);
                }
                if (block is IMyGasGenerator)
                {
                    output = new _Resource(meta, _ResType.GASGEN);
                    PowerConsumers.Add(output);
                }
                if (block is IMyOxygenFarm)
                {
                    output = new _Resource(meta, _ResType.OXYFARM);
                    PowerConsumers.Add(output);
                }

                return output;
            }

            bool AllBlocksSetup()
            {
                bool setup = true;
                BlockMeta meta = new BlockMeta(ROOT);
                for (int i = 0; i < BlockBuffers.Length; i++)
                    if (!BlockListSetup(meta, BlockBuffers[i]))
                        setup = false;

                SetupQueIndex += SetupCap;
                return setup;
            }


            /// Setup
            void RefinerySetup(_Refinery refinery)
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
            void InventorySetup(_Inventory inventory)
            {
                inventory.FilterProfile.Filters.Clear();
                inventory.FilterProfile.DEFAULT_IN = true;
                inventory.FilterProfile.DEFAULT_OUT = true;

                string[] data = inventory.Block.CustomData.Split('\n');                                     // Break customData into lines

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
                    inventory.FilterProfile.Append(nextline);
                }

                inventory.FilterProfile.Filters.RemoveAll(x => x.ItemType == "any" && x.ItemSubType == "any");  // Redundant. Refer to inventory default mode
            }
            void ProductionSetup()
            {
                Pool.Clear<_Production>();

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
                    _Production nextProd = new _Production(meta, ROOT);
                    Pool.Roots.Add(nextProd);
                }
            }

            /// Inventory
            void TallyUpdates()
            {
                Counter.Reset();

                for (int i = ProdSearchIndex; i < (ProdSearchIndex + ProdSearchCap) && i < DATA.Productions.Count(); i++)
                {
                    DATA.Productions[i].TallyUpdate(inventory, ref Counter);
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

                    for (int i = InvSearchIndex; i < (InvSearchIndex + InvSearchCap) && i < DATA.Inventories.Count(); i++)
                    {
                        if (Counter.Check(Count.MOVED)) // Total Items moved
                            break;
                        EmptyToCandidate(inventory, DATA.Inventories[i], nextItem);
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
            //void EmptyToCandidates()
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
        }
        public class _Root
        {
            public RootMeta Root;
            public virtual void Setup()
            {

            }
            public virtual void Update()
            {
                Root.DebugBuilder.Clear();
            }
            public _Root(RootMeta meta)
            {
                Root = meta;
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
        public class _FilterProfile : _Root
        {
            public List<_Filter> Filters;

            public bool DEFAULT_OUT;
            public bool DEFAULT_IN;
            public bool FILL;
            public bool EMPTY;

            public _FilterProfile(RootMeta meta, bool defIn = true, bool defOut = true, bool defFill = false, bool defEmpty = false) : base(meta)
            {
                Filters = new List<_Filter>();
                DEFAULT_OUT = defOut;
                DEFAULT_IN = defIn;
                FILL = defFill;
                EMPTY = defEmpty;
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
                    Filters.Add(new _Filter(meta, itemID));
                }

            }
        }
        public class _Tally : _Root
        {
            public _Inventory Inventory;
            public MyItemType ItemType;
            public MyFixedPoint CurrentAmount;
            public MyFixedPoint OldAmount;

            public _Tally(RootMeta meta, _Inventory inventory, MyInventoryItem item) : base(meta)
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
        public class _Production : _Root
        {
            public MyDefinitionId Def;
            public string ProducerType;

            public _Filter Filter;
            public MyFixedPoint Current;
            public List<_Tally> Tallies;

            public _Production(ProdMeta meta, RootMeta root) : base(root)
            {
                Def = meta.Def;
                ProducerType = meta.ProducerType;

                FilterMeta filter = new FilterMeta(true, true, meta.Target);
                Filter = new _Filter(filter, Def);
                Tallies = new List<_Tally>();
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

                    _Tally sourceTally = Tallies.Find(x => x.Inventory == sourceInventory);
                    if (sourceTally == null)
                    {

                        sourceTally = new _Tally(Root, sourceInventory, item);
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

                List<_Producer> candidates = Root.DataBase.Pool.Return(ProducerType);
                List<MyProductionItem> existingQues = new List<MyProductionItem>();

                foreach (_Producer producer in candidates)
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

                    existingQues.AddRange(nextList.FindAll(x => FilterCompare(Root.DebugBuilder, Filter, x)));
                }

                MyFixedPoint existingQueAmount = 0;
                foreach (MyProductionItem item in existingQues)
                    existingQueAmount += item.Amount;

                MyFixedPoint projectedTotal = Current + existingQueAmount;
                MyFixedPoint projectedOverage = Filter.Meta.Target - projectedTotal;

                if (projectedOverage >= 0)
                {
                    MyFixedPoint remove = new MyFixedPoint();

                    foreach (_Producer producer in candidates)
                    {
                        existingQues.Clear();
                        producer.ProdBlock.GetQueue(existingQues);
                        for (int i = 0; i < existingQues.Count; i++)
                        {
                            if (!FilterCompare(Root.DebugBuilder, Filter, existingQues[i]))
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

                    foreach (_Producer producer in candidates)                                  // Distribute
                        producer.ProdBlock.AddQueueItem(Def, qeueIndividual);
                }
            }
        }

        public class _Block : _Root
        {
            public long BlockID;
            public int Priority;
            public string CustomName;
            public bool bDetatchable;
            public IMyTerminalBlock Block;

            public _Block(BlockMeta meta) : base(meta.Root)
            {
                Block = meta.Block;
                CustomName = Block.CustomName.Replace(meta.Root.Signature, "");
                BlockID = Block.EntityId;
                Priority = -1;
            }
            public bool CheckBlockExists()
            {
                IMyTerminalBlock maybeMe = Root.Program.GridTerminalSystem.GetBlockWithId(Block.EntityId); // BlockID?
                if (maybeMe != null &&                          // Exists?
                    maybeMe.CustomName.Contains(Signature))    // Signature?
                    return true;

                if (!bDetatchable)
                    Root.DataBase.Pool.Roots.Remove(this);

                return false;
            }
        }
        public class _Display : _Block
        {
            public IMyTextPanel Panel;
            public string oldData;
            public float oldFontSize;

            public StringBuilder rawOutput;
            public StringBuilder fOutput;

            public int[] WrapVectors = new int[2]; // chars, lines
            public int OutputIndex;
            public int Scroll;
            public int Delay;
            public int Timer;

            public DisplayMeta Meta;

            public _Display(BlockMeta meta, float fontSize = 1) : base(meta)
            {
                Panel = (IMyTextPanel)meta.Block;
                RebootScreen(fontSize);
                oldData = Panel.CustomData;
                oldFontSize = Panel.FontSize;
                rawOutput = new StringBuilder();
                fOutput = new StringBuilder();
                OutputIndex = 0;
                Scroll = 1;
                Delay = 10;
                Timer = 0;

                Meta = new DisplayMeta(meta.Root);
            }
            public override void Setup()
            {
                Meta.FilterProfile.Filters.Clear();

                string[] data = Panel.CustomData.Split('\n');

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
                                Meta.Mode = _ScreenMode.STATUS;
                            if (Contains(nextline, "inv"))
                                Meta.Mode = _ScreenMode.INVENTORY;
                            if (Contains(nextline, "prod"))
                                Meta.Mode = _ScreenMode.PRODUCTION;
                            if (Contains(nextline, "res"))
                                Meta.Mode = _ScreenMode.RESOURCE;
                            if (Contains(nextline, "tally"))
                                Meta.Mode = _ScreenMode.TALLY;
                            break;

                        case '@': // Target
                            if (Contains(nextline, "block"))
                            {
                                _Block block = Root.DataBase.Blocks.Find(x => x.CustomName.Contains(lineblocks[1]));

                                if (block != null)
                                {

                                    Meta.TargetType = _Target.BLOCK;
                                    Meta.TargetBlock = block;
                                    Meta.TargetName = block.CustomName;
                                }
                                else
                                {
                                    Meta.TargetType = _Target.DEFAULT;
                                    Meta.TargetName = "Block not found!";
                                }
                                break;
                            }
                            if (Contains(nextline, "group"))
                            {
                                IMyBlockGroup targetGroup = Root.DataBase.BlockGroups.Find(x => x.Name.Contains(lineblocks[1]));
                                if (targetGroup != null)
                                {
                                    Meta.TargetType = _Target.GROUP;
                                    Meta.TargetGroup = targetGroup;
                                    Meta.TargetName = targetGroup.Name;
                                }
                                else
                                {
                                    Meta.TargetType = _Target.DEFAULT;
                                    Meta.TargetName = "Group not found!";
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
                                Meta.Notation = _Notation.DEFAULT;
                            if (Contains(nextline, "simp"))
                                Meta.Notation = _Notation.SIMPLIFIED;
                            if (Contains(nextline, "sci"))
                                Meta.Notation = _Notation.SCIENTIFIC;
                            if (Contains(nextline, "%"))
                                Meta.Notation = _Notation.PERCENT;
                            break;

                        default:
                            Meta.FilterProfile.Append(nextline);
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
                int Offset = (Meta.TargetType == _Target.GROUP && Meta.Mode == _ScreenMode.INVENTORY) ? 4 : 2; // Header Work around
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

                switch (Meta.Mode)
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

                if (Meta.Mode > (_ScreenMode)3)
                    return; // No Target for tally systems

                switch (Meta.TargetType)
                {
                    case _Target.DEFAULT:
                        rawOutput.Append("=" + "/" + Meta.TargetName);
                        break;

                    case _Target.BLOCK:
                        if (Meta.Mode == _ScreenMode.RESOURCE || Meta.Mode == _ScreenMode.STATUS)
                            TableHeaderBuilder();
                        RawBlockBuilder(Meta.TargetBlock);
                        break;

                    case _Target.GROUP:
                        RawGroupBuilder(Meta.TargetGroup);
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
                            if (!Root.DataBase.bShowProdBuilding)
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

                                    for (int i = 0; i < Root.DataBase.ProdCharBuffer - blocks[1].Length; i++)
                                        formattedString += " ";
                                    formattedString += " | " + blocks[2];
                                    for (int i = 0; i < (chars - (Root.DataBase.ProdCharBuffer + blocks[2].Length + blocks[3].Length + blocks[4].Length + 4)); i++)
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
                rawOutput.Append("=" + Seperator + "(" + Meta.TargetName + ")\n");
                rawOutput.Append("#\n");

                TableHeaderBuilder();

                List<IMyTerminalBlock> groupList = new List<IMyTerminalBlock>();
                Meta.TargetGroup.GetBlocks(groupList);

                foreach (IMyTerminalBlock nextTermBlock in groupList)
                {
                    _Block next = Root.DataBase.Pool.Return(nextTermBlock);
                    if (next != null)
                        RawBlockBuilder(next);
                    else
                        rawOutput.Append("!" + "/" + "Block data class not found! Signature missing from block in group!\n");
                }
            }
            void RawBlockBuilder(_Block target)
            {
                switch (Meta.Mode)
                {
                    case _ScreenMode.DEFAULT:

                        break;

                    case _ScreenMode.INVENTORY:
                        BlockInventoryBuilder(target);
                        break;

                    case _ScreenMode.RESOURCE:
                        try
                        {
                            _Resource resource = (_Resource)target;
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
            void BlockInventoryBuilder(_Block targetBlock)
            {
                rawOutput.Append("=" + Seperator + targetBlock.CustomName + "\n");

                if (targetBlock is _Cargo)
                    InventoryBuilder(PullInventory((_Cargo)targetBlock));

                else if (targetBlock is _Inventory)
                {
                    rawOutput.Append("=" + Seperator + "|Input|\n");
                    InventoryBuilder(((IMyProductionBlock)((_Inventory)targetBlock).Block).InputInventory);
                    rawOutput.Append("#\n");
                    rawOutput.Append("=" + Seperator + "|Output|\n");
                    InventoryBuilder(((IMyProductionBlock)((_Inventory)targetBlock).Block).OutputInventory);
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
                    rawOutput.Append(ParseItemTotal(next, Meta) + "\n");
            }
            void ProductionBuilder()
            {
                rawOutput.Append("#");

                foreach (_Production prod in Root.DataBase.Pool.Roots)
                {
                    rawOutput.Append("\n@" + Seperator);
                    string nextDef = prod.Filter.ItemSubType;
                    rawOutput.Append(nextDef + Seperator);
                    Root.DataBase.ProdCharBuffer = (Root.DataBase.ProdCharBuffer > nextDef.Length) ? Root.DataBase.ProdCharBuffer : nextDef.Length;

                    rawOutput.Append(
                        prod.ProducerType + Seperator +
                        prod.Current + Seperator +
                        prod.Filter.Meta.Target);
                }
            }
            void ItemTallyBuilder()
            {
                rawOutput.Append("#" + Seperator + "\n");

                if (Meta.FilterProfile.Filters.Count == 0)
                {
                    rawOutput.Append("!" + Seperator + "No Filter!\n");
                    return;
                }

                Dictionary<string, MyFixedPoint> itemTotals = new Dictionary<string, MyFixedPoint>();
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                _Inventory targetInventory;

                switch (Meta.TargetType)
                {
                    case _Target.BLOCK:
                        targetInventory = Root.DataBase.Pool..Find(x => x == (_Inventory)Block);
                        PullInventory(targetInventory).GetItems(items);
                        ItemListBuilder(itemTotals, items, Meta.FilterProfile);
                        break;

                    case _Target.GROUP:

                        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                        Meta.TargetGroup.GetBlocks(blocks);
                        foreach (IMyTerminalBlock block in blocks)
                        {
                            targetInventory = Root.DataBase.Pool.Return<_Inventory>(block);
                            if (targetInventory == null)
                                continue;
                            items.Clear();
                            PullInventory(targetInventory).GetItems(items);
                            ItemListBuilder(itemTotals, items, Meta.FilterProfile);
                        }
                        break;
                }

                foreach (var next in itemTotals)
                    rawOutput.Append(ParseItemTotal(next, Meta) + "\n");
            }
            void TableHeaderBuilder()
            {
                switch (Meta.Mode)
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
            void BlockResourceBuilder(_Resource targetBlock)
            {
                rawOutput.Append("%" + Seperator + targetBlock.CustomName + Seperator);

                string value = string.Empty;
                int percent = 0;
                string unit = "n/a";

                switch (Meta.Notation)
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


                rawOutput.Append(((Meta.Notation == _Notation.PERCENT) ? percent + "| % " : (value + Seperator + unit)) + "\n");
            }
        }
        public class _Inventory : _Block
        {
            public _FilterProfile FilterProfile;
            public int CallBackIndex;
            public _Inventory(BlockMeta meta, bool active = true) : base(meta)
            {
                FilterProfile = new _FilterProfile(meta.Root);
                CallBackIndex = -1;
            }
        }
        public class _Cargo : _Inventory
        {
            public _Cargo(BlockMeta meta) : base(meta)
            {

            }
        }
        public class _Producer : _Inventory
        {
            public IMyProductionBlock ProdBlock;

            public bool CLEAN;
            public _Producer(BlockMeta meta) : base(meta)
            {
                ProdBlock = (IMyProductionBlock)meta.Block;
                CLEAN = true;
            }
        }
        public class _Refinery : _Inventory
        {
            public IMyRefinery RefineBlock;
            public bool AutoRefine;

            public _Refinery(BlockMeta meta, bool auto = false) : base(meta)
            {
                RefineBlock = (IMyRefinery)meta.Block;
                AutoRefine = auto;
                RefineBlock.UseConveyorSystem = AutoRefine;
            }
        }
        public class _Resource : _Block
        {
            public _ResType Type;
            public bool bIsValue;

            public _Resource(BlockMeta meta, _ResType type, bool isValue = true) : base(meta)
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

 
        /// Production
        string GenerateRecipes()
        {
            Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId> recipeList = new Dictionary<MyProductionItem, VRage.ObjectBuilders.SerializableDefinitionId>();

            List<MyProductionItem> nextList = new List<MyProductionItem>();
            string finalList = string.Empty;

            foreach (_Producer producer in DATA.Pool.Roots)
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
                    foreach (_Producer producer in DATA.Pool.Roots)
                    {
                        Echo("Clearing...");
                        if (producer.ProdBlock != null)
                            producer.ProdBlock.ClearQueue();
                        Echo("Cleared!");
                    }

                    break;
            }
        }

        /// Power
        
        void PowerAdjust(bool bAdjustUp = true)
        {
            if (bAdjustUp)
                foreach (_Block block in DATA.PowerConsumers)
                    ((IMyFunctionalBlock)block.Block).Enabled = true;

            else
                for (int i = DATA.PowerConsumers.Count - 1; i > -1; i--)
                {
                    if (((IMyFunctionalBlock)DATA.PowerConsumers[i].Block).Enabled)
                    {
                        ((IMyFunctionalBlock)DATA.PowerConsumers[i].Block).Enabled = false;
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
                    DATA.BlockDetection();
                    //ClearDataBase();
                    bSetupComplete = false;
                    bUpdated = false;
                    break;

                case "BUILD":
                    //ClearDataBase();
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

        

        /*void RunClock()
        {
            // Persistent Updates
            if (bPowerRunning)
                PowerUpdate();

            if (bDisplayRunning)
                foreach (_Display display in Base.Displays)
                    //if (CheckBlockExists(display))
                    display.DisplayRefresh();

            // Inventory Updates
            if(InventoryClock == ClockMax &&
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
                ProdClock == ClockMax &&
                Productions.Count > 0)
            {
                ProductionUpdate(Productions[ProdQueIndex]);

                ProdQueIndex++;
                ProdQueIndex = (ProdQueIndex >= Productions.Count) ? 0 : ProdQueIndex;
            }

            // Display Updates
            if (bDisplayRunning &&
                DisplayClock == ClockMax &&
                Displays.Count > 0)
            {
                if (CheckBlockExists(Displays[DisplayQueIndex]))
                Displays[DisplayQueIndex].DisplayUpdate();

                DisplayQueIndex++;
                DisplayQueIndex = (DisplayQueIndex >= Displays.Count) ? 0 : DisplayQueIndex;
            }

            // Clock Updates
            DisplayClock = (DisplayClock == ClockMax) ? 0 : (DisplayClock + 1);
            InventoryClock = (InventoryClock == ClockMax) ? 0 : (InventoryClock + 1);
            ProdClock = (ProdClock == ClockMax) ? 0 : (ProdClock + 1);
        }*/

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

            if (FAIL)
                return;

            if (!bSetupComplete)
                bSetupComplete = BlockListSetup();

            if (bSetupComplete && !bUpdated)
                bUpdated = UpdateSettings();

            if (bSetupComplete && bUpdated)
            {
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

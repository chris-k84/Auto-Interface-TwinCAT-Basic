using TCatSysManagerLib;
using System.Xml;
using System.Windows;
using System.Collections.Generic;
using System;
using System.IO;

namespace Engine
{
    public enum CpuAffinity : ulong
    {
        CPU1 = 0x0000000000000001,
        CPU2 = 0x0000000000000002,
        CPU3 = 0x0000000000000004,
        CPU4 = 0x0000000000000008,
        CPU5 = 0x0000000000000010,
        CPU6 = 0x0000000000000020,
        CPU7 = 0x0000000000000040,
        CPU8 = 0x0000000000000080,
        None = 0x0000000000000000,
        MaskSingle = CPU1,
        MaskDual = CPU1 | CPU2,
        MaskQuad = MaskDual | CPU3 | CPU4,
        MaskHexa = MaskQuad | CPU5 | CPU6,
        MaskOct = MaskHexa | CPU7 | CPU8,
        MaskAll = 0xFFFFFFFFFFFFFFFF
    }
    public interface ITwinCATHandler
    {
        void CreateTask(string taskName);
        void CreateTask(string taskName, int taskPriority);
        void CreateTask(string taskName, int taskPriority, int taskCycleTime);
        void AssignCores(string taskName, int core);
        void CreateLink(string source, string destination);
        void GetMappings();
        void LoadMappings(string mappingInfo);
        void SetIsolatedCores();
        void ActivateSolution();
    }
    public class TwinCATHandler : ITwinCATHandler
    {
        #region Fields
        private ITcSysManager8 _sysMan;
        Dictionary<string, string> _tcomModuleTable = new Dictionary<string, string>();
        private ISystemManager _systemManager;
        #endregion
        #region Properties
        public ISystemManager systemManager
        {
            get
            {
                return _systemManager;
            }
            set
            {
                _systemManager = value;
                _sysMan = value.SysMan;
            }
        }
        #endregion
        #region Constructors
        public TwinCATHandler()
        {

        }
        public TwinCATHandler(ISystemManager systemManager)
        {
            this._sysMan = systemManager.SysMan;
        }
        #endregion
        #region Methods
        public void CreateTask(string taskName)
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
            }
            catch (Exception c)
            {
               
            }
        }
        public void CreateTask(string taskName, int taskPriority)
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
                string xmlPriority = String.Format("<TreeItem><TaskDef><Priority>{0}</Priority></TaskDef></TreeItem>", taskPriority.ToString());
                tTask.ConsumeXml(xmlPriority);
            }
            catch (Exception c)
            {
                
            }
        }
        public void CreateTask(string taskName, int taskPriority, int taskCycleTime)
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
                string xmlPriority = String.Format("<TreeItem><TaskDef><Priority>{0}</Priority></TaskDef></TreeItem>", taskPriority.ToString());
                string xmlCycle = String.Format("<TreeItem><TaskDef><CycleTime>{0}</CycleTime></TaskDef></TreeItem>", taskCycleTime.ToString());
                tTask.ConsumeXml(xmlPriority);
                tTask.ConsumeXml(xmlCycle); //defined in 100s of ns so 10000 = 1ms
            }
            catch (Exception c)
            {
               
            }
        }
        public void CreatePDOonTask(string task, string name, string type)
        {
            ITcSmTreeItem task1 = _sysMan.LookupTreeItem(task);
            task1.CreateChild(name, 0, null, null);
        }
        public void EnableSingleCoreForRT(int core)
        {
            ITcSmTreeItem realtimeSettings = _sysMan.LookupTreeItem("TIRS");
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlTextWriter.Create(stringWriter))
                {
                    writer.WriteStartElement("TreeItem");
                    writer.WriteStartElement("RTimeSetDef");
                    writer.WriteElementString("MaxCPUs", "4");
                    string affinityString = "";
                    switch (core)
                    {
                        case 1:
                            affinityString = string.Format("#x{0}", ((ulong)CpuAffinity.CPU1).ToString("x16"));
                            break;
                        case 2:
                            affinityString = string.Format("#x{0}", ((ulong)CpuAffinity.CPU2).ToString("x16"));
                            break;
                        case 3:
                            affinityString = string.Format("#x{0}", ((ulong)CpuAffinity.CPU3).ToString("x16"));
                            break;
                        case 4:
                            affinityString = string.Format("#x{0}", ((ulong)CpuAffinity.CPU4).ToString("x16"));
                            break;
                        default:
                            affinityString = string.Format("#x{0}", ((ulong)CpuAffinity.CPU1).ToString("x16"));
                            break;
                    }
                    writer.WriteElementString("Affinity", affinityString);
                    writer.WriteStartElement("CPUs");
                    EnableACore(writer, 0, 10, 10000, 200);
                    writer.WriteEndElement();     // CPUs     
                    writer.WriteEndElement();     // RTimeSetDef     
                    writer.WriteEndElement();     // TreeItem 
                }
                string xml = stringWriter.ToString();
                realtimeSettings.ConsumeXml(xml);
            }
        }
        private void EnableACore(XmlWriter writer, int id, int loadLimit, int baseTime, int latencyWarning)
        {
            writer.WriteStartElement("CPU");
            writer.WriteAttributeString("id", id.ToString());
            writer.WriteElementString("LoadLimit", loadLimit.ToString());
            writer.WriteElementString("BaseTime", baseTime.ToString());
            writer.WriteElementString("LatencyWarning", latencyWarning.ToString());
            writer.WriteEndElement();
        }
        public void AssignCores(string taskName, int core)
        {
            String path = string.Format("TIRT^{0}", taskName);
            ITcSmTreeItem tasks = _sysMan.LookupTreeItem(path);
            string coreAssignment = null;
            switch (core)
            {
                case 1:
                    coreAssignment = string.Format("0x{0:X}", CpuAffinity.CPU1);
                    break;
                case 2:
                    coreAssignment = string.Format("0x{0:X}", CpuAffinity.CPU2);
                    break;
                case 3:
                    coreAssignment = string.Format("0x{0:X}", CpuAffinity.CPU3);
                    break;
                case 4:
                    coreAssignment = string.Format("0x{0:X}", CpuAffinity.CPU4);
                    break;
                default:
                    coreAssignment = string.Format("0x{0:X}", CpuAffinity.CPU1);
                    break;
            }

            string xmlCore = String.Format("<TreeItem><TaskDef><CpuAffinity>{0}</CpuAffinity></TaskDef></TreeItem>", coreAssignment);
            tasks.ConsumeXml(xmlCore);
        }
        public void CreateLink(string source, string destination)
        {
            try
            {
                _sysMan.LinkVariables(source, destination);
            }
            catch (Exception e)
            {
                
            }
        }
        public void GetMappings()
        {
            try
            {
                string mappingInfo = _sysMan.ProduceMappingInfo();
            }
            catch (Exception e)
            {
                
            }
        }
        public void LoadMappings(string mappingInfo)
        {
            try
            {
                _sysMan.ConsumeMappingInfo(mappingInfo);

            }
            catch (Exception e)
            {
               
            }
        }
        public void SetIsolatedCores()
        {
            ITcSmTreeItem ITrItm = _sysMan.LookupTreeItem("TIRS");
            string sCpuConfig = ITrItm.ProduceXml();
            sCpuConfig = sCpuConfig.Replace("<MaxCPUs NonWindowsCPUs=\"2\">5</MaxCPUs>", "<MaxCPUs NonWindowsCPUs=\"4\">5</MaxCPUs>");
            ITrItm.ConsumeXml(sCpuConfig);
        }
        public void ActivateSolution()
        {
            _sysMan.ActivateConfiguration();
            _sysMan.StartRestartTwinCAT();
        }
        public ITcSmTreeItem LookUpTcCOMNode()
        {
            return _sysMan.LookupTreeItem("TIRC^TcCOM Objects");
        }
        public ITcSmTreeItem LookUpTaskNode()
        {
            return _sysMan.LookupTreeItem("TIRT");
        }
        public ITcSmTreeItem LookUpRealTimeNode()
        {
            return _sysMan.LookupTreeItem("TIRS");
        }
        public void LoadTcCOMToStore(string modelName, string guid)
        {
            try
            {
                _tcomModuleTable.Add(modelName, guid);
            }
            catch (Exception e)
            {
                
            }
        }
        public void AddTcCOM(string Module, ITcSmTreeItem TcCom)
        {
            try
            {
                ITcSmTreeItem tempController = TcCom.CreateChild(Module, 0, "", _tcomModuleTable[Module]);
            }
            catch (Exception e)
            {
                
            }
        }
        public void AddTcCOM(string Module, string guid, ITcSmTreeItem TcCom) 
        {
            try
            {
                ITcSmTreeItem tempController = TcCom.CreateChild(Module, 0, "Test^", guid);
            }
            catch (Exception e)
            {

            }
        }
        public ITcModuleManager2 GetModuleManager()
        {
            return (ITcModuleManager2)_sysMan.GetModuleManager();
        }
        public IList<ITcModuleInstance2> GetModules(ITcModuleManager2 ModuleManager)
        {
            List<ITcModuleInstance2> modules = new List<ITcModuleInstance2>();  
            foreach (ITcModuleInstance2 module in ModuleManager)
            {
                modules.Add(module);
            }
            return modules;
        }
        public void SetModuleContext(uint index, uint Oid, ITcModuleInstance2 module)
        {
            ITcModuleManager2 moduleManager = (ITcModuleManager2)_sysMan.GetModuleManager();
            module.SetModuleContext(index, Oid);
        }
        public Tuple<uint, uint> GetModuleContext(ITcModuleInstance2 module)
        {
            uint index = 0;
            uint Oid = 0;
            ITcModuleManager2 moduleManager = (ITcModuleManager2)_sysMan.GetModuleManager();
            module.GetModuleContext(index, ref Oid);
            return Tuple.Create(index, Oid);
        }
        public ITcSmTreeItem LookUpNode(string node)
        {
            return _sysMan.LookupTreeItem(node);
        }
        public string GetTreeItemXml(ITcSmTreeItem node)
        {
            return node.ProduceXml();
        }
        public void DeployTreeItemXml(ITcSmTreeItem node, string xml)
        {
            node.ConsumeXml(xml);
        }
        public void GetTreeItemXti(ITcSmTreeItem node, string child, string file)
        {
            node.ExportChild(child, file);
        }
        #endregion
    }
}

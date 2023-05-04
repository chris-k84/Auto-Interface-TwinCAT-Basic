using TCatSysManagerLib;
using System.Xml;
using System.Windows;
using System.Collections.Generic;
using System;

namespace Engine
{
    public interface ITwinCATHandler
    {
        void CreateTask(string taskName);
        void CreateTask(string taskName, int taskPriority);
        void CreateTask(string taskName, int taskPriority, int taskCycleTime);
        void AssignCores();
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
        public void AssignCores() //TODO add code for this function
        {

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

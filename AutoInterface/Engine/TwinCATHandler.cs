using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Engine
{
    public interface ITwinCATHandler
    {
        void CreateTask(string taskName);
        
        void CreateTask(string taskName, int taskPriority);
        
        void CreateTask(string taskName, int taskPriority, int taskCycleTime);
        
        void AssignCores();
   
        void SetAMSNET(string amsNetId);
        
        string ScanADSDevices();


        void CreateLink(string source, string destination);


        void GetMappings();


        void LoadMappings(string mappingInfo);


        void SetIsolatedCores();
        
        void ActivateSolution();
    }
    public class TwinCATHandler : ITwinCATHandler
    {
        #region Fields
        ITcSysManager13 _sysMan;
        string _xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        string _routes;
        XmlDocument _routeXml = new XmlDocument();
        Dictionary<string, Guid> _tcomModuleTable = new Dictionary<string, Guid>();
        
        #endregion

        #region Constructors
        public TwinCATHandler()
        {

        }
        public TwinCATHandler(ITcSysManager13 sysManager)
        {
            _sysMan = sysManager;
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
                MessageBox.Show(c.Message);
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
                MessageBox.Show(c.Message);
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
                MessageBox.Show(c.Message);
            }
        }
        public void AssignCores() //TODO add code for this function
        {

        }
        public void SetAMSNET(string amsNetId)
        {
            try
            {
                _sysMan.SetTargetNetId(amsNetId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public string ScanADSDevices()
        {
            try
            {
                ITcSmTreeItem routes = _sysMan.LookupTreeItem("TIRR");
                routes.ConsumeXml(_xmlRouteString);
                _routes = routes.ProduceXml();

                _routeXml.LoadXml(_routes);
            
                XmlNodeList xmlDeviceList = _routeXml.SelectNodes("TreeItem/RoutePrj/TargetList/Target");
                foreach (XmlNode node in xmlDeviceList) //TODO handle choosing the right target
                {
                    return node.SelectSingleNode("NetId").InnerText;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - ADS Error - " + e.Message);
                _routes = "";
            }
            return _routes;
        }
        public void CreateLink(string source, string destination)
        {
            try
            {
                _sysMan.LinkVariables(source, destination);
            }
            catch (Exception e)
            {
                MessageBox.Show("Process error - Link error" + e.Message);
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
                MessageBox.Show(e.Message);
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
                MessageBox.Show(e.Message);
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
        public ITcSmTreeItem LookUpTcCOM()
        {
            return _sysMan.LookupTreeItem("TIRC^TcCOM Objects");
        }
        public void LoadTcCOM(string tcCOM)
        {
            try
            {
                _tcomModuleTable.Add("TctSmplTempCtrl", Guid.Parse("{2462082a-5901-4cf0-b10d-4f472a5918d4}"));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void AddTcCOM(string Module, ITcSmTreeItem TcCom)
        {
            try
            {
                ITcSmTreeItem tempController = TcCom.CreateChild(Module, 0, "", _tcomModuleTable["TctSmplTempCtrl"]);
                tempController.Name = "Onat";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        public ITcSmTreeItem LookUpNode(string node)
        {
            return _sysMan.LookupTreeItem(node);
        }
        public string GetTreeItemXml(ITcSmTreeItem node)
        {
            return node.ProduceXml();
        }
        public void DeployTreeItemXml(ITcSmTreeItem node,string xml)
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

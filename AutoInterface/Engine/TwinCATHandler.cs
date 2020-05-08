using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using System.IO;
using System.Windows.Forms;

namespace Engine
{
    class TwinCATHandler
    {
        #region Fields
        ITcSysManager3 sysMan;
        string tcTemplate = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        string _dirPath;
        dynamic solution;
        string _solName;
        dynamic tcProject;
        string xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        string _routes;
        #endregion

        #region Methods
        public void CreateTCProj()
        {
            try
            {
                tcProject = solution.AddFromTemplate(tcTemplate, Path.Combine(_dirPath, _solName), _solName);
                sysMan = tcProject.Object;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - TC Project Creation - " + e.Message);
            }
        }
        public void CreateTask(string taskName)
        {
            try
            {
                ITcSmTreeItem tasks = sysMan.LookupTreeItem("TIRT");
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
                ITcSmTreeItem tasks = sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
                string xmlPriority = String.Format("<TreeItem><TaskDef><Priority>{0}</Priority></TaskDef></TreeItem>", taskPriority.ToString());
                tTask.ConsumeXml(xmlPriority);
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }
        public void CreateTask(string taskName, int taskPriority, int taskCycleTime) //TODO add reference to task to main class
        {
            try
            {
                ITcSmTreeItem tasks = sysMan.LookupTreeItem("TIRT");
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
                sysMan.SetTargetNetId(amsNetId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public string ScanADSDevices() //TODO to return pointer to xml
        {
            try
            {
                ITcSmTreeItem routes = sysMan.LookupTreeItem("TIRR");
                routes.ConsumeXml(xmlRouteString);
                _routes = routes.ProduceXml();
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - PLC Creation - " + e.Message);
                _routes = "";
            }
            return _routes;
        }
        public void CreateLink(string source, string destination)
        {
            try
            {
                sysMan.LinkVariables(source, destination);
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
                string mappingInfo = sysMan.ProduceMappingInfo();
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
                sysMan.ConsumeMappingInfo(mappingInfo);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
    }
}

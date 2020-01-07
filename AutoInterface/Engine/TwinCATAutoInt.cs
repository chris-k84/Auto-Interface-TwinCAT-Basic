using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EnvDTE;
using TCatSysManagerLib;
using System.Windows.Forms;
using System.Xml;

namespace Engine
{
    public class TwinCATAutoInt
    {

        #region Fields Region
        ITcSysManager3 sysMan; //be careful with ITcSysManager(X) it has multi layered interfaces
        ITcSmTreeItem _plcProj;
        ITcSmTreeItem _gvlFolder;
        ITcSmTreeItem _gvlList;
        ITcPlcDeclaration _itcGvlList;
        List<ITcSmTreeItem> devices;
        EnvDTE.DTE dte;
        string _dirPath;
        string _solName;
        dynamic solution;
        dynamic tcProject;
        string tcTemplate = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        string xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        string _routes;
        #endregion

        #region Properties Region   
        #endregion

        #region Constructor Region
        #endregion

        #region Method Region
        public void SetVSDevEnv()
        {
            try
            {
                Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE");
                dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
                dte.SuppressUI = false;
                dte.MainWindow.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - GVL Creation" + e.Message);
            }
        }

        public void CreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path,true);
                }
                Directory.CreateDirectory(path);
                _dirPath = path;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - Directory Path Creation - " + e.Message);
            }
        }

        public void CreateSolution(string name)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(_dirPath, name));
                solution = dte.Solution;
                solution.Create(_dirPath, name);
                _solName = name;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - Solution Creation - " + e.Message);
            }
        }

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

        public void SaveAll()
        {
            string slnName = _solName + ".sln";
            solution.SaveAs(Path.Combine(_dirPath, slnName));
            tcProject.Save();
        } //TODO save all save project and solution, need to check this

        public void CreatePLCProj(string name)
        {
            try
            {
                ITcSmTreeItem plc = sysMan.LookupTreeItem("TIPC");
                _plcProj = plc.CreateChild(name, 0, "", $@"Standard PLC Template.plcproj");
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - PLC Creation - " + e.Message);
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

        public void AddGVL() //TODO extend to return the iterface to the GVL declaration area
        {
            try
            {
                _gvlFolder = sysMan.LookupTreeItem("TIPC^" + _plcProj.Name + "^" + _plcProj.Name + " Project^GVLs");
                _gvlList = _gvlFolder.CreateChild("Global_IO", 615, "", IECLANGUAGETYPES.IECLANGUAGE_ST);
                _itcGvlList = (ITcPlcDeclaration)_gvlList;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - GVL Creation" + e.Message);
            }
            
        }

        public void ScanIO() //TODO still working on this - add ams net id to the method call
        {
            XmlDocument xmlDoc = new XmlDocument();
            int deviceCount = 0;
            devices = new List<ITcSmTreeItem>();
            string xml;
            ITcSmTreeItem ioDevicesItem = sysMan.LookupTreeItem("TIID");
            string scannedXml = ioDevicesItem.ProduceXml(false);
            xmlDoc.LoadXml(scannedXml);
            XmlNodeList xmlDeviceList = xmlDoc.SelectNodes("TreeItem/DeviceGrpDef/FoundDevices/Device");
            foreach (XmlNode node in xmlDeviceList)
            {

                int itemSubType = int.Parse(node.SelectSingleNode("ItemSubType").InnerText);
                string typeName = node.SelectSingleNode("ItemSubTypeName").InnerText;
                XmlNode xmlAddress = node.SelectSingleNode("AddressInfo");
                if (itemSubType == 111)
                {
                    ITcSmTreeItem device = ioDevicesItem.CreateChild(string.Format("Device_{0}", ++deviceCount, typeName), itemSubType, string.Empty, null);
                    xml = string.Format("<TreeItem><DeviceDef>{0}</DeviceDef></TreeItem>", xmlAddress.OuterXml);
                    device.ConsumeXml(xml);
                    devices.Add(device);
                }
            }
            foreach (ITcSmTreeItem device in devices)
            {
                xml = "<TreeItem><DeviceDef><ScanBoxes>1</ScanBoxes></DeviceDef></TreeItem>";
                try
                {
                    device.ConsumeXml(xml);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Warning: {0}", ex.Message);
                }
                for (int i = 1; i < device.ChildCount; i++)
                {
                    //Console.WriteLine(device.Child[i].Name);
                    Console.WriteLine(device.Child[i].PathName);
                //TODO: try pathName
                }
            }

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
        #endregion
    }
}

using TCatSysManagerLib;
using System.Xml;
using System.Windows;

namespace Engine
{
    public interface ITwinCATHandler
    {
        void CreateTask(string taskName);
        void CreateTask(string taskName, int taskPriority);
        void CreateTask(string taskName, int taskPriority, int taskCycleTime);
        void AssignCores();
        void SetAMSNET(string amsNetId);
        void CreateLink(string source, string destination);
        void GetMappings();
        void LoadMappings(string mappingInfo);
        void SetIsolatedCores();
        void ActivateSolution();
    }
    public class TwinCATHandler : ITwinCATHandler
    {
        #region Fields
        private ITcSysManager15 _sysMan;
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
        public void SetAMSNET(string amsNetId)
        {
            try
            {
                _sysMan.SetTargetNetId(amsNetId);
            }
            catch (Exception e)
            {
                
            }
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
        public ITcSmTreeItem LookUpTcCOM()
        {
            return _sysMan.LookupTreeItem("TIRC^TcCOM Objects");
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
                ITcSmTreeItem tempController = TcCom.CreateChild(Module, 0, "", guid);
            }
            catch (Exception e)
            {

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
        public void DeployTreeItemXml(ITcSmTreeItem node, string xml)
        {
            node.ConsumeXml(xml);
        }
        public void GetTreeItemXti(ITcSmTreeItem node, string child, string file)
        {
            node.ExportChild(child, file);
        }

        //public void BroadcastSearch()
        //{
        //    ///BROADCAST SEARCH AND FILTER
        //    //Search EtherNET at a specific IP Address for a TwinCAT installation
        //    //string xmlString = "<TreeItem><RoutePrj><TargetList><Search>"+textBox2.Text+"</Search></TargetList></RoutePrj></TreeItem>";
        //    string xmlString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        //    ITcSmTreeItem routes = sysManager.LookupTreeItem("TIRR");
        //    routes.ConsumeXml(xmlString);
        //    //"result" will contain the XML information required to create the route
        //    string result = routes.ProduceXml();
        //    System.Windows.Forms.MessageBox.Show(result);

        //    //Extract required information from the Previous search results
        //    XmlDocument xmlDocument = new XmlDocument();
        //    xmlDocument.LoadXml(result);
        //    //xmlDocument.Load();
        //    //string amsNetId = xmlDocument.SelectSingleNode("//TreeItem/RoutePrj/ActualRoutes/Route/IpAddr[text()=\"" + textBox2.Text + "\"]/../NetId").InnerText;

        //    string remoteName = "no remote name found";
        //    //string amsNetId = "no remote AmsNetId Found";
        //    string IpAddr = "No IP Address Found";
        //    int i = 0;

        //    do
        //    {
        //        remoteName = xmlDocument.GetElementsByTagName("Name")[i].InnerText;
        //        if (remoteName.Contains(textBox6.Text))
        //        {
        //            amsNetId = xmlDocument.GetElementsByTagName("NetId")[i].InnerText;
        //            IpAddr = xmlDocument.GetElementsByTagName("IpAddr")[i].InnerText;
        //        }
        //        i++;

        //    } while ((amsNetId == "no remote AmsNetId Found") && (i < 10));

        //    if (amsNetId == "no remote AmsNetId Found") //set failed tickbox on 
        //    {
        //        checkBox8.Checked = true;
        //    }
        //    //System.Windows.Forms.MessageBox.Show(amsNetId);
        //    //string name = xmlDocument.SelectSingleNode("//TreeItem/RoutePrj/TargetList/Target/IpAddr[text()=\"" + textBox2.Text + "\"]/../Name").InnerText;

        //    //Create the route on the remote device
        //    xmlString = "<TreeItem><ItemName>Route Settings</ItemName><PathName>TIRR</PathName><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList><AddRoute><RemoteName>" + textBox5.Text + "</RemoteName><RemoteNetId>" + amsNetId + "</RemoteNetId><RemoteIpAddr>" + IpAddr + "</RemoteIpAddr><UserName>" + textBox3.Text + "</UserName><Password>" + textBox4.Text + "</Password><NoEncryption></NoEncryption></AddRoute></RoutePrj></TreeItem>";
        //    //xmlString = "< TreeItem >< ItemName > Route Settings </ ItemName >< PathName > TIRR </ PathName >< RoutePrj >< TargetList >< BroadcastSearch > true </ BroadcastSearch ></ TargetList >< AddRoute >< RemoteName >" + textBox5.Text + "</ RemoteName >< RemoteNetId >"+amsNetId+ "</ RemoteNetId >< RemoteIpAddr >" + textBox2.Text + "</ RemoteIpAddr >< UserName >" + textBox3.Text + "</ UserName >< Password >" + textBox4.Text + "</ Password >< NoEncryption ></ NoEncryption >< LocalName >"+localName+"</ LocalName ></ AddRoute ></ RoutePrj ></ TreeItem >";
        //    routes = sysManager.LookupTreeItem("TIRR");
        //    System.Windows.Forms.MessageBox.Show(remoteName);
        //    routes.ConsumeXml(xmlString);

        //    //Point o the TwinCAT runtime we are going to activate the configuration on
        //    sysManager2.SetTargetNetId(amsNetId);

        //}

        //public void AddRoute()
        //{
        //    string xmlString = "<TreeItem><RoutePrj><TargetList><Search>" + textBox2.Text + "</Search></TargetList></RoutePrj></TreeItem>";
        //    ITcSmTreeItem routes = sysManager.LookupTreeItem("TIRR");
        //    routes.ConsumeXml(xmlString);
        //    //"result" will contain the XML information required to create the route
        //    string result = routes.ProduceXml();
        //    //System.Windows.Forms.MessageBox.Show(result);
        //    //Extract required information from the Previous search results
        //    XmlDocument xmlDocument = new XmlDocument();
        //    xmlDocument.LoadXml(result);
        //    XmlNodeList nodeList;
        //    XmlNode root = xmlDocument.DocumentElement;
        //    nodeList = root.SelectNodes("descendant::TargetList[Target/IpAddr='" + textBox2.Text + "']");
        //    foreach (XmlNode Target in nodeList)
        //    {
        //        XmlNode example = Target.SelectSingleNode("Target");
        //        if (example != null)
        //        {
        //            amsNetId = example["NetId"].InnerText;
        //        }
        //    }
        //    if (amsNetId == "no remote AmsNetId Found") //set failed tickbox on 
        //    {
        //        checkBox8.Checked = true;
        //    }
        //    xmlString = "<TreeItem><ItemName>Route Settings</ItemName><PathName>TIRR</PathName><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList><AddRoute><RemoteName>" + textBox5.Text + "</RemoteName><RemoteNetId>" + amsNetId + "</RemoteNetId><RemoteIpAddr>" + textBox2.Text + "</RemoteIpAddr><UserName>" + textBox3.Text + "</UserName><Password>" + textBox4.Text + "</Password><NoEncryption></NoEncryption></AddRoute></RoutePrj></TreeItem>";
        //    //xmlString = "< TreeItem >< ItemName > Route Settings </ ItemName >< PathName > TIRR </ PathName >< RoutePrj >< TargetList >< BroadcastSearch > true </ BroadcastSearch ></ TargetList >< AddRoute >< RemoteName >" + textBox5.Text + "</ RemoteName >< RemoteNetId >"+amsNetId+ "</ RemoteNetId >< RemoteIpAddr >" + textBox2.Text + "</ RemoteIpAddr >< UserName >" + textBox3.Text + "</ UserName >< Password >" + textBox4.Text + "</ Password >< NoEncryption ></ NoEncryption >< LocalName >"+localName+"</ LocalName ></ AddRoute ></ RoutePrj ></ TreeItem >";
        //    routes = sysManager.LookupTreeItem("TIRR");
        //    routes.ConsumeXml(xmlString);

        //    //Point o the TwinCAT runtime we are going to activate the configuration on
        //    sysManager2.SetTargetNetId(amsNetId);
        //}

        #endregion
    }
}

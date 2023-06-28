using System;
using TCatSysManagerLib;
using System.Xml;

namespace Engine
{
    public interface IIOHandler
    {
        void ScanIO();
        void AddSyncUnit();
    }
    public class IOHandler : IIOHandler
    {
        #region Fields
        List<ITcSmTreeItem> _devices;
        List<String> _slaveDevicePaths = new List<string>();
        private ITcSysManager15 _sysMan;
        private ISystemManager _systemManager;
        #endregion
        #region Constructors
        public IOHandler()
        {

        }
        public IOHandler(ISystemManager systemManager)
        {
            this._sysMan = systemManager.SysMan;
        }
        #endregion
        #region Properties
        public List<string> SlaveDevicePaths
        {
            get { return _slaveDevicePaths; }
        }
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
        #region Methods
        public void ScanIO() //TODO - refactor this code
        {
            XmlDocument xmlDoc = new XmlDocument();
            int deviceCount = 0;
            _devices = new List<ITcSmTreeItem>();
            string xml;
            ITcSmTreeItem ioDevicesItem = _sysMan.LookupTreeItem("TIID");
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
                    _devices.Add(device);
                }
            }
            foreach (ITcSmTreeItem device in _devices)
            {
                xml = "<TreeItem><DeviceDef><ScanBoxes>1</ScanBoxes></DeviceDef></TreeItem>";
                try
                {
                    device.ConsumeXml(xml);
                }
                catch (Exception ex)
                {
                    
                }
                for (int i = 1; i < device.ChildCount; i++)
                {
                    _slaveDevicePaths.Add(device.Child[i].PathName);
                }
            }
        }
        public void Scan2()//TODO - check this against ScanIO
        {
            ITcSmTreeItem ioDevicesItem = _sysMan.LookupTreeItem("TIID");
            string scannedXml = ioDevicesItem.ProduceXml(false);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(scannedXml);
            XmlNodeList xmlDeviceList = xmlDoc.SelectNodes("TreeItem/DeviceGrpDef/FoundDevices/Device");
            List<ITcSmTreeItem> devices = new List<ITcSmTreeItem>();

            int deviceCount = 0;
            foreach (XmlNode node in xmlDeviceList)
            {
                int itemSubType = int.Parse(node.SelectSingleNode("ItemSubType").InnerText);
                string typeName = node.SelectSingleNode("ItemSubTypeName").InnerText;
                XmlNode xmlAddress = node.SelectSingleNode("AddressInfo");
                ITcSmTreeItem device = ioDevicesItem.CreateChild(string.Format("Device_{0}", ++deviceCount), itemSubType, string.Empty, null);
                string xml = string.Format("<TreeItem><DeviceDef>{0}</DeviceDef></TreeItem>", xmlAddress.OuterXml);
                device.ConsumeXml(xml);
                devices.Add(device);
            }
            foreach (ITcSmTreeItem device in devices)
            {
                string xml = "<TreeItem><DeviceDef><ScanBoxes>1</ScanBoxes></DeviceDef></TreeItem>";
                try
                {
                    device.ConsumeXml(xml);
                }
                catch (Exception ex)
                {
                    
                }
                //foreach (ITcSmTreeItem box in device)
                //{
                //    Console.WriteLine(box.Name);
                //}
            }
        }
        public void AddSyncUnit() //TODO check code function, pass string into function
        {
            ITcSmTreeItem ITrItm = _sysMan.LookupTreeItem("TIID^Device 1 (EtherCAT)^Term 1 (EK1100)^Term 4 (EL2008)");
            string xmlDescr = ITrItm.ProduceXml();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlDescr);
            XmlNode EtherCatSlave = xmlDoc.SelectSingleNode("//EtherCAT/Slave");

            XmlElement SyncUnit = xmlDoc.CreateElement("SyncUnits");
            SyncUnit.InnerXml = "<SyncUnit RepeatSupport = \"true\">meineSyncUnit</SyncUnit>";
            EtherCatSlave.AppendChild(SyncUnit);

            xmlDescr = xmlDoc.InnerXml;
            ITrItm.ConsumeXml(xmlDescr);

        }
        public void EditIoXml()//Todo pass reference to node for edits
        {
            ITcSmTreeItem _devices = _sysMan.LookupTreeItem("TIID");
            ITcSmTreeItem newEtherCatMaster = _devices.CreateChild("EtherCAT Master", 111, null, null);
            ITcSmTreeItem newEk1100 = newEtherCatMaster.CreateChild("EK1100", 9099, "", "EK1100-0000-0001");
            ITcSmTreeItem el1004 = newEk1100.CreateChild("EL1004", 9099, "", "EL1004-0000-0000");
            string xml = el1004.ProduceXml();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            // get element in xml by tag name
            XmlNode itemName = xmlDoc.GetElementsByTagName("ItemName")[0];
            // change inner text of tag
            if (itemName != null)
            {
                itemName.InnerText = "ChangedByAi";
            }
            el1004.ConsumeXml(xmlDoc.InnerXml);
        }
        public void CreateCanInterface(int NoOfMessages, int CobIdLength)
        {
            ITcSmTreeItem _devices = _sysMan.LookupTreeItem("TIID");
            ITcSmTreeItem CANMaster = _devices.CreateChild("CanDevice", 87, null, null);
            string templateDir;
            if (CobIdLength == 29)
            {
                templateDir = @"D:\11 Development\_0004_C# Automation Interface\Box 1 (29 bit CAN Interface).xti";
            }
            else if(CobIdLength ==11)
            {
                templateDir = @"D:\11 Development\_0004_C# Automation Interface\Box 1 (11 bit CAN Interface).xti";
            }
            else
            {
                templateDir = @"D:\11 Development\_0004_C# Automation Interface\Box 1 (CAN Interface).xti";
            }
            ManipulateCanInterfaceXti(templateDir, NoOfMessages);
            ITcSmTreeItem CanInterface = CANMaster.ImportChild(templateDir, "", true, "CAN Interface");
        }
        private void ManipulateCanInterfaceXti(string templateDir, int NoPdos)//todo can you get at GUIDs
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(templateDir);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("Name");
            var guids = new List<string>();
            foreach (XmlNode node in nodes)
            {
                if ((node.Attributes.Count != 0) & (node.InnerText.Contains(NoPdos.ToString())))
                {
                    guids.Add(node.Attributes["GUID"].Value);
                }
            }
            nodes = xmlDoc.GetElementsByTagName("Vars");
            foreach (XmlNode node in nodes)
            {
                XmlNode var = node.ChildNodes[1];
                if (var.ChildNodes[0].InnerText.Contains("Rx"))
                {
                    var.ChildNodes[1].Attributes["GUID"].Value = guids[0].ToString();
                }
                else
                {
                    if (NoPdos == 1)
                    {
                        var.ChildNodes[1].Attributes["GUID"].Value = guids[2].ToString();
                    }
                    else
                    {
                        var.ChildNodes[1].Attributes["GUID"].Value = guids[1].ToString();
                    }
                }
            }
            xmlDoc.Save(templateDir);
        }
        public ITcSmTreeItem CreateEcMaster(ITcSmTreeItem devices)  
        {
            return devices.CreateChild("EtherCAT Master", 111, null, null);
        }
        public ITcSmTreeItem CreateChildDevice(ITcSmTreeItem device, string name, int type)
        {
            return device.CreateChild(name, type, null, name);

        }
        public ITcSmTreeItem AddRtUdpModule(ITcSmTreeItem device, string name)
        {
            Dictionary<string, Guid> tcomModuleTable = new Dictionary<string, Guid>();
            tcomModuleTable.Add("TCP/UDP RT", Guid.Parse("{080D0399-6A65-408D-80E1-18D8F699496A}"));
            return device.CreateChild(name, 0, "", tcomModuleTable["TCP/UDP RT"]);
        }
        public void EnableADSOnEtherCATMaster(string master)
        {
            ITcSmTreeItem EcMaster = _sysMan.LookupTreeItem(master);
            string port = String.Format("<TreeItem><ImageDef><AdsServer><Port>{0}</Port></AdsServer></ImageDef></TreeItem>", "27908");
            string setADSSymbols = String.Format("<TreeItem><ImageDef><AdsServer><CreateSymbols>{0}</CreateSymbols></AdsServer></ImageDef></TreeItem>", "TRUE");
            EcMaster.ConsumeXml(port);
            EcMaster.ConsumeXml(setADSSymbols);
        }
        #endregion
    }
}

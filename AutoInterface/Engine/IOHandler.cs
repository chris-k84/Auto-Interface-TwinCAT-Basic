using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using System.Windows.Forms;
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
        ITcSysManager13 _sysMan;
        #endregion
        #region Constructors
        public IOHandler()
        {

        }
        public IOHandler(ITcSysManager13 sysManager)
        {
            _sysMan = sysManager;
        }
        #endregion

        #region Properties
        public List<string> SlaveDevicePaths
        {
            get { return _slaveDevicePaths; }
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
                    MessageBox.Show("Warning: {0}", ex.Message);
                }
                for (int i = 1; i < device.ChildCount; i++)
                {
                    _slaveDevicePaths.Add(device.Child[i].PathName);
                }
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
                templateDir = @"C:\Users\chrisk\Desktop\Box 1 (29 bit CAN Interface).xti";
            }
            else
            {
                templateDir = @"C:\Users\chrisk\Desktop\Box 1 (11 bit CAN Interface).xti";
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
            foreach(XmlNode node in nodes)
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
                    if (NoPdos ==1)
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
        #endregion
    }
}

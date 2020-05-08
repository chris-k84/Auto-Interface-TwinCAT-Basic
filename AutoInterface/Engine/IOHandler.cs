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
    public class IOHandler
    {
        #region Fields
        List<ITcSmTreeItem> _devices;
        ITcSysManager3 _sysMan;
        #endregion
        #region Constructors
        public IOHandler()
        {

        }
        public IOHandler(ITcSysManager3 sysManager)
        {
            _sysMan = sysManager;
        }
        #endregion

        #region Methods
        public void ScanIO() //TODO still working on this - add ams net id to the method call
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
                    //Console.WriteLine(device.Child[i].Name);
                    Console.WriteLine(device.Child[i].PathName);
                    //TODO: try pathName
                }
            }
            #endregion
        }
    }
}

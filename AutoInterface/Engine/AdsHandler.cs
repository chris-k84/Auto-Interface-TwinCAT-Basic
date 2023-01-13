using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TCatSysManagerLib;

namespace Engine
{
    public interface IAdsHandler
    {
        List<XmlNode> ScanADSDevices();
    }
    public class AdsHandler : IAdsHandler
    {
        #region Fields
        private ITcSysManager15 _sysMan { get; set; }
        private string _xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        private ISystemManager _systemManager;
        #endregion

        #region Properties
        public ITcSysManager15 SysManager 
        {
            get
            {
                return _sysMan;
            }
            set 
            {
                _sysMan = value;
            }
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

        #region Constructors
        public AdsHandler()
        { }
        public AdsHandler(ISystemManager systemManager)
        {
            this._sysMan = systemManager.SysMan;
        }
        #endregion

        #region Methods
        public List<XmlNode> ScanADSDevices()
        {
            List<XmlNode> routeXmls = new List<XmlNode>();
            XmlDocument _routeXml = new XmlDocument();
            try
            {
                string _routes;
                ITcSmTreeItem routes = _sysMan.LookupTreeItem("TIRR");
                routes.ConsumeXml(_xmlRouteString);
                _routes = routes.ProduceXml();
                _routeXml.LoadXml(_routes);

                XmlNodeList xmlDeviceList = _routeXml.SelectNodes("TreeItem/RoutePrj/TargetList/Target");
                foreach (XmlNode node in xmlDeviceList) //TODO handle choosing the right target
                {
                    //return node.SelectSingleNode("NetId").InnerText;
                    routeXmls.Add(node);
                }
            }
            catch (Exception e)
            {
                routeXmls = null;
            }
            return routeXmls;
        }
        public string CreateRouteString(XmlNode route)
        {
             
            string netid = route.SelectSingleNode("NetId").InnerText;
            string ipaddr = route.SelectSingleNode("IpAddr").InnerText;
            string remotename = route.SelectSingleNode("Name").InnerText;
            string username = "Administrator";
            string password = "1";
            string localname = "CHRISK-NB7";

            return string.Format("<TreeItem><ItemName>Route Settings</ItemName><PathName>TIRR</PathName><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList><AddRoute><RemoteName>{0}</RemoteName><RemoteNetId>{1}</RemoteNetId><RemoteIpAddr>{2}</RemoteIpAddr><UserName>{3}</UserName><Password>{4}</Password><NoEncryption></NoEncryption><LocalName>{5}</LocalName></AddRoute></RoutePrj></TreeItem>",
                                    remotename, netid, ipaddr, username, password, localname);
        }
        public void CreateRoute(string route)
        {
            ITcSmTreeItem routes = _sysMan.LookupTreeItem("TIRR");
            routes.ConsumeXml(route);
        }
        #endregion
    }
}

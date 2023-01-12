﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TCatSysManagerLib;

namespace Engine
{
    public interface IAdsHandler
    {
        List<XmlNode> ScanADSDevices();
    }
    public class AdsHandler : IAdsHandler
    {
        private ITcSysManager15 _sysMan { get; set; }
        public ITcSysManager15 SysManager15 { get; set; }

        private string _xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        string _routes;
        XmlDocument _routeXml = new XmlDocument();
        public AdsHandler(ITcSysManager15 sysman )
        {
            this._sysMan = sysman;
        }
        public List<XmlNode> ScanADSDevices()
        {
            List<XmlNode> routeXmls = new List<XmlNode>();
            try
            {
                
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
    }
}

using System;
using Engine;
using System.Xml;
using TCatSysManagerLib;
using System.Collections.Generic;
using System.Xml.Linq;

namespace QuickTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VisualStudioHandler newVisualStudio = new VisualStudioHandler();
            TwinCATHandler TcHandler;
            AdsHandler AdsHandler;
            newVisualStudio.InitialiseVSEnv();
            if (true)
            {
                Console.WriteLine("Started Creating......");

                newVisualStudio.CreateDirectory(@"C:\Users\ChrisK\Documents\TcXaeShell\TestProject");
                newVisualStudio.CreateSolution("Test");

                newVisualStudio.CreateTCProj("Test");

                Console.WriteLine("Finished Creating......");

                TcHandler = new TwinCATHandler(newVisualStudio.SysMan);
                AdsHandler = new AdsHandler(newVisualStudio);

                newVisualStudio.Save();
                Console.WriteLine("Saving......");
            }
            else
            {
                Console.WriteLine("LoadingProject");
                newVisualStudio.LoadTcProject(@"C:\Users\ChrisK\Documents\TcXaeShell\RandomShit\RandomShit.sln");

                TcHandler = new TwinCATHandler(newVisualStudio.SysMan);
                Console.WriteLine("Finished Loading");
            }

            ////////////Sectuion adding EtherCAT Master to project/////////
            //Console.WriteLine("Creating Ec MAster......");
            //ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            //IOHandler io = new IOHandler(newVisualStudio.SysMan);
            //ITcSmTreeItem Eth = io.CreateChildDevice(devices, "EtherNET", 109);
            //Console.WriteLine("Eth Master Ready......");

            /////////Section Adding RtUdp module to IO////////////////////
            //ITcSmTreeItem RT = io.AddRtUdpModule(Eth, "RTIO");
            //Console.WriteLine("RTIO is Ready......");
            //newVisualStudio.Save();

            //////////Section setting the RT settings for a project
            //ITcSmTreeItem RealTime = TcHandler.LookUpNode("TIRS");
            //XmlDocument xdoc = new XmlDocument();
            //xdoc.Load(@"D:\11 Development\_0004_C# Automation Interface\AutoInterface\QuickTest\bin\Debug\myTest.xml");
            //string RTSettings = xdoc.InnerXml;
            //TcHandler.DeployTreeItemXml(RealTime, RTSettings);


            /////////Section getting the XTI vs ItemXml of a device for comparison/////////////
            //ITcSmTreeItem EthMaster = TcHandler.LookUpNode("TIID^Device 1 (EtherCAT)");
            //xdoc.LoadXml(TcHandler.GetTreeItemXml(EthMaster));
            //xdoc.Save("myCheck.xml");
            //XmlDocument xdoc1 = new XmlDocument();
            //ITcSmTreeItem io = TcHandler.LookUpNode("TIID");
            //xdoc.LoadXml(TcHandler.GetTreeItemXml(io));
            //TcHandler.GetTreeItemXti(io, "Device 1 (EtherCAT)", @"D:\11 Development\_0004_C# Automation Interface\AutoInterface\QuickTest\bin\Debug\Test.txt");

            //////////////Checking Scan ADS function///////////////////
            //List<XmlNode> test =  AdsHandler.ScanADSDevices();
            //string route = (AdsHandler.CreateRouteString(test[0]));
            //AdsHandler.CreateRoute(route);
           
            Console.ReadLine();
        }
    }
}

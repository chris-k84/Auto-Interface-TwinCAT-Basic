using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using TCatSysManagerLib;

namespace QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            VisualStudioHandler newVisualStudio = new VisualStudioHandler();
            Console.WriteLine("Started Creating......");
            newVisualStudio.InitialiseVSEnv();
            newVisualStudio.CreateDirectory(@"C:\Users\ChrisK\Desktop\TestAI");
            newVisualStudio.CreateSolution("Test");
          
            newVisualStudio.CreateTCProj("Test");
          

            Console.WriteLine("Finished Creating......");

            TwinCATHandler TcHandler = new TwinCATHandler(newVisualStudio.SysMan);
            
            newVisualStudio.Save();
            Console.WriteLine("Saving......");
            
            Console.WriteLine("Creating Ec MAster......");
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");

            IOHandler io = new IOHandler(newVisualStudio.SysMan);

            ITcSmTreeItem Eth = io.CreateChildDevice(devices, "EtherNET", 109);
            Console.WriteLine("Eth Master Ready......");

            ITcSmTreeItem RT = io.AddRtUdpModule(Eth, "RTIO");
            Console.WriteLine("RTIO is Ready......");
            newVisualStudio.Save();

            ITcSmTreeItem RealTime = TcHandler.LookUpNode("TIRS");

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"D:\11 Development\_0004_C# Automation Interface\AutoInterface\QuickTest\bin\Debug\myTest.xml");
            string RTSettings = xdoc.InnerXml;

            TcHandler.DeployTreeItemXml(RealTime, RTSettings);



            
            xdoc.LoadXml(TcHandler.GetTreeItemXml(RealTime));
            xdoc.Save("myCheck.xml");
            newVisualStudio.Save();
            Console.ReadLine();
        }
    }
}

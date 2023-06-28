﻿using System;
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
            if (false)
            {
                Console.WriteLine("Started Creating......");
                newVisualStudio.SetEnvVisability(true, true);

                newVisualStudio.CreateDirectory(@"C:\Users\ChrisK\Documents\TcXaeShell\TestProject");
                newVisualStudio.CreateSolution("Test");

                newVisualStudio.CreateTCProj("Test");

                Console.WriteLine("Finished Creating......");

                TcHandler = new TwinCATHandler(newVisualStudio);
                AdsHandler = new AdsHandler(newVisualStudio);

                newVisualStudio.Save();
                Console.WriteLine("Saving......");
            }
            else
            {
                Console.WriteLine("LoadingProject");
                newVisualStudio.LoadTcProject(@"C:\Users\ChrisK\Documents\TcXaeShell\RandomTesting\RandomTesting.sln");

                TcHandler = new TwinCATHandler(newVisualStudio);
                Console.WriteLine("Finished Loading");
            }

            ////////////Creating PDOs on task with image//////////////////////
            //TcHandler.CreateTask("MyTask");
            //TcHandler.CreatePDOonTask("TIRT^MyTask^Inputs", "driver", "BOOL");

            ////////////Section adding EtherCAT Master to project/////////
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

            ///////////Section to retireive time context form node///////////////
            //ITcSmTreeItem RealTime = TcHandler.LookUpTcCOMNode();
            //TcHandler.LoadTcCOMToStore("SimulinkPositionControl", "{5479AD7B-8B2A-FCB2-6CB8-AE49C415C898}");
            //TcHandler.AddTcCOM("SimulinkPositionControl", RealTime);
            //TcHandler.AddTcCOM("SimulinkPositionControl", "{5479AD7B-8B2A-FCB2-6CB8-AE49C415C898}", RealTime);
            //TcHandler.CreateTask("NewTask", 12, 10000);
            //uint index = 0;
            //uint Oid = 33620000;
            //IList<ITcModuleInstance2> modules = TcHandler.GetModules(TcHandler.GetModuleManager());
            //TcHandler.SetModuleContext(index, Oid, modules[0]);
            //Tuple<uint, uint> module;
            //module = TcHandler.GetModuleContext(modules[0]);

            /////////Section getting the XTI vs ItemXml of a device for comparison/////////////
            //XmlDocument xdoc = new XmlDocument();
            //ITcSmTreeItem EthMaster = TcHandler.LookUpNode("TIRT^NewTask");
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

            //////////////Checking Produce/Consumne Update/////////////////////////////
            //XmlDocument firstCheck = new XmlDocument();
            //IOHandler io = new IOHandler(newVisualStudio);
            //io.CreateCanInterface(10, 9);
            //ITcSmTreeItem CAN = TcHandler.LookUpNode("TIID^CanDevice");
            //firstCheck.LoadXml(TcHandler.GetTreeItemXml(CAN));
            //string xml = string.Format("<TreeItem><DeviceDef><Fcxxxx><Baudrate>{0}</Baudrate></Fcxxxx></DeviceDef></TreeItem>", "CAN 10k");
            //CAN.ConsumeXml(xml);
            //XmlDocument secondCheck = new XmlDocument();
            //secondCheck.LoadXml(TcHandler.GetTreeItemXml(CAN));

            ///////////Adding a Existing PLC project////////////////////////
            //PLCHandler plc = new PLCHandler(newVisualStudio);
            //plc.AddPLCProj(@"D:\03 TwinCAT Functions\11 Labview\LabviewVIs\portableTrainingRig.tpzip", "Test");

            ///////////////Enable 2 Cores and Assign a Task to core 2////////////////
            //TcHandler.EnableSingleCoreForRT(4);
            //TcHandler.CreateTask("TestTask");
            //TcHandler.AssignCores("TestTask", 4);
            //TcHandler.SetIsolatedCores();

            /////////////////Checking addressing of added IO///////////////////
            //IOHandler io = new IOHandler(newVisualStudio);
            //ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            //ITcSmTreeItem ec = io.CreateEcMaster(devices);
            //ITcSmTreeItem ek1100 = io.CreateChildDevice(ec, "EK1100", 9099);
            //io.CreateChildDevice(ek1100, "EL1008", 9099);
            //io.CreateChildDevice(ek1100, "EL2008", 9099);

            /////////////////Set EL7021 into DMC mode/////////////////////////
            IOHandler io = new IOHandler(newVisualStudio);
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem ec = io.CreateEcMaster(devices);
            ITcSmTreeItem ek1100 = io.CreateChildDevice(ec, "EK1100", 9099);
            ITcSmTreeItem el7201 = io.CreateChildDevice(ek1100, "EL7201-0010", 9099);

            ///This DOES NOT WORK, TOO MANY CHANGES TO THE XML ARE MADE IN TC/////
            //XmlDocument TreeItem = new XmlDocument();
            //TreeItem.LoadXml(TcHandler.GetTreeItemXml(el7201));
            //var oldElem = TreeItem.SelectSingleNode("./TreeItem/EtherCAT/Slave/ProcessData");
            //XmlDocument SubXml = new XmlDocument();
            //SubXml.Load(@"D:\11 Development\_0004_C# Automation Interface\test.xml");
            //////Create a new title element.
            //XmlElement elem = TreeItem.CreateElement("AlternativeSmMappings");
            //elem.InnerXml = SubXml.FirstChild.InnerXml;
            ////////Replace the title element.
            //oldElem.ReplaceChild(elem, oldElem.LastChild);
            //TcHandler.DeployTreeItemXml(el7201, TreeItem.OuterXml);

            //XmlDocument real = new XmlDocument();
            //real.Load(@"D:\11 Development\_0004_C# Automation Interface\el7201_dmc.xml");
            //TcHandler.DeployTreeItemXml(el7201, real.OuterXml);

            
            Console.ReadLine();
        }
    }
}

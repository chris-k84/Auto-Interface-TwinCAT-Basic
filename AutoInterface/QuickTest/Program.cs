using System;
using Engine;
using System.Xml;
using TCatSysManagerLib;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QuickTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VisualStudioHandler newVisualStudio = new VisualStudioHandler();
            TwinCATHandler TcHandler;
            AdsHandler AdsHandler;
            IOHandler IOHandler;
            PLCHandler PLCHandler;
            CppHandler CppHandler;
            newVisualStudio.InitialiseVSEnv();
            if (true)
            {
                Console.WriteLine("Started Creating......");
                newVisualStudio.SetEnvVisability(true, true);

                newVisualStudio.CreateDirectory(@"C:\Users\ChrisK\Documents\TcXaeShell\Test");
                newVisualStudio.CreateSolution("Test");

                newVisualStudio.CreateTCProj("Test");

                Console.WriteLine("Finished Creating......");

                TcHandler = new TwinCATHandler(newVisualStudio);
                AdsHandler = new AdsHandler(newVisualStudio);
                IOHandler = new IOHandler(newVisualStudio);
                PLCHandler = new PLCHandler(newVisualStudio);
                CppHandler = new CppHandler(newVisualStudio);   

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

            //AddTaskDemo(TcHandler);

            //AddTerminalToEtherCATNetworkDemo(TcHandler, IOHandler);

            //AddRtuModuleDemo(TcHandler, IOHandler);

            //SetRTSettingsForProject(TcHandler);

            //FindTheTaskTimeDemo(TcHandler);

            //TcHandler.CreateLink(task.PathName + "^driver", el1008.PathName + "^Channel 1^Input");

            //SetRedundancyModeOnEtherCAT(TcHandler, IOHandler);

            //AdsBroadcastSearchandRouteDemo(AdsHandler);

            //SetEL7201IntoDMCModeDemo(TcHandler, IOHandler);

            //AddExistingPLCDemo(PLCHandler);

            //TreeItemProduceConsumeDemo(TcHandler, IOHandler);

            //TreeItemProduceConsumeDemo(TcHandler, IOHandler);

            //SetTaskCoreAssignmentDemo(TcHandler);

            //TestAddTcCOMAfterProjDemo(TcHandler);

            //AddingCppModulesReloadDemo(TcHandler, CppHandler);

            SettingConfigModeDemo(AdsHandler, newVisualStudio);



            Console.ReadLine();

        }

        static public void AddTaskDemo(ITwinCATHandler TcHandler)
        {
            ////////////Creating PDOs on task with image//////////////////////

            TcHandler.CreateTask("MyTask");
            ITcSmTreeItem task = TcHandler.CreatePDOonTask("TIRT^MyTask^Inputs", "driver", "BOOL");
            Console.WriteLine(task.PathName);
        }
        static public void AddTerminalToEtherCATNetworkDemo(ITwinCATHandler TcHandler, IIOHandler io)
        {
            ////////////Section adding EtherCAT Master to project/////////
            Console.WriteLine("Creating Ec MAster......");
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem Eth = io.CreateChildDevice(devices, "EtherCAT", 111);
            ITcSmTreeItem EK100 = Eth.CreateChild("EK1100", 9099, "", "EK1100-0000-0017");
            ITcSmTreeItem el1008 = EK100.CreateChild("Term 2 - EL1008", 9099, "", "EL1008-0000-0017");
            Console.WriteLine(el1008.PathName);
            Console.WriteLine("Eth Master Ready......");
        }
        static public void AddRtuModuleDemo(ITwinCATHandler TcHandler, IOHandler io)
        {
            /////////Section Adding RtUdp module to IO////////////////////
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem RT = io.AddRtUdpModule(devices, "RTIO");
        }
        static public void SetRTSettingsForProject(ITwinCATHandler TcHandler)
        {
            //////////Section setting the RT settings for a project
            ITcSmTreeItem RealTime = TcHandler.LookUpNode("TIRS");
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(@"D:\11 Development\_0004_C# Automation Interface\AutoInterface\QuickTest\bin\Debug\myTest.xml");
            string RTSettings = xdoc.InnerXml;
            TcHandler.DeployTreeItemXml(RealTime, RTSettings);
        }
        static public void FindTheTaskTimeDemo(ITwinCATHandler TcHandler)
        {
            ///////////Section to retireive time context form node///////////////
            ITcSmTreeItem RealTime = TcHandler.LookUpTcCOMNode();
            TcHandler.LoadTcCOMToStore("SimulinkPositionControl", "{5479AD7B-8B2A-FCB2-6CB8-AE49C415C898}");
            TcHandler.AddTcCOM("SimulinkPositionControl", RealTime);
            TcHandler.AddTcCOM("SimulinkPositionControl", "{5479AD7B-8B2A-FCB2-6CB8-AE49C415C898}", RealTime);
            TcHandler.CreateTask("NewTask", 12, 10000);
            uint index = 0;
            uint Oid = 33620000;
            IList<ITcModuleInstance2> modules = TcHandler.GetModules(TcHandler.GetModuleManager());
            TcHandler.SetModuleContext(index, Oid, modules[0]);
            Tuple<uint, uint> module;
            module = TcHandler.GetModuleContext(modules[0]);
        }
        static public void SetRedundancyModeOnEtherCATDemo(ITwinCATHandler TcHandler, IIOHandler io)
        {
            ////////////Set Ec Master into redundancy mode/////////////////////
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem Eth = io.CreateChildDevice(devices, "EtherCAT", 111);
            string master = TcHandler.GetTreeItemXml(Eth);
            string xml = string.Format("<Redundancy><Mode>2</Mode><PreviousPort Selected=\"true\"><Port>C</Port><PhysAddr>1019</PhysAddr></PreviousPort></Redundancy>");
            //string xml = string.Format("<Redundancy><Mode>2</Mode></Redundancy>");
            string redundancy = master.Insert(2156, xml);
            try
            {
                Eth.ConsumeXml(redundancy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string error = Eth.GetLastXmlError();
                Console.WriteLine(error);
            }
            XmlDocument SubXml = new XmlDocument();
            SubXml.Load(@"C:\Users\chrisk\Desktop\Ec.xml");

            try
            {
                Eth.ConsumeXml(SubXml.InnerText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string error = Eth.GetLastXmlError();
                Console.WriteLine(error);
            }
        } //Currently Unsupported Do Not Use
        static public void AdsBroadcastSearchandRouteDemo(IAdsHandler AdsHandler)
        {
            //////////////Checking Scan ADS function///////////////////
            List<XmlNode> test = AdsHandler.ScanADSDevices();
            string route = (AdsHandler.CreateRouteString(test[0]));
            AdsHandler.CreateRoute(route);
        }
        static public void SetEL7201IntoDMCModeDemo(ITwinCATHandler TcHandler, IIOHandler io)  // Do not try to edit XML, 100+ changes need to be made, use a template!
        {
            /////////////////Set EL7021 into DMC mode/////////////////////////
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem ec = io.CreateEcMaster(devices);
            ITcSmTreeItem ek1100 = io.CreateChildDevice(ec, "EK1100", 9099);
            ITcSmTreeItem el7201 = io.CreateChildDevice(ek1100, "EL7201-0010", 9099);
            XmlDocument real = new XmlDocument();
            real.Load(@"D:\11 Development\_0004_C# Automation Interface\el7201_dmc.xml");
            TcHandler.DeployTreeItemXml(el7201, real.OuterXml);
        }
        static public void AddExistingPLCDemo(IPLCHandler plc)
        {
            ///////////Adding a Existing PLC project////////////////////////
            plc.AddPLCProj(@"D:\03 TwinCAT Functions\11 Labview\LabviewVIs\portableTrainingRig.tpzip", "Test");
        }
        static public void TreeItemProduceConsumeDemo(ITwinCATHandler TcHandler, IIOHandler io)
        {
            //////////////Checking Produce/Consumne Update/////////////////////////////
            XmlDocument firstCheck = new XmlDocument();
            io.CreateCanInterface(10, 9);
            ITcSmTreeItem CAN = TcHandler.LookUpNode("TIID^CanDevice");
            firstCheck.LoadXml(TcHandler.GetTreeItemXml(CAN));
            string xml = string.Format("<TreeItem><DeviceDef><Fcxxxx><Baudrate>{0}</Baudrate></Fcxxxx></DeviceDef></TreeItem>", "CAN 10k");
            CAN.ConsumeXml(xml);
            XmlDocument secondCheck = new XmlDocument();
            secondCheck.LoadXml(TcHandler.GetTreeItemXml(CAN));
        }
        static public void TreeItemVsXtiExportDemo(ITwinCATHandler TcHandler, IIOHandler io)
        {
            /////////Section getting the XTI vs ItemXml of a device for comparison/////////////
            XmlDocument xdoc = new XmlDocument();
            ITcSmTreeItem devices = TcHandler.LookUpNode("TIID");
            ITcSmTreeItem EthMaster = io.CreateChildDevice(devices, "Device 1 (EtherCAT)", 111);
            xdoc.LoadXml(TcHandler.GetTreeItemXml(EthMaster));
            xdoc.Save("myCheck.xml");
            TcHandler.GetTreeItemXti(devices, "Device 1 (EtherCAT)", @"D:\11 Development\_0004_C# Automation Interface\AutoInterface\QuickTest\bin\Debug\Test.txt");
        }
        static public void SetTaskCoreAssignmentDemo(ITwinCATHandler TcHandler)
        {
            ///////////////Enable 2 Cores and Assign a Task to core 2////////////////
            TcHandler.EnableSingleCoreForRT(4);
            TcHandler.CreateTask("TestTask");
            TcHandler.AssignCores("TestTask", 4);
            TcHandler.SetIsolatedCores();
        }
        static public void TestAddTcCOMAfterProjDemo(ITwinCATHandler TcHandler)
        {
            ITcSmTreeItem RealTime = TcHandler.LookUpTcCOMNode();
            Console.WriteLine("Enter module name");
            string name = Console.ReadLine();
            Console.WriteLine("GUID");
            string guid = Console.ReadLine();
            RealTime.ConsumeXml("<TreeItem><ReloadTmc Path=\"C:\\Users\\chrisk\\Documents\\TcXaeShell\\Test\\Test\\NewCppProject\\NewCppProject.tmc\" > true</ReloadTmc></TreeItem>");
            TcHandler.AddTcCOM(name, guid, RealTime);
        }
        static public void AddingCppModulesReloadDemo(ITwinCATHandler TcHandler, ICppHandler cppHandler)
        {
            ITcSmTreeItem RealTime = TcHandler.LookUpTcCOMNode();
            cppHandler.AddCppTemplate();
            Console.WriteLine("Enter module name");
            string name = Console.ReadLine();
            Console.WriteLine("GUID");
            string guid = Console.ReadLine();
            TcHandler.AddTcCOM(name, guid, RealTime);
            Console.WriteLine("Enter module name");
            name = Console.ReadLine();
            Console.WriteLine("GUID");
            guid = Console.ReadLine();
            TcHandler.AddTcCOM(name, guid, RealTime);
        }
        static public void SettingConfigModeDemo(IAdsHandler adsHandler, IVisualStudioHandler vboHandler)
        {
            adsHandler.SetAMSNET("10.112.0.23.1.1");

            Console.ReadLine();
            vboHandler.SetTargetConfigMode();

            Console.ReadLine();
            vboHandler.RestartTwinCATInRunMode();
        }
    }
}


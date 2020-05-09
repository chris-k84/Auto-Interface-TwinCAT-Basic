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
            TwinCATHandler newTwinCAT;
            PLCHandler newPLC;
            IOHandler newIO;
            
            Console.WriteLine("Initialising environment.............");
            newVisualStudio.SetVSDevEnv();
            Console.WriteLine("Environment Ready");
            Console.WriteLine("Enter File Path");
            string pathway = Console.ReadLine();
            newVisualStudio.CreateDirectory(pathway);
            Console.WriteLine("Enter Solution Name");
            string solName = Console.ReadLine();    
            newVisualStudio.CreateSolution(solName);
            Console.WriteLine("Creating TwinCAT PRoject");
            newVisualStudio.CreateTCProj();
            newTwinCAT = new TwinCATHandler(newVisualStudio.SysMan);
            newIO = new IOHandler(newVisualStudio.SysMan);
            Console.WriteLine("Creating PLC Rroject");
            newPLC = new PLCHandler(newVisualStudio.SysMan);
            newPLC.CreatePLCProj("Testing");
            Console.WriteLine("Checking ADS routes");
            string route = newTwinCAT.ScanADSDevices();
            newTwinCAT.SetAMSNET(route);
            Console.WriteLine("Scanning IO........");
            newIO.ScanIO();

            //Console.WriteLine("Activating config");
            //newTwinCAT.ActivateSolution();
            Console.ReadLine();
        }
    }
}

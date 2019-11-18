using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            TwinCATAutoInt newClass = new TwinCATAutoInt();

            Console.ReadLine();

            

            newClass.SetVSDevEnv();

            Console.WriteLine("Enter File Path");
            string pathway = Console.ReadLine();
            
            newClass.CreateDirectory(pathway);

            Console.WriteLine("Enter Project Name");
            string solName = Console.ReadLine();

            newClass.CreateSolution(solName);

            Console.WriteLine("Create TC PRoj");
            newClass.CreateTCProj();

            //Console.WriteLine("Set AMS Net");
            //Console.ReadLine();

            newClass.SetAMSNET("5.52.64.65.1.1");
            //newClass.SetAMSNET("169.254.153.178.1.1");
            //newClass.SetAMSNET("5.67.156.222.1.1");

            Console.WriteLine("Scan IO");
            Console.ReadLine();

            newClass.ScanIO();
           


            //newClass.CreatePLCProj("PLC1");

            //Console.WriteLine("Scan for devices?");
            //Console.ReadLine();

            //string routes = newClass.ScanADSDevices();
            //XmlDocument xmlDocument = new XmlDocument();
            //xmlDocument.LoadXml(routes);
            //xmlDocument.Save(Console.Out);

            //Console.WriteLine("Add GVL");
            //Console.ReadLine();

            //newClass.AddGVL();

            //newClass.SaveAll();

            Console.ReadLine();
        }
    }
}

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

            Console.ReadLine();
            
            newVisualStudio.SetVSDevEnv();

            Console.WriteLine("Enter File Path");
            string pathway = Console.ReadLine();
            
            newVisualStudio.CreateDirectory(pathway);

            Console.WriteLine("Enter Project Name");
            string solName = Console.ReadLine();

            newVisualStudio.CreateSolution(solName);
            newVisualStudio.CreateTCProj();

            Console.WriteLine("creating TwinCAT handler");
            Console.ReadLine();

            newTwinCAT = new TwinCATHandler(newVisualStudio.SysMan);

            newTwinCAT.CreateTask("Bob", 23);

            Console.ReadLine();
        }
    }
}

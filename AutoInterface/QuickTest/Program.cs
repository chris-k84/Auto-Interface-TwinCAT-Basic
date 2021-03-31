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
            
            newVisualStudio.InitialiseVSEnv();
            newVisualStudio.CreateDirectory(@"C:\Users\ChrisK\Desktop\mike");
            newVisualStudio.CreateSolution("Test");

            newVisualStudio.CreateTCProj("Test");

            TwinCATHandler TcHandler = new TwinCATHandler(newVisualStudio.SysMan);

            newVisualStudio.Save();

            TcHandler.LoadTcCOM("test");
            TcHandler.AddTcCOM("walalalalalala", TcHandler.LookUpNode("test"));

            newVisualStudio.Save();

            Console.ReadLine();
        }
    }
}

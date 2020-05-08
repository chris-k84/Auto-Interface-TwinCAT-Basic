using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using System.IO;

namespace Engine
{
    class VisualStudioHandler
    {
        #region Fields
        EnvDTE.DTE dte;
        string _dirPath;
        dynamic solution;
        string _solName;
        dynamic tcProject;
        #endregion

        #region Methods
        public void SetVSDevEnv() //TODO : make able to use different VS versions
        {
            try
            {
                Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE");
                //Type VsVer = System.Type.GetTypeFromProgID("TcXaeShell.DTE.15.0"); //allows to set shell
                dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
                dte.SuppressUI = false;
                dte.MainWindow.Visible = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - Failed Environment Creation" + e.Message);
            }      
        }
        public void CreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Directory.CreateDirectory(path);
                _dirPath = path;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - Directory Path Creation - " + e.Message);
            }
        }
        public void CreateSolution(string name)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(_dirPath, name));
                solution = dte.Solution;
                solution.Create(_dirPath, name);
                _solName = name;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - Solution Creation - " + e.Message);
            }
        }
        public void SaveAll()
        {
            string slnName = _solName + ".sln";
            solution.SaveAs(Path.Combine(_dirPath, slnName));
            tcProject.Save();
        } //TODO save all save project and solution, need to check this
        #endregion
    }
}

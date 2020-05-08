using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using System.IO;
using TCatSysManagerLib;

namespace Engine
{
    public class VisualStudioHandler
    {
        #region Fields
        EnvDTE.DTE _dte;
        string _dirPath;
        dynamic _solution;
        string _solName;
        dynamic _tcProject;
        string _tcTemplate = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        ITcSysManager3 _sysMan;
        #endregion

        #region Properties
        public ITcSysManager3 SysMan 
        {
            get { return _sysMan; }
        }
        #endregion

        #region Methods
        public void SetVSDevEnv() //TODO : make able to use different VS versions
        {
            try
            {
                Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE");
                //Type VsVer = System.Type.GetTypeFromProgID("TcXaeShell.DTE.15.0"); //allows to set shell
                _dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
                _dte.SuppressUI = false;
                _dte.MainWindow.Visible = true;
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
                _solution = _dte.Solution;
                _solution.Create(_dirPath, name);
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
            _solution.SaveAs(Path.Combine(_dirPath, slnName));
            _tcProject.Save();
        } //TODO save all save project and solution, need to check this
        public void CreateTCProj()
        {
            try
            {
                _tcProject = _solution.AddFromTemplate(_tcTemplate, Path.Combine(_dirPath, _solName), _solName);
                _sysMan = _tcProject.Object;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - TC Project Creation - " + e.Message);
            }
        }
        #endregion
    }
}

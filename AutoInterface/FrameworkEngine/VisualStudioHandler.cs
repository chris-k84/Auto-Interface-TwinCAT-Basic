using System;
using EnvDTE;
using System.IO;
using TCatSysManagerLib;
using System.Collections.Generic;

namespace Engine
{
    public interface IVisualStudioHandler
    {
        void Save();
        void LoadTcProject(string solutionPath);
    }
    public interface ISystemManager
    {
        ITcSysManager8 SysMan { get; }
    }

    public class VisualStudioHandler : ISystemManager, IVisualStudioHandler
    {
        #region Fields
        private DTE _dte;
        private string _dirPath;
        private Solution _solution;
        private string _solName;
        private Project _tcProject;
        private string _tcTemplate = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        private ITcSysManager8 _sysMan;
        #endregion

        #region Properties
        public ITcSysManager8 SysMan
        {
            get { return _sysMan; }
        }
        #endregion

        #region Methods
        public void InitialiseVSEnv()
        {
                List<Type> vsVer = GetInstalledVersions();
                int count = vsVer.Count;
                _dte = (EnvDTE.DTE)System.Activator.CreateInstance(vsVer[count-1]);

        }
        public void SetEnvVisability(bool UI, bool mainVisible)
        {
            if (_dte != null)
            {
                _dte.SuppressUI = UI;
                _dte.MainWindow.Visible = mainVisible;
                _dte.UserControl = false;
            }
        }
        public void CreateDirectory(string path)
        {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Directory.CreateDirectory(path);
                _dirPath = path;
        }
        public void CreateSolution(string name)
        {
                Directory.CreateDirectory(Path.Combine(_dirPath, name));
                _solution = _dte.Solution;
                _solution.Create(_dirPath, name);
                _solName = name + ".sln"; 
        }
        public void Save()
        {
            if (_solution != null)
            {
                _solution.SaveAs(Path.Combine(_dirPath, _solName));
            }

            if (_tcProject != null)
            {
                _tcProject.Save();
            }
        }
        public void CreateTCProj(string name)
        {
            try
            {
                _tcProject = _solution.AddFromTemplate(_tcTemplate, Path.Combine(_dirPath, name), name);
                System.Threading.Thread.Sleep(2000);
                _sysMan = _tcProject.Object;
            }
            catch (Exception e)
            {
                
            }
        }
        public void LoadTcProject(string solutionPath)
        {
            _solution = _dte.Solution;
            _solution.Open(solutionPath);
            System.Threading.Thread.Sleep(2000);
            _tcProject = _solution.Projects.Item(1);
            System.Threading.Thread.Sleep(2000);
            _sysMan = _tcProject.Object;
        }
        private List<Type> GetInstalledVersions()
        {
            string[] vsVersions = { "11.0", "12.0", "14.0", "15.0", "16.0" };

            List<Type> installedVers = new List<Type>();

            for (int runs = 0; runs < vsVersions.Length; runs++)
            {
                Type vsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE." + vsVersions[runs]);
                if (vsVer != null)
                {
                    installedVers.Add(vsVer);
                }
            }
            if (installedVers.Count == 0)
            {
                Type vsVer = System.Type.GetTypeFromProgID("TcXaeShell.DTE.15.0");
                if (vsVer != null)
                {
                    installedVers.Add(vsVer);
                }
            }

            return installedVers;
            //Type VsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE.11.0"); //VS2012
            //Type VsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE.12.0"); //VS2013
            //Type VsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE.14.0"); //VS2015
            //Type VsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE.15.0"); //VS2017
            //Type VsVer = System.Type.GetTypeFromProgID("VisualStudio.DTE.16.0"); //VS2019
            //Type VsVer = System.Type.GetTypeFromProgID("TcXaeShell.DTE.15.0"); //allows to set shell
        }
        #endregion
    }
}

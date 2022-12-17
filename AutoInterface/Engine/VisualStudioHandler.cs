using System;
using EnvDTE;
using System.IO;
using TCatSysManagerLib;

namespace Engine
{
    public interface IVisualStudioHandler
    {
        void SetVSDevEnv();
        void CreateDirectory(string path);
        void CreateSolution(string name);
        void SaveAll();
        void CreateTCProj();
    }
    public class VisualStudioHandler
    {
        #region Fields
        DTE _dte;
        string _dirPath;
        Solution _solution;
        string _solName;
        Project _tcProject;
        string _tcTemplate = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        ITcSysManager13 _sysMan;
        #endregion

        #region Properties
        public ITcSysManager13 SysMan
        {
            get { return _sysMan; }
        }
        #endregion

        #region Methods
        public void InitialiseVSEnv()
        {
            try
            {
                List<Type> vsVer = GetInstalledVersions();
                _dte = (EnvDTE.DTE)System.Activator.CreateInstance(vsVer.Last());
            }
            catch (Exception e)
            {
               
            }
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
               
            }
        }
        public void CreateSolution(string name)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(_dirPath, name));
                _solution = _dte.Solution;
                _solution.Create(_dirPath, name);
                _solName = name + ".sln"; ;
            }
            catch (Exception e)
            {
               
            }
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
        List<Type> GetInstalledVersions()
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

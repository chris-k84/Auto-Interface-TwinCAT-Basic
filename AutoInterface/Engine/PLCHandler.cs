using System;
using TCatSysManagerLib;

namespace Engine
{
    public interface IPLCHandler
    {
        void CreatePLCProj(string name);
        void AddPLCProj(string pathToProjectFile, string name);
        void CreateGVL();
        void AddDeclarationToGVL(List<string> slaves);
    }
    public class PLCHandler : IPLCHandler
    {
        #region Fields
        private ITcSysManager15 _sysMan;
        ITcSmTreeItem _plcProj;
        ITcSmTreeItem _gvlFolder;
        ITcSmTreeItem _gvlList;
        ITcPlcDeclaration _itcGvlList;
        #endregion
        #region Properties
        #endregion
        #region Constructor
        public PLCHandler()
        { }
        public PLCHandler(ISystemManager systemManager)
        {
            this._sysMan = systemManager.SysMan;
        }
        #endregion
        #region Methods
        public void CreatePLCProj(string name)
        {
            try
            {
                ITcSmTreeItem plc = _sysMan.LookupTreeItem("TIPC");
                _plcProj = plc.CreateChild(name, 0, "", $@"Standard PLC Template.plcproj");
            }
            catch (Exception e)
            {
                
            }
        }
        public void AddPLCProj(string pathToProjectFile, string name)
        {
            try
            {
                ITcSmTreeItem plc = _sysMan.LookupTreeItem("TIPC");
                plc.CreateChild(name, 0, "", pathToProjectFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void CreateGVL()
        {
            try
            {
                _gvlFolder = _sysMan.LookupTreeItem("TIPC^" + _plcProj.Name + "^" + _plcProj.Name + " Project^GVLs");
                _gvlList = _gvlFolder.CreateChild("Global_IO", 615, "", IECLANGUAGETYPES.IECLANGUAGE_ST);
                _itcGvlList = (ITcPlcDeclaration)_gvlList;
            }
            catch (Exception e)
            {
                
            }

        }
        public void AddDeclarationToGVL(List<string> slaves)
        {
            try
            {
                foreach (string slave in slaves)
                {
                    _itcGvlList.DeclarationText = slave;
                }
            }
            catch (Exception e)
            {
                
            }
        }
        public void AutoStartBoot(string PlcName)
        {
            ITcSmTreeItem plcProjectRoot = _sysMan.LookupTreeItem(string.Format("TIPC^{0}",PlcName));
            ITcPlcProject plcProjectRootIec = (ITcPlcProject)plcProjectRoot;
            plcProjectRootIec.BootProjectAutostart = true;
            plcProjectRootIec.GenerateBootProject(true);
        }
        #endregion
    }
}

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
        private ISystemManager _systemManager;
        ITcSmTreeItem _plcProj;
        ITcSmTreeItem _gvlFolder;
        ITcSmTreeItem _gvlList;
        ITcPlcDeclaration _itcGvlList;
        #endregion
        #region Properties
        public ISystemManager systemManager
        {
            get
            {
                return _systemManager;
            }
            set
            {
                _systemManager = value;
                _sysMan = value.SysMan;
            }
        }
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
                ITcSmTreeItem newProject = plc.CreateChild(name, 0, "", pathToProjectFile);
            }
            catch (Exception e)
            {
                
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
        #endregion
    }
}

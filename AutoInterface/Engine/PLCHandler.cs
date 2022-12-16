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
        ITcSysManager13 _sysMan;
        ITcSmTreeItem _plcProj;
        ITcSmTreeItem _gvlFolder;
        ITcSmTreeItem _gvlList;
        ITcPlcDeclaration _itcGvlList;
        #endregion
        #region Constructor
        public PLCHandler()
        { }
        public PLCHandler(ITcSysManager13 sysManager)
        {
            _sysMan = sysManager;
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
                MessageBox.Show("Process Error - PLC Creation - " + e.Message);
            }
        }
        public void AddPLCProj(string pathToProjectFile, string name)
        {
            try
            {
                ITcSmTreeItem plc = _sysMan.LookupTreeItem("TIPC");
                ITcSmTreeItem newProject = plc.CreateChild("NameOfProject", 0, "", pathToProjectFile);
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - PLC project failed to add - " + e.Message);
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
                MessageBox.Show("Process Error - GVL Creation" + e.Message);
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
                MessageBox.Show("Process Error - Writing GVL failed - " + e.Message);
            }
        }
        #endregion
    }
}

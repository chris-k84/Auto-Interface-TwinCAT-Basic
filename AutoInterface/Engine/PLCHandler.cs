using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using System.Windows.Forms;

namespace Engine
{
    class PLCHandler
    {
        #region Fields
        ITcSysManager3 sysMan;
        ITcSmTreeItem _plcProj;
        ITcSmTreeItem _gvlFolder;
        ITcSmTreeItem _gvlList;
        ITcPlcDeclaration _itcGvlList;
        #endregion
        #region Methods
        public void CreatePLCProj(string name)
        {
            try
            {
                ITcSmTreeItem plc = sysMan.LookupTreeItem("TIPC");
                _plcProj = plc.CreateChild(name, 0, "", $@"Standard PLC Template.plcproj");
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - PLC Creation - " + e.Message);
            }
        }
        public void AddGVL() //TODO extend to return the iterface to the GVL declaration area
        {
            try
            {
                _gvlFolder = sysMan.LookupTreeItem("TIPC^" + _plcProj.Name + "^" + _plcProj.Name + " Project^GVLs");
                _gvlList = _gvlFolder.CreateChild("Global_IO", 615, "", IECLANGUAGETYPES.IECLANGUAGE_ST);
                _itcGvlList = (ITcPlcDeclaration)_gvlList;
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - GVL Creation" + e.Message);
            }

        }
        #endregion
    }
}

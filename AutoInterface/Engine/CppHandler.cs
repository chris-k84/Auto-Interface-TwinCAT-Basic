using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TCatSysManagerLib;

namespace Engine
{
    public interface ICppHandler
    {
        public void AddCppTemplate();
    }
    public class CppHandler : ICppHandler
    {
        #region Fields
        private ITcSysManager15 _sysMan { get; set; }
        private ISystemManager _systemManager;
        string pathToProjectTemplateFile = "TcVersionedDriverWizard";
        string pathToModuleTemplateFile = "TcModuleCyclicCallerWizard";

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
        #region Constructors
        public CppHandler()
        {
            
        }
        public CppHandler(ISystemManager systemManager)
        {
            this._sysMan = systemManager.SysMan;
        }
        #endregion
        #region Methods
        public void AddCppTemplate()
        {
            ITcSmTreeItem cpp = _sysMan.LookupTreeItem("TIXC");
            ITcSmTreeItem cppProject = cpp.CreateChild("NewCppProject", 0, "", pathToProjectTemplateFile);
            ITcSmTreeItem cppModule = cppProject.CreateChild("NewModule", 1, "", pathToModuleTemplateFile);
        }
        #endregion
        
    }
}

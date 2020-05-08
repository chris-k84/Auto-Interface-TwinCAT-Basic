﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using System.IO;
using System.Windows.Forms;

namespace Engine
{
    public class TwinCATHandler
    {
        #region Fields
        ITcSysManager3 _sysMan;
        string _xmlRouteString = "<TreeItem><RoutePrj><TargetList><BroadcastSearch>true</BroadcastSearch></TargetList></RoutePrj></TreeItem>";
        string _routes;
        #endregion

        #region Constructors
        public TwinCATHandler()
        {

        }
        public TwinCATHandler(ITcSysManager3 sysManager)
        {
            _sysMan = sysManager;
        }
        #endregion

        #region Methods
        public void CreateTask(string taskName)
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }
        public void CreateTask(string taskName, int taskPriority)
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
                string xmlPriority = String.Format("<TreeItem><TaskDef><Priority>{0}</Priority></TaskDef></TreeItem>", taskPriority.ToString());
                tTask.ConsumeXml(xmlPriority);
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }
        public void CreateTask(string taskName, int taskPriority, int taskCycleTime) //TODO add reference to task to main class
        {
            try
            {
                ITcSmTreeItem tasks = _sysMan.LookupTreeItem("TIRT");
                ITcSmTreeItem tTask = tasks.CreateChild(taskName, 0, null, null);
                string xmlPriority = String.Format("<TreeItem><TaskDef><Priority>{0}</Priority></TaskDef></TreeItem>", taskPriority.ToString());
                string xmlCycle = String.Format("<TreeItem><TaskDef><CycleTime>{0}</CycleTime></TaskDef></TreeItem>", taskCycleTime.ToString());
                tTask.ConsumeXml(xmlPriority);
                tTask.ConsumeXml(xmlCycle); //defined in 100s of ns so 10000 = 1ms
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }
        public void AssignCores() //TODO add code for this function
        {

        }
        public void SetAMSNET(string amsNetId)
        {
            try
            {
                _sysMan.SetTargetNetId(amsNetId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public string ScanADSDevices() //TODO to return pointer to xml
        {
            try
            {
                ITcSmTreeItem routes = _sysMan.LookupTreeItem("TIRR");
                routes.ConsumeXml(_xmlRouteString);
                _routes = routes.ProduceXml();
            }
            catch (Exception e)
            {
                MessageBox.Show("Process Error - PLC Creation - " + e.Message);
                _routes = "";
            }
            return _routes;
        }
        public void CreateLink(string source, string destination)
        {
            try
            {
                _sysMan.LinkVariables(source, destination);
            }
            catch (Exception e)
            {
                MessageBox.Show("Process error - Link error" + e.Message);
            }
        }
        public void GetMappings()
        {
            try
            {
                string mappingInfo = _sysMan.ProduceMappingInfo();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void LoadMappings(string mappingInfo)
        {
            try
            {
                _sysMan.ConsumeMappingInfo(mappingInfo);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
    }
}

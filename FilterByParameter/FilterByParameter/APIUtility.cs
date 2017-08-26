using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.UI;

namespace FilterByParameter
{
    public partial class APIUtility
    {
        /// <summary>
        /// Store a reference to the application.
        /// </summary>
        public void Initialize(UIApplication uiApplication)
        {
            m_uiApplication = uiApplication;
        }

        /// <summary>
        /// Get the current Modeless command.
        /// </summary>
        public ModelessCommand ModelessCommand { get { return m_ModelessCommand; } }

        public void RunModelessCommand(ModelessCommandData command)
        {
            switch (command.CommandType)
            {
                case ModelessCommandType.PrintMainPageStatistics:
                {
                    command.WindowSummaryData = GetPaneSummary(Ribbon.thisApp.MainPageDockablePaneId);
                    ModelessCommand.Make(command);
                    break;
                }

                case ModelessCommandType.PrintSelectedPageStatistics:
                {
                    command.WindowSummaryData = GetPaneSummary(command.SelectedPaneId);
                    ModelessCommand.Make(command);
                    break;
                }

                case ModelessCommandType.gridSelect:
                {
                    UIDocument uidoc = m_uiApplication.ActiveUIDocument;
                    GridDistance gd = new GridDistance();
                    gd.Execute(uidoc);
                    ModelessCommand.Make(command);
                    break;
                }
                case ModelessCommandType.beamSelect:
                {
                    UIDocument uidoc = m_uiApplication.ActiveUIDocument;
                    BeamSelect bm = new BeamSelect();
                    bm.Execute(uidoc);
                    ModelessCommand.Make(command);
                    break;
                }


                default:
                    break;
            }
        }

        /// <summary>
        /// Return dockable pane inforamtion, given a dockable pane Guid.
        /// </summary>
        public string GetPaneSummary(string paneGuidString)
        {
            Guid paneGuid;
            try
            {
                paneGuid = new Guid(paneGuidString);
            }
            catch (Exception)
            {
                return "Invalid Guid";
            }
            DockablePaneId paneId = new DockablePaneId(paneGuid);
            return GetPaneSummary(paneId);
        }


        /// <summary>
        /// Return dockable pane inforamtion, given a DockablePaneId
        /// </summary>
        public string GetPaneSummary(DockablePaneId id)
        {
            DockablePane pane = null;
            try
            {
                pane = m_uiApplication.GetDockablePane(id);
                return GetPaneSummary(pane);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Return dockable pane inforamtion, given a DockablePaneId
        /// </summary>
        public static string GetPaneSummary(DockablePane pane)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("-RevitDockablePane- Title: " + pane.GetTitle() + ", Id-Guid: " + pane.Id.Guid.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Display docking state information as a string.
        /// </summary>
        public static string GetDockStateSummary(DockablePaneState paneState)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(" -DockablePaneState-");
            sb.AppendLine(" Left: " + paneState.FloatingRectangle.Left.ToString());
            sb.AppendLine(" Right: " + paneState.FloatingRectangle.Right.ToString());
            sb.AppendLine(" Top: " + paneState.FloatingRectangle.Top.ToString());
            sb.AppendLine(" Bottom: " + paneState.FloatingRectangle.Bottom.ToString());
            sb.AppendLine(" Position: " + paneState.DockPosition.ToString());
            sb.AppendLine(" Tab target guid:" + paneState.TabBehind.Guid.ToString());
            return sb.ToString();
        }

        #region Data
        private Autodesk.Revit.UI.UIApplication m_uiApplication;
        private ModelessCommand m_ModelessCommand = new ModelessCommand();
        #endregion

    }

    public class ModelessCommand
    {

        /// <summary>
        /// Set data into the command.
        /// </summary>
        public void Make(ModelessCommandData commandData)
        {
            lock (this)
            {
                m_data = commandData;
            }
        }

        /// <summary>
        /// Get data from the command.
        /// </summary>
        public ModelessCommandData Take()
        {
            lock (this)
            {
                return m_data;
            }

        }
        private ModelessCommandData m_data = new ModelessCommandData();

    }
    public enum ModelessCommandType : int
    {
        PrintMainPageStatistics,
        PrintSelectedPageStatistics,
        gridSelect,
        beamSelect,
        Return
    }

    public class ModelessCommandData
    {
        public ModelessCommandData() { }
        public ModelessCommandType CommandType;
        public string WindowSummaryData;
        public string SelectedPaneId;
        public string gridSelect;
        public string beamSelect;
    }

    public class Log
    {
        public static void Message(string message, int level = 0)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            if (level > 0)
                TaskDialog.Show("Revit", message);
        }
    }

    class Globals
    {

        public const string ApplicationName = "DockableDialogs";
        public const string DiagnosticsTabName = "DockableDialogs";
        public const string DiagnosticsPanelName = "DockableDialogs Panel";

        public const string RegisterPage = "Register Page";
        public const string ShowPage = "Show Page";
        public const string HidePage = "Hide Page";


        public static DockablePaneId sm_UserDockablePaneId = new DockablePaneId(new Guid("{3BAFCF52-AC5C-4CF8-B1CB-D0B1D0E90237}"));

    }


    /// <summary>
    /// A simple utility class to route calls from Console.WriteLine and other standard IO to 
    /// a TextBox.  Note that one side effect of this system is that any time a host application calls
    /// Console.WriteLine, cout, printf, or something similar, the output will be funneled through here,
    /// giving occasional output you may not have expected.
    /// </summary>
    public class StandardIORouter : TextWriter
    {

        /// <summary>
        /// Create a new router given a WPF Textbox to output to.
        /// </summary>
        public StandardIORouter(System.Windows.Controls.TextBox output)
        {
            m_outputTextBox = output;
        }

        /// <summary>
        /// Write a character from standardIO to a Textbox.
        /// </summary>
        public override void Write(char oneCharacter)
        {
            m_outputTextBox.AppendText(oneCharacter.ToString());
            m_outputTextBox.ScrollToEnd();
        }

        /// <summary>
        /// A default override to use UTF8 text
        /// </summary>
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        /// <summary>
        /// A stored reference of a textbox to output to.
        /// </summary>
        private System.Windows.Controls.TextBox m_outputTextBox = null;
    }
}

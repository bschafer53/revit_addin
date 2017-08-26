using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace FilterByParameter
{
    /// <summary>
    /// Interaction logic for DockWindow.xaml
    /// </summary>
    public partial class DockWindow : Page, Autodesk.Revit.UI.IDockablePaneProvider
    {
        public DockWindow()
        {
            InitializeComponent();
            m_exEvent = Autodesk.Revit.UI.ExternalEvent.Create(m_handler);
            //m_textWriter = new StandardIORouter("gridSelect");
            //Console.SetOut(m_textWriter); //Send all standard output to the text rerouter.
        }

        UIApplication Application
        {
            set { m_application = value; }
        }

        #region UI State

        private void DozeOff()
        {
            EnableCommands(false);
        }

        private void EnableCommands(bool status)
        {
            if (status == false)
                this.Cursor = Cursors.Wait;
            else
                this.Cursor = Cursors.Arrow;
        }

        public void WakeUp()
        {
            EnableCommands(true);
        }

        #endregion

        #region UI Support

        public void UpdateUI(ModelessCommandData data)
        {
            switch (data.CommandType)
            {
                case (ModelessCommandType.PrintMainPageStatistics):
                {
                    Log.Message("***Main Pane***");
                    Log.Message(data.WindowSummaryData);
                    break;
                }

                case (ModelessCommandType.PrintSelectedPageStatistics):
                {
                    Log.Message("***Selected Pane***");
                    Log.Message(data.WindowSummaryData);
                    break;
                }
                case (ModelessCommandType.gridSelect):
                {
                    HGrid.Content = GridDistance.HGrid;
                    VGrid.Content = GridDistance.VGrid;
                    
                    break;
                }
                case (ModelessCommandType.beamSelect):
                {
                    BMark.Content= BeamSelect.BMark;
                    XDist.Content = BeamSelect.stX1;
                    YDist.Content = BeamSelect.stY1;
                    XDist2.Content = BeamSelect.edX1;
                    YDist2.Content = BeamSelect.edY1;
                    CMark.Content = BeamSelect.CMark;
                    XCol.Content = BeamSelect.clX1;
                    YCol.Content = BeamSelect.clY1;

                    break;
                }

                default:
                    break;
            }
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem) return (childItem) child;
                else
                {

                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null) return childOfChild;
                }
            }

            return null;
        }

        #endregion

        #region UI Support

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
        }


        #endregion

        #region Data

        private Autodesk.Revit.UI.UIApplication m_application;
        private Autodesk.Revit.UI.ExternalEvent m_exEvent;
        private APIExternalEventHandler m_handler = new APIExternalEventHandler();
        //private System.IO.TextWriter m_textWriter; //Used to re-route any standard IO to the WPF UI.


        #endregion

        /// <summary>
        /// Called by Revit to initialize dockable pane settings set in DockingSetupDialog.
        /// </summary>
        /// <param name="data"></param>
        public void SetupDockablePane(Autodesk.Revit.UI.DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            DockablePaneProviderData d = new DockablePaneProviderData();


            data.InitialState = new Autodesk.Revit.UI.DockablePaneState();
            data.InitialState.DockPosition = m_position;
            DockablePaneId targetPane;
            if (m_targetGuid == Guid.Empty)
                targetPane = null;
            else targetPane = new DockablePaneId(m_targetGuid);
            if (m_position == DockPosition.Tabbed)
                data.InitialState.TabBehind = targetPane;


            if (m_position == DockPosition.Floating)
            {
                data.InitialState.SetFloatingRectangle(
                    new Autodesk.Revit.DB.Rectangle(m_left, m_top, m_right, m_bottom));
            }

            Log.Message("***Intial docking parameters***");
            Log.Message(APIUtility.GetDockStateSummary(data.InitialState));

        }

        private void PaneInfoButton_Click(object sender, RoutedEventArgs e)
        {
            RaisePrintSummaryCommand();
        }


        private void RaisePrintSummaryCommand()
        {
            ModelessCommandData data = new ModelessCommandData();
            data.CommandType = ModelessCommandType.PrintMainPageStatistics;
            Ribbon.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            m_exEvent.Raise();
        }

        private void RaisePrintSpecificSummaryCommand(string guid)
        {
            ModelessCommandData data = new ModelessCommandData();
            data.CommandType = ModelessCommandType.PrintSelectedPageStatistics;
            data.SelectedPaneId = guid;
            Ribbon.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            m_exEvent.Raise();
        }


        public string GetPageWpfData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-WFP Page Info-");
            sb.AppendLine("FrameWorkElement.Width=" + this.Width);
            sb.AppendLine("FrameWorkElement.Height=" + this.Height);

            return sb.ToString();
        }

        public void SetInitialDockingParameters(int left, int right, int top, int bottom, DockPosition position,
            Guid targetGuid)
        {
            m_position = position;
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
            m_targetGuid = targetGuid;
        }

        #region Data

        private Guid m_targetGuid;
        private DockPosition m_position = DockPosition.Bottom;
        private int m_left = 1;
        private int m_right = 1;
        private int m_top = 1;
        private int m_bottom = 1;

        #endregion

        private void select_grids_Click(object sender, RoutedEventArgs e)
        {
 
            ModelessCommandData data = new ModelessCommandData();
            data.CommandType = ModelessCommandType.gridSelect;
            Ribbon.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            m_exEvent.Raise();


            //Log.Message("***Main Pane WPF info***");
            //Log.Message(Ribbon.thisApp.GetMainWindow().GetPageWpfData());
        }

        private void beamSelect_Click(object sender, RoutedEventArgs e)
        {
            ModelessCommandData data = new ModelessCommandData();
            data.CommandType = ModelessCommandType.beamSelect;
            Ribbon.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            m_exEvent.Raise();
        }
    }
}

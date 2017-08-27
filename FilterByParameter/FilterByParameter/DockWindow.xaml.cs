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
 
                case (ModelessCommandType.gridSelect):
                {
                    HGrid.Content = GridDistance.HGrid;
                    VGrid.Content = GridDistance.VGrid;
                    
                    break;
                }
                case (ModelessCommandType.beamSelect):
                {
                    Mark.Content = BeamSelect.Mark;
                    StartX.Content = BeamSelect.StartX;
                    StartY.Content = BeamSelect.StartY;
                    EndX.Content = BeamSelect.EndX;
                    EndY.Content = BeamSelect.EndY;

                    BMark.Text = BeamSelect.BMark;
                    StX.Text = BeamSelect.stX1;
                    StY.Text = BeamSelect.stY1;
                    EdX.Text = BeamSelect.edX1;
                    EdY.Text = BeamSelect.edY1;


                    break;
                }

                default:
                    break;
            }
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

        public void SetInitialDockingParameters( DockPosition position)
        {
            m_position = position;
            m_left = 10;
            m_right = 710;
            m_top = 10;
            m_bottom = 710;

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

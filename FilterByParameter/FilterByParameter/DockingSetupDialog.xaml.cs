using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FilterByParameter
{
    /// <summary>
    /// Interaction logic for DockingSetupDialog.xaml
    /// </summary>
    public partial class DockingSetupDialog : Window
    {
        public DockingSetupDialog()
        {
            InitializeComponent();
            this.tb_newGuid.Text = Globals.sm_UserDockablePaneId.Guid.ToString();
        }

        /// <summary>
        /// Take user-input data for docking dialog choices and attempt to parse it
        /// into higher-level data for later use.
        /// </summary>
        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(this.tb_newGuid.Text))
                m_mainPageGuid = this.tb_newGuid.Text;

          m_dockPosition = Autodesk.Revit.UI.DockPosition.Floating;

            this.DialogResult = true;
            this.Close();
        }

        private string m_mainPageGuid;
        private Autodesk.Revit.UI.DockPosition m_dockPosition;

        /// <summary>
        /// The guid of the main docking page.
        /// </summary>
        public Guid MainPageGuid
        {
            get
            {
                Guid retval = Guid.Empty;
                if (m_mainPageGuid == "null")
                    return retval;
                else
                {

                    try
                    {
                        retval = new Guid(m_mainPageGuid);

                    }
                    catch (Exception)
                    {
                    }
                    return retval;
                }
            }
        }


        public Autodesk.Revit.UI.DockPosition DockPosition { get { return m_dockPosition; } }

    }
}

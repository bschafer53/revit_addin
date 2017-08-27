using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Settings = FilterByParameter.Properties.Settings;

namespace FilterByParameter
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Ribbon : IExternalApplication
    {
        // ExternalCommands assembly path
        static string AddInPath = typeof(Ribbon).Assembly.Location;
        // Button icons directory
        static string ButtonIconsFolder = Path.GetDirectoryName(AddInPath);

        public string uname { get; set; }
        public string tbox { get; set; }
        public Document doc { get; set; }
        public RadioButtonGroup rbGroup;

        public string UserName()
        {
            string myVariable = uname;
            return myVariable;
        }

        public void RegisterDockableWindow(UIApplication application, Guid mainPageGuid)
        {
            Globals.sm_UserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(Globals.sm_UserDockablePaneId, "Distance from Reference Grid", Ribbon.thisApp.GetMainWindow() as IDockablePaneProvider);
        }

        /// <summary>
        /// Register a dockable Window
        /// </summary>
        public void RegisterDockableWindow(UIControlledApplication application, Guid mainPageGuid)
        {
            Globals.sm_UserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(Globals.sm_UserDockablePaneId, "Distance from Reference Grid", Ribbon.thisApp.GetMainWindow() as IDockablePaneProvider);
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {

                
                CreateTheRibbonPanel(application);
                application.ControlledApplication.DocumentOpened += OnDocOpened;
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return Result.Failed;
            }
        }
        private void OnDocOpened(object sender, DocumentOpenedEventArgs args)
        {
            //TaskDialog.Show("Now", "Now1");
            UIApplication app = new UIApplication(sender as Application);
            doc = args.Document;
            string username = doc.Application.Username;
            List<RibbonPanel> myPanels = app.GetRibbonPanels("BPS Customs");
            ComboBox cb = myPanels[2].GetItems()[3] as ComboBox;
            ComboBoxMemberData cboxMemDataC = new ComboBoxMemberData("C", username);
            cb.AddItem(cboxMemDataC);
            IList<ComboBoxMember> cm = cb.GetItems();
            switch (Settings.Default.UName)
            {
                case 0:
                    cb.Current = cm[0];
                    break;
                case 1:
                    cb.Current = cm[1];
                    break;
                case 2:
                    cb.Current = cm[2];
                    break;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                List<RibbonPanel> myPanels = application.GetRibbonPanels("BPS Customs");
                Autodesk.Revit.UI.TextBox textBox = myPanels[1].GetItems()[0] as Autodesk.Revit.UI.TextBox;
                textBox.EnterPressed -= new EventHandler<
                    Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);
                TextBox tbox = myPanels[2].GetItems()[2] as TextBox;
                tbox.EnterPressed -=
                    new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(ProcessText);
                application.ControlledApplication.DocumentOpened -= OnDocOpened;
                ComboBox cbox = myPanels[2].GetItems()[3] as ComboBox;
                cbox.CurrentChanged -= new EventHandler<ComboBoxCurrentChangedEventArgs>(ProcessCb);
                RadioButtonGroup rb = myPanels[2].GetItems()[0] as RadioButtonGroup;
                
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return Result.Failed;
            }
        }

        public void CreateTheRibbonPanel(UIControlledApplication application)
        {
            string firstPanelName = "Selection Filters";
            string theribbon = "BPS Customs";
            string secondPanelName = "Mark Search";
            string panelParameterName = "Options for Section Cuts";
            string gridPanel = "Grid Distance";
            application.CreateRibbonTab(theribbon);
            thisApp = this;
            m_APIUtility = new APIUtility();

            RibbonPanel TheRibbonPanel = application.CreateRibbonPanel(theribbon, firstPanelName);

            #region Create Split Button
            //SplitButtonData splitButtonData = new SplitButtonData("Parameter Filter","Select a member to");
            //SplitButton splitButton = TheRibbonPanel.AddItem(splitButtonData) as SplitButton;
            PushButton pushButton = TheRibbonPanel.AddItem(new PushButtonData("Parameter Filter",
                "Filter by \n Parameter", AddInPath, "FilterByParameter.ParameterFilter")) as PushButton;
            pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "magnifyingglass.png"), UriKind.Absolute));
            pushButton.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "magnifyingglass-s.png"), UriKind.Absolute));
            pushButton.ToolTip = "Select an element with the desired parameter value. Select the parameter you would like to filter by in the dropdown menu.";
            //pushButton.ToolTipImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Blue4thFloorBuilding.gif"), UriKind.Absolute));
            //pushButton = splitButton.AddPushButton(new PushButtonData("Parameter Filter Project", "Filter by Parameter: Entire Project", AddInPath, "FilterByParameter.ParameterFilterProject"));
            //pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "magnifyingglass.png"), UriKind.Absolute));
            //pushButton.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "magnifyingglass-s.png"), UriKind.Absolute));
            //TheRibbonPanel.AddSeparator();
            #endregion

            RibbonPanel SecRibbonPanel = application.CreateRibbonPanel(theribbon, secondPanelName);
            TextBoxData testBoxData = new TextBoxData("SearchMark");
            Autodesk.Revit.UI.TextBox textBox = (Autodesk.Revit.UI.TextBox)(SecRibbonPanel.AddItem(testBoxData));
            textBox.PromptText = "new Mark search"; //default wall mark
            textBox.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "magnifyingglass-s.png"), UriKind.Absolute));
            textBox.ToolTip = "Search for Elements with a Mark containing: ";
            textBox.Width = 150;
            textBox.ShowImageAsButton = true;
            textBox.EnterPressed += new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);


            RibbonPanel SectionPanel = application.CreateRibbonPanel(theribbon, panelParameterName);
            PushButtonData clipTitleData = new PushButtonData("Far Clip Offset", "Far Clip Offset", AddInPath, firstPanelName);
            
            //The beginning of the radio button
            //Another Line
            RadioButtonGroupData radioData = new RadioButtonGroupData("radioGroup");
            TextBoxData testBoxData2 = new TextBoxData("SectionDistance");
            testBoxData2.Name = "Section Distance";
            ComboBoxData cbName = new ComboBoxData("UserName");
            cbName.Name = "Combo Box";
            
            rbGroup = SectionPanel.AddItem(radioData) as RadioButtonGroup;
            ToggleButtonData tb1 = new ToggleButtonData("toggleButton1", "Prompt");
            tb1.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "questionmark.png"), UriKind.Absolute));
           
            ToggleButtonData tb2 = new ToggleButtonData("toggleButton2", "No \n Prompt");
            tb2.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "questionmark.png"), UriKind.Absolute));
            rbGroup?.AddItem(tb1);
            rbGroup?.AddItem(tb2);
            SectionPanel.AddSeparator();
            rbGroup.ToolTip = "Auto Naming Prompt";

            IList<RibbonItem> stackedItems = SectionPanel.AddStackedItems(clipTitleData, testBoxData2, cbName);

            if (stackedItems.Count > 1)
            {
                PushButton pb = stackedItems[0] as PushButton;
                pb.Enabled = false;

                TextBox tbox = stackedItems[1] as TextBox;
                if (tbox != null)
                {
                    tbox.Value = Settings.Default["Clip"];
                    tbox.ShowImageAsButton = true;
                    tbox.ToolTip = "Set the Clipping Distance and press Enter";
                    tbox.Width = 200;
                    // Register event handler ProcessText
                    tbox.EnterPressed +=
                        new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(ProcessText);
                }

                ComboBox cBox = stackedItems[2] as ComboBox;
                if (cBox != null)
                {
                    cBox.ItemText = "ComboBox";
                    cBox.ToolTip = "Select an Option";
                    cBox.LongDescription = "Select a number or letter";
                    

                    if (Environment.UserName != null)
                    {
                        string te = Environment.UserName;
                        ComboBoxMemberData cboxMemDataA = new ComboBoxMemberData("A", te);

                        cBox.AddItem(cboxMemDataA);
                        cboxMemDataA.GroupName = "Username";
                    }

                    if (Environment.GetEnvironmentVariable("Initials") != null)
                    {
                        string et = Environment.GetEnvironmentVariable("Initials");
                        ComboBoxMemberData cboxMemDataB = new ComboBoxMemberData("B", et);

                        cBox.AddItem(cboxMemDataB);
                        cboxMemDataB.GroupName = "Username";
                    }
                    

                    cBox.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(ProcessCb);
                }
            }

            RibbonPanel gridRibbonPanel = application.CreateRibbonPanel(theribbon, gridPanel);


            PushButtonData pushButtonRegisterPageData = new PushButtonData(Globals.RegisterPage, Globals.RegisterPage,
                AddInPath, typeof(ExternalCommandRegisterPage).FullName);
            pushButtonRegisterPageData.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Register.png")));
            pushButtonRegisterPageData.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Register-s.png")));
            
            

            PushButtonData pushButtonShowPageData = new PushButtonData(Globals.ShowPage, Globals.ShowPage, AddInPath, typeof(ExternalCommandShowPage).FullName);
            pushButtonShowPageData.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Show.png")));
            pushButtonShowPageData.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "Show-s.png")));
            
            

            IList<RibbonItem> stackedItems2 = gridRibbonPanel.AddStackedItems(pushButtonRegisterPageData, pushButtonShowPageData);
            if (stackedItems2.Count > 1)
            {
                PushButton pushButtonRegisterPage = stackedItems2[0] as PushButton;
                pushButtonRegisterPage.AvailabilityClassName = typeof(ExternalCommandRegisterPage).FullName;

                PushButton pushButtonShowPage = stackedItems2[1] as PushButton;
                pushButtonShowPage.AvailabilityClassName = typeof(ExternalCommandShowPage).FullName;
            }
            // create toggle buttons and add to radio button group

        }

        /// <summary>
        /// Bind to text box's EnterPressed Event, show a dialogue tells user value of test box changed.
        /// </summary>
        /// <param name="evnetArgs">Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs</param>
        public void SetTextBoxValue(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox tested = sender as TextBox;
            string search = tested.Value.ToString();
            UIApplication doc = args.Application;
            SearchMark sm = new SearchMark();
            sm.Execute(doc, search);
        }

        public void ProcessText(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox test = sender as TextBox;
            string value = test.Value.ToString();
            tbox = value;
            Settings.Default["Clip"] = value;
            Settings.Default.Save();
        }
        public void ProcessCb(object sender, ComboBoxCurrentChangedEventArgs args)
        {
            string test = args.NewValue.ItemText;
            ComboBox cb = sender as ComboBox;
            uname = test;
            int i = cb.GetItems().IndexOf(args.NewValue);
            Settings.Default["UName"] = i;
            Settings.Default.Save();
        }

        /// <summary>
        /// Create the new WPF Page that Revit will dock.
        /// </summary>
        public void CreateWindow()
        {
            m_mainPage = new DockWindow();
        }

        /// <summary>
        /// Show or hide a dockable pane.
        /// </summary>
        public void SetWindowVisibility(Autodesk.Revit.UI.UIApplication application, bool state)
        {
            DockablePane pane = application.GetDockablePane(Globals.sm_UserDockablePaneId);
            if (pane != null)
            {
                if (state)
                    pane.Show();
                else
                    pane.Hide();
            }


        }


        public bool IsMainWindowAvailable()
        {

            if (m_mainPage == null)
                return false;

            bool isAvailable = true;
            try { bool isVisible = m_mainPage.IsVisible; }
            catch (Exception)
            {
                isAvailable = false;
            }
            return isAvailable;

        }

        public DockWindow GetMainWindow()
        {
            if (!IsMainWindowAvailable())
                throw new InvalidOperationException("Main window not constructed.");
            return m_mainPage;
        }

        public APIUtility GetDockableAPIUtility() { return m_APIUtility; }



        public Autodesk.Revit.UI.DockablePaneId MainPageDockablePaneId
        {

            get { return Globals.sm_UserDockablePaneId; }
        }

        #region Data

        DockWindow m_mainPage;
        internal static Ribbon thisApp = null;
        private APIUtility m_APIUtility;


        #endregion
    }




}

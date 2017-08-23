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
        public RadioButtonGroup rbGroup;

        public string UserName()
        {
            string myVariable = uname;
            return myVariable;
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
            Document doc = args.Document;
            string username = doc.Application.Username;
            List<RibbonPanel> myPanels = app.GetRibbonPanels("BPS Customs");
            ComboBox cb = myPanels[2].GetItems()[2] as ComboBox;
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
            //TaskDialog.Show("Now", app.ActiveUIDocument.Document.PathName);

            //Your code here...
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                List<RibbonPanel> myPanels = application.GetRibbonPanels("BPS Customs");
                Autodesk.Revit.UI.TextBox textBox = myPanels[1].GetItems()[0] as Autodesk.Revit.UI.TextBox;
                textBox.EnterPressed -= new EventHandler<
                    Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);
                TextBox tbox = myPanels[2].GetItems()[1] as TextBox;
                tbox.EnterPressed -=
                    new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(ProcessText);
                application.ControlledApplication.DocumentOpened -= OnDocOpened;
                ComboBox cbox = myPanels[2].GetItems()[2] as ComboBox;
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
            application.CreateRibbonTab(theribbon);

            RibbonPanel TheRibbonPanel = application.CreateRibbonPanel(theribbon, firstPanelName);

            #region Create Split Button
            SplitButtonData splitButtonData = new SplitButtonData("Parameter Filter","Select a member to");
            SplitButton splitButton = TheRibbonPanel.AddItem(splitButtonData) as SplitButton;
            PushButton pushButton = splitButton.AddPushButton(new PushButtonData("Parameter Filter",
                "Filter by Parameter: View", AddInPath, "FilterByParameter.ParameterFilter"));
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
            textBox.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "WallMark.png"), UriKind.Absolute));
            textBox.ToolTip = "Search for Elements with a Mark containing: ";
            textBox.ShowImageAsButton = true;
            textBox.EnterPressed += new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);


            RibbonPanel SectionPanel = application.CreateRibbonPanel(theribbon, panelParameterName);

            
            RadioButtonGroupData radioData = new RadioButtonGroupData("radioGroup");
            TextBoxData testBoxData2 = new TextBoxData("SectionDistance");
            testBoxData2.Name = "Section Distance";
            ComboBoxData cbName = new ComboBoxData("UserName");
            cbName.Name = "Combo Box";
            
            rbGroup = SectionPanel.AddItem(radioData) as RadioButtonGroup;
            ToggleButtonData tb1 = new ToggleButtonData("toggleButton1", "Prompt");
            ToggleButtonData tb2 = new ToggleButtonData("toggleButton2", "No Prompt");
            rbGroup?.AddItem(tb1);
            rbGroup?.AddItem(tb2);
            SectionPanel.AddSeparator();

            IList<RibbonItem> stackedItems = SectionPanel.AddStackedItems(testBoxData2, cbName);

            if (stackedItems.Count > 1)
            {


                TextBox tbox = stackedItems[0] as TextBox;
                if (tbox != null)
                {
                    tbox.Value = Settings.Default["Clip"];
                    tbox.ShowImageAsButton = true;
                    tbox.ToolTip = "Set the Clipping Distance and press Enter";
                    // Register event handler ProcessText
                    tbox.EnterPressed +=
                        new EventHandler<Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(ProcessText);
                }

                ComboBox cBox = stackedItems[1] as ComboBox;
                if (cBox != null)
                {
                    cBox.ItemText = "ComboBox";
                    cBox.ToolTip = "Select an Option";
                    cBox.LongDescription = "Select a number or letter";

                    string te = Environment.UserName;
                    string et = Environment.GetEnvironmentVariable("Initials");
                    //string re = uname;
                    ComboBoxMemberData cboxMemDataA = new ComboBoxMemberData("A", te);

                    cBox.AddItem(cboxMemDataA);
                    cboxMemDataA.GroupName = "Username";

                    ComboBoxMemberData cboxMemDataB = new ComboBoxMemberData("B", et);

                    cBox.AddItem(cboxMemDataB);
                    cboxMemDataB.GroupName = "Username";

                    cBox.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(ProcessCb);
                   /* ComboBoxMemberData cboxMemDataC = new ComboBoxMemberData("C", re);

                    cBox.AddItem(cboxMemDataC);*/
                }
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
            //TaskDialog.Show("TextBox EnterPressed Event", search);
        }

        public void ProcessText(object sender, TextBoxEnterPressedEventArgs args)
        {
            TextBox test = sender as TextBox;
            string value = test.Value.ToString();
            tbox = value;
            Settings.Default["Clip"] = value;
            Settings.Default.Save();
            //TaskDialog.Show("Option", value + " : " + uname);
        }
        public void ProcessCb(object sender, ComboBoxCurrentChangedEventArgs args)
        {
            string test = args.NewValue.ItemText;
            ComboBox cb = sender as ComboBox;
            uname = test;
            int i = cb.GetItems().IndexOf(args.NewValue);
            Settings.Default["UName"] = i;
            Settings.Default.Save();
            //TaskDialog.Show("Option", test + " : " + rbGroup.Current.ItemText) ;
        }
    }


}

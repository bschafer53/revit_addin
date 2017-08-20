using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

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
        // uiApplication
        static UIApplication uiApplication = null;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                CreateTheRibbonPanel(application);

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                //remove events
               /* List<RibbonPanel> myPanels = application.GetRibbonPanels();
                Autodesk.Revit.UI.ComboBox comboboxLevel = (Autodesk.Revit.UI.ComboBox)(myPanels[0].GetItems()[2]);
                application.ControlledApplication.DocumentCreated -= new EventHandler<
                    Autodesk.Revit.DB.Events.DocumentCreatedEventArgs>(DocumentCreated);
                Autodesk.Revit.UI.TextBox textBox = myPanels[0].GetItems()[5] as Autodesk.Revit.UI.TextBox;
                textBox.EnterPressed -= new EventHandler<
                    Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);*/
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
            string firstPanelName = "The First Panel";
            string theribbon = "New Ribbon";
            application.CreateRibbonTab(theribbon);

            RibbonPanel TheRibbonPanel = application.CreateRibbonPanel(theribbon, firstPanelName);

            #region Create Split Button
            SplitButtonData splitButtonData = new SplitButtonData("Parameter Filter","Select a member to");
            SplitButton splitButton = TheRibbonPanel.AddItem(splitButtonData) as SplitButton;
            PushButton pushButton = splitButton.AddPushButton(new PushButtonData("Parameter Filter",
                "Filter by Parameter", AddInPath, "FilterByParameter.ParameterFilter"));
            pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWall.png"), UriKind.Absolute));
            pushButton.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWall-S.png"), UriKind.Absolute));
            pushButton.ToolTip = "This button will eventually filter by parameter";
            pushButton.ToolTipImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWallTooltip.bmp"), UriKind.Absolute));

            #endregion
        }

        public void DocumentCreated(object sender, Autodesk.Revit.DB.Events.DocumentCreatedEventArgs e)
        {
            uiApplication = new UIApplication(e.Document.Application);
            List<RibbonPanel> myPanels = uiApplication.GetRibbonPanels();

            Autodesk.Revit.UI.ComboBox comboboxLevel = (Autodesk.Revit.UI.ComboBox)(myPanels[0].GetItems()[2]);
            if (null == comboboxLevel) { return; }
            FilteredElementCollector collector = new FilteredElementCollector(uiApplication.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(Level)).ToElements();
            foreach (Element elem in founds)
            {
                Level level = elem as Level;
                ComboBoxMemberData comboBoxMemberData = new ComboBoxMemberData(level.Name, level.Name);
                ComboBoxMember comboboxMember = comboboxLevel.AddItem(comboBoxMemberData);
                comboboxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "LevelsSelector.png"), UriKind.Absolute));
            }
            //refresh level list (in case user created new level after document created)
            comboboxLevel.DropDownOpened += new EventHandler<ComboBoxDropDownOpenedEventArgs>(AddNewLevels);
        }

        public void AddNewLevels(object sender, ComboBoxDropDownOpenedEventArgs args)
        {
            Autodesk.Revit.UI.ComboBox comboboxLevel = sender as Autodesk.Revit.UI.ComboBox;
            if (null == comboboxLevel) { return; }
            FilteredElementCollector collector = new FilteredElementCollector(uiApplication.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(Level)).ToElements();
            foreach (Element elem in founds)
            {
                Level level = elem as Level;
                bool alreadyContained = false;
                foreach (ComboBoxMember comboboxMember in comboboxLevel.GetItems())
                {
                    if (comboboxMember.Name == level.Name)
                    {
                        alreadyContained = true;
                    }
                }
                if (!alreadyContained)
                {
                    ComboBoxMemberData comboBoxMemberData = new ComboBoxMemberData(level.Name, level.Name);
                    ComboBoxMember comboboxMember = comboboxLevel.AddItem(comboBoxMemberData);
                    comboboxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "LevelsSelector.png"), UriKind.Absolute));
                }
            }

        }

        public void SetTextBoxValue(object sender, TextBoxEnterPressedEventArgs args)
        {
            TaskDialog.Show("TextBox EnterPressed Event", "New wall's mark changed.");
        }

    }
}

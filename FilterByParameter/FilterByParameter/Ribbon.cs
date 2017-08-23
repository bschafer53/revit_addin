using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
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
                List<RibbonPanel> myPanels = application.GetRibbonPanels("BPS Customs");
                Autodesk.Revit.UI.TextBox textBox = myPanels[1].GetItems()[0] as Autodesk.Revit.UI.TextBox;
                textBox.EnterPressed -= new EventHandler<
                    Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs>(SetTextBoxValue);
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
    }


}

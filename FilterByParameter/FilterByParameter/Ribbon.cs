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

            #endregion
        }

    }
}

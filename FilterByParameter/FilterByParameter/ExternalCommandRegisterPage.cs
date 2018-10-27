using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace FilterByParameter
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class ExternalCommandRegisterPage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            Ribbon.thisApp.GetDockableAPIUtility().Initialize(commandData.Application);
            Ribbon.thisApp.CreateWindow();
            

            Ribbon.thisApp.GetMainWindow().SetInitialDockingParameters(Autodesk.Revit.UI.DockPosition.Floating);
            try
            {
                Ribbon.thisApp.RegisterDockableWindow(commandData.Application, Globals.sm_UserDockablePaneId.Guid);
                TaskDialog.Show("Success", "The window has been registered");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// Onlys show the dialog when a document is open, as Dockable dialogs are only available
        /// when a document is open.
        /// </summary>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            if (applicationData.ActiveUIDocument == null)
                return true;
            else
                return false;
        }
    }
}

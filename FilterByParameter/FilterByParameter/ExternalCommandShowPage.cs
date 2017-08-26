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
    class ExternalCommandShowPage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                Ribbon.thisApp.GetDockableAPIUtility().Initialize(commandData.Application);
                Ribbon.thisApp.SetWindowVisibility(commandData.Application, true);
            }
            catch (Exception)
            {
                TaskDialog.Show("Dockable Dialogs", "Dialog not registered.");
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
                return false;
            else
                return true;
        }
    }
}

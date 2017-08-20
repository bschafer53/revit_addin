using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;


namespace FilterByParameter
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class ParameterFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            try
            {
                Transaction t = new Transaction(data.Application.ActiveUIDocument.Document, "New");
                t.Start();

                TaskDialog.Show("Hello", "Hello this worked");
                t.Commit();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                return Result.Failed;
            }
        }
    }
}

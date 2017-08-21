using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI.Selection;


namespace FilterByParameter
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class ParameterFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            Selection selection = uidoc.Selection;
            Transaction t = new Transaction(data.Application.ActiveUIDocument.Document, "New");
            t.Start();
            try
            {
                Reference hasPicked = selection.PickObject(ObjectType.Element);
                if (hasPicked != null)
                {
                    ElementId id = hasPicked.ElementId;
                    Element el = uidoc.Document.GetElement(id);
                    ParameterSet parameterSet = el.Parameters;

                    ParameterSelector ps = new ParameterSelector(parameterSet, data, id);
                    ps.Show();

                 /*   string parameterList = "";
                    ICollection<ElementId> eid = new List<ElementId>();


                    IList<Element> elems = new FilteredElementCollector(uidoc.Document, uidoc.Document.ActiveView.Id).ToElements();

                    foreach (Element elem in elems)
                    {
                        if (null != elem.LookupParameter("Area"))
                        {
                            if (elem.LookupParameter("Area").AsValueString() == el.LookupParameter("Area").AsValueString())
                            {
                                eid.Add(elem.Id);
                                parameterList += elem.Name + ": ";
                                parameterList += elem.Id + "\n";
                            }
                        }
                    }

                    //eid.Add(el.Id);
                    uidoc.Selection.SetElementIds(eid);
                    TaskDialog.Show("Elements", parameterList);*/
                }
                
                t.Commit();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.Message);
                t.Commit();
                return Result.Failed;
            }
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class RunFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            try
            {

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Wrong", e.Message);
                return Result.Failed;
            }
        }

        public Result Execute(ElementId id, Parameter pa, ExternalCommandData data)
        {
            try
            {
                Document e = data.Application.ActiveUIDocument.Document;
                //TaskDialog.Show("Success", pa.Definition.Name);

                string parameterList = "";
                ICollection<ElementId> eid = new List<ElementId>();
                UIDocument uidoc = data.Application.ActiveUIDocument;

                IList<Element> elems = new FilteredElementCollector(uidoc.Document, uidoc.Document.ActiveView.Id).ToElements();
                Element el = uidoc.Document.GetElement(id);

                foreach (Element elem in elems)
                {
                    if (null != elem.get_Parameter(pa.Definition))
                    {
                        if (elem.get_Parameter(pa.Definition).AsValueString() == el.get_Parameter(pa.Definition).AsValueString())
                        {
                            eid.Add(elem.Id);
                            parameterList += elem.Name + ": ";
                            parameterList += elem.Id + "\n";
                        }
                    }
                }

                //eid.Add(el.Id);
                uidoc.Selection.SetElementIds(eid);
                //TaskDialog.Show("Elements", parameterList);

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Value", e.Message);
                return Result.Failed;
            }
        }
    }
}

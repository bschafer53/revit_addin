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

                    ParameterSelector ps = new ParameterSelector(parameterSet, data, id, false);
                    ps.Show();
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
    class ParameterFilterProject : IExternalCommand
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

                    ParameterSelector ps = new ParameterSelector(parameterSet, data, id, true);
                    ps.Show();
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

        public Result Execute(ElementId id, IList<Parameter> pa, ExternalCommandData data, bool andor, bool proj)
        {
            try
            {

                string parameterList = "";
                ICollection<ElementId> eid = new List<ElementId>();
                UIDocument uidoc = data.Application.ActiveUIDocument;
                IList<bool> check = new List<bool>();
                IList<bool> check2 = new List<bool>();
                IList<Element> elems = new List<Element>();
                if (proj)
                {
                    FilteredElementCollector fec = new  FilteredElementCollector(uidoc.Document).WhereElementIsNotElementType();
                    foreach (Element e in fec)
                    {
                        if (null != e.Category && e.CanHaveTypeAssigned())
                        {
                            elems.Add(e);
                        }
                        
                    }
                }
                else
                {
                    elems = new FilteredElementCollector(uidoc.Document, uidoc.Document.ActiveView.Id).ToElements();
                }
                
                Element el = uidoc.Document.GetElement(id);
                if (andor)
                {
                    foreach (Element elem in elems)
                    {
                        check.Clear();
                        check2.Clear();
                        foreach (Parameter pas in pa)
                        {
                            if (null != elem.get_Parameter(pas.Definition))
                            {
                                check.Add(true);
                            }
                            else
                            {
                                check.Add(false);
                            }
                        }
                        if (check.Contains(false))
                        {

                        }
                        else
                        {
                            foreach (Parameter pas in pa)
                            {
                                if (elem.get_Parameter(pas.Definition).AsValueString() != null)
                                {
                                    check2.Add(elem.get_Parameter(pas.Definition).AsValueString() ==
                                               el.get_Parameter(pas.Definition).AsValueString());
                                }
                                else
                                {
                                    check2.Add(elem.get_Parameter(pas.Definition).AsString() ==
                                               el.get_Parameter(pas.Definition).AsString());
                                }
                            }
                            if (check2.Contains(false))
                            {

                            }
                            else
                            {
                                eid.Add(elem.Id);
                                parameterList += elem.Name + ": ";
                                parameterList += elem.Id + "\n";
                            }
                        }
                    }
                }
                else
                {
                    foreach (Element elem in elems)
                    {

                        check.Clear();
                        check2.Clear();
                        foreach (Parameter pas in pa)
                        {
                            if (null != elem.get_Parameter(pas.Definition))
                            {
                                check.Add(true);
                            }
                            else
                            {
                                check.Add(false);
                            }
                        }

                        for (int i = 0; i < check.Count; i++)
                        {
                            if (!check[i]) continue;
                            if (elem.get_Parameter(pa[i].Definition).AsValueString() != null)
                            {
                                check2.Add(elem.get_Parameter(pa[i].Definition).AsValueString() ==
                                           el.get_Parameter(pa[i].Definition).AsValueString());
                            }
                            else
                            {
                                check2.Add(elem.get_Parameter(pa[i].Definition).AsString() ==
                                           el.get_Parameter(pa[i].Definition).AsString());
                            }
                            
                        }
                        if (check2.Contains(true))
                        {
                            eid.Add(elem.Id);
                        }
                    }

                }

                uidoc.Selection.SetElementIds(eid);
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Value", e.Message);
                return Result.Failed;
            }
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class SearchMark : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }

        public Result Execute(UIApplication app, string search)
        {


            try

            {
                Document doc = app.ActiveUIDocument.Document;
                ICollection<ElementId> eid = new List<ElementId>();
                IList<Element> elems = new List<Element>();
                elems = new FilteredElementCollector(doc, doc.ActiveView.Id).ToElements();
                foreach (Element elem in elems)
                {
                    if (null == elem.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)) continue;
                    if (elem.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString() == search)
                    {
                        eid.Add(elem.Id);
                    }
                }
                if (eid.Count > 0)
                {
                    app.ActiveUIDocument.Selection.SetElementIds(eid);
                    //TaskDialog.Show("Success", uidoc.Document.PathName);
                }
                else
                {
                    TaskDialog.Show("No Match", "No elements in the current view have the specified Mark.");
                }
               
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Failed", e.Message);
                return Result.Failed;
            }
        }
    }
}

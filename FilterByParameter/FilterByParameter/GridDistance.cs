using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI.Selection;
using Microsoft.VisualBasic;

namespace FilterByParameter
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class GridDistance : IExternalCommand
    {
        
        public static string HGrid { get; set; }
        public static string VGrid { get; set; }
        public static XYZ origin { get; set; }
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }

        public Result Execute(UIDocument data)
        {
            UIDocument uidoc = data.Application.ActiveUIDocument;
            ISelectionFilter selFilter = new GridSelectionFilter();
            
            Selection selection = uidoc.Selection;
            Selection selection2 = uidoc.Selection;
            
            try
            {
                Reference hasPicked = selection.PickObject(ObjectType.Element, selFilter);
                ElementId id = hasPicked.ElementId;
                Element el = uidoc.Document.GetElement(id);
                Grid grid = el as Grid;
                Curve gcurve = grid.Curve;
                

                Reference hasPicked2 = selection2.PickObject(ObjectType.Element, selFilter);
                ElementId id2 = hasPicked2.ElementId;
                Element el2 = uidoc.Document.GetElement(id2);
                Grid grid2 = el2 as Grid;
                Curve gcurve2 = grid2.Curve;

                if (gcurve.GetEndPoint(0).X == gcurve.GetEndPoint(1).X)
                {
                    HGrid = grid.Name;
                    VGrid = grid2.Name;
                }
                else
                {
                    VGrid = grid.Name;
                    HGrid = grid2.Name;
                }

                origin = Intersection(gcurve, gcurve2);

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Grid", e.Message);
                return Result.Failed;
            }
        }

        public string XYZToString(XYZ point)
        {
            return "(" + point.X + ", " + point.Y + ", " + point.Z + ")";
        }



        public static XYZ Intersection(Curve c1, Curve c2)
        {
            XYZ p1 = c1.GetEndPoint(0);
            XYZ q1 = c1.GetEndPoint(1);
            XYZ p2 = c2.GetEndPoint(0);
            XYZ q2 = c2.GetEndPoint(1);
            XYZ v1 = q1 - p1;
            XYZ v2 = q2 - p2;
            XYZ w = p2 - p1;

            double c = (v2.X * w.Y - v2.Y * w.X)
                       / (v2.X * v1.Y - v2.Y * v1.X);

            double x = p1.X + c * v1.X;
            double y = p1.Y + c * v1.Y;

            XYZ p5 = new XYZ(x, y, 0);

            return p5;
        }
    }

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class BeamSelect : IExternalCommand
    {
        public static string Mark { get; set; }
        public static string StartX { get; set; }
        public static string StartY { get; set; }
        public static string EndX { get; set; }
        public static string EndY { get; set; }

        public static string BMark { get; set; }
        public static string CMark { get; set; }
        public static string stX1 { get; set; }
        public static string stY1 { get; set; }
        public static string edX1 { get; set; }
        public static string edY1 { get; set; }
        public static string clX1 { get; set; }
        public static string clY1 { get; set; }

        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }

        public Result Execute(UIDocument data)
        {
            try
            {
                UIDocument uidoc = data.Application.ActiveUIDocument;
                Selection selection3 = uidoc.Selection;
                ISelectionFilter selFilter2 = new FrameSelectionFilter();

                Reference hasPicked3 = selection3.PickObject(ObjectType.Element, selFilter2);
                ElementId id3 = hasPicked3.ElementId;
                Element el3 = uidoc.Document.GetElement(id3);
                if (el3.Category.Name == "Structural Framing")
                {
                    LocationCurve lc = (LocationCurve)el3.Location;
                    Curve lcc = lc.Curve;
                    GridDistance dist = new GridDistance();
                    BMark = el3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                    Mark = "Beam Mark";
                    StartX = "Beam Start X";
                    StartY = "Beam Start Y";
                    EndX = "Beam End X";
                    EndY = "Beam End Y";
                    

                    XYZ st = DistancePoint(GridDistance.origin, lcc, 0);
                    double stX = st.X;
                    stX1 = decToFrac(stX);
                    double stY = st.Y;
                    stY1 = decToFrac(stY);

                    XYZ ed = DistancePoint(GridDistance.origin, lcc, 1);
                    double edX = ed.X;
                    edX1 = decToFrac(edX);
                    double edY = ed.Y;
                    edY1 = decToFrac(edY);
                }
                else if (el3.Category.Name == "Structural Columns")
                {
                    LocationPoint lp = el3.Location as LocationPoint;
                    BMark = el3.get_Parameter(BuiltInParameter.COLUMN_LOCATION_MARK).AsString();
                    Mark = "Column Location Mark";
                    StartX = "Column X Distance";
                    StartY = "Column Y Distance";

                    XYZ cP = DPoint(GridDistance.origin, lp);
                    double clX = cP.X;
                    stX1 = decToFrac(clX);
                    double clY = cP.Y;
                    stY1 = decToFrac(clY);
                    edX1 = "??";
                    edY1 = "??";
                }
                else
                {
                    LocationCurve wc = (LocationCurve) el3.Location;
                    Curve wcc = wc.Curve;

                    BMark = el3.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                    Mark = "Wall Mark";
                    StartX = "Wall Start X";
                    StartY = "Wall Start Y";
                    EndX = "Wall End X";
                    EndY = "Wall End Y";

                    XYZ st = DistancePoint(GridDistance.origin, wcc, 0);
                    double stX = st.X;
                    stX1 = decToFrac(stX);
                    double stY = st.Y;
                    stY1 = decToFrac(stY);

                    XYZ ed = DistancePoint(GridDistance.origin, wcc, 1);
                    double edX = ed.X;
                    edX1 = decToFrac(edX);
                    double edY = ed.Y;
                    edY1 = decToFrac(edY);

                }
                

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Revit", e.Message);
                return Result.Cancelled;
            }
        }

        public static string decToFrac(double o)
        {
            double whole;
            double wholeinch;
            double fracinch;
            string stfrac = null;
            if (o < 0)
            {
                whole = Math.Ceiling(o);
                double remain =  whole - o;                
                double inch = remain * 12;
                wholeinch = Math.Floor(inch);
                double remainin = inch - wholeinch;
                if (remainin == 0)
                {

                }else if ((Math.Round(remainin * 8) % 2) == 0)
                {
                    fracinch = Math.Round(Math.Round(remainin * 16) / 4);
                    stfrac = fracinch + "/4";
                }else if ((Math.Round(remainin * 16) % 2) == 0)
                {
                    fracinch = Math.Round(remainin * 8);
                    stfrac = fracinch + "/8";
                }else
                {
                    fracinch = Math.Round(remainin * 16);
                    stfrac = fracinch + "/16";
                }
            }
            else
            {
                whole = Math.Floor(o);
                double remain = o - whole;
                double inch = remain * 12;
                wholeinch = Math.Floor(inch);
                double remainin = inch - wholeinch;
                if (remainin == 0)
                {

                }
                else if ((Math.Round(remainin * 8) % 2) == 0)
                {
                    fracinch = Math.Round(remainin * 4);
                    stfrac = fracinch + "/4";
                }
                else if ((Math.Round(remainin * 16) % 2) == 0)
                {
                    fracinch = Math.Round(remainin * 8);
                    stfrac = fracinch + "/8";
                }
                else
                {
                    fracinch = Math.Round(remainin * 16);
                    stfrac = fracinch + "/16";
                }
            }

            if (stfrac == null)
            {
                return whole.ToString() + "' " + wholeinch.ToString() + "\"";
            }
            else
            {
                return whole.ToString() + "' " + wholeinch.ToString() + " " + stfrac + "\"";
            }
            
        }
        public static XYZ DistancePoint(XYZ o, Curve beam, int loc)
        {
            XYZ p1 = beam.GetEndPoint(loc);

            double sX = p1.X - o.X;
            double sY = p1.Y - o.Y;
            double sZ = p1.Z - o.Z;

            XYZ dp = new XYZ(sX, sY, sZ);
            return dp;
        }

        public static XYZ DPoint(XYZ o, LocationPoint cl)
        {
            double sX = cl.Point.X - o.X;
            double sY = cl.Point.Y - o.Y;
            double sZ = cl.Point.Z - o.Z;

            XYZ dp = new XYZ(sX, sY, sZ);
            return dp;
        }
    }

    public class GridSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name.Contains("Grid"))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }

    public class FrameSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name == "Structural Framing" || element.Category.Name == "Structural Columns" || element.Category.Name == "Walls")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }


}

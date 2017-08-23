using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace FilterByParameter
{
    public partial class ParameterSelector : System.Windows.Forms.Form
    {
        private ExternalCommandData datas;
        private ElementId ids;
        private bool projs;
        public ParameterSelector(ParameterSet param, ExternalCommandData data, ElementId id, bool proj)
        {
            
            InitializeComponent();
            ToolTip ToolTip1 = new ToolTip();
            ToolTip ToolTil2 = new ToolTip();
            ToolTip1.SetToolTip(andRadio, "Only elements that match all selected parameters will be selected.");
            ToolTil2.SetToolTip(orRadio, "Any element that matches any selected parameter will be selected");
            List<Par> parList = new List<Par>();
            datas = data;
            ids = id;
            projs = proj;

            if (proj)
            {
                foreach (Parameter p in param)
                {
                    if (p.Definition.Name == "Mark")
                    {
                        parList.Add(new Par() { ID = p, Name = p.Definition.Name + ": " + p.AsValueString() + ": " + p.HasValue + ": "+ p.AsString()});
                    }
                    
                }
            }
            else
            {
                foreach (Parameter p in param)
                {
                    if (p.AsValueString() != null)
                    {
                        parList.Add(new Par() {ID = p, Name = p.Definition.Name + ": " + p.AsValueString()});
                    }
                    else
                    {
                        parList.Add(new Par() { ID = p, Name = p.Definition.Name + ": " + p.AsString() });
                    }
                   
                }
            }

            List<Par> test = parList.OrderBy(o => o.Name).ToList();
            checksParam.DataSource = test;
            checksParam.ValueMember = "ID";
            checksParam.DisplayMember = "Name";
        }


        private void filterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                bool andor = andRadio.Checked;

                Par par = new Par();
                IList<Parameter> pas = new List<Parameter>();

                foreach (var item in checksParam.CheckedItems)
                {
                    par = item as Par;
                    pas.Add(par.ID);
                    
                }

                RunFilter rf = new RunFilter();
                rf.Execute(ids, pas, datas, andor, projs);
                Close();
            }
            catch (Exception exception)
            {
                TaskDialog.Show("Error", exception.Message);
                throw;
            }

        }
    }

    public class Par
    {
        public Parameter ID { get; set; }
        public string Name { get; set; }
    }
}

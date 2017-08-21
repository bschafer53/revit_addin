using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace FilterByParameter
{
    public partial class ParameterSelector : System.Windows.Forms.Form
    {
        private ExternalCommandData datas;
        private ElementId ids;
        public ParameterSelector(ParameterSet param, ExternalCommandData data, ElementId id)
        {
            InitializeComponent();
            List<Par> parList = new List<Par>();
            datas = data;
            ids = id;


            foreach (Parameter p in param)
            {
                parList.Add(new Par(){ID=p,Name = p.Definition.Name + ": " + p.AsValueString()});
                //cbParams.Items.Add(p + ": " + p.Definition.Name + ": " + p.AsValueString());
            }

            cbParams.DataSource = parList;
            cbParams.ValueMember = "ID";
            
            cbParams.DisplayMember = "Name";
            cbParams.SelectedIndex = 0;
        }


        private void filterBtn_Click(object sender, EventArgs e)
        {
            try
            {

                UIDocument el = datas.Application.ActiveUIDocument;

                Parameter pa = cbParams.SelectedValue as Parameter;
     
                //TaskDialog.Show("View", el.Application.ActiveUIDocument.Document.GetElement(ids).get_Parameter(pa.Definition).AsValueString());
                RunFilter rf = new RunFilter();
                rf.Execute(ids, pa, datas);
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

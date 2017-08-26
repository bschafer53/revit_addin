using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

namespace FilterByParameter
{
    class APIExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            ModelessCommandData data = Ribbon.thisApp.GetDockableAPIUtility().ModelessCommand.Take();
            //Ribbon.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            Ribbon.thisApp.GetMainWindow().UpdateUI(Ribbon.thisApp.GetDockableAPIUtility().ModelessCommand.Take());
            Ribbon.thisApp.GetMainWindow().WakeUp();
        }

        public string GetName()
        {
            return "APIExternalEventHandler";
        }
    }


}

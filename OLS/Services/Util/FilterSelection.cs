using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLS.Services.Util
{
    public class FilterSelection<T> where T : class
    {
        public List<T> filteredList { get; set; }

        public FilterSelection(SelectionSet selectionSet, Transaction ts)
        {
            filteredList = new List<T>();

            foreach (SelectedObject ssObj in selectionSet)
            {
                if (ssObj != null)
                {
                    Autodesk.AutoCAD.DatabaseServices.Entity entity = ts.GetObject(ssObj.ObjectId, OpenMode.ForWrite) as Autodesk.AutoCAD.DatabaseServices.Entity;
                    if (entity is T)
                        filteredList.Add(entity as T);
                }
            }
        }
    }
}

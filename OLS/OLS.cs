#region Liberary
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using OLS.Services.Classfications.Database;
using OLS.Services.OLSes;
#endregion



namespace OLS
{
    public class OLS
    {
        [CommandMethod("dar_Draw_OLS", CommandFlags.UsePickSet)]
        public void Main()
        {
            #region Documents
            //Documents
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document cadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument _civildoc = CivilApplication.ActiveDocument;
            Database db = cadDoc.Database;
            #endregion
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    #region Select Runway Corridor
                    //Select Runway Corridor
                    Alignment runwayAlignment;
                    PromptEntityResult sPrompt = ed.GetEntity("Select Runway Alignment");
                    if (sPrompt.Status == PromptStatus.OK)
                        runwayAlignment = trans.GetObject(sPrompt.ObjectId, OpenMode.ForRead) as Alignment;

                    PromptDoubleResult RunWayWidth = ed.GetDistance(System.Environment.NewLine);
                    PromptResult AirportClass = ed.GetString("Insert Airport Class");
                    #endregion

                    #region Detect Class from Database
                    OlsCodeAttriputesDB OlsDatabase = new OlsCodeAttriputesDB();
                    #endregion

                    #region Deticting Geometry Points

                    #endregion

                    #region Drawing Surfaces
                    TakeOff_OLS takeOff_OLS = new TakeOff_OLS(OlsDatabase.classA_DB.takeOffAttriputes);
                    #endregion
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage(ex.Message);
                    trans.Abort();
                }
                trans.Commit();
            }
        }
    }
}
#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database;
using OLS.Services.Classfications.Database.Classes;
using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.OLSes;
using OLS.UI;
#endregion

namespace OLS
{
    public class OLS
    {
        [CommandMethod("dar_Draw_OLS", CommandFlags.UsePickSet)]
        public void Main()
        {
            #region Documents
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document cadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument _civildoc = CivilApplication.ActiveDocument;
            Database db = cadDoc.Database;
            #endregion

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                #region Decuments
                BlockTable acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                #endregion

                try
                {
                    #region Start UI
                    //On click button it get and populate instance from CastumClass_DB
                    Form1 form = new Form1();
                    form.ShowDialog();
                    #endregion

                    #region Selecting
                    //Select Runway Alignment
                    Alignment runwayAlignment = null;
                    PromptEntityResult sPrompt = ed.GetEntity("Select Runway Alignment");
                    if (sPrompt.Status != PromptStatus.OK)
                    {
                        trans.Abort();
                        return;
                    }
                    runwayAlignment = trans.GetObject(sPrompt.ObjectId, OpenMode.ForRead) as Alignment;

                    //Select profile
                    PromptKeywordOptions profileOptions = new PromptKeywordOptions("");
                    profileOptions.Message = "\nSelect Profile: ";
                    int i = 1;
                    string msg = "Avaliable profiles orders:";
                    foreach (ObjectId profileId in runwayAlignment.GetProfileIds())
                    {
                        Profile tempProfile = trans.GetObject(profileId, OpenMode.ForWrite) as Profile;
                        msg += "\n" + i.ToString() + "- " + tempProfile.Name;
                        profileOptions.Keywords.Add(i++.ToString());
                    }
                    Application.ShowAlertDialog(msg);
                    profileOptions.AllowNone = false;
                    PromptResult profileKeyRes = cadDoc.Editor.GetKeywords(profileOptions);
                    if (profileKeyRes.Status != PromptStatus.OK)
                    {
                        trans.Abort();
                        return;
                    }
                    int profidId = int.Parse(profileKeyRes.StringResult) - 1;
                    Profile profile = trans.GetObject(runwayAlignment.GetProfileIds()[profidId], OpenMode.ForWrite) as Profile;

                    #endregion

                    #region Detect Class from Database
                    IClass_DB class_DB = null;
                    /*OlsCodeAttriputesDB OlsDatabase = new OlsCodeAttriputesDB();
                    switch (AirportClassKeyRes.StringResult)
                    {
                        case "A":
                            class_DB = OlsDatabase.classA_DB;
                            break;
                        case "B":
                            class_DB = OlsDatabase.classB_DB;
                            break;
                        case "C":
                            class_DB = OlsDatabase.classC_DB;
                            break;
                        case "D":
                            class_DB = OlsDatabase.classD_DB;
                            break;
                        default:
                            break;
                    }*/
                    class_DB = CastumClass_DB.getIntstance();
                    #endregion

                    #region Deticting Geometry Points
                    double dist = runwayAlignment.StartingStation;
                    Point3d startAlignment = runwayAlignment.GetPointAtDist(profile.StartingStation - dist);
                    double startAlignment_Z = profile.ElevationAt(profile.StartingStation);
                    startAlignment = new Point3d(startAlignment.X, startAlignment.Y, startAlignment_Z);

                    Point3d endAlignment = runwayAlignment.GetPointAtDist(profile.EndingStation - dist);
                    double endAlignment_Z = profile.ElevationAt(profile.EndingStation);
                    endAlignment = new Point3d(endAlignment.X, endAlignment.Y, endAlignment_Z);

                    double dX = startAlignment.X - endAlignment.X;
                    double dY = startAlignment.Y - endAlignment.Y;
                    double dZ = startAlignment.Z - endAlignment.Z;

                    Vector3d startAlignmentVector = new Vector3d(dX, dY, 0); //Not include deff of Z
                    startAlignmentVector = startAlignmentVector.GetNormal();
                    Vector3d endAlignmentVector = new Vector3d(-dX, -dY, 0); //Not include deff of Z
                    endAlignmentVector = endAlignmentVector.GetNormal();

                    Vector3d startPrepAlignmentVector = startAlignmentVector.GetPerpendicularVector();
                    Vector3d endPrepAlignmentVector = endAlignmentVector.GetPerpendicularVector();
                    #endregion

                    #region Drawing Surfaces
                    //TakeOff Ols
                    TakeOff_OLS takeOff_OLS_Start = new TakeOff_OLS(class_DB.takeOffAttriputes, startAlignment, startAlignmentVector, startPrepAlignmentVector);
                    takeOff_OLS_Start.CreatePolylines(acBlkTblRec, trans);
                    takeOff_OLS_Start.CreateSurface(_civildoc, trans);

                    TakeOff_OLS takeOff_OLS_End = new TakeOff_OLS(class_DB.takeOffAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                    takeOff_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    takeOff_OLS_End.CreateSurface(_civildoc, trans);

                    //Landing Ols
                    Landding_OLS landing_OLS_Start = new Landding_OLS(class_DB.landdingAttriputes, startAlignment, startAlignmentVector, startPrepAlignmentVector);
                    landing_OLS_Start.CreatePolylines(acBlkTblRec, trans);
                    landing_OLS_Start.CreateSurface(_civildoc, trans);

                    Landding_OLS landing_OLS_End = new Landding_OLS(class_DB.landdingAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                    landing_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    landing_OLS_End.CreateSurface(_civildoc, trans);

                    //Inner Ols
                    InnerHorizontal_OLS innerHorizontal_OLS = new InnerHorizontal_OLS(class_DB.innerHorizontalAttriputes, startAlignment, endAlignment,
                                                        startAlignmentVector, startPrepAlignmentVector, endAlignmentVector, endPrepAlignmentVector);
                    innerHorizontal_OLS.CreatePolylines(acBlkTblRec,trans, db, ed);
                    innerHorizontal_OLS.CreateSurface(_civildoc, trans);

                    //Conical Ols
                    Conical_OLS conical_OLS = new Conical_OLS(class_DB.conicalAttriputes, innerHorizontal_OLS);
                    conical_OLS.CreatePolylines(acBlkTblRec, trans);
                    conical_OLS.CreateSurface(_civildoc, trans);

                    //Transtional Ols
                    Transvare_OLS transvare_OLS_Start = new Transvare_OLS(class_DB.transvareAttriputes, class_DB.landdingAttriputes, innerHorizontal_OLS,
                                        startAlignment, endAlignment, startAlignmentVector, endAlignmentVector, startPrepAlignmentVector);
                    transvare_OLS_Start.CreatePolylines(acBlkTblRec, trans);
                    transvare_OLS_Start.CreateSurface(_civildoc, trans);

                    Transvare_OLS transvare_OLS_End = new Transvare_OLS(class_DB.transvareAttriputes, class_DB.landdingAttriputes, innerHorizontal_OLS,
                                        startAlignment, endAlignment, startAlignmentVector, endAlignmentVector, endPrepAlignmentVector);
                    transvare_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    transvare_OLS_End.CreateSurface(_civildoc, trans);
                    #endregion

                    trans.Commit();
                }
                catch (System.Exception ex)
                {
                    #region Exception Handelling
                    ed.WriteMessage(ex.Message);
                    trans.Abort();
                    #endregion
                }
            }
        }
    }
}
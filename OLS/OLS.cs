#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Services.Classfications.Database;
using OLS.Services.Classfications.Database.Classes.InterfaceClass;
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
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document cadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument _civildoc = CivilApplication.ActiveDocument;
            Database db = cadDoc.Database;
            #endregion

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                try
                {
                    #region Selecting
                    //Select Runway Corridor
                    Alignment runwayAlignment = null;
                    PromptEntityResult sPrompt = ed.GetEntity("Select Runway Alignment");
                    if (sPrompt.Status != PromptStatus.OK)
                        trans.Commit();
                    runwayAlignment = trans.GetObject(sPrompt.ObjectId, OpenMode.ForRead) as Alignment;
                    Profile Profile = trans.GetObject(runwayAlignment.GetProfileIds()[1], OpenMode.ForRead) as Profile;

                    // check to make sure we have a profile:
                    if (Profile == null)
                    {
                        ed.WriteMessage("Must have at least one alignment with one profile");
                        return;
                    }

                    //PromptDoubleResult RunWayWidth = ed.GetDistance(System.Environment.NewLine);
                    PromptKeywordOptions AirportClassOptions = new PromptKeywordOptions("");
                    AirportClassOptions.Message = "\nRunway Class: ";
                    AirportClassOptions.Keywords.Add("A");
                    AirportClassOptions.Keywords.Add("B");
                    AirportClassOptions.Keywords.Add("C");
                    AirportClassOptions.Keywords.Add("D");
                    AirportClassOptions.AllowNone = false;
                    PromptResult AirportClassKeyRes = cadDoc.Editor.GetKeywords(AirportClassOptions);
                    #endregion

                    #region Detect Class from Database
                    IClass_DB class_DB = null;
                    OlsCodeAttriputesDB OlsDatabase = new OlsCodeAttriputesDB();
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
                    }
                    #endregion

                    #region Deticting Geometry Points
                    Point3d startAlignment = runwayAlignment.StartPoint;
                    double z = Profile.ElevationAt(runwayAlignment.StartingStation);
                    startAlignment = new Point3d(startAlignment.X, startAlignment.Y, z);
                    Point3d endAlignment = runwayAlignment.EndPoint;
                    z = Profile.ElevationAt(runwayAlignment.EndingStation);
                    endAlignment = new Point3d(endAlignment.X, endAlignment.Y, z);

                    double dX = startAlignment.X - endAlignment.X;
                    double dY = startAlignment.Y - endAlignment.Y;
                    double dZ = startAlignment.Z - endAlignment.Z;

                    Vector3d startAlignmentVector = new Vector3d(dX, dY, dZ);
                    startAlignmentVector = startAlignmentVector.GetNormal();
                    Vector3d endAlignmentVector = new Vector3d(-dX, -dY, -dZ);
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
                    InnerHorizontal_OLS innerHorizontal_OLS = new InnerHorizontal_OLS(class_DB.innerHorizontalAttriputes, startAlignment, endAlignment);
                    innerHorizontal_OLS.CreatePolylines(acBlkTblRec, trans);
                    innerHorizontal_OLS.CreateSurface(_civildoc, trans);

                    //Conical Ols
                    Conical_OLS conical_OLS = new Conical_OLS(class_DB.conicalAttriputes, innerHorizontal_OLS);
                    conical_OLS.CreatePolylines(acBlkTblRec, trans);
                    conical_OLS.CreateSurface(_civildoc, trans);

                    //Transtional Ols
                    Transvare_OLS transvare_OLS_Start = new Transvare_OLS(class_DB.transvareAttriputes,class_DB.landdingAttriputes,innerHorizontal_OLS,
                                        startAlignment, endAlignment,startAlignmentVector,endAlignmentVector ,startPrepAlignmentVector);
                    transvare_OLS_Start.CreatePolylines(acBlkTblRec, trans);
                    transvare_OLS_Start.CreateSurface(_civildoc, trans);

                    Transvare_OLS transvare_OLS_End = new Transvare_OLS(class_DB.transvareAttriputes, class_DB.landdingAttriputes,innerHorizontal_OLS, 
                                        startAlignment, endAlignment, startAlignmentVector, endAlignmentVector, endPrepAlignmentVector);
                    transvare_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    transvare_OLS_End.CreateSurface(_civildoc, trans);
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
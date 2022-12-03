﻿#region Liberary
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
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Document cadDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument _civildoc = CivilApplication.ActiveDocument;
            Database db = cadDoc.Database;
            #endregion

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = trans.GetObject(db.BlockTableId,OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = trans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],OpenMode.ForWrite) as BlockTableRecord;
                try
                {
                    #region Select Runway Corridor
                    //Select Runway Corridor
                    Alignment runwayAlignment = null;
                    PromptEntityResult sPrompt = ed.GetEntity("Select Runway Alignment");
                    if (sPrompt.Status == PromptStatus.OK)
                        runwayAlignment = trans.GetObject(sPrompt.ObjectId, OpenMode.ForRead) as Alignment;

                    //PromptDoubleResult RunWayWidth = ed.GetDistance(System.Environment.NewLine);
                    PromptResult AirportClass = ed.GetString("Insert Airport Class");
                    #endregion

                    #region Detect Class from Database
                    OlsCodeAttriputesDB OlsDatabase = new OlsCodeAttriputesDB();
                    #endregion

                    #region Deticting Geometry Points
                    Point3d startAlignment = runwayAlignment.StartPoint;
                    Point3d endAlignment = runwayAlignment.EndPoint;

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
                    TakeOff_OLS takeOff_OLS_Start = new TakeOff_OLS(OlsDatabase.classA_DB.takeOffAttriputes, startAlignment, startAlignmentVector, startPrepAlignmentVector);
                    takeOff_OLS_Start.CreatePolylines(acBlkTblRec , trans);
                    takeOff_OLS_Start.CreateSurface(_civildoc, trans);

                    TakeOff_OLS takeOff_OLS_End = new TakeOff_OLS(OlsDatabase.classA_DB.takeOffAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                    takeOff_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    takeOff_OLS_End.CreateSurface(_civildoc, trans);

                    //Landing Ols
                    Landding_OLS landing_OLS_Start = new Landding_OLS(OlsDatabase.classA_DB.landdingAttriputes, startAlignment, startAlignmentVector, startPrepAlignmentVector);
                    landing_OLS_Start.CreatePolylines(acBlkTblRec, trans);
                    landing_OLS_Start.CreateSurface(_civildoc, trans);

                    Landding_OLS landing_OLS_End = new Landding_OLS(OlsDatabase.classA_DB.landdingAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                    landing_OLS_End.CreatePolylines(acBlkTblRec, trans);
                    landing_OLS_End.CreateSurface(_civildoc, trans);

                    //Inner Ols

                    //Transtional Ols

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
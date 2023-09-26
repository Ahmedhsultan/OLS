#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using OLS.Persistence;
using OLS.Persistence.Models;
using OLS.Services.Classfications.Database.Classes;
using OLS.Services.Classfications.Database.Classes.InterfaceClass;
using OLS.Services.OLSes;
using OLS.Services.Util;
using OLS.UI;
using System.Collections.Generic;
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

            using (Transaction trans = db.TransactionManager.StartOpenCloseTransaction())
            {
                #region Decuments
                BlockTable acBlkTbl = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                #endregion
                #region clear DB
                //Clear runways data
                RunwayDB.getInstance().runwaysList.Clear();
                //clear class db data
                var instance = CastumClass_DB.getIntstance();
                instance = null;
                #endregion

                try
                {
                    #region Selecting
                    //  Select all alignments
                    SelectionSet selectionSet = null;
                    List<Alignment> alignmentsList = new List<Alignment>();
                    PromptSelectionOptions Options = new PromptSelectionOptions();
                    Options.MessageForAdding = "Select Alignments";
                    PromptSelectionResult targetPrompt = ed.GetSelection(Options);
                    if (targetPrompt.Status == PromptStatus.OK && targetPrompt.Value != null)
                        selectionSet = targetPrompt.Value;
                    else
                    {
                        trans.Abort();
                        return;
                    }

                    //  Filter alignments
                    FilterSelection<Alignment> fs = new FilterSelection<Alignment>(selectionSet, trans);
                    alignmentsList = fs.filteredList;

                    //Get All profiles and add alignment and profile to runwaydb
                    foreach (Alignment alignment in alignmentsList)
                    {
                        Runway runway = new Runway();
                        runway.alignment = alignment;

                        foreach (ObjectId profileId in alignment.GetProfileIds())
                        {
                            Profile tempProfile = trans.GetObject(profileId, OpenMode.ForWrite) as Profile;
                            runway.userInputs.allProfiles.Add(tempProfile);
                        }
                        RunwayDB.getInstance().runwaysList.Add(runway);
                    }
                    #endregion

                    #region Start UI
                    //Message to save before use
                    Application.ShowAlertDialog("Warning: Please save your work before using any tool.");
                    //Get Profile, startstation and endstation
                    AlignmentDataForm alignmentData = new AlignmentDataForm();
                    alignmentData.ShowDialog();
                    //On click button it get and populate instance from CastumClass_DB
                    ClassficationDataForm form = new ClassficationDataForm();
                    form.ShowDialog();
                    #endregion

                    #region Detect Class from Database
                    IClass_DB class_DB = null;
                    class_DB = CastumClass_DB.getIntstance();
                    #endregion

                    foreach (Runway runway in RunwayDB.getInstance().runwaysList)
                    {
                        #region Deticting Geometry Points
                        double dist = runway.alignment.StartingStation;
                        Point3d startAlignment = runway.alignment.GetPointAtDist(runway.userInputs.startStation - dist);
                        double startAlignment_Z = runway.profile.ElevationAt(runway.userInputs.startStation);
                        startAlignment = new Point3d(startAlignment.X, startAlignment.Y, startAlignment_Z);

                        Point3d endAlignment = runway.alignment.GetPointAtDist(runway.userInputs.endStation - dist);
                        double endAlignment_Z = runway.profile.ElevationAt(runway.userInputs.endStation);
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
                        takeOff_OLS_Start.CreatePolylines(db, trans);
                        takeOff_OLS_Start.CreateSurface(_civildoc, trans);
                        runway.takeOff_OLS_Start = takeOff_OLS_Start;
                        trans.Commit();

                        TakeOff_OLS takeOff_OLS_End = new TakeOff_OLS(class_DB.takeOffAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                        takeOff_OLS_End.CreatePolylines(db, trans);
                        takeOff_OLS_End.CreateSurface(_civildoc, trans);
                        runway.takeOff_OLS_End = takeOff_OLS_End;
                        trans.Commit();

                        //Landing Ols
                        Landding_OLS landing_OLS_Start = new Landding_OLS(class_DB.landdingAttriputes, startAlignment, startAlignmentVector, startPrepAlignmentVector);
                        landing_OLS_Start.CreatePolylines(db, trans);
                        landing_OLS_Start.CreateSurface(_civildoc, trans);
                        runway.landding_OLS_Start = landing_OLS_Start;
                        trans.Commit();

                        Landding_OLS landing_OLS_End = new Landding_OLS(class_DB.landdingAttriputes, endAlignment, endAlignmentVector, endPrepAlignmentVector);
                        landing_OLS_End.CreatePolylines(db, trans);
                        landing_OLS_End.CreateSurface(_civildoc, trans);
                        runway.landding_OLS_Start = landing_OLS_End;
                        trans.Commit();

                        //Inner Ols
                        InnerHorizontal_OLS innerHorizontal_OLS = new InnerHorizontal_OLS(class_DB.innerHorizontalAttriputes, startAlignment, endAlignment,
                                                            startAlignmentVector, startPrepAlignmentVector, endAlignmentVector, endPrepAlignmentVector);
                        innerHorizontal_OLS.CreatePolylines(trans, db, ed);
                        runway.innerHorizontal_OLS = innerHorizontal_OLS;
                        trans.Commit();

                        //Conical Ols
                        Conical_OLS conical_OLS = new Conical_OLS(class_DB.conicalAttriputes, innerHorizontal_OLS);
                        conical_OLS.CreatePolylines(db, trans);
                        runway.conical_OLS = conical_OLS;
                        trans.Commit();

                        //Transtional Ols
                        Transvare_OLS transvare_OLS_Start = new Transvare_OLS(class_DB.transvareAttriputes, class_DB.landdingAttriputes, innerHorizontal_OLS,
                                            startAlignment, endAlignment, startAlignmentVector, endAlignmentVector, startPrepAlignmentVector);
                        transvare_OLS_Start.CreatePolylines(db, trans);
                        transvare_OLS_Start.CreateSurface(_civildoc, trans);
                        runway.transvare_OLS_Start = transvare_OLS_Start;
                        trans.Commit();

                        Transvare_OLS transvare_OLS_End = new Transvare_OLS(class_DB.transvareAttriputes, class_DB.landdingAttriputes, innerHorizontal_OLS,
                                            startAlignment, endAlignment, startAlignmentVector, endAlignmentVector, endPrepAlignmentVector);
                        transvare_OLS_End.CreatePolylines(db, trans);
                        transvare_OLS_End.CreateSurface(_civildoc, trans);
                        runway.transvare_OLS_End = transvare_OLS_End;
                        trans.Commit();
                        #endregion
                    }

                    #region Multi runway case
                    //Merge conical and hl ols by tangent functionality
                    if (RunwayDB.getInstance().runwaysList.Count > 1)
                    {
                        //Get tangent and merge innerhorizontal ols
                        List<Polyline> polylines1 = new List<Polyline>();
                        RunwayDB.getInstance().runwaysList.ForEach(x => polylines1.Add(x.innerHorizontal_OLS.pline));
                        var boundry1 = new MergeConnicalAndHL_OLS(trans, db, polylines1);
                        boundry1.getTangent();
                        boundry1.mergePolylinesWithTangents();
                        boundry1.CreateInnerHLSurface(_civildoc);
                        trans.Commit();

                        //Get tangent and merge conical ols
                        List<Polyline> polylines2 = new List<Polyline>();
                        RunwayDB.getInstance().runwaysList.ForEach(x => polylines2.Add(x.conical_OLS.pline));
                        var boundry2 = new MergeConnicalAndHL_OLS(trans, db, polylines2);
                        boundry2.getTangent();
                        boundry2.mergePolylinesWithTangents();
                        boundry2.createConicalSurface(_civildoc, boundry1.innerHLSurface);
                        trans.Commit();
                    }
                    else
                    {
                        foreach (Runway runway in RunwayDB.getInstance().runwaysList)
                        {
                            //Create innerHL surface
                            runway.innerHorizontal_OLS.CreateSurface(_civildoc, trans);
                            //Create conical surface
                            runway.conical_OLS.CreateSurface(_civildoc, trans);
                        }
                    }
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
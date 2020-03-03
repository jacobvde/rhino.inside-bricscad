using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Collections.Generic;
using System;
using _OdGe = Teigha.Geometry;

namespace GH_BC
{
  public class LinearSolidComponent : GH_Component
  {
    public LinearSolidComponent() : base("Linear Solid Info", "LS", "Returns information (axis, extrusion path and profile) about a linear solid present in the BricsCAD drawing.", "BricsCAD", GhUI.Information)
    { }
    public override Guid ComponentGuid => new Guid("69CA0B78-C4D6-4822-A8EF-BC79067016FD");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override bool IsPreviewCapable { get { return false; } }
    public override bool IsBakeCapable { get { return false; } }
    protected override System.Drawing.Bitmap Icon => Properties.Resources.linearsolid;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new BcEntity(), "BuildingElement", "BE", "Building element to analyze", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddCurveParameter("Axis", "Ax", "Axis of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ExtrusionPath", "ExP", "Extrusion path of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ProfileCurves", "PC", "Profile curves of linear solid", GH_ParamAccess.list);
      pManager.AddParameter(new Profile(), "Profile", "P", "Assigned profile", GH_ParamAccess.item);
      pManager.AddPointParameter("StartPoint", "SP", "The start point of the axis", GH_ParamAccess.item);
      pManager.AddPointParameter("EndPoint", "EPo", "The end point of the axis", GH_ParamAccess.item);
      // axis angle
      pManager.AddNumberParameter("AxisAngle", "AA", "Axis angle of the linear solid", GH_ParamAccess.item);
      // eccentricity
      pManager.AddNumberParameter("Eccentricity", "E", "Eccentricity of the linear solid", GH_ParamAccess.item);
    }
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Types.BcEntity bcEnt = null;
      if (!DA.GetData("BuildingElement", ref bcEnt))
        return;

      var geom = new Bricscad.Bim.BIMLinearGeometry(bcEnt.ObjectId);
      if (geom != null)
      {
        var axis = geom.GetAxis();
        var extrusionPath = geom.GetExtrusionPath();
        var profileCurves = geom.GetProfile();
        var bimProfile = geom.GetAssignedProfile();
        if (axis != null)
        {
          DA.SetData("Axis", axis.ToRhino());
          var startPoint = axis.StartPoint.ToRhino();
          DA.SetData("StartPoint", startPoint);
          var endPoint = axis.EndPoint.ToRhino();
          DA.SetData("EndPoint", endPoint);
        }
        if (extrusionPath != null)
          DA.SetData("ExtrusionPath", extrusionPath.ToRhino());
        if (profileCurves.Count != 0)
        {
          var curves = new List<Curve>();
          profileCurves.ForEach(loop => loop.ForEach(geCurve => curves.Add(geCurve.ToRhino())));
          DA.SetDataList("ProfileCurves", curves);
        }
        if (bimProfile != null)
        {
          var profile = new Types.Profile(bimProfile);
          DA.SetData("Profile", profile);
        }
        // TODO: is this what is asked/needed?
        if (extrusionPath != null && axis != null)
        {
          var distance = axis.GetDistanceTo(extrusionPath);
          DA.SetData("Eccentricity", distance);
        }
        // angle
        if (axis != null)
        {
          // linearData = bim_core::getLinearElement(pBim); --> ?
          var zAxis = (axis.EndPoint - axis.StartPoint).GetNormal();
          var possibleXAxis = zAxis.IsParallelTo(_OdGe.Vector3d.ZAxis) ?
                                       _OdGe.Vector3d.XAxis :
                                       _OdGe.Vector3d.ZAxis.CrossProduct(zAxis).GetNormal();
          var angle = GetXAxis(geom).GetAngleTo(possibleXAxis, -zAxis);
        }
      }
    }

    private _OdGe.Vector3d GetXAxis(Bricscad.Bim.BIMLinearGeometry geom)
    {
      // TODO - correct way to get x-axis?
      var zAxis = (geom.GetAxis().EndPoint - geom.GetAxis().StartPoint).GetNormal();
      var transformation = getTranformation(_OdGe.Vector3d.ZAxis, zAxis);
      var xAxis = _OdGe.Vector3d.XAxis.TransformBy(transformation);
      return xAxis;
    }

    private _OdGe.Matrix3d getTranformation(_OdGe.Vector3d fromAxis, _OdGe.Vector3d toAxis)
    {
      // TODO
      throw new NotImplementedException();
    }
  }

  public class LinearSolidComponent_OLD : GH_Component
  {
    public LinearSolidComponent_OLD() : base("Linear Solid Info", "LS", "Returns information (axis, extrusion path and profile curves) about a linear solid present in the BricsCAD drawing.", "BricsCAD", GhUI.Information)
    { }
    public override Guid ComponentGuid => new Guid("FAFCBABF-A270-4D42-AEC5-1C508CC004A9");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override bool IsPreviewCapable { get { return false; } }
    public override bool IsBakeCapable { get { return false; } }
    protected override System.Drawing.Bitmap Icon => Properties.Resources.linearsolid;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new BcEntity(), "BuildingElement", "BE", "Building element to analyze", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddCurveParameter("Axis", "A", "Axis of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ExtrusionPath", "EP", "Extrusion path of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ProfileCurves", "PC", "Profile curves of linear solid", GH_ParamAccess.list);
    }
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Types.BcEntity bcEnt = null;
      if (!DA.GetData("BuildingElement", ref bcEnt))
        return;

      var geom = new Bricscad.Bim.BIMLinearGeometry(bcEnt.ObjectId);
      if (geom != null)
      {
        var axis = geom.GetAxis();
        var extrusionPath = geom.GetExtrusionPath();
        var profileCurves = geom.GetProfile();
        if (axis != null)
          DA.SetData("Axis", axis.ToRhino());
        if (extrusionPath != null)
          DA.SetData("ExtrusionPath", extrusionPath.ToRhino());
        if (profileCurves.Count != 0)
        {
          var curves = new List<Curve>();
          profileCurves.ForEach(loop => loop.ForEach(geCurve => curves.Add(geCurve.ToRhino())));
          DA.SetDataList("ProfileCurves", curves);
        }
      }
    }
  }
}

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Collections.Generic;
using System;

namespace GH_BC
{  
  public class LinearSolidComponent : GH_Component
  {
    public LinearSolidComponent() : base("Linear Solid Info", "LS", "Returns information (axis, extrusion path and profile curves) about a linear solid present in the BricsCAD drawing.", "BricsCAD", GhUI.Information)
    {}
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
      pManager.AddCurveParameter("Axis", "A", "Axis of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ExtrusionPath", "EP", "Extrusion path of linear solid", GH_ParamAccess.item);
      pManager.AddCurveParameter("ProfileCurves", "PC", "Profile curves of linear solid", GH_ParamAccess.list);
      pManager.AddParameter(new Profile(), "Profile", "P", "Assigned profile", GH_ParamAccess.item);
      pManager.AddNumberParameter("StartPoint", "SP", "The (x, y, z)-values of the start point", GH_ParamAccess.list);
      pManager.AddNumberParameter("EndPoint", "EP", "The (x, y, z)-values of the end point", GH_ParamAccess.list);
      // axis angle
      // eccentricity
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
        if(axis != null)
        {
          DA.SetData("Axis", axis.ToRhino());
          var startPoint = axis.StartPoint;
          var startXYZ = new List<double>(startPoint.ToArray());
          if (startXYZ.Count == 3)
          {
            DA.SetDataList("StartPoint", startXYZ);
          }
          var endPoint = axis.EndPoint;
          var endXYZ = new List<double>(endPoint.ToArray());
          if (endXYZ.Count == 3)
          {
            DA.SetDataList("EndPoint", endXYZ);
          }
        }
        if(extrusionPath != null)
          DA.SetData("ExtrusionPath", extrusionPath.ToRhino());
        if (profileCurves.Count != 0)
        {
          var curves = new List<Curve>();
          profileCurves.ForEach(loop => loop.ForEach(geCurve => curves.Add(geCurve.ToRhino())));
          DA.SetDataList("ProfileCurves", curves);
        }
        if(bimProfile != null)
        {
          var profile = new Types.Profile(bimProfile);
          DA.SetData("Profile", profile);
        }
      }
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

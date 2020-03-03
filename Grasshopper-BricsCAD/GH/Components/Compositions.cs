using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;

namespace GH_BC
{
  public class Composition : GH_Param<Types.Composition>
  {
    public override Guid ComponentGuid => new Guid("9E8ECDD1-1A37-4FCB-AC89-640D89EF8F20");
    public Composition() : base(new GH_InstanceDescription("Composition", "Composition", "Represents a BricsCAD composition.", "BricsCAD", "Composition")) { }
    public override GH_Exposure Exposure => GH_Exposure.hidden;
  }

  public class CompositionName : GH_ValueList
  {
    public CompositionName()
    {
      Category = "BricsCAD";
      SubCategory = GhUI.BimData; // TODO which one?
      Name = "Composition Names";
      Description = "Provides a name picker for all the compositions present in Compositions in BricsCAD.";
      ListMode = GH_ValueListMode.DropDown;
      NickName = "Composition name";
    }
    public override Guid ComponentGuid => new Guid("1C4D4F3C-3E37-4A1E-93D7-B4830ACD48CB");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override Bitmap Icon => Properties.Resources.link; // TODO change resource
    public void RefreshList()
    {
      var selectedItems = ListItems.Where(x => x.Selected).Select(x => x.Expression).ToList();
      ListItems.Clear();
      var compositionNames = new HashSet<string>();
      var compositions = Bricscad.Bim.BIMComposition.AvailableStringCompositions(PlugIn.LinkedDocument.Database);
      foreach (var compName in compositions)
      {
        if (!compositionNames.Contains(compName))
        {
          var item = new GH_ValueListItem(compName, "\"" + compName + "\"");
          item.Selected = selectedItems.Contains(item.Expression);
          ListItems.Add(item);
          compositionNames.Add(compName);
        }
      }
    }
    protected override IGH_Goo InstantiateT() => new GH_String();
    protected override void CollectVolatileData_Custom()
    {
      RefreshList();
      base.CollectVolatileData_Custom();
    }
  }

  public class LibraryComposition : GH_Component
  {
    public LibraryComposition() : base("Composition", "C", "Returns a composition, according to the given name.", "BricsCAD", GhUI.Information) // TODO SubCategory?
    { }
    public override Guid ComponentGuid => new Guid("801C8D72-660F-41B5-BFCA-91F49F2773EA");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override Bitmap Icon => Properties.Resources.link; // TODO change resource
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager[pManager.AddTextParameter("CompositionName", "N", "Composition name", GH_ParamAccess.item)].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new Composition(), "Composition", "C", "Library composition", GH_ParamAccess.list);
    }
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      string compName = null;
      DA.GetData("CompositionName", ref compName);
      if (compName != null)
      {
        var res = new List<Types.Composition>();
        var composition = Bricscad.Bim.BIMComposition.GetComposition(PlugIn.LinkedDocument.Database, compName);
        if (composition != null)
          res.Add(new Types.Composition(composition));
        DA.SetDataList("Composition", res);
      }
      else
      {
        var res = new List<Types.Composition>();
        var compositions = Bricscad.Bim.BIMComposition.AvailableObjectCompositions(PlugIn.LinkedDocument.Database);
        foreach (var composition in compositions)
        {
          res.Add(new Types.Composition(composition));
        }
        DA.SetDataList("Composition", res);
      }
    }
  }

  public class CompositionInfo : GH_Component
  {
    public CompositionInfo() : base("Composition Info", "CI", "Returns information about the specified composition.", "BricsCAD", GhUI.Information)
    { }
    public override Guid ComponentGuid => new Guid("E08676DF-F1AF-4FEB-ABB2-FF77E4695B5C");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override Bitmap Icon => Properties.Resources.link; // TODO Change resource
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new Composition(), "Composition", "C", "Composition information", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("CompositionName", "N", "Composition name", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Types.Composition composition = null;
      if (!DA.GetData("Composition", ref composition))
        return;
      DA.SetData("CompositionName", composition.Value.Name);
    }
  }
}

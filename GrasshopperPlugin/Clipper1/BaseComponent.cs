using System;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;

namespace StudioAvw.Clipper.Components.Clipper1;

/// <summary>
/// Base component that provides Clipper version and EvenOdd fill rule toggling
/// via right-click menu, with serialization and undo support.
/// </summary>
public abstract class BaseComponent : GH_Component
{
    protected BaseComponent(string name, string nickname, string description)
        : base(name, nickname, description, "Clipper", "Polyline")
    {
    }

    #region ClipperVersion

    private ClipperVersion _version = ClipperVersion.Clipper1;

    public ClipperVersion Version
    {
        get => _version;
        set
        {
            _version = value;
            UpdateMessage();
        }
    }

    #endregion

    #region EvenOdd

    private bool _evenOdd = true;

    public bool EvenOdd
    {
        get => _evenOdd;
        set
        {
            _evenOdd = value;
            UpdateMessage();
        }
    }

    #endregion

    private void UpdateMessage()
    {
        var fill = EvenOdd ? "Even/Odd" : "Non-Zero";
        var ver = Version == ClipperVersion.Clipper1 ? "v1" : "v2";
        Message = $"{fill} | {ver}";
    }

    #region Serialization

    public override bool Write(GH_IWriter writer)
    {
        writer.SetBoolean("EvenOdd", EvenOdd);
        writer.SetInt32("ClipperVersion", (int)Version);
        return base.Write(writer);
    }

    public override bool Read(GH_IReader reader)
    {
        if (reader.ItemExists("EvenOdd"))
            EvenOdd = reader.GetBoolean("EvenOdd");
        if (reader.ItemExists("ClipperVersion"))
            Version = (ClipperVersion)reader.GetInt32("ClipperVersion");
        else
            Version = ClipperVersion.Clipper1;
        return base.Read(reader);
    }

    #endregion

    #region Context menu

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
        var evenOddItem = Menu_AppendItem(menu, "EvenOdd", Menu_EvenOddClicked, true, EvenOdd);
        evenOddItem.ToolTipText = @"When checked, even/odd sorting rule is used.";

        Menu_AppendSeparator(menu);

        var v1Item = Menu_AppendItem(menu, "Clipper 1", Menu_Clipper1Clicked, true, Version == ClipperVersion.Clipper1);
        v1Item.ToolTipText = @"Use Clipper1 library (legacy).";

        var v2Item = Menu_AppendItem(menu, "Clipper 2", Menu_Clipper2Clicked, true, Version == ClipperVersion.Clipper2);
        v2Item.ToolTipText = @"Use Clipper2 library.";

        AppendComponentMenuItems(menu);
    }

    /// <summary>
    /// Override this to add component-specific menu items after the version/fill toggles.
    /// </summary>
    protected virtual void AppendComponentMenuItems(ToolStripDropDown menu) { }

    private void Menu_EvenOddClicked(object sender, EventArgs e)
    {
        RecordUndoEvent("EvenOdd");
        EvenOdd = !EvenOdd;
        ExpireSolution(true);
    }

    private void Menu_Clipper1Clicked(object sender, EventArgs e)
    {
        if (Version == ClipperVersion.Clipper1) return;
        RecordUndoEvent("ClipperVersion");
        Version = ClipperVersion.Clipper1;
        ExpireSolution(true);
    }

    private void Menu_Clipper2Clicked(object sender, EventArgs e)
    {
        if (Version == ClipperVersion.Clipper2) return;
        RecordUndoEvent("ClipperVersion");
        Version = ClipperVersion.Clipper2;
        ExpireSolution(true);
    }

    #endregion
}
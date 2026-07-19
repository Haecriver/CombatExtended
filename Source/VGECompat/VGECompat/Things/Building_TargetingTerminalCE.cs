using RimWorld;
using System.Collections.Generic;
using VanillaGravshipExpanded;
using Verse;
using Verse.Sound;

#region License
// This file includes modified portions of code from:
// https://github.com/Vanilla-Expanded/VanillaGravshipExpanded/blob/main/Source/Things/Building_TargetingTerminal.cs
//
// Original code © Oskar Potocki and the Vanilla Gravship Expanded Team.
// Incorporated with permission for Combat Extended–Vanilla Gravship Expended compatibility purposes only.
// All rights to the original code remain with the original authors.
#endregion

namespace CombatExtended.Compatibility.VGECompat;

[StaticConstructorOnStartup]
public class Building_TargetingTerminalCE : Building_TargetingTerminal, ITurretLinkerCE
{
    public Building_GravshipTurretCE linkedTurretCE;
    public List<Building_GravshipTurretCE> linkedTurretsCE = new List<Building_GravshipTurretCE>();
    public virtual IEnumerable<Building_GravshipTurretCE> LinkedTurretsCE => linkedTurretsCE;
    public override void ExposeData()
    {
        // skip the linkedTurret save/load, we don't need its
        linkedTurret = null;
        base.ExposeData();

        Scribe_References.Look(ref linkedTurretCE, "linkedTurretCE");
        Scribe_Collections.Look(ref linkedTurretsCE, "linkedTurretsCE", LookMode.Reference);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            linkedTurretsCE ??= new List<Building_GravshipTurretCE>();
            if (linkedTurretCE != null && !linkedTurretsCE.Contains(linkedTurretCE))
            {
                linkedTurretsCE.Add(linkedTurretCE);
            }
            linkedTurretsCE.RemoveAll(x => x == null);
            linkedTurretCE = null;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        // dummy value to skip logic in base.SpawnSetup
        linkedTurrets = new List<Building_GravshipTurret>() { new Building_GravshipTurret() };
        base.SpawnSetup(map, respawningAfterLoad);

        if (linkedTurretsCE.Count == 0)
        {
            EnableOverlay();
        }
    }

    public override void Tick()
    {
        // skip the turret unlink from base.Tick (because this instance is not spawned and never will be)
        linkedTurrets = new List<Building_GravshipTurret>();
        base.Tick();

        // VGE logic
        for (int i = linkedTurretsCE.Count - 1; i >= 0; i--)
        {
            var turret = linkedTurretsCE[i];
            if (turret.Destroyed || !turret.Spawned)
            {
                Unlink(turret);
            }
        }
    }

    public override void DrawExtraSelectionOverlays()
    {
        // skip the turret unlink from base.DrawExtraSelectionOverlays
        linkedTurrets = new List<Building_GravshipTurret>();

        base.DrawExtraSelectionOverlays();

        // VGE logic
        foreach (var turret in linkedTurretsCE)
        {
            GenDraw.DrawLineBetween(this.TrueCenter(), turret.TrueCenter(), SimpleColor.White);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (linkedTurretCE != null)
        {
            linkedTurrets = new List<Building_GravshipTurret>(); // dummy to skip base unlink logic
        }

        foreach (var gizmo in this.GetLinkerGizmos(LinkRange))
        {
            yield return gizmo;
        }
    }

    public virtual void LinkTo(Building_GravshipTurretCE turret)
    {
        if (linkedTurretsCE.Count >= MaxLinkedTurrets)
        {
            Unlink(linkedTurrets[0]);
        }
        linkedTurretsCE.Add(turret);
        turret.LinkTo(this);
    }

    public virtual void Unlink(Building_GravshipTurretCE turret)
    {
        if (linkedTurretsCE.Remove(turret))
        {
            turret.unlinking = true;
            turret.Unlink();
            turret.unlinking = false;
            if (linkedTurretsCE.Count == 0 && !Destroyed && Spawned)
            {
                EnableOverlay();
            }
        }
    }

    public new void Unlink()
    {
        for (int i = linkedTurretsCE.Count - 1; i >= 0; i--)
        {
            Unlink(linkedTurretsCE[i]);
        }
    }
}


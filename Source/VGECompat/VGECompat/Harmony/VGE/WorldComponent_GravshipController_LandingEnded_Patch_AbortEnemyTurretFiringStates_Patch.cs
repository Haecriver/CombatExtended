using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Linq;
using VanillaGravshipExpanded;
using Verse;

namespace CombatExtended.Compatibility.VGECompat;

[HarmonyPatch(typeof(WorldComponent_GravshipController_LandingEnded_Patch), nameof(WorldComponent_GravshipController_LandingEnded_Patch.AbortEnemyTurretFiringStates))]
[HarmonyBefore("vanillaexpanded.gravship")]
public class WorldComponent_GravshipController_LandingEnded_Patch_AbortEnemyTurretFiringStates_Patch
{
    public static void Postfix(Gravship gravship)
    {
        foreach (var map in Find.Maps)
        {
            foreach (var thing in map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial))
            {
                if (thing is Building_GravshipTurretCE gravshipTurret && gravshipTurret.Faction != null && gravshipTurret.Faction.HostileTo(Faction.OfPlayer))
                {
                    if (!gravshipTurret.globalTargetInfo.IsValid || !gravship.Things.Contains(gravshipTurret.globalTargetInfo.Thing))
                    {
                        continue;
                    }
                    gravshipTurret.AbortFiringState();
                }
            }
        }
    }
}


using Verse;

namespace CombatExtended;

public class CompProperties_OrbitalTurret : CompProperties
{
    public float interLayerPrecisionBonusFactor = 1;
    public bool isMarkMandatory = false;

    public CompProperties_OrbitalTurret()
    {
        compClass = typeof(CompOrbitalTurret);
    }
}

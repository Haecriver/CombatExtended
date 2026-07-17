using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaGravshipExpanded;

namespace CombatExtended.Compatibility.VGECompat;

public interface ITurretLinkerCE : ITurretLinker
{
    IEnumerable<Building_GravshipTurretCE> LinkedTurretsCE { get; }
    void LinkTo(Building_GravshipTurretCE turret);
    void Unlink(Building_GravshipTurretCE turret);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum MovementType
    {
        General, // Special data flag: Not used for movement type for units but for passability rules. This rule will be used for this tile data if more specific data is not found for a given movement type

        BasicUnit, // Catch-all for whatever doesn't fit in the following
        GroundVehicle, // Regular sized human vehicles, from cars to tanks. Should have bonus on road terrain?
        WaterVehicle, // Seaborne vessels that can't enter land but can travel on water tiles
        AmphibiousVehicle, // Will there even be an amphibious vehicle that doesn't fly? idk

        SmallUnit, // Small 1x1 giant units
        MediumUnit, // Medium 2x2 giant units
        LargeUnit, // Large 3x3 giant units
        GiantUnit, // G-Sized 4x4 or greater giant units

        FlyingVehicle, // Flying vehicles, like helicopters
        FlyingUnit, // Flying giant units like the Harpy
    }
}

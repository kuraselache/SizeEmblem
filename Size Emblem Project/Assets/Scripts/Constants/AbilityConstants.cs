using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Scripts.Constants
{
    public enum AbilityCategory
    {
        None    = 0b_000, // Empty category
        Base    = 0b_001, // Flag for actions that don't appear under a subcategory, such as Stay or Defend or Use Item
        Attack  = 0b_010, // Abilities that appear under the Attack category
        Special = 0b_100, // Abilities that appear under the Special category
    }

    // Rules for determining the Min/Max range of an ability
    public enum AbilityRangeDistanceRule
    {
        Basic, // Use the Min/Max range values in the Ability object itself
        SizeRangeSmall, // Based on the size of the user of the ability: It scales with the dimension of the unit. Assumes min range of 1
         
    }


    public enum AbilityRangeSpecialRule
    {
        None, // No special rules for range. Just use Min/Max distance from unit position
        Directional, // Range is only applicable for edges of the unit in 4 directions. Still uses Min/Max values

    }
}

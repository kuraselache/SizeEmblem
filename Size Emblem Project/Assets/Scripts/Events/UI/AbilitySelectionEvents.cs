using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Events.UI
{


    public delegate void AbilitySelectionButtonSelectedHandler(IAbilitySelectionButton button, AbilitySelectedEventArgs e);

    public class AbilitySelectedEventArgs : EventArgs
    {
        public IGameUnit User { get; }
        public IAbility Ability { get; }

        public AbilitySelectedEventArgs(IGameUnit user, IAbility ability)
        {
            User = user;
            Ability = ability;
        }
    }
}

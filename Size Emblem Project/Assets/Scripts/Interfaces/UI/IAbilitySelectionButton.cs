using SizeEmblem.Assets.Scripts.Events.UI;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI
{
    public interface IAbilitySelectionButton
    {
        IGameUnit User { get; }
        IAbility Ability { get; }

        bool IsEnabled { get; }
        bool IsHovering { get; }

        bool IsVisible { get; set; }

        event AbilitySelectionButtonSelectedHandler Selected;

        void ClearAbilityData();
        void UpdateAbilityData(IGameUnit user, IAbility ability);
    }
}

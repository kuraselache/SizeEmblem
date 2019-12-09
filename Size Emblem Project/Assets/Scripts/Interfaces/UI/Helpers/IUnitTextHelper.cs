using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI.Helpers
{
    public interface IUnitTextHelper
    {
        IGameUnit SelectedUnit { get; set; }

        void RefreshText();
    }
}

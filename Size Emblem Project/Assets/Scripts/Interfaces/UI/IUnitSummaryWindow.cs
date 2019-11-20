using SizeEmblem.Assets.Scripts.Interfaces.UI.Base;
using SizeEmblem.Scripts.Interfaces.GameUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI
{
    public interface IUnitSummaryWindow : IWindowBase
    {
        bool IsEnabled { get; set; }

        IGameUnit SelectedUnit { get; set; }
    }
}

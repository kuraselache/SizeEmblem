using SizeEmblem.Assets.Scripts.Interfaces.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI
{
    public interface IUnitActionWindow : IWindowBase
    {
        event Action AttackSelected;
        event Action SpecialSelected;
        event Action DefendSelected;
        event Action ItemSelected;
    }
}

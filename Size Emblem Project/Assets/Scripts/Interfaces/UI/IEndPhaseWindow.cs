using SizeEmblem.Assets.Scripts.Interfaces.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI
{
    public interface IEndPhaseWindow : IWindowBase
    {
        event Action OKAction;
        event Action CancelAction;
    }
}

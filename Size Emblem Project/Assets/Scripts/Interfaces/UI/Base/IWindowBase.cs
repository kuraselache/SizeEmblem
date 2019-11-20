using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizeEmblem.Assets.Scripts.Interfaces.UI.Base
{
    public interface IWindowBase
    {
        bool IsVisible { get; set; }

        void RefreshUI();
    }
}

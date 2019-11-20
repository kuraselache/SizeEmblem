using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI.Base;
using System;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class EndPhaseWindow : WindowBase, IEndPhaseWindow
    {


        public event Action OKAction;

        public void OKClicked()
        {
            OKAction?.Invoke();
        }

        public event Action CancelAction;

        public void CancelClicked()
        {
            CancelAction?.Invoke();
        }

    }
}
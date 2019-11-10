using SizeEmblem.Assets.Scripts.UI.Base;
using System;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class EndPhaseWindow : WindowBase
    {


        public Action OKAction;

        public void OKClicked()
        {
            OKAction?.Invoke();
        }

        public Action CancelAction;

        public void CancelClicked()
        {
            CancelAction?.Invoke();
        }

    }
}
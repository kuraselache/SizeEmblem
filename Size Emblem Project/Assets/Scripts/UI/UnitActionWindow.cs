using SizeEmblem.Assets.Scripts.Interfaces.UI;
using SizeEmblem.Assets.Scripts.UI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SizeEmblem.Assets.Scripts.UI
{
    public class UnitActionWindow : WindowBase, IUnitActionWindow
    {

        public event Action AttackSelected;
        public event Action SpecialSelected;
        public event Action DefendSelected;

        public event Action ItemSelected;

        public void OnAttackSelected()
        {
            AttackSelected?.Invoke();
        }

        public void OnSpecialSelected()
        {
            SpecialSelected?.Invoke();
        }

        public void OnDefendSelected()
        {
            DefendSelected?.Invoke();
        }

        public void OnItemSelected()
        {
            ItemSelected?.Invoke();
        }


    }
}

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace SizeEmblem.Assets.Scripts.Editors
{
    public class AbilityEditorWindow : EditorWindow
    {
        [MenuItem("Window/Editors/Ability")]
        public static void ShowWindow()
        {
            GetWindow<AbilityEditorWindow>("Ability Editor");
        }


        public void OnGUI()
        {

        }


        public void LoadAbilities()
        {

        }

        public void SavAbilities()
        {

        }
    }
}

#endif
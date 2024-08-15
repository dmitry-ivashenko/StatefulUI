using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class TextReference : BaseReference
    {
        [Role(typeof(TextRoleAttribute), "Drop Link", "RemoveReference")]
        public int Role;

        public string Identificator;

        [HideInInspector]
        public bool IsTextMeshPro;

        [ChildOnly]
        public Text Text;
        
        [ChildOnly]
        public TextMeshProUGUI TMP;
        
        public string Value => IsTextMeshPro && TMP != null ? TMP.text : Text != null ? Text.text : "[empty]";
        public string ObjectName => IsTextMeshPro && TMP != null ? TMP.name.Replace("TMP", "") : Text != null ? Text.name : "null";

        public bool Localize = true;

        public bool IsEmpty => Text == null && TMP == null;
        public bool ContainsInInnerComponent { get; set; }
        public int LocalizedTimes { get; set; }

        private Color GetIdGuiColor()
        {
            if (LocalizedTimes > 1)
            {
                return new Color(1f, 0.44f, 0.46f);    
            }

            return ContainsInInnerComponent ? new Color(1f, 0.94f, 0.73f) : Color.white;
        }

        public void SetText(object text)
        {
            if (IsTextMeshPro)
            {
                TMP.text = text.ToString();
            }
            else
            {
                Text.text = text.ToString();
            }
        }

        public void SetActive(bool isActive)
        {
            if (IsTextMeshPro)
            {
                TMP.gameObject.SetActive(isActive);
            }
            else
            {
                Text.gameObject.SetActive(isActive);
            }
        }
    }
}

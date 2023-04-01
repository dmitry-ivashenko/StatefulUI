using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using TMPro;

namespace StatefulUI.Runtime.States
{
    public struct StatePhraseReference
    {
        [Role(typeof(StateRoleAttribute))]
        public int Role;

        public string Identificator;

        [ChildOnly]
        public TextMeshProUGUI TMP;

        public string Value => TMP.text;
    }
}

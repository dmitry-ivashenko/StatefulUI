using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;

namespace StatefulUI.Runtime.References
{
    [Serializable]
    public class AnimatorReference : BaseReference
    {
        [Role(typeof(AnimatorRoleAttribute))]
        public int Role;

        [ChildOnly]
        public Animator Animator;

        public void SetBool(int param, bool value)
        {
            if (Animator != null && Animator.gameObject.activeInHierarchy)
            {
                Animator.SetBool(param, value);
            }
        }

        public void SetTrigger(int param)
        {
            if (Animator != null && Animator.gameObject.activeInHierarchy)
            {
                Animator.SetTrigger(param);
            }
        }

        public void SetTrigger(string param)
        {
            if (Animator != null && Animator.gameObject.activeInHierarchy)
            {
                Animator.SetTrigger(param);
            }
        }
    }
}

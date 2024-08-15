using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using TMPro;
using UnityEngine.UI;

namespace StatefulUI.Runtime.References
{
	[Serializable]
	public class TextInputReference : BaseReference
	{
		[Role(typeof(TextInputRoleAttribute), "Drop Link", "RemoveReference")]
		public int Role;

		public TMP_InputField InputFieldTMP;

		public InputField InputField;

		public bool IsTMP;

		public string Text() => IsTMP ? InputFieldTMP.text : InputField.text;
		public bool IsEmpty => InputFieldTMP == null && InputField == null;

		public void SetText(string text)
		{
			if (IsTMP) InputFieldTMP.text = text;
			else InputField.text = text;
		}

		public void SetActive(bool isActive)
		{
			if (IsTMP) InputFieldTMP.gameObject.SetActive(isActive);
			else InputField.gameObject.SetActive(isActive);
		}
	}
}

using UnityEngine;

namespace StatefulUI.Runtime.Core
{
	public class ChildOnlyAttribute : PropertyAttribute
	{
		public string OnSelected { get; }

		public bool HasSelector => !string.IsNullOrEmpty(OnSelected);

		public ChildOnlyAttribute()
		{}
		
		public ChildOnlyAttribute(string onSelected)
		{
			OnSelected = onSelected;
		}
	}
}
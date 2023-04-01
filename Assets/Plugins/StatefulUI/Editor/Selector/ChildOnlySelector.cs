using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Editor.Selector
{
	public class ChildOnlySelector : EditorWindow
	{
		[SerializeField] private bool _isFlatMode;
		[SerializeField] private string _nameFilter;

		private readonly SelectorStyles _styles = new SelectorStyles();
		private readonly Hierarchy _hierarchy = new Hierarchy();

		private HierarchyDrawer _hierarchyDrawer;
		private Action<Object> _selectionCallback;
		private bool _focusNameFilter;

		private static void Show([NotNull] Type assetType, SerializedProperty serializedProperty, Action<Object> callback, 
			[CanBeNull] Object selectedAsset = null)
		{
			var childOnlySelector = CreateInstance<ChildOnlySelector>();
			childOnlySelector._hierarchyDrawer = new HierarchyDrawer(childOnlySelector._hierarchy, childOnlySelector._styles, callback);
			childOnlySelector._selectionCallback = callback;
			childOnlySelector._hierarchy.UpdateAssetList(serializedProperty, assetType);
			
			if (selectedAsset)
			{
				childOnlySelector._hierarchy.SelectedAssetEntry = childOnlySelector._hierarchy.GetEntries(false)
					.FirstOrDefault(entry => entry.Asset == selectedAsset);
			}

			childOnlySelector.ShowAuxWindow();
			childOnlySelector._focusNameFilter = true;
			childOnlySelector.Focus();
		}
		
		public static void PropertyField(Rect position, [NotNull] SerializedProperty property,
			[NotNull] GUIContent label, [NotNull] Type assetType, Action callback)
		{
			void SelectCallback(Object selectedObject)
			{
				property.serializedObject.Update();
				property.objectReferenceValue = selectedObject;
				property.serializedObject.ApplyModifiedProperties();
				callback();
			}

			PropertyField(position, property, property.objectReferenceValue, SelectCallback, label, assetType);
		}

		private static void PropertyField(Rect position, [NotNull] SerializedProperty property, [CanBeNull] Object currentValue, 
			[NotNull] Action<Object> selectCallback, [NotNull] GUIContent label, [NotNull] Type assetType)
		{
			using (var scope = new EditorGUI.PropertyScope(position, label, property))
			{
				EditorGUI.BeginChangeCheck();
				var buttonPosition = position;
				buttonPosition.xMin = buttonPosition.xMax - EditorGUIUtility.singleLineHeight;
				var requesterWindow = focusedWindow;

				string controlName = property.serializedObject.targetObject.GetInstanceID() + "_" + property.propertyPath;
				var currentEvent = Event.current;

				var showHotKey = GUI.GetNameOfFocusedControl() == controlName && currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Return;
				var deleteHotKey = GUI.GetNameOfFocusedControl() == controlName && currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Delete;

				if (showHotKey || deleteHotKey)
				{
					currentEvent.Use();
				}

				if (deleteHotKey)
				{
					selectCallback(null);
				}
				else if (showHotKey || GUI.Button(buttonPosition, "", GUIStyle.none))
				{
					Show(assetType, property, selectedObject =>
						{
							property.serializedObject.Update();
							selectCallback(selectedObject);
							property.serializedObject.ApplyModifiedProperties();

							if (requesterWindow != null)
							{
								requesterWindow.Repaint();
								requesterWindow.Focus();
							}
						},
						currentValue
					);
				}

				GUI.SetNextControlName(controlName);

				var objectField = EditorGUI.ObjectField(position, scope.content, currentValue, assetType, true);

				if (EditorGUI.EndChangeCheck())
				{
					selectCallback(objectField);
				}
			}
		}

		static ChildOnlySelector()
		{
		}

		public ChildOnlySelector()
		{
			Selection.selectionChanged += Repaint;
		}

		private void OnEnable()
		{
			titleContent = new GUIContent("Assets");
			_hierarchyDrawer?.Serialize();
		}

	    private void OnDisable()
		{
			_hierarchyDrawer?.Deserialize();
		}

		private void OnGUI()
		{
			_styles.InitStyles();
			HandleKeyDown();
			
			using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(false)))
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					GUILayout.Label("Name filter:", GUILayout.Width(70), GUILayout.ExpandWidth(false));
					EditorGUI.BeginChangeCheck();
					GUI.SetNextControlName("NameFilter");
					_nameFilter = EditorGUILayout.TextField(_nameFilter ?? "");

					if (_focusNameFilter)
					{
						EditorGUI.FocusTextInControl("NameFilter");
						_focusNameFilter = false;
					}

					if (EditorGUI.EndChangeCheck())
					{
						_hierarchy.UpdateFilter(_nameFilter);
					}
				}
			}
			
			_hierarchyDrawer.DisplayList(_isFlatMode, this);
		}

		private void HandleKeyDown()
		{
			var currentEvent = Event.current;
			if (currentEvent.type != EventType.KeyDown)
			{
				return;
			}

			switch (currentEvent.keyCode)
			{
				case KeyCode.Return:
					OnAssetSelected();
					currentEvent.Use();
					break;
				case KeyCode.Escape:
					Close();
					currentEvent.Use();
					return;
				case KeyCode.DownArrow:
					_hierarchy.SelectedAssetEntry =
						_hierarchy.GetEntries(true)
							.SkipWhile(entry => _hierarchy.SelectedAssetEntry != null && entry != _hierarchy.SelectedAssetEntry)
							.Skip(1)
							.FirstOrDefault(e => !e.Hidden) ?? _hierarchy.SelectedAssetEntry;

					_hierarchyDrawer.OnAssetHover();
					currentEvent.Use();
					Repaint();
					break;
					
				case KeyCode.UpArrow:
					_hierarchy.SelectedAssetEntry =
						_hierarchy.GetEntries(true)
							.Reverse<HierarchyEntry>()
							.SkipWhile(entry => _hierarchy.SelectedAssetEntry != null && entry != _hierarchy.SelectedAssetEntry)
							.Skip(1)
							.FirstOrDefault(e => !e.Hidden) ?? _hierarchy.SelectedAssetEntry;

					_hierarchyDrawer.OnAssetHover();
					currentEvent.Use();
					Repaint();
					break;
			}
		}
		
		private void OnAssetSelected()
		{
			if (_hierarchy.SelectedAssetEntry == null) return;

			if (_selectionCallback != null)
			{
				_selectionCallback(_hierarchy.SelectedAssetEntry.Asset);
			}
			else
			{
				Selection.activeObject = _hierarchy.SelectedAssetEntry.Asset;
			}

			Close();
		}
	}
}
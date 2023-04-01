using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Editor.Selector
{
    public class HierarchyDrawer
    {
	    private const int AssetShowCountStep = 50;
	    private const string HierarchyPrefsKey = "SelectorHierarchyPrefsKey";

	    private readonly Hierarchy _hierarchy;
	    private readonly SelectorStyles _styles;
	    private readonly Action<Object> _selectionCallback;
	    private readonly HashSet<string> _foldedPaths = new HashSet<string>();

	    private int _assetShowCount = AssetShowCountStep;
	    private int _assetsShown;
	    private Vector2 _scroll;
	    private bool _scrollToSelected;
	    private bool _enableSelectionOnClick;

	    public HierarchyDrawer(Hierarchy hierarchy, SelectorStyles styles, Action<Object> selectionCallback)
	    {
		    _hierarchy = hierarchy;
		    _styles = styles;
		    _selectionCallback = selectionCallback;
		    _scrollToSelected = true;
	    }

	    public void DisplayList(bool isFlatMode, EditorWindow window)
	    {
		    _assetsShown = 0;

		    using (var scope = new EditorGUILayout.ScrollViewScope(_scroll, GUI.skin.box, GUILayout.ExpandHeight(true)))
		    {
			    _scroll = scope.scrollPosition;
			    DisplayList(_hierarchy.GetEntries(isFlatMode), window);
		    }

		    if (_assetsShown == _assetShowCount)
		    {
			    if (GUILayout.Button(_assetsShown + " assets shown. Show more..."))
			    {
				    _assetShowCount += AssetShowCountStep;
			    }
		    }
		    else
		    {
			    GUILayout.Box(_assetsShown + " assets shown.", GUILayout.ExpandWidth(true));
		    }
		    
		    if (_scrollToSelected)
		    {
			    window.Repaint();
		    }
	    }

	    private void DisplayList(List<HierarchyEntry> list, EditorWindow window)
		{
			if (list == null)
			{
				return;
			}

			foreach (var entry in list)
			{
				if (_assetsShown >= _assetShowCount)
				{
					return;
				}
				if (entry.Hidden)
				{
					continue;
				}

				if (entry.Children == null)
				{
					DisplayAsset(entry, window);
					_assetsShown++;
				}
				else
				{
					// this is a folder
					var ex = EditorGUILayout.Foldout(entry.Expanded, entry.Name, true, _styles.Folder);
					// store folded status
					if (ex != entry.Expanded)
					{
						if (ex)
						{
							_foldedPaths.Remove(entry.Name);
						}
						else
						{
							_foldedPaths.Add(entry.Name);
						}
						entry.Expanded = ex;
					}
					// show contents
					// ReSharper disable once AssignmentInConditionalExpression
					if (entry.Expanded)
					{
						using (new EditorGUILayout.HorizontalScope())
						{
							GUILayout.Space(10);
							using (new EditorGUILayout.VerticalScope())
							{
								DisplayList(entry.Children, window);
							}
						}
					}
				}
			}
		}

		private void DisplayAsset(HierarchyEntry entry, EditorWindow window)
		{
			Rect rect;
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(16);
				rect = GUILayoutUtility.GetRect(new GUIContent(entry.Name), GUI.skin.label, GUILayout.ExpandWidth(true));
			}

			var selected = entry == _hierarchy.SelectedAssetEntry;

			if (Event.current.type == EventType.Repaint)
			{
				if (_scrollToSelected && selected)
				{
					_scroll.y = rect.yMin - (window.position.height - 50) / 2;
					_scrollToSelected = false;
				}
			}

			if (rect.Contains(Event.current.mousePosition))
			{
				if (_selectionCallback != null)
				{
					if (Event.current.type == EventType.MouseDown && (_enableSelectionOnClick || Event.current.clickCount > 1))
					{
						_selectionCallback(entry.Asset);
                        if (_enableSelectionOnClick)
                        {
                            _hierarchy.SelectedAssetEntry = entry;
                        }
                        if (Event.current.clickCount > 1)
                        {
                            window.Close();
                        }
                        Event.current.Use();
					}
				}

				if (_selectionCallback == null && Event.current.type == EventType.MouseDrag)
				{
					DragAndDrop.PrepareStartDrag();
					if (!selected)
					{
						DragAndDrop.objectReferences = new[] {entry.Asset};
					}
					else
					{
						DragAndDrop.objectReferences = Selection.objects;
					}
					DragAndDrop.StartDrag("AssetPicker");
					Event.current.Use();
				}

				if (Event.current.isMouse && Event.current.button != 0)
				{
					return;
				}
			}

			var style = selected ? _styles.Selected : _styles.Object;
			if (GUI.RepeatButton(rect, entry.Name, style))
			{
				if (_selectionCallback == null)
				{
					if (Event.current.control)
					{
						Selection.objects = selected
							? Selection.objects.Where(o => o != entry.Asset).ToArray()
							: Selection.objects.Concat(new[] {entry.Asset}).ToArray();
					}
					else if (!Event.current.shift)
					{
						_hierarchy.SelectedAssetEntry = entry;
						OnAssetHover(false);
						window.Repaint();
					}
				}
				else
				{
					_hierarchy.SelectedAssetEntry = entry;
				}
			}
		}

		public void OnAssetHover(bool scrollToAsset = true)
		{
			if (_hierarchy.SelectedAssetEntry == null) return;

			if (scrollToAsset) _scrollToSelected = true;

			if (_selectionCallback == null)
			{
				Selection.activeObject = _hierarchy.SelectedAssetEntry.Asset;
			}

			if (_enableSelectionOnClick)
			{
				_selectionCallback?.Invoke(_hierarchy.SelectedAssetEntry.Asset);
			}
		}

		public void Serialize()
		{
			var folded = EditorPrefs.GetString(HierarchyPrefsKey, "");
			_foldedPaths.UnionWith(folded.Split('\n'));
		}

		public void Deserialize()
		{
			EditorPrefs.SetString(HierarchyPrefsKey, string.Join("\n", _foldedPaths.ToArray()));
		}
    }
}
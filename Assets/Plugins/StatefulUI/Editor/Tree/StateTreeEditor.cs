using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using StatefulUI.Editor.ReferenceInspectors;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.States;
using UnityEditor;
using UnityEngine;

namespace StatefulUI.Editor.Tree
{
    public class StateTreeEditor : EditorWindow
    {
        public const int nodeWidth = 120;
        public const int nodeHeight = 40;
        public const int nodeTextPadding = 20;
        public const int padding = 20;
        
        public const int connectorWidth = 30;
        public const int connectorHeight = 14;
        
        private const float DragThreshold = 10;
        private const float ZoomMin = 0.4f;
        private const float ZoomMax = 1.0f;
        
        public StateReference State;
        
        private readonly List<Node> _nodes = new List<Node>();
        private readonly List<Node> _selectedNodes = new List<Node>();
        private readonly List<Connection> _connections = new List<Connection>();
        private readonly Rect _gridRect = new Rect(0, 0, 2000, 2000);

        private StateInspector _stateInspector; 

        [UsedImplicitly]
        private bool _showInspector;
        private float _zoom = ZoomMax;
        private GUIStyle _nodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _inPointStyle;
        private GUIStyle _outPointStyle;
        private ConnectionPoint _selectedInPoint;
        private ConnectionPoint _selectedOutPoint;
        private StatefulComponent _view;
        private Vector2 _zoomCoordsOrigin = Vector2.zero;
        private Vector2 _offset = Vector2.zero;
        private Node _lastSelectedNode;
        private Rect _zoomArea;
        private SerializedObject _stateObject;
        private SerializedProperty _roleProperty;
        
        private bool _isLeftButtonClicking;
        private bool _isRightButtonClicking;
        private bool _selectionShapeDrawing;
        
        private Vector2 _shapeLastPosition;
        private Vector2 _startRightDragPosition;
        private Vector2 _startLeftDragPosition;
        private Rect _lastInspectorRect;


        public static void OpenWindow(StatefulComponent StatefulComponent)
        {
            var window = GetWindow<StateTreeEditor>();
            window.Init(StatefulComponent);
            window.titleContent = new GUIContent("State Tree Editor");
        }

        protected void OnEnable()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.fontSize = 12;
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);
            _nodeStyle.alignment = TextAnchor.MiddleCenter;

            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            _selectedNodeStyle.normal.textColor = Color.white;
            _selectedNodeStyle.fontSize = 12;
            _selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            _selectedNodeStyle.alignment = TextAnchor.MiddleCenter;

            _inPointStyle = new GUIStyle();
            _inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
            _inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
            _inPointStyle.border = new RectOffset(4, 4, 12, 12);

            _outPointStyle = new GUIStyle();
            _outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
            _outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
            _outPointStyle.border = new RectOffset(4, 4, 12, 12);

            StatefulUiManager.Instance.ValidateStatefulComponent += OnValidateStatefulComponent;
        }

        private void OnValidateStatefulComponent(StatefulComponent view)
        {
            if (view == _view)
            {
                Init(view);
            }
        }

        private void OnDisable()
        {
            StatefulUiManager.Instance.ValidateStatefulComponent -= OnValidateStatefulComponent;
        }

        private void Init(StatefulComponent StatefulComponent)
        {
            _zoom = ZoomMax;
            _zoomCoordsOrigin = _gridRect.center.ModifyX(-position.width / 2).ModifyY(-position.height / 2) ; 
            _offset = Vector2.zero;
            _view = StatefulComponent;
            _lastSelectedNode = null;
            _showInspector = false;
            State = null;
            
            _connections.Clear();
            _nodes.Clear();
            
            var nodePosition = _gridRect.center.ModifyX(-position.width / 2).ModifyY(-position.height / 4);
            
            foreach (var state in _view.States)
            {
                state.NodeData ??= new NodeData();
                
                if (nodePosition.x > _gridRect.center.x + position.width / 2 - nodeWidth - padding)
                {
                    nodePosition = nodePosition.ChangeX(_gridRect.center.x - position.width / 2).ModifyY(nodeHeight + padding);
                }
                
                if (state.NodeData.Position != Vector2.zero)
                {
                    nodePosition = state.NodeData.Position;
                }
                else
                {
                    state.NodeData.Position = nodePosition;
                }
                
                CreateNode(state, nodePosition);
                
                nodePosition = nodePosition.ModifyX(nodeWidth + padding);
            }
            
            foreach (var state in _view.States)
            {
                var stateNode = _nodes.Find(node => node.Conf.StateReference == state);

                foreach (var parentRole in state.NodeData.ParentRoles)
                {
                    var parentNode = _nodes.Find(node => node.Conf.StateReference.Role == parentRole);

                    if (stateNode != null && parentNode != null)
                    {
                        _connections.Add(new Connection(stateNode.inPoint, parentNode.outPoint, OnClickRemoveConnection));
                    }
                }
            }
        }

        protected void OnGUI()
        {
            if (Reinit())
            {
                _showInspector = false;
                DrawStateInspector();
                return;
            }

            try
            {
                DrawToolbar();
                DrawZoomArea();
                HandleEvents();
                DrawInspector();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void DrawInspector()
        {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    DrawStateInspector();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStateInspector()
        {
            var rect = EditorGUILayout.BeginVertical("box");

            if (rect.width > 0)
            {
                _lastInspectorRect = rect;    
            }

            if (_showInspector)
            {
                _stateObject.UpdateIfRequiredOrScript();
                EditorGUILayout.PropertyField(_roleProperty, GUILayout.MinWidth(300));
                _stateInspector?.OnInspectorGUI();
                EditorGUILayout.Space(12);
                _stateObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear node data", EditorStyles.toolbarButton))
                {
                    var clearSelected = EditorUtility.DisplayDialog("Clear the state tree?",
                        "Connections between states will be removed, the states themselves will not change.", 
                        "Clear", 
                        "Cancel");
                    
                    if (clearSelected)
                    {
                        foreach (var stateReference in _view.States)
                        {
                            stateReference.NodeData = null;
                        }

                        Init(_view);

                        EditorUtility.SetDirty(_view);
                    }
                }
                
                if (GUILayout.Button("Close inspector", EditorStyles.toolbarButton))
                {
                    _nodes.ForEach(node => node.isSelected = false);
                    _showInspector = false;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        public void Update()
        {
            Repaint();
        }

        private void DrawZoomArea()
        {
            _zoomArea = new Rect(0, 0, position.width, position.height);

            EditorZoomArea.Begin(_zoom, _zoomArea);

            var screenRect = new Rect(
                -_zoomCoordsOrigin.x,
                -_zoomCoordsOrigin.y,
                position.width / Mathf.Min(1, _zoom) + _zoomCoordsOrigin.x,
                position.height / Mathf.Min(1, _zoom) + _zoomCoordsOrigin.y);

            GUILayout.BeginArea(screenRect);
            {
                DrawGrid(20, 0.2f, Color.gray, _gridRect);
                DrawGrid(100, 0.4f, Color.gray, _gridRect);

                DrawNodes();
                DrawConnections();
                DrawConnectionLine(Event.current);
                DrawSelectionShape();
            }
            GUILayout.EndArea();

            EditorZoomArea.End();
        }

        private void DrawSelectionShape()
        {
            if (_selectionShapeDrawing)
            {
                Handles.DrawSolidRectangleWithOutline(
                    GetShapeSelectionRect(), 
                    new Color(0.44f, 0.41f, 0.54f, 0.07f), 
                    new Color(0.9f, 0.9f, 0.9f, 0.8f)
                );
            }
        }

        private void CreateNode(StateReference state, Vector2 nodePosition)
        {
            var conf = new NodeConfig
            {
                StateReference = state,
                Position = nodePosition,
                Width = nodeWidth,
                Height = nodeHeight,
                NodeStyle = _nodeStyle,
                SelectedStyle = _selectedNodeStyle,
                InPointStyle = _inPointStyle,
                OutPointStyle = _outPointStyle,
                OnClickInPoint = OnClickInPoint,
                OnClickOutPoint = OnClickOutPoint,
                OnClickRemoveNode = OnClickRemoveNode,
                OnDuplicateNode = OnClickDuplicateNode,
            };

            _nodes.Add(new Node(conf));
        }

        private bool Reinit()
        {
            StatefulComponent StatefulComponent = null;
            if (Selection.activeGameObject != null)
            {
                StatefulComponent = Selection.activeGameObject.GetComponent<StatefulComponent>();
            }

            if (_view == null || _nodes == null)
            {
                if (StatefulComponent != null)
                {
                    Init(StatefulComponent);
                }

                return true;
            }

            if (StatefulComponent != null && _view != StatefulComponent)
            {
                Init(StatefulComponent);
            }

            return false;
        }

        private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
        {
            return (screenCoords - _zoomArea.TopLeft()) / _zoom + _zoomCoordsOrigin;
        }

        private void HandleEvents()
        {
            var evnt = Event.current;

            if (_lastInspectorRect.Contains(evnt.mousePosition)) return;

            ProcessNodesEvents(evnt);
            ProcessEvents(evnt);
            ProcessWorkspaceEvents(evnt);
            
            if (evnt.type == EventType.ScrollWheel)
            {
                var screenCoordsMousePos = evnt.mousePosition;
                var delta = evnt.delta;
                var zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
                var zoomDelta = -delta.y / 150.0f;
                var oldZoom = _zoom;
                _zoom += zoomDelta;
                _zoom = Mathf.Clamp(_zoom, ZoomMin, ZoomMax);
                _zoomCoordsOrigin += zoomCoordsMousePos - _zoomCoordsOrigin - oldZoom / _zoom * (zoomCoordsMousePos - _zoomCoordsOrigin);
            }
        }

        private void ProcessWorkspaceEvents(Event evnt)
        {
            if (evnt.button == 0)
            {
                var mousePosition = evnt.mousePosition / _zoom + _zoomCoordsOrigin;
                switch (evnt.type)
                {
                    case EventType.MouseDown:
                    {
                        _isLeftButtonClicking = true;
                        _startLeftDragPosition = evnt.mousePosition;
                        Node newSelectedNode = null;

                        foreach (var node in _nodes)
                        {
                            if (node.rect.Contains(mousePosition) && !node.isSelected)
                            {
                                newSelectedNode = node;
                            }
                        }
                        
                        if (newSelectedNode != null)
                        {
                            if (evnt.control || evnt.command)
                            {
                                newSelectedNode.SetSelected(true);
                            }
                            else
                            {
                                foreach (var node in _nodes)
                                {
                                    node.SetSelected(node == newSelectedNode);
                                }    
                            }
                        }

                        break;
                    }
                    case EventType.MouseDrag:
                    {
                        if (_isLeftButtonClicking && (_startLeftDragPosition - evnt.mousePosition).magnitude > DragThreshold)
                        {
                            _isLeftButtonClicking = false;
                        }

                        if (!_isLeftButtonClicking && !_selectionShapeDrawing)
                        {
                            _selectionShapeDrawing = true;
                        }

                        if (_selectionShapeDrawing)
                        {
                            _shapeLastPosition = evnt.mousePosition;
                        }

                        break;
                    }
                    case EventType.MouseUp:
                    {
                        if (_isLeftButtonClicking)
                        {
                            _isLeftButtonClicking = false;

                            OnLeftMouseClick(mousePosition);
                        }

                        if (_selectionShapeDrawing)
                        {
                            _selectionShapeDrawing = false;
                        }

                        break;
                    }
                }
            }

            if (evnt.button == 1 || evnt.button == 2 && !_isRightButtonClicking)
            {
                switch (evnt.type)
                {
                    case EventType.MouseDrag:
                        var delta = evnt.delta;
                        delta /= _zoom;
                        _zoomCoordsOrigin -= delta;
                        break;
                    case EventType.MouseDown:
                        ClearConnectionSelection();
                        break;
                    case EventType.MouseUp:
                        EditorUtility.SetDirty(_view);
                        break;
                }
            }

            if (_selectionShapeDrawing)
            {
                if (!_zoomArea.Contains(evnt.mousePosition))
                {
                    _selectionShapeDrawing = false;
                }

                var shape = GetShapeSelectionRect();
                foreach (var node in _nodes)
                {
                    node.SetSelected(shape.Overlaps(node.rect));
                }
            }
        }

        private void OnLeftMouseClick(Vector2 mousePosition)
        {
            ClearConnectionSelection();
            
            var hasGroupSelectionDragging = false;

            foreach (var selectedNode in _selectedNodes)
            {
                if (selectedNode.rect.Contains(mousePosition))
                {
                    hasGroupSelectionDragging = true;
                }
            }

            if (!hasGroupSelectionDragging)
            {
                foreach (var node in _nodes)
                {
                    node.SetSelected(node.rect.Contains(mousePosition));
                }
            }
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Rect mainRect)
        {
            var widthDivs = Mathf.CeilToInt(mainRect.width / gridSpacing);
            var heightDivs = Mathf.CeilToInt(mainRect.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            var newOffset = new Vector3(-_offset.x % gridSpacing, -_offset.y % gridSpacing, 0);
            
            for (var i = 0; i < widthDivs; i++)
            {
                var from = new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset;
                var to = new Vector3(gridSpacing * i, mainRect.height, 0f) + newOffset;
                Handles.DrawLine(from, to);
            }

            for (var j = 0; j < heightDivs; j++)
            {
                var from = new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset;
                var to = new Vector3(mainRect.width, gridSpacing * j, 0f) + newOffset;
                Handles.DrawLine(from, to);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawConnectionLine(Event e)
        {
            if (_selectedInPoint != null && _selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    _selectedInPoint.Rect.center,
                    e.mousePosition,
                    _selectedInPoint.Rect.center + Vector2.down * 50f,
                    e.mousePosition - Vector2.down * 50f,
                    Color.white,
                    null,
                    5f
                );

                GUI.changed = true;
            }

            if (_selectedOutPoint != null && _selectedInPoint == null)
            {
                Handles.DrawBezier(
                    _selectedOutPoint.Rect.center,
                    e.mousePosition,
                    _selectedOutPoint.Rect.center - Vector2.down * 50f,
                    e.mousePosition + Vector2.down * 50f,
                    Color.white,
                    null,
                    5f
                );

                GUI.changed = true;
            }
        }

        private void DrawConnections()
        {
            if (_connections == null) return;
            for (var i = 0; i < _connections.Count; i++)
            {
                _connections[i].Draw();
            }
        }

        private void DrawNodes()
        {
            if (_nodes == null) return;
            
            for (var index = 0; index < _nodes.Count; index++)
            {
                _nodes[index].Draw();
            }
        }

        private void ProcessNodesEvents(Event evnt)
        {
            if (_nodes == null) return;

            _selectedNodes.Clear();
            
            foreach (var node in _nodes)
            {
                if (node.isSelected)
                {
                    _selectedNodes.Add(node);
                }
            }
            
            if (_selectedNodes.Count != 1)
            {
                _showInspector = false;
            }

            for (var index = _nodes.Count - 1; index >= 0; index--)
            {
                PrecessNodeEvent(evnt, index);
            }
        }

        private void PrecessNodeEvent(Event evnt, int index)
        {
            var node = _nodes[index];
            var info = GetNodeClickInfo(evnt, node);

            if (info.DragDelta != Vector2.zero)
            {
                foreach (var otherNode in _nodes)
                {
                    if (otherNode.isSelected && otherNode != node)
                    {
                        otherNode.Drag(info.DragDelta);
                    }
                }
            }

            if (node.isSelected && _selectedNodes.Count == 1)
            {
                _lastSelectedNode = node;

                if (State != _lastSelectedNode.Conf.StateReference)
                {
                    State = _lastSelectedNode.Conf.StateReference;
                    _stateObject = new SerializedObject(_view);
                    var statesProperty = _stateObject.FindProperty(nameof(StatefulComponent.States));
                    var stateProperty = statesProperty.GetArrayElementAtIndex(index);
                    _roleProperty = stateProperty.FindPropertyRelative(nameof(StateReference.Role));
                    _stateInspector = new StateInspector(stateProperty);
                }

                _showInspector = true;

                if (info.IsDoubleClick)
                {
                    _view.ApplyState(State);
                    SceneView.RepaintAll();
                    VisualizeStateApplying();
                }
            }

            if (info.guiChanged)
            {
                GUI.changed = true;
            }
        }

        private void VisualizeStateApplying()
        {
            var list = NodesSearcher.GetParents(State, _view).ToList();
            list.Add(State);
            var delay = 0f;

            foreach (var stateReference in list)
            {
                _nodes.Find(item => item.Conf.StateReference == stateReference).Pulse(delay);
                delay += 0.08f;
            }
        }

        private Node.NodeClickInfo GetNodeClickInfo(Event evnt, Node node)
        {
            var info = new Node.NodeClickInfo();
            var isLeftButton = evnt.button == 0;
            var isRightButton = evnt.button == 1;
            var mousePosition = evnt.mousePosition / _zoom + _zoomCoordsOrigin;

            switch (evnt.type)
            {
                case EventType.MouseDown:
                {
                    if (isLeftButton && node.rect.Contains(mousePosition))
                    {
                        if (node.isSelected && EditorApplication.timeSinceStartup - node.LastClickTime < 0.5f)
                        {
                            info.IsDoubleClick = true;
                        }

                        node.isDragged = true;
                        node.LastClickTime = EditorApplication.timeSinceStartup;
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    node.isDragged = false;

                    if (_isRightButtonClicking && isRightButton && node.rect.Contains(mousePosition))
                    {
                        node.ProcessContextMenu();
                        evnt.Use();
                    }

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (isLeftButton && node.isDragged)
                    {
                        info.DragDelta = evnt.delta / _zoom;
                        node.Drag(info.DragDelta);
                        evnt.Use();
                        info.guiChanged = true;
                    }

                    break;
                }
            }

            return info;
        }

        private void ProcessEvents(Event evnt)
        {
            var isRightButton = evnt.button == 1;
            
            switch (evnt.type)
            {
                case EventType.MouseUp:
                    if (isRightButton && _isRightButtonClicking)
                    {
                        _isRightButtonClicking = false;
                        ProcessContextMenu(evnt.mousePosition);
                    }
                    break;
                    
                case EventType.MouseDown:
                    if (isRightButton)
                    {
                        _isRightButtonClicking = true;
                        _startRightDragPosition = evnt.mousePosition;
                    }
                    break;

                case EventType.MouseDrag:
                    if (isRightButton 
                        && _isRightButtonClicking 
                        && (_startRightDragPosition - evnt.mousePosition).magnitude > DragThreshold)
                    {
                        _isRightButtonClicking = false;    
                    }
                    break;
            }
        }

        private void OnClickAddNode(Vector2 mousePosition)
        {
            var stateReference = new StateReference
            {
                Role = 0,
                Description = new List<StateDescription>(),
                NodeData = new NodeData(),
            };
            
            _view.States.Add(stateReference);
            
            CreateNode(stateReference, mousePosition / _zoom + _zoomCoordsOrigin);
            EditorUtility.SetDirty(_view);
        }

        private void OnClickInPoint(ConnectionPoint inPoint)
        {
            _selectedInPoint = inPoint;

            if (_selectedOutPoint == null) return;
            
            if (_selectedOutPoint.Node != _selectedInPoint.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }

        private void OnClickOutPoint(ConnectionPoint outPoint)
        {
            _selectedOutPoint = outPoint;

            if (_selectedInPoint == null) return;
            
            if (_selectedOutPoint.Node != _selectedInPoint.Node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }

        private void OnClickRemoveConnection(Connection connection)
        {
            _connections.Remove(connection);
            connection.inPoint.Node.Conf.StateReference.NodeData.ParentRoles
                .Remove(connection.outPoint.Node.Conf.StateReference.Role);
        }

        private void CreateConnection()
        {
            var conn = _connections.Find(connection => connection.inPoint == _selectedInPoint
                                                       && connection.outPoint == _selectedOutPoint);
            if (conn != null) return;
            
            _connections.Add(new Connection(_selectedInPoint, _selectedOutPoint, OnClickRemoveConnection));

            _selectedInPoint.Node.Conf.StateReference.NodeData.ParentRoles
                .Add(_selectedOutPoint.Node.Conf.StateReference.Role);

            EditorUtility.SetDirty(_view);
        }

        private void ClearConnectionSelection()
        {
            _selectedInPoint = null;
            _selectedOutPoint = null;
        }

        private void OnClickRemoveNode(Node node)
        {
            if (_connections != null)
            {
                var connectionsToRemove = new List<Connection>();

                for (var i = 0; i < _connections.Count; i++)
                {
                    if (_connections[i].inPoint == node.inPoint || _connections[i].outPoint == node.outPoint)
                    {
                        connectionsToRemove.Add(_connections[i]);
                    }
                }

                for (var i = 0; i < connectionsToRemove.Count; i++)
                {
                    _connections.Remove(connectionsToRemove[i]);
                }
            }

            _nodes.Remove(node);

            _view.States.Remove(node.Conf.StateReference);
            EditorUtility.SetDirty(_view);
        }

        private void OnClickDuplicateNode(Node node)
        {
            var state = node.Conf.StateReference;
            var description = state.Description;
            var newDescription = new List<StateDescription>();
            
            foreach (var stateDescription in description)
            {
                var item = JsonUtility.FromJson<StateDescription>(JsonUtility.ToJson(stateDescription));
                newDescription.Add(item);
            }
            
            var stateReference = new StateReference
            {
                Role = state.Role,
                Description = newDescription,
                NodeData = new NodeData(),
            };
            
            _view.States.Add(stateReference);
            var nodePosition = state.NodeData.Position.ModifyX(nodeWidth + padding).ModifyY(nodeHeight + padding);
            CreateNode(stateReference, nodePosition);
            EditorUtility.SetDirty(_view);
        }

        private Rect GetShapeSelectionRect()
        {
            var shapeLastPosition = _shapeLastPosition / Mathf.Min(1, _zoom);
            var shapeStartPosition = _startLeftDragPosition / Mathf.Min(1, _zoom);
            var rect = new Rect(shapeStartPosition  + _zoomCoordsOrigin, shapeLastPosition - shapeStartPosition);

            return rect.Abs();
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add State"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }
    }
}

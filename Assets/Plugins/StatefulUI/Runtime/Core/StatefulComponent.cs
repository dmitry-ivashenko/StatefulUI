using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using StatefulUI.Runtime.Localization;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.States;
using UnityEngine;
using UnityEngine.UI;

namespace StatefulUI.Runtime.Core
{
    public sealed partial class StatefulComponent : MonoBehaviour
    {
        // ReSharper disable once EventNeverSubscribedTo.Global
        public event Action<string> AnimationEvent;

        // ReSharper disable once UnusedMember.Global
        public RectTransform RectTransform => transform as RectTransform;
        
        public List<StatePhraseReference> PhrasesFromStates { get; } = new List<StatePhraseReference>();
        
        public List<ButtonReference> Buttons;
        public List<TextReference> Texts;
        public List<AnimatorReference> Animators;
        public List<ContainerReference> Containers;
        public List<SliderReference> Sliders;
        public List<ToggleReference> Toggles;
        public List<InnerComponentReference> InnerComponents;
        public List<ImageReference> Images;
        public List<VideoPlayerReference> VideoPlayers;
        public List<DropdownReference> Dropdowns;
        public List<TextInputReference> TextsInputs;
        public List<ObjectReference> Objects;
        public List<StateReference> States;
        
        public bool LocalizeOnEnable = true;
        public bool ApplyInitialStateOnEnable;

        [SerializeField]
        private StateReference _initialState;
        
        public List<int> StateHistory { get; set; } = new List<int>();
        public StateReference InitialState => _initialState;
        
        private StateProcessor _stateProcessor;
        
        private void Awake()
        {
            _stateProcessor ??= new StateProcessor(this);
        }

        private void OnEnable()
        {
            _stateProcessor ??= new StateProcessor(this);

            if (Application.isPlaying)
            {
                if (LocalizeOnEnable)
                {
                    Localize();
                }
                
                if (ApplyInitialStateOnEnable)
                {
                    ApplyState(_initialState);
                }
            }
        }

        public void Update()
        {
            _stateProcessor?.OnUpdate();
        }

        private void OnDestroy()
        {
            _stateProcessor = null;
        }

        public void Localize()
        {
            foreach (var reference in Texts)
            {
                if (reference.Identificator.IsNonEmpty() && !reference.IsEmpty && reference.Localize)
                {
                    reference.SetText(LocalizationUtils.GetPhrase(reference.Identificator, "---"));
                }
            }
        }
        
        [UsedImplicitly]
        private void OnAnimatorEvent(string eventName)
        {
            AnimationEvent?.Invoke(eventName);
        }

        public string CopyLocalization()
        {
            var result = "";
            
            foreach (var text in Texts)
            {
                if (text.Identificator != string.Empty && (text.Text != null || text.TMP != null) && text.Localize)
                {
                    result += $"\"{text.Identificator}\"\t\"{text.Value}\"\n";
                }
            }
            
            foreach (var InnerComponent in InnerComponents)
            {
                result += InnerComponent.InnerComponent.CopyLocalization();
            }
            
            foreach (var containerReference in Containers)
            {
                if (containerReference.Prefab.TryGetComponent<StatefulComponent>(out var view))
                {
                    result += view.CopyLocalization();
                }
            }
            
            foreach (var reference in PhrasesFromStates)
            {
                result += $"\"{reference.Identificator}\"\t\"{reference.Value}\"\n";
            }

            return result;
        }

        public bool HasState(int role)
        {
            foreach (var state in States)
            {
                if (state.Role == role)
                {
                    return true;
                }
            }

            return false;
        }

        public float SetState(int role)
        {
            foreach (var state in States)
            {
                if (state.Role == role)
                {
                    return ApplyState(state);
                }
            }

            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.StateRoleType, role);
            Debug.LogError($"View {name} does not contain state with role {roleName}, scene path: {go.GetScenePath()}", go);

            return 0f;
        }
        
        public float ApplyState(StateReference state)
        {
            StateHistory.Add(state.Role);
            var duration = _stateProcessor.Apply(state);
            return duration;
        }

        private ButtonReference GetButton(int role)
        {
            for (var i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i].Role == role)
                {
                    return Buttons[i];
                }
            }

            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.ButtonRoleType, role);
            Debug.LogError($"View {name} does not contain button with role {roleName}, scene path: {go.GetScenePath()}", go);
            
            return null;
        }
        
        private ObjectReference GetObject(int role)
        {
            for (var i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Role == role)
                {
                    return Objects[i];
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.ObjectRoleType, role);
            Debug.LogError($"View {name} does not contain object with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        [UsedImplicitly]
        private Image GetImage(int role)
        {
            for (var i = 0; i < Images.Count; i++)
            {
                if (Images[i].Role == role)
                {
                    return Images[i].Image;
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.ImageRoleType, role);
            Debug.LogError($"View {name} does not contain image with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        private ImageReference GetImageReference(int role)
        {
            for (var i = 0; i < Images.Count; i++)
            {
                if (Images[i].Role == role)
                {
                    return Images[i];
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.ImageRoleType, role);
            Debug.LogError($"View {name} does not contain image with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }
        
        public void SetImageByRawRole(int role, Sprite sprite)
        {
            if (!HasImage(role))
            {
                var go = gameObject;
                var roleName = RoleUtils.GetName(RoleUtils.ImageRoleType, role);
                Debug.LogError($"View {name} does not contain image with role {roleName}, scene path: {go.GetScenePath()}", go);
                return;
            }
            
            for (var i = 0; i < Images.Count; i++)
            {
                if (Images[i].Role == role)
                {
                    Images[i].Image.sprite = sprite;
                }
            }
        }
        
        public void SetImageByRawRole(int role, string spritePath)
        {
            if (!HasImage(role))
            {
                var go = gameObject;
                var roleName = RoleUtils.GetName(RoleUtils.ImageRoleType, role);
                Debug.LogError($"View {name} does not contain image with role {roleName}, scene path: {go.GetScenePath()}", go);
                return;
            }

            if (spritePath.TryLoadSprite(out var sprite))
            {
                SetImageByRawRole(role, sprite);    
            }
            else
            {
                var go = gameObject;
                var roleName = RoleUtils.GetName(RoleUtils.ImageRoleType, role);
                Debug.LogError($"Can't load a sprite {spritePath} for an image with a role {roleName}, scene path: {go.GetScenePath()}", go);
            }
        }
        
        private AnimatorReference GetAnimator(int role)
        {
            for (var i = 0; i < Animators.Count; i++)
            {
                if (Animators[i].Role == role)
                {
                    return Animators[i];
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.AnimatorRoleType, role);
            Debug.LogError($"View {name} does not contain animator with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        public bool TryGetContainer(int role, out ContainerReference view)
        {
            view = null;
            
            for (var i = 0; i < Containers.Count; i++)
            {
                if (Containers[i].Role == role)
                {
                    view = Containers[i];
                    return true;
                }
            }

            return false;
        }

        private ContainerReference GetContainer(int role)
        {
            if (!TryGetContainer(role, out var view))
            {
                var go = gameObject;
                var roleName = RoleUtils.GetName(RoleUtils.ContainerRoleType, role);
                Debug.LogError($"View {name} does not contain container with role {roleName}, scene path: {go.GetScenePath()}", go);
            }

            return view;
        }

        private SliderReference GetSlider(int role)
        {
            for (var i = 0; i < Sliders.Count; i++)
            {
                if (Sliders[i].Role == role)
                {
                    return Sliders[i];
                }
            }

            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.SliderRoleType, role);
            Debug.LogError($"View {name} does not contain slider with role {roleName}, scene path: {go.GetScenePath()}", go);
            
            return null;
        }

        private Toggle GetToggle(int role)
        {
            for (var i = 0; i < Toggles.Count; i++)
            {
                if (Toggles[i].Role == role)
                {
                    return Toggles[i].Toggle;
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.ToggleRoleType, role);
            Debug.LogError($"View {name} does not contain toggle with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        private InnerComponentReference GetInnerComponent(int role)
        {
            for (var i = 0; i < InnerComponents.Count; i++)
            {
                if (InnerComponents[i].Role == role)
                {
                    return InnerComponents[i];
                }
            }

            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.InnerComponentRoleType, role);
            Debug.LogError($"View {name} does not contain inner reference view with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        private TextReference GetText(int role)
        {
            for (var i = 0; i < Texts.Count; i++)
            {
                if (Texts[i].Role == role)
                {
                    return Texts[i];
                }
            }
            
            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.TextRoleType, role);
            Debug.LogError($"View {name} does not contain text with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        public void SetRawTextByRole(int role, object text) => SetText(role, text);

        public void SetRawTextValuesByRole(int role, params object[] args)
        {
            foreach (var reference in Texts)
            {
                if (reference.Role == role && reference.Identificator.IsNonEmpty())
                {
                    reference.SetText(string.Format(LocalizationUtils.GetPhrase(reference.Identificator, "---"), args));
                }
            }
        }

        private void SetText(int role, object text)
        {
            for (var i = 0; i < Texts.Count; i++)
            {
                if (Texts[i].Role == role)
                {
                    Texts[i].SetText(text);
                }
            }
        }
        
        private TextInputReference GetTextInput(int role)
        {
            for (var i = 0; i < TextsInputs.Count; i++)
            {
                if (TextsInputs[i].Role == role)
                {
                    return TextsInputs[i];
                }
            }

            return null;
        }
        
        private DropdownReference GetDropdown(int role)
        {
            for (var i = 0; i < Dropdowns.Count; i++)
            {
                if (Dropdowns[i].Role == role)
                {
                    return Dropdowns[i];
                }
            }

            return null;
        }
        
        private VideoPlayerReference GetVideoPlayer(int role)
        {
            for (var i = 0; i < VideoPlayers.Count; i++)
            {
                if (VideoPlayers[i].Role == role)
                {
                    return VideoPlayers[i];
                }
            }

            var go = gameObject;
            var roleName = RoleUtils.GetName(RoleUtils.VideoPlayerRoleType, role);
            Debug.LogError($"View {name} does not contain video player with role {roleName}, scene path: {go.GetScenePath()}", go);

            return null;
        }

        public T GetItem<T>(int roleValue) where T : class
        {
            var type = typeof(T);
            var manager = StatefulUiManager.Instance;
            
            if (type == manager.AnimatorReferenceType) return GetAnimator(roleValue) as T;
            if (type == manager.ButtonReferenceType) return GetButton(roleValue) as T;
            if (type == manager.ContainerReferenceType) return GetContainer(roleValue) as T;
            if (type == manager.DropdownReferenceType) return GetDropdown(roleValue) as T;
            if (type == manager.ImageReferenceType) return GetImageReference(roleValue) as T;
            if (type == manager.InnerComponentReferenceType) return GetInnerComponent(roleValue) as T;
            if (type == manager.ObjectReferenceType) return GetObject(roleValue) as T;
            if (type == manager.SliderReferenceType) return GetSlider(roleValue) as T;
            if (type == manager.TextInputReferenceType) return GetTextInput(roleValue) as T;
            if (type == manager.TextReferenceType) return GetText(roleValue) as T;
            if (type == manager.ToggleReferenceType) return GetToggle(roleValue) as T;
            if (type == manager.VideoPlayerReferenceType) return GetVideoPlayer(roleValue) as T;

            throw new Exception($"Type {type} is not supported");
        }

        public bool HasItem<T>(int roleValue) where T : class
        {
            var type = typeof(T);
            var manager = StatefulUiManager.Instance;

            if (type == manager.ButtonReferenceType) return HasButton(roleValue);
            if (type == manager.ImageReferenceType) return HasImage(roleValue);
            if (type == manager.AnimatorReferenceType) return HasAnimator(roleValue);
            if (type == manager.ContainerReferenceType) return HasContainer(roleValue);
            if (type == manager.DropdownReferenceType) return HasDropdown(roleValue);
            if (type == manager.InnerComponentReferenceType) return HasInnerComponent(roleValue);
            if (type == manager.ObjectReferenceType) return HasObject(roleValue);
            if (type == manager.SliderReferenceType) return HasSlider(roleValue);
            if (type == manager.TextInputReferenceType) return HasTextInput(roleValue);
            if (type == manager.TextReferenceType) return HasText(roleValue);
            if (type == manager.ToggleReferenceType) return HasToggle(roleValue);
            if (type == manager.VideoPlayerReferenceType) return HasVideoPlayer(roleValue);

            throw new Exception($"Type {type} is not supported");
        }
        
        public bool DropItem<T>(T reference) where T : class
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Drop item");
#endif
            
            var type = typeof(T);
            var manager = StatefulUiManager.Instance;
            bool result;

            if (type == manager.ButtonReferenceType) result = Buttons.Remove(reference as ButtonReference);
            else if (type == manager.ImageReferenceType) result = Images.Remove(reference as ImageReference);
            else if (type == manager.AnimatorReferenceType) result = Animators.Remove(reference as AnimatorReference);
            else if (type == manager.ContainerReferenceType) result = Containers.Remove(reference as ContainerReference);
            else if (type == manager.DropdownReferenceType) result = Dropdowns.Remove(reference as DropdownReference);
            else if (type == manager.InnerComponentReferenceType) result = InnerComponents.Remove(reference as InnerComponentReference);
            else if (type == manager.ObjectReferenceType) result = Objects.Remove(reference as ObjectReference);
            else if (type == manager.SliderReferenceType) result = Sliders.Remove(reference as SliderReference);
            else if (type == manager.TextInputReferenceType) result = TextsInputs.Remove(reference as TextInputReference);
            else if (type == manager.TextReferenceType) result = Texts.Remove(reference as TextReference);
            else if (type == manager.ToggleReferenceType) result = Toggles.Remove(reference as ToggleReference);
            else if (type == manager.VideoPlayerReferenceType) result = VideoPlayers.Remove(reference as VideoPlayerReference);
            else return false;
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            OnValidate();
#endif

            return result;
        }
        
        private bool HasVideoPlayer(int role)
        {
            foreach (var videoPlayerReference in VideoPlayers)
            {
                if (videoPlayerReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasToggle(int role)
        {
            foreach (var toggleReference in Toggles)
            {
                if (toggleReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasText(int role)
        {
            foreach (var textReference in Texts)
            {
                if (textReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasTextInput(int role)
        {
            foreach (var textInputReference in TextsInputs)
            {
                if (textInputReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasSlider(int role)
        {
            foreach (var sliderReference in Sliders)
            {
                if (sliderReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasObject(int role)
        {
            foreach (var objectReference in Objects)
            {
                if (objectReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasInnerComponent(int role)
        {
            foreach (var InnerComponentReference in InnerComponents)
            {
                if (InnerComponentReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasDropdown(int role)
        {
            foreach (var dropdownReference in Dropdowns)
            {
                if (dropdownReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasContainer(int role)
        {
            foreach (var containerReference in Containers)
            {
                if (containerReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnimator(int role)
        {
            foreach (var animatorReference in Animators)
            {
                if (animatorReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasButton(int role)
        {
            foreach (var button in Buttons)
            {
                if (button.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasImage(int role)
        {
            foreach (var imageReference in Images)
            {
                if (imageReference.Role == role)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

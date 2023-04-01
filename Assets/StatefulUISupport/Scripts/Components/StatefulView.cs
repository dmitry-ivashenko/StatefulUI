// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
namespace StatefulUISupport.Scripts.Components
{
    public abstract class StatefulView : MonoBehaviour, IStatefulView
    {
        [SerializeField] private StatefulComponent _statefulComponent;

        public StatefulComponent StatefulComponent 
            => _statefulComponent;

        public StatefulComponent GetInnerComponent(InnerComponentRole role) 
            => _statefulComponent.GetInnerComponent(role);

        public StatefulComponent Get(InnerComponentRole role) 
            => _statefulComponent.GetInnerComponent(role);

        public ContainerView GetContainer(ContainerRole role) 
            => _statefulComponent.GetContainer(role).Container;

        public ContainerView Get(ContainerRole role) 
            => _statefulComponent.GetContainer(role).Container;

        public Toggle GetToggle(ToggleRole role) 
            => _statefulComponent.GetToggle(role).Toggle;

        public Toggle Get(ToggleRole role) 
            => _statefulComponent.GetToggle(role).Toggle;

        public Slider GetSlider(SliderRole role) 
            => _statefulComponent.GetSlider(role).Slider;

        public Slider Get(SliderRole role) 
            => _statefulComponent.GetSlider(role).Slider;

        public Button GetButton(ButtonRole role) 
            => _statefulComponent.GetButton(role).Button;

        public Button Get(ButtonRole role) 
            => _statefulComponent.GetButton(role).Button;

        public GameObject GetObject(ObjectRole role) 
            => _statefulComponent.GetObject(role).Object;

        public GameObject Get(ObjectRole role) 
            => _statefulComponent.GetObject(role).Object;

        public Image GetImage(ImageRole role) 
            => _statefulComponent.GetImage(role).Image;

        public Image Get(ImageRole role) 
            => _statefulComponent.GetImage(role).Image;

        public Animator GetAnimator(AnimatorRole role) 
            => _statefulComponent.GetAnimator(role).Animator;

        public VideoPlayer GetVideoPlayer(VideoPlayerRole role) 
            => _statefulComponent.GetVideoPlayer(role).VideoPlayer;

        public VideoPlayer Get(VideoPlayerRole role) 
            => _statefulComponent.GetVideoPlayer(role).VideoPlayer;

        public TextReference GetText(TextRole role) 
            => _statefulComponent.GetText(role);

        public TextReference Get(TextRole role) 
            => _statefulComponent.GetText(role);

        public TextInputReference GetTextInput(TextInputRole role) 
            => _statefulComponent.GetTextInput(role);

        public TextInputReference Get(TextInputRole role) 
            => _statefulComponent.GetTextInput(role);

        public DropdownReference GetDropdown(DropdownRole role) 
            => _statefulComponent.GetDropdown(role);

        public DropdownReference Get(DropdownRole role) 
            => _statefulComponent.GetDropdown(role);

        public bool HasState(StateRole role) 
            => _statefulComponent.HasState((int) role);

        public float SetState(StateRole role) 
            => _statefulComponent.SetState((int) role);

        public bool HasAnimator(AnimatorRole role) 
            => _statefulComponent.HasAnimator(role);

        public bool HasVideoPlayer(VideoPlayerRole role) 
            => _statefulComponent.HasVideoPlayer(role);

        public bool HasText(TextRole role) 
            => _statefulComponent.HasText(role);

        public bool HasTextInput(TextInputRole role) 
            => _statefulComponent.HasTextInput(role);

        public bool HasDropdown(DropdownRole role) 
            => _statefulComponent.HasDropdown(role);

        public bool HasToggle(ToggleRole role) 
            => _statefulComponent.HasToggle(role);

        public bool HasSlider(SliderRole role) 
            => _statefulComponent.HasSlider(role);

        public bool HasButton(ButtonRole role) 
            => _statefulComponent.HasButton(role);

        public bool HasObject(ObjectRole role) 
            => _statefulComponent.HasObject(role);

        public bool HasImage(ImageRole role) 
            => _statefulComponent.HasImage(role);

        public bool HasContainer(ContainerRole role) 
            => _statefulComponent.HasContainer(role);

        public bool HasInnerComponent(InnerComponentRole role) 
            => _statefulComponent.HasInnerComponent(role);

        public void SetText(TextRole role, object text) 
            => _statefulComponent.SetText(role, text);

        public void SetTextValues(TextRole role, params object[] values) 
            => _statefulComponent.SetTextValues(role, values);

        public void SetImage(ImageRole role, string spritePath) 
            => _statefulComponent.SetImage(role, spritePath);

        public void SetImage(ImageRole role, Sprite sprite) 
            => _statefulComponent.SetImage(role, sprite);

        private void OnValidate() 
            => _statefulComponent ??= GetComponent<StatefulComponent>();

    }
}

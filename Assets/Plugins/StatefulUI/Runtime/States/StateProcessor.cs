using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.Localization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StatefulUI.Runtime.States
{
    public class StateProcessor
    {
        private readonly StatefulComponent _view;
        private readonly List<StateAnimationInfo> _animations = new List<StateAnimationInfo>();
        private readonly List<StateReference> _nodes = new List<StateReference>();

        public StateProcessor(StatefulComponent StatefulComponent)
        {
            _view = StatefulComponent;
        }

        public float Apply(StateReference state)
        {
            var maxDuration = 0f;
            
            _nodes.Clear();
            _nodes.AddRange(NodesSearcher.GetParents(state, _view));

            for (var index = 0; index < _nodes.Count; index++)
            {
                var duration = ApplyState(_nodes[index]);
                maxDuration = Mathf.Max(maxDuration, duration);
            }

            var stateDuration = ApplyState(state);
            
            return Mathf.Max(maxDuration, stateDuration);
        }

        private float ApplyState(StateReference state)
        {
            var maxDuration = 0f;
            foreach (var desc in state.Description)
            {
                if (desc.IsEmpty) continue;

                switch (desc.Type)
                {
                    case StateDescriptionTargetType.GameObject:      ApplyGameObject(desc);         break;
                    case StateDescriptionTargetType.Component:       ApplyComponent(desc);          break;
                    case StateDescriptionTargetType.Graphic:         ApplyGraphic(desc);            break;
                    case StateDescriptionTargetType.RectTransform:   ApplyRectTransform(desc);      break;
                    case StateDescriptionTargetType.Image:           ApplyImage(desc);              break;
                    case StateDescriptionTargetType.TextMeshPro:     ApplyTextMeshPro(desc);        break;
                    case StateDescriptionTargetType.CanvasGroup:     ApplyCanvasGroup(desc);        break;
                    case StateDescriptionTargetType.Button:          ApplyButton(desc);             break;
                    case StateDescriptionTargetType.Animator:        ApplyAnimator(desc);           break;
                    case StateDescriptionTargetType.LayoutGroup:     ApplyLayoutGroup(desc);        break;
                    case StateDescriptionTargetType.TextMeshProInputField: ApplyTextMeshProInputField(desc); break;
                    case StateDescriptionTargetType.Animation:
                    {
                        maxDuration = Mathf.Max(maxDuration, ApplyAnimation(state, desc));   
                        break;
                    }
                    case StateDescriptionTargetType.State:           ApplyInnerState(desc);     break;
                    case StateDescriptionTargetType.LayoutElement:   ApplyILayoutElement(desc); break;
                }
            }

            return maxDuration;
        }

        private void ApplyTextMeshProInputField(StateDescription desc)
        {
            desc.TextMeshProInputField.interactable = desc.TextMeshProInputFieldInteractable;
        }

        private void ApplyGraphic(StateDescription desc)
        {
            desc.Graphic.material = desc.GraphicMaterial;
        }

        private void ApplyComponent(StateDescription desc)
        {
            desc.Component.enabled = desc.ComponentIsEnabled;
        }

        private static void ApplyLayoutGroup(StateDescription desc)
        {
            if (desc.LayoutGroupImpactType == LayoutGroupImpactType.SetReverseArrangement)
            {
                desc.LayoutGroup.reverseArrangement = desc.IsLayoutGroupReversed;
            }

            if (desc.LayoutGroupImpactType == LayoutGroupImpactType.BottomPadding)
            {
                desc.LayoutGroup.padding.bottom = desc.PaddingValue;
            }
        }

        private static void ApplyAnimator(StateDescription desc)
        {
            switch (desc.AnimImpact)
            {
                case AnimatorImpactType.SetBoolTrue:
                {
                    desc.Animator.SetBool(desc.AnimParamSetBoolTrue, true);
                    break;
                }
                case AnimatorImpactType.SetBoolFalse:
                {
                    desc.Animator.SetBool(desc.AnimParamSetBoolFalse, false);
                    break;
                }
                case AnimatorImpactType.SetTrigger:
                {
                    desc.Animator.SetTrigger(desc.AnimParamSetTrigger);
                    break;
                }
                case AnimatorImpactType.ResetTrigger:
                {
                    desc.Animator.ResetTrigger(desc.AnimParamResetTrigger);
                    break;
                }
                case AnimatorImpactType.SetInteger:
                {
                    desc.Animator.SetInteger(desc.AnimParamSetInteger, desc.AnimParamSetIntegerValue);
                    break;
                }
                case AnimatorImpactType.SetFloat:
                {
                    desc.Animator.SetFloat(desc.AnimParamSetFloat, desc.AnimParamSetFloatValue);
                    break;
                }
            }
        }

        private float ApplyAnimation(StateReference state, StateDescription desc)
        {
            var duration = 0f;
            if (desc.IsEmpty) return duration;
            
            var clip = desc.AnimationClip;
            var target = desc.AnimationTarget;
            var randomStartTime = desc.IsStartTimeRandomized;
            var stateAnimationInfo = new StateAnimationInfo(clip, target, randomStartTime);

            switch (desc.AnimationImpactType)
            {
                case AnimationImpactType.StartAnimation:
                {
                    state.ScheduleForPlaying(stateAnimationInfo);

                    if (!clip.isLooping)
                    {
                        duration = clip.length;
                    }
                    
                    StatefulUiUtils.StartEditorUpdateLoop(clip.isLooping ? 100 * clip.length : clip.length);
                    
                    stateAnimationInfo.OnUpdate();
                    break;
                }
                case AnimationImpactType.StopAnimation:
                {
                    state.ScheduleForStopping(stateAnimationInfo);
                    break;
                }
            }

            return duration;
        }

        private static void ApplyButton(StateDescription desc)
        {
            desc.Button.interactable = desc.ButtonInteractable;
        }

        private static void ApplyCanvasGroup(StateDescription desc)
        {
            switch (desc.CanvasGroupImpactType)
            {
                case CanvasGroupImpactType.SetAlpha:
                {
                    desc.CanvasGroup.alpha = desc.CanvasGroupAlpha;
                    break;
                }
                case CanvasGroupImpactType.SetInteractable:
                {
                    desc.CanvasGroup.interactable = desc.CanvasGroupInteractable;
                    break;
                }
            }
        }

        private void ApplyTextMeshPro(StateDescription desc)
        {
            switch (desc.TextMeshProImpact)
            {
                case TextMeshProImpactType.SetColor:
                {
                    desc.TextMeshPro.color = desc.TextMeshProColor;
                    break;
                }
                case TextMeshProImpactType.SetPhrase:
                {
                    desc.TextMeshPro.text = LocalizationUtils.GetPhrase(desc.TextMeshProPhraseCode, "---");
                    break;
                }
                case TextMeshProImpactType.SetFontSize:
                {
                    desc.TextMeshPro.fontSize = desc.TextMeshProFontSize;
                    break;
                }
                case TextMeshProImpactType.SetMaxFontSize:
                {
                    desc.TextMeshPro.fontSizeMax = desc.TextMeshProMaxFontSize;
                    break;
                }
                case TextMeshProImpactType.SetFont:
                {
                    desc.TextMeshPro.font = desc.TextMeshProFont;
                    break;
                }
                case TextMeshProImpactType.SetHorizontalAlignment:
                {
                    desc.TextMeshPro.horizontalAlignment = desc.TextMeshProTextHorizontalAlignment;
                    break;
                }
            }
        }

        private static void ApplyImage(StateDescription desc)
        {
            switch (desc.ImageImpact)
            {
                case ImageImpactType.SetEnabled:
                {
                    desc.Image.enabled = desc.ImageEnabled;
                    break;
                }
                case ImageImpactType.SetSprite:
                {
                    desc.Image.sprite = desc.ImageSetSprite;
                    break;
                }
                case ImageImpactType.SetColor:
                {
                    desc.Image.color = desc.ImageSetColor;
                    break;
                }
                case ImageImpactType.SetMaterialFloatValue:
                {
                    if (desc.Image.material.HasProperty(desc.ImageSetMaterialFloatParam))
                    {
                        if (desc.ImageMaterialClone == null)
                        {
                            if (desc.Image.material.name.EndsWith("(Clone)"))
                            {
                                desc.ImageMaterialClone = desc.Image.material;
                            }
                            else
                            {
                                desc.ImageMaterialClone = Object.Instantiate(desc.Image.material);
                                desc.Image.material = desc.ImageMaterialClone;
                            }
                        }

                        desc.ImageMaterialClone.SetFloat(
                            desc.ImageSetMaterialFloatParam,
                            desc.ImageSetMaterialFloatValue
                        );
                    }

                    break;
                }
            }
        }

        private static void ApplyRectTransform(StateDescription desc)
        {
            switch (desc.RectTransformImpactType)
            {
                case RectTransformImpactType.SetPosition:
                {
                    desc.RectTransform.anchoredPosition = desc.RectTransformPosition;
                    break;
                }
                case RectTransformImpactType.SetSize:
                {
                    desc.RectTransform.sizeDelta = desc.RectTransformSize;
                    break;
                }
                case RectTransformImpactType.SetRotation:
                {
                    desc.RectTransform.rotation = Quaternion.Euler(desc.RectTransformRotation);
                    break;
                }
                case RectTransformImpactType.SetScale:
                {
                    desc.RectTransform.localScale = desc.RectTransformScale;
                    break;
                }
                case RectTransformImpactType.SetWidth:
                {
                    var sizeDeltaX = desc.RectTransform.sizeDelta;
                    sizeDeltaX.x = desc.RectTransformWidth;
                    desc.RectTransform.sizeDelta = sizeDeltaX;
                    break;
                }
                case RectTransformImpactType.SetHeight:
                {
                    var sizeDeltaY = desc.RectTransform.sizeDelta;
                    sizeDeltaY.y = desc.RectTransformHeight;
                    desc.RectTransform.sizeDelta = sizeDeltaY;
                    break;
                }
                case RectTransformImpactType.SetTop:
                {
                    desc.RectTransform.SetTop(desc.RectTransformTop);
                    break;
                }
                case RectTransformImpactType.SetBottom:
                {
                    desc.RectTransform.SetBottom(desc.RectTransformBottom);
                    break;
                }
                case RectTransformImpactType.SetLeft:
                {
                    desc.RectTransform.SetLeft(desc.RectTransformLeft);
                    break;
                }
                case RectTransformImpactType.SetRight:
                {
                    desc.RectTransform.SetRight(desc.RectTransformRight);
                    break;
                }
                case RectTransformImpactType.SetAnchors:
                {
                    desc.RectTransform.anchorMin = desc.RectTransformAnchorMin;
                    desc.RectTransform.anchorMax = desc.RectTransformAnchorMax;
                    break;
                }
                case RectTransformImpactType.SetPivot:
                {
                    desc.RectTransform.pivot = desc.RectTransformPivot;
                    break;
                }
                case RectTransformImpactType.ForceRebuildLayout:
                {
                    desc.RectTransform.gameObject.ForceRebuildLayout();
                    break;
                }
            }
        }

        private static void ApplyGameObject(StateDescription desc)
        {
            desc.GameObject.SetActive(desc.GameObjectIsActive);
        }

        private static void ApplyInnerState(StateDescription desc)
        {
            desc.InnerStatefulComponent.SetState(desc.StateRole);
        }

        private static void ApplyILayoutElement(StateDescription desc)
        {
            switch (desc.LayoutElementImpactType)
            {
                 case LayoutElementImpactType.LayoutElementPreferredWidth:
                 {
                     desc.LayoutElement.preferredWidth = desc.LayoutElementPreferredWidth;
                     break;
                 }
                 case LayoutElementImpactType.LayoutElementPreferredHeight:
                 {
                     desc.LayoutElement.preferredHeight = desc.LayoutElementPreferredHeight;
                     break;
                 }
                 case LayoutElementImpactType.LayoutElementMinWidth:
                 {
                     desc.LayoutElement.minWidth = desc.LayoutElementMinWidth;
                     break;
                 }
                 case LayoutElementImpactType.LayoutElementMinHeight:
                 {
                     desc.LayoutElement.minHeight = desc.LayoutElementMinHeight;
                     break;
                 }
                case LayoutElementImpactType.LayoutElementFlexibleWidth:
                {
                    desc.LayoutElement.flexibleWidth = desc.LayoutElementFlexibleWidth;
                    break;
                }
                case LayoutElementImpactType.LayoutElementFlexibleHeight:
                {
                    desc.LayoutElement.flexibleHeight = desc.LayoutElementFlexibleHeight;
                    break;
                }
            }
        }

        public void OnUpdate()
        {
            foreach (var stateReference in _view.States)
            {
                HandleStateAnimations(stateReference);
            }
            
            HandleStateAnimations(_view.InitialState);
            
            foreach (var animationInfo in _animations)
            {
                animationInfo.OnUpdate();
            }

            _animations.RemoveAll(info => info.IsFinished);
        }

        private void HandleStateAnimations(StateReference stateReference)
        {
            stateReference.HandleScheduledStates(this);
        }

        public void Run(StateAnimationInfo newInfo)
        {
            foreach (var info in _animations)
            {
                if (info.Clip == newInfo.Clip && info.Target == newInfo.Target)
                {
                    info.IsFinished = true;
                }
            }

            _animations.Add(newInfo);
        }

        public void Stop(StateAnimationInfo newInfo)
        {
            foreach (var info in _animations)
            {
                if (info.Clip == newInfo.Clip && info.Target == newInfo.Target)
                {
                    info.IsFinished = true;
                }
            }
        }
    }
}

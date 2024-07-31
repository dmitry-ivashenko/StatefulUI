using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.RoleAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StatefulUI.Runtime.States
{
    [Serializable]
    public class StateDescription
    {
        public StateDescriptionTargetType Type;
        
        
        [ChildOnly]
        public GameObject GameObject;
        public bool GameObjectIsActive;
        
        
        [ChildOnly("OnRectTransformChanged")]
        public RectTransform RectTransform;
        public RectTransformImpactType RectTransformImpactType;
        public Vector2 RectTransformPosition;
        public Vector2 RectTransformSize;
        public Vector3 RectTransformRotation;
        public Vector3 RectTransformScale;
        public float RectTransformWidth;
        public float RectTransformHeight;
        public float RectTransformTop;
        public float RectTransformBottom;
        public float RectTransformLeft;
        public float RectTransformRight;
        public Vector2 RectTransformAnchorMin;
        public Vector2 RectTransformAnchorMax;
        public Vector2 RectTransformPivot;

        [ChildOnly("OnLayoutElementChanged")]
        public LayoutElement LayoutElement;
        public LayoutElementImpactType LayoutElementImpactType;
        public float LayoutElementPreferredWidth;
        public float LayoutElementPreferredHeight;
        public float LayoutElementMinWidth;
        public float LayoutElementMinHeight;
        public float LayoutElementFlexibleWidth;
        public float LayoutElementFlexibleHeight;

        [ChildOnly]
        public CanvasGroup CanvasGroup;
        public CanvasGroupImpactType CanvasGroupImpactType;
        public float CanvasGroupAlpha = 1f;
        public bool CanvasGroupInteractable = true;
        
        
        [ChildOnly("OnImageChanged")]
        public Image Image;
        public ImageImpactType ImageImpact;
        public bool ImageEnabled;
        public Sprite ImageSetSprite;
        public Color ImageSetColor;
        public string ImageSetMaterialFloatParam;
        public float ImageSetMaterialFloatValue;
        public Material ImageMaterialClone { get; set; }
        
        
        [ChildOnly("OnTextMeshProChanged")]
        public TextMeshProUGUI TextMeshPro;
        public TextMeshProImpactType TextMeshProImpact;
        public Color TextMeshProColor;
        public string TextMeshProPhraseCode;
        public float TextMeshProFontSize;
        public float TextMeshProMaxFontSize;
        public TMP_FontAsset TextMeshProFont;
        public HorizontalAlignmentOptions TextMeshProTextHorizontalAlignment;
        
        
        [ChildOnly]
        public Button Button;
        public bool ButtonInteractable;
        
        
        [ChildOnly]
        public Animator Animator;
        public AnimatorImpactType AnimImpact;
        public string AnimParamSetBoolTrue;
        public string AnimParamSetBoolFalse;
        public string AnimParamSetTrigger;
        public string AnimParamResetTrigger;
        public string AnimParamSetInteger;
        public int AnimParamSetIntegerValue;
        public string AnimParamSetFloat;
        public int AnimParamSetFloatValue;
        

        public AnimationImpactType AnimationImpactType;
        public AnimationClip AnimationClip;
        [ChildOnly]
        public GameObject AnimationTarget;
        public bool IsStartTimeRandomized;
        
        
        public HorizontalOrVerticalLayoutGroup LayoutGroup;
        public LayoutGroupImpactType LayoutGroupImpactType;
        public bool IsLayoutGroupReversed;
        public int PaddingValue;
        
        
        [ChildOnly]
        public StatefulComponent InnerStatefulComponent;
        [InnerRole(typeof(StateRoleAttribute))]
        public int StateRole;
        
        [ChildOnly]
        public Behaviour Component;
        public bool ComponentIsEnabled;
        
        [ChildOnly]
        public Graphic Graphic;
        public Material GraphicMaterial;
        
        [ChildOnly]
        public TMP_InputField TextMeshProInputField;
        public bool TextMeshProInputFieldInteractable;
        

        public bool IsEmpty
        {
            get
            {
                switch (Type)
                {
                    case StateDescriptionTargetType.GameObject: return GameObject == null;
                    case StateDescriptionTargetType.Animator: return Animator == null;
                    case StateDescriptionTargetType.Image: return Image == null;
                    case StateDescriptionTargetType.TextMeshPro: return TextMeshPro == null;
                    case StateDescriptionTargetType.RectTransform: return RectTransform == null;
                    case StateDescriptionTargetType.Button: return Button == null;
                    case StateDescriptionTargetType.CanvasGroup: return CanvasGroup == null;
                    case StateDescriptionTargetType.Animation: return AnimationClip == null || AnimationTarget == null;
                    case StateDescriptionTargetType.LayoutGroup: return LayoutGroup == null;
                    case StateDescriptionTargetType.State: return InnerStatefulComponent == null;
                    case StateDescriptionTargetType.LayoutElement: return LayoutElement == null;
                    case StateDescriptionTargetType.Component: return Component == null;
                    case StateDescriptionTargetType.Graphic: return Graphic == null;
                    case StateDescriptionTargetType.TextMeshProInputField: return TextMeshProInputField == null;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public string ReadableDescription
        {
            get
            {
                if (IsEmpty) return "empty";

                switch (Type)
                {
                    case StateDescriptionTargetType.GameObject: 
                        return $"{GameObject.GetName()} {(GameObjectIsActive ? "enable" : "disable")}";
                    case StateDescriptionTargetType.Graphic:
                        return $"{Graphic.GetName()} set material {(GraphicMaterial == null ? "null" : GraphicMaterial.GetName())}";
                    case StateDescriptionTargetType.TextMeshProInputField:
                        return $"{TextMeshProInputField.GetName()} set interactible {TextMeshProInputFieldInteractable}";
                    case StateDescriptionTargetType.Component: 
                        return $"{Component.GetName()} component {Component.GetType().Name} {(ComponentIsEnabled ? "enable" : "disable")}";
                    case StateDescriptionTargetType.Animator:
                        return AnimImpact switch
                        {
                            AnimatorImpactType.SetBoolTrue => $"{Animator.GetName()} set bool '{AnimParamSetBoolTrue}' to True",
                            AnimatorImpactType.SetBoolFalse => $"{Animator.GetName()} set bool '{AnimParamSetBoolFalse}' to False",
                            AnimatorImpactType.SetTrigger => $"{Animator.GetName()} set trigger '{AnimParamSetTrigger}'",
                            AnimatorImpactType.ResetTrigger => $"{Animator.GetName()} reset trigger '{AnimParamResetTrigger}'",
                            AnimatorImpactType.SetInteger => $"{Animator.GetName()} set integer '{AnimParamSetInteger}' to {AnimParamSetIntegerValue}",
                            AnimatorImpactType.SetFloat => $"{Animator.GetName()} set float '{AnimParamSetFloatValue}' to {AnimParamSetFloatValue}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.Image: 
                        return ImageImpact switch
                        {
                            ImageImpactType.SetEnabled => $"{Image.GetName()} set {(ImageEnabled ? "enabled" : "disabled")}",
                            ImageImpactType.SetSprite => $"{Image.GetName()} set sprite {ImageSetSprite.GetName()}",
                            ImageImpactType.SetColor => $"{Image.GetName()} set color #{ColorUtility.ToHtmlStringRGBA(ImageSetColor)}",
                            ImageImpactType.SetMaterialFloatValue => $"{Image.GetName()} set material float {ImageSetMaterialFloatParam} to {ImageSetMaterialFloatValue}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.TextMeshPro: 
                        return TextMeshProImpact switch
                        {
                            TextMeshProImpactType.SetColor => $"{TextMeshPro.GetName()} set color #{ColorUtility.ToHtmlStringRGBA(TextMeshProColor)}",
                            TextMeshProImpactType.SetPhrase => $"{TextMeshPro.GetName()} set phrase {TextMeshProPhraseCode}",
                            TextMeshProImpactType.SetFontSize => $"{TextMeshPro.GetName()} set font size {TextMeshProFontSize}",
                            TextMeshProImpactType.SetMaxFontSize => $"{TextMeshPro.GetName()} set max font size {TextMeshProMaxFontSize}",
                            TextMeshProImpactType.SetHorizontalAlignment => $"{TextMeshPro.GetName()} set h.align {TextMeshProTextHorizontalAlignment}",
                            TextMeshProImpactType.SetFont => $"{TextMeshPro.GetName()} set font {TextMeshProFont.GetName()}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.RectTransform: 
                        return RectTransformImpactType switch
                        {
                            RectTransformImpactType.SetPosition => $"{RectTransform.GetName()} set position {RectTransformPosition}",
                            RectTransformImpactType.SetSize => $"{RectTransform.GetName()} set size {RectTransformSize}",
                            RectTransformImpactType.SetRotation => $"{RectTransform.GetName()} set rotation {RectTransformRotation}",
                            RectTransformImpactType.SetScale => $"{RectTransform.GetName()} set scale {RectTransformScale}",
                            RectTransformImpactType.SetWidth => $"{RectTransform.GetName()} set width {RectTransformWidth}",
                            RectTransformImpactType.SetHeight => $"{RectTransform.GetName()} set height {RectTransformHeight}",
                            RectTransformImpactType.SetTop => $"{RectTransform.GetName()} set top {RectTransformTop}",
                            RectTransformImpactType.SetBottom => $"{RectTransform.GetName()} set bottom {RectTransformBottom}",
                            RectTransformImpactType.SetLeft => $"{RectTransform.GetName()} set left {RectTransformLeft}",
                            RectTransformImpactType.SetRight => $"{RectTransform.GetName()} set right {RectTransformRight}",
                            RectTransformImpactType.SetAnchors => $"{RectTransform.GetName()} set anchors {RectTransformAnchorMin} {RectTransformAnchorMax}",
                            RectTransformImpactType.SetPivot => $"{RectTransform.GetName()} set pivot {RectTransformPivot}",
                            RectTransformImpactType.ForceRebuildLayout => $"{RectTransform.GetName()} Force Rebuild Layout",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.Button: 
                        return $"{Button.GetName()} set intaractible {ButtonInteractable}";
                    case StateDescriptionTargetType.CanvasGroup: 
                        return CanvasGroupImpactType switch
                        {
                            CanvasGroupImpactType.SetAlpha => $"{CanvasGroup.GetName()} set alpha {CanvasGroupAlpha}",
                            CanvasGroupImpactType.SetInteractable => $"{CanvasGroup.GetName()} set interactible {CanvasGroupInteractable}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.Animation:
                        return AnimationImpactType switch
                        {
                            AnimationImpactType.StartAnimation => $"start animation {AnimationClip.GetName()} on {AnimationTarget.GetName()}",
                            AnimationImpactType.StopAnimation => $"stop animation {AnimationClip.GetName()} on {AnimationTarget.GetName()}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.LayoutGroup:
                        return LayoutGroupImpactType switch
                        {
                            LayoutGroupImpactType.SetReverseArrangement => $"{LayoutGroup.GetName()} set reverse arrangement {IsLayoutGroupReversed}",
                            LayoutGroupImpactType.BottomPadding => $"{LayoutGroup.GetName()} set bottom padding {PaddingValue}",
                            _ => "undefined"
                        };
                    case StateDescriptionTargetType.State: 
                        
                        return $"{InnerStatefulComponent.GetName()} set state {RoleUtils.GetName(RoleUtils.StateRoleType, StateRole)}";
                    case StateDescriptionTargetType.LayoutElement:
                        return LayoutElementImpactType switch
                        {
                            LayoutElementImpactType.LayoutElementPreferredWidth => $"{LayoutElement.GetName()} set preferred width {LayoutElementPreferredWidth}",
                            LayoutElementImpactType.LayoutElementPreferredHeight => $"{LayoutElement.GetName()} set preferred height {LayoutElementPreferredHeight}",
                            LayoutElementImpactType.LayoutElementMinWidth => $"{LayoutElement.GetName()} set min width {LayoutElementMinWidth}",
                            LayoutElementImpactType.LayoutElementMinHeight => $"{LayoutElement.GetName()} set min height {LayoutElementMinHeight}",
                            LayoutElementImpactType.LayoutElementFlexibleWidth => $"{LayoutElement.GetName()} set flexible width {LayoutElementFlexibleWidth}",
                            LayoutElementImpactType.LayoutElementFlexibleHeight => $"{LayoutElement.GetName()} set flexible height {LayoutElementFlexibleHeight}",
                            _ => "undefined"
                        };
                    default:
                        return "undefined StateDescriptionTargetType";
                }
            }
        }
        
        private void OnRectTransformChanged()
        {
            if (RectTransform == null) return;
            
            if (RectTransformSize == default) RectTransformSize = RectTransform.sizeDelta;
            if (RectTransformPosition == default) RectTransformPosition = RectTransform.anchoredPosition;
            if (RectTransformRotation == default) RectTransformRotation = RectTransform.rotation.eulerAngles;
            if (RectTransformScale == default) RectTransformScale = RectTransform.localScale;
            if (RectTransformAnchorMax == default) RectTransformAnchorMax = RectTransform.anchorMax;
            if (RectTransformAnchorMin == default) RectTransformAnchorMin = RectTransform.anchorMin;
                
            if (RectTransformWidth == 0f) RectTransformWidth = RectTransform.sizeDelta.x;
            if (RectTransformHeight == 0f) RectTransformHeight = RectTransform.sizeDelta.y;
            if (RectTransformTop == 0f) RectTransformTop = -RectTransform.offsetMax.y;
            if (RectTransformBottom == 0f) RectTransformBottom = RectTransform.offsetMin.y;
            if (RectTransformRight == 0f) RectTransformRight = -RectTransform.offsetMax.x;
            if (RectTransformLeft == 0f) RectTransformLeft = RectTransform.offsetMin.x;
        }
        
        private void OnTextMeshProChanged()
        {
            if (TextMeshPro != null && TextMeshProColor == default)
            {
                TextMeshProColor = TextMeshPro.color;
            }
            
            if (TextMeshPro != null && TextMeshProFont == default)
            {
                TextMeshProFont = TextMeshPro.font;
            }
        }

        private void OnImageChanged()
        {
            if (Image != null && ImageSetSprite == null)
            {
                ImageSetSprite = Image.sprite;
            }
            
            if (Image != null && ImageSetColor == default)
            {
                ImageSetColor = Image.color;
            }
        }

        private void OnLayoutElementChanged()
        {
            if (LayoutElement == null) return;

            LayoutElementPreferredWidth = LayoutElement.preferredWidth;
            LayoutElementPreferredHeight = LayoutElement.preferredHeight;
        }

        public override string ToString()
        {
            return $"{base.ToString()} - {ReadableDescription}";
        }
    }
}

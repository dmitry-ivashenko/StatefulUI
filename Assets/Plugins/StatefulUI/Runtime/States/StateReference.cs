using System;
using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.RoleAttributes;
using UnityEngine;

namespace StatefulUI.Runtime.States
{
    [Serializable]
    public class StateReference : BaseReference
    {
        [Role(typeof(StateRoleAttribute))]
        public int Role;

        [HideInInspector]
        public NodeData NodeData;

        public List<StateDescription> Description;

        public bool HasDuplicateRole { get; set; }
        
        private List<StateAnimationInfo> _scheduledForPlaying = new List<StateAnimationInfo>();
        private List<StateAnimationInfo> _scheduledForStopping = new List<StateAnimationInfo>();

        private bool _isDirty;

        public void ScheduleForPlaying(StateAnimationInfo stateAnimationInfo)
        {
            _scheduledForPlaying.Add(stateAnimationInfo);
            _isDirty = true;
        }
        
        public void ScheduleForStopping(StateAnimationInfo stateAnimationInfo)
        {
            _scheduledForStopping.Add(stateAnimationInfo);
            _isDirty = true;
        }

        public void HandleScheduledStates(StateProcessor stateProcessor)
        {
            if (!_isDirty) return;

            foreach (var newInfo in _scheduledForPlaying)
            {
                stateProcessor.Run(newInfo);
            }

            _scheduledForPlaying.Clear();

            foreach (var newInfo in _scheduledForStopping)
            {
                stateProcessor.Stop(newInfo);
            }

            _scheduledForStopping.Clear();

            _isDirty = false;
        }
    }
    
    public enum AnimatorImpactType
    {
        SetBoolTrue,
        SetBoolFalse,
        SetTrigger,
        ResetTrigger,
        SetInteger,
        SetFloat,
    }

    public enum ImageImpactType
    {
        SetEnabled,
        SetSprite,
        SetColor,
        SetMaterialFloatValue,
    }

    public enum StateDescriptionTargetType
    {
        GameObject,
        Animator,
        Image,
        TextMeshPro,
        RectTransform,
        Button,
        CanvasGroup,
        Animation,
        LayoutGroup, 
        State,
        LayoutElement,
        Component,
        Graphic,
        TextMeshProInputField,
    }

    public enum RectTransformImpactType
    {
        Undefined = -1,
        SetPosition,
        SetSize,
        SetRotation,
        SetScale,
        SetWidth,
        SetHeight,
        SetTop,
        SetBottom,
        SetLeft,
        SetRight,
        SetAnchors,
        SetPivot,
        ForceRebuildLayout,
    }

    public enum AnimationImpactType
    {
        StartAnimation,
        StopAnimation,
    }

    public enum CanvasGroupImpactType
    {
        SetAlpha,
        SetInteractable,
    }

    public enum LayoutGroupImpactType
    {
        SetReverseArrangement,
        BottomPadding
    }
    
    public enum TextMeshProImpactType
    {
        SetColor,
        SetPhrase,
        SetFontSize,
        SetHorizontalAlignment,
        SetFont,
        SetMaxFontSize,
    }

    public enum TextColorImpactType
    {
        SetColorByName
    }
    
    public enum LayoutElementImpactType
    {
        LayoutElementPreferredWidth,
        LayoutElementPreferredHeight,
        LayoutElementMinWidth,
        LayoutElementMinHeight,
        LayoutElementFlexibleWidth,
        LayoutElementFlexibleHeight,
    }
}
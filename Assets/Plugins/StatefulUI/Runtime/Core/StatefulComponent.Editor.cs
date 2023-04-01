#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace StatefulUI.Runtime.Core
{
    [ExecuteAlways]
    public sealed partial class StatefulComponent
    {
        public void OnValidate()
        {
            Validate();
            StatefulUiManager.Instance.ValidateStatefulComponent?.Invoke(this);
        }

        public void Validate()
        {
            if (Texts == null) Texts = new List<TextReference>();
            if (TextsInputs == null) TextsInputs = new List<TextInputReference>();
            if (Dropdowns == null) Dropdowns = new List<DropdownReference>();
            if (Buttons == null) Buttons = new List<ButtonReference>();
            if (Animators == null) Animators = new List<AnimatorReference>();
            if (Containers == null) Containers = new List<ContainerReference>();
            if (Sliders == null) Sliders = new List<SliderReference>();
            if (Toggles == null) Toggles = new List<ToggleReference>();
            if (InnerComponents == null) InnerComponents = new List<InnerComponentReference>();
            if (States == null) States = new List<StateReference>();
            if (Images == null) Images = new List<ImageReference>();
            if (VideoPlayers == null) VideoPlayers = new List<VideoPlayerReference>();

            foreach (var component in GetComponentsInChildren<ContainerView>(true))
            {
                if (Containers.Any(reference => reference.Container == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Containers.Add(new ContainerReference { Role = 0, Container = component, });
            }

            foreach (var component in GetComponentsInChildren<Button>(true))
            {
                if (Buttons.Any(reference => reference.Button == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Buttons.Add(new ButtonReference { Role = 0, Button = component, });
            }

            foreach (var component in GetComponentsInChildren<Animator>(true))
            {
                if (Animators.Any(reference => reference.Animator == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Animators.Add(new AnimatorReference { Role = 0, Animator = component, });
            }

            foreach (var component in GetComponentsInChildren<Text>(true))
            {
                if (Texts.Any(reference => reference.Text == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Texts.Add(new TextReference { IsTextMeshPro = false, Role = 0, Text = component });
            }

            foreach (var component in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (Texts.Any(reference => reference.TMP == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Texts.Add(new TextReference { IsTextMeshPro = true, Role = 0, TMP = component });
            }

            foreach (var component in GetComponentsInChildren<InputField>(true))
            {
                if (TextsInputs.Any(reference => reference.InputField == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                TextsInputs.Add(new TextInputReference { IsTMP = false, Role = 0, InputField = component });
            }

            foreach (var component in GetComponentsInChildren<TMP_InputField>(true))
            {
                if (TextsInputs.Any(reference => reference.InputFieldTMP == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                TextsInputs.Add(new TextInputReference { IsTMP = true, Role = 0, InputFieldTMP = component });
            }

            foreach (var component in GetComponentsInChildren<Dropdown>(true))
            {
                if (Dropdowns.Any(reference => reference.DropdownField == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Dropdowns.Add(new DropdownReference { IsTMP = false, Role = 0, DropdownField = component });
            }

            foreach (var component in GetComponentsInChildren<TMP_Dropdown>(true))
            {
                if (Dropdowns.Any(reference => reference.DropdownFieldTMP == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Dropdowns.Add(new DropdownReference { IsTMP = true, Role = 0, DropdownFieldTMP = component });
            }

            foreach (var component in GetComponentsInChildren<Slider>(true))
            {
                if (Sliders.Any(reference => reference.Slider == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Sliders.Add(new SliderReference { Role = 0, Slider = component });
            }

            foreach (var component in GetComponentsInChildren<Toggle>(true))
            {
                if (Toggles.Any(reference => reference.Toggle == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                Toggles.Add(new ToggleReference { Role = 0, Toggle = component });
            }

            foreach (var component in GetComponentsInChildren<StatefulComponent>(true))
            {
                if (component == this) continue;
                if (InnerComponents.Any(reference => reference.InnerComponent == component)) continue;
                var statefulComponents = component.GetComponentsInParent<StatefulComponent>(true);
                if (statefulComponents.Length > 2 && statefulComponents[1] != this) continue;
                InnerComponents.Add(new InnerComponentReference { Role = 0, InnerComponent = component });
            }
            
            foreach (var component in GetComponentsInChildren<VideoPlayer>(true))
            {
                if (VideoPlayers.Any(reference => reference.VideoPlayer == component)) continue;
                if (component.GetComponentInParent<StatefulComponent>() != this) continue;
                VideoPlayers.Add(new VideoPlayerReference { Role = 0, VideoPlayer = component, });
            }
            
            for (var index = Texts.Count - 1; index >= 0; index--)
            {
                var textReference = Texts[index];
                if (textReference.Identificator.IsEmpty())
                {
                    textReference.Identificator = $"{name.CamelCase()}_{textReference.ObjectName.CamelCase()}_{index:00}";
                }
            }
            
            foreach (var stateReference in States)
            {
                if (stateReference.Role == -1)
                {
                    stateReference.Role = 0;
                }

                stateReference.HasDuplicateRole = States.Count(reference => reference.Role == stateReference.Role) > 1;
            }

            Buttons.RemoveAll(reference => reference.Button == null);
            Animators.RemoveAll(reference => reference.Animator == null);
            Containers.RemoveAll(reference => reference.Container == null);
            Texts.RemoveAll(reference => reference.IsEmpty);
            TextsInputs.RemoveAll(reference => reference.IsEmpty);
            Dropdowns.RemoveAll(reference => reference.IsEmpty);
            Sliders.RemoveAll(reference => reference.Slider == null);
            Toggles.RemoveAll(reference => reference.Toggle == null);
            InnerComponents.RemoveAll(reference => reference.InnerComponent == null || reference.InnerComponent == this);
            VideoPlayers.RemoveAll(reference => reference.VideoPlayer == null);

            foreach (var textRef in Texts)
            {
                var contains = false;
                var localizedTimes = textRef.Localize ? 1 : 0;

                foreach (var InnerComponent in InnerComponents)
                {
                    foreach (var InnerComponentText in InnerComponent.InnerComponent.Texts)
                    {
                        if (!InnerComponentText.IsTextMeshPro && textRef.Text == InnerComponentText.Text 
                            || InnerComponentText.IsTextMeshPro && textRef.TMP == InnerComponentText.TMP)
                        {
                            contains = true;

                            if (InnerComponentText.Localize)
                            {
                                localizedTimes++;
                            }
                        }
                    }
                }

                textRef.ContainsInInnerComponent = contains;
                textRef.LocalizedTimes = localizedTimes;
            }

            if (_initialState != null)
            {
                _initialState.Role = -1;
            }

            UpdatePhrasesFromStates();
        }
        
        private void UpdatePhrasesFromStates()
        {
            PhrasesFromStates.Clear();
            
            foreach (var stateReference in States)
            {
                FetchPhrases(stateReference);
            }
            
            FetchPhrases(_initialState);
        }

        private void FetchPhrases(StateReference state)
        {
            if (state?.Description == null) return;

            foreach (var desc in state.Description)
            {
                if (desc.Type == StateDescriptionTargetType.TextMeshPro 
                    && desc.TextMeshProImpact == TextMeshProImpactType.SetPhrase)
                {
                    PhrasesFromStates.Add(new StatePhraseReference
                    {
                        Role = state.Role,
                        Identificator = desc.TextMeshProPhraseCode,
                        TMP = desc.TextMeshPro
                    });
                }
            }
        }

        public void CopyLocalizationInspector()
        {
            var result = CopyLocalization();
            UnityEditor.EditorGUIUtility.systemCopyBuffer = result;
            Debug.Log("result = " + result);
        }
    }
}

#endif
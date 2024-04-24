#if UNITY_EDITOR
using System.Linq;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Editor
{
    using Dimasyechka.Lubribrary.SimpleInputRebinder.Core;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [CustomPropertyDrawer(typeof(InputActionBindingData))]
    public class InputActionBindingDataEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _actionProperty = property.FindPropertyRelative("InputActionReference");
            _bindingProperty = property.FindPropertyRelative("BindingId");
            _displayStringOptionsProperty = property.FindPropertyRelative("DisplayStringOptions");

            RefreshBindingOptions();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(_bindingLabel, Styles.BoldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(_actionProperty);

                var newSelectedBinding = EditorGUILayout.Popup(_bindingLabel, _selectedBindingOption, _bindingOptions);
                if (newSelectedBinding != _selectedBindingOption)
                {
                    var bindingId = _bindingOptionValues[newSelectedBinding];
                    _bindingProperty.stringValue = bindingId;
                    _selectedBindingOption = newSelectedBinding;
                }

                var optionsOld = (InputBinding.DisplayStringOptions)_displayStringOptionsProperty.intValue;
                var optionsNew = (InputBinding.DisplayStringOptions)EditorGUILayout.EnumFlagsField(_displayOptionsLabel, optionsOld);
                if (optionsOld != optionsNew)
                    _displayStringOptionsProperty.intValue = (int)optionsNew;
            }

            if (EditorGUI.EndChangeCheck())
            {
                RefreshBindingOptions();
            }
        }

        protected void RefreshBindingOptions()
        {
            var actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
            var action = actionReference?.action;

            if (action == null)
            {
                _bindingOptions = new GUIContent[0];
                _bindingOptionValues = new string[0];
                _selectedBindingOption = -1;
                return;
            }

            var bindings = action.bindings;
            var bindingCount = bindings.Count;

            _bindingOptions = new GUIContent[bindingCount];
            _bindingOptionValues = new string[bindingCount];
            _selectedBindingOption = -1;

            var currentBindingId = _bindingProperty.stringValue;
            for (var i = 0; i < bindingCount; ++i)
            {
                var binding = bindings[i];
                var bindingId = binding.id.ToString();
                var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                var displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                var displayString = action.GetBindingDisplayString(i, displayOptions);

                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                displayString = displayString.Replace('/', '\\');

                if (haveBindingGroups)
                {
                    var asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        var controlSchemes = string.Join(", ",
                            binding.groups.Split(InputBinding.Separator)
                                .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                _bindingOptions[i] = new GUIContent(displayString);
                _bindingOptionValues[i] = bindingId;

                if (currentBindingId == bindingId)
                    _selectedBindingOption = i;
            }
        }

        private SerializedProperty _actionProperty;
        private SerializedProperty _bindingProperty;
        private SerializedProperty _displayStringOptionsProperty;

        private GUIContent _bindingLabel = new GUIContent("Binding");
        private GUIContent _displayOptionsLabel = new GUIContent("Display Options");
        private GUIContent _uiLabel = new GUIContent("UI");

        private GUIContent[] _bindingOptions;
        private string[] _bindingOptionValues;
        private int _selectedBindingOption;

        private static class Styles
        {
            public static GUIStyle BoldLabel = new GUIStyle("MiniBoldLabel");
        }
    }
}
#endif
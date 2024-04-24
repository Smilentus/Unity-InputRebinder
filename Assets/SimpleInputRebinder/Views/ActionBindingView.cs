using Dimasyechka.Lubribrary.SimpleInputRebinder.Core.Rebinding;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Views
{
    public class ActionBindingView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _bindingTMP;


        [SerializeField]
        private InputActionBindingData _bindingData;



        private InputActionRebinder _rebinderReference;


        private void Awake()
        {
            _rebinderReference = RebindingController.Instance.Rebinder;

            UpdateUI();
        }

        private void OnDestroy()
        {
            UnSubscribe();

            _rebinderReference = null;
        }


        private void OnEnable()
        {
            Subscribe();

            UpdateUI();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }


        public void SetBindingData(InputActionBindingData bindingData)
        {
            _bindingData = bindingData;

            UpdateUI();
        }


        private void Subscribe()
        {
            if (_rebinderReference != null)
            {
                _rebinderReference.onInputActionChanged += OnInputActionChanged;
            }

            InputSystem.onActionChange += OnInputSystemActionChanged;
        }

        private void UnSubscribe()
        {
            if (_rebinderReference != null)
            {
                _rebinderReference.onInputActionChanged -= OnInputActionChanged;
            }

            InputSystem.onActionChange -= OnInputSystemActionChanged;
        }


        private void OnInputSystemActionChanged(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            UpdateUI();
        }

        private void OnInputActionChanged(ActionChangedData data)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_bindingData.InputActionReference == null) return;

            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            var action = _bindingData.InputActionReference.action;
            if (action != null)
            {
                if (_bindingData.BindingIndex != -1)
                    displayString = action.GetBindingDisplayString(_bindingData.BindingIndex, out deviceLayoutName,
                        out controlPath,
                        _bindingData.DisplayStringOptions);
            }

            if (_bindingTMP != null)
            {
                _bindingTMP.text = displayString;
            }
        }


        public void Rebind()
        {
            RebindingController.Instance.StartRebinding(_bindingData.InputActionReference, _bindingData.BindingIndex);
        }
    }
}

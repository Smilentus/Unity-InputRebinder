using Dimasyechka.Lubribrary.SimpleInputRebinder.Core;
using Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays;
using Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays.Base;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Views
{
    [ExecuteInEditMode]
    public class SimpleInputActionRebinderView : MonoBehaviour
    {
        [Header("Rebinding")]
        [SerializeField]
        private TextMeshProUGUI _actionLabelTMP;

        [SerializeField]
        private TextMeshProUGUI _bindingTMP;


        [SerializeField]
        private InputActionBindingData _bindingData;


        // Для тестов костыль
        [Header("Rebinding Overlay")]
        [SerializeField]
        private BaseRebindingOverlayBehaviour _rebindingOverlayBehaviour;


        private InputActionRebinder _rebinder;


        private void Awake()
        {
            SetupRebinder();
            UpdateUI();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }


        private void OnEnable()
        {
            SetupRebinder();

            Subscribe();

            UpdateUI();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }


        private void Subscribe()
        {
            if (_rebinder != null)
            {
                _rebinder.onInputActionChanged += OnInputActionChanged;
                _rebinder.onOperationCompleted += OnOperationCompleted;
                _rebinder.onRebindingSetup += OnOperationSetup;
            }
        }

        private void UnSubscribe()
        {
            if (_rebinder != null)
            {
                _rebinder.onInputActionChanged -= OnInputActionChanged;
                _rebinder.onOperationCompleted -= OnOperationCompleted;
                _rebinder.onRebindingSetup -= OnOperationSetup;
            }
        }


#if UNITY_EDITOR
        protected void OnValidate()
        {
            SetupRebinder();

            _rebinder.SetInputActionReference(_bindingData.InputActionReference);
            _rebinder.SetBindingId(_bindingData.BindingId);

            UpdateUI();
        }
#endif


        private void OnInputActionChanged(ActionChangedData data)
        {
            UpdateUI();
        }

        private void OnOperationCompleted(RebindingOperationCompletionData data)
        {
            UpdateUI();

            _rebindingOverlayBehaviour.Hide();
        }

        private void OnOperationSetup(RebindingSetupData data)
        {
            _rebindingOverlayBehaviour.Show(new StandardOverlayData()
            {
                OperationTitle = data.OperationTitle,
                OperationStatus =  data.OperationStatus
            });
        }


        private void UpdateUI()
        {
            SetupRebinder();

            if (_rebinder.InputAction == null) return;

            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);

            var action = _rebinder.InputAction;
            if (action != null)
            {
                if (_rebinder.RebindableIndex != -1)
                    displayString = action.GetBindingDisplayString(_rebinder.RebindableIndex, out deviceLayoutName,
                        out controlPath,
                        _bindingData.DisplayStringOptions);
            }

            if (_actionLabelTMP != null)
            {
                _actionLabelTMP.text = $"{_rebinder.InputAction.name}";
            }

            if (_bindingTMP != null)
            {
                _bindingTMP.text = displayString;
            }
        }


        private void SetupRebinder()
        {
            if (_rebinder != null) return;

            _rebinder = new InputActionRebinder(_bindingData.InputActionReference, _bindingData.BindingId);
        }


        public void Rebind()
        {
            _rebinder?.StartRebinding();
        }
    }
}

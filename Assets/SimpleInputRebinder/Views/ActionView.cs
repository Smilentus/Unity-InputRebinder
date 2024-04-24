using Dimasyechka.Lubribrary.SimpleInputRebinder.Core.Rebinding;
using Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays;
using Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays.Base;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Views
{
    public class ActionView : MonoBehaviour
    {
        [Tooltip("Использовать только имя действия, не разбивая на биндинги")]
        [SerializeField]
        private bool _useActionNameOnly = false;


        [Header("Rebinding")]
        [SerializeField]
        private TextMeshProUGUI _actionLabelTMP;


        [SerializeField]
        private List<ActionBindingView> _actionBindingViews = new List<ActionBindingView>(2);


        // Для тестов костыль
        [Header("Rebinding Overlay")]
        [SerializeField]
        private BaseRebindingOverlayBehaviour _rebindingOverlayBehaviour;


        private InputActionRebinder _rebinder;


        private void Awake()
        {
            _rebinder = RebindingController.Instance.Rebinder;

            UpdateUI();
        }

        private void OnDestroy()
        {
            UnSubscribe();

            _rebinder = null;
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


        private void Subscribe()
        {
            if (_rebinder != null)
            {
                _rebinder.onOperationCompleted += OnOperationCompleted;
                _rebinder.onOperationCanceled += OnOperationCanceled;
                _rebinder.onRebindingSetup += OnOperationSetup;
            }
        }

        private void UnSubscribe()
        {
            if (_rebinder != null)
            {
                _rebinder.onOperationCompleted -= OnOperationCompleted;
                _rebinder.onOperationCanceled -= OnOperationCanceled;
                _rebinder.onRebindingSetup -= OnOperationSetup;
            }
        }

        private void OnOperationCompleted(RebindingOperationCompletionData data)
        {
            _rebindingOverlayBehaviour.Hide();
        }

        private void OnOperationCanceled(RebindingOperationCancelationData data)
        {
            _rebindingOverlayBehaviour.Hide();
        }

        private void OnOperationSetup(RebindingSetupData data)
        {
            string title = "";

            string exceptedType = !string.IsNullOrEmpty(data.RebindOperation.expectedControlType)
                ? data.RebindOperation.expectedControlType
                : "Any";

            string status = $"\nОжидаемый тип: '{exceptedType}'";


            if (data.InputActionReference.action.bindings[data.BindingIndex].isPartOfComposite)
            {
                title = $"Переназначение действия {data.InputActionReference.action.name} {data.InputActionReference.action.bindings[data.BindingIndex].name}";
            }
            else
            {
                title = $"Переназначение действия {data.InputActionReference.action.name}";
            }

            _rebindingOverlayBehaviour.Show(new StandardOverlayData()
            {
                OperationTitle = title,
                OperationStatus = status
            });
        }

        private void UpdateUI()
        {
            //if (_actionLabelTMP != null)
            //{
            //    if (_useActionNameOnly)
            //    {
            //        _actionLabelTMP.text = $"{_rebinder.InputAction.name}";
            //    }
            //    else
            //    {
            //        if (_rebinder.InputAction.bindings[_rebinder.RebindableIndex].isPartOfComposite)
            //        {
            //            _actionLabelTMP.text =
            //                $"{_rebinder.InputAction.name} {_rebinder.InputAction.bindings[_rebinder.RebindableIndex].name}";
            //        }
            //        else
            //        {
            //            _actionLabelTMP.text = $"{_rebinder.InputAction.name}";
            //        }
            //    }
            //}
        }
    }
}

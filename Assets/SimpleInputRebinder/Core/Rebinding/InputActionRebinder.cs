using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Core.Rebinding
{
    // –азбить на более мелкую ответственность, всЄ равно много всего в одном месте - удолить
    // ѕлюс иде€ всЄ-таки вынести это в общий класс контроллер, к которому мы обращаемс€, чтобы управл€ть более просто
    public class InputActionRebinder
    {
        public event Action<ActionChangedData> onInputActionChanged;

        public event Action<RebindingSetupData> onRebindingSetup;

        public event Action<RebindingOperationCancelationData> onOperationCanceled;
        public event Action<RebindingOperationCompletionData> onOperationCompleted;


        private bool _isActionAssetDisabledWithOperation = true;


        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation = null;


        private List<string> _bindingExcludings = new List<string>();
        private string _defaultCancellingThrough = "<Keyboard>/escape";


        private bool _isRebindingInProgress = false;


        public InputActionRebinder() { }


        public bool IsActionAssetDisabledWithOperation
        {
            get => _isActionAssetDisabledWithOperation;
            set => _isActionAssetDisabledWithOperation = value;
        }

        public bool IsRebindingInProgress => _isRebindingInProgress;


        public void SetCancelingThrough(string defaultCancelingThrough)
        {
            _defaultCancellingThrough = defaultCancelingThrough;
        }


        public void AddExcludings(string[] excludings)
        {
            foreach (string excluding in excludings)
            {
                AddExcluding(excluding);
            }
        }
        public void AddExcluding(string excluding)
        {
            if (_bindingExcludings.Contains(excluding)) return;

            _bindingExcludings.Add(excluding);
        }

        public void ClearExcludings()
        {
            _bindingExcludings.Clear();
        }

        public void RemoveExcludings(string[] excludings)
        {
            foreach (string excluding in excludings)
            {
                RemoveExcluding(excluding);
            }
        }
        public void RemoveExcluding(string excluding)
        {
            if (!_bindingExcludings.Contains(excluding)) return;

            _bindingExcludings.Remove(excluding);
        }


        private void DisposeOperation()
        {
            _rebindingOperation?.Dispose();
            _rebindingOperation = null;
        }


        public void StartRebinding(InputActionReference inputActionReference, int bindingIndex)
        {
            if (_isRebindingInProgress) return;

            UnityEngine.Debug.Log($"StartRebinding");

            if (inputActionReference.action.bindings[bindingIndex].isComposite)
            {
                int firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < inputActionReference.action.bindings.Count &&
                    inputActionReference.action.bindings[firstPartIndex].isPartOfComposite)
                {
                    PerformRebinding(inputActionReference, firstPartIndex, true);
                }
            }
            else
            {
                PerformRebinding(inputActionReference, bindingIndex);
            }
        }

        private void PerformRebinding(InputActionReference inputActionReference, int bindingIndex, bool isAllComposite = false)
        {
            if (_rebindingOperation != null)
            {
                _rebindingOperation?.Cancel();
                DisposeOperation();
            }

            if (_isActionAssetDisabledWithOperation)
            {
                inputActionReference.asset.Disable();
            }

            _isRebindingInProgress = true;

            _rebindingOperation = inputActionReference.action.PerformInteractiveRebinding(bindingIndex);

            _rebindingOperation.WithCancelingThrough(_defaultCancellingThrough);

            foreach (string excluding in _bindingExcludings)
            {
                _rebindingOperation.WithControlsExcluding(excluding);
            }

            _rebindingOperation.WithMagnitudeHavingToBeGreaterThan(0.5f);

            _rebindingOperation.OnMatchWaitForAnother(0.25f);

            _rebindingOperation.OnCancel(
                operation =>
                {
                    onInputActionChanged?.Invoke(new ActionChangedData()
                    {
                        InputActionReference = inputActionReference,
                        BindingIndex = bindingIndex
                    });

                    onOperationCanceled?.Invoke(new RebindingOperationCancelationData()
                    {
                        RebindingOperation = _rebindingOperation
                    });

                    DisposeOperation();

                    if (_isActionAssetDisabledWithOperation)
                    {
                        inputActionReference.asset.Enable();
                    }

                    _isRebindingInProgress = false;

                    UnityEngine.Debug.Log($"OnCancelRebinding");
                });

            _rebindingOperation.OnComplete(
                operation =>
                {
                    onInputActionChanged?.Invoke(new ActionChangedData()
                    {
                        InputActionReference = inputActionReference,
                        BindingIndex = bindingIndex
                    });

                    onOperationCompleted?.Invoke(new RebindingOperationCompletionData()
                    {
                        RebindingOperation = _rebindingOperation
                    });

                    DisposeOperation();

                    if (_isActionAssetDisabledWithOperation)
                    {
                        inputActionReference.asset.Enable();
                    }

                    _isRebindingInProgress = false;

                    UnityEngine.Debug.Log($"OnCompleteRebinding");

                    if (isAllComposite)
                    {
                        int nextBindingIndex = bindingIndex + 1;

                        if (nextBindingIndex < inputActionReference.action.bindings.Count
                            && inputActionReference.action.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformRebinding(inputActionReference, nextBindingIndex, true);
                        }
                    }
                });


            onRebindingSetup?.Invoke(new RebindingSetupData()
            {
                RebindOperation = _rebindingOperation,
                InputActionReference = inputActionReference,
                IsAllComposite = isAllComposite,
                BindingIndex = bindingIndex
            });


            _rebindingOperation.Start();
        }
    }

    [Serializable]
    public class RebindingSetupData
    {
        public InputActionRebindingExtensions.RebindingOperation RebindOperation;
        public InputActionReference InputActionReference;

        public bool IsAllComposite;
        public int BindingIndex;
    }


    [Serializable]
    public class ActionChangedData
    {
        public InputActionReference InputActionReference;
        public int BindingIndex;
    }

    [Serializable]
    public class RebindingOperationData
    {
        public InputActionRebindingExtensions.RebindingOperation RebindingOperation;
    }

    [Serializable]
    public class RebindingOperationCancelationData : RebindingOperationData { }

    [Serializable]
    public class RebindingOperationCompletionData : RebindingOperationData { }
}

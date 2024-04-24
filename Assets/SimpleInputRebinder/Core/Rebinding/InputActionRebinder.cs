using System;
using System.Collections.Generic;
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

        private string _bindingId;
        private int _rebindableIndex = 0;
        private InputActionReference _inputActionReference = null;
        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation = null;


        private List<string> _bindingExcludings = new List<string>();
        private string _defaultCancellingThrough = "<Keyboard>/escape";


        private bool _isRebindingInProgress = false;


        public InputActionRebinder() { }


        public InputAction InputAction => _inputActionReference == null ? null : _inputActionReference.action;

        public int RebindableIndex => _rebindableIndex;

        public bool IsActionAssetDisabledWithOperation
        {
            get => _isActionAssetDisabledWithOperation;
            set => _isActionAssetDisabledWithOperation = value;
        }

        public bool IsRebindingInProgress => _isRebindingInProgress;


        public void SetInputActionReference(InputActionReference inputActionReference)
        {
            _inputActionReference = inputActionReference;
        }

        public void SetBindingId(string bindingId)
        {
            _bindingId = bindingId;
        }

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


        public void StartRebinding()
        {
            if (_isRebindingInProgress) return;

            UnityEngine.Debug.Log($"StartRebinding");

            GetRebindingIndex();

            if (_inputActionReference.action.bindings[_rebindableIndex].isComposite)
            {
                int firstPartIndex = _rebindableIndex + 1;
                if (firstPartIndex < _inputActionReference.action.bindings.Count &&
                    _inputActionReference.action.bindings[firstPartIndex].isPartOfComposite)
                {
                    PerformRebinding(firstPartIndex, true);
                }
            }
            else
            {
                PerformRebinding(_rebindableIndex);
            }
        }

        private void GetRebindingIndex()
        {
            if (string.IsNullOrEmpty(_bindingId) || this.InputAction == null) return;

            var bindingId = new Guid(_bindingId);
            _rebindableIndex = this.InputAction.bindings.IndexOf(x => x.id == bindingId);
        }

        private void PerformRebinding(int bindingIndex, bool isAllComposite = false)
        {
            if (_rebindingOperation != null)
            {
                _rebindingOperation?.Cancel();
                DisposeOperation();
            }

            if (_isActionAssetDisabledWithOperation)
            {
                _inputActionReference.asset.Disable();
            }

            _isRebindingInProgress = true;

            _rebindingOperation = this.InputAction.PerformInteractiveRebinding(bindingIndex);

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
                        InputActionReference = _inputActionReference,
                        BindingIndex = bindingIndex
                    });

                    onOperationCanceled?.Invoke(new RebindingOperationCancelationData()
                    {
                        RebindingOperation = _rebindingOperation
                    });

                    DisposeOperation();

                    if (_isActionAssetDisabledWithOperation)
                    {
                        _inputActionReference.asset.Enable();
                    }

                    _isRebindingInProgress = false;

                    UnityEngine.Debug.Log($"OnCancelRebinding");
                });

            _rebindingOperation.OnComplete(
                operation =>
                {
                    onInputActionChanged?.Invoke(new ActionChangedData()
                    {
                        InputActionReference = _inputActionReference,
                        BindingIndex = bindingIndex
                    });

                    onOperationCompleted?.Invoke(new RebindingOperationCompletionData()
                    {
                        RebindingOperation = _rebindingOperation
                    });

                    DisposeOperation();

                    if (_isActionAssetDisabledWithOperation)
                    {
                        _inputActionReference.asset.Enable();
                    }

                    _isRebindingInProgress = false;

                    UnityEngine.Debug.Log($"OnCompleteRebinding");

                    if (isAllComposite)
                    {
                        int nextBindingIndex = bindingIndex + 1;

                        if (nextBindingIndex < this.InputAction.bindings.Count
                            && this.InputAction.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformRebinding(nextBindingIndex, true);
                        }
                    }
                });


            onRebindingSetup?.Invoke(new RebindingSetupData()
            {
                RebindOperation = _rebindingOperation,
                InputActionReference = _inputActionReference,
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

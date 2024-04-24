using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Core.Rebinding
{
    public class RebindingController : MonoBehaviour
    {
        private static RebindingController _instance;

        public static RebindingController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RebindingController>();
                }

                return _instance;
            }
        }


        private InputActionRebinder _rebinder;
        public InputActionRebinder Rebinder => _rebinder;


        private void Awake()
        {
            SetupRebinder();
        }

        private void OnDestroy()
        {
            _rebinder = null;
            _instance = null;
        }


        public void StartRebinding(InputActionReference inputActionReference, int bindingIndex)
        {
            SetupRebinder();

            _rebinder.StartRebinding(inputActionReference, bindingIndex);
        }


        private void SetupRebinder()
        {
            if (_rebinder != null) return;

            _rebinder = new InputActionRebinder();
        }
    }
}

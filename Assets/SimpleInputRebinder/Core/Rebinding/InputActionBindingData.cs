using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Core.Rebinding
{
    [System.Serializable]
    public class InputActionBindingData
    {
        public InputActionReference InputActionReference;
        public string BindingId;
        public int BindingIndex;
        public InputBinding.DisplayStringOptions DisplayStringOptions;
    }
}

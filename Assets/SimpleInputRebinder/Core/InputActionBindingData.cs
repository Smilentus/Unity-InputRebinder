using UnityEngine.InputSystem;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Core
{
    [System.Serializable]
    public class InputActionBindingData
    {
        public InputActionReference InputActionReference;
        public string BindingId;
        public InputBinding.DisplayStringOptions DisplayStringOptions;
    }
}

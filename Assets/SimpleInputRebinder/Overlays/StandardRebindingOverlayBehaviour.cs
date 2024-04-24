using Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays.Base;
using TMPro;
using UnityEngine;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays
{
    public class StandardRebindingOverlayBehaviour : BaseRebindingOverlayBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _operationTitle;

        [SerializeField]
        private TextMeshProUGUI _operationStatus;


        public override void OnShow()
        {
            StandardOverlayData data = TryConvertOverlayData<StandardOverlayData>();

            if (data == null)
            {
                _operationTitle.text = $"Неизвестная операция";
                _operationStatus.text = $"Неизвестный статус";
            }
            else
            {
                _operationTitle.text = data.OperationTitle;
                _operationStatus.text = data.OperationStatus;
            }
        }

        public override void OnHide() { }
    }

    public class StandardOverlayData : OverlayData
    {
        public string OperationTitle;
        public string OperationStatus;
    }
}

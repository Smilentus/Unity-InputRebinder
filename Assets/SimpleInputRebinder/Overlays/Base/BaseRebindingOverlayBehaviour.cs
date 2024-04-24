using UnityEngine;

namespace Dimasyechka.Lubribrary.SimpleInputRebinder.Overlays.Base
{
    public abstract class BaseRebindingOverlayBehaviour : MonoBehaviour
    {
        protected OverlayData _overlayData;


        protected T TryConvertOverlayData<T>() where T : OverlayData
        {
            if (_overlayData.GetType() == typeof(T))
            {
                return (T)_overlayData;
            }
            else
            {
                return null;
            }
        }


        public void Show()
        {
            this.gameObject.SetActive(true);

            OnShow();
        }
        public void Show(OverlayData overlayData)
        {
            _overlayData = overlayData;

            Show();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);

            OnHide();
        }


        public abstract void OnShow();
        public abstract void OnHide();
    }

    public class OverlayData { }
}
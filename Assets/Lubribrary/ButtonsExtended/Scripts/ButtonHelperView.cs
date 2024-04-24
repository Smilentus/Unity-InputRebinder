namespace ButtonsExtended
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class ButtonHelperView : MonoBehaviour
    {
        [Header("Internal References")]
        [SerializeField] private CanvasGroup faderButtonGroup;

        [SerializeField] private TMP_Text buttonTitle;

        [SerializeField] private ButtonRotationHelper highlightRotator;


        private Tweener faderTweener;


        /// <summary>
        ///     Устанавливает наименование кнопки (W, A, S, D, etc...)
        /// </summary>
        /// <param name="title">
        ///     Наименование кнопки
        /// </param>
        public void SetButtonTitle(string title)
        {
            buttonTitle.text = title;
        }

        [ContextMenu("ShowFade")]
        public void ShowWithFade()
        {
            if (faderTweener != null)
                faderTweener.Kill();

            faderButtonGroup.alpha = 0;

            faderTweener = faderButtonGroup.DOFade(1, 0.25f);

            if (highlightRotator != null)
            {
                highlightRotator.isActive = true;
            }
        }

        [ContextMenu("HideFade")]
        public void HideWithFade()
        {
            if (faderTweener != null)
                faderTweener.Kill();

            faderButtonGroup.alpha = 1;

            faderTweener = faderButtonGroup.DOFade(0, 0.25f);

            if (highlightRotator != null)
            {
                highlightRotator.isActive = false;
            }
        }
    }
}
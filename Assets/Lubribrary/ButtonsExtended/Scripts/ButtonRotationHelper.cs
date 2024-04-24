namespace ButtonsExtended
{
    using System.Collections;
    using UnityEngine;

    public class ButtonRotationHelper : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed;

        [SerializeField] private Vector3 rotationVector;

        [SerializeField] private bool rotateClockwise;


        public bool isActive;


        private Coroutine rotationCoroutine;


        private void Start()
        {
            rotationCoroutine = StartCoroutine(DelayedRotation());
        }

        private void OnEnable()
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }

            rotationCoroutine = StartCoroutine(DelayedRotation());
        }

        private void OnDisable()
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
        }

        private void OnDestroy()
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
        }

        private IEnumerator DelayedRotation()
        {
            while (true)
            {
                if (isActive)
                {
                    this.transform.localEulerAngles += rotationVector * rotationSpeed * (rotateClockwise ? -1 : 1);
                }

                yield return new WaitForSeconds(0.04f);
            }
        }
    }
}
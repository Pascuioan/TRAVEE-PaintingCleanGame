using UnityEngine;

namespace PaintingCleanGame
{
        public class SpongeController : MonoBehaviour
    {
        [SerializeField] private Transform _leftHandAnchor;
        [SerializeField] private Transform _rightHandAnchor;
        [SerializeField] private Transform _sponge;

        private InputData _inputData;

        public void Init(InputData inputData)
        {
            _inputData = inputData;
            SetBodySide(_inputData);
            SetScale(_inputData);
        }

        public void UpdateData(InputData inputData)
        {
            _inputData = inputData;
            SetBodySide(_inputData);
            SetScale(_inputData);
        }

        public void StartGame()
        {
            SetBodySide(_inputData);
        }
        private void SetScale(InputData inputData)
        {
            float sizeRatio = (float)inputData.SpongeSize / 10.0f;
            float minScale = 0.1f;
            float maxScale = 0.4f;
            float scale = minScale + sizeRatio * (maxScale - minScale);
            _sponge.localScale = new Vector3(scale, scale, scale);
        }
        private void SetBodySide(InputData inputData)
        {
            Transform handToUse = _rightHandAnchor;

            if (_inputData.BodySide == BodySide.BODY_SIDE_LEFT)
                handToUse = _leftHandAnchor;

            if (_inputData.BodySide == BodySide.BODY_SIDE_RIGHT)
                handToUse = _rightHandAnchor;

            if (handToUse != null && _sponge != null)
            {
                _sponge.parent = handToUse;
                _sponge.localPosition = new Vector3(0f, 0.03f, -0.03f);
                _sponge.localRotation = Quaternion.Euler(90, 0, 0).normalized;
            }
        }
    }
}
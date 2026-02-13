using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace PaintingCleanGame
{
    public class PaintingController : MonoBehaviour
    {
        [SerializeField]
        private Transform ovrCameraRig;
        [SerializeField]
        private Transform pivot;
        [SerializeField]
        private Transform window;
        [SerializeField]
        private ScoreController _scoreController;
        [SerializeField]
        private PaneController _pane;
        [SerializeField]
        private RawImage rawImage;
        [SerializeField]
        private Texture2D[] paintings;

        private InputData _inputData;

        private UnityAction _onPaintingClean;

        private Vector3 _initialCameraPosition;
        private bool _positionInitialized = false;

        public void Init(InputData inputData, UnityAction onPaintingCleanAction)
        {
            _inputData = inputData;
            _onPaintingClean = onPaintingCleanAction;
            _initialCameraPosition = ovrCameraRig.transform.position;
            _positionInitialized = false;
            SetPaintingPosition();
            SetPaintingSize();
            SetPainting();
            _pane.Init(_inputData, OnPaneCleaned);
        }

        public void UpdateData(InputData inputData)
        {
            _inputData = inputData;
            SetPaintingPosition();
            SetPaintingSize();
            _pane.UpdateData(_inputData);
        }

        public void StartGame()
        {
            _pane.StartGame();
        }
        public float GetCleanRatio()
        {
            return _pane.GetCleanRatio();
        }
        private void OnPaneCleaned()
        {
            _onPaintingClean.Invoke();
        }
        private void SetPainting()
        {
            int index;
            if (_inputData.Painting == 0)
            {
                index = Random.Range(0, paintings.Length);
            }
            else
            {
                index = _inputData.Painting - 1;
            }

            rawImage.texture = paintings[index];
        }
        private void SetPaintingPosition()
        {
            if (!_positionInitialized)
            {
                _initialCameraPosition = ovrCameraRig.transform.position;
                _positionInitialized = true;
            }

            const float minRadius = ParametersController.minRadius;
            const float maxRadius = ParametersController.maxRadius;
            const float minHeight = ParametersController.minHeight;
            const float maxHeight = ParametersController.maxHeight;
            float alpha = (float)_inputData.PaintingAngle * Mathf.Deg2Rad;
            float radius = (float)_inputData.PaintingDistance / 20f;
            float height = (float)_inputData.PaintingHeightPosition / 100f;
            radius = minRadius + ((maxRadius - minRadius) * radius);
            height = minHeight + ((maxHeight - minHeight) * height);
            float sin = Mathf.Sin(alpha);
            float cos = Mathf.Cos(alpha);

            Vector3 newPos = _initialCameraPosition + new Vector3(sin * radius, 0, -cos * radius);
            newPos.y = height;
            transform.position = newPos;

            Vector3 lookDirection = _initialCameraPosition - transform.position;
            lookDirection.y = 0;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                pivot.transform.rotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 180, 0);
            }
        }
        private void SetPaintingSize()
        {
            float heightScale = (_inputData.PaintingHeightSize - 1) / 100f;
            float widthScale = (_inputData.PaintingWidthSize - 1) / 100f;

            float minScale = ParametersController.minScale;
            float maxScale = ParametersController.maxScale;

            heightScale = minScale + (maxScale - minScale) * heightScale;
            widthScale = minScale + (maxScale - minScale) * widthScale;


            pivot.localScale = new Vector3(widthScale, heightScale, 1);
        }
        public void Update()
        {
            int score = Mathf.FloorToInt(GetCleanRatio() * 1000f);
            _scoreController.SetScore(score);
        }
    }

}
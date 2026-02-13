using Meta.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PaintingCleanGame
{
    public class PaneController : MonoBehaviour
    {
        [SerializeField]
        private int _textureSize = 512;

        private float _cleanRadiusWorldSpace = 0.05f;

        private InputData _inputData;

        private Texture2D _paneTexture;
        private Color[] _pixels;
        private Renderer _paneRenderer;
        private int _totalDirtyPixels;
        private int _cleanedPixels = 0;

        private UnityAction _onPaneCleaned;
        private bool _hasBeenCleaned = false;

        private Color clean = new Color(0.52f, 0.83f, 0.89f, 0.05f);
        private Color dirty = new Color(0.7f, 0.7f, 0.7f, 0.9f);


        public void Init(InputData inputData, UnityAction onPaneCleanedAction)
        {
            _inputData = inputData;
            _onPaneCleaned = onPaneCleanedAction;
            _hasBeenCleaned = false;
            _cleanedPixels = 0;
            SetCleanRadius();
            SetUpTexture();
        }
        public void UpdateData(InputData inputData)
        {
            _inputData = inputData;
            SetCleanRadius();
        }
        public void StartGame()
        {
            _cleanedPixels = 0;
            _hasBeenCleaned = false;
            SetUpTexture();
        }
        private void SetUpTexture()
        {
            _paneTexture = new Texture2D(_textureSize, _textureSize);
            _paneTexture.filterMode = FilterMode.Bilinear;

            _pixels = new Color[_textureSize * _textureSize];

            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] = dirty;
            }

            _totalDirtyPixels = _pixels.Length;
            _paneTexture.SetPixels(_pixels);
            _paneTexture.Apply();

            _paneRenderer = GetComponent<Renderer>();
            _paneRenderer.material.mainTexture = _paneTexture;
            _paneRenderer.material.color = Color.white;
        }
        public void CleanAtWorldPosition(Vector3 worldPos)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            float u = (localPos.x) + 0.5f;
            float v = (localPos.y) + 0.5f;

            u = Mathf.Clamp01(u);
            v = Mathf.Clamp01(v);

            int pixelX = Mathf.RoundToInt(u * _textureSize);
            int pixelY = Mathf.RoundToInt(v * _textureSize);

            CleanCircularArea(pixelX, pixelY);
        }
        private void CleanCircularArea(int centerX, int centerY)
        {
            Vector3 worldScale = transform.lossyScale;
            float averageScale = (worldScale.x + worldScale.y) / 2f;

            float radiusInTextureSpace = (_cleanRadiusWorldSpace / averageScale);
            int radiusInPixels = Mathf.FloorToInt(radiusInTextureSpace * _textureSize);

            bool textureChanged = false;

            for (int i = -radiusInPixels; i <= radiusInPixels; i++)
            {
                for (int j = -radiusInPixels; j <= radiusInPixels; j++)
                {
                    if (i * i + j * j > radiusInPixels * radiusInPixels) continue;

                    int pixelX = centerX + i;
                    int pixelY = centerY + j;

                    if (pixelX < 0 || pixelX >= _textureSize || pixelY < 0 || pixelY >= _textureSize) continue;

                    int index = pixelY * _textureSize + pixelX;

                    if (_pixels[index].a <= 0.1f) continue;

                    if (_pixels[index] == clean) continue;

                    _pixels[index] = clean;

                    _cleanedPixels++;
                    textureChanged = true;
                }
            }

            if (!textureChanged) return;

            _paneTexture.SetPixels(_pixels);
            _paneTexture.Apply();

            if (_hasBeenCleaned) return;

            float cleanRatio = GetCleanRatio();
            float cleanRequirement = (float)_inputData.CleanRequirement / 100f;
            if (cleanRatio > cleanRequirement)
            {
                _hasBeenCleaned = true;
                _onPaneCleaned.Invoke();
            }
        }
        public float GetCleanRatio()
        {
            return (float)_cleanedPixels / (float)_totalDirtyPixels;
        }
        private void SetCleanRadius()
        {
            float minCleanRadius = ParametersController.minCleanRadius;
            float maxCleanRadius = ParametersController.maxCleanRadius;
            float size = (_inputData.SpongeSize - 1) / 9f;
            _cleanRadiusWorldSpace = minCleanRadius + (size * (maxCleanRadius - minCleanRadius));
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Sponge"))
            {
                CleanAtWorldPosition(other.transform.position);
            }
        }
    }
}
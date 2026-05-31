using UnityEngine;
using TMPro;

namespace Ko.Result
{
    public class ClosingTextAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textComponent;

        [Header("揺れ設定")]
        public float angleSpeed = 5f;   // 揺れる速さ
        public float waveHeight = 20f;  // 揺れる高さ
        public float waveWidth = 0.2f;  // 波の細かさ（文字間のズレ）

        [Header("色設定")]
        [SerializeField] private float _gradientDensity = 0.05f;
        [SerializeField] private float _hueCycleSpeed = 0.4f;

        private float _hueOffset;

        private void Awake()
        {
            if (_textComponent == null)
            {
                _textComponent = GetComponent<TMP_Text>();
            }
        }

        public void ResetAnimationState()
        {
            _hueOffset = 0f;
        }

        void Update()
        {
            if (_textComponent == null) return;

            _hueOffset = Mathf.Repeat(_hueOffset + (_hueCycleSpeed * Time.deltaTime), 1f);

            // テキスト情報の更新
            _textComponent.ForceMeshUpdate();
            var textInfo = _textComponent.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];

                // 文字が表示されていない、または空白の場合はスキップ
                if (!charInfo.isVisible) continue;

                // この文字が使用しているメッシュの頂点データを取得
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;
                Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Sin波を使ってオフセット（上下のズレ）を計算
                // i * waveWidth を加えることで、文字ごとにタイミングをずらす
                float offsetY = Mathf.Sin(Time.time * angleSpeed + i * waveWidth) * waveHeight;
                Vector3 offset = new Vector3(0, offsetY, 0);

                float charHue = Mathf.Repeat(_hueOffset + (i * _gradientDensity), 1f);
                Color32 color = Color.HSVToRGB(charHue, 1f, 1f);

                // 4つの頂点（左下、左上、右上、右下）すべてにオフセットを適用
                sourceVertices[vertexIndex + 0] += offset;
                sourceVertices[vertexIndex + 1] += offset;
                sourceVertices[vertexIndex + 2] += offset;
                sourceVertices[vertexIndex + 3] += offset;

                if (vertexColors.Length > vertexIndex + 3)
                {
                    vertexColors[vertexIndex + 0] = color;
                    vertexColors[vertexIndex + 1] = color;
                    vertexColors[vertexIndex + 2] = color;
                    vertexColors[vertexIndex + 3] = color;
                }
            }

            _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

namespace Ko.Casino.NoMoneyPanel
{
    public class WarningTextAnimation : MonoBehaviour
    {
        private TMP_Text warningText;
        private string originalText;
        private bool isAnimating = true;

        [Header("点滅の設定")]
        [SerializeField] private float animationChance = 0.5f; // 点滅する確率
        [SerializeField] private float jitterAmount = 5f; // 揺れの強さ
        
        void Awake()
        {
            warningText = GetComponent<TMP_Text>();
            originalText = warningText.text;
        }

        void Start()
        {
            StartCoroutine(AnimationWarningText());
        }

        IEnumerator AnimationWarningText()
        {
            while (isAnimating)
            {
                warningText.ForceMeshUpdate();
                TMP_TextInfo textInfo = warningText.textInfo;
                int characterCount = textInfo.characterCount;

                Color32[][] vertexColors = new Color32[textInfo.meshInfo.Length][];
                Vector3[][] sourceVertics = new Vector3[textInfo.meshInfo.Length][];

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    sourceVertics[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
                }

                for (int i = 0; i < characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;
                    if (Random.value > animationChance) continue;

                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    Vector3 offset = new Vector3(Random.Range(-jitterAmount, jitterAmount), Random.Range(-jitterAmount, jitterAmount), 0);

                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    warningText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                }

                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            }
        }
    }
}

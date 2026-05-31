using UnityEngine;
using TMPro;
using System.Collections;

namespace Ko.Casino.NoMoneyPanel
{
    public class VertexJitter : MonoBehaviour
    {
        public float jitterMagnitude = 5.0f; // 揺れの強さ
        public float updateSpeed = 0.05f;    // 更新間隔（秒）

        private TMP_Text m_TextComponent;
        private bool hasTextChanged;

        void Awake()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        void OnTextChanged(Object obj)
        {
            if (obj == m_TextComponent)
                hasTextChanged = true;
        }

        IEnumerator Start()
        {
            while (true)
            {
                // テキストのメッシュ情報を強制更新
                m_TextComponent.ForceMeshUpdate();

                TMP_TextInfo textInfo = m_TextComponent.textInfo;
                int characterCount = textInfo.characterCount;

                if (characterCount == 0)
                {
                    yield return new WaitForSeconds(updateSpeed);
                    continue;
                }

                // 各文字の頂点をループで処理
                for (int i = 0; i < characterCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                    Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                    for(int j = 0; j < 4; j++)
                    {
                        float noise = Random.Range(-jitterMagnitude, jitterMagnitude);
                        sourceVertices[vertexIndex + j] += new Vector3(noise, 0, 0);
                    }
                }

                // 変更したメッシュを反映
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return new WaitForSeconds(updateSpeed);
            }
        }
    }
}
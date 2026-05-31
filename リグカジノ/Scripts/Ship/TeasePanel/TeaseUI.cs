using UnityEngine;
using TMPro;
using Alchemy.Inspector;

namespace Ko.Ship.TeasePanel
{
    public class TeaseUI : MonoBehaviour
    {
        [Title("左側")]
        [SerializeField, Header("からかいのテキスト(左)")] private TMP_Text _teaseTextLeft;
        [SerializeField, Header("からかいのセリフの吹き出し(左)")] private RectTransform _teaseBubbleLeft;
        [Title("右側")]
        [SerializeField, Header("からかいのテキスト(右)")] private TMP_Text _teaseTextRight;
        [SerializeField, Header("からかいのセリフの吹き出し(右)")] private RectTransform _teaseBubbleRight;
        private Vector2 _teaseTextPosition;

        public void Initialize()
        {
            _teaseTextLeft.text = "";
            _teaseTextRight.text = "";
            _teaseBubbleLeft.gameObject.SetActive(false);
            _teaseBubbleRight.gameObject.SetActive(false);
            _teaseTextPosition = Vector2.zero;
        }

        public void SetUIPosition(Vector2 position, bool isLeftSide)
        {
            _teaseTextPosition = position;

            if (isLeftSide)
            {
                _teaseBubbleLeft.gameObject.SetActive(true);
                _teaseBubbleRight.gameObject.SetActive(false);
                _teaseBubbleLeft.anchoredPosition = position;
            }
            else
            {
                _teaseBubbleLeft.gameObject.SetActive(false);
                _teaseBubbleRight.gameObject.SetActive(true);
                _teaseBubbleRight.anchoredPosition = position;
            }
        }

        public void SetUIText(string text, bool isLeftSide)
        {
            if (isLeftSide)
            {
                _teaseTextLeft.text = text;
            }
            else
            {
                _teaseTextRight.text = text;
            }
        }

        public void GenerateSpeechBubble(Vector2 position, string text, bool isLeftSide)
        {
            SetUIPosition(position, isLeftSide);
            SetUIText(text, isLeftSide);
        }
    }
}
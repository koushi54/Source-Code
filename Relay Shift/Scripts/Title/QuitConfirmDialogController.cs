using UnityEngine.UI;
using UnityEngine;

namespace Ko
{
    public class QuitConfirmDialogController : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _yesButtonImage;
        [SerializeField] private Image _noButtonImage;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private Color _normalColor = Color.white;

        public bool IsYesSelected { get; private set; } = true;

        private void Start()
        {
            Hide();
        }

        public void Show()
        {
            IsYesSelected = true;
            _backgroundImage.gameObject.SetActive(true);
            _yesButtonImage.gameObject.SetActive(true);
            _noButtonImage.gameObject.SetActive(true);
            SetSelectedOption(IsYesSelected);
        }

        public void Hide()
        {
            _backgroundImage.gameObject.SetActive(false);
            _yesButtonImage.gameObject.SetActive(false);
            _noButtonImage.gameObject.SetActive(false);
        }

        public void SetSelectedOption(bool isYesSelected)
        {
            IsYesSelected = isYesSelected;

            // 選択されている方のボタンの色を変えるなどの処理をここに書く
            if (isYesSelected)
            {
                _yesButtonImage.color = _selectedColor; // 例: 選択されている方を黄色にする
                _noButtonImage.color = _normalColor;
            }
            else
            {
                _yesButtonImage.color = _normalColor;
                _noButtonImage.color = _selectedColor; // 例: 選択されている方を黄色にする
            }
        }
    }
}
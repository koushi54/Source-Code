using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Ko.InGame.GameClear
{
    public class GameClearContainer : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject[] _confetties;
        [SerializeField] private GameObject _deadEraserPrefab;
        [SerializeField] private TMP_Text _gameClearText;
        [SerializeField] private TMP_Text _clearTimeText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private CinemachineCamera _clearCamera;

        public GameObject Player => _player;
        public GameObject[] Confetties => _confetties;
        public TMP_Text GameClearText => _gameClearText;
        public TMP_Text ClearTimeText => _clearTimeText;
        public Button NextButton => _nextButton;
        public CinemachineCamera ClearCamera => _clearCamera;
        private Vector3 _playerPosition;
        private GameObject _currentDeadEraser;
        public GameObject CurrentDeadEraser => _currentDeadEraser;

        void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var playerPos = _player.transform.localPosition;
            foreach (var confetti in _confetties)
            {
                confetti.transform.localPosition = playerPos;
                confetti.SetActive(false);
            }
            _clearCamera.transform.localPosition = new Vector3(playerPos.x, playerPos.y + 2f, playerPos.z - 4f);
            _gameClearText.gameObject.SetActive(false);
            _gameClearText.transform.localScale = Vector3.zero;
            _clearTimeText.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);
        }

        private void SetClearTimeText(float limitTime, float currentTime)
        {
            // クリアタイムを表示するテキストを更新
            // クリアタイムはlimitTimeからCurrentTimeを引いた値を分秒形式で表示
            var clearTime = Mathf.Max(0f, limitTime - currentTime);
            int minutes = Mathf.FloorToInt(clearTime / 60f);
            int seconds = Mathf.FloorToInt(clearTime % 60f);
            _clearTimeText.text = $"クリアタイム: {minutes}分{seconds}秒";
        }

        public void ShowGameClearUI(float limitTime, float currentTime)
        {
            var playerPos = _player.transform.localPosition;
            foreach (var confetti in _confetties)
            {
                confetti.transform.localPosition = playerPos;
                confetti.SetActive(true);
            }
            SetClearTimeText(limitTime, currentTime);
            _gameClearText.gameObject.SetActive(true);
            _clearTimeText.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
        }

        public void HideGameClearUI()
        {
            foreach (var confetti in _confetties)
            {
                confetti.SetActive(false);
            }
            _gameClearText.gameObject.SetActive(false);
            _clearTimeText.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);
        }

        public void GenerateDeadEraser()
        {
            _playerPosition = _player.transform.position;
            _clearCamera.transform.localPosition = new Vector3(_playerPosition.x, _playerPosition.y + 2f, _playerPosition.z - 5f);
            _currentDeadEraser = Instantiate(_deadEraserPrefab, _playerPosition, Quaternion.identity);
            _currentDeadEraser.transform.rotation = _player.transform.rotation;
            _currentDeadEraser.SetActive(true);
        }
    }
}

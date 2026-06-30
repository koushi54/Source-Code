using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ko.InGame.GameOver
{
    public class GameOverContainer : MonoBehaviour
    {
        [Header("プレイヤー関連")]
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _deadEraserPrefab;
        [Header("UI関連")]
        [SerializeField] private GameObject _gameOverText;
        [SerializeField] private Button _continueButton;
        [Header("その他")]
        [SerializeField] private Light _directionalLight;

        private Vector2 _gameOverTextInitialPos;
        private Vector2 _continueButtonInitialPos;

        public GameObject Player => _player;
        public GameObject DeadEraser => _deadEraserPrefab;
        public GameObject GameOverText => _gameOverText;
        public Button ContinueButton => _continueButton;
        private Vector3 _playerPosition;
        private GameObject _currentDeadEraser;
        public GameObject CurrentDeadEraser => _currentDeadEraser;
        public Light DirectionalLight => _directionalLight;
        public Vector2 GameOverTextInitialPos => _gameOverTextInitialPos;
        public Vector2 ContinueButtonInitialPos => _continueButtonInitialPos;

        void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _deadEraserPrefab.SetActive(false);
            _gameOverText.SetActive(false);
            _continueButton.gameObject.SetActive(false);
            _gameOverTextInitialPos = _gameOverText.transform.position;
            _continueButtonInitialPos = _continueButton.transform.position;
        }

        public void GenerateDeadEraser()
        {
            _playerPosition = _player.transform.position;
            _currentDeadEraser = Instantiate(_deadEraserPrefab, _playerPosition, Quaternion.identity);
            _currentDeadEraser.transform.rotation = _player.transform.rotation;
            _currentDeadEraser.SetActive(true);
        }

        public void ShowGameOverUI()
        {
            _gameOverText.SetActive(true);
            _continueButton.gameObject.SetActive(true);
        }

        public void HideGameOverUI()
        {
            Initialize(); // UIを初期状態にリセット
        }
    }
}
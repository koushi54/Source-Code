using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitMotion;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.InputSystem;

namespace Ko
{
    public class ResultManager : MonoBehaviour
    {
        [SerializeField] private GameObject _baton;
        [SerializeField] private GameObject _goalPositionObj;
        [Header("ResultCanvas")]
        [SerializeField] private Image _resultImage;
        [SerializeField] private TMP_Text _resultText;
        [SerializeField] private TMP_Text _scoreText;
        [Header("Messages")]
        [SerializeField] private CanvasGroup _messageCanvasGroup;
        [SerializeField] private TMP_Text _retryText;
        [SerializeField] private TMP_Text _quitText;
        private MotionHandle _countUpMotion;
        private Vector3 _batonOriginalPosition;


#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                PlayResultAnimation(100).Forget();
            }
        }
#endif

        private void Start()
        {
            _batonOriginalPosition = _baton.transform.position;
            InitializeUI();
        }

        private void InitializeUI()
        {
            _resultImage.rectTransform.anchoredPosition = new Vector2(0, 500);
            _resultImage.gameObject.SetActive(false);
            _resultText.gameObject.SetActive(false);
            _scoreText.gameObject.SetActive(false);
            _scoreText.text = "回";
            _messageCanvasGroup.alpha = 0;
            _messageCanvasGroup.gameObject.SetActive(false);
        }

        public async UniTask PlayResultAnimation(int score)
        {
            await ScreenEffect.Instance.FadeIn(3f);
            await PlayBatonAnimation();
            await ShowResult();
            await ShowScore(score); // 仮のスコア
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            ShowMessage();
        }

        // 結果パネルのアニメーション
        private async UniTask ShowResult()
        {
            _resultImage.gameObject.SetActive(true);
            _resultText.gameObject.SetActive(true);
            _scoreText.gameObject.SetActive(true);
            await LMotion.Create(new Vector2(0, 500), Vector2.zero, 1f)
                .WithEase(Ease.OutBack)
                .Bind(_resultImage.rectTransform, (val, target) =>
                {
                    target.anchoredPosition = val;
                });
        }

        // スコアのアニメーション
        private async UniTask ShowScore(int score)
        {
            if (_countUpMotion != null && _countUpMotion.IsActive())
            {
                _countUpMotion.Cancel();
            }
            _countUpMotion = LMotion.Create(0, score, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(_scoreText, (val, target) =>
                {
                    target.text = Mathf.FloorToInt(val).ToString() + "回";
                });
            await _countUpMotion.ToUniTask();
            MotionHandle enphasismotion = LMotion.Create(1f, 2f, 0.3f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .Bind(_scoreText.rectTransform, (val, target) =>
                {
                    target.localScale = Vector3.one * val;
                });
        }

        private void ShowMessage()
        {
            _messageCanvasGroup.gameObject.SetActive(true);
            LMotion.Create(0f, 1f, 3f)
                .WithEase(Ease.OutQuad)
                .Bind(_messageCanvasGroup, (val, target) =>
                {
                    target.alpha = val;
                });
        }

        private async UniTask PlayBatonAnimation()
        {
            Vector3 targetPosition = _goalPositionObj.transform.position; // 目的地の座標を指定
            float randomRotationAngle = UnityEngine.Random.Range(-40f, -30f); // ランダムな回転角度を生成
            MotionHandle batonMotion = LSequence.Create()
                .Append(LMotion.Create(_batonOriginalPosition, targetPosition, 4f)
                    .WithEase(Ease.OutQuad)
                    .Bind(_baton.transform, (val, target) =>
                    {
                        target.position = val;
                    }))
                .Join(LMotion.Create(_baton.transform.rotation.eulerAngles, _baton.transform.rotation.eulerAngles + new Vector3(0, 0, randomRotationAngle), 4f)
                    .WithEase(Ease.OutQuad)
                    .Bind(_baton.transform, (val, target) =>
                    {
                        target.rotation = Quaternion.Euler(val);
                    }))
                    .Run()
                    .AddTo(this);
            await batonMotion.ToUniTask();
        }
    }
}

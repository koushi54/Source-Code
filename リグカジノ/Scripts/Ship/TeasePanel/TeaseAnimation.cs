using UnityEngine;
using Unity.Netcode;
using Ko.SO;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cysharp.Threading.Tasks;


namespace Ko.Ship.TeasePanel
{
    public class TeaseAnimation : NetworkBehaviour
    {
        [SerializeField] private RectTransform _parentRectTransform;
        [SerializeField] private TeasingData _teasingData;
        [SerializeField] private TeaseUI _teaseUIPrefab;
        [SerializeField] private Key.SO.PlayerEvents _playerEvents;

        private void Awake()
        {
            Logger.LoggerManager.Log("TeaseAnimationがAwakeされました。");
            _playerEvents.OnTeasing += ShowTeaseUIClientRpc;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _playerEvents.OnTeasing -= ShowTeaseUIClientRpc;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                ShowTeaseUI();
            }
        }
        #endif

        [Rpc(SendTo.Everyone)]
        public void ShowTeaseUIClientRpc()
        {
            ShowTeaseUI();
        }
        private void ShowTeaseUI()
        {
            (TeaseUI teaseUI, Transform teaseUITransform, CanvasGroup teaseUICanvasGroup) = GenerateTeaseUIPrefab();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(teaseUICanvasGroup.DOFade(1f, 0.1f))
                .Join(teaseUITransform.DOScale(1f, 0.1f).SetEase(Ease.OutBounce));

            // 3秒後にからかいUIを非表示にする
            UniTask.Delay((int)_teasingData.TeasingTextDisplayTime*1000).ContinueWith(() =>
            {
                HideTeaseUI(teaseUI, teaseUITransform, teaseUICanvasGroup);
            });
        }

        private void HideTeaseUI(TeaseUI teaseUI, Transform teaseUITransform, CanvasGroup teaseUICanvasGroup)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(teaseUICanvasGroup.DOFade(0f, 0.1f))
                .Join(teaseUITransform.DOScale(0f, 0.1f).SetEase(Ease.InBack))
                .AppendCallback(() => Destroy(teaseUI.gameObject));
        }

        private (TeaseUI teaseUI, Transform teaseUITransform, CanvasGroup teaseUICanvasGroup) GenerateTeaseUIPrefab()
        {
            Vector2 randomPosition = _teasingData.TeasingTextPosition[Random.Range(0, _teasingData.TeasingTextPosition.Length)];
            string randomText = _teasingData.TeasingTexts[Random.Range(0, _teasingData.TeasingTexts.Length)];
            bool isLeftSide = randomPosition.x < 0 ? true : false;
            TeaseUI teaseUI = Instantiate(_teaseUIPrefab, _parentRectTransform);

            teaseUI.Initialize();
            teaseUI.GenerateSpeechBubble(randomPosition, randomText, isLeftSide);
            Transform teaseUITransform = teaseUI.transform;

            if (!teaseUI.TryGetComponent<CanvasGroup>(out CanvasGroup teaseUICanvasGroup))
            {
                teaseUICanvasGroup = teaseUI.gameObject.AddComponent<CanvasGroup>();
            }

            Logger.LoggerManager.Log("からかいUIを生成しました。");
            return (teaseUI, teaseUITransform, teaseUICanvasGroup);
        }
    }
}

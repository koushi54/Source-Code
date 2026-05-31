using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Ko.SO;

namespace Ko.Casino.QuizPanel
{
    public class QuizController : NetworkBehaviour, Key.Casino.QuizPanel.IController
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _quizPanel;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text[] _optionTexts;
        [SerializeField] private RectTransform _framePanel;
        [SerializeField] private CanvasGroup _framecanvasGroup;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _quizImage;
        [SerializeField] private QuizDatabase _quizDatabase;
        [SerializeField] private Key.SO.PlayerEvents _playerEvents;

        Sequence _frameSequence = null;

        private void Awake()
        {
            _playerEvents.OnOptionChosen += PlaySelectingOptionAnimation;
            _playerEvents.OnOptionChosenComplete += ShowSelectedOptionAnimation;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _playerEvents.OnOptionChosen -= PlaySelectingOptionAnimation;
            _playerEvents.OnOptionChosenComplete -= ShowSelectedOptionAnimation;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (!IsServer) return;
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                ShowQuizPanelClientRpc(2); // クイズ番号を仮に2としています
            }
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                PlaySelectingOptionAnimation(1);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                PlaySelectingOptionAnimation(2);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                PlaySelectingOptionAnimation(3);
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                ShowSelectedOptionAnimation();
            }
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                HideQuizPanelClientRpc();
            }
        }
        #endif

        private void ShowQuizPanel(int questionNumber)
        {

            _quizImage.fillAmount = 1f; // クイズパネルの背景イメージを満タンにしてから表示アニメーションを開始する
            _fillImage.fillAmount = 0f; // フィルイメージの初期値を0に設定
            _framecanvasGroup.alpha = 0f; // フレームのキャンバスグループの初期透明度を0に設定
            // クイズパネルを表示するロジックをここに実装
            _questionText.text = $"{_quizDatabase.QuizData[questionNumber - 1].Question}";
            for (int i = 0; i < _optionTexts.Length; i++)
            {
                if (i < _quizDatabase.QuizData[questionNumber - 1].Options.Length)
                {
                    _optionTexts[i].text = $"{i+1} {_quizDatabase.QuizData[questionNumber - 1].Options[i]}";
                    _optionTexts[i].transform.parent.gameObject.SetActive(true);
                    _optionTexts[i].alpha = 0f; // フェードインのために最初は透明にしておく
                }
                else
                {
                    _optionTexts[i].transform.parent.gameObject.SetActive(false);
                }
            }

            // 上から落ちてくる動きと同時にフェードイン
            Vector2 targetPos = _quizPanel.anchoredPosition;
            Vector2 startPos = new Vector2(targetPos.x, targetPos.y + 200f);
            _quizPanel.anchoredPosition = startPos;
            _canvasGroup.alpha = 0f;

            Sequence seq = DOTween.Sequence();
            Sequence optionSeq = DOTween.Sequence();

            seq.Append(_quizPanel.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutCubic))
                .Join(_canvasGroup.DOFade(1f, 0.5f));

            for (int i = 0; i < _optionTexts.Length; i++)
            {
                if (i < _quizDatabase.QuizData[questionNumber - 1].Options.Length)
                {
                    RectTransform rect = _optionTexts[i].GetComponent<RectTransform>();
                    targetPos = rect.anchoredPosition;
                    startPos = new Vector2(targetPos.x + 1000f, targetPos.y);

                    rect.anchoredPosition = startPos;
                    _optionTexts[i].alpha = 0f;

                    // 各選択肢の開始時間を 0.1秒ずつズラす
                    float startTime = i * 0.1f;
                    float duration = 1.0f - (0.1f * i);

                    // Insert を使うことで、Sequence 内の絶対時間を指定してアニメーションを配置できます
                    optionSeq.Insert(startTime, rect.DOAnchorPos(targetPos, duration).SetEase(Ease.OutCubic));
                    optionSeq.Insert(startTime, _optionTexts[i].DOFade(1f, 0.2f).SetEase(Ease.Linear));

                }
            }


            // 親のシーケンスに結合
            seq.Append(optionSeq).OnComplete(() =>
            {
                // アニメーションがすべて完了した後の処理をここに書くことができます
                _frameSequence = DOTween.Sequence(); // 新しいシーケンスを作成して代入
                _frameSequence.Append(_framecanvasGroup.DOFade(1f, 0.4f).SetEase(Ease.Linear)).SetLoops(-1, LoopType.Yoyo);
            });

            Logger.LoggerManager.Log($"<color=green>[QuizController]</color> クイズパネルを表示します。クイズ番号: {questionNumber}");
        }

        [Rpc(SendTo.Everyone)]
        public void ShowQuizPanelClientRpc(int questionNumber)
        {
            ShowQuizPanel(questionNumber);
            PlaySelectingOptionAnimation(1); // クイズパネルが表示されたときに、最初の選択肢のフレーム位置に移動するアニメーションを再生
        }

        private void PlaySelectingOptionAnimation(int option)
        {
            Vector2 targetPos1 = new Vector2(_framePanel.anchoredPosition.x, -10f);
            Vector2 targetPos2 = new Vector2(_framePanel.anchoredPosition.x, -130f);
            Vector2 targetPos3 = new Vector2(_framePanel.anchoredPosition.x, -255f);


            if (option < 1 || option > _optionTexts.Length) return;

            switch (option)
            {
                case 1:
                    _framePanel.DOAnchorPos(targetPos1, 0.3f).SetEase(Ease.OutCubic);
                    break;
                case 2:
                    _framePanel.DOAnchorPos(targetPos2, 0.3f).SetEase(Ease.OutCubic);
                    break;
                case 3:
                    _framePanel.DOAnchorPos(targetPos3, 0.3f).SetEase(Ease.OutCubic);
                    break;
            }
        }

        private void ShowSelectedOptionAnimation()
        {
            Sequence seq = DOTween.Sequence();
            if (_frameSequence != null)
            {
                _frameSequence.Kill(); // フレームの点滅アニメーションを停止
                _framecanvasGroup.alpha = 0f; // フレームの透明度をリセット
                _frameSequence = DOTween.Sequence(); // 新しいシーケンスを作成して代入
                _frameSequence.Append(_framecanvasGroup.DOFade(1f, 0.1f).SetEase(Ease.Linear)).SetLoops(5, LoopType.Yoyo);
                seq.Append(_frameSequence);
                seq.Append(_fillImage.DOFillAmount(1f, 0.5f).SetEase(Ease.OutQuart)).OnComplete(() =>
                {
                    Logger.LoggerManager.Log($"<color=green>[QuizController]</color> 選択が確定しました");
                });
            }
        }

        [Rpc(SendTo.Everyone)]
        public void HideQuizPanelClientRpc()
        {
            HideQuizPanel();
        }

        private void HideQuizPanel()
        {
            // クイズパネルを非表示にするロジックをここに実装
            Sequence panelSeq = DOTween.Sequence();
            panelSeq.Append(_quizImage.DOFillAmount(0f, 0.5f).SetEase(Ease.OutQuart));
            Logger.LoggerManager.Log($"<color=green>[QuizController]</color> クイズパネルを非表示にします。");
        }
    }

}

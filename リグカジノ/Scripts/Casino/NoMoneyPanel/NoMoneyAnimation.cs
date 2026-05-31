using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class NoMoneyAnimation : NetworkBehaviour
{
    [SerializeField] private CanvasGroup _backgroundCanvasGroup;
    [SerializeField] private RectTransform _warningImage;
    [SerializeField] private CanvasGroup _warningImageCanvasGroup;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private CanvasGroup _warningTextCanvasGroup;
    [SerializeField] private RectTransform _topWarningStripe;
    [SerializeField] private RectTransform _bottomWarningStripe;

    private void Awake()
    {
        InitializeAnimation();
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ShowNoMoneyAnimation();
        }
    }
    #endif

    [Rpc(SendTo.Everyone)]
    public void ShowNoMoneyAnimationRpc()
    {
        ShowNoMoneyAnimation();
    }

    // お金がないときのアニメーションを表示する
    private void ShowNoMoneyAnimation()
    {
        InitializeAnimation();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_backgroundCanvasGroup.DOFade(0.6f, 0.5f).SetLoops(5, LoopType.Yoyo))
            .Join(_warningImageCanvasGroup.DOFade(1f,0.5f).SetLoops(5, LoopType.Yoyo))
            .Join(_topWarningStripe.DOAnchorPosY(0,0.5f).SetEase(Ease.OutQuad))
            .Join(_bottomWarningStripe.DOAnchorPosY(0,0.5f).SetEase(Ease.OutQuad))
            .AppendCallback(() => _warningImage.DOAnchorPosX(-400, 0.5f).SetEase(Ease.OutQuad))
            .AppendCallback(() => _warningTextCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad));

    }

    // アニメーション開始前の初期状態を設定する
    private void InitializeAnimation()
    {
        _backgroundCanvasGroup.alpha = 0f;
        _warningImage.gameObject.SetActive(true);
        _warningImage.anchoredPosition = new Vector2(0, 0);
        _warningImageCanvasGroup.alpha = 0f;
        _warningText.gameObject.SetActive(true);
        _warningTextCanvasGroup.alpha = 0f;
        _topWarningStripe.gameObject.SetActive(true);
        _topWarningStripe.anchoredPosition = new Vector2(0, 100);
        _bottomWarningStripe.gameObject.SetActive(true);
        _bottomWarningStripe.anchoredPosition = new Vector2(0, -100);
    }
}

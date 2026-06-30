using LitMotion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ButtonSEManager _buttonSEManager;
    [SerializeField] private float _hoverScale = 1.2f;
    [SerializeField] private float _animationDuration = 0.1f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;
    private Vector3 _originalScale;
    private MotionHandle _hoverHandle;

    void Start()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ボタンにマウスオーバーしたときの処理を実装
        if (_hoverHandle.IsActive())
        {
            _hoverHandle.Cancel();
        }
        _hoverHandle = LMotion.Create(transform.localScale, Vector3.one * _hoverScale, _animationDuration)
            .WithEase(_animationEase)
            .Bind(scale =>
            {
                transform.localScale = scale;
            });
        _buttonSEManager.PlayHoveringButtonSE();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ボタンからマウスが離れたときの処理を実装
        if (_hoverHandle.IsActive())
        {
            _hoverHandle.Cancel();
        }
        _hoverHandle = LMotion.Create(transform.localScale, _originalScale, _animationDuration)
            .WithEase(_animationEase)
            .Bind(scale =>
            {
                transform.localScale = scale;
            });
    }

    private void OnDestroy()
    {
        if (_hoverHandle.IsActive())
        {
            _hoverHandle.Cancel();
        }
    }
}

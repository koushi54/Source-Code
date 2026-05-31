using UnityEngine.UI;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Ko
{
    public class SettingPanelView : MonoBehaviour
    {
        [SerializeField] private Image _panelImage;
        public Image PanelImage => _panelImage;

        public MotionHandle OpenPanel(float duration)
        {
            _panelImage.gameObject.SetActive(true);
            return LMotion.Create(0f, 1f, duration)
                .WithEase(LitMotion.Ease.OutBack)
                .BindToLocalScaleX(_panelImage.rectTransform);
        }

        public MotionHandle ClosePanel(float duration)
        {
            return LMotion.Create(1f, 0f, duration)
                .WithEase(LitMotion.Ease.InBack)
                .BindToLocalScaleX(_panelImage.rectTransform);
        }

        public void SetActive(bool v)
        {
            if (_panelImage != null)
                _panelImage.gameObject.SetActive(v);
        }

        public void SetScaleX(float x)
        {
            if (_panelImage == null) return;
            var r = _panelImage.rectTransform;
            r.localScale = new Vector3(x, r.localScale.y, r.localScale.z);
        }
    }
}

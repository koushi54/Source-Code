using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class SettingPanelAnimator : MonoBehaviour
{
    // 色をRGBで指定しているため、0-255の範囲で設定
    [SerializeField] private Color _normalColor = new Color(120, 120, 120);
    [SerializeField] private Color _selectedColor = new Color(0, 70, 170);
    [SerializeField] private Color _normalHandlerColor = new Color(80, 80, 80);
    [SerializeField] private Color _selectedHandlerColor = new Color(255, 255, 255);
    private MotionHandle _selectEffectMotion;
    public void ChangeOptionBackground(Image[] backgrounds, int currentIndex)
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (i == currentIndex)
            {
                backgrounds[i].color = _selectedColor;
            }
            else
            {
                backgrounds[i].color = _normalColor;
            }
        }
    }

    public void ChangeSliderFillColor(Image[] fillImages, int currentIndex, bool isSelected)
    {
        for (int i = 0; i < fillImages.Length; i++)
        {
            if (i == currentIndex)
            {
                fillImages[i].color = isSelected ? _selectedHandlerColor : _normalHandlerColor;
            }
            else
            {
                fillImages[i].color = _normalHandlerColor;
            }
        }
    }
}

using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace Ko.Result
{
    public class TestUIAnimation : NetworkBehaviour
    {
        [SerializeField] private GameObject _ui; // アニメーションさせるUI要素
        #if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                PlayUIAnimation();
            }
        }
        #endif

        public void PlayUIAnimation()
        {
            Sequence animationSequence = DOTween.Sequence();

            animationSequence.Append(_ui.transform.DOScale(2f, 0.5f).SetEase(Ease.OutBack)) //拡大
                .Join(_ui.transform.DORotate(new Vector3(0, 1035, 0), 0.5f, RotateMode.FastBeyond360)) //回転
                .Append(_ui.transform.DOScale(1f, 0.5f).SetEase(Ease.InBack)); //縮小
        }
    }
}
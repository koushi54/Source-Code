using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Ko.Result
{
    public class ClosingTextController : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _closingText;
        [SerializeField] private Ease _textEase;
        [SerializeField] private GameObject[] _celebrationParticlePrefabs;

        [SerializeField] private ClosingTextAnimation _closingTextAnimation;

        #if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                ShowClosingText();
            }
        }
        #endif

        void Start()
        {
            _closingText.gameObject.SetActive(false);
            _closingText.transform.localScale = Vector3.zero;
            foreach (var prefab in _celebrationParticlePrefabs)
            {
                prefab.SetActive(false);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void ShowClosingTextRpc()
        {
            ShowClosingText();
        }

        private void ShowClosingText()
        {
            // すでに動いているTweenがあれば止める（連打対策）
            DOTween.Kill(_closingText);

            _closingText.gameObject.SetActive(true);
            _closingTextAnimation?.ResetAnimationState();

            // スケールのアニメーション（Sequence）
            Sequence textSequence = DOTween.Sequence();
            textSequence.Append(_closingText.transform.DOScale(2f, 0.5f).SetEase(_textEase))
                .Append(_closingText.transform.DOScale(1f, 0.5f).SetEase(Ease.Linear))
                .SetTarget(_closingText);

            foreach (var prefab in _celebrationParticlePrefabs)
            {
                prefab.SetActive(true);
            }
        }
    }
}
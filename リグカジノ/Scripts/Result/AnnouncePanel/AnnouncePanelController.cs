using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using AudioManager.SE;

namespace Ko.Result
{
    public class AnnouncePanelController : NetworkBehaviour
    {
        [SerializeField] RectTransform _announcePanelRectTrancform;
        [SerializeField] CanvasGroup _announcePanelCanvasGroup;
        [SerializeField] TMP_Text _announceText;

        void Start()
        {
            _announcePanelRectTrancform.gameObject.SetActive(false);

        }

#if UNITY_EDITOR
        void Update()
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                ShowAnnouncePanel();
            }
        }
        #endif

        private async void ShowAnnouncePanel()
        {
            _announcePanelRectTrancform.anchoredPosition = new Vector2(0,700);
            _announcePanelRectTrancform.gameObject.SetActive(true);
            _announcePanelRectTrancform.DOAnchorPosY(-540, 1f).SetEase(Ease.OutBounce);
            SEManager.Instance.Play(SEName.Kekkahappyooo);
            await UniTask.Delay(TimeSpan.FromSeconds(3f));

            _announcePanelRectTrancform.DOAnchorPosY(700, 1f).SetEase(Ease.InBack).
            OnComplete(() =>
            {
                _announcePanelRectTrancform.gameObject.SetActive(false);
            });

        }

        [Rpc(target: SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
        public void ShowAnnouncePanelRpc()
        {
            ShowAnnouncePanel();
        }
    }
}

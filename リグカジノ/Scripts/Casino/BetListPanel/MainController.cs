using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ko.Casino.BetListPanel
{
    public class MainController : NetworkBehaviour, Key.Casino.BetListPanel.IController
    {
        [SerializeField] private CanvasGroup _betpanelCanvasGroup;
        [SerializeField] private RectTransform _betPanel;
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private CanvasGroup _betList;
        [SerializeField] private RectTransform _betListPrefab;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _betAmount;

        Sequence seq = null;
        private List<RectTransform> _spawnedLists = new();

        private void Awake()
        {
            _betpanelCanvasGroup.alpha = 0;
        }
        #if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                ShowBetListPanel(new int[] { 1000, 2000, 5000, 10000, 20000, 20000, 20000, 20000 }); // 仮のベット金額を渡しています
            }
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                HideBetListPanel();
            }
        }
        #endif

        private void ShowBetListPanel(int[] betAmounts)
        {
            // 初期化
                _betpanelCanvasGroup.alpha = 0;
                _betPanel.anchoredPosition = new Vector2(0, 500);
                _headerText.text = "賭け金一覧";

                // パネルをフェードインさせる
                seq?.Kill(); // 以前のシーケンスがあれば停止

            // betAmountsを元にベットリストを生成
            GenerateBetList(betAmounts);

            seq = DOTween.Sequence();
            seq.Append(_betpanelCanvasGroup.DOFade(1f, 0.75f))
            .Join(_betPanel.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack));

            for (int i = 0; i < _spawnedLists.Count; i++)
            {
                RectTransform list = _spawnedLists[i];
                CanvasGroup listCanvasGroup = list.GetComponent<CanvasGroup>();

                if (listCanvasGroup == null)
                {
                    listCanvasGroup = list.gameObject.AddComponent<CanvasGroup>();
                }

                listCanvasGroup.alpha = 0f; // フェードインのために最初は透明にしておく
                list.anchoredPosition = new Vector2(1000f, list.anchoredPosition.y); // 最初は右外に配置

                float startTime = i * 0.1f; // 各テキストの開始時間を0.1秒ずつズラす
                float duration = Mathf.Max(0.2f, 1.0f - (0.1f * i)); // 極端に短くなり過ぎないようにする

                seq.Insert(startTime, list.DOAnchorPosX(0, duration).SetEase(Ease.OutCubic));
                seq.Insert(startTime, listCanvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear));
            }
        }

        private void HideBetListPanel()
        {
            seq?.Kill(); // 以前のシーケンスがあれば停止
            seq = DOTween.Sequence();
            seq.Append(_betPanel.DOAnchorPosY(1500, 0.5f).SetEase(Ease.InBack)).AppendCallback(() =>
            {
                DestroySpawnedLists();
            });

        }

        [Rpc(SendTo.Everyone)]
        public void ShowClientRpc(int[] betAmounts)
        {
            Logger.LoggerManager.Log($"BetListPanel shown with bet amounts: {string.Join(", ", betAmounts)}");
            ShowBetListPanel(betAmounts);
        }

        [Rpc(SendTo.Everyone)]
        public void HideClientRpc()
        {
            Logger.LoggerManager.Log("BetListPanel hidden");
            HideBetListPanel();
        }

        private void GenerateBetList(int[] betAmounts)
        {
            // 以前に生成したものがあれば削除してリセット
            foreach (var obj in _spawnedLists)
            {
                if (obj != null) Destroy(obj.gameObject);
            }
            _spawnedLists.Clear();

            for (int i = 0; i < betAmounts.Length; i++)
            {
                RectTransform newList = Instantiate(_betListPrefab, _betPanel);
                CanvasGroup canvasGroup = newList.GetComponent<CanvasGroup>();
                TMP_Text[] texts = newList.GetComponentsInChildren<TMP_Text>(true);

                TMP_Text nameText = null;
                TMP_Text betAmountText = null;
                foreach (var text in texts)
                {
                    if (nameText == null && _name != null && text.name == _name.name)
                    {
                        nameText = text;
                    }

                    if (betAmountText == null && _betAmount != null && text.name == _betAmount.name)
                    {
                        betAmountText = text;
                    }
                }

                newList.gameObject.SetActive(true); // 元が非表示なら表示にする
                canvasGroup.alpha = 0f; // フェードインのために最初は透明にしておく
                newList.anchoredPosition = new Vector2(0, 150 - i * 60); // 最初は右外に配置

                if (nameText != null)
                {
                    nameText.text = $"プレイヤー{i+1}";
                }

                if (betAmountText != null)
                {
                    betAmountText.text = betAmounts[i].ToString("N0");
                }

                // 管理用リストに追加
                _spawnedLists.Add(newList);
            }
        }

        private void DestroySpawnedLists()
        {
            foreach (var obj in _spawnedLists)
            {
                if (obj != null) Destroy(obj.gameObject);
            }
            _spawnedLists.Clear();
        }
    }
}

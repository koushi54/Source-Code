using Unity.Netcode;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Ko.Result
{
    public class RankingUIController : NetworkBehaviour
    {
        [SerializeField] private NetworkObject[] _rankingUIPrefab; // ランキングUIの配列（1位、2位、3位）
        [SerializeField] private Vector3 _uiLocalOffset = new Vector3(0f, 1f, -0.5f);
        private GameObject[] _rankingUIInstances;
        private Vector3[] _rankingUIBaseScales;

        private void ClearRankingUIInstances()
        {
            if (_rankingUIInstances == null) return;

            foreach (var ui in _rankingUIInstances)
            {
                if (ui == null) continue;
                Destroy(ui);
            }
        }
        

        // ランキングUIを上位プレイヤーを親として生成する
        public void GenerateRankingUI(NetworkObject[] topPlayerObjects)
        {
            ClearRankingUIInstances();
            _rankingUIInstances = new GameObject[_rankingUIPrefab.Length];
            _rankingUIBaseScales = new Vector3[_rankingUIPrefab.Length];

            for (int i = 0; i < _rankingUIPrefab.Length; i++)
            {
                if (i < topPlayerObjects.Length && topPlayerObjects[i] != null)
                {
                    var uiInstance = Instantiate(_rankingUIPrefab[i].gameObject, topPlayerObjects[i].transform);
                    // プレイヤー基準の相対座標 (0, 2, 0) にUIを配置する
                    uiInstance.transform.localPosition = _uiLocalOffset;
                    uiInstance.SetActive(false); // 最初は非表示にしておく
                    _rankingUIInstances[i] = uiInstance;
                    _rankingUIBaseScales[i] = uiInstance.transform.localScale;
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        public void GenerateRankingUIRpc(NetworkObjectReference[] topPlayerObjects)
        {
            NetworkObject[] resolvedObjects = new NetworkObject[topPlayerObjects.Length];
            for (int i = 0; i < topPlayerObjects.Length; i++)
            {
                resolvedObjects[i] = topPlayerObjects[i].TryGet(out NetworkObject obj) ? obj : null;
            }

            GenerateRankingUI(resolvedObjects);
        }

        // ランキングUIを表示する
        public async UniTask ShowRankingUI(int rank)
        {
            int rankIndex = rank - 1;
            if (_rankingUIInstances == null || rankIndex < 0 || rankIndex >= _rankingUIInstances.Length) return;

            var ui = _rankingUIInstances[rankIndex];
            if (ui == null) return;

            ui.SetActive(true);

            Vector3 baseScale = (_rankingUIBaseScales != null && rankIndex < _rankingUIBaseScales.Length && _rankingUIBaseScales[rankIndex] != Vector3.zero)
                ? _rankingUIBaseScales[rankIndex]
                : Vector3.one;

            Transform uiTransform = ui.transform;
            uiTransform.DOKill();
            uiTransform.localScale = baseScale;
            uiTransform.localRotation = Quaternion.identity;
            
            AnimateRankingUI(uiTransform);
        }

        public void AnimateRankingUI(Transform uiTransform)
        {
            Sequence animationSequence = DOTween.Sequence();
            animationSequence.Append(uiTransform.DOScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutBack))
                .Join(uiTransform.DORotate(new Vector3(0f, 1035f, 0f), 0.5f, RotateMode.FastBeyond360))
                .Append(uiTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack));
        }

        [Rpc(SendTo.Everyone)]
        public void AnimateAllRankingUIsRpc()
        {
            if (_rankingUIInstances == null) return;

            foreach (var ui in _rankingUIInstances)
            {
                if (ui != null && ui.activeSelf)
                {
                    AnimateRankingUI(ui.transform);
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        public void ShowRankingUIRpc(int rank)
        {
            ShowRankingUI(rank).Forget();
        }

        public void HideAllRankingUI()
        {
            if (_rankingUIInstances == null) return;

            foreach (var ui in _rankingUIInstances)
            {
                if (ui == null) continue;
                ui.SetActive(false);
            }
        }

        [Rpc(SendTo.Everyone)]
        public void HideAllRankingUIRpc()
        {
            HideAllRankingUI();
        }
    }
}

using TMPro;
using UnityEngine;

namespace Ko.StageSelect
{
    public sealed class StageSelectItem : MonoBehaviour
    {
        [Header("ステージ")]
        [SerializeField]
        private StageDefinition stageDefinition;

        [Header("見た目")]
        [SerializeField]
        private Transform viewRoot;

        [Header("ステージ状態ごとの見た目")]
        [SerializeField]
        private GameObject lockedViewPrefab;

        [SerializeField]
        private GameObject availableViewPrefab;

        [SerializeField]
        private GameObject clearedViewPrefab;
        [Header("ステージの名前")]
        [SerializeField] TMP_Text _stageNameText;

        private StageSelectManager _manager;
        private GameObject _currentView;

        public StageDefinition Definition => stageDefinition;
        public StageState State { get; private set; }
        public Transform ViewRoot => viewRoot;
        public TMP_Text StageNameText => _stageNameText;

        public void Initialize(StageSelectManager manager)
        {
            _manager = manager;
            if (_stageNameText != null && stageDefinition != null)
            {
                _stageNameText.text = stageDefinition.DisplayName;
            }
        }

        public void ApplyState(StageState state)
        {
            if (_currentView != null && State == state)
            {
                return;
            }

            State = state;

            if (_currentView != null)
            {
                Destroy(_currentView);
                _currentView = null;
            }

            GameObject viewPrefab = GetViewPrefab(state);

            if (viewPrefab == null)
            {
                Debug.LogError(
                    $"{name}の{state}用Prefabが設定されていません。",
                    this);

                return;
            }

            if (viewRoot == null)
            {
                Debug.LogError(
                    $"{name}にViewRootが設定されていません。",
                    this);

                return;
            }

            _currentView = Instantiate(viewPrefab, viewRoot, false);
        }

        private GameObject GetViewPrefab(StageState state)
        {
            return state switch
            {
                StageState.Locked => lockedViewPrefab,
                StageState.Available => availableViewPrefab,
                StageState.Cleared => clearedViewPrefab,
                _ => null
            };
        }

        private void OnMouseEnter()
        {
            _manager?.OnStagePointerEntered(this);
        }

        private void OnMouseExit()
        {
            _manager?.OnStagePointerExited(this);
        }

        private void OnMouseDown()
        {
            _manager?.OnStageClicked(this);
        }
    }
}
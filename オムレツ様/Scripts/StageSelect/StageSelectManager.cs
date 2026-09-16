using Key.Core;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Project.Key.ScreenFade;

namespace Ko.StageSelect
{
    public sealed class StageSelectManager : MonoBehaviour
    {
        [SerializeField] private StageSelectItem[] _stageItems;

        [SerializeField] private StageSelectAnimator _stageSelectAnimator;

        private StageProgressStore _progressStore;
        private StageSelectItem _selectedItem;
        private bool _isLoading;

        private void Start()
        {
            _progressStore = new StageProgressStore();

            InitializeItems();
            RefreshStageStates();
            _stageSelectAnimator?.InitializeConfirmButton();
        }

        private void InitializeItems()
        {
            foreach (StageSelectItem item in _stageItems)
            {
                if (item == null)
                {
                    continue;
                }

                item.Initialize(this);
            }
        }

        private void RefreshStageStates()
        {
            foreach (StageSelectItem item in _stageItems)
            {
                if (item == null || item.Definition == null)
                {
                    continue;
                }

                StageState state = CalculateState(item.Definition);
                item.ApplyState(state);
            }
        }

        private StageState CalculateState(StageDefinition stage)
        {
            if (_progressStore.IsCleared(stage.StageId))
            {
                return StageState.Cleared;
            }

            StageDefinition[] prerequisites = stage.PrerequisiteStages;

            if (prerequisites == null || prerequisites.Length == 0)
            {
                return StageState.Available;
            }

            foreach (StageDefinition prerequisite in prerequisites)
            {
                if (prerequisite == null)
                {
                    Debug.LogError($"{stage.name}の開放条件に未設定の要素があります。", stage);
                    return StageState.Locked;
                }

                if (!_progressStore.IsCleared(prerequisite.StageId))
                {
                    return StageState.Locked;
                }
            }

            return StageState.Available;
        }

        public void OnStageClicked(StageSelectItem item)
        {
            if (_isLoading || _selectedItem != null)
            {
                return;
            }

            if (item == null || item.Definition == null)
            {
                return;
            }

            if (item.State == StageState.Locked)
            {
                Debug.Log($"{item.Definition.DisplayName}は未開放です。");
                _stageSelectAnimator?.PlayLockedStageClickedAnimation(item);
                return;
            }

            _selectedItem = item;
            _stageSelectAnimator?.PlaySelectedAnimation(item);
        }
        public async UniTask LoadSelectedStageAsync()
        {
            if (_selectedItem == null || _isLoading)
            {
                return;
            }

            Generated.SceneName sceneName = _selectedItem.Definition.SceneName;

            if (sceneName == default)
            {
                Debug.LogError($"{_selectedItem.Definition.name}にシーン名が設定されていません。", _selectedItem.Definition);
                return;
            }

            _isLoading = true;

            await FadeManager.Instance.FadeOutAsync(1f);
            await AddictiveSceneMananger.UnloadAddictiveScene(Generated.SceneName.StageSelect);
            await AddictiveSceneMananger.LoadAddictiveScene(sceneName);
        }

        public void OnStagePointerEntered(StageSelectItem item)
        {
            if (_selectedItem != null)
            {
                return;
            }

            _stageSelectAnimator?.PlayPointerEnterAnimation(item);
        }

        public void OnStagePointerExited(StageSelectItem item)
        {
            if (_selectedItem != null)
            {
                return;
            }
            _stageSelectAnimator?.PlayPointerExitAnimation(item);
        }

        public void OnConfirmButtonPressed()
        {
            LoadSelectedStageAsync().Forget();
        }

        public void OnCancelButtonPressed()
        {
            if (_selectedItem == null || _isLoading)
            {
                return;
            }

            StageSelectItem canceledItem = _selectedItem;
            _selectedItem = null;
            _stageSelectAnimator?.PlayPointerExitAnimation(canceledItem);
            _stageSelectAnimator?.PlayCameraReturnAnimation();
            _stageSelectAnimator?.PlayConfirmButtonExitAnimation();
        }
    }
}
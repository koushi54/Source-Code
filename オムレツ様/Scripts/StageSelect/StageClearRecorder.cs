using UnityEngine;

namespace Ko.StageSelect
{
    public sealed class StageClearRecorder : MonoBehaviour
    {
        [SerializeField] private StageDefinition _currentStage;

        private StageProgressStore _progressStore;

        private void Awake()
        {
            InitializeProgressStore();
        }

        //　ステージクリア時に呼び出すことで、クリア済みとして保存する
        [ContextMenu("Debug/現在のステージをクリア済みにする")]
        public void RecordClear()
        {
            if (_currentStage == null)
            {
                Debug.LogError(
                    "現在のStageDefinitionが設定されていません。",
                    this);

                return;
            }

            InitializeProgressStore();

            _progressStore.MarkCleared(_currentStage.StageId);

            Debug.Log(
                $"{_currentStage.DisplayName}をクリア済みとして保存しました。",
                this);
        }

        [ContextMenu("Debug/すべての進捗をリセットする")]
        private void ResetProgress()
        {
            InitializeProgressStore();
            _progressStore.ResetProgress();
            Debug.Log("すべてのステージ進捗をリセットしました。", this);
        }

        private void InitializeProgressStore()
        {
            _progressStore ??= new StageProgressStore();
        }
    }
}
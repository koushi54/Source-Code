using System;
using UnityEngine;

namespace Ko.StageSelect
{
    public sealed class StageProgressStore
    {
        private const string SaveKey = "2weeksGameJamVol3.StageProgress.v1";

        private StageProgressSaveData _saveData;

        public StageProgressStore()
        {
            _saveData = Load();
        }

        public bool IsCleared(string stageId)
        {
            return _saveData.clearedStageIds.Contains(stageId);
        }

        public void MarkCleared(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
            {
                Debug.LogError("空のStageIdは保存できません。");
                return;
            }

            if (IsCleared(stageId))
            {
                return;
            }

            _saveData.clearedStageIds.Add(stageId);
            Save();
        }

        public void ResetProgress()
        {
            _saveData = new StageProgressSaveData();

            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        private StageProgressSaveData Load()
        {
            string json = PlayerPrefs.GetString(SaveKey, string.Empty);

            if (string.IsNullOrEmpty(json))
            {
                return new StageProgressSaveData();
            }

            try
            {
                StageProgressSaveData saveData =
                    JsonUtility.FromJson<StageProgressSaveData>(json);

                if (saveData == null)
                {
                    return new StageProgressSaveData();
                }

                saveData.clearedStageIds ??= new();
                return saveData;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"ステージ進捗の読み込みに失敗しました。\n{exception}");

                return new StageProgressSaveData();
            }
        }

        private void Save()
        {
            string json = JsonUtility.ToJson(_saveData);

            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }
    }
}
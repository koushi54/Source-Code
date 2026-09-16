using UnityEngine;

namespace Ko.StageSelect
{
    [CreateAssetMenu(fileName = "StageDefinition", menuName = "ScriptableObjects/StageDefinition", order = 1)]
    public class StageDefinition : ScriptableObject
    {
        /// <summary>
        /// ステージを識別するためのID
        /// </summary>
        [SerializeField] private string _stageId;
        /// <summary>
        /// 表示するUI上のステージ名
        /// </summary>
        [SerializeField] private string _displayName;
        /// <summary>
        /// ステージのシーン名
        /// </summary>
        [SerializeField] private Generated.SceneName _sceneName;
        /// <summary>
        /// このステージを解放するためにクリアする必要のあるステージ
        /// </summary>  
        [SerializeField] private StageDefinition[] _prerequisiteStages;

        public string StageId => _stageId;
        public string DisplayName => _displayName;
        public Generated.SceneName SceneName => _sceneName;
        public StageDefinition[] PrerequisiteStages => _prerequisiteStages;
    }
}

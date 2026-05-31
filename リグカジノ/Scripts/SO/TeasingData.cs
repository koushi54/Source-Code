using UnityEngine;

namespace Ko.SO
{
    [CreateAssetMenu(fileName = "TeasingData", menuName = "ScriptableObjects/TeasingData", order = 3)]
    public class TeasingData : ScriptableObject
    {
        [SerializeField, Header("からかい文句")] private string[] _teasingTexts;
        [SerializeField, Header("からかいのセリフの表示時間")] private float _teasingTextDisplayTime;
        [SerializeField, Header("からかいのセリフの表示位置")] private Vector2[] _teasingTextPositions;

        /// <summary>
        /// からかいのセリフ
        /// </summary>
        public string[] TeasingTexts => _teasingTexts;
        
        /// <summary>
        /// からかいのセリフの表示時間
        /// </summary>
        public float TeasingTextDisplayTime => _teasingTextDisplayTime;

        /// <summary>
        /// からかいのセリフの表示位置
        /// </summary>
        public Vector2[] TeasingTextPosition => _teasingTextPositions;
    }
}
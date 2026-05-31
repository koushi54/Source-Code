using UnityEngine;

namespace Ko.SO
{
    [System.Serializable]
    public class QuizDatum
    {
        [SerializeField, Header("問題文")] private string _question;
        [SerializeField, Header("選択肢")] private string[] _options;
        [SerializeField, Header("正解番号(１から)")] private int _correctOption;
        [SerializeField, Header("難易度")] private int _difficultyLevel;
        [SerializeField, Header("倍率")] private float _magnification;
        [SerializeField, Header("解答解説"), Multiline] private string _explanation;

        /// <summary>
        /// 問題文
        /// </summary>
        public string Question => _question;
        /// <summary>
        /// 選択肢
        /// </summary>
        public string[] Options => _options;
        /// <summary>
        /// 正解番号(１から)
        /// </summary>
        public int CorrectOption => _correctOption;

        /// <summary>
        /// 難易度
        /// </summary>
        public int DifficultyLevel => _difficultyLevel;

        /// <summary>
        /// 倍率
        /// </summary>
        public float Magnification => _magnification;

        /// <summary>
        /// 解答解説
        /// </summary>
        public string Explanation => _explanation;
    }
}




using UnityEngine;

namespace Ko.SO
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 2)]
    public class GameData : ScriptableObject
    {
        [SerializeField, Header("現在の問題番号")] private int _currentProblemNumber;
        [SerializeField, Header("現在の所持金")] private int[] _currentBalance;
        [SerializeField, Header("最初の所持金")] private int _initialBalance;

        [SerializeField, Header("掛け金の設定時間")] private float _bettingTime;
        [SerializeField, Header("クイズの制限時間")] private float _quizAnswerTime;
        [SerializeField, Header("ミニゲームの時間")] private float _miniGameTime;

        /// <summary>
        /// 現在の問題番号
        /// </summary>
        public int CurrentProblemNumber => _currentProblemNumber;
        /// <summary>
        /// 現在の所持金
        /// </summary>
        public int[] CurrentBalance => _currentBalance;
        /// <summary>
        /// 最初の所持金
        /// </summary>
        public int InitialBalance => _initialBalance;
        /// <summary>
        /// 掛け金の設定時間
        /// </summary>
        public float BettingTime => _bettingTime;
        /// <summary>
        /// クイズの制限時間
        /// </summary>
        public float QuizAnswerTime => _quizAnswerTime;
        /// <summary>
        /// ミニゲームの時間
        /// </summary>
        public float MiniGameTime => _miniGameTime;

        public void ChangeCurrentProblemNumber(int newNumber)
        {
            _currentProblemNumber = newNumber;
        }

        public void ChangeCurrentBalance(int[] newBalance)
        {
            _currentBalance = newBalance;
        }
    }
}

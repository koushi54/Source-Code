using System.Collections.Generic;
using UnityEngine;

namespace Ko.SO
{
    [CreateAssetMenu(fileName = "QuizDatabase", menuName = "ScriptableObjects/QuizDatabase", order = 1)]
    public class QuizDatabase : ScriptableObject
    {
        [SerializeField] private List<QuizDatum> _quizData;
        public List<QuizDatum> QuizData => _quizData;
    }
}

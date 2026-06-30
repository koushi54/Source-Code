using R3;
using UnityEngine;

namespace Ko.InGame
{
    public class StrengthCurve : MonoBehaviour
    {
        [SerializeField] private Kakky.PlayerParamData _playerParamData;
        [SerializeField] private AnimationCurve _strengthCurve;

        public float GetStrength(float remainAmount)
        {
            remainAmount = Mathf.Clamp01(remainAmount);
            return _strengthCurve.Evaluate(remainAmount);
        }

        void Start()
        {
            _playerParamData.RemainAmount.Subscribe(remainAmount =>
            {
                var strength = GetStrength(remainAmount);
                _playerParamData.Strength.Value = strength;
            }).AddTo(this);
        }
    }
}
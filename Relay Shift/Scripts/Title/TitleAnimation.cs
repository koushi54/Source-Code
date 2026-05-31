using UnityEngine;
using TMPro;
using LitMotion;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using AudioManager.BGM;

namespace Ko
{
    public class TitleAnimation : MonoBehaviour
    {
        [Header("バトンのGameObject")]
        [SerializeField] private GameObject[] _baton;
        [Header("UI")]
        [SerializeField] private TMP_Text[] _optionTexts;
        [SerializeField] private Image _titleImage;
        private MotionHandle[] _batonMotion;

        private void Start()
        {
            BGMManager.Instance.Play(BGMName.Title);
            InitializeUI();
            RunStartAnimation().Forget();
        }

        private async UniTask RunStartAnimation()
        {
            await PlayInitialBatonAnimation();
            ScreenEffect.Instance.Flash(1f).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            ShowUI();
            PlayBatonAnimation();
        }

        private void InitializeUI()
        {
            for (int i = 0; i < _optionTexts.Length; i++)
            {
                _optionTexts[i].gameObject.SetActive(false);
            }
            _titleImage.gameObject.SetActive(false);
        }

        private void ShowUI()
        {
            _titleImage.gameObject.SetActive(true);
            foreach (var text in _optionTexts)
            {
                text.gameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                ScreenEffect.Instance.Flash(1f).Forget();
            }
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                ScreenEffect.Instance.FadeIn(1f).Forget();
            }
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                ScreenEffect.Instance.FadeOut(1f).Forget();
            }
        }
#endif

        private async UniTask PlayInitialBatonAnimation()
        {
            MotionHandle[] initialMotion = new MotionHandle[_baton.Length];
            initialMotion[0] = LMotion.Create(-10f, 0f, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(_baton[0].transform, (val, target) =>
                {
                    target.transform.position = new Vector3(target.transform.position.x, val, target.transform.position.z);
                });
            initialMotion[1] = LMotion.Create(-10f, 0f, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(_baton[1].transform, (val, target) =>
                {
                    target.transform.position = new Vector3(target.transform.position.x, val, target.transform.position.z);
                });
            await initialMotion[0].ToUniTask();
            await initialMotion[1].ToUniTask();

            MotionHandle[] splitMotion = new MotionHandle[_baton.Length];
            splitMotion[0] = LMotion.Create(0f, 5.5f, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(_baton[0].transform, (val, target) =>
                {
                    target.transform.position = new Vector3(val, target.transform.position.y, target.transform.position.z);
                });
            splitMotion[1] = LMotion.Create(0f, -5.5f, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(_baton[1].transform, (val, target) =>
                {
                    target.transform.position = new Vector3(val, target.transform.position.y, target.transform.position.z);
                });
            await splitMotion[0].ToUniTask();
            await splitMotion[1].ToUniTask();
        }

        public void PlaySelectAnimation(TMP_Text text)
        {
            LMotion.Create(1f, 0f, 0.1f)
                .WithLoops(5, LoopType.Yoyo)
                .WithEase(Ease.Linear)
                .WithOnComplete(() => text.color = new Color(0, 0, 0, 1))
                .Bind(text, (val, target) =>
                {
                    // 元の色（RGB）を維持したまま、アルファ値だけを更新
                    Color c = target.color;
                    c.a = val;
                    target.color = c;
                });
            LMotion.Create(0f, 10f, 1f)
                .WithEase(Ease.OutQuad)
                .Bind(text, (val, target) =>
                {
                    text.characterSpacing = val;
                });
        }

        private void PlayBatonAnimation()
        {
            _batonMotion = new MotionHandle[_baton.Length];
            for (int i = 0; i < _baton.Length; i++)
            {
                // バトンを回転させるアニメーション
                _batonMotion[i] = LMotion.Create(0f, 360f, 2.5f)
                    .WithEase(Ease.Linear)
                    .WithLoops(-1, LoopType.Restart)
                    .Bind(_baton[i].transform, (val, target) =>
                    {
                        target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.x, val, target.localRotation.eulerAngles.z);
                    })
                    .AddTo(this);
            }
        }

    }
}

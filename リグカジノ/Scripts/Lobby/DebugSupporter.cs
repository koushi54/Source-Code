using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Alchemy.Inspector;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Key.Core.State;

namespace Ko.Lobby
{
    public class DebugSupporter : NetworkBehaviour
    {
        [SerializeField, Header("デバッグ用のUI")] private Image _debugUIImage;
        [Title("更新用のUI")]
        [SerializeField] private Image _playerDataImagePrefab;
        [SerializeField] private TMP_Text _playerText;
        [SerializeField] private TMP_InputField _newPossessionInputField;
        private readonly List<Image> _playerUIList = new List<Image>();
        private readonly List<TMP_InputField> _newPossessionInputFieldList = new List<TMP_InputField>();
        private Key.Core.Network.ServerManager _serverManager;
        private Cheat.Controller.CommandManager _commandManager;


        public void Start()
        {
            _debugUIImage.gameObject.SetActive(false);
            _serverManager = Key.Core.Network.ServerManager.Instance;
            _commandManager = FindAnyObjectByType<Cheat.Controller.CommandManager>();
            _commandManager.OnDebugCommandComplete += OnDebugCommandComplete;
        }

        private void Initialize()
        {
            for (int i = 0; i < _playerUIList.Count; i++)
            {
                if (_playerUIList[i] != null)
                {
                    Destroy(_playerUIList[i].gameObject);
                }
            }

            _playerUIList.Clear();

            //プレイヤー数分のPrefabを生成し、配置する
            _newPossessionInputFieldList.Clear();
            for (int i = 0; i < _serverManager.PlayerDataList.Count; i++)
            {
                Image playerUI = Instantiate(_playerDataImagePrefab, _debugUIImage.transform);
                _playerText = playerUI.GetComponentInChildren<TMP_Text>();
                _newPossessionInputField = playerUI.GetComponentInChildren<TMP_InputField>();
                _playerUIList.Add(playerUI);
                playerUI.transform.localPosition = new Vector3(150, 150 - (50 * i), 0); // 縦に並べる
                _playerText.text = $"プレイヤー {i + 1}";

                if (_newPossessionInputField != null)
                {
                    _newPossessionInputField.text = _serverManager.PlayerDataList[i].Money.Value.ToString();
                    _newPossessionInputFieldList.Add(_newPossessionInputField);
                }
                else
                {
                    Logger.LoggerManager.Log($"<color=red>[DebugSupporter]</color> プレイヤー {i + 1} のInputFieldが見つかりませんでした。");
                    _newPossessionInputFieldList.Add(null);
                }
            }
        }

        public async void OnGoToShipButtonClicked()
        {
            Key.Core.Network.ServerManager.Instance.ResetSceneLoadedClients();
            NetworkManager.Singleton.SceneManager.LoadScene("Ship", LoadSceneMode.Single); // 遷移先のシーン名を指定してください
            Logger.LoggerManager.Log("<color=yellow>[ReadyState]</color> 全員がシーンの読み込みを完了するのを待っています...");
            await Key.Core.Network.ServerManager.Instance.WaitForAllClientsToLoadScene();
            Logger.LoggerManager.Log("<color=yellow>[ReadyState]</color> 全員がシーンの読み込みを完了しました！ゲームを開始します！");
            Key.Core.State.GameStateMachine.Instance.ChangeSceneState(SceneState.Ship, ShipState.Playing);
        }

        public async void OnGoToResultButtonClicked()
        {
            Key.Core.Network.ServerManager.Instance.ResetSceneLoadedClients();
            NetworkManager.Singleton.SceneManager.LoadScene("Result", LoadSceneMode.Single); // 遷移先のシーン名を指定してください
            Logger.LoggerManager.Log("<color=yellow>[ReadyState]</color> 全員がシーンの読み込みを完了するのを待っています...");
            await Key.Core.Network.ServerManager.Instance.WaitForAllClientsToLoadScene();
            Logger.LoggerManager.Log("<color=yellow>[ReadyState]</color> 全員がシーンの読み込みを完了しました！ゲームを開始します！");
            Key.Core.State.GameStateMachine.Instance.ChangeSceneState(SceneState.Result, ResultState.Resulting);
            Key.Core.Network.ServerManager.Instance.UpdatePlayerState(Key.Player.PlayerState.Result);
            Key.Core.Network.ServerManager.Instance.UpdatePlayerResultState(Key.Player.ResultState.Waiting);
        }

        public void OnUpdatePossessionButtonClicked()
        {
            if (!IsServer)
            {
                Logger.LoggerManager.Log("<color=red>[DebugSupporter]</color> 所持金更新はサーバー側でのみ実行できます。");
                return;
            }

            // 生成したUIのInputFieldに入力された値をそれぞれに対応するプレイヤーの持ち金に更新する
            int targetCount = Mathf.Min(_serverManager.PlayerDataList.Count, _newPossessionInputFieldList.Count);
            int updatedCount = 0;

            for (int i = 0; i < targetCount; i++)
            {
                TMP_InputField inputField = _newPossessionInputFieldList[i];
                if (inputField == null)
                {
                    Logger.LoggerManager.Log($"<color=red>[DebugSupporter]</color> プレイヤー {i + 1} のInputField参照がありません。");
                    continue;
                }

                string inputText = inputField.text.Trim();
                if (string.IsNullOrEmpty(inputText))
                {
                    continue;
                }

                if (!int.TryParse(inputText, out int newMoney) || newMoney < 0)
                {
                    Logger.LoggerManager.Log($"<color=red>[DebugSupporter]</color> プレイヤー {i + 1} の入力値 '{inputText}' は無効です。0以上の整数を入力してください。");
                    continue;
                }

                var playerData = _serverManager.PlayerDataList[i];
                playerData.Money.Value = newMoney;
                updatedCount++;
                Logger.LoggerManager.Log($"<color=green>[DebugSupporter]</color> クライアントID {playerData.OwnerClientId} の所持金を {newMoney} に更新しました。");
            }

            Logger.LoggerManager.Log($"<color=green>[DebugSupporter]</color> 所持金更新が完了しました。更新件数: {updatedCount}/{targetCount}");
        }

        ///
        /// コマンドが入力されたら、debugUIを表示する
        ///
        private void OnDebugCommandComplete()
        {
            Initialize();
            _debugUIImage.gameObject.SetActive(true);
            Logger.LoggerManager.Log($"デバッグコマンドが成功しました。デバッグUIを表示します。生成UI数: {_playerUIList.Count}");
        }
    }
}

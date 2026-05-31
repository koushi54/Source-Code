using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace Ko.Casino.PlayerPositionSetter
{
    public class MainController : NetworkBehaviour, Key.Casino.PlayerPositionSetter.IController
    {
        [SerializeField] private GameObject[] _spawnPoints;

        #if UNITY_EDITOR
        private void Update()
        {
            if (!IsServer) return;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SetAllPlayerPositions();
            }
        }
        #endif

        public void SetAllPlayerPositions()
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject.TryGetComponent(out Transform playerTransform))
                {
                    int index = (int)((client.ClientId - 1) % (ulong)_spawnPoints.Length);
                    Vector3 spawnPosition = _spawnPoints[index].transform.position;
                    playerTransform.position = spawnPosition;
                    playerTransform.rotation = _spawnPoints[index].transform.rotation;
                    Logger.LoggerManager.Log($"<color=green>[MainController]</color> プレイヤー {client.ClientId} を位置 {spawnPosition} に移動させました。");
                }
            }
        }
    }
}

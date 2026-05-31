using Unity.Netcode;
using UnityEngine;

namespace Ko.Result
{
    public class EffectManager : NetworkBehaviour
    {
        [SerializeField] private ParticleSystem[] _fireworks;

        [Rpc(SendTo.Everyone)]
        public void PlayFireworksRpc()
        {
            foreach (var firework in _fireworks)
            {
                firework.Play();
            }
        }
    }
}

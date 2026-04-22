using MultiplayerZombies.Core;
using MultiplayerZombies.Player;
using TMPro;
using UnityEngine;

namespace MultiplayerZombies.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private TMP_Text waveText;

        private PlayerHealth _localHealth;
        private PlayerShooting _localShooting;
        private float _nextLookupTime;

        private void Update()
        {
            EnsureLocalPlayer();

            if (_localHealth != null && healthText != null)
            {
                healthText.text = $"Health: {_localHealth.Health}";
            }

            if (_localShooting != null && ammoText != null)
            {
                ammoText.text = $"Ammo: {_localShooting.Ammo}";
            }

            if (GameManager.Instance != null && waveText != null)
            {
                waveText.text = $"Wave: {GameManager.Instance.Wave}";
            }
        }

        private void EnsureLocalPlayer()
        {
            if (_localHealth != null && _localShooting != null)
            {
                return;
            }

            if (Time.unscaledTime < _nextLookupTime)
            {
                return;
            }

            _nextLookupTime = Time.unscaledTime + 3f;

            var players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player != null && player.Object != null && player.Object.HasInputAuthority)
                {
                    _localHealth = player;
                    _localShooting = player.GetComponent<PlayerShooting>();
                    return;
                }
            }
        }
    }
}

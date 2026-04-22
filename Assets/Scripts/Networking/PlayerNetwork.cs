using Fusion;
using UnityEngine;

namespace MultiplayerZombies.Networking
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private bool showAuthorityDebug = true;
        [SerializeField] private Vector3 debugOffset = new(0f, 2f, 0f);

        private static readonly GUIStyle LabelStyle = new() { normal = { textColor = Color.white } };

        private void OnGUI()
        {
            if (!showAuthorityDebug || Object == null || Camera.main == null)
            {
                return;
            }

            var worldPosition = transform.position + debugOffset;
            var screen = Camera.main.WorldToScreenPoint(worldPosition);
            if (screen.z < 0f)
            {
                return;
            }

            var text = $"SA:{Object.HasStateAuthority} IA:{Object.HasInputAuthority}";
            var rect = new Rect(screen.x - 70f, Screen.height - screen.y, 180f, 20f);
            GUI.Label(rect, text, LabelStyle);
        }
    }
}

using Fusion;
using UnityEngine;

namespace MultiplayerZombies.Player
{
    public struct PlayerInputData : INetworkInput
    {
        public Vector2 Move;
        public Vector2 Look;
        public NetworkBool Fire;
    }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 720f;

        [Networked] public float RecentMovementMagnitude { get; private set; }

        private CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public static PlayerInputData CaptureInput()
        {
            var move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            var look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            return new PlayerInputData
            {
                Move = Vector2.ClampMagnitude(move, 1f),
                Look = look,
                Fire = Input.GetButton("Fire1")
            };
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out PlayerInputData input))
            {
                return;
            }

            var moveInput = new Vector3(input.Move.x, 0f, input.Move.y);
            var worldMove = moveInput.normalized * (moveSpeed * Runner.DeltaTime);
            if (_characterController != null)
            {
                _characterController.Move(transform.TransformDirection(worldMove));
            }
            else
            {
                transform.position += transform.TransformDirection(worldMove);
            }

            if (input.Look.x != 0f)
            {
                var delta = input.Look.x * rotationSpeed * Runner.DeltaTime;
                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + delta, 0f);
            }

            if (Object.HasStateAuthority)
            {
                RecentMovementMagnitude = input.Move.magnitude;
            }
        }
    }
}

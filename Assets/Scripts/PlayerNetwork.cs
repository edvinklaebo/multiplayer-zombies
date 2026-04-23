using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : NetworkBehaviour
{
    private CharacterController _controller;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;

    private float _yRotation;

    public override void Spawned()
    {
        _controller = GetComponent<CharacterController>();

        if (!Object.HasInputAuthority) 
            return;
        
        if (Camera.main == null) 
            return;
        
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 1.5f, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out PlayerInputData input)) 
            return;
        
        Move(input);
        Look(input);
        HandleFire(input);
    }

    private void Move(PlayerInputData input)
    {
        var move = new Vector3(input.Move.x, 0, input.Move.y);
        move = transform.TransformDirection(move);

        _controller.Move(move * speed * Runner.DeltaTime);
    }

    private void Look(PlayerInputData input)
    {
        transform.Rotate(Vector3.up * input.Look.x * mouseSensitivity);

        _yRotation -= input.Look.y * mouseSensitivity;
        _yRotation = Mathf.Clamp(_yRotation, -80f, 80f);

        if (Camera.main != null && Camera.main.transform.parent == transform)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(_yRotation, 0, 0);
        }
    }

    private void HandleFire(PlayerInputData input)
    {
        if (input.Fire)
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (!Object.HasStateAuthority) return;

        if (Camera.main != null)
        {
            var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.Raycast(ray, out var hit, 100f))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);

                var zombie = hit.collider.GetComponent<ZombieController>();
                if (zombie != null)
                {
                    zombie.TakeDamage(1);
                }
            }
        }
    }
}
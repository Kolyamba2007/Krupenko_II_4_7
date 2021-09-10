using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private PlayerControls _playerControls;

    private PlayerData _playerData;
    public int ID { private set => _playerData.ID = value; get => _playerData.ID; }
    public uint Health { private set => _playerData.Health = value; get => _playerData.Health; }
    public Vector3 Position => new Vector3(_playerData.PositionX, transform.position.y, _playerData.PositionZ);

    [SerializeField, Min(0)]
    private float _movementSpeed;

    public event Action<int> Died;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }
    private void Update()
    {
        Vector2 movement = _playerControls.Player.Movement.ReadValue<Vector2>();
        if (movement.x != 0 || movement.y != 0)
        {
            transform.position += new Vector3(movement.x, 0, movement.y) * _movementSpeed * Time.deltaTime;
        }

        Vector2 mousePosition = _playerControls.Player.Pointer.ReadValue<Vector2>();
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, 500f))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z), Vector3.up);
        }
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }
    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public void Hit(DamageArgs args)
    {
        if (Health - args.Value > 0) Health -= args.Value;
        else
        {
            Health = 0;
            Died?.Invoke(args.SourceID);
        }
    }
}

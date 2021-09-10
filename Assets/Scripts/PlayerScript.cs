using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
public class PlayerScript : MonoBehaviour, IPunObservable
{
    private PlayerControls _playerControls;
    private PhotonView _photonView;

    private PlayerData _playerData;
    public int ID { private set => _playerData.ID = value; get => _playerData.ID; }
    public uint Health { private set => _playerData.Health = value; get => _playerData.Health; }

    [SerializeField, Min(0)]
    private float _movementSpeed;
    [SerializeField, Min(0)]
    private float _attackCooldown = 2f;
    private bool CanAttack { set; get; } = true;

    public event Action<PlayerScript> Died;
    public event Action<Vector3> Fire;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine) return;
        _playerControls = new PlayerControls();
    }
    private void Update()
    {
        if (!_photonView.IsMine) return;

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

        var fire = _playerControls.Player.Fire.IsPressed();
        if (fire && CanAttack)
        {
            Fire?.Invoke(transform.forward);
            StartCoroutine(AttackCooldown());
        }
    }
    private IEnumerator AttackCooldown()
    {
        CanAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        CanAttack = true;
    }

    private void OnEnable()
    {
        Enable(true);
    }
    private void OnDisable()
    {
        Enable(false);
    }

    public void Hit(DamageArgs args)
    {
        if (Health - args.Value > 0) Health -= args.Value;
        else
        {
            Health = 0;
            Died?.Invoke(args.Source);
        }
    }
    public void Enable(bool isEnabled)
    {
        CanAttack = isEnabled;
        if (isEnabled) _playerControls.Enable();
        else _playerControls.Disable();
    }

    private void UpdateProperties(PlayerData data)
    {
        Health = data.Health;
        transform.position = new Vector3(data.PositionX, transform.position.y, data.PositionZ);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, data.RotationY, transform.eulerAngles.z);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            UpdateProperties((PlayerData)stream.ReceiveNext());
        }
        else
        {
            stream.SendNext(PlayerData.Parse(this));
        }
    }
}

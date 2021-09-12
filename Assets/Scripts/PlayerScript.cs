using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class PlayerScript : MonoBehaviour, IEquatable<PlayerScript>, IComparable<PlayerScript>, IPunObservable
{
    private PlayerControls _playerControls;
    private PhotonView _photonView;

    private PlayerData _playerData;
    public int ID => _photonView.Owner.ActorNumber;
    public uint Health;
    public string Nickname => _photonView.Owner.NickName;

    [SerializeField, Min(0)]
    private float _movementSpeed;
    [SerializeField, Min(0)]
    private float _attackCooldown = 2f;

    private Rigidbody Rigidbody { set; get; }

    private bool CanAttack { set; get; } = true;

    public event Action<PlayerScript> Died;
    public event Action Fire;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        Rigidbody = GetComponent<Rigidbody>();
        if (_photonView.IsMine)
        {
            _playerControls = new PlayerControls();
        }
        GameManager.RegisterPlayer(this);
    }
    private void Update()
    {
        if (!_photonView.IsMine) return;

        #region Movement
        Vector2 movement = _playerControls.Player.Movement.ReadValue<Vector2>();
        if (movement.x != 0 || movement.y != 0)
        {
            transform.position += new Vector3(movement.x, 0, movement.y) * _movementSpeed * Time.deltaTime;
        }
        #endregion

        #region Rotation
        Vector2 mousePosition = _playerControls.Player.Pointer.ReadValue<Vector2>();
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out var hit, 500f))
        {
            Vector3 targetPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            transform.rotation = targetRotation;
        }
        #endregion

        #region Fire
        var fire = _playerControls.Player.Fire.IsPressed();
        if (fire && CanAttack)
        {
            Fire?.Invoke();
            StartCoroutine(AttackCooldown());
        }
        #endregion
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
    private void Enable(bool isEnabled)
    {
        CanAttack = isEnabled;
        Rigidbody.isKinematic = !isEnabled;
        Rigidbody.detectCollisions = isEnabled;

        if (!_photonView.IsMine) return;
        if (isEnabled) _playerControls.Enable();
        else _playerControls.Disable();
    }

    public void Hit(DamageArgs args)
    {
        if (Health - args.Value > 0) Health -= args.Value;
        else
        {
            Die();
            Died?.Invoke(args.Source);
        }
    }
    private void Die()
    {
        Health = 0;
        Enable(false);
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

    public bool Equals(PlayerScript other) => ID == other.ID;
    public int CompareTo(PlayerScript other)
    {
        if (ID <= other.ID) return -1;
        else return 1;
    }
}

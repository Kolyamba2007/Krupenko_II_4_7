using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
public class ProjectileScript : MonoBehaviour, IPunObservable
{
    private PlayerScript _owner;

    [SerializeField, Min(0)]
    private float _movementSpeed;
    [SerializeField]
    private uint _damage;
    [SerializeField, Min(0)]
    private float _lifetime;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }
    private void Update()
    {
        if (_movementSpeed > 0)
        {
            transform.position += transform.forward * _movementSpeed * Time.deltaTime;
        }
    }

    public void Blast(PlayerScript owner)
    {
        _owner = owner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<PlayerScript>();
            if (player != _owner)
            {
                player.Hit(new DamageArgs(_damage, _owner));
                Destroy(gameObject);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            var data = (ProjectileData)stream.ReceiveNext();
            transform.position = new Vector3(data.PositionX, transform.position.y, data.PositionZ);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, data.RotationY, transform.eulerAngles.z);
        }
        else
        {
            stream.SendNext(ProjectileData.Parse(this));
        }
    }
}

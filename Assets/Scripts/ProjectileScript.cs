using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileScript : MonoBehaviour
{
    private PlayerScript _owner;
    private Vector3 _direction;

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
            transform.Translate(_direction * _movementSpeed * Time.deltaTime);
        }
    }

    public void Blast(Vector3 direction, PlayerScript owner)
    {
        _owner = owner;
        _direction = direction;
        //transform.LookAt(transform.forward + direction, Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerScript>();
        if (player != null && player != _owner)
        {
            player.Hit(new DamageArgs(_damage, _owner));
        }
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform AttachedTarget { set; get; }
    private Vector3 Velocity;

    [SerializeField]
    private Vector2 _offset;

    public void AttachTo(PlayerScript player)
    {
        AttachedTarget = player.transform;
    }

    private void Update()
    {
        if (AttachedTarget != null)
        {
            var targetPosition = new Vector3(AttachedTarget.position.x + _offset.x, transform.position.y, AttachedTarget.position.z + _offset.y);
            var position = Vector3.SmoothDamp(transform.position, targetPosition, ref Velocity, 0.4f);
            transform.position = position;
        }
    }
}

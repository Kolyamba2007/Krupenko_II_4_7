using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            var player = collider.gameObject.GetComponent<PlayerScript>();
            player.Hit(new DamageArgs(player.Health, null));
        }
    }
}

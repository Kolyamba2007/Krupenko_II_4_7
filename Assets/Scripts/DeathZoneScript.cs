using UnityEngine;

public class DeathZoneScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {

        }
    }
}

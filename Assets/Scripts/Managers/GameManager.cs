using UnityEngine;
using Photon.Pun;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private GameObject _projectilePrefab;

    [SerializeField]
    private Transform _unitsRoot;

    private void Awake()
    {
        if (_unitsRoot == null) throw new NullReferenceException("Units Root is null");

        var player = InstantiatePlayer(new Vector3(0, 5, 0));
    }

    private PlayerScript InstantiatePlayer(Vector3 position)
    {
        var player = PhotonNetwork.Instantiate($"Prefabs/{_playerPrefab.name}", position, Quaternion.identity);
        var component = player.GetComponent<PlayerScript>();
        component.Fire += (direction) =>
        {
            var projectile = InstantiateProjectile(player.transform.position + direction * 1.2f);
            projectile.Blast(new Vector3(direction.x, 0, direction.z), component);
        };
        component.Died += (source) =>
        {
            component.Enable(false);
        };
        component.transform.SetParent(_unitsRoot);

        return player.GetComponent<PlayerScript>();
    }
    private ProjectileScript InstantiateProjectile(Vector3 position)
    {
        var projectile = PhotonNetwork.Instantiate($"Prefabs/{_projectilePrefab.name}", position, Quaternion.identity);
        return projectile.GetComponent<ProjectileScript>();
    }
}

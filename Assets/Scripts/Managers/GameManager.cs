using UnityEngine;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager Instance { set; get; }

    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private GameObject _projectilePrefab;

    [SerializeField]
    private Transform _unitsRoot;

    private Leaderboard Leaderboard { set; get; } = new Leaderboard();

    [Header("Controllers"), SerializeField]
    private UIManager _UIManager;
    [Space, SerializeField]
    private CameraController _cameraController;

    private void Awake()
    {
        if (_unitsRoot == null) throw new NullReferenceException("Units Root is null");
        if (Instance == null) Instance = this;

        PhotonPeer.RegisterType(typeof(PlayerData), 0, PlayerData.Serialize, PlayerData.Deserialize);
        PhotonPeer.RegisterType(typeof(ProjectileData), 1, ProjectileData.Serialize, ProjectileData.Deserialize);
    }
    private void Start()
    {
        var player = InstantiatePlayer(new Vector3(0, 2, 0));
        _cameraController.AttachTo(player);
    }

    private void OnPlayerDied(int playerID, int? killerID)
    {
        var player = Leaderboard.PlayersList[playerID];

        if (killerID.HasValue)
        {
            var killer = Leaderboard.PlayersList[killerID.Value];
            Debug.Log($"Player {player.Nickname} was eliminated by {killer.Nickname}!");
        }
        else Debug.Log($"Player {player.Nickname} died!");

        StartCoroutine(DiedCoroutine(player.ID));
    }
    private IEnumerator DiedCoroutine(int playerID)
    {
        Coroutine coroutine;
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            coroutine = StartCoroutine(_UIManager.ShowText($"You are dead!", 3f));
        }
        else coroutine = StartCoroutine(_UIManager.ShowText($"You won!", 3f));
        yield return coroutine;

        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room.");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room.");
    }

    private PlayerScript InstantiatePlayer(Vector3 position)
    {
        var player = PhotonNetwork.Instantiate($"Prefabs/{_playerPrefab.name}", position, Quaternion.identity);
        var component = player.GetComponent<PlayerScript>();

        return player.GetComponent<PlayerScript>();
    }
    private ProjectileScript InstantiateProjectile(PlayerScript player)
    {
        Vector3 position = player.transform.position + player.transform.forward * 1.5f;
        var projectile = PhotonNetwork.Instantiate($"Prefabs/{_projectilePrefab.name}", position, player.transform.rotation);
        return projectile.GetComponent<ProjectileScript>();
    }

    public static void RegisterPlayer(PlayerScript player)
    {
        player.Fire += () =>
        {
            var projectile = Instance.InstantiateProjectile(player);
            projectile.Blast(player);
        };
        player.Died += (killerID) => Instance.OnPlayerDied(player.ID, killerID);
        player.name = $"Player {player.Nickname}";
        player.transform.SetParent(Instance._unitsRoot);
        Instance.Leaderboard.AddPlayer(player);
    }
}

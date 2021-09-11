using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text _nickname;
    [SerializeField]
    private Button _createRoomButton;
    [SerializeField]
    private Button _connectButton;

    private void Awake()
    {
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();     
    }

    private bool IsNullOrEmpty(string str)
    {
        if (str == null || str.Length == 0) return true;

        const char zeroWidthSymbol = (char)8203;

        int index = str.IndexOf(zeroWidthSymbol);
        while (index >= 0)      
        {
            str = str.Remove(index, 1);
            index = str.IndexOf(zeroWidthSymbol);
        }
        return str.Length == 0 ? true : false;
    }
    private void Enable(bool isEnabled)
    {
        _createRoomButton.interactable = isEnabled;
        _connectButton.interactable = isEnabled;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Player {_nickname.text} created room.");
    }
    public override void OnConnectedToMaster()
    {
        Enable(true);
        Debug.Log("Connected to Master.");
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);
        Debug.Log($"Player {PhotonNetwork.NickName} joined to room.");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Enable(true);
        Debug.LogWarning("Unable to join the room.");
        Debug.LogError(message);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Enable(true);
        Debug.LogWarning("Unable to join the room.");
        Debug.LogError(message);
    }

    public void CreateRoom_UnityEditor()
    {
        if (IsNullOrEmpty(_nickname.text) || _nickname.text.StartsWith(" "))
        {
            Debug.LogWarning("Nickname cannot be empty.");
            return;
        }
        PhotonNetwork.NickName = _nickname.text;
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        Enable(false);
    }
    public void Connect_UnityEditor()
    {
        if (IsNullOrEmpty(_nickname.text) || _nickname.text.StartsWith(" "))
        {
            Debug.LogWarning("Nickname cannot be empty.");
            return;
        }
        PhotonNetwork.NickName = _nickname.text;
        PhotonNetwork.JoinRandomRoom();
        Enable(false);
    }
}

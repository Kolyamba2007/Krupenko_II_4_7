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

    private void Start()
    {
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    private bool IsNullOrEmpty(string str)
    {
        if (str == null || str.Length == 0) return true;

        int index = str.IndexOf((char)8203);
        if (index == 0)
        {
            str = str.Remove(index, 1);
            return true;
        }
        return false;
    }
    private void Enable(bool isEnabled)
    {
        _createRoomButton.interactable = isEnabled;
        _connectButton.interactable = isEnabled;
    }

    public override void OnConnectedToMaster()
    {
        Enable(true);
        Debug.Log("Connected to Master.");
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = _nickname.text;
        PhotonNetwork.LoadLevel(1);
        Debug.Log($"Player {PhotonNetwork.NickName} joined to room.");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Enable(true);
        Debug.Log("Unable to join the room.");
        Debug.LogError(message);
    }

    public void CreateRoom_UnityEditor()
    {
        if (IsNullOrEmpty(_nickname.text) || _nickname.text.StartsWith(" "))
        {
            Debug.LogWarning("Nickname cannot be empty.");
            return;
        }
        PhotonNetwork.CreateRoom("", new RoomOptions { MaxPlayers = 2 });
        Enable(false);
    }
    public void Connect_UnityEditor()
    {
        if (IsNullOrEmpty(_nickname.text) || _nickname.text.StartsWith(" "))
        {
            Debug.LogWarning("Nickname cannot be empty.");
            return;
        }
        PhotonNetwork.JoinRandomRoom();
        Enable(false);
    }
}

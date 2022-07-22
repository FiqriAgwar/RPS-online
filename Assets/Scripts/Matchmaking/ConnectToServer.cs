using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public static ConnectToServer instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedRoom()
    {
        // CreateOrJoinRoom();
        Debug.Log("Joined a room");
    }

    public override void OnConnectedToMaster()
    {
        // CreateOrJoinRoom();
        Debug.Log("Ready to Connect");
    }

    public void CreateOrJoinRoom()
    {
        if (PhotonNetwork.CountOfRooms > 0)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(null, options);
        }
    }

    [PunRPC]
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}

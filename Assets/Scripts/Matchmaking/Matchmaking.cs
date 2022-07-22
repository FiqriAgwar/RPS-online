using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Matchmaking : MonoBehaviourPunCallbacks
{
    public GameObject PlayPanel;
    public GameObject LeavePanel;

    public Button LeaveButton;
    public Button gameStarting;

    public TMP_Text player1Name;
    public TMP_Text player2Name;

    public TMP_Text loadingText;

    private void Start()
    {
        loadingText.gameObject.SetActive(true);
        PlayPanel.gameObject.SetActive(false);
        LeavePanel.gameObject.SetActive(false);
        gameStarting.gameObject.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        SetPanel(PlayPanel);
        loadingText.gameObject.SetActive(false);
        gameStarting.gameObject.SetActive(false);
    }

    public void OnPlayButton()
    {
        ConnectToServer.instance.CreateOrJoinRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public override void OnJoinedRoom()
    {
        player2Name.text = "Finding Player...";

        SetPanel(LeavePanel);
        LeaveButton.gameObject.SetActive(true);
        gameStarting.gameObject.SetActive(false);

        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    void UpdateLobbyUI()
    {
        player1Name.text = PhotonNetwork.CurrentRoom.GetPlayer(1).NickName;
        player2Name.text = PhotonNetwork.PlayerList.Length == 2 ? PhotonNetwork.CurrentRoom.GetPlayer(2).NickName : "Finding Player...";

        if(PhotonNetwork.PlayerList.Length == 2)
        {
            SetPanel(LeavePanel);
            LeaveButton.gameObject.SetActive(false);
            gameStarting.gameObject.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                Invoke("TryStartGame", 1.0f);
            }
        }
    }

    void TryStartGame()
    {
        if(PhotonNetwork.PlayerList.Length == 2)
        {
            ConnectToServer.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Gameplay - PVP");
        }
        else
        {
            SetPanel(LeavePanel);
            LeaveButton.gameObject.SetActive(true);
            gameStarting.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void OnLeaveButton()
    {
        PhotonNetwork.LeaveRoom();
        SetPanel(PlayPanel);
        LeaveButton.gameObject.SetActive(false);
        gameStarting.gameObject.SetActive(false);
    }

    public void SetPanel(GameObject panel)
    {
        PlayPanel.SetActive(false);
        LeavePanel.SetActive(false);

        panel.SetActive(true);
    }

    public void OnChangeName(TMP_InputField nameInput)
    {
        PhotonNetwork.NickName = nameInput.text;
    }
}

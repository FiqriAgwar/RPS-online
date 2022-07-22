using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InputManager : MonoBehaviourPun
{
    Player photonPlayer;
    public int playerID;

    public ButtonChoice rockButton;
    public ButtonChoice paperButton;
    public ButtonChoice scissorButton;

    public void ChooseRPS(string choice)
    {
        GameManager.Instance.photonView.RPC("InputChoice", RpcTarget.AllBuffered, choice, playerID);
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        Debug.Log(player.ActorNumber);
        Debug.Log(player.IsLocal);

        this.photonPlayer = player;
        this.playerID = player.ActorNumber;

        if (player.IsLocal)
        {
            rockButton.SetButton(this);
            paperButton.SetButton(this);
            scissorButton.SetButton(this);
        }
    }

    [PunRPC]
    public void SetActiveButton(bool active)
    {
        if(rockButton.GetComponent<ButtonChoice>().myInput.playerID == this.playerID)
        {
            rockButton.GetComponent<Button>().interactable = active;
        }

        if (paperButton.GetComponent<ButtonChoice>().myInput.playerID == this.playerID)
        {
            paperButton.GetComponent<Button>().interactable = active;
        }

        if (scissorButton.GetComponent<ButtonChoice>().myInput.playerID == this.playerID)
        {
            scissorButton.GetComponent<Button>().interactable = active;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance { get; private set; }

    public TMP_Text ResultUI;
    public Image EnemyChoice;
    public Image PlayerChoice;

    public Sprite RockSprite;
    public Sprite PaperSprite;
    public Sprite ScissorSprite;
    public Sprite ChoosingSprite;

    private string[] Choices = new string[] { "Rock", "Paper", "Scissor", "Choosing" };

    public TMP_Text PlayerPointText;
    public TMP_Text EnemyPointText;
    public TMP_Text RoundText;

    public TMP_Text Player1Name;
    public TMP_Text Player2Name;

    private float Player1Point;
    private float Player2Point;
    private int Rounds;

    private string Player1Choice;
    private string Player2Choice;

    public InputManager Player1Input;
    public InputManager Player2Input;

    public bool vsBot = false;

    public GameObject GameOverPanel;
    public TMP_Text GameOverText;

    public Animator Player1Animator;
    public Animator Player2Animator;

    public MatchDatabase database;
    private List<RoundHistory> histories;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        histories = new List<RoundHistory>();
    }

    private void Start()
    {
        GameOverPanel.SetActive(false);

        photonView.RPC("SetPlayers", RpcTarget.AllBuffered);

        Player1Point = 0;
        Player2Point = 0;
        Rounds = 1;

        Player1Name.text = PhotonNetwork.PlayerList[0].NickName;
        Player2Name.text = PhotonNetwork.PlayerList[1] != null && !vsBot ? PhotonNetwork.PlayerList[1].NickName : "BOT";

        PlayerPointText.text = Player1Point.ToString();
        EnemyPointText.text = Player2Point.ToString();

        RoundText.text = "Rounds: " + Rounds.ToString();

        photonView.RPC("RoundStart", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SetPlayers()
    {
        Player1Input.photonView.TransferOwnership(1);
        Player2Input.photonView.TransferOwnership(2);

        Player1Input.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(1));
        Player2Input.photonView.RPC("Initialize", RpcTarget.AllBuffered, PhotonNetwork.CurrentRoom.GetPlayer(2));
    }

    [PunRPC]
    public void RoundStart()
    {
        Player1Choice = "Choosing";
        Player2Choice = "Choosing";

        EnemyChoice.sprite = ChoosingSprite;
        PlayerChoice.sprite = ChoosingSprite;

        Player1Animator.enabled = true;
        Player2Animator.enabled = true;

        Player1Animator.Play("Shuffle");
        Player2Animator.Play("Shuffle");

        ResultUI.gameObject.SetActive(false);

        Player1Input.photonView.RPC("SetActiveButton", RpcTarget.AllBuffered, true);
        Player2Input.photonView.RPC("SetActiveButton", RpcTarget.AllBuffered, true);
    }

    [PunRPC]
    public void InputChoice(string choice, int playerID)
    {
        if (playerID == Player1Input.playerID)
        {
            Player1Choice = choice;
            Player1Animator.enabled = false;
            PlayerChoice.sprite = ChoosingSprite;
            Player1Input.photonView.RPC("SetActiveButton", RpcTarget.AllBuffered, false);
        }

        if (playerID == Player2Input.playerID)
        {
            Player2Choice = choice;
            Player2Animator.enabled = false;
            EnemyChoice.sprite = ChoosingSprite;
            Player2Input.photonView.RPC("SetActiveButton", RpcTarget.AllBuffered, false);
        }

        BotRandom();

        if(Player1Choice != "Choosing" && Player2Choice != "Choosing")
        {
            Play();
        }
    }

    public void BotRandom()
    {
        Player2Choice = vsBot ? Choices[Random.Range(0, Choices.Length - 1)] : Player2Choice;
    }


    private void Play()
    {
        int result = GetResult(Player1Choice, Player2Choice);
        switch (result)
        {
            case -1:
                ResultUI.text = Player2Name.text + " wins round " + Rounds.ToString();
                Player2Point += 1;
                break;

            case 0:
                ResultUI.text = "Draw";
                Player1Point += 0.5f;
                Player2Point += 0.5f;
                break;

            case 1:
                ResultUI.text = Player1Name.text + " wins round " + Rounds.ToString();
                Player1Point += 1;
                break;

            default:
                ResultUI.text = "Error occured";
                break;
        }

        RoundHistory newHistory = new RoundHistory(Rounds, Player1Choice, Player2Choice);
        histories.Add(newHistory);

        ResultUI.gameObject.SetActive(true);

        Rounds += 1;

        photonView.RPC("UpdateInfo", RpcTarget.AllBuffered, Player1Choice, Player2Choice);
    }

    [PunRPC]
    public async void UpdateInfo(string playerChoice, string enemyChoice)
    {
        PlayerPointText.text = Player1Point.ToString();
        EnemyPointText.text = Player2Point.ToString();

        PlayerChoice.sprite = SwitchSprite(playerChoice);
        EnemyChoice.sprite = SwitchSprite(enemyChoice);

        if(Player1Point >= 3 || Player2Point >= 3)
        {
            RoundText.text = "Finished";
            await SaveToDatabase();
            StartCoroutine(DelayBeforeFinish());
        }
        else
        {
            RoundText.text = "Rounds: " + Rounds.ToString();
            StartCoroutine(RpcRoundStart());
        }
    }

    public async Task SaveToDatabase()
    {
        await database.WriteScoreToDatabase(Player1Name.text, Player2Name.text, Player1Point, Player2Point, histories);
    }

    IEnumerator RpcRoundStart()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("RoundStart", RpcTarget.AllBuffered);
    }

    IEnumerator DelayBeforeFinish()
    {
        yield return new WaitForSeconds(3f);
        photonView.RPC("GameFinished", RpcTarget.AllBuffered);
    }

    private Sprite SwitchSprite(string choice)
    {
        switch (choice)
        {
            case "Rock":
                return RockSprite;

            case "Paper":
                return PaperSprite;

            case "Scissor":
                return ScissorSprite;

            default:
                return ChoosingSprite;
        }
    }

    public int GetResult(string playerChoice, string enemyChoice)
    {
        int result = -2;

        switch (playerChoice)
        {
            case "Rock":
                switch (enemyChoice)
                {
                    case "Rock":
                        result = 0; break;

                    case "Paper":
                        result = -1; break;

                    case "Scissor":
                        result = 1; break;
                }
                break;

            case "Paper":
                switch (enemyChoice)
                {
                    case "Rock":
                        result = 1; break;

                    case "Paper":
                        result = 0; break;

                    case "Scissor":
                        result = -1; break;
                }
                break;

            case "Scissor":
                switch (enemyChoice)
                {
                    case "Rock":
                        result = -1; break;

                    case "Paper":
                        result = 1; break;

                    case "Scissor":
                        result = 0; break;
                }
                break;
        }

        return result;
    }

    [PunRPC]
    public void GameFinished()
    {        
        GameOverText.text = Player1Point == Player2Point ? "Game Draw" : Player1Point > Player2Point ? Player1Name.text + " wins the game" : Player2Name.text + " wins the game";
        GameOverPanel.SetActive(true);
    }

    public void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        ConnectToServer.instance.ChangeScene("Menu");
    }
}

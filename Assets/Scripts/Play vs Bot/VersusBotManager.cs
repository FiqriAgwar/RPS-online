using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VersusBotManager : MonoBehaviour
{
    public static VersusBotManager Instance { get; private set; }

    public TMP_Text ResultUI;
    public Image EnemyChoice;
    public Image PlayerChoice;

    public Sprite RockSprite;
    public Sprite PaperSprite;
    public Sprite ScissorSprite;
    public Sprite ChoosingSprite;

    public Button RockButton;
    public Button PaperButton;
    public Button ScissorButton;

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

    public Animator Player1Animator;
    public Animator Player2Animator;

    public GameObject GameOverPanel;
    public TMP_Text GameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GameOverPanel.SetActive(false);
        Player1Point = 0;
        Player2Point = 0;
        Rounds = 1;

        PlayerPointText.text = Player1Point.ToString();
        EnemyPointText.text = Player2Point.ToString();

        Player1Name.text = "Player 1";
        Player2Name.text = "BOT";

        RoundText.text = "Rounds: " + Rounds.ToString();

        RoundStart();
    }

    public void RoundStart()
    {
        Player1Choice = "Choosing";
        Player2Choice = "Choosing";

        Player1Animator.enabled = true;
        Player2Animator.enabled = true;

        Player1Animator.Play("Shuffle");
        Player2Animator.Play("Shuffle");

        ResultUI.gameObject.SetActive(false);

        SetButtonActive(true);
    }

    public string BotRandom()
    {
        return Choices[Random.Range(0, Choices.Length - 1)];
    }

    public void PlayerChoose(string choice)
    {
        Player1Choice = choice;

        Player1Animator.enabled = false;
        Player2Animator.enabled = false;

        EnemyChoice.sprite = ChoosingSprite;
        PlayerChoice.sprite = ChoosingSprite;

        SetButtonActive(false);
        Play();
    }

    private void Play()
    {
        Player2Choice = BotRandom();

        int result = GetResult(Player1Choice, Player2Choice);
        switch (result)
        {
            case -1:
                ResultUI.text = Player2Name.text + " win round " + Rounds.ToString();
                Player2Point += 1;
                break;

            case 0:
                ResultUI.text = "Draw";
                Player1Point += 0.5f;
                Player2Point += 0.5f;
                break;

            case 1:
                ResultUI.text = Player1Name.text + " win round " + Rounds.ToString();
                Player1Point += 1;
                break;

            default:
                ResultUI.text = "Error occured";
                break;
        }

        ResultUI.gameObject.SetActive(true);

        Rounds += 1;

        UpdateInfo(Player1Choice, Player2Choice);
    }

    public void UpdateInfo(string playerChoice, string enemyChoice)
    {
        PlayerPointText.text = Player1Point.ToString();
        EnemyPointText.text = Player2Point.ToString();

        PlayerChoice.sprite = SwitchSprite(playerChoice);
        EnemyChoice.sprite = SwitchSprite(enemyChoice);

        if (Player1Point >= 3 || Player2Point >= 3)
        {
            RoundText.text = "Finished";
            StartCoroutine(DelayBeforeFinish());
        }
        else
        {
            RoundText.text = "Rounds: " + Rounds.ToString();
            StartCoroutine(NewRoundStart());
        }
    }

    IEnumerator NewRoundStart()
    {
        yield return new WaitForSeconds(3f);
        RoundStart();
    }

    IEnumerator DelayBeforeFinish()
    {
        yield return new WaitForSeconds(3f);
        GameFinished();
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

    private void SetButtonActive(bool active)
    {
        RockButton.interactable = active;
        PaperButton.interactable = active;
        ScissorButton.interactable = active;
    }

    public void GameFinished()
    {
        GameOverText.text = Player1Point == Player2Point ? "Game Draw" : Player1Point > Player2Point ? Player1Name.text + " wins the game" : Player2Name.text + " wins the game";
        GameOverPanel.SetActive(true);
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

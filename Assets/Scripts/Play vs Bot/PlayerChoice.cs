using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoice : MonoBehaviour
{
    public void ChooseRPS(string choice)
    {
        VersusBotManager.Instance.PlayerChoose(choice);
    }
}

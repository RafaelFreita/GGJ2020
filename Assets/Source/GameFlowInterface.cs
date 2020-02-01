using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowInterface : MonoBehaviour
{
    public void StartGame()
    {
        GameFlowController.Instance.StartGame();
    }
}

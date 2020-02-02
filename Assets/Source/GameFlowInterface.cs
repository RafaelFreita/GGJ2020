using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowInterface : MonoBehaviour
{

    [SerializeField] private GameObject creditsPanel;
    private bool creditsOpen = false;

    public void StartGame()
    {
        GameFlowController.Instance.StartGame();
    }

    public void ToggleCredits()
    {
        creditsOpen = !creditsOpen;
        creditsPanel.SetActive(creditsOpen);
    }
}

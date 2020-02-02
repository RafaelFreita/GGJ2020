using System.Collections.Generic;
using UnityEngine;
using LUT;
using UnityEngine.SceneManagement;
using LUT.Snippets;

public class GameFlowController : Singleton<GameFlowController>
{
	public SceneReference mainMenu;
	public List<SceneReference> minigames;

	public int currentScore = 0;
	public int previousScore = 0;

	public int maxLife = 3;
	public int currentLife = 3;
	public bool hasLostLife = false;

	[SerializeField]
	private int lastMinigame = -1;

    public int gameIterations = 0;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}

	public void StartGame()
	{
		currentLife = maxLife;
		currentScore = 0;
		previousScore = 0;
		lastMinigame = -1;
        gameIterations = 0;

		if (!MicController.Instance.AlreadyCalibrated)
		{
			MicController.Instance.StartCalibration(LoadNextMinigame);
		}
		else
		{
			LoadNextMinigame();
		}
	}

	public void OnEndGame(bool value, int score)
	{
		Debug.Log($"OnEndGame {value}, {score}");
		hasLostLife = !value;
		if (!value)
		{
			currentLife -= 1;
		}
		GiveScore(score);
        gameIterations++;
	}

	private int GetNextMinigame()
	{
		if (minigames.Count == 1)
		{
			return 0;
		}
		else if (minigames.Count == 0)
		{
			Debug.LogError("Game flow doesn't have any minigame configured");
			return 0;
		}
		else
		{
			int rand = 0;
			do
			{
				rand = Random.Range(0, minigames.Count);
			} while (rand == lastMinigame);
			return rand;
		}
	}


	public void LoadNextMinigame()
	{
		if (currentLife <= 0) return;

		lastMinigame = GetNextMinigame();
		SceneManager.LoadScene(minigames[lastMinigame].SceneToLoadName);
	}

	public void ContinueGame()
	{
		if (IsAlive())
		{
			LoadNextMinigame();
		}
		else
		{
			SceneManager.LoadScene(mainMenu.SceneToLoadName);
		}
	}

	public void GiveScore(int newScore)
	{
		previousScore = currentScore;
		currentScore += newScore;
	}


	public bool IsAlive()
	{
		return currentLife > 0;
	}

}

using System.Collections.Generic;
using UnityEngine;
using LUT;
using UnityEngine.SceneManagement;
using LUT.Events.Primitives;
using LUT.Snippets;

public class GameFlowController : Singleton<GameFlowController>
{
	public EventBool onEndGame;


	public SceneReference mainMenu;
	public List<SceneReference> minigames;

	public int currentScore = 0;
	public int lastGivenScore = 0;

	public int maxLife = 3;
	public int currentLife = 3;
	public bool hasLostLife = false;


	private int lastMinigame = -1;

	public override void Start()
	{
		base.Start();
		onEndGame.Register(OnEndGame);
	}

	public void OnDestroy()
	{
		//❤❤❤❤❤❤❤😍😍😍😍😍🤳🤳🤳🤳
		onEndGame.Unregister(OnEndGame);

	}

	public void StartGame()
	{
		currentLife = maxLife;
		currentScore = 0;
		lastGivenScore = 0;
		lastMinigame = -1;

		LoadNextMinigame();
	}

	public void OnEndGame(bool value)
	{
		hasLostLife = !value;
		if (!value)
		{
			currentLife -= 1;
		}
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
		if(IsAlive())
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
		currentScore += newScore;
		lastGivenScore = newScore;
	}


	public bool IsAlive()
	{
		return currentLife > 0;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HammerMinigame : GameEndController
{

	// Window of time that the user can hit the object
	public float hitTimespan = 0.5f;
	public float spawnRate = 2.0f;
	public int objectsToSpawn = 6;
	public int maxErrors = 3;

	[Header("References")]
	public GameObject objectPrefab;
	public GameObject linePrefab;
	public Transform objectSpawnLocation;
	public ParticleSystem hitzonePS;

	[Header("Hammer Sprites")]
	public Image hammerImage;
	public Sprite hammerDownSprite;
	public Sprite hammerUpSprite;

	public Sprite toySprite;


	[Header("Ui")]
	public Transform errorsContainer = null;
	public GameObject errorsPrefab = null;

	private bool isBlowing = false;
	private bool objectCanBeHit = false;
	// Timer for next spawned object
	private float nextSpawnTimer = 0.0f;
	private int objectsSpawned = 0;
	[SerializeField]
	private int objectsHit = 0;
	[SerializeField]
	private int objectsMissed = 0;
	private float timeToNextSpawn = 2.0f;

	private List<GameObject> gameObjectsInHitzone = new List<GameObject>();

	public Transform linesParent;
	private float nextSpawnTimerx3;
	private float spawnRatex3;

	new private void Start()
	{
		base.Start();

        objectsToSpawn += GameFlowController.Instance.gameIterations * 2;

        nextSpawnTimerx3 = nextSpawnTimer / 5f;
		spawnRatex3 = spawnRate / 5f;

		for (int i = 0; i < maxErrors; i++)
		{
			Instantiate(errorsPrefab, errorsContainer);
		}

		WarmupRoller();
		GetNextSpawnTime();
	}

	private void GetNextSpawnTime()
	{
		timeToNextSpawn = spawnRate + Random.Range(-spawnRate * 0.5f, spawnRate * 1.5f);
	}

	private void WarmupRoller()
	{
		int iterationsPerTimer = Mathf.CeilToInt(spawnRatex3 / Time.fixedDeltaTime);
		for (int i = -1; i < 20; i++)
		{
			Vector3 offset = Vector3.down * linePrefab.GetComponent<HammerObjectMovement>().speed * Time.fixedDeltaTime * i * iterationsPerTimer;
			//Debug.Log(offset);
			Instantiate(linePrefab, objectSpawnLocation.position + offset, Quaternion.identity, linesParent);
		}
	}

	private void Update()
	{
		if (!isGamePaused || IsCoutdownAlive())
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				HitHammer();
			}


			if (Input.GetKeyUp(KeyCode.Space))
			{
				LiftHammer();
			}

			nextSpawnTimer += Time.deltaTime;
			nextSpawnTimerx3 += Time.deltaTime;

			if (nextSpawnTimerx3 >= spawnRatex3)
			{
				nextSpawnTimerx3 = 0.0f;
				Instantiate(linePrefab, objectSpawnLocation.position, Quaternion.identity, linesParent);
			}

			if (nextSpawnTimer >= timeToNextSpawn)
			{
				if (objectsSpawned < objectsToSpawn)
				{
					SpawnObject();
				}
				else // If all objects have already been spawned
				{
					CheckGameEnded();
				}
			}

		}
	}

	private bool IsCoutdownAlive()
	{
		return FindObjectOfType<Countdown>() != null;
	}

	private void CheckGameEnded()
	{
		if (objectsMissed >= maxErrors)
		{
			OnLose();
		}
		else if ((objectsHit + objectsMissed) == objectsToSpawn)
		{
			OnWin();
		}
	}

	private void SpawnObject()
	{
		if (IsCoutdownAlive())
		{
			return;
		}

		Instantiate(objectPrefab, objectSpawnLocation.position, Quaternion.identity, objectSpawnLocation);

		GetNextSpawnTime();
		nextSpawnTimer = 0.0f;
		objectsSpawned++;

	}

	private void HitHammer()
	{
		if (isGamePaused) return;
		// Set hammer to hit position
		hammerImage.sprite = hammerDownSprite;
		hitzonePS.Play();

		while (gameObjectsInHitzone.Count > 0)
		{
			//Destroy(gameObjectsInHitzone[0]);

			gameObjectsInHitzone[0].GetComponent<Image>().sprite = toySprite;

			gameObjectsInHitzone.RemoveAt(0);
			objectsHit++;
		}
	}

	private void LiftHammer()
	{
		if (isGamePaused) return;
		// Set hammer back lifted
		hammerImage.sprite = hammerUpSprite;
	}

	public override void OnBlowStatusChange(bool state)
	{
		// Start blowing is a hit
		if (!isBlowing && state) { HitHammer(); }

		// Stop blowing turns hammer back up
		if (isBlowing && !state) { LiftHammer(); }

		isBlowing = state;
	}

	public override void OnLose()
	{
		base.OnLose();
	}

	public override void OnWin()
	{
		base.OnWin();
	}

	public void OnObjectEnterHitzone(GameObject go)
	{
		//Debug.Log("ENTERED");
		gameObjectsInHitzone.Add(go);
	}

	public void OnObjectLeaveHitzone(GameObject go)
	{
		if (isGamePaused || IsCoutdownAlive()) return;


		// Only if GO still exist (it can leave the trigger when it's destroyed)
		if (gameObjectsInHitzone.Find(el => el == go))
		{
			errorsContainer.GetChild(objectsMissed).GetChild(0).gameObject.SetActive(true);
			//Debug.Log("LEFT");
			objectsMissed++;
			gameObjectsInHitzone.Remove(go);
			CheckGameEnded();
		}

		// Effect of losing object?
	}

}

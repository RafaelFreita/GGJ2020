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
    public int minObjectsToHit = 3;

    [Header("References")]
    public GameObject objectPrefab;
    public GameObject linePrefab;
    public Transform objectSpawnLocation;
    public ParticleSystem hitzonePS;

    [Header("Hammer Sprites")]
    public Image hammerImage;
    public Sprite hammerDownSprite;
    public Sprite hammerUpSprite;

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

        nextSpawnTimerx3 = nextSpawnTimer / 5f;
        spawnRatex3 = spawnRate / 5f;

        WarmupRoller();
        GetNextSpawnTime();
    }

    private void GetNextSpawnTime()
    {
        timeToNextSpawn = spawnRate + Random.Range(-spawnRate*0.5f, spawnRate*1.5f);
    }

    private void WarmupRoller()
    {
        int iterationsPerTimer = Mathf.CeilToInt(spawnRatex3 / Time.fixedDeltaTime);
        for (int i = 0; i < 12; i++)
        {
            Vector3 offset = Vector3.down * linePrefab.GetComponent<HammerObjectMovement>().speed * Time.fixedDeltaTime * i * iterationsPerTimer;
            //Debug.Log(offset);
            Instantiate(linePrefab, objectSpawnLocation.position + offset, Quaternion.identity, linesParent);
        }
    }

    private void Update()
    {
        if (!gameFinished)
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

    private void CheckGameEnded()
    {
        if ((objectsHit + objectsMissed) == objectsToSpawn)
        {
            if (objectsHit >= minObjectsToHit)
            {
                OnWin();
            }
            else
            {
                OnLose();
            }
        }
    }

    private void SpawnObject()
    {
        Instantiate(objectPrefab, objectSpawnLocation.position, Quaternion.identity, objectSpawnLocation);
        GetNextSpawnTime();
        nextSpawnTimer = 0.0f;
        objectsSpawned++;
    }

    private void HitHammer()
    {
        // Set hammer to hit position
        hammerImage.sprite = hammerDownSprite;
        hitzonePS.Play();

        while (gameObjectsInHitzone.Count > 0)
        {
            //Destroy(gameObjectsInHitzone[0]);

            // TODO: CHANGE SPRITE INSTEAD OF COLOR OR SOMETHING
            gameObjectsInHitzone[0].GetComponent<Image>().color = Color.green;

            gameObjectsInHitzone.RemoveAt(0);
            objectsHit++;
        }
    }

    private void LiftHammer()
    {
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

        // Only if GO still exist (it can leave the trigger when it's destroyed)
        if (gameObjectsInHitzone.Find(el => el == go))
        {
            //Debug.Log("LEFT");
            objectsMissed++;
            gameObjectsInHitzone.Remove(go);
        }
        Destroy(go);

        // Effect of losing object?
    }

}

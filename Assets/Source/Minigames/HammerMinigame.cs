using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HammerMinigame : GameEndController
{

    // Window of time that the user can hit the object
    public float hitTimespan = 0.5f;
    public float spawnRate = 2.0f;
    public int objectsToSpawn = 6;
    public int minObjectsToHit = 3;

    public GameObject objectPrefab;

    public Text headline;

    private bool isBlowing = false;
    private bool objectCanBeHit = false;
    // Timer for next spawned object
    private float nextSpawnTimer = 0.0f;
    private int objectsSpawned = 0;

    new private void Start()
    {
        base.Start();

        headline.text = "Hit the hammer on the right time to fix the toys!";
    }

    private void Update()
    {
        if (!gameFinished)
        {
            // Only increment timer for next object when last object is already done
            if (!objectCanBeHit)
            {
                nextSpawnTimer += Time.deltaTime;
            }

            if(nextSpawnTimer >= spawnRate)
            {
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        Instantiate(objectPrefab);

        nextSpawnTimer = 0.0f;
        objectsSpawned++;
    }

    public override void OnBlowStatusChange(bool state)
    {
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
    
}

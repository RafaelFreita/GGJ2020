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

    [Header("References")]
    public GameObject objectPrefab;
    public Transform objectSpawnLocation;
    public Canvas canvas;
    public Text headline;

    private bool isBlowing = false;
    private bool objectCanBeHit = false;
    // Timer for next spawned object
    private float nextSpawnTimer = 0.0f;
    private int objectsSpawned = 0;

    private int objectsHit = 0;
    private int objectsMissed = 0;

    private List<GameObject> gameObjectsInHitzone = new List<GameObject>();

    new private void Start()
    {
        base.Start();

        headline.text = "Hit the hammer on the right time to fix the toys!";
    }

    private void Update()
    {
        if (!gameFinished)
        { 
            nextSpawnTimer += Time.deltaTime;

            if (nextSpawnTimer >= spawnRate)
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
        Instantiate(objectPrefab, objectSpawnLocation.position, Quaternion.identity, canvas.transform);

        nextSpawnTimer = 0.0f;
        objectsSpawned++;
    }

    private void HitHammer()
    {
        // Set hammer to hit position

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
        Debug.Log("ENTERED");
        gameObjectsInHitzone.Add(go);
    }

    public void OnObjectLeaveHitzone(GameObject go)
    {

        // Only if GO still exist (it can leave the trigger when it's destroyed)
        if (gameObjectsInHitzone.Find(el => el == go))
        {
            Debug.Log("LEFT");
            objectsMissed++;
            gameObjectsInHitzone.Remove(go);
        }
        Destroy(go);

        // Effect of losing object?
    }

}

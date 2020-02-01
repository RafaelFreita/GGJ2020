using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerHitZone : MonoBehaviour
{

    public HammerMinigame hammerMinigame;

    private void OnTriggerEnter2D(Collider2D other)
    {
        hammerMinigame.OnObjectEnterHitzone(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        hammerMinigame.OnObjectLeaveHitzone(other.gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LUT.Events.Primitives;

public class LungFeedback : MonoBehaviour
{
    [SerializeField]
    public EventBool onGameEnd;

    public Sprite happyLung = null;
    public Sprite sadLung = null;

    public AnimationClip happyLungAnimClip;
    public AnimationClip sadLungAnimClip;

    private Image[] lungs;

    private void Awake()
    {
        onGameEnd.Register(OnGameEnd);
        lungs = GetComponentsInChildren<Image>();
    }

    private void OnDestroy()
    {
        onGameEnd.Unregister(OnGameEnd);
    }

    public void OnGameEnd(bool won)
    {
        var gameflow = GameFlowController.Instance;
        for (int i = 0; i < lungs.Length; i++)
        {
            Animation anim = lungs[i].GetComponent<Animation>();
            if (gameflow.currentLife > i)
            {
                lungs[i].sprite = happyLung;

                anim.Play(happyLungAnimClip.name);
            }
            else
            {
                lungs[i].sprite = sadLung;

                anim.Play(sadLungAnimClip.name);
            }
        }
    }
}

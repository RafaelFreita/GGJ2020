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
	private Image[] lungs;
	
	private GameFlowController cachedGameFlow;
	void Awake()
	{
		onGameEnd.Register(OnGameEnd);
		cachedGameFlow = GameFlowController.Instance;
		lungs = GetComponentsInChildren<Image>();
	}

	private void OnDestroy()
	{
		onGameEnd.Unregister(OnGameEnd);
	}

	public void OnGameEnd(bool won)
	{
		Debug.Log($"has {lungs.Length} lungs and {cachedGameFlow.currentLife} life");
		for(int i = 0; i < lungs.Length; i++)
		{
			if(cachedGameFlow.currentLife > i)
			{
				lungs[i].sprite = happyLung;
			}
			else
			{
				lungs[i].sprite = sadLung;
			}
		}
	}
}

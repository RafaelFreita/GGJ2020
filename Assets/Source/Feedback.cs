using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LUT.Events.Primitives;

public class Feedback : MonoBehaviour
{
	[SerializeField]
	public EventBool onGameEnd;

	public Text feedbackText;

	protected void Start()
	{
		onGameEnd.Register(OnGameEnd);
	}

	private void OnDestroy()
	{
		onGameEnd.Unregister(OnGameEnd);
	}

	public void OnGameEnd(bool state)
	{
		feedbackText.text = state ? "You win" : "You Lose";
	}
}

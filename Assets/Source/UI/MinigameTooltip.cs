using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MinigameTooltip : MonoBehaviour
{
	private GameEndController gameEndController;

	private TextMeshProUGUI tooltip;

	// Start is called before the first frame update
	private void Start()
	{
		gameEndController = FindObjectOfType<GameEndController>();
		tooltip = GetComponent<TextMeshProUGUI>();
		tooltip.text = gameEndController.GetRandomMinigameTooltip();
	}

}

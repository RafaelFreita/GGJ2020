using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class TimeSlider : MonoBehaviour
{
	private GameEndController gameEndController;

	private Slider slider;

	// Start is called before the first frame update
	private void Start()
	{
		gameEndController = FindObjectOfType<GameEndController>();
		slider = GetComponent<Slider>();
	}

	// Update is called once per frame
	private void Update()
	{
		slider.value = 1 - gameEndController.GetNormalizedRemainingTime();
	}
}

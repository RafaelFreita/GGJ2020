using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaloonMinigame : GameEndController
{

	public float minTimeFill = 2.0f;
	public float maxTimeFill = 2.5f;
	public int balloonsToFill = 3;
	public float maxMinigameTime = 10.0f;

	// Ballon Image Refs
	[SerializeField] Transform ballonRootBone;
	[SerializeField] Transform ballonGrowBone;

	public Text headerText;
	public Color balloonNotReadyColor;
	public Color balloonReadyColor;
	public Text textBalloonsFilled;
	public Slider slider;
	private Image sliderImage;

	private float totalMinigameTime = 0.0f;
	private float currentBlowingTime = 0f;
	private int balloonsFilled = 0;

	private bool isBlowing = false;

	new private void Start()
	{
		base.Start();

		headerText.text = $"Blow to fill ballons! SAVE THE PARTY!!!";

		sliderImage = slider.fillRect.GetComponent<Image>();
	}

	private void Update()
	{
		if (!gameFinished)
		{
			totalMinigameTime += Time.deltaTime;
			if (totalMinigameTime >= maxMinigameTime)
			{
				OnLose();
			}

			if (isBlowing)
			{
				currentBlowingTime += Time.deltaTime;
			}
			else
			{
				currentBlowingTime -= Time.deltaTime;
			}
			currentBlowingTime = Mathf.Max(currentBlowingTime, 0.0f);

			float blowSlider = currentBlowingTime / maxTimeFill;
			if (blowSlider > 1.0f)
			{
				OnLose();
			}
			ballonRootBone.rotation = Quaternion.AngleAxis(90 + Mathf.Sin(blowSlider * 135) * 2, Vector3.forward);
			ballonGrowBone.localScale = Vector3.one * (1.0f + blowSlider * 1.8f);
			if (slider)
			{
				slider.value = blowSlider;
			}

			if (sliderImage)
			{
				sliderImage.color = (IsBalloonReady())
					? balloonReadyColor
					: balloonNotReadyColor;
			}
		}
	}

	private bool IsBalloonReady()
	{
		// If blowing time is between min and max
		return currentBlowingTime >= minTimeFill && currentBlowingTime <= maxTimeFill;
	}

	private void CheckCurrentBalloon()
	{
		if (IsBalloonReady())
		{
			balloonsFilled++;
			textBalloonsFilled.text = balloonsFilled.ToString();
			currentBlowingTime = 0.0f;

			// Check if all balloons were filled
			if (balloonsFilled >= balloonsToFill)
			{
				OnWin();
			}
		}
	}

	public override void OnBlowStatusChange(bool state)
	{
		// If is blowing and stops, check if balloon is filled
		if (isBlowing && !state)
		{
			CheckCurrentBalloon();
		}

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


	public override float GetNormalizedRemainingTime()
	{
		return totalMinigameTime / maxMinigameTime;
	}

}

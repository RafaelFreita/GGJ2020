using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Countdown : MonoBehaviour
{
	[SerializeField]
	private Image panel = null;
	[SerializeField]
	private TMP_Text countDownText = null;
	private int count = 3;
	private float countFlt = 3;
	private string finalMsg = null;
	private float scale = 1;
	private Action callback = null;

	public static Countdown Instantiate(int count, string finalMsg = null, float scale = 1, Action callback = null)
	{
		GameObject prefab = Resources.Load<GameObject>("Countdown");
		GameObject instance = GameObject.Instantiate(prefab);
		Countdown countdown = instance.GetComponent<Countdown>();
		countdown.Setup(count, finalMsg, scale, callback);
		return countdown;
	}

	public static Countdown Instantiate(int count, Color bgColor, Color txtColor, string finalMsg = null, float scale = 1, Action callback = null)
	{
		GameObject prefab = Resources.Load<GameObject>("Countdown");
		GameObject instance = GameObject.Instantiate(prefab);
		Countdown countdown = instance.GetComponent<Countdown>();
		countdown.Setup(count, bgColor, txtColor, finalMsg, scale, callback);
		return countdown;
	}

	public void Setup(int count, string finalMsg = null, float scale = 1, Action callback = null)
	{
		this.count = count;
		this.countFlt = count + 0.99f;
		this.finalMsg = finalMsg;
		this.scale = scale;
		this.callback = callback;
		countDownText.text = count.ToString();
	}

	public void Setup(int count, Color bgColor, Color txtColor, string finalMsg = null, float scale = 1, Action callback = null)
	{
		panel.color = bgColor;
		countDownText.color = txtColor;
		this.count = count;
		this.countFlt = count + 0.99f;
		this.finalMsg = finalMsg;
		this.scale = scale;
		this.callback = callback;
		countDownText.text = count.ToString();
	}

	private void Update()
	{
		countFlt -= Time.deltaTime * scale;
		count = (int)countFlt;
		string text = count.ToString();
		if (count > 0)
		{
			countDownText.text = text;
			return;
		}

		if (finalMsg == null)
		{
			callback?.Invoke();
			Destroy(gameObject);
			return;
		}

		if (count > -1.0f)
		{
			countDownText.text = finalMsg;
			return;
		}

		callback?.Invoke();
		Destroy(gameObject);
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundFadeOutOnLevelLoad : MonoBehaviour
{

	public AnimationCurve curve;
	public float secondsToFadeOut = 0.25f;
	private AudioSource audioSource;
	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		DontDestroyOnLoad(gameObject);
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}

	void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnActiveSceneChanged;
	}

	private void OnActiveSceneChanged(Scene arg0, Scene arg1)
	{
		StartCoroutine(FadeOutAndDestroy());
	}


	private IEnumerator FadeOutAndDestroy()
	{
		float time = 0;
		while (time < secondsToFadeOut)
		{
			audioSource.volume = curve.Evaluate(time);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}
}

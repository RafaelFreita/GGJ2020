using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
	public AudioMixer audioMixer;
	public string playerVolumeName = "PlayerVolume";


	public void SetPlayerMicFeedback(bool mute)
	{
		audioMixer.SetFloat(playerVolumeName, mute ? -80 : 0);
	}
}

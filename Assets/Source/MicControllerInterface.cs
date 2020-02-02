using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicControllerInterface : MonoBehaviour
{
	public void StartCalibration()
	{
		MicController.Instance.StartCalibration();
	}
}

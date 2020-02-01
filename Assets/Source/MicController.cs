using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MicController : MonoBehaviour
{
	public Text resultDisplay;   // GUIText for displaying results
	public Text blowDisplay;     // GUIText for displaying blow or not blow.
	public int recordedLength = 50;    // How many previous frames of sound are analyzed.
	public int requiedBlowTime = 4;    // How long a blow must last to be classified as a blow (and not a sigh for instance).
	public int clamp = 160;            // Used to clamp dB (I don't really understand this either).

	public int SampleCount = 1024;
	public int Frequency = 48000;    // Wavelength, I think.
	public float RefValue = 0.1f;    // RMS  value for 0 dB.
	public float Threshold = 0.02f;  // Minimum amplitude to extract pitch (recieve anything)
	public float Alpha = 0.05f;      // The alpha for the low pass filter (I don't really understand this).
	public int RecordAudioLength = 10;
	public float LowPassfilterSomething = -30;

	private float[] samples;           // Samples
	private float[] spectrum;          // Spectrum
	private List<float> dbValues;      // Used to average recent volume.
	private List<float> pitchValues;   // Used to average recent pitch.

	private float rmsValue;            // Volume in RMS (root medium square)
	private float dbValue;             // Volume in DB
	private float pitchValue;          // Pitch - Hz (is this frequency?)
	private int blowingTime;           // How long each blow has lasted

	private float lowPassResults;      // Low Pass Filter result
	private float peakPowerForChannel; //

	new private AudioSource audio;

	public void Awake()
	{
		audio = GetComponent<AudioSource>();
	}

	public void Start()
	{
		samples = new float[SampleCount];
		spectrum = new float[SampleCount];
		dbValues = new List<float>();
		pitchValues = new List<float>();

		StartMicListener();
	}

	public void Update()
	{

		// If the audio has stopped playing, this will restart the mic play the clip.
		if (!audio.isPlaying)
		{
			StartMicListener();
		}

		resultDisplay.text = "";

		// Gets volume and pitch values
		AnalyzeSound();

		// Runs a series of algorithms to decide whether a blow is occuring.
		DeriveBlow();

		// Update the meter display.
		resultDisplay.text += "RMS: " + rmsValue.ToString("F2") + " (" + dbValue.ToString("F1") + " dB)\n" + "Pitch: " + pitchValue.ToString("F0") + " Hz";
	}

	/// Starts the Mic, and plays the audio back in (near) real-time.
	private void StartMicListener()
	{
		audio.clip = Microphone.Start(null, true, RecordAudioLength, Frequency);
		// HACK - Forces the function to wait until the microphone has started, before moving onto the play function.
		while (!(Microphone.GetPosition(null) > 0))
		{
		}
		audio.Play();
	}

	/// Credits to aldonaletto for the function, http://goo.gl/VGwKt
	/// Analyzes the sound, to get volume and pitch values.
	private void AnalyzeSound()
	{

		// Get all of our samples from the mic.
		audio.GetOutputData(samples, 0);

		// Sums squared samples
		float sum = 0;
		for (int i = 0; i < SampleCount; i++)
		{
			sum += Mathf.Pow(samples[i], 2);
		}

		//Debug.Log($"sum is {sum}");
		// RMS is the square root of the average value of the samples.
		rmsValue = Mathf.Sqrt(sum / SampleCount);
		dbValue = 20 * Mathf.Log10(rmsValue / RefValue);

		// Clamp it to {clamp} min
		if (dbValue < -clamp)
		{
			dbValue = -clamp;
		}

		// Gets the sound spectrum.
		audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float loudestFrequency = 0;
		int loudestSample = 0;

		for (int i = 1; i < spectrum.Length - 1; i++)
		{
			Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
			Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
			Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
		}

		// Find the highest sample.
		for (int i = 0; i < SampleCount; i++)
		{
			if (spectrum[i] > loudestFrequency && spectrum[i] > Threshold)
			{
				loudestFrequency = spectrum[i];
				loudestSample = i; 
			}
		}

		//Debug.Log($"Loudest frequency {loudestFrequency}, Loudest sample {loudestSample}");

		// Pass the index to a float variable
		float interpolatedSample = loudestSample;

		// Interpolate index using neighbours
		if (loudestSample > 0 && loudestSample < SampleCount - 1)
		{
			float deltaLeftFreq = spectrum[loudestSample - 1] / spectrum[loudestSample];
			float deltaRightFreq = spectrum[loudestSample + 1] / spectrum[loudestSample];
			interpolatedSample += 0.5f * (deltaRightFreq * deltaRightFreq - deltaLeftFreq * deltaLeftFreq);
		}

		// Convert index to frequency
		pitchValue = interpolatedSample * (Frequency * 0.5f) / SampleCount;
	}

	private void DeriveBlow()
	{

		UpdateRecords(dbValue, dbValues);
		//UpdateRecords(pitchValue, pitchValues);

		// Find the average pitch in our records (used to decipher against whistles, clicks, etc).
		//float sumPitch = 0;
		//foreach (float num in pitchValues)
		//{
		//	sumPitch += num;
		//}
		//float avgPitch = sumPitch / pitchValues.Count;

		// Run our low pass filter.
		lowPassResults = LowPassFilter(dbValue);


		resultDisplay.text += $" lowPassResult: {lowPassResults} \n";

		// Decides whether this instance of the result could be a blow or not.
		if (lowPassResults > LowPassfilterSomething/* && avgPitch == 0*/)
		{
			blowingTime += 1;
		}
		else
		{
			blowingTime = 0;
		}

		// Once enough successful blows have occured over the previous frames (requiredBlowTime), the blow is triggered.
		// This example says "blowing", or "not blowing", and also blows up a sphere.
		if (blowingTime > requiedBlowTime)
		{
			blowDisplay.text = "Blowing";
		}
		else
		{
			blowDisplay.text = "Not blowing";
		}
	}

	// Updates a record, by removing the oldest entry and adding the newest value (val).
	private void UpdateRecords(float val, List<float> record)
	{
		if (record.Count > recordedLength)
		{
			record.RemoveAt(0);
		}
		record.Add(val);
	}

	/// Gives a result (I don't really understand this yet) based on the peak volume of the record
	/// and the previous low pass results.
	private float LowPassFilter(float peakVolume)
	{
		return Alpha * peakVolume + (1.0f - Alpha) * lowPassResults;
	}
}



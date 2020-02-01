using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LUT.Events.Primitives;

[RequireComponent(typeof(AudioSource))]
public class MicController : MonoBehaviour
{
	[Header("Debug")]
	public Text resultDisplay;
	public Text blowDisplay;

	[Header("Setup")]
	public EventBool OnBlowStatusChanged;

	[Header("Mic Rec Info")]
	/// How many previous frames of sound are analyzed.
	public int SampleCount = 1024;
	public int Frequency = 48000;
	public int RecordAudioLength = 10;

	[Header("Detection")]
	public int RecordedLength = 50;
	/// Used to clamp dB. 
	public int Clamp = 160;
	// RMS  value for 0 dB.
	public float RefValue = 0.1f;
	// Minimum amplitude to extract pitch (recieve anything)
	public float Threshold = 0.02f;
	// The alpha for the low pass filter (I don't really understand this).
	public float Alpha = 0.05f;

	public bool calibratingAmbient = false;
	public bool CalibratingAmbient
	{
		get { return calibratingAmbient; }
		set { calibratingAmbient = value; }
	}
	public int CalAmbientLen = 0;
	public float CalAmbientStdDev = 3;
	public float CalAmbientLoudness = -30;
	[SerializeField]
	private Text CalAmbientText;

	public bool calibratingBlow = false;
	public bool CalibratingBlow
	{
		get { return calibratingBlow; }
		set { calibratingBlow = value; }
	}
	public int CalBlowLen = 0;
	public float CalBlowStdDev = 0;
	public float CalBlowLoudness = 0;
	[SerializeField]
	private Text CalBlowText = null;

	/// How long a blow must last to be classified as a blow (and not a sigh for instance). (in seconds)
	public float RequiredTimeToStartBlowing = 0.25f;
	/// How long a blow must not be indentify to consider that the blow has actually stopped (in seconds)
	public float RequiredTimeToStopBlowing = 0.25f;

	private float[] _samples;           // Samples
	private float[] _spectrum;          // Spectrum
	private List<float> _dbValues;      // Used to average recent volume.
	private List<float> _pitchValues;   // Used to average recent pitch.

	private float _rmsValue;            // Volume in RMS (root medium square)
	private float _dbValue;             // Volume in DB
	private float _pitchValue;          // Pitch - Hz (is this frequency?)

	private float _lowPassResults;      // Low Pass Filter result
	private float _peakPowerForChannel; //

	private bool _wasBlowing = false;
	private float _blowingTime;           // How long current blow is lasting lasting
	private float _notBlowingTime;      // How long the user is not blowing

	new private AudioSource audio;

	public void Awake()
	{
		audio = GetComponent<AudioSource>();
	}

	public void Start()
	{
		_samples = new float[SampleCount];
		_spectrum = new float[SampleCount];
		_dbValues = new List<float>();
		_pitchValues = new List<float>();

		StartMicListener();
	}

	public void Update()
	{

		// If the audio has stopped playing, this will restart the mic play the clip.
		if (!audio.isPlaying)
		{
			StartMicListener();
		}

		if (resultDisplay)
		{
			resultDisplay.text = "";
		}

		// Gets volume and pitch values
		AnalyzeSound();

		// Runs a series of algorithms to decide whether a blow is occuring.
		DeriveBlow();

		// Update the meter display.
		if (resultDisplay)
		{
			resultDisplay.text += "RMS: " + _rmsValue.ToString("F2") + " (" + _dbValue.ToString("F1") + " dB)\n" + "Pitch: " + _pitchValue.ToString("F0") + " Hz";
		}
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
		audio.GetOutputData(_samples, 0);

		// Sums squared samples
		float sum = 0;
		for (int i = 0; i < SampleCount; i++)
		{
			sum += Mathf.Pow(_samples[i], 2);
		}

		//Debug.Log($"sum is {sum}");
		// RMS is the square root of the average value of the samples.
		_rmsValue = Mathf.Sqrt(sum / SampleCount);
		_dbValue = 20 * Mathf.Log10(_rmsValue / RefValue);

		// Clamp it to {clamp} min
		if (_dbValue < -Clamp)
		{
			_dbValue = -Clamp;
		}

		// Gets the sound spectrum.
		audio.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
		float loudestFrequency = 0;
		int loudestSample = 0;

		// Find the highest sample.
		for (int i = 0; i < SampleCount; i++)
		{
			if (_spectrum[i] > loudestFrequency && _spectrum[i] > Threshold)
			{
				loudestFrequency = _spectrum[i];
				loudestSample = i;
			}
		}

		//Debug.Log($"Loudest frequency {loudestFrequency}, Loudest sample {loudestSample}");

		// Pass the index to a float variable
		float interpolatedSample = loudestSample;

		// Interpolate index using neighbours
		if (loudestSample > 0 && loudestSample < SampleCount - 1)
		{
			float deltaLeftFreq = _spectrum[loudestSample - 1] / _spectrum[loudestSample];
			float deltaRightFreq = _spectrum[loudestSample + 1] / _spectrum[loudestSample];
			interpolatedSample += 0.5f * (deltaRightFreq * deltaRightFreq - deltaLeftFreq * deltaLeftFreq);
		}

		// Convert index to frequency
		_pitchValue = interpolatedSample * (Frequency * 0.5f) / SampleCount;
	}

	private void DeriveBlow()
	{

		UpdateRecords(_dbValue, _dbValues);
		//UpdateRecords(pitchValue, pitchValues);

		// Find the average pitch in our records (used to decipher against whistles, clicks, etc).
		//float sumPitch = 0;
		//foreach (float num in pitchValues)
		//{
		//	sumPitch += num;
		//}
		//float avgPitch = sumPitch / pitchValues.Count;

		// Run our low pass filter.
		_lowPassResults = LowPassFilter(_dbValue);

		if (resultDisplay)
		{
			resultDisplay.text += $" lowPassResult: {_lowPassResults} \n";
		}

		// Decides whether this instance of the result could be a blow or not.
		float ambientLoudnessDiff = _lowPassResults - CalAmbientLoudness;
		float blowLoudnessDiff = _lowPassResults - CalBlowLoudness;
		if (blowLoudnessDiff > CalBlowStdDev * -2.0f && ambientLoudnessDiff > CalAmbientStdDev * 3.0f)
		{
			_notBlowingTime = 0;
			_blowingTime += Time.deltaTime;
		}
		else
		{
			_notBlowingTime += Time.deltaTime;
			_blowingTime = 0;
		}

		if (_wasBlowing && _notBlowingTime > RequiredTimeToStopBlowing)
		{
			UpdateBlowTo(false);

		}
		else if (!_wasBlowing && _blowingTime > RequiredTimeToStartBlowing)
		{
			UpdateBlowTo(true);
		}
	}

	private void UpdateBlowTo(bool value)
	{
		if (blowDisplay)
		{
			blowDisplay.text = value ? "Blowing" : "Not blowing";
		}
		_wasBlowing = value;
		OnBlowStatusChanged.Invoke(value);
	}

	// Updates a record, by removing the oldest entry and adding the newest value (val).
	private void UpdateRecords(float val, List<float> record)
	{
		if (record.Count > RecordedLength)
		{
			record.RemoveAt(0);
		}
		record.Add(val);
		if (calibratingAmbient)
		{
			if (CalAmbientLen < RecordedLength)
			{
				CalAmbientLen++;
			}
			else
			{
				float mean = 0;
				foreach (float db in record)
				{
					mean += db;
				}
				mean /= record.Count;

				float stddev = 0;
				foreach (float db in record)
				{
					stddev += Mathf.Pow(db - mean, 2.0f);
				}
				stddev /= record.Count;
				stddev = Mathf.Sqrt(stddev);
				CalAmbientStdDev = stddev;
				CalAmbientLoudness = mean;
				if (CalAmbientText != null)
				{
					CalAmbientText.text = $"AmbientStdDev: {CalAmbientStdDev.ToString("F2")}\n" +
										$"AmbientLoudness: {CalAmbientLoudness.ToString("F2")}";
				}
				calibratingAmbient = false;
				CalAmbientLen = 0;
			}
		}

		if (calibratingBlow)
		{
			if (CalBlowLen < RecordedLength)
			{
				CalBlowLen++;
			}
			else
			{
				float mean = 0;
				foreach (float db in record)
				{
					mean += db;
				}
				mean /= record.Count;

				float stddev = 0;
				foreach (float db in record)
				{
					stddev += Mathf.Pow(db - mean, 2.0f);
				}
				stddev /= record.Count;
				stddev = Mathf.Sqrt(stddev);
				CalBlowStdDev = stddev;
				CalBlowLoudness = mean;
				if (CalBlowText != null)
				{
					CalBlowText.text = $"BlowStdDev: {CalBlowStdDev.ToString("F2")}\n" +
					                   $"BlowLoudness: {CalBlowLoudness.ToString("F2")}";
				}
				calibratingBlow = false;
				CalBlowLen = 0;
			}
		}
	}

	// !! this is not a low pass filter, actually it's more like a damping
	/// Gives a result (I don't really understand this yet) based on the peak volume of the record
	/// and the previous low pass results.
	private float LowPassFilter(float peakVolume)
	{
		return Alpha * peakVolume + (1.0f - Alpha) * _lowPassResults;
	}
}



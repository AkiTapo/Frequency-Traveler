using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Wave : MonoBehaviour {

    public GameObject rectangle;
    public float recordInterval;
    public int waveAmount;
    public float waveWidth, waveHeight;
    public int waveIntensity;
    GameObject[] waves;
    float increaseLowFreq;

    
    //Audio
    public float RmsValue;
    public float DbValue;
    public float PitchValue;

    private const int QSamples = 2400;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private AudioSource audioSource;
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;
    bool playingAudio, startedRecording;
    float timer;
    float recordTimer;
    FFTWindow fFTWindow;
    //Audio
    

    private float[] samples = new float[1024];
    // Use this for initialization
    void Start () {

        /*
        //Audio
        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;

        audioSource = this.GetComponent<AudioSource>();
        //Audio
        */

        waves = new GameObject [waveAmount];

        for (int x = 0; x < waveAmount; x++)
        {
            waves[x] = Instantiate(rectangle, new Vector3(x * rectangle.GetComponent<Transform>().localScale.x - Mathf.Abs(transform.position.x), transform.position.y, 0), Quaternion.identity) as GameObject;
            waves[x].transform.localScale = new Vector3(waves[x].transform.localScale.x * waveWidth, 1, 1);
            waves[x].transform.position = new Vector3(waves[x].transform.position.x * waveWidth, waves[x].transform.position.y, waves[x].transform.position.z);
        }

        //linerenderer = GetComponent<LineRenderer>();
        //linerenderer.SetVertexCount(samples.Length);

        /*
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 100, 44100);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        Debug.Log("start playing... position is " + Microphone.GetPosition(null));
        audio.Play();

        */
        //Check if there is at least one microphone connected  
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Microphone not connected!");
        }
        else
        {
            micConnected = true;
            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }

            audioSource = this.GetComponent<AudioSource>();

        }
        //Start recording when game starts


    }
	
	// Update is called once per frame
	void Update () {
        timer = Time.time;

        audioSource.GetSpectrumData(samples, 0, fFTWindow);

        if (!playingAudio) {

            if (!startedRecording)
            {
                recordTimer = timer;
                startedRecording = true;
                audioSource.clip = Microphone.Start(null, true, (int)1000, maxFreq);
                audioSource.Play();
                print("Started Recording");
            }
        }



        for (int i = 0; i<waveAmount; i++) {
            increaseLowFreq = Mathf.Exp(3) * i;
            //Get the highest from array
            //Push it and its surounding waves
            waves[i].transform.localScale = new Vector3(waves[i].transform.localScale.x, waveHeight + samples[i + 20] * increaseLowFreq * waveIntensity, waves[i].transform.localScale.z);
            waves[i].GetComponent<Renderer>().material.color = new Vector4(1 / (256 - 88), 1 / (256 - 121), 1 / waves[i].transform.localScale.y);
        }
    }
    /*
    void AnalyzeSound()
    {
        audioSource.GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        print(DbValue);
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
        
        // get sound spectrum
        audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max


        }
        float freqN = maxN; // pass the index to a float variable

        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency
        //PitchValue = freqN * AudioSettings.outputSampleRate / QSamples; // convert index to frequency

        //print(freqN);

    
    }
    */
}

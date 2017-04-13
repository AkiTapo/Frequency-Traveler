using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Wave : MonoBehaviour
{


    // public float recordInterval;
    [Range(1, 3)]
    public int inputSwitch = 1;
    [Range(10, 120)]
    public int waveAmount;
    [Range(1, 15)]
    public float waveWidth;

    public GameObject waveShape;
    public static int maxWaterLevelLocal = 14;
    public static float minWaterLevelLocal;
    public float minWaterLevel;
    public int maxWaterLevel;
    public float waveResetSpeed;

    GameObject[] waves;
    float maxWave;
    int maxWaveIndex;

    [SerializeField]
    [Range(4, 100)]
    public int waveBlur = 15;
    [SerializeField]
    [Range(0, 30)]
    public float waveSpeed = 5f;
    [Range(0.1f, 10f)]
    // public float wavesmoother = 5;
    // [Range(0f, 10f)]
    public int waveIntensity = 5;
    float waveY;
    private bool recording;
    [Range(0, 1)]
    public int micSwitch;

    //Audio
    private AudioSource audioSource;
    private bool micConnected = false;
    private int minFreq = 2000;
    private int maxFreq;
    bool playingAudio, startedRecording;
    float timer;
    float recordTimer;
    FFTWindow fFTWindow;
    float increaseLowFreq;
    //Audio
    public static bool waveRestart;


    private float[] samples = new float[1024];
    private float smoother;
    private int devider;

    // Use this for initialization
    void Start()
    {
        waves = new GameObject[waveAmount];

        for (int x = 0; x < waveAmount; x++)
        {
            waves[x] = Instantiate(waveShape, new Vector3(x * waveShape.GetComponent<Transform>().localScale.x - Mathf.Abs(transform.position.x), transform.position.y, 0), Quaternion.identity) as GameObject;
            waves[x].transform.localScale = new Vector3(waves[x].transform.localScale.x * waveWidth, 1, waves[x].transform.localScale.z);
            waves[x].transform.position = new Vector3(waves[x].transform.position.x * waveWidth, waves[x].transform.position.y, transform.position.z);
        }

        //Check if there is at least one microphone connected  

    }

    void Awake()
    {
        if (Microphone.devices.Length >= 0)
        {
            micConnected = true;
            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);


            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }
            minFreq = 2000;
            audioSource = null;
            audioSource = GetComponent<AudioSource>();
            //audioSource.clip = Microphone.Start(null, true, (int)1000, maxFreq);
        }
        else
        {
            Debug.LogWarning("Microphone not connected!");
        }
    }
    void LateUpdate()
    {

        if (GameManager.gameRestart)
        {
            restarWaveHeigts();

        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        // Call once when game is pused
        if (GameManager.instance.isPlaying == false)
        {
            audioSource.clip = null;
            recording = false;
            startedRecording = false;
        }

        //Call once when game starts or continues
        if (GameManager.instance.isPlaying && !recording)
        {
            recording = true;
            audioSource.clip = Microphone.Start(null, true, 5000, maxFreq);

            if (!startedRecording)
            {
                recordTimer = timer;
                startedRecording = true;
                audioSource.Play();
            }
        }



        if (GameManager.instance.isPlaying)
        {

            timer = Time.time;
            waveRestart = false;
            audioSource.GetSpectrumData(samples, micSwitch, FFTWindow.Rectangular);
            minWaterLevelLocal = minWaterLevel;
            maxWaterLevelLocal = maxWaterLevel;
            StartCoroutine(formWave());



            if (Ship.drowning)
            {
                for (int i = 0; i < waveAmount; i++)
                {
                    waves[i].GetComponent<Collider>().enabled = false;
                }
            }
            if (Ship.drowning == false)
            {
                for (int i = 0; i < waveAmount; i++)
                {
                    waves[i].GetComponent<Collider>().enabled = true;
                }
            }

        }
        else
        {
            StopCoroutine(formWave());

        }
        //Get the highest from array
        //Push it and its surounding waves
        //getHighestAmplitude();

    }

    //Get maimum amplitude from all frequencies samples[]
    //Not used Now
    void getHighestAmplitude()
    {
        int i;

        for (i = 1; i < waveAmount; i++)
        {
            if (samples[i] > maxWave)
            {
                maxWave = samples[i];
                maxWaveIndex = i;
            }
        }
        //print(maxWave + " maxWave" + " Index " + maxWaveIndex);
        StartCoroutine(formWave());
    }

    IEnumerator formWave()
    {
        //Do it for all wave elements in array
        for (int i = 0; i < waveAmount; i++)
        {
            //Do "blur" to smoothen wave
            smoother = 0;
            devider = 0;
            for (int x = -waveBlur / 2; x < waveBlur / 2; x++)
            {
                if (i + x > 0 && i + x < waveAmount)
                {
                    //count wave width
                    devider++;
                    if (x != 0)
                    {
                        smoother += waves[i + x].transform.localScale.y;
                    }
                    else
                    {
                        smoother += (minWaterLevelLocal + samples[i] * waveIntensity * 5000);
                        //waveY = (((waterLevel + samples[i] * waveIntensity * 1000) - (waterLevel + samples[i] * waveIntensity * 1000) / (waves[1].transform.localScale.x);
                    }
                }
                //waves[i].GetComponent<Renderer>().material.color = new Vector4(256 / (256 - 88), 256 / (256 - 121), 256 / waves[i].transform.localScale.y * 10);
            }
            //ORIGINAL WORKS
            //waveY = (((waterLevel + samples[i] * waveIntensity * 1000) - (waterLevel + samples[i] * waveIntensity * 1000) / wavesmoother) + smoother) / devider;


            if (waves[i].transform.localScale.y < maxWaterLevelLocal)
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, Mathf.Lerp(waves[i].transform.localScale.y, smoother / devider, waveSpeed / 500 + 0.01f), waves[1].transform.localScale.z);
            }
        }

        StartCoroutine(resetWave());


        yield return new WaitForSeconds(10);
    }

    //To slowly calm wave down to original position
    IEnumerator resetWave()
    {

        for (int i = 0; i < waveAmount; i++)
        {

            if (waves[i].transform.localScale.y > minWaterLevelLocal)
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, waves[i].transform.localScale.y - waveSpeed / 500 + 0.01f, waves[1].transform.localScale.z);
            }
            else
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, minWaterLevelLocal, waves[1].transform.localScale.z);
            }

        }
        yield return new WaitForSeconds(10);
    }

    //Get input from brain
    void brainRead()
    {
        //Range (0.16 to 40 Hz) ?
        // https://www.emotiv.com/forums/topic/How_to_extract_different_frequency_range_FFT/

    }

    public void restarWaveHeigts()
    {
        print("Reseting wave");
        for (int x = 0; x < waveAmount; x++)
        {

            waves[x].transform.localScale = new Vector3(waves[x].transform.localScale.x * waveWidth, minWaterLevel, waves[x].transform.localScale.z);
        }
        waveRestart = true;

    }
}


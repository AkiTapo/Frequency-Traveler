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
    int  maxWaveIndex;
    float expWaveHeight;

    [SerializeField]
    [Range(4, 100)]
    public int waveBlur = 15;
    [SerializeField]
    [Range (0, 30)]
    public float waveSpeed = 5f;
    [Range(0.1f,10f)]
   // public float wavesmoother = 5;
   // [Range(0f, 10f)]
    public int waveIntensity = 5;
    float waveY;


    //Audio
    private AudioSource audioSource;
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;
    bool playingAudio, startedRecording;
    float timer;
    float recordTimer;
    FFTWindow fFTWindow;
    float increaseLowFreq;
    //Audio


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
            minFreq = 200;
            audioSource = null;
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(null, true, (int)1000, maxFreq);
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        minWaterLevelLocal = minWaterLevel;
        maxWaterLevelLocal = maxWaterLevel;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
            audioSource = null;
        }
        timer = Time.time;

        audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        if (!startedRecording)
        {
            //audioSource = null;
            recordTimer = timer;
            startedRecording = true;
            audioSource.Play();
        }

        //Get the highest from array
        //Push it and its surounding waves
        //getHighestAmplitude();
        StartCoroutine(formWave());

        if (Ship.drowning)
        {
                for (int i = 0; i < waveAmount; i++)
            {
                waves[i].GetComponent<Collider>().enabled = false;
            }
        }
        if(Ship.shipReset)
        {
            print("IN");
            for (int i = 0; i < waveAmount; i++)
            {
                waves[i].GetComponent<Collider>().enabled = true;
            }
        }
    }

    //Get maimum amplitude from all frequencies samples[]
    //Not used Now
    void getHighestAmplitude()
    {
        int i;

        for (i = 1; i < waveAmount; i++)
        {
            if (samples[i] > maxWave) {
                maxWave = samples[i];
                maxWaveIndex = i;     
            }
        }
        //print(maxWave + " maxWave" + " Index " + maxWaveIndex);
        StartCoroutine(formWave());
    }

    IEnumerator formWave (){

        //Do it for all wave elements in array
        for (int i = 0; i < waveAmount; i++)
        {
            //Do "blur" to smoothen wave
            smoother = 0;
            devider = 0;
            for (int x = - waveBlur / 2; x < waveBlur /2 ; x++)
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


            if (waves[i].transform.localScale.y < maxWaterLevelLocal) {
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

            if (waves[i].transform.localScale.y > minWaterLevelLocal) {
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




    }

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Wave : MonoBehaviour
{

    public GameObject rectangle;
    // public float recordInterval;
    [Range(10, 120)]
    public int waveAmount;
    [Range(9, 15)]
    public int maxWater = 14;
    public float waveWidth, waterLevel;
    public float waveResetSpeed;

    GameObject[] waves;
    float maxWave;
    int  maxWaveIndex;
    float expWaveHeight;

    [SerializeField]
    [Range(2, 100)]
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
            waves[x] = Instantiate(rectangle, new Vector3(x * rectangle.GetComponent<Transform>().localScale.x - Mathf.Abs(transform.position.x), transform.position.y, 0), Quaternion.identity) as GameObject;
            waves[x].transform.localScale = new Vector3(waves[x].transform.localScale.x * waveWidth, 1, 1);
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
            audioSource.clip = Microphone.Start(null, true, (int)100, maxFreq);
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (Input.GetKey("escape"))
        {
            Application.Quit();
            audioSource = null;
        }
        timer = Time.time;

        audioSource.GetSpectrumData(samples, 0, fFTWindow);

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
                        smoother += (waterLevel + samples[i] * waveIntensity * 5000);
                        //waveY = (((waterLevel + samples[i] * waveIntensity * 1000) - (waterLevel + samples[i] * waveIntensity * 1000) / (waves[1].transform.localScale.x);
                    }
                }
                //waves[i].GetComponent<Renderer>().material.color = new Vector4(256 / (256 - 88), 256 / (256 - 121), 256 / waves[i].transform.localScale.y * 10);
            }
            //ORIGINAL WORKS
            //waveY = (((waterLevel + samples[i] * waveIntensity * 1000) - (waterLevel + samples[i] * waveIntensity * 1000) / wavesmoother) + smoother) / devider;


            if (waves[i].transform.localScale.y < maxWater) {
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

            if (waves[i].transform.localScale.y > waterLevel) {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, waves[i].transform.localScale.y - waveSpeed / 500 + 0.01f, waves[1].transform.localScale.z);
            }
            else
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, waterLevel, waves[1].transform.localScale.z);
            }
        
        }
        yield return new WaitForSeconds(10);
    }
    

}

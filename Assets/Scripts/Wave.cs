using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Arduino
using System.IO.Ports;
using System;

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
    public static int _waveBlur;
    [SerializeField]
    [Range(0, 30)]
    public float waveSpeed = 5f;
    public static float _waveSpeed;
    [Range(1f, 10f)]
    public float waveIntensity = 5;
    public static float _waveIntensity;
    float waveY;
    private bool recording;
    [Range(0, 1)]
    // public int micSwitch = 0;

    //Audio
    private AudioSource audioSource;
    private bool micConnected = false;
    private int minFreq = 2000;
    private int maxFreq;
    bool playingAudio, startedRecording, startLightSensors;
    float timer;
    float recordTimer;
    FFTWindow fFTWindow;
    float increaseLowFreq;
    //Audio
    public static bool waveRestart;


    private float[] samples = new float[1024];
    private float smoother;
    private int devider;


    //Arduino
    float[] lightSensor = new float[4];
    public string arduinoPort = "COM4";
    SerialPort sp;
    int arduinoStringToInt;
    bool micParametersSet, lightParametersSet;
    float intensityMulti = 1;
    int[] lightSensorCenters = new int[] { 15, 38, 67, 92 };
    private int samplesAway;

    // Use this for initialization
    void Start()
    {
        //Arduino

        try
        {
            sp = new SerialPort(arduinoPort, 9600);
            sp.Open();
            sp.ReadTimeout = 10;
        }
        catch
        {
            print("Arduino is not connected or port is not availible");
        }


        _waveSpeed = waveSpeed;
        _waveBlur = waveBlur;
        _waveIntensity = waveIntensity;

        waves = new GameObject[waveAmount];

        for (int x = 0; x < waveAmount; x++)
        {
            waves[x] = Instantiate(waveShape, new Vector3(x * waveShape.GetComponent<Transform>().localScale.x - Mathf.Abs(transform.position.x), transform.position.y, 0), Quaternion.identity) as GameObject;
            waves[x].transform.localScale = new Vector3(waves[x].transform.localScale.x * waveWidth, 1, waves[x].transform.localScale.z);
            waves[x].transform.position = new Vector3(waves[x].transform.position.x * waveWidth, waves[x].transform.position.y, transform.position.z);
        }
        setInputParameters();
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
        //_waveSpeed = waveSpeed;
        //_waveBlur = waveBlur;
        //_waveIntensity = waveIntensity;

        if (GameManager.gameRestart)
        {
            restarWaveHeigts();

        }

        if (Input.GetMouseButtonUp(0))
        {
            changeInput();
            resetWave();
            StopCoroutine(formAWave());
            StartCoroutine(formAWave());
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
        if (GameManager.instance.isPlaying)
        {
            if (inputSwitch == 1)
            {
                //put all bellow to if
                recording = true;

            }



        }
        if (!startedRecording && inputSwitch == 1)
        {
            startLightSensors = false;
            startedRecording = true;
            audioSource.clip = Microphone.Start(null, true, 5000, maxFreq);
            recordTimer = timer;
            audioSource.Play();

        }

        if (!startLightSensors && inputSwitch == 2)
        {
            startLightSensors = true;
            startedRecording = false;
            audioSource.Stop();
            audioSource.clip = null;
            //audioSource = null;
            //audioSource.enabled = false;
            //GetComponent<AudioListener>().enabled = false;
        }


        if (GameManager.instance.isPlaying)
        {

            timer = Time.time;
            waveRestart = false;
            if (inputSwitch == 1)
            {
                audioSource.GetSpectrumData(samples, 1, FFTWindow.Rectangular);
            }

            minWaterLevelLocal = minWaterLevel;
            maxWaterLevelLocal = maxWaterLevel;
            StartCoroutine(formAWave());

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
            StopCoroutine(formAWave());

        }

    }


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
        //formAWave();
    }

    IEnumerator formAWave()
    {
        //If input is light sensor
        if (inputSwitch == 2)
        {
            try
            {
                if (Int32.TryParse(sp.ReadLine(), out arduinoStringToInt))
                {
                    //print(arduinoStringToInt);
                    if (arduinoStringToInt < 1999)
                    {
                        lightSensor[0] = arduinoStringToInt - 1700;
                        //samples[samples.Length / lightSensor.Length / 2] = lightSensor[0];
                        //samples[128] = lightSensor[0];
                    }
                    if (arduinoStringToInt > 1999 && arduinoStringToInt < 2999)
                    {
                        lightSensor[1] = arduinoStringToInt - 2700;
                        //samples[samples.Length / lightSensor.Length + samples.Length / lightSensor.Length / 2] = lightSensor[0];
                        //samples[384] = lightSensor[1];
                    }
                    if (arduinoStringToInt > 2999 && arduinoStringToInt < 3999)
                    {
                        lightSensor[2] = arduinoStringToInt - 3700;
                        //samples[samples.Length / 2 + samples.Length / lightSensor.Length / 2] = lightSensor[2];
                        //samples[640] = lightSensor[2];
                    }
                    if (arduinoStringToInt > 3999)
                    {
                        lightSensor[3] = arduinoStringToInt - 4700;
                        //samples[samples.Length / lightSensor.Length * 3 + samples.Length / lightSensor.Length / 2] = lightSensor[3];
                        //samples[896] = lightSensor[3];

                    }
                }
                else
                {
                    print("String from Arduino could not be parsed.");
                }

            }
            catch (System.Exception)
            {

            }
        }

        //Do it for all wave elements in array
        for (int i = 0; i < waveAmount; i++)
        {
            //Do "blur" to smoothen wave
            smoother = 0;
            devider = 0;

            //If microphone as an input
            if (inputSwitch == 1)
            {
                    for (int x = -waveBlur / 2 + 1; x < waveBlur / 2; x++)
                {
                    //check if x is above zero and up till max wave amount
                    if (i + x > 0 && i + x < waveAmount)
                    {
                        // if microphone as input

                        //count wave width to blur
                        devider++;

                        if (x != 0)
                        {
                            //For other waves than current
                            smoother += waves[i + x].transform.localScale.y;
                        }
                        else
                        {
                            //for current wave
                            smoother += (minWaterLevelLocal + samples[i] * waveIntensity * intensityMulti);
                        }
                    }
                }
            }
            //If light sensors as an input
            if (inputSwitch == 2)
            {
                //devider++;
                for (int y = 0; y < 4; y++)
                {
                    //for light sensor representitive wave
                    if (i == lightSensorCenters[y])
                    {
                        samples[i] = minWaterLevelLocal + lightSensor[y] - lightSensor[y] / waveIntensity;
                        //print(samples[i]);
                    }
                    // check weather current i for wave is close to sample that is reacting to sensor
                    if (i != lightSensorCenters[y] && Mathf.Abs(i - lightSensorCenters[y]) < waveBlur / 2)
                    {
                        samplesAway = lightSensorCenters[y] - i;
                        // WORKS //samples[i] = samples[i + samplesAway] - Mathf.Abs(samplesAway) / 1.2f;
                        samples[i] = samples[i + samplesAway] - Mathf.Abs(samplesAway) / (1 / (1 + Mathf.Exp(-Mathf.Abs(samplesAway)))) * Mathf.Abs(samplesAway) / 20f;

                        //sigmoid model
                        //1 / (1 + (Mathf.Pow(Mathf.Exp, -i)
                    }
                }

            }

            // if wave reached maximum level, set it back to max
            if (waves[i].transform.localScale.y >= maxWaterLevelLocal)
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, maxWaterLevelLocal - 0.1f, waves[1].transform.localScale.z);
            }
            else
            {
                if (inputSwitch == 1)
                {
                    waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, Mathf.Lerp(waves[i].transform.localScale.y, smoother / devider, waveSpeed / 500 + 0.01f), waves[1].transform.localScale.z);
                }
                if (inputSwitch == 2)
                {
                    waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, Mathf.Lerp(waves[i].transform.localScale.y, samples[i], waveSpeed / 500 + 0.01f), waves[1].transform.localScale.z);
                }
            }
        }

        //waves[50].transform.localScale = new Vector3(waves[50].transform.localScale.x, Mathf.Lerp(waves[50].transform.localScale.y, minWaterLevelLocal + lightSensor[0], waveSpeed / 500 + 0.01f), waves[50].transform.localScale.z);

        StartCoroutine(resetWave());


        yield return new WaitForSeconds(1);
    }

    //To slowly calm wave down to original position
    IEnumerator resetWave()
    {
        for (int i = 0; i < waveAmount; i++)
        {

            if (waves[i].transform.localScale.y <= minWaterLevelLocal)
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, minWaterLevelLocal, waves[1].transform.localScale.z);

            }
            else
            {
                waves[i].transform.localScale = new Vector3(waves[1].transform.localScale.x, waves[i].transform.localScale.y - waveSpeed / 500 + 0.01f, waves[1].transform.localScale.z);
            }
        }
        yield return new WaitForSeconds(1);
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
    public void changeInput()
    {
        if (inputSwitch == 2)
        {
            inputSwitch = 1;
            setInputParameters();

        }
        else if(inputSwitch != 2)
        { 
            inputSwitch = 2;
            setInputParameters();
        }
    }

    void setInputParameters()
    {
        //Input  is microphone
        if (inputSwitch == 1 && !micParametersSet)
        {
            micParametersSet = true;
            lightParametersSet = false;
            waveBlur = 16;
            waveSpeed = 6;
            waveIntensity = 6;
            intensityMulti = 3000;
            print("Input: Microphone");

        }
        //If input is light sensors
        if (inputSwitch == 2 && !lightParametersSet)
        { 
            print("Input: Light Sensors");
            lightParametersSet = true;
            micParametersSet = false;
            waveBlur = waveAmount / 4 +1;
            waveSpeed = 1;
            waveIntensity = 1.1f;
            intensityMulti = 2;
        }
    }
    /*
    //Arduino
    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }
    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (System.TimeoutException)
        {
            return null;
        }
    }




public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }
    */
}


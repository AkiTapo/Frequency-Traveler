using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class Move : MonoBehaviour
{
    public string port = "COM4";
    SerialPort sp;

    void Start()
    {
        try {
            sp = new SerialPort(port, 9600);
            sp.Open();
            sp.ReadTimeout = 10;
        }
        catch
        {
            print("Arduino is not connected or port is not availible");
        }
    }

    void LateUpdate()
    {
        try
        {
            print(sp.ReadLine());
        }
        catch (System.Exception)
        {

        }
    }
}
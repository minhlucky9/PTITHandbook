using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class RealTimeClock : MonoBehaviour
{
    public GameObject theDisplay;
    public int Hour;
    public int Minute;
    public int Second;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Hour = System.DateTime.Now.Hour;
        Minute = System.DateTime.Now.Minute;
        Second = System.DateTime.Now.Second;
        theDisplay.GetComponent<Text>().text = "" + Hour + ":" + Minute + ":" + Second;
    }
}

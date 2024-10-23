using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class SwitchCamera : MonoBehaviour
{
    public GameObject[] CameraList;
    public GameObject Camera_1;
    public GameObject Camera_2;
    public int Manager;
    public int index;
    // Start is called before the first frame update
    void Start()
    {

       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void ManagedCamera()
    {
        if (Manager == 0)
        {
            Cam2();
            Manager = 1;
        }
        else
        {
            Cam1();
            Manager = 0;
        }
    }

    public void Cam1()
    {
            Camera_1.SetActive(true);
            Camera_2.SetActive(false);
        
     
     
    }

    public void Cam2()
    {
       
           
                Camera_1.SetActive(false);
                Camera_2.SetActive(true);
    
            
       
    }
}

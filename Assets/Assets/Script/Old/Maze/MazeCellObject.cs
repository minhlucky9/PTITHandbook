using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject leftWall;
    [SerializeField] public GameObject endPoint;

    public void Init (bool top, bool bottom, bool right, bool left, bool endpoint)
    {
        topWall.SetActive (top);
        bottomWall.SetActive (bottom);
        rightWall.SetActive (right);
        leftWall.SetActive (left);
        endPoint.SetActive (endpoint);
    }

    public void InitEndPoint(bool endpoint)
    {
        endPoint.SetActive (endpoint);
        
    }
}

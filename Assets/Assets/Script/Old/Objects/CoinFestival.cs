using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CoinFestival : MonoBehaviour
{
    public float rotateSpeedZ = 5;
    public float rotateSpeedX;

    [Header("Config")]
    [SerializeField] private float respawnTimeSeconds = 8;
    [SerializeField] private int goldGained = 1;

    private BoxCollider circleCollider;
    private MeshRenderer visual;

    private void Awake() 
    {
        circleCollider = GetComponent<BoxCollider>();
        visual = GetComponentInChildren<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, rotateSpeedX, rotateSpeedZ);
    }
    private void CollectCoin() 
    {
        circleCollider.enabled = false;
        visual.gameObject.SetActive(false);
     //   GameEventsManager.instance.goldEvents.GoldGained(goldGained);
        GameEventsManager.instance.miscEvents.StarFestivalCollected();
      //  StopAllCoroutines();
      //  StartCoroutine(RespawnAfterTime());
    }

    private IEnumerator RespawnAfterTime()
    {
        yield return new WaitForSeconds(respawnTimeSeconds);
        circleCollider.enabled = true;
        visual.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider otherCollider) 
    {
        if (otherCollider.CompareTag("Player"))
        {
            CollectCoin();
        }
    }
}
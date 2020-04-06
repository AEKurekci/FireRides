using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] AudioClip breakSound;
    [SerializeField] [Range(0, 1)] float volumeOfBreak = 0.5f;
    [SerializeField] int pointValue = 1;
    [SerializeField] bool small = false;

    private void OnTriggerEnter(Collider collision)
    {
        FindObjectOfType<PointDisplay>().ShowPoint(pointValue);
        if (small)
        {
            Destroy(transform.parent.gameObject);
            FindObjectOfType<Motion>().ShowPoint("WOW");
        }
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volumeOfBreak);
    }
}

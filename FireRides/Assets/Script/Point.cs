using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] AudioClip breakSound;
    [SerializeField] [Range(0, 1)] float volumeOfBreak = 0.5f;

    private void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volumeOfBreak);
    }
}

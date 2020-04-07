using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    /*
     * pointValue: It keeps value to give player when player breaks the circle
     * isSmall: It defines circle class belongs to nested small green circle or big yellow circle
     * when isSmall is true, it means circle is green small circle
     */
    [SerializeField] AudioClip breakSound;
    [SerializeField] [Range(0, 1)] float volumeOfBreak = 0.5f;
    [SerializeField] int pointValue = 1;
    [SerializeField] bool isSmall = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (isSmall)
        {
            transform.GetComponentInChildren<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().enabled = false;
            AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volumeOfBreak);
            ParentDisable();
            Invoke("DestroyParent", 1f);
            string pointMessage = DetectMessage(collision.gameObject);
            FindObjectOfType<Motion>().ShowPoint(pointMessage);

        }
        else
        {
            transform.gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Invoke("DestroyIt", 1f);
            collision.gameObject.GetComponent<Ball>().pointSerious = 0;
            pointValue = 1;
        }
        AudioSource.PlayClipAtPoint(breakSound, Camera.main.transform.position, volumeOfBreak);
        FindObjectOfType<PointDisplay>().ShowPoint(pointValue);
    }

    private string DetectMessage(GameObject player)
    {
        string theMessage = "";
        int serious = player.GetComponent<Ball>().pointSerious;
        if (serious >= 3)
            serious = 3;
        switch (serious)
        {
            case 0:
                player.GetComponent<Ball>().pointSerious += 1;
                pointValue = 2;
                theMessage = "WOW";
                break;
            case 1:
                player.GetComponent<Ball>().pointSerious += 1;
                pointValue = 3;
                theMessage = "COOL";
                break;
            case 2:
                player.GetComponent<Ball>().pointSerious += 1;
                pointValue = 4;
                theMessage = "AWESOME";
                break;
            case 3:
                player.GetComponent<Ball>().pointSerious += 1;
                pointValue = 5;
                theMessage = "MIND-BLOWING";
                break;
        }
        return theMessage;
    }

    private void ParentDisable()
    {
        transform.parent.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        transform.parent.gameObject.GetComponents<CapsuleCollider>()[0].enabled = false;
        transform.parent.gameObject.GetComponents<CapsuleCollider>()[1].enabled = false;
    }

    private void DestroyIt()
    {
        Destroy(gameObject);
    }

    private void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}

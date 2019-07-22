using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebar : MonoBehaviour
{

    private readonly int rotationSpeed = 70;


    void Update()
    {
        transform.RotateAround(transform.parent.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerMovement>().Hit();
        }
    }

}

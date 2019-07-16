using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckUp : MonoBehaviour
{

    public bool hasGround;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            hasGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            hasGround = false;
        }
    }


}

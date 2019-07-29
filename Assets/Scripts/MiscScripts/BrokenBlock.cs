using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBlock : MonoBehaviour
{

    [SerializeField] private GameObject brokenPartUpRight;
    [SerializeField] private GameObject brokenPartUpLeft;
    [SerializeField] private GameObject brokenPartDownRight;
    [SerializeField] private GameObject brokenPartDownLeft;

    void Start()
    {
        AddForceToParts();
        Destroy(gameObject, 4.0f);
    }

    private void AddForceToParts()
    {
        float xForce = Mathf.Cos(145) * 50;
        float yForce = Mathf.Sin(90) * 300;
        brokenPartUpRight.GetComponent<Rigidbody2D>().AddForce(new Vector2(xForce, yForce));
        brokenPartUpLeft.GetComponent<Rigidbody2D>().AddForce(new Vector2(-xForce, yForce));
        brokenPartDownRight.GetComponent<Rigidbody2D>().AddForce(new Vector2(xForce, yForce / 1.2f));
        brokenPartDownLeft.GetComponent<Rigidbody2D>().AddForce(new Vector2(-xForce, yForce / 1.2f));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] Material material;

    private Vector2 offset;
    private float speed = 0.1f;


    void Start()
    {
        offset = new Vector2(speed, 0.0f);
    }

    void Update()
    {
        if (GameManager.isScrollingOn)
        {
            transform.position = new Vector2(player.transform.position.x + 0.5f, transform.position.y);
            material.mainTextureOffset += offset * Time.deltaTime;
        }
    }

}

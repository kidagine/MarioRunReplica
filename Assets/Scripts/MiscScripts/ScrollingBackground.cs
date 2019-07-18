using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameObject player;
    [SerializeField] private Material material;

    private Vector2 offset;
    private readonly float speed = 0.1f;

    void Update()
    {
        if (playerMovement.rb != null && !playerMovement.isWallInfront)
        {
            if (!GameManager.isBubbled)
            {
                offset = new Vector2(playerMovement.rb.velocity.x / 20, 0.0f);
            }
            else
            {
                Bubble bubble = FindObjectOfType<Bubble>();
                if (!bubble.hasReachedStart)
                {
                    offset = new Vector2(-0.5f, 0.0f);
                }
                else
                {
                    offset = new Vector2(0.0f, 0.0f);
                }
            }
            material.mainTextureOffset += offset * Time.deltaTime;
            transform.position = new Vector2(player.transform.position.x + 0.5f, transform.position.y);
        }
    }

}

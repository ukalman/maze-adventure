using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowYRotation : MonoBehaviour
{
    [Header("Minimap Rotations")] 
    private Transform player;
    [SerializeField] private float playerOffset = 10.0f;    
    void Start()
    {
        player = GameManager.Instance.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y + playerOffset, player.position.z);
            transform.rotation = Quaternion.Euler(90.0f, player.eulerAngles.y, 0.0f);
        }
    }
}

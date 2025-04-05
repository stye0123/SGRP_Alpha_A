using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
} 
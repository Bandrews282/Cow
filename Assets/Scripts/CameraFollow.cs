using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour
{
    [SerializeField][Range(5,50)] 
    private float smoothSpeed = 5;

    [SerializeField]
    private Vector3 offset;

    [SerializeField, Range(1f, 10f)]
    private float minZoom, maxZoom;

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float orthoSize = Camera.main.orthographicSize;

        if(Input.GetAxis("Mouse Scrollwheel") > 0 || Input.GetKey(KeyCode.S))
        {
            orthoSize -= 0.5f;
        }
        if (Input.GetAxis("Mouse Scrollwheel") < 0 || Input.GetKey(KeyCode.W))
        {
            orthoSize += 0.5f;
        }

        Camera.main.orthographicSize = Mathf.Clamp(orthoSize, 1f, 10f);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPositon = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPositon;
    }
}

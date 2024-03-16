using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrackCamera : MonoBehaviour
{
    public Transform target;

    public float h = 40;
    public float z = -26;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var destPositon = target.position + new Vector3(0, h, z);

        transform.position = Vector3.SmoothDamp(transform.position, destPositon, ref velocity, 1.0f, 10.0f);
    }
}

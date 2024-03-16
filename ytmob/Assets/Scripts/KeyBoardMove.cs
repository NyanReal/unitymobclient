using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardMove : MonoBehaviour
{
    public Vector3 movedebug;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vx = 0;
        float vz = 0;
        if (Input.GetKey(KeyCode.W)) vz += 1.0f;
        if (Input.GetKey(KeyCode.S)) vz -= 1.0f;
        if (Input.GetKey(KeyCode.A)) vx -= 1.0f;
        if (Input.GetKey(KeyCode.D)) vx += 1.0f;
        Vector3 move = (Vector3.right * vx + Vector3.forward * vz) * Time.deltaTime;
        transform.Translate(move);
        movedebug = move;
    }
}

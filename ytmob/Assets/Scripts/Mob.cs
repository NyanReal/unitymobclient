using UnityEngine;

public class Mob : MonoBehaviour
{
    Vector3 destination;
    public float speed = 10f;

    public float minDist = 1 / 60.0f;

    public int st = 0;

    void Start()
    {
        destination = transform.position;
    }

    public void  SetDest  (Vector3 d)
    {
        destination = d;
    }

    // Update is called once per frame
    void Update()
    {
        float diff = Vector3.Distance(transform.position, destination);
        st = 0;

        if (diff > speed * minDist)
        {
            var dir = (destination - transform.position);
            dir.Normalize();
            st = 1;

            transform.Translate(dir * speed * Time.deltaTime, Space.World);
            transform.LookAt(destination, Vector3.up);
        }
        else if (diff > float.Epsilon)
        {
            st = 2;
            transform.position = destination;
        }
    }
}

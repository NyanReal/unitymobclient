//using FreeNet;
using Bass.Net.Client;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseMove : MonoBehaviour
{
    public Vector3 destination;

    public float speed = 10f;

    public float minDist = 1 / 60.0f;

    public int st = 0;

#if USE_FREENET_BASE_CLIENT
    public TCPTestClient testClient;
    public FRNClient testClient2;
#endif

    public ClientNetwork ClientNetworkSocket;



    public TextMeshProUGUI nextPos;

    public Transform myAvatar;
    public Transform destPoint;

    public Mob boxBlue;
    public Mob boxGhost;

    public short myid = -1;

    static public MouseMove inst;

    Dictionary<short, Mob> mapChar = new Dictionary<short, Mob>();

    ConcurrentQueue<MoveData> movedatalist = new ConcurrentQueue<MoveData>();

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        destination = myAvatar.position;
    }

    public void Cast(MoveData d)
    {
        movedatalist.Enqueue(d);
        Debug.Log(movedatalist.Count);
    }

    public void SetMyID(short id)
    {
        this.myid = id;
    }

    public void Cast(short userid, float x, float y, float z, float r)
    {
        Debug.Log($"Cast222 {userid} {x} {y} ");

        if (!mapChar.ContainsKey(userid))
        {
            Mob prefab = boxBlue;
            if (userid == myid)
            {
                prefab = boxGhost;
            }

            Mob mob = Instantiate<Mob>(prefab, new Vector3(x, y, z), Quaternion.identity);
            mapChar.Add(userid, mob);
        }

        mapChar[userid].SetDest(new Vector3(x, y, z));
    }


    // Update is called once per frame
    void Update()
    {
        if (movedatalist.TryDequeue(out var msg))
        {
            Debug.Log("Dequeue");
            short userid = msg.userid;
            float x = msg.x;
            float y = msg.y;
            float z = msg.z;
            float r = msg.r;
            Cast(userid, x, y, z, r);
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("LEFT");


            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo))
            {
                destination = hitInfo.point;
                ClientNetworkSocket?.SendPos(destination);


                Debug.Log($"{Input.mousePosition.ToString()} {destination.ToString()}");

                //.SendDestPos(destination);
                destPoint.position = destination + Vector3.up;
                destPoint.gameObject.SetActive(true);
            }
        }

        nextPos.text = $"{myAvatar.position.x:F1},{myAvatar.position.y:F1} -> {destination.x:F1},{destination.y:F1}";

        float diff = Vector3.Distance(myAvatar.position, destination);

        st = 0;

        if (diff > speed * minDist)
        {
            var dir = (destination - myAvatar.position);
            dir.Normalize();
            st = 1;

            myAvatar.Translate(dir * speed * Time.deltaTime, Space.World);
            myAvatar.LookAt(destination, Vector3.up);
        }
        else if (diff > float.Epsilon)
        {
            st = 2;
            myAvatar.position = destination;
            destPoint.gameObject.SetActive(false);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        if (st == 1) Gizmos.color = Color.yellow;
        if (st == 2) Gizmos.color = Color.red;
        if (st > 0)
        {
            Gizmos.DrawWireSphere(transform.position, 1.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(destination, 1.0f);
        }
    }
}

public class MoveData
{
    public short userid;
    public float x, y, z, r;
}



using CSampleClient;
using System;
using System.Net;
using UnityEngine;

public class FRNClient : MonoBehaviour
{
    string yturl = string.Empty;
    public ProgramEntry programEntry = null;
    // Start is called before the first frame update
    void Start()
    {
        WebClient wc = new WebClient();
        var serverlist = wc.DownloadString("https://nyanreal.github.io/ytmob/serverlist.txt");

        var lines = serverlist.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        string domain = string.Empty;
        int port = 0;


        foreach (var line in lines)
        {
            var cols =  line.Split(",", StringSplitOptions.RemoveEmptyEntries);
            if(cols.Length == 2)
            {
                yturl = cols[1];
                Debug.Log("yturl = " +  yturl);             

                var ipport = cols[0].Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (ipport.Length == 2)
                {
                    domain = ipport[0];
                    int.TryParse(ipport[1], out port);
                }
            }
        }

        //Debug.Log($" {domain} :: {port}");

        var addresses = Dns.GetHostAddresses(domain);

        //addresses = Dns.GetHostAddresses("127.0.0.1");

        foreach (var address in addresses)
        {
            //Debug.Log(address.ToString());
        }



        IPEndPoint endpoint = new IPEndPoint(addresses[0], port);

        programEntry = new ProgramEntry();
        programEntry.Connect(endpoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

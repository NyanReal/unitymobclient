#if USE_FREENET_BASE_CLIENT

//using CSampleClient;
using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FRNClient : MonoBehaviour
{
    string yturl = string.Empty;
    public ProgramEntry programEntry = null;
    public Button btnLive;

    public TextMeshProUGUI netError;

    // Start is called before the first frame update
    void Start()
    {
        btnLive.gameObject.SetActive(false);

        WebClient wc = new WebClient();
        var verurl = $"https://nyanreal.github.io/ytmob/versions/{Application.version}.txt";
        var storeurl = $"https://nyanreal.github.io/ytmob/store.txt";

        Debug.Log("ver : " + verurl);

        string verinfo = string.Empty;
        try
        {
            verinfo = wc.DownloadString(verurl);
        }
        catch (WebException ex)
        {
            netError.text = ex.Message;
            return;
        }


        if (verinfo == string.Empty)
        {
            netError.text = "version info not found!";
            return;
        }

        if (!verinfo.Equals("OK"))
        {
            netError.text = verinfo;
            try
            {
                Debug.Log(storeurl);
                var storelink = wc.DownloadString(storeurl);

                Debug.Log(storelink);

                if (storelink.Contains("play.google.com"))
                {
                    Application.OpenURL(storelink);
                }
            }
            catch (WebException ex)
            {
                netError.text = ex.Message;
                return;
            }
            return;
        }

        var serverlist = wc.DownloadString("https://nyanreal.github.io/ytmob/serverlist.txt");

        var lines = serverlist.Split("\n", StringSplitOptions.RemoveEmptyEntries);

        string domain = string.Empty;
        int port = 0;


        foreach (var line in lines)
        {
            var cols = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (cols.Length == 2)
            {
                yturl = cols[1];
                btnLive.gameObject.SetActive(true);
                Debug.Log("yturl = " + yturl);

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


    public void OpenUrl()
    {
        Application.OpenURL(yturl);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

#endif
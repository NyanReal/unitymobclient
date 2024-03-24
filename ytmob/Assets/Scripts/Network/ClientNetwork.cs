using B3.CSCommon;
using B3.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ClientNetwork : MonoBehaviour
{
    [SerializeField]
    public Button BtnLive;
    public Button BtnConnect;

    public TMP_Dropdown dropdown;

    public MouseMove mouse;

    [SerializeField]
    public TextMeshProUGUI NetworkError;

    private string yturl = string.Empty;


    private Dictionary<string, IPEndPoint> mServerList = new Dictionary<string, IPEndPoint>();
    private string mLastServerKey = string.Empty;


    private void Awake()
    {
        B3Network.Instance.OnConnectedCallback = _OnConected;
        B3Network.Instance.OnDisconnectedCallback = _OnDisconnected;
        B3Network.Instance.OnConnectFailedCallback = _OnConnectFailed;
    }

    private void Start()
    {
        _LiveButtonActive(false);

        if (false == _VersionCheck())
            return;

        if (false == _GetServerList())
            return;

        if (mServerList.Count == 0)
            return; // no Server List;
    }


    private void Update()
    {
        // Packet 처리
        if (false == B3Network.Instance.IsConnected)
            return;

        var recvList = B3Network.Instance.GetReceivePacketList();
        if (recvList.Count == 0)
            return;

        foreach (var packet in recvList)
        {
            bool bRet = _DispatchPacket(packet);
            if (false == bRet)
            {
#if DEBUG
                Console.WriteLine($"Packet Dispatch Failed!! Protocol ({packet.Protocol})");
#endif
            }
        }

        recvList.Clear();
    }


    public void OnClickConnect()
    {
        Debug.Log("server info " + dropdown.value );
        mLastServerKey = mServerList.Keys.ToList().ElementAt(dropdown.value);

        var connectEndPoint = mServerList[mLastServerKey];

        Debug.Log("endpoint info " + connectEndPoint.ToString());

        B3Network.Instance.Connect(connectEndPoint.Address.ToString(), connectEndPoint.Port);
        mouse.EnableInput = true;
    }


    /// <summary>
    /// 서버 목록 URL로부터 서버 목록을 얻어옵니다.
    /// </summary>
    /// <returns>작업 성공 여부</returns>
    private bool _GetServerList()
    {
        dropdown.ClearOptions();

        List<string> listString = new List<string>();

        const string serverListUrl = "https://nyanreal.github.io/ytmob/serverlist.txt";

        using (WebClient wc = new WebClient())
        {
            try
            {
                var serverlist = wc.DownloadString(serverListUrl);

                var lines = serverlist.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                string domain = string.Empty;
                int port = 0;

                mServerList.Clear();

                foreach (var line in lines)
                {
                    if (mServerList.ContainsKey(line))
                        continue;   // 이미 동일 주소 존재

                    var cols = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (cols.Length == 3)
                    {
                        var servername = cols[0];
                        yturl = cols[2];
                        BtnLive?.gameObject?.SetActive(true);
                        Debug.Log("yturl = " + yturl);
                        Debug.Log("servername = " + servername);

                        listString.Add(servername);

                        var ipport = cols[1].Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (ipport.Length == 2)
                        {
                            domain = ipport[0];
                            int.TryParse(ipport[1], out port);
                        }
                        else
                        {
                            continue;   // invalid format
                        }

                        var addresses = Dns.GetHostAddresses(domain);
                        if (addresses.Length == 0)
                            continue;   // invalid address

                        IPEndPoint endpoint = new IPEndPoint(addresses[0], port);
                        mLastServerKey = line;
                        mServerList.Add(line, endpoint);
                    }
                }
            }
            catch (Exception e)
            {
                _NetworkErrorMessage(e);
                return false;
            }
        }

#if DEBUG_SERVER
        mLastServerKey = "127.0.0.1";
        mServerList.Add("127.0.0.1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), CommonData.SERVER_PORT));
#endif

        dropdown.AddOptions(listString);

        return true;
    }


    /// <summary>
    /// 어플리케이션 버전을 확인합니다.
    /// </summary>
    /// <returns>버전 확인 성공 여부</returns>
    private bool _VersionCheck()
    {
        var verurl = $"https://nyanreal.github.io/ytmob/versions/{Application.version}.txt";
        const string storeurl = "https://nyanreal.github.io/ytmob/store.txt";

        Debug.Log("ver : " + verurl);

        string verinfo = string.Empty;


        using (WebClient wc = new WebClient())
        {
            try
            {
                verinfo = wc.DownloadString(verurl);
            }
            catch (WebException ex)
            {
                _NetworkErrorMessage(ex);
                return false;
            }


            if (string.IsNullOrWhiteSpace(verinfo))
            {
                _NetworkErrorMessage("version info not found!");
                return false;
            }

            if (!verinfo.Equals("OK"))
            {
                _NetworkErrorMessage(verinfo);

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
                    _NetworkErrorMessage(ex);
                }

                return false;
            }
        }
        return true;
    }


    #region 외부 컨트롤 처리

    private void _LiveButtonActive(bool onoff) => BtnLive?.gameObject?.SetActive(onoff);


    private void _NetworkErrorMessage(Exception e) => _NetworkErrorMessage(e.Message);

    private void _NetworkErrorMessage(string msg)
    {
        if (null == NetworkError)
            return;

        NetworkError.text = msg;
    }


    #endregion




    #region Network Event delegate

    private void _OnConnectFailed()
    {
        Debug.Log("Connect Failed...");
    }

    private void _OnDisconnected()
    {
        Debug.Log("Disconnected!");
    }

    private void _OnConected()
    {
        Debug.Log("Connected!");
    }

    #endregion



}

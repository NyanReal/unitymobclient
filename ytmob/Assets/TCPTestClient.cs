
// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License. 
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/ 
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



// https://github.com/danielbierwirth



public class TCPTestClient : MonoBehaviour
{
    //public Transform TransformPlayer;

    ConcurrentQueue<Vector3> poslist = new ConcurrentQueue<Vector3>();

    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    #endregion
    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SendMessage();
        }
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            //socketConnection = new TcpClient("sam.gamebass.net", 3369);
            socketConnection = new TcpClient("127.0.0.1", 3369);

            Byte[] bytes = new Byte[24];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while (true)
                    {
                        if (!stream.DataAvailable)
                        {
                            Thread.Sleep(1000);
                            //stream.
                            Debug.Log("W1000");
                            continue;
                        }

                        Debug.Log("read "+ bytes.Length);
                        length = stream.Read(bytes, 0, bytes.Length);

                        if (length == 0)
                        {
                            Debug.Log("zero len");
                            
                            continue;
                            //break;
                        }

                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        //string serverMessage = Encoding.ASCII.GetString(incommingData);

                        string serverMessage = "BYTE";

                        foreach(byte b in incommingData)
                        {
                            serverMessage +=  " " + b.ToString("X");
                        }

                        if(poslist.Count > 0)
                        {
                            if(poslist.TryDequeue(out var vDest))
                            {
                                //SendDestPosInternal(vDest);
                            }                            
                        }

                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendDestPosQ(Vector3 pos)
    {
        poslist.Enqueue(pos);
    }

    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void SendDestPos(Vector3 pos)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();

            if (stream.CanWrite)
            {
                // short 3
                // x,y,z,r
                int len = 22; // 4 2 4 4 4 4
                float r = 0;
                short pname = 3;

                List<byte> data = new List<byte>();
                data.AddRange(BitConverter.GetBytes(len));
                data.AddRange(BitConverter.GetBytes(pname));
                data.AddRange(BitConverter.GetBytes(pos.x));
                data.AddRange(BitConverter.GetBytes(pos.y));
                data.AddRange(BitConverter.GetBytes(pos.z));
                data.AddRange(BitConverter.GetBytes(r));
                stream.Write(data.ToArray(), 0, 22);

                Debug.Log("SEND");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
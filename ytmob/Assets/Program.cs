using System;
using System.Collections.Generic;
using System.Net;
using FreeNet;

using GameServer;
using UnityEngine;

namespace CSampleClient
{
    public class ProgramEntry
	{
		static List<IPeer> game_servers = new List<IPeer>();

		CConnector connector = null;


        public void Connect(IPEndPoint endpoint)
		{
            CPacketBufferManager.initialize(2000);
			// CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.
			// 메시지 송,수신은 서버, 클라이언트 모두 동일한 로직으로 처리될 수 있으므로
			// CNetworkService객체를 생성하여 Connector객체에 넘겨준다.
			CNetworkService service = new CNetworkService(true);

			// endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.
			connector = new CConnector(service);
            // 접속 성공시 호출될 콜백 매소드 지정.
            connector.connected_callback += on_connected_gameserver;
            connector.connect(endpoint);			
			System.Threading.Thread.Sleep(10);
		}

		public  void disconnect()
		{
            ((CRemoteServerPeer)game_servers[0]).token.disconnect();
        }

		public void SendPos(Vector3 vpos)
		{
            CPacket msg = CPacket.create((short)PROTOCOL.MOVE_REQ);
            msg.push(vpos.x);
            msg.push(vpos.y);
            msg.push(vpos.z);
            msg.push(3.3f); // r
            game_servers[0].send(msg);
        }

		/// <summary>
		/// 접속 성공시 호출될 콜백 매소드.
		/// </summary>
		/// <param name="server_token"></param>
		void on_connected_gameserver(CUserToken server_token)
		{
			lock (game_servers)
			{
				IPeer server = new CRemoteServerPeer(server_token);
                server_token.on_connected();
				game_servers.Add(server);
				Console.WriteLine("Connected!");
			}
		}
	}
}

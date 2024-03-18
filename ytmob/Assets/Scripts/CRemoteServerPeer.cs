using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;
using Protocol;

namespace CSampleClient
{
	//using GameServer;

	class CRemoteServerPeer : IPeer
	{
		public CUserToken token { get; private set; }

		public CRemoteServerPeer(CUserToken token)
		{
			this.token = token;
			this.token.set_peer(this);
		}

        int recv_count = 0;
		void IPeer.on_message(CPacket msg)
		{
            System.Threading.Interlocked.Increment(ref this.recv_count);

            //PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();
            //switch (protocol_id)
            //{
            //	case PROTOCOL.CHAT_MSG_ACK:
            //		{
            //			string text = msg.pop_string();
            //			Console.WriteLine(string.Format("받 text {0}", text));
            //		}
            //                 break;
            //             case PROTOCOL.USER_INFO:
            //                 {
            //                     short userid = msg.pop_int16();
            //                     Console.WriteLine($"my id {userid} ");
            //                     MouseMove.inst.SetMyID(userid);
            //                 }
            //                 break;
            //             case PROTOCOL.MOVE_CAST:
            //                 {
            //                     short userid = msg.pop_int16();
            //                     float x = msg.pop_float();
            //                     float y = msg.pop_float();
            //                     float z = msg.pop_float();
            //                     float r = msg.pop_float();
            //                     Console.WriteLine($"move {userid} {x} {y} {z} {r}");

            //			var d = new MoveData();
            //			d.x = x;
            //			d.y = y;
            //			d.z = z;
            //			d.r = r;
            //			d.userid = userid;
            //			MouseMove.inst.Cast(d);
            //		}
            //                 break;
            //}


            switch (msg.pop_protocol_id().ToProtocol())
            {
                case EPacketProtocol.USER_INFO:
                    {
                        SCUserInfo info = new SCUserInfo(msg);
                        Console.WriteLine($"my id {info.UserID} ");
                        MouseMove.inst.SetMyID(info.UserID);
                    }
                    break;
                case EPacketProtocol.MOVE_CAST:
                    {
                        SCMoveCast move = new SCMoveCast(msg);
                        Console.WriteLine(move.ToString());

                        var d = new MoveData();
                        d.x = move.X;
                        d.y = move.Y;
                        d.z = move.Z;
                        d.r = move.Rotation;
                        d.userid = move.UserID;
                        MouseMove.inst.Cast(d);
                    }
                    break;
            }



        }

        void IPeer.on_removed()
		{
			Console.WriteLine("Server removed.");
            Console.WriteLine("recv count " + this.recv_count);
        }

		void IPeer.send(CPacket msg)
		{
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
		}

		void IPeer.disconnect()
		{
            this.token.disconnect();
		}
	}
}

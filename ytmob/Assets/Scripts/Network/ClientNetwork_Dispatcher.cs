using B3.CSCommon;
using B3.Protocol;
using Bass.Net;
using System;

public partial class ClientNetwork
{
    private bool _DispatchPacket(Packet packet)
    {
        if (null == packet)
            return false;

        EPacketProtocol protocol = (EPacketProtocol)packet.Protocol;

        switch (protocol)
        {
            case EPacketProtocol.SC_UserInfoNotify: return _OnSCUserInfoNotify(packet.GetData<SCUserInfoNotify>());
            case EPacketProtocol.SC_MoveRes: return _OnSCMoveRes(packet.GetData<SCMoveRes>());
            case EPacketProtocol.SC_LeaveNotify: return _OnSCLeaveNotify(packet.GetData<SCLeaveNotify>());


            default:
                {
#if DEBUG
                    Console.WriteLine($"No Protocol Dispatcher ({protocol} / {packet.Protocol})");
#endif
                    return false;
                }
        }
    }

    private bool _OnSCUserInfoNotify(SCUserInfoNotify msg)
    {
#if DEBUG
        Console.WriteLine($"my id {msg.MyUserID} ");
#endif
        MouseMove.inst.SetMyID(msg.MyUserID);
        return true;
    }

    private bool _OnSCMoveRes(SCMoveRes msg)
    {
        var d = new MoveData();
        d.x = msg.X;
        d.y = msg.Y;
        d.z = msg.Z;
        d.r = msg.Rotation;
        d.userid = msg.UserID;

        MouseMove.inst.Cast(d);
        return true;
    }


    private bool _OnSCLeaveNotify(SCLeaveNotify msg)
    {
        MouseMove.inst.LeaveUser(msg.UserID);
        return false;
    }



}
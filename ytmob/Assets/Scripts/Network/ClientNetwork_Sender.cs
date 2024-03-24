using B3.Network;
using UnityEngine;

public partial class ClientNetwork
{
    public bool SendPos(Vector3 pos)
    {
        if (!B3Network.Instance.IsConnected)
            return false;

        B3Network.Instance.SendMoveReq(pos.x, pos.y, pos.z, 3.3f);
        return true;
    }
}
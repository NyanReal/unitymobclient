using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public enum PROTOCOL : short
	{
		BEGIN = 0,

		CHAT_MSG_REQ = 1,
		CHAT_MSG_ACK = 2,
        MOVE_REQ = 3,
        MOVE_CAST = 4,
		USER_INFO = 5,

        END
	}
}

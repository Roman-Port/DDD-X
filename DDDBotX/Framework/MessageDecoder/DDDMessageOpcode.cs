using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder
{
    public enum DDDMessageOpcode
    {
        ModAuthRequest = 0,
        PlayerConnectRequest = 1,
        PlayerDisconnectRequest = 2,
        PlayerKilledEvent = 3,
        PlayerSwitchedTeamsEvent = 4,
        PlayerSwitchedClassEvent = 5,
        PlayerExecCommandAction = 6,
        PlayerChangeNameEvent = 7
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerDisconnectRequestPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            player_guid = ReadIntString(data, 1);
        }

        public int player_guid;
    }
}

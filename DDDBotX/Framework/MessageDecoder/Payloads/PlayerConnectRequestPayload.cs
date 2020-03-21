using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerConnectRequestPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            steam_id = ReadString(data, 1, 22);
            name = ReadString(data, 23, 32);
            player_guid = ReadIntString(data, 55);
        }

        public string steam_id;
        public string name;
        public int player_guid;
    }
}
 
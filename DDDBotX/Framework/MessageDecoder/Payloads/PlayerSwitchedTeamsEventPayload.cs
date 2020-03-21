using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerSwitchedTeamsEventPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            player_guid = ReadIntString(data, 1);
            team = data[13];
            old_team = data[14];
        }

        public int player_guid;
        public byte team;
        public byte old_team;
    }
}

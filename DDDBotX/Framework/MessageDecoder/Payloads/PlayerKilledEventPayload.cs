using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerKilledEventPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            killed_guid = ReadIntString(data, 1);
            attacker_guid = ReadIntString(data, 13);
            killed_kills = ReadIntString(data, 25);
            killed_deaths = ReadIntString(data, 37);
            attacker_kills = ReadIntString(data, 49);
            attacker_deaths = ReadIntString(data, 61);
            attacker_weapon = ReadString(data, 73, 32);
        }

        public int killed_guid;
        public int attacker_guid;

        public int killed_kills;
        public int killed_deaths;

        public int attacker_kills;
        public int attacker_deaths;

        public string attacker_weapon;
    }
}

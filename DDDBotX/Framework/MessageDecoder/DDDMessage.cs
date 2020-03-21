using DDDBotX.Framework.MessageDecoder.Payloads;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder
{
    public abstract class DDDMessage
    {
        public DDDMessageOpcode opcode;

        public abstract void Decode(byte[] data, DDDConnection conn);

        internal string ReadString(byte[] data, int index, int length)
        {
            //Determine the real length (since this function does not automatically determine null-termination
            int realLength = 0;
            while (realLength < length && data[realLength + index] != 0)
                realLength++;

            //Read
            return Encoding.UTF8.GetString(data, index, realLength);
        }

        internal int ReadIntString(byte[] data, int index)
        {
            //Consumes 12 bytes
            string s = ReadString(data, index, 12);
            return int.Parse(s);
        }

        public static DDDMessage DecodeBytes(byte[] data, DDDConnection conn)
        {
            //Get the opcode
            DDDMessageOpcode op = (DDDMessageOpcode)data[0];

            //Create the desired message
            DDDMessage msg;
            switch(op)
            {
                case DDDMessageOpcode.ModAuthRequest: msg = new ModAuthRequestPayload(); break;
                case DDDMessageOpcode.PlayerConnectRequest: msg = new PlayerConnectRequestPayload(); break;
                case DDDMessageOpcode.PlayerDisconnectRequest: msg = new PlayerDisconnectRequestPayload(); break;
                case DDDMessageOpcode.PlayerKilledEvent: msg = new PlayerKilledEventPayload(); break;
                case DDDMessageOpcode.PlayerSwitchedTeamsEvent: msg = new PlayerSwitchedTeamsEventPayload(); break;
                case DDDMessageOpcode.PlayerSwitchedClassEvent: msg = new PlayerSwitchedClassEventPayload(); break;
                case DDDMessageOpcode.PlayerExecCommandAction: msg = new PlayerExecCommandActionPayload(); break;
                case DDDMessageOpcode.PlayerChangeNameEvent: msg = new PlayerChangeNamePayload(); break;
                default: return null;
            }

            //Set info
            msg.opcode = op;
            msg.Decode(data, conn);

            return msg;
        }
    }
}

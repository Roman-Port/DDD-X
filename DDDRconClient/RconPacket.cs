using System;
using System.Collections.Generic;
using System.Text;

namespace DDDRconClient
{
    class RconPacket
    {
        public byte[] payload;
        public RconType type;
        public int id;
    }
}

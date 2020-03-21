using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RomanPort.SourceLogLib
{
    class SourceLogSqlClient
    {
        public Socket sock;
        public byte[] buffer;
        public bool isAuth;
    }
}

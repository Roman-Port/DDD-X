using System;
using System.Collections.Generic;
using System.Text;

namespace DDDRconClient
{
    enum RconType
    {
        SERVERDATA_AUTH = 3,
        SERVERDATA_EXECCOMMAND = 2, //or SERVERDATA_AUTH_RESPONSE
        SERVERDATA_RESPONSE_VALUE = 0
    }
}

#include <sourcemod>
#include <sdktools>
 
public Plugin myinfo =
{
	name = "Dino D-Day X Bot",
	author = "RomanPort",
	description = "DDD-X Discord Bot",
	version = "1.0.0.0",
	url = "https://romanport.com/"
}

public Database hDatabase;
 
public void OnPluginStart()
{
    HookEvent("player_death", Event_PlayerScoreChanged, EventHookMode_Post);
    HookEvent("player_team", Event_PlayerSwitchedTeams, EventHookMode_Post);
    HookEvent("player_class", Event_PlayerSwitchedClass, EventHookMode_Post);
    HookEvent("player_changename", Event_PlayerChangedName, EventHookMode_Post);
    ConnectToSQL();
}

/* CMD framework */

void CopyStringToBuffer(char[] buffer, char[] str, int index, int len) {
    //Writes a string to the buffer
    for(int i = 0; i<len; i+=1) {
        buffer[index + i] = str[i];
    }
}

void CopyConstStringToBuffer(char[] buffer, const char[] str, int index, int len) {
    //Writes a string to the buffer
    for(int i = 0; i<len; i+=1) {
        buffer[index + i] = str[i];
    }
}

void WriteClientIdToBuffer(char[] buffer, int index, int clientIndex) {
    //Consumes 22 bytes of space
    char[] sbuf = new char[22];
    GetClientAuthId(clientIndex, AuthId_SteamID64, sbuf, 22, true);
    CopyStringToBuffer(buffer, sbuf, index, 22);
}

void WriteShortToBuffer(char[] buffer, int index, int value) {
    //Consumes 12 bytes. Gross because of our odd limitation of 255 values for a byte, not 256
    char[] sbuf = new char[12];
    IntToString(value, sbuf, 12);
    CopyStringToBuffer(buffer, sbuf, index, 12);
}

/* SQL PART */

public void ConnectToSQL() {
    PrintToServer("Connecting to DB...");
    Database.Connect(GotDatabase);
}

public void GotDatabase(Database db, const char[] error, any data)
{
    if (db == null)
	{
		//Error!
        PrintToServer("DB Connect Error!");
	} 
    else 
    {
		hDatabase = db;
        PrintToServer("DB Connected!");
        SendAuthInfo();
	}
}

public void GotResponse(Database db, DBResultSet results, const char[] error, any data)
{
	if(results == INVALID_HANDLE && hDatabase != null) {
        PrintToServer("DB was disconnected!");
        PrintToServer(error);
        hDatabase = null;
    }
}

void ExecuteRemoteCommand(char[] cmd, int len)
{
	//Check if we have a DB
    if(hDatabase == null) {
        return;
    }
    
    //Since we have null-termination problems, we are going to increase EVERY byte in the char array by one. This will remove all null bytes
    for(int i = 0; i<len; i+=1) {
        cmd[i] = cmd[i] + 1;
    }
    
    //Send
    hDatabase.Query(GotResponse, cmd, 0);
}

/* Message senders */

void SendAuthInfo() {
    //Get server name
    char[] name = new char[32];
    GetConVarString(FindConVar("hostname"), name, 32); 

    //Get map name
    char[] mapName = new char[32];
    GetCurrentMap(mapName, 32);

    //Create buffer
    char[] buffer = new char[67];
    buffer[0] = 0;
    buffer[1] = 1; //Version major
    buffer[2] = 2; //Version minor
    CopyStringToBuffer(buffer, name, 3, 32);
    CopyStringToBuffer(buffer, mapName, 35, 32);
    
    //Send
    ExecuteRemoteCommand(buffer, 67);
}

/* Events */

public void OnClientAuthorized(int client, const char[] auth) {
    //Get client name
    char[] name = new char[32];
    GetClientName(client, name, 32);

    //Create buffer
    char[] buffer = new char[67];
    buffer[0] = 1;
    WriteClientIdToBuffer(buffer, 1, client); //23
    CopyStringToBuffer(buffer, name, 23, 32); //55
    WriteShortToBuffer(buffer, 55, GetClientUserId(client)); //67

    //Send
    ExecuteRemoteCommand(buffer, 67);
}

public void OnClientDisconnect(int client) {
    //Create buffer
    char[] buffer = new char[13];
    buffer[0] = 2;
    WriteShortToBuffer(buffer, 1, GetClientUserId(client));

    //Send
    ExecuteRemoteCommand(buffer, 13);
}

public void Event_PlayerScoreChanged(Event event, const char[] name, bool dontBroadcast)
{
    //Get clients
    int killed_guid = event.GetInt("userid");
    int attacker_guid = event.GetInt("attacker");

    //Get client IDs
    int killed = GetClientOfUserId(killed_guid);
    int attacker = 0;
    if(attacker_guid != 0) {
        attacker = GetClientOfUserId(attacker_guid);
    }

    //Get attacker weapon
    char[] weapon = new char[32];
    if(attacker != 0) {
        GetClientWeapon(attacker, weapon, 32);
    }

    //Get kills/deaths of both
    int killed_kills = GetClientFrags(killed);
    int killed_deaths = GetClientDeaths(killed);
    int attacker_kills = 0;
    int attacker_deaths = 0;
    if(attacker != 0) {
        attacker_kills = GetClientFrags(attacker);
        attacker_deaths = GetClientDeaths(attacker);
    }

    //Create output
    char[] buffer = new char[105];
    buffer[0] = 3;
    WriteShortToBuffer(buffer, 1, killed_guid); //13
    WriteShortToBuffer(buffer, 13, attacker_guid); //25
    WriteShortToBuffer(buffer, 25, killed_kills); //37
    WriteShortToBuffer(buffer, 37, killed_deaths); //49
    WriteShortToBuffer(buffer, 49, attacker_kills); //61
    WriteShortToBuffer(buffer, 61, attacker_deaths); //73
    CopyStringToBuffer(buffer, weapon, 73, 32); //105

    //Send
    ExecuteRemoteCommand(buffer, 105);
}

public void Event_PlayerSwitchedTeams(Event event, const char[] name, bool dontBroadcast) {
    //Create output
    char[] buffer = new char[15];
    buffer[0] = 4;
    WriteShortToBuffer(buffer, 1, event.GetInt("userid")); //13
    buffer[13] = event.GetInt("team");
    buffer[14] = event.GetInt("oldteam");

    //Send
    ExecuteRemoteCommand(buffer, 15);
}

public void Event_PlayerSwitchedClass(Event event, const char[] name, bool dontBroadcast) {
    //Get char
    char[] classname = new char[32];
    event.GetString("class", classname, 32, "INVALID_CLASS");

    //Create output
    char[] buffer = new char[45];
    buffer[0] = 5;
    WriteShortToBuffer(buffer, 1, event.GetInt("userid")); //13
    CopyStringToBuffer(buffer, classname, 13, 32);

    //Send
    ExecuteRemoteCommand(buffer, 45);
}

public Action OnClientSayCommand(int client, const char[] command, const char[] sArgs) {
    //Create output
    char[] buffer = new char[1293];
    buffer[0] = 6;
    WriteShortToBuffer(buffer, 1, GetClientUserId(client));
    CopyConstStringToBuffer(buffer, command, 13, 256);
    CopyConstStringToBuffer(buffer, sArgs, 269, 1024);

    //Send
    ExecuteRemoteCommand(buffer, 1293);

    return Plugin_Continue;
}

public void OnMapStart() {
    SendAuthInfo();
    if(hDatabase == null) {
        PrintToServer("Map switched; Was not connected. Reconnecting...");
        ConnectToSQL();
    }
}

public void Event_PlayerChangedName(Event event, const char[] name, bool dontBroadcast) {
    //Get old name
    char[] oldName = new char[32];
    event.GetString("oldname", oldName, 32, "INVALID_NAME");

    //Get new name
    char[] newName = new char[32];
    event.GetString("newname", newName, 32, "INVALID_NAME");

    //Create output
    char[] buffer = new char[77];
    buffer[0] = 7;
    WriteShortToBuffer(buffer, 1, event.GetInt("userid")); //13
    CopyStringToBuffer(buffer, oldName, 13, 32);
    CopyStringToBuffer(buffer, newName, 45, 32);

    //Send
    ExecuteRemoteCommand(buffer, 77);
}
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RomanPort.SourceLogLib
{
    public delegate void OnReceiveMessage(DDDStringReader reader, ref int state);

    public class SourceLogConnection
    {
        public IPAddress addr;
        public int port;
        public event OnReceiveMessage handler;

        public TcpListener listener;

        public const int RECEIVE_BUFFER_SIZE = 65536;

        public int state; //A user-defined object that a user can change
        
        public SourceLogConnection(IPAddress addr, int port)
        {
            this.addr = addr;
            this.port = port;

            listener = new TcpListener(addr, port);
        }

        public void StartListen()
        {
            listener.Start();
            listener.BeginAcceptSocket(OnAcceptSocket, null);
        }

        public void Log(string topic, string msg)
        {
            //return;
            Console.WriteLine($"[{DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToShortTimeString()}] [SourceLogConnection -> {topic}] {msg}");
        }

        private void OnAcceptSocket(IAsyncResult ar)
        {
            //Get the socket
            Socket sock = listener.EndAcceptSocket(ar);
            Log("SocketStatusChanged", "(Connection Created)");

            //Wait for events from this socket
            byte[] buf = new byte[RECEIVE_BUFFER_SIZE];
            SourceLogSqlClient data = new SourceLogSqlClient
            {
                isAuth = false,
                sock = sock,
                buffer = buf
            };
            sock.BeginReceive(buf, 0, RECEIVE_BUFFER_SIZE, SocketFlags.None, OnSocketReceive, data);

            //We're lazy, so we don't actually do a real protocol. This is just a fake message to keep the client happy. This is directly from my Wireshark capture of a *real* server.
            sock.Send(Convert.FromBase64String("WwAAAAo1LjcuMjYtMHVidW50dTAuMTYuMDQuMQAQAAAAMHISdE4+C2sA//cIAgD/gRUAAAAAAAAAAAAAKzpKH1IXUyFCI3xOAG15c3FsX25hdGl2ZV9wYXNzd29yZAA="));

            //Begin listening again
            listener.BeginAcceptSocket(OnAcceptSocket, null);
        }

        private void OnSocketReceive(IAsyncResult ar)
        {
            //Grab the socket
            SourceLogSqlClient state = (SourceLogSqlClient)ar.AsyncState;
            Socket sock = state.sock;

            try
            {
                int length = sock.EndReceive(ar);

                //Close and abort if we lost connection
                if (!sock.Connected || length == 0)
                {
                    Log("SocketStatusChanged", "(Connection Lost)");
                    sock.Close();
                    return;
                }

                //Handle
                if (!state.isAuth)
                {
                    //Login request. Just assume it was OK.
                    Log("OnLoginRequest", "Got login request.");
                    sock.Send(Convert.FromBase64String("BwAAAgAAAAIAAAA="));
                    state.isAuth = true;
                }
                else
                {
                    //This is a command. Get the opcode
                    byte opcode = state.buffer[4];
                    if (opcode == 3)
                    {
                        //Query message

                        //Read query string
                        byte[] data = new byte[length - 5];
                        Array.Copy(state.buffer, 5, data, 0, length - 5);
                        string sdata = Encoding.UTF8.GetString(data);

                        //Write OK to keep the game running
                        sock.Send(Convert.FromBase64String("BwAAAQAAAAIAAAA="));

                        //Check if this is special
                        if (sdata.StartsWith("SET"))
                        {
                            //Do nothing.
                        }
                        else
                        {
                            try
                            {
                                handler(new DDDStringReader(data), ref this.state);
                            } catch (Exception ex)
                            {
                                Console.WriteLine("EXCEPTION HIT when processing server command: " + ex.Message + ex.StackTrace);
                            }
                        }
                    }
                    else if (opcode == 1)
                    {
                        //Quit message
                        Log("SocketStatusChanged", "Server shut down connection.");
                        sock.Close();
                        return;
                    }
                    else
                    {
                        //Unknown
                        Log("SocketStatusChanged", "Warning: Unknown command type " + state.buffer[4].ToString() + " sent. Shutting down connection...");
                        sock.Close();
                        return;
                    }
                }

                //Begin listening again
                sock.BeginReceive(state.buffer, 0, RECEIVE_BUFFER_SIZE, SocketFlags.None, OnSocketReceive, state);
            } catch (Exception ex)
            {
                Log("SocketError", "Shutting down socket because of error -> " + ex.Message + ex.StackTrace);
                try
                {
                    sock.Close();
                }
                catch { }
            }
        }
    }
}

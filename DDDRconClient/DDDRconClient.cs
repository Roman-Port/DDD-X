using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DDDRconClient
{
    public class RconClient
    {
        /// <summary>
        /// Size of the incoming buffer
        /// </summary>
        public const int RECEIVE_BUFFER_SIZE = 4096;
        
        /// <summary>
        /// Socket that actually holds the connection
        /// </summary>
        public Socket sock;

        /// <summary>
        /// Object to lock during long operations
        /// </summary>
        public bool locked;

        public async Task<bool> ConnectAsync(IPEndPoint endpoint)
        {
            //Create a new socket
            sock = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Establish a connection
                await sock.ConnectAsync(endpoint);
            } catch
            {
                return false;
            }

            //If we failed to connect, stop here
            if (!sock.Connected)
                return false;

            return true;
        }

        public async Task<bool> AuthenticateAsync(string password)
        {
            //Wait for unlock
            await WaitForUnlock();
            locked = true;

            try
            {
                //Convert the password to bytes
                byte[] payload = Encoding.ASCII.GetBytes(password);

                //Send this and get a response
                await PrivateSendPacket(new RconPacket
                {
                    type = RconType.SERVERDATA_AUTH,
                    payload = payload,
                    id = 3
                });

                //Read packets until we get an auth response
                RconPacket decoded;
                while (true)
                {
                    //Wait for data to download
                    byte[] buffer = new byte[RECEIVE_BUFFER_SIZE];
                    int length = await sock.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                    //Decode
                    decoded = DecodePacketFromBuffer(buffer);

                    //Decide
                    if (decoded.type == RconType.SERVERDATA_EXECCOMMAND)
                        break;
                }

                //Return status
                return decoded.id != -1;
            } finally
            {
                locked = false;
            }
        }

        public async Task<string> ExecuteCommandAsync(string cmd)
        {
            //Convert the password to bytes
            byte[] payload = Encoding.UTF8.GetBytes(cmd);

            //Send this and get the response
            RconPacket response = await PrivateSendPacketGetResponseAsync(RconType.SERVERDATA_EXECCOMMAND, payload);

            //Now, return the payload decoded
            string message = Encoding.UTF8.GetString(response.payload);
            return message;
        }

        /// <summary>
        /// THIS IS NOT 100% A SAFE REPLACEMENT, AFAIK. It should only be used to prevent accidents. DO NOT USE THIS ON A PUBLIC FACING COMMAND
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EscapeString(string input)
        {
            return input.Replace(';', ':');
        }

        private async Task WaitForUnlock()
        {
            while (locked)
                await Task.Delay(100);
        }

        private async Task<RconPacket> PrivateSendPacketGetResponseAsync(RconType type, byte[] payload)
        {
            //Wait for unlock
            await WaitForUnlock();
            locked = true;

            try
            {
                //Send the request with ID 1
                await PrivateSendPacket(new RconPacket
                {
                    type = type,
                    payload = payload,
                    id = 1
                });

                //Now, read packets until we get a packet with ID 2, our end packet
                List<RconPacket> packets = new List<RconPacket>();
                bool hasSentTest = false;
                while (true)
                {
                    //Wait for data to download
                    byte[] buffer = new byte[RECEIVE_BUFFER_SIZE];
                    int length = await sock.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                    //Decode
                    RconPacket decoded = DecodePacketFromBuffer(buffer);

                    //Depending on the ID returned, deal
                    if (decoded.id == 1)
                        packets.Add(decoded); //Reply
                    else if (decoded.id == 2)
                        break; //This is our end of message packet
                    else
                        throw new Exception("Unexpected packet returned with ID " + decoded.id); //Should never happen

                    //Now, write the test packet if we can
                    break;
                    //DDD seems very unhappy with this. We'll come back to it later
                    if(!hasSentTest)
                    {
                        await PrivateSendPacket(new RconPacket
                        {
                            id = 2,
                            type = RconType.SERVERDATA_RESPONSE_VALUE,
                            payload = new byte[0]
                        });
                        hasSentTest = true;
                    }
                }

                //We should never get zero incoming packets. If that happened, die
                if (packets.Count == 0)
                    throw new Exception("Recieved stop message, but never got payload.");

                //If there were only one packet, just return that
                if (packets.Count == 1)
                    return packets[0];

                //We'll have to combine the buffers of all incoming packets. Total the size and create a buffer
                int endBufferSize = 0;
                foreach (var p in packets)
                    endBufferSize += p.payload.Length;

                //Now, copy all
                int pos = 0;
                byte[] endPayload = new byte[endBufferSize];
                foreach (var p in packets)
                {
                    Array.Copy(p.payload, 0, endPayload, pos, p.payload.Length);
                    pos += p.payload.Length;
                }

                //Now, return a packet with the Id and type of the first packet, but the body of all of the packets combined
                return new RconPacket
                {
                    payload = endPayload,
                    id = packets[0].id,
                    type = packets[0].type
                };
            } finally
            {
                //Unlock
                locked = false;
            }
        }

        private RconPacket DecodePacketFromBuffer(byte[] buffer)
        {
            //Read the packet data
            int sourceLength = ByteEncoder.ReadInt32(buffer, 0);
            int sourceId = ByteEncoder.ReadInt32(buffer, 4);
            int sourceType = ByteEncoder.ReadInt32(buffer, 8);

            //Now, read the content using the source length. Do not read the null terminator or the last null byte.
            byte[] payload = new byte[sourceLength - 2 - 8];
            Array.Copy(buffer, 12, payload, 0, payload.Length);

            //Convert to a RconPacket
            return new RconPacket
            {
                id = sourceId,
                payload = payload,
                type = (RconType)sourceType
            };
        }

        private async Task PrivateSendPacket(RconPacket packet)
        {
            //Create the buffer to send
            byte[] buf = new byte[packet.payload.Length + 4 + 4 + 4 + 2];

            //Write header data
            ByteEncoder.WriteInt32(buf, 0, buf.Length - 4);
            ByteEncoder.WriteInt32(buf, 4, packet.id);
            ByteEncoder.WriteInt32(buf, 8, (int)packet.type);

            //Write content
            Array.Copy(packet.payload, 0, buf, 12, packet.payload.Length);

            //Send to network
            await sock.SendAsync(new ArraySegment<byte>(buf), SocketFlags.None);
        }
    }
}

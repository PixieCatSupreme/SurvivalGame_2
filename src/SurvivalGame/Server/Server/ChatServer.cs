using Lidgren.Network;
using Mentula.Utilities.Udp;
using System.Collections.Generic;
using ID = System.Collections.Generic.KeyValuePair<string, long>;
using MESSAGE = System.Collections.Generic.KeyValuePair<System.Collections.Generic.KeyValuePair<string, long>, string>;

namespace Mentula.Server
{
    public sealed class ChatServer
    {
        private GameLogic gl;
        private Queue<MESSAGE> newMessages;

        public ChatServer(GameLogic logic)
        {
            gl = logic;
            newMessages = new Queue<MESSAGE>();
        }

        public void AddMessage(string msg)
        {
            if (msg[msg.Length - 1] != '\n') msg += '\n';
            newMessages.Enqueue(new MESSAGE(new ID("Global", 0), msg));
        }

        public void HandleMsg(NetIncomingMessage msg)
        {
            long id = msg.SenderConnection != null ? msg.SenderConnection.RemoteUniqueIdentifier : -1;
            string zone = msg.ReadString();
            string text = msg.ReadString();

            newMessages.Enqueue(new MESSAGE(new ID(zone, id), text));
        }

        public void Tick(NetServer server)
        {
            while (newMessages.Count > 0)
            {
                MESSAGE message = newMessages.Dequeue();
                string name = message.Key.Value != 0 ? gl.GetPlayer(message.Key.Value).Name : "Server";

                for (int i = 0; i < server.Connections.Count; i++)
                {
                    NetConnection conn = server.Connections[i];
                    if (conn.RemoteUniqueIdentifier != message.Key.Value)
                    {
                        NetOutgoingMessage nom = server.CreateMessage();
                        nom.Write((byte)NDT.Chat);
                        nom.Write(message.Key.Key);
                        nom.Write(name);
                        nom.Write(message.Value);
                        server.SendMessage(nom, conn, NetDeliveryMethod.Unreliable);
                    }
                }
            }
        }
    }
}
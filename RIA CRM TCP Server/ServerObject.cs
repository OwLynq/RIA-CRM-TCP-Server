using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace RIA_CRM_TCP_Server
{
    public class ServerObject
    {
        static TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>(); 

        protected internal void AddConnection(ClientObject clientObject) 
        {
            clients.Add(clientObject);
            Console.WriteLine("Пользователь {0} подключился к серверу", clientObject.ID1);
        }
        protected internal void RemoveConnection(string id, ClientObject clientObject)
        {
            
            ClientObject client = clients.FirstOrDefault(c => c.ID1 == id);
            if (client != null)
                clients.Remove(client);
            Console.WriteLine("Пользователь {0} отключился от сервера", clientObject.ID1);
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true) 
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        protected internal void BroadcastMessage(string message, string ID2) 
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (Convert.ToInt64(clients[i].ID1) == Convert.ToInt64(ID2))
                {
                    clients[i].Stream.Write(data, 0, data.Length); 
                }
            }
        }
        protected internal void Disconnect() 
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
    }
}

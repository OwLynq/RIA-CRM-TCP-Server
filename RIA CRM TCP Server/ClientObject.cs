using System;
using System.Net.Sockets;
using System.Text;

namespace RIA_CRM_TCP_Server
{
    public class ClientObject 
    {
        protected internal string ID1 { get; private set; } 
        protected internal string ID2 { get; private set; } 
        protected internal NetworkStream Stream { get; private set; } 
        TcpClient client; 
        ServerObject server; 

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            Stream = client.GetStream();

            string[] message = Convert.ToString(GetMessage()).Split('/');
            ID1 = message[0];
            ID2 = message[1];

            serverObject.AddConnection(this);
        }
        public void Process() 
        {
            try
            {
                Console.WriteLine("Пользователь {0} выбрал чат", this.ID1);
                string message; 
                while (true) 
                {
                    try
                    {
                        message = GetMessage(); 
                        server.BroadcastMessage(message, this.ID2); 
                        message = String.Format("Пользователь {0} написал: {1}", this.ID1, message);
                        Console.WriteLine(message);
                    }
                    catch
                    {
                        message = String.Format("Пользователь {0} покинул чат", this.ID1);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.ID1, this);
                Close();
            }
        }

        private string GetMessage() 
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            if (builder.Length != 0)
                return builder.ToString();
            else
                return null;
        }
        protected internal void Close() 
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}

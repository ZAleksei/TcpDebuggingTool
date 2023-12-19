using System.Net.Sockets;

namespace TcpDebuggingTool
{
    public class ClientTCP 
    {
        public LogMesDelegate? LMDelegate;
        protected int SId;
        protected string SendIp;
        protected int SendPort;
        public void Log( string TextInfo)
        {
            LMDelegate?.Invoke(SId, "@"+TextInfo);
        }
        public ClientTCP(int SId, string SendIp, int SendPort, LogMesDelegate lmd)
        {
            this.SId = SId;
            LMDelegate = lmd;
            this.SendIp = SendIp;
            this.SendPort = SendPort;
            Log( $"Подключение к={SendIp}:{SendPort}");
        }

        protected TcpClient tcpClient = new TcpClient();
        public bool Connect()
        {
         
            try
            {
                tcpClient.Connect(SendIp, SendPort);
            }
            catch (SocketException ex)
            {
                return false;
            }
            return true;
        }
        public void Close()
        {
            tcpClient?.Close();
            Log($"Отключен");
        }
        public NetworkStream GetStream()
        {
           return tcpClient.GetStream();
        }

    }

}
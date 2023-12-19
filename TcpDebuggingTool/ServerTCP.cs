using System;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

namespace TcpDebuggingTool
{
    public delegate void LogMesDelegate(int Session,string TextInfo);
    public class ServerTCP
    {
        public int sesionId = 0;
        public LogMesDelegate? LMDelegate;
        protected TcpListener tcpListener;
        protected string SendIp;
        protected int SendPort;
        public ServerTCP(int port, string SendIp, int SendPort, LogMesDelegate lmd)
        {
            LMDelegate = lmd;
            this.SendIp = SendIp;
            this.SendPort = SendPort;
            tcpListener = new TcpListener(IPAddress.Any, port);
            Log(0, $"Listern Port={port}");
      
        }
        private void Log(int Session, string TextInfo)
        {
            LMDelegate?.Invoke(Session, TextInfo);
        }
        public void Start()
        {
            Task.Run(Run);
        }
       async  protected void Run()
        {
            tcpListener.Start();    // запускаем сервер
            Log(0,"Мониторинг запущен. Ожидание подключений... ");
            while (true)
            {
                // получаем подключение в виде TcpClient
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
               
                // создаем новую задачу для обслуживания нового клиента
                Task.Run(async () => await ProcessClientAsync(tcpClient));

                // вместо задач можно использовать стандартный Thread
                // new Thread(async ()=>await ProcessClientAsync(tcpClient)).Start();
            }
        }

        async Task ProcessClientAsync(TcpClient tcpClient)
        {
            

            var cSesionId = Interlocked.Increment(ref sesionId);
           // Log(cSesionId, "Подключен клиент... ");
            ClientTCP cTCP = new ClientTCP(cSesionId, SendIp, SendPort, Log);

            if (cTCP.Connect())
            {
                byte[] bufer = new byte[2000];
                var streamServer = tcpClient.GetStream();
                var streamClient = cTCP.GetStream();
                Boolean servflag = true;
                Boolean clientflag = true;
                try
                {
                    while (true)
                    {
                        servflag = true;
                        clientflag = false;
                        if (streamServer.DataAvailable)
                        {
                            int len = streamServer.Read(bufer, 0, 2000);
                            if (len > 0)
                            {
                                servflag = false;
                                clientflag = true;
                                streamClient.Write(bufer, 0, len);
                                Log(cSesionId, $"==> исходящее {FunctionMain.BuferToString(bufer, len)}");
                            }
                        }
                        servflag = false;
                        clientflag = true;
                        if (streamClient.DataAvailable)
                        {
                            int len = streamClient.Read(bufer, 0, 2000);
                            if (len > 0)
                            {
                                servflag = true;
                                clientflag = false;
                                streamServer.Write(bufer, 0, len);
                                Log(cSesionId, $"<== входящее {FunctionMain.BuferToString(bufer, len)}");
                            }
                        }
                        servflag = false;
                        clientflag = false;
                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    if(servflag)
                    {
                        Log(cSesionId, $"Разрыв входящего соединения ");
                    }
                    if (clientflag)
                    {
                        Log(cSesionId, $"Разрыв исходящего соединения ");
                    }
                    if (!clientflag && !servflag)
                    {
                        Log(cSesionId, $"Разрыв соединения ");
                    }


                    cTCP.Close();
                   
                }
            }
            else
            {
                cTCP.Close();
                

                // Log(cSesionId, "Отключен клиент... ");
                return;
            }

         
            tcpClient.Close();
        }


    }

}
using System.IO;
using System.Text;
using TcpDebuggingTool.Properties;

namespace TcpDebuggingTool
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {


        }
        public void AddLog(String txt)
        {
            Log(0, txt);
           // this.richTextBox1.AppendText($"{DateTime.Now.ToString("dd HH:mm:ss.fff")} {txt}\n");
        }
        int ТекущийЧас = -1;
        public void Log(int Session, string TextInfo)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new LogMesDelegate(Log), new object[] { Session, TextInfo });
                return;
            }

            richTextBox1.SelectionColor = Color.Black;
            //richTextBox1.SelectionColor = Color.Brown;

            if (TextInfo.IndexOf("==>", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                richTextBox1.SelectionColor = Color.Blue;
            }

    //        int i = TextInfo.IndexOf("Отправлено", StringComparison.CurrentCultureIgnoreCase);
            if (TextInfo.IndexOf("<==", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                richTextBox1.SelectionColor = Color.Brown;
            }

            if(ТекущийЧас!= DateTime.Now.Hour)
            {
                ТекущийЧас = DateTime.Now.Hour;
                this.richTextBox1.AppendText($"{DateTime.Now}  текущее время \n");
            }

            this.richTextBox1.AppendText($"{DateTime.Now.ToString("mm:ss.fff")} [{Session}] {TextInfo}\n");

            if (this.richTextBox1.TextLength > 60000*4)
            {
                this.richTextBox1.SaveFile($"log\\Log_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.rtf");
                this.richTextBox1.Clear();
            }
        }

        public ServerTCP? serv;
        private void Form1_Load(object sender, EventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Settings s = new Settings();

            DirectoryInfo dirInfo = new DirectoryInfo("log");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            AddLog("Старт");
            this.Text = $"Входящий {s.Port} подключение к {s.TunelIP}:{s.TunelPort} [==> от сервера к клиенту; <== от клиента к серверу]";
            serv = new ServerTCP(s.Port, s.TunelIP, s.TunelPort, Log);
            serv.Start();




        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            this.richTextBox1.SaveFile($"log\\LogClose{DateTime.Now.ToString("yyyy_MM_dd_hh_ss_mm")}.rtf");

        }
    }

}
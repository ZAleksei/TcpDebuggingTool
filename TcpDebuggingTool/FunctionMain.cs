using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TcpDebuggingTool
{


    public static class FunctionMain
    {


        public static string BuferToString(Byte[] Буфер)
        {
            return BuferToString(Буфер, Буфер.Length);
        }

        public static string BuferToString(Byte[] Буфер, int count)
        {
            return BuferToString(Буфер, 0, count);
        }

        public static string BuferToString(Byte[] Буфер, int ofset, int count)
        {

            
            Encoding encodingDOS = Encoding.GetEncoding(866);
            string res = (count - ofset).ToString() + " байт \r\n ";

            int Счет16 = 16;
            // Encoding ascii = Encoding.ASCII;
            bool флагЗакрыт = false;
            string Окончание = "";
            string hex = "";
            for (int i = ofset; (i - ofset) < count; i++)
            {

                Счет16 = Счет16 - 1;
                if (Буфер[i] >= 32)
                {
                    // Окончание = Окончание + ascii.GetString(Буфер, i, 1);
                    Окончание += encodingDOS.GetString(Буфер, i, 1);
                    флагЗакрыт = false;
                }
                else
                {
                    Окончание = Окончание + "?";
                    флагЗакрыт = false;
                }

                hex = hex + " " + Буфер[i].ToString("x2");

                if (Счет16 <= 0)
                {
                    res += hex.PadRight(52) + Окончание + "\r\n ";
                    Окончание = "";
                    hex = "";
                    Счет16 = 16;
                    флагЗакрыт = true;
                }
            }
            if (!флагЗакрыт)
            {
                res += hex.PadRight(52) + Окончание + "\r\n ";
            }

            return res;
        }

   





    }

}
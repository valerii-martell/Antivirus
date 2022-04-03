using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antivirus
{
    static class Signatures
    {
        private static Dictionary<string, string> signatures = new Dictionary<string, string>();

        static Signatures()
        {
            signatures.Add("CreateRemoteThread", "Trojan");
            signatures.Add("GetAsyncKeyState", "Trojan");
            signatures.Add("GetForegroundWindow", "Keylogger");
            signatures.Add("GetWindowText", "Keylogger");
            signatures.Add("JOIN", "Trojan");
            signatures.Add("MD5CryptoServiceProvider", "Crypter");
            signatures.Add("NtUnmapViewOfSection", "Trojan");
            signatures.Add("PRIVMSG", "Trojan");
            signatures.Add("RijndaelManaged", "Crypter");
            signatures.Add("SetWindowsHookEx", "Keylogger");
            signatures.Add(@"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*", "Virus"); 
        }

        public static Dictionary<string, string> getSignatures()
        {
            return signatures;
        }

        public static Dictionary<string, string> getOnlineSignatures()
        {
            string URL = "https://antivirus-8c781.firebaseio.com/.json";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.ContentType = "application/json: charset=utf-8";
            WebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responsestream = response.GetResponseStream())
            {
                StreamReader read = new StreamReader(responsestream, Encoding.UTF8);
                string str = read.ReadToEnd();
                str = str.Substring(15, str.Length - 17);
                string[] signaturesJson = Regex.Split(str, ",");
                Parallel.ForEach(signaturesJson, signatureJson =>
                {
                    string[] buf = Regex.Split(signatureJson, ":");
                    try
                    {
                        signatures.Add(Regex.Replace(buf[0], "\"", ""), Regex.Replace(buf[1], "\"", ""));
                    }
                    catch { };
                });
            }
            return signatures;
        }
    }
}

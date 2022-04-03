using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Antivirus
{
    public class FileMD5
    {
        private string mainFilePath;
        private string md5;
        private DateTime dateTime;

        public FileMD5(string mainFilePath)
        {
            this.mainFilePath = mainFilePath;
            using (var md5Checksum = MD5.Create())
            {
                using (var stream = File.OpenRead(mainFilePath))
                {
                    md5 = ToHex(md5Checksum.ComputeHash(stream), true);
                }
            }
            dateTime = DateTime.Now;
        }

        public FileMD5(string mainFilePath, string md5, DateTime dateTime)
        {
            this.mainFilePath = mainFilePath;
            this.md5 = md5;
            this.dateTime = dateTime;
        }

        public FileMD5() { }

        private string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public string MainFilePath
        {
            get
            {
                return mainFilePath;
            }

            set
            {
                mainFilePath = value;
            }
        }

        public string Md5
        {
            get
            {
                return md5;
            }

            set
            {
                md5 = value;
            }
        }

        public DateTime Date
        {
            get
            {
                return dateTime;
            }

            set
            {
                dateTime = value;
            }
        }
    }
}

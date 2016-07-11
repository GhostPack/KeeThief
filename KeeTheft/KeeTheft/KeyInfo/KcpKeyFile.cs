using KeeTheft.KeyInfo;
using System;
using System.Text;

namespace KeeTheft
{
    public class KcpKeyFile : IUserKey
    {
        public KcpKeyFile()
        {
        }

        public IntPtr encryptedBlobAddress { get; set; }
        public byte[] encryptedBlob { get; set; }
        public byte[] plaintextBlob { get; set; }
        public int encryptedBlobLen { get; set; }
        public string keyFilePath { get; set; }
        public string databaseLocation { get; set; }
        
        override public string ToString()
        {
            StringBuilder str = new StringBuilder();
            string Type = this.GetType().Name;

            str.AppendLine(Type + "Database Location:    " + databaseLocation);
            str.AppendLine(Type + "Path:    " + keyFilePath);
            str.AppendLine(Type + "Addr:    0x" + encryptedBlobAddress.ToString("X8"));
            str.Append(Type + "EncBlob: ");
            for (int i = 0; i < encryptedBlobLen; i++)
            {
                str.Append("0x" + encryptedBlob[i].ToString("X2"));
                if (i != encryptedBlobLen - 1)
                    str.Append(",");
            }
            str.AppendLine();

            if (plaintextBlob == null)
                str.AppendLine(Type + "Plain:   null");
            else
                str.AppendLine(Type + "Plain:   " + Convert.ToBase64String(plaintextBlob));

            return str.ToString();
        }
    }
}

using System;

namespace KeeTheft.KeyInfo
{
    public interface IUserKey
    {
        IntPtr encryptedBlobAddress { get; set; }
        byte[] encryptedBlob { get; set; }
        byte[] plaintextBlob { get; set; }
        int encryptedBlobLen { get; set; }
    }
}

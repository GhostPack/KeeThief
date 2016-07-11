/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2016 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#if PocketPC || Smartphone || WindowsCE
#undef KDB_ANSI
#else
// If compiling for the ANSI version of KeePassLibC, define KDB_ANSI.
// If compiling for the Unicode version of KeePassLibC, do not define KDB_ANSI.
#define KDB_ANSI
#endif

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KeePass.DataExchange
{
	/// <summary>
	/// Structure containing information about a password group. This structure
	/// doesn't contain any information about the entries stored in this group.
	/// </summary>
#if PocketPC || Smartphone || WindowsCE
	[StructLayout(LayoutKind.Sequential)]
#else
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct KdbGroup
	{
		/// <summary>
		/// The GroupID of the group.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 GroupId;

		/// <summary>
		/// The ImageID of the group.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 ImageId;

		/// <summary>
		/// The Name of the group.
		/// </summary>
#if KDB_ANSI
		[MarshalAs(UnmanagedType.LPStr)]
		public string Name;
#else
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Name;
#endif

		/// <summary>
		/// The creation time of the group.
		/// </summary>
		public KdbTime CreationTime;

		/// <summary>
		/// The last modification time of the group.
		/// </summary>
		public KdbTime LastModificationTime;

		/// <summary>
		/// The last access time of the group.
		/// </summary>
		public KdbTime LastAccessTime;

		/// <summary>
		/// The expiry time of the group.
		/// </summary>
		public KdbTime ExpirationTime;

		/// <summary>
		/// Indentation level of the group.
		/// </summary>
		[MarshalAs(UnmanagedType.U2)]
		public UInt16 Level;

#if VPF_ALIGN
		/// <summary>
		/// Dummy entry for alignment purposes.
		/// </summary>
		[MarshalAs(UnmanagedType.U2)]
		public UInt16 Dummy;
#endif

		/// <summary>
		/// Flags of the group (see <c>KdbGroupFlags</c>).
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 Flags;
	}

	/// <summary>
	/// Password group flags.
	/// </summary>
	[Flags]
	public enum KdbGroupFlags
	{
		/// <summary>
		/// No special flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// The group is expanded.
		/// </summary>
		Expanded = 1
	}

	/// <summary>
	/// Structure containing information about a password entry.
	/// </summary>
#if PocketPC || Smartphone || WindowsCE
	[StructLayout(LayoutKind.Sequential)]
#else
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct KdbEntry
	{
		/// <summary>
		/// The UUID of the entry.
		/// </summary>
		public KdbUuid Uuid;

		/// <summary>
		/// The group ID of the enty.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 GroupId;

		/// <summary>
		/// The image ID of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 ImageId;

#if KDB_ANSI
		/// <summary>
		/// The title of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPStr)]
		public string Title;

		/// <summary>
		/// The URL of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPStr)]
		public string Url;

		/// <summary>
		/// The user name of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPStr)]
		public string UserName;

		/// <summary>
		/// The password length of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 PasswordLength;

		/// <summary>
		/// The password of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPStr)]
		public string Password;

		/// <summary>
		/// The notes field of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPStr)]
		public string Additional;
#else
		/// <summary>
		/// The title of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Title;

		/// <summary>
		/// The URL of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Url;

		/// <summary>
		/// The user name of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string UserName;

		/// <summary>
		/// The password length of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 PasswordLength;

		/// <summary>
		/// The password of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Password;

		/// <summary>
		/// The notes field of the entry.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Additional;
#endif

		/// <summary>
		/// The creation time of the entry.
		/// </summary>
		public KdbTime CreationTime;

		/// <summary>
		/// The last modification time of the entry.
		/// </summary>
		public KdbTime LastModificationTime;

		/// <summary>
		/// The last access time of the entry.
		/// </summary>
		public KdbTime LastAccessTime;

		/// <summary>
		/// The expiration time of the entry.
		/// </summary>
		public KdbTime ExpirationTime;

		/// <summary>
		/// The description of the binary attachment.
		/// </summary>
#if KDB_ANSI
		[MarshalAs(UnmanagedType.LPStr)]
		public string BinaryDescription;
#else
		[MarshalAs(UnmanagedType.LPWStr)]
		public string BinaryDescription;
#endif

		/// <summary>
		/// The attachment of the entry.
		/// </summary>
		public IntPtr BinaryData;

		/// <summary>
		/// The length of the attachment.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		public UInt32 BinaryDataLength;
	}

	/// <summary>
	/// Structure containing UUID bytes (16 bytes).
	/// </summary>
#if PocketPC || Smartphone || WindowsCE
	[StructLayout(LayoutKind.Sequential)]
#else
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct KdbUuid
	{
		public Byte V0; public Byte V1; public Byte V2; public Byte V3;
		public Byte V4; public Byte V5; public Byte V6; public Byte V7;
		public Byte V8; public Byte V9; public Byte VA; public Byte VB;
		public Byte VC; public Byte VD; public Byte VE; public Byte VF;

		/// <summary>
		/// Convert UUID to a byte array of length 16.
		/// </summary>
		/// <returns>Byte array (16 bytes).</returns>
		public byte[] ToByteArray()
		{
			return new byte[]{ this.V0, this.V1, this.V2, this.V3, this.V4,
				this.V5, this.V6, this.V7, this.V8, this.V9, this.VA,
				this.VB, this.VC, this.VD, this.VE, this.VF };
		}

		/// <summary>
		/// Set the UUID using an array of 16 bytes.
		/// </summary>
		/// <param name="pb">Bytes to set the UUID to.</param>
		public void Set(byte[] pb)
		{
			Debug.Assert((pb != null) && (pb.Length == 16));
			if(pb == null) throw new ArgumentNullException("pb");
			if(pb.Length != 16) throw new ArgumentException();

			this.V0 = pb[0]; this.V1 = pb[1]; this.V2 = pb[2]; this.V3 = pb[3];
			this.V4 = pb[4]; this.V5 = pb[5]; this.V6 = pb[6]; this.V7 = pb[7];
			this.V8 = pb[8]; this.V9 = pb[9]; this.VA = pb[10]; this.VB = pb[11];
			this.VC = pb[12]; this.VD = pb[13]; this.VE = pb[14]; this.VF = pb[15];
		}
	}

	/// <summary>
	/// Structure containing time information in a compact, but still easily
	/// accessible form.
	/// </summary>
#if PocketPC || Smartphone || WindowsCE
	[StructLayout(LayoutKind.Sequential)]
#else
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
#endif
	public struct KdbTime
	{
		[MarshalAs(UnmanagedType.U2)]
		public UInt16 Year;
		[MarshalAs(UnmanagedType.U1)]
		public Byte Month;
		[MarshalAs(UnmanagedType.U1)]
		public Byte Day;
		[MarshalAs(UnmanagedType.U1)]
		public Byte Hour;
		[MarshalAs(UnmanagedType.U1)]
		public Byte Minute;
		[MarshalAs(UnmanagedType.U1)]
		public Byte Second;

#if VPF_ALIGN
		/// <summary>
		/// Dummy entry for alignment purposes.
		/// </summary>
		[MarshalAs(UnmanagedType.U1)]
		public Byte Dummy;
#endif

		/// <summary>
		/// Construct a <c>KdbTime</c> with initial values.
		/// </summary>
		/// <param name="uYear">Year.</param>
		/// <param name="uMonth">Month.</param>
		/// <param name="uDay">Day.</param>
		/// <param name="uHour">Hour.</param>
		/// <param name="uMinute">Minute.</param>
		/// <param name="uSecond">Second.</param>
		public KdbTime(UInt16 uYear, Byte uMonth, Byte uDay, Byte uHour,
			Byte uMinute, Byte uSecond)
		{
			this.Year = uYear;
			this.Month = uMonth;
			this.Day = uDay;
			this.Hour = uHour;
			this.Minute = uMinute;
			this.Second = uSecond;

#if VPF_ALIGN
			this.Dummy = 0;
#endif
		}

		/// <summary>
		/// Convert the current <c>KdbTime</c> object to a <c>DateTime</c> object.
		/// </summary>
		public DateTime ToDateTime()
		{
			if((this.Year == 0) || (this.Month == 0) || (this.Day == 0))
				return DateTime.Now;

			// https://sourceforge.net/p/keepass/discussion/329221/thread/07599afd/
			try
			{
				int dy = (int)this.Year;
				if(dy > 2999) { Debug.Assert(false); dy = 2999; }

				int dm = (int)this.Month;
				if(dm > 12) { Debug.Assert(false); dm = 12; }

				int dd = (int)this.Day;
				if(dd > 31) { Debug.Assert(false); dd = 28; }
				// Day might not exist in month

				int th = (int)this.Hour;
				if(th > 23) { Debug.Assert(false); th = 23; }

				int tm = (int)this.Minute;
				if(tm > 59) { Debug.Assert(false); tm = 59; }

				int ts = (int)this.Second;
				if(ts > 59) { Debug.Assert(false); ts = 59; }

				return new DateTime(dy, dm, dd, th, tm, ts);
			}
			catch(Exception) { Debug.Assert(false); }

			return DateTime.Now;
		}

		/// <summary>
		/// Copy data from a <c>DateTime</c> object to the current <c>KdbTime</c> object.
		/// </summary>
		/// <param name="dt">Data source.</param>
		public void Set(DateTime dt)
		{
			this.Year = (UInt16)dt.Year;
			this.Month = (Byte)dt.Month;
			this.Day = (Byte)dt.Day;
			this.Hour = (Byte)dt.Hour;
			this.Minute = (Byte)dt.Minute;
			this.Second = (Byte)dt.Second;
		}

		/// <summary>
		/// A KdbTime element that is used to indicate the never expire time.
		/// </summary>
		public static readonly KdbTime NeverExpireTime =
			new KdbTime(2999, 12, 28, 23, 59, 59);
	}

	/// <summary>
	/// Error codes for various functions (<c>OpenDatabase</c>, etc.).
	/// </summary>
	public enum KdbErrorCode
	{
		/// <summary>
		/// Unknown error occurred.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Successful function call.
		/// </summary>
		Success,

		/// <summary>
		/// Invalid parameters were given.
		/// </summary>
		InvalidParam,

		/// <summary>
		/// Not enough memory to perform requested operation.
		/// </summary>
		NotEnoughMemory,

		/// <summary>
		/// Invalid key was supplied.
		/// </summary>
		InvalidKey,

		/// <summary>
		/// The file could not be read.
		/// </summary>
		NoFileAccessRead,

		/// <summary>
		/// The file could not be written.
		/// </summary>
		NoFileAccessWrite,

		/// <summary>
		/// A file read error occurred.
		/// </summary>
		FileErrorRead,

		/// <summary>
		/// A file write error occurred.
		/// </summary>
		FileErrorWrite,

		/// <summary>
		/// Invalid random source was given.
		/// </summary>
		InvalidRandomSource,

		/// <summary>
		/// Invalid file structure was detected.
		/// </summary>
		InvalidFileStructure,

		/// <summary>
		/// Cryptographic error occurred.
		/// </summary>
		CryptoError,

		/// <summary>
		/// Invalid file size was given/detected.
		/// </summary>
		InvalidFileSize,

		/// <summary>
		/// Invalid file signature was detected.
		/// </summary>
		InvalidFileSignature,

		/// <summary>
		/// Invalid file header was detected.
		/// </summary>
		InvalidFileHeader,

		/// <summary>
		/// The keyfile could not be read.
		/// </summary>
		NoFileAccessReadKey
	}

	/// <summary>
	/// Manager class for KDB files. It can load/save databases, add/change/delete
	/// groups and entries, check for KeePassLibC library existence and version, etc.
	/// </summary>
	public sealed class KdbManager : IDisposable
	{
		private const string DllFile32 = "KeePassLibC32.dll";
		private const string DllFile64 = "KeePassLibC64.dll";

		private static readonly bool m_bX64 =
			(Marshal.SizeOf(typeof(IntPtr)) == 8);

#if KDB_ANSI
		private const CharSet DllCharSet = CharSet.Ansi;
#else
		private const CharSet DllCharSet = CharSet.Unicode;
#endif

		private static bool m_bFirstInstance = true;
		private IntPtr m_pManager = IntPtr.Zero;

		[DllImport(DllFile32, EntryPoint = "GetKeePassVersion")]
		private static extern UInt32 GetKeePassVersion32();
		[DllImport(DllFile64, EntryPoint = "GetKeePassVersion")]
		private static extern UInt32 GetKeePassVersion64();
		/// <summary>
		/// Get the KeePass version, which the KeePassLibC library supports.
		/// </summary>
		public static UInt32 KeePassVersion
		{
			get
			{
				if(m_bX64) return GetKeePassVersion64();
				else return GetKeePassVersion32();
			}
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "GetKeePassVersionString")]
		private static extern IntPtr GetKeePassVersionString32();
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "GetKeePassVersionString")]
		private static extern IntPtr GetKeePassVersionString64();
		/// <summary>
		/// Get the KeePass version, which the KeePassLibC library supports
		/// (the version is returned as a displayable string, if you need to
		/// compare versions: use the <c>KeePassVersion</c> property).
		/// </summary>
		public static string KeePassVersionString
		{
#if KDB_ANSI
			get
			{
				if(m_bX64) return Marshal.PtrToStringAnsi(GetKeePassVersionString64());
				else return Marshal.PtrToStringAnsi(GetKeePassVersionString32());
			}
#else
			get
			{
				if(m_bX64) return Marshal.PtrToStringUni(GetKeePassVersionString64());
				else return Marshal.PtrToStringUni(GetKeePassVersionString32());
			}
#endif
		}

		[DllImport(DllFile32, EntryPoint = "GetLibraryBuild")]
		private static extern UInt32 GetLibraryBuild32();
		[DllImport(DllFile64, EntryPoint = "GetLibraryBuild")]
		private static extern UInt32 GetLibraryBuild64();
		/// <summary>
		/// Get the library build version. This version has nothing to do with
		/// the supported KeePass version (see <c>KeePassVersion</c> property).
		/// </summary>
		public static UInt32 LibraryBuild
		{
			get
			{
				if(m_bX64) return GetLibraryBuild64();
				else return GetLibraryBuild32();
			}
		}

		[DllImport(DllFile32, EntryPoint = "GetNumberOfEntries")]
		private static extern UInt32 GetNumberOfEntries32(IntPtr pMgr);
		[DllImport(DllFile64, EntryPoint = "GetNumberOfEntries")]
		private static extern UInt32 GetNumberOfEntries64(IntPtr pMgr);
		/// <summary>
		/// Get the number of entries in this manager instance.
		/// </summary>
		public UInt32 EntryCount
		{
			get
			{
				if(m_bX64) return GetNumberOfEntries64(m_pManager);
				else return GetNumberOfEntries32(m_pManager);
			}
		}

		[DllImport(DllFile32, EntryPoint = "GetNumberOfGroups")]
		private static extern UInt32 GetNumberOfGroups32(IntPtr pMgr);
		[DllImport(DllFile64, EntryPoint = "GetNumberOfGroups")]
		private static extern UInt32 GetNumberOfGroups64(IntPtr pMgr);
		/// <summary>
		/// Get the number of groups in this manager instance.
		/// </summary>
		public UInt32 GroupCount
		{
			get
			{
				if(m_bX64) return GetNumberOfGroups64(m_pManager);
				else return GetNumberOfGroups32(m_pManager);
			}
		}

		[DllImport(DllFile32, EntryPoint = "GetKeyEncRounds")]
		private static extern UInt32 GetKeyEncRounds32(IntPtr pMgr);
		[DllImport(DllFile64, EntryPoint = "GetKeyEncRounds")]
		private static extern UInt32 GetKeyEncRounds64(IntPtr pMgr);
		[DllImport(DllFile32, EntryPoint = "SetKeyEncRounds")]
		private static extern void SetKeyEncRounds32(IntPtr pMgr, UInt32 dwRounds);
		[DllImport(DllFile64, EntryPoint = "SetKeyEncRounds")]
		private static extern void SetKeyEncRounds64(IntPtr pMgr, UInt32 dwRounds);
		/// <summary>
		/// Get or set the number of key transformation rounds (in order to
		/// make key-searching attacks harder).
		/// </summary>
		public UInt32 KeyTransformationRounds
		{
			get
			{
				if(m_bX64) return GetKeyEncRounds64(m_pManager);
				else return GetKeyEncRounds32(m_pManager);
			}
			set
			{
				if(m_bX64) SetKeyEncRounds64(m_pManager, value);
				else SetKeyEncRounds32(m_pManager, value);
			}
		}

		[DllImport(DllFile32, EntryPoint = "InitManager")]
		private static extern void InitManager32(out IntPtr ppMgr,
			[MarshalAs(UnmanagedType.Bool)] bool bFirstInstance);
		[DllImport(DllFile64, EntryPoint = "InitManager")]
		private static extern void InitManager64(out IntPtr ppMgr,
			[MarshalAs(UnmanagedType.Bool)] bool bFirstInstance);

		[DllImport(DllFile32, EntryPoint = "NewDatabase")]
		private static extern void NewDatabase32(IntPtr pMgr);
		[DllImport(DllFile64, EntryPoint = "NewDatabase")]
		private static extern void NewDatabase64(IntPtr pMgr);
		/// <summary>
		/// Construct a new KDB manager instance.
		/// </summary>
		public KdbManager()
		{
			if(!m_bX64) // Only check 32-bit structures
			{
#if VPF_ALIGN
				bool bAligned = true;
#else
				bool bAligned = false;
#endif

				// Static structure layout assertions
				int nExpectedSize = (bAligned ? 52 : 46);
				KdbGroup g = new KdbGroup();
				Debug.Assert(Marshal.SizeOf(g) == nExpectedSize);
				if(Marshal.SizeOf(g) != nExpectedSize)
					throw new FormatException("SizeOf(KdbGroup) invalid!");

				nExpectedSize = (bAligned ? 92 : 88);
				KdbEntry e = new KdbEntry();
				Debug.Assert(Marshal.SizeOf(e) == nExpectedSize);
				if(Marshal.SizeOf(e) != nExpectedSize)
					throw new FormatException("SizeOf(KdbEntry) invalid!");

				KdbUuid u = new KdbUuid();
				Debug.Assert(Marshal.SizeOf(u) == 16);
				if(Marshal.SizeOf(u) != 16)
					throw new FormatException("SizeOf(KdbUuid) invalid!");

				nExpectedSize = (bAligned ? 8 : 7);
				KdbTime t = new KdbTime();
				Debug.Assert(Marshal.SizeOf(t) == nExpectedSize);
				if(Marshal.SizeOf(t) != nExpectedSize)
					throw new FormatException("SizeOf(KdbTime) invalid!");
			}

			if(m_bX64) KdbManager.InitManager64(out m_pManager, m_bFirstInstance);
			else KdbManager.InitManager32(out m_pManager, m_bFirstInstance);

			m_bFirstInstance = false;

			if(m_pManager == IntPtr.Zero)
				throw new InvalidOperationException("Failed to initialize manager! DLL installed?");

			if(m_bX64) KdbManager.NewDatabase64(m_pManager);
			else KdbManager.NewDatabase32(m_pManager);
		}

		~KdbManager()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[DllImport(DllFile32, EntryPoint = "DeleteManager")]
		private static extern void DeleteManager32(IntPtr pMgr);
		[DllImport(DllFile64, EntryPoint = "DeleteManager")]
		private static extern void DeleteManager64(IntPtr pMgr);
		private void Dispose(bool bDisposing)
		{
			if(m_pManager != IntPtr.Zero)
			{
				try
				{
					if(m_bX64) KdbManager.DeleteManager64(m_pManager);
					else KdbManager.DeleteManager32(m_pManager);
				}
				catch(Exception) { Debug.Assert(false); }

				m_pManager = IntPtr.Zero;
			}
		}

		[Obsolete]
		public void Unload()
		{
			Dispose(true);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "SetMasterKey")]
		private static extern Int32 SetMasterKey32(IntPtr pMgr, string pszMasterKey,
			[MarshalAs(UnmanagedType.Bool)] bool bDiskDrive, string pszSecondKey,
			IntPtr pARI, [MarshalAs(UnmanagedType.Bool)] bool bOverwrite);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "SetMasterKey")]
		private static extern Int32 SetMasterKey64(IntPtr pMgr, string pszMasterKey,
			[MarshalAs(UnmanagedType.Bool)] bool bDiskDrive, string pszSecondKey,
			IntPtr pARI, [MarshalAs(UnmanagedType.Bool)] bool bOverwrite);
		/// <summary>
		/// Set the master key, which will be used when you call the
		/// <c>OpenDatabase</c> or <c>SaveDatabase</c> functions.
		/// </summary>
		/// <param name="strMasterKey">Master password or path to key file. Must not be <c>null</c>.</param>
		/// <param name="bDiskDrive">Indicates if a key file is used.</param>
		/// <param name="strSecondKey">Path to the key file when both master password
		/// and key file are used.</param>
		/// <param name="pARI">Random source interface. Set this to <c>IntPtr.Zero</c>
		/// if you want to open a KDB file normally. If you want to create a key file,
		/// <c>pARI</c> must point to a <c>CNewRandomInterface</c> (see KeePass 1.x
		/// source code).</param>
		/// <param name="bOverwrite">Indicates if the target file should be overwritten when
		/// creating a new key file.</param>
		/// <returns>Error code (see <c>KdbErrorCode</c>).</returns>
		public KdbErrorCode SetMasterKey(string strMasterKey, bool bDiskDrive,
			string strSecondKey, IntPtr pARI, bool bOverwrite)
		{
			Debug.Assert(strMasterKey != null);
			if(strMasterKey == null) throw new ArgumentNullException("strMasterKey");

			if(m_bX64)
				return (KdbErrorCode)KdbManager.SetMasterKey64(m_pManager,
					strMasterKey, bDiskDrive, strSecondKey, pARI, bOverwrite);
			else
				return (KdbErrorCode)KdbManager.SetMasterKey32(m_pManager,
					strMasterKey, bDiskDrive, strSecondKey, pARI, bOverwrite);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "GetNumberOfItemsInGroup")]
		private static extern UInt32 GetNumberOfItemsInGroup32(IntPtr pMgr, string pszGroup);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "GetNumberOfItemsInGroup")]
		private static extern UInt32 GetNumberOfItemsInGroup64(IntPtr pMgr, string pszGroup);
		/// <summary>
		/// Get the number of entries in a group.
		/// </summary>
		/// <param name="strGroupName">Group name, which identifies the group. Note
		/// that multiple groups can have the same name, in this case you'll need to
		/// use the other counting function which uses group IDs.</param>
		/// <returns>Number of entries in the specified group.</returns>
		public UInt32 GetNumberOfEntriesInGroup(string strGroupName)
		{
			Debug.Assert(strGroupName != null);
			if(strGroupName == null) throw new ArgumentNullException("strGroupName");

			if(m_bX64) return KdbManager.GetNumberOfItemsInGroup64(m_pManager, strGroupName);
			else return KdbManager.GetNumberOfItemsInGroup32(m_pManager, strGroupName);
		}

		[DllImport(DllFile32, EntryPoint = "GetNumberOfItemsInGroupN")]
		private static extern UInt32 GetNumberOfItemsInGroupN32(IntPtr pMgr, UInt32 idGroup);
		[DllImport(DllFile64, EntryPoint = "GetNumberOfItemsInGroupN")]
		private static extern UInt32 GetNumberOfItemsInGroupN64(IntPtr pMgr, UInt32 idGroup);
		/// <summary>
		/// Get the number of entries in a group.
		/// </summary>
		/// <param name="uGroupId">Group ID.</param>
		/// <returns>Number of entries in the specified group.</returns>
		public UInt32 GetNumberOfEntriesInGroup(UInt32 uGroupId)
		{
			Debug.Assert((uGroupId != 0) && (uGroupId != UInt32.MaxValue));
			if((uGroupId == 0) || (uGroupId == UInt32.MaxValue))
				throw new ArgumentException("Invalid group ID!");

			if(m_bX64) return KdbManager.GetNumberOfItemsInGroupN64(m_pManager, uGroupId);
			else return KdbManager.GetNumberOfItemsInGroupN32(m_pManager, uGroupId);
		}

		[DllImport(DllFile32, EntryPoint = "LockEntryPassword")]
		private static extern IntPtr LockEntryPassword32(IntPtr pMgr, IntPtr pEntry);
		[DllImport(DllFile64, EntryPoint = "LockEntryPassword")]
		private static extern IntPtr LockEntryPassword64(IntPtr pMgr, IntPtr pEntry);
		[DllImport(DllFile32, EntryPoint = "UnlockEntryPassword")]
		private static extern IntPtr UnlockEntryPassword32(IntPtr pMgr, IntPtr pEntry);
		[DllImport(DllFile64, EntryPoint = "UnlockEntryPassword")]
		private static extern IntPtr UnlockEntryPassword64(IntPtr pMgr, IntPtr pEntry);

		[DllImport(DllFile32, EntryPoint = "GetEntry")]
		private static extern IntPtr GetEntry32(IntPtr pMgr, UInt32 dwIndex);
		[DllImport(DllFile64, EntryPoint = "GetEntry")]
		private static extern IntPtr GetEntry64(IntPtr pMgr, UInt32 dwIndex);
		/// <summary>
		/// Get an entry.
		/// </summary>
		/// <param name="uIndex">Index of the entry. This index must be valid, otherwise
		/// an <c>ArgumentOutOfRangeException</c> is thrown.</param>
		/// <returns>The requested entry. Note that any modifications to this
		/// structure won't affect the internal data structures of the manager.</returns>
		public KdbEntry GetEntry(uint uIndex)
		{
			Debug.Assert(uIndex < this.EntryCount);

			IntPtr p;
			if(m_bX64) p = KdbManager.GetEntry64(m_pManager, uIndex);
			else p = KdbManager.GetEntry32(m_pManager, uIndex);

			if(p == IntPtr.Zero) throw new ArgumentOutOfRangeException("uIndex");

			if(m_bX64) KdbManager.UnlockEntryPassword64(m_pManager, p);
			else KdbManager.UnlockEntryPassword32(m_pManager, p);

			KdbEntry kdbEntry = (KdbEntry)Marshal.PtrToStructure(p, typeof(KdbEntry));
			
			if(m_bX64) KdbManager.LockEntryPassword64(m_pManager, p);
			else KdbManager.LockEntryPassword32(m_pManager, p);
			
			return kdbEntry;
		}

		[DllImport(DllFile32, EntryPoint = "GetEntryByGroup")]
		private static extern IntPtr GetEntryByGroup32(IntPtr pMgr, UInt32 idGroup, UInt32 dwIndex);
		[DllImport(DllFile64, EntryPoint = "GetEntryByGroup")]
		private static extern IntPtr GetEntryByGroup64(IntPtr pMgr, UInt32 idGroup, UInt32 dwIndex);
		/// <summary>
		/// Get an entry in a specific group.
		/// </summary>
		/// <param name="uIndex">Index of the entry in the group. This index must
		/// be valid, otherwise an <c>ArgumentOutOfRangeException</c> is thrown.</param>
		/// <param name="uGroupId">ID of the group containing the entry.</param>
		/// <returns>The requested entry. Note that any modifications to this
		/// structure won't affect the internal data structures of the manager.</returns>
		public KdbEntry GetEntryByGroup(UInt32 uGroupId, UInt32 uIndex)
		{
			Debug.Assert((uGroupId != 0) && (uGroupId != UInt32.MaxValue));
			if((uGroupId == 0) || (uGroupId == UInt32.MaxValue))
				throw new ArgumentException("Invalid group ID!");

			Debug.Assert(uIndex < this.EntryCount);

			IntPtr p;
			if(m_bX64) p = GetEntryByGroup64(m_pManager, uGroupId, uIndex);
			else p = GetEntryByGroup32(m_pManager, uGroupId, uIndex);

			if(p == IntPtr.Zero) throw new ArgumentOutOfRangeException();

			if(m_bX64) KdbManager.UnlockEntryPassword64(m_pManager, p);
			else KdbManager.UnlockEntryPassword32(m_pManager, p);

			KdbEntry kdbEntry = (KdbEntry)Marshal.PtrToStructure(p, typeof(KdbEntry));

			if(m_bX64) KdbManager.LockEntryPassword64(m_pManager, p);
			else KdbManager.LockEntryPassword32(m_pManager, p);

			return kdbEntry;
		}

		[DllImport(DllFile32, EntryPoint = "GetGroup")]
		private static extern IntPtr GetGroup32(IntPtr pMgr, UInt32 dwIndex);
		[DllImport(DllFile64, EntryPoint = "GetGroup")]
		private static extern IntPtr GetGroup64(IntPtr pMgr, UInt32 dwIndex);
		/// <summary>
		/// Get a group.
		/// </summary>
		/// <param name="uIndex">Index of the group. Must be valid, otherwise an
		/// <c>ArgumentOutOfRangeException</c> is thrown.</param>
		/// <returns>Group structure.</returns>
		public KdbGroup GetGroup(UInt32 uIndex)
		{
			Debug.Assert(uIndex < this.GroupCount);

			IntPtr p;
			if(m_bX64) p = KdbManager.GetGroup64(m_pManager, uIndex);
			else p = KdbManager.GetGroup32(m_pManager, uIndex);

			if(p == IntPtr.Zero) throw new ArgumentOutOfRangeException("uIndex");

			return (KdbGroup)Marshal.PtrToStructure(p, typeof(KdbGroup));
		}

		[DllImport(DllFile32, EntryPoint = "GetGroupById")]
		private static extern IntPtr GetGroupById32(IntPtr pMgr, UInt32 idGroup);
		[DllImport(DllFile64, EntryPoint = "GetGroupById")]
		private static extern IntPtr GetGroupById64(IntPtr pMgr, UInt32 idGroup);
		/// <summary>
		/// Get a group via the GroupID.
		/// </summary>
		/// <param name="uGroupId">ID of the group.</param>
		/// <returns>Group structure.</returns>
		public KdbGroup GetGroupById(UInt32 uGroupId)
		{
			Debug.Assert((uGroupId != 0) && (uGroupId != UInt32.MaxValue));
			if((uGroupId == 0) || (uGroupId == UInt32.MaxValue))
				throw new ArgumentException("Invalid group ID!");

			IntPtr p;
			if(m_bX64) p = KdbManager.GetGroupById64(m_pManager, uGroupId);
			else p = KdbManager.GetGroupById32(m_pManager, uGroupId);

			if(p == IntPtr.Zero) throw new ArgumentOutOfRangeException("uGroupId");

			return (KdbGroup)Marshal.PtrToStructure(p, typeof(KdbGroup));
		}

		[DllImport(DllFile32, EntryPoint = "GetGroupByIdN")]
		private static extern UInt32 GetGroupByIdN32(IntPtr pMgr, UInt32 idGroup);
		[DllImport(DllFile64, EntryPoint = "GetGroupByIdN")]
		private static extern UInt32 GetGroupByIdN64(IntPtr pMgr, UInt32 idGroup);
		/// <summary>
		/// Get the group index via the GroupID.
		/// </summary>
		/// <param name="uGroupId">ID of the group.</param>
		/// <returns>Group index.</returns>
		public UInt32 GetGroupByIdN(UInt32 uGroupId)
		{
			Debug.Assert((uGroupId != 0) && (uGroupId != UInt32.MaxValue));
			if((uGroupId == 0) || (uGroupId == UInt32.MaxValue))
				throw new ArgumentException("Invalid group ID!");

			if(m_bX64) return KdbManager.GetGroupByIdN64(m_pManager, uGroupId);
			else return KdbManager.GetGroupByIdN32(m_pManager, uGroupId);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "OpenDatabase")]
		private static extern Int32 OpenDatabase32(IntPtr pMgr, string pszFile, IntPtr pRepair);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "OpenDatabase")]
		private static extern Int32 OpenDatabase64(IntPtr pMgr, string pszFile, IntPtr pRepair);
		/// <summary>
		/// Open a KDB database.
		/// </summary>
		/// <param name="strFile">File path of the database.</param>
		/// <param name="pRepairInfo">Pointer to a repair information structure. If
		/// you want to open a Kdb file normally, set this parameter to
		/// <c>IntPtr.Zero</c>.</param>
		/// <returns>Error code (see <c>KdbErrorCode</c>).</returns>
		public KdbErrorCode OpenDatabase(string strFile, IntPtr pRepairInfo)
		{
			Debug.Assert(strFile != null);
			if(strFile == null) throw new ArgumentNullException("strFile");

			if(m_bX64) return (KdbErrorCode)KdbManager.OpenDatabase64(m_pManager, strFile, pRepairInfo);
			else return (KdbErrorCode)KdbManager.OpenDatabase32(m_pManager, strFile, pRepairInfo);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "SaveDatabase")]
		private static extern Int32 SaveDatabase32(IntPtr pMgr, string pszFile);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "SaveDatabase")]
		private static extern Int32 SaveDatabase64(IntPtr pMgr, string pszFile);
		/// <summary>
		/// Save the current contents of the manager to a KDB file on disk.
		/// </summary>
		/// <param name="strFile">File to create.</param>
		/// <returns>Error code (see <c>KdbErrorCode</c>).</returns>
		public KdbErrorCode SaveDatabase(string strFile)
		{
			Debug.Assert(strFile != null);
			if(strFile == null) throw new ArgumentNullException("strFile");

			if(m_bX64) return (KdbErrorCode)KdbManager.SaveDatabase64(m_pManager, strFile);
			else return (KdbErrorCode)KdbManager.SaveDatabase32(m_pManager, strFile);
		}

		/// <summary>
		/// Close current database and create a new and empty one.
		/// </summary>
		public void NewDatabase()
		{
			if(m_bX64) KdbManager.NewDatabase64(m_pManager);
			else KdbManager.NewDatabase32(m_pManager);
		}

		/// <summary>
		/// Get the date/time object representing the 'Never Expires' status.
		/// </summary>
		/// <returns><c>DateTime</c> object.</returns>
		public static DateTime GetNeverExpireTime()
		{
			return KdbTime.NeverExpireTime.ToDateTime();
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "AddGroup")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AddGroup32(IntPtr pMgr, ref KdbGroup pTemplate);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "AddGroup")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AddGroup64(IntPtr pMgr, ref KdbGroup pTemplate);
		/// <summary>
		/// Add a new password group.
		/// </summary>
		/// <param name="pNewGroup">Template containing new group information.</param>
		/// <returns>Returns <c>true</c> if the group was created successfully.</returns>
		public bool AddGroup(ref KdbGroup pNewGroup)
		{
			if(m_bX64) return KdbManager.AddGroup64(m_pManager, ref pNewGroup);
			else return KdbManager.AddGroup32(m_pManager, ref pNewGroup);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "SetGroup")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetGroup32(IntPtr pMgr, UInt32 dwIndex, ref KdbGroup pTemplate);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "SetGroup")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetGroup64(IntPtr pMgr, UInt32 dwIndex, ref KdbGroup pTemplate);
		/// <summary>
		/// Set/change a password group.
		/// </summary>
		/// <param name="uIndex">Index of the group to be changed.</param>
		/// <param name="pNewGroup">Template containing new group information.</param>
		/// <returns>Returns <c>true</c> if the group was created successfully.</returns>
		public bool SetGroup(UInt32 uIndex, ref KdbGroup pNewGroup)
		{
			Debug.Assert(uIndex < this.GroupCount);

			if(m_bX64) return KdbManager.SetGroup64(m_pManager, uIndex, ref pNewGroup);
			else return KdbManager.SetGroup32(m_pManager, uIndex, ref pNewGroup);
		}

		[DllImport(DllFile32, EntryPoint = "DeleteGroupById")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteGroupById32(IntPtr pMgr, UInt32 uGroupId);
		[DllImport(DllFile64, EntryPoint = "DeleteGroupById")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteGroupById64(IntPtr pMgr, UInt32 uGroupId);
		/// <summary>
		/// Delete a password group.
		/// </summary>
		/// <param name="uGroupID">ID of the group to be deleted.</param>
		/// <returns>If the group has been deleted, the return value is <c>true</c>.</returns>
		public bool DeleteGroupByID(UInt32 uGroupID)
		{
			Debug.Assert((uGroupID != 0) && (uGroupID != UInt32.MaxValue));
			if((uGroupID == 0) || (uGroupID == UInt32.MaxValue))
				throw new ArgumentException("Invalid group ID!");

			if(m_bX64) return KdbManager.DeleteGroupById64(m_pManager, uGroupID);
			else return KdbManager.DeleteGroupById32(m_pManager, uGroupID);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "AddEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AddEntry32(IntPtr pMgr, ref KdbEntry pTemplate);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "AddEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AddEntry64(IntPtr pMgr, ref KdbEntry pTemplate);
		/// <summary>
		/// Add a new password entry.
		/// </summary>
		/// <param name="peNew">Template containing new entry information.</param>
		/// <returns>Returns <c>true</c> if the entry was created successfully.</returns>
		public bool AddEntry(ref KdbEntry peNew)
		{
			if(m_bX64) return KdbManager.AddEntry64(m_pManager, ref peNew);
			else return KdbManager.AddEntry32(m_pManager, ref peNew);
		}

		[DllImport(DllFile32, CharSet = DllCharSet, EntryPoint = "SetEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetEntry32(IntPtr pMgr, UInt32 dwIndex, ref KdbEntry pTemplate);
		[DllImport(DllFile64, CharSet = DllCharSet, EntryPoint = "SetEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetEntry64(IntPtr pMgr, UInt32 dwIndex, ref KdbEntry pTemplate);
		/// <summary>
		/// Set/change a password entry.
		/// </summary>
		/// <param name="uIndex">Index of the entry to be changed.</param>
		/// <param name="peNew">Template containing new entry information.</param>
		/// <returns>Returns <c>true</c> if the entry was created successfully.</returns>
		public bool SetEntry(UInt32 uIndex, ref KdbEntry peNew)
		{
			Debug.Assert(uIndex < this.EntryCount);

			if(m_bX64) return KdbManager.SetEntry64(m_pManager, uIndex, ref peNew);
			else return KdbManager.SetEntry32(m_pManager, uIndex, ref peNew);
		}

		[DllImport(DllFile32, EntryPoint = "DeleteEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteEntry32(IntPtr pMgr, UInt32 dwIndex);
		[DllImport(DllFile64, EntryPoint = "DeleteEntry")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeleteEntry64(IntPtr pMgr, UInt32 dwIndex);
		/// <summary>
		/// Delete a password entry.
		/// </summary>
		/// <param name="uIndex">Index of the entry.</param>
		/// <returns>If the entry has been deleted, the return value is <c>true</c>.</returns>
		public bool DeleteEntry(UInt32 uIndex)
		{
			Debug.Assert(uIndex < this.EntryCount);

			if(m_bX64) return KdbManager.DeleteEntry64(m_pManager, uIndex);
			else return KdbManager.DeleteEntry32(m_pManager, uIndex);
		}

		/// <summary>
		/// Helper function to extract file attachments.
		/// </summary>
		/// <param name="pMemory">Native memory pointer (as stored in the
		/// <c>BinaryData</c> member of <c>KdbEntry</c>.</param>
		/// <param name="uSize">Size in bytes of the memory block.</param>
		/// <returns>Managed byte array.</returns>
		public static byte[] ReadBinary(IntPtr pMemory, uint uSize)
		{
			Debug.Assert(pMemory != IntPtr.Zero);
			if(pMemory == IntPtr.Zero) throw new ArgumentNullException("pMemory");

			byte[] pb = new byte[uSize];
			if(uSize == 0) return pb;

			Marshal.Copy(pMemory, pb, 0, (int)uSize);
			return pb;
		}
	}
}

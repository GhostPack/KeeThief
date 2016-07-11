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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using KeePassLib;
using KeePassLib.Native;

namespace KeePass.Util
{
	public static partial class ClipboardUtil
	{
		// https://sourceforge.net/p/keepass/patches/84/
		// https://sourceforge.net/p/keepass/patches/85/
		private const AppRunFlags XSelFlags = (AppRunFlags.GetStdOutput |
			AppRunFlags.GCKeepAlive | AppRunFlags.DoEvents |
			AppRunFlags.DisableForms);

		private static string GetStringM()
		{
			// string strGtk = GtkGetString();
			// if(strGtk != null) return strGtk;

			return (NativeLib.RunConsoleApp("pbpaste", "-pboard general") ??
				string.Empty);
		}

		private static void SetStringM(string str)
		{
			// if(GtkSetString(str)) return;

			NativeLib.RunConsoleApp("pbcopy", "-pboard general", str);
		}

		private static string GetStringU()
		{
			// string strGtk = GtkGetString();
			// if(strGtk != null) return strGtk;

			// string str = NativeLib.RunConsoleApp("xclip",
			//	"-out -selection clipboard");
			// if(str != null) return str;

			string str = NativeLib.RunConsoleApp("xsel",
				"--output --clipboard", null, XSelFlags);
			if(str != null) return str;

			if(Clipboard.ContainsText())
				return (Clipboard.GetText() ?? string.Empty);

			return string.Empty;
		}

		private static void SetStringU(string str)
		{
			// if(GtkSetString(str)) return;

			// string r = NativeLib.RunConsoleApp("xclip",
			//	"-in -selection clipboard", str);
			// if(r != null) return;

			if(string.IsNullOrEmpty(str))
			{
				// xsel with an empty input can hang, thus use --clear
				if(NativeLib.RunConsoleApp("xsel", "--clear --primary",
					null, XSelFlags) != null)
				{
					NativeLib.RunConsoleApp("xsel", "--clear --clipboard",
						null, XSelFlags);
					return;
				}

				try { Clipboard.Clear(); }
				catch(Exception) { Debug.Assert(false); }
				return;
			}

			// xsel does not support --primary and --clipboard together
			if(NativeLib.RunConsoleApp("xsel", "--input --primary",
				str, XSelFlags) != null)
			{
				NativeLib.RunConsoleApp("xsel", "--input --clipboard",
					str, XSelFlags);
				return;
			}

			try { Clipboard.SetText(str); }
			catch(Exception) { Debug.Assert(false); }
		}

		/* private static bool GtkGetClipboard(out Type t, out object o)
		{
			t = null;
			o = null;

			try
			{
				Assembly asmGdk = KeePass.Native.NativeMethods.LoadAssembly(
					"gdk-sharp", "gdk-sharp.dll");
				if(asmGdk == null) return false;
				Assembly asmGtk = KeePass.Native.NativeMethods.LoadAssembly(
					"gtk-sharp", "gtk-sharp.dll");
				if(asmGtk == null) return false;

				if(!KeePass.Native.NativeMethods.GtkEnsureInit()) return false;

				Type tAtom = asmGdk.GetType("Gdk.Atom", true);
				MethodInfo miAtomIntern = tAtom.GetMethod("Intern",
					BindingFlags.Public | BindingFlags.Static);
				if(miAtomIntern == null) { Debug.Assert(false); return false; }

				object oAtomClip = miAtomIntern.Invoke(null, new object[] {
					"CLIPBOARD", false });
				if(oAtomClip == null) { Debug.Assert(false); return false; }

				t = asmGtk.GetType("Gtk.Clipboard", true);
				MethodInfo miClipboardGet = t.GetMethod("Get",
					BindingFlags.Public | BindingFlags.Static);
				if(miClipboardGet == null) { Debug.Assert(false); return false; }

				o = miClipboardGet.Invoke(null, new object[] { oAtomClip });
				if(o == null) { Debug.Assert(false); return false; }

				return true;
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		}

		private static string GtkGetString()
		{
			Type t;
			object o;
			if(!GtkGetClipboard(out t, out o)) return null;

			try
			{
				MethodInfo miTest = t.GetMethod("WaitIsTextAvailable",
					BindingFlags.Public | BindingFlags.Instance);
				if(miTest == null) { Debug.Assert(false); return null; }

				bool bText = (bool)miTest.Invoke(o, null);
				if(!bText) return string.Empty;

				MethodInfo miGet = t.GetMethod("WaitForText",
					BindingFlags.Public | BindingFlags.Instance);
				if(miGet == null) { Debug.Assert(false); return null; }

				return (miGet.Invoke(o, null) as string);
			}
			catch(Exception) { Debug.Assert(false); }

			return null;
		}

		private static bool GtkSetString(string str)
		{
			Type t;
			object o;
			if(!GtkGetClipboard(out t, out o)) return false;

			try
			{
				MethodInfo miClear = t.GetMethod("Clear", BindingFlags.Public |
					BindingFlags.Instance);
				miClear.Invoke(o, null);

				PropertyInfo piText = t.GetProperty("Text", BindingFlags.Public |
					BindingFlags.Instance);
				piText.SetValue(o, (str ?? string.Empty), null);

				// Prevent deadlock when pasting in own window
				MethodInfo miStore = t.GetMethod("Store", BindingFlags.Public |
					BindingFlags.Instance);
				miStore.Invoke(o, null);

				return true;
			}
			catch(Exception) { Debug.Assert(false); }

			return false;
		} */
	}
}

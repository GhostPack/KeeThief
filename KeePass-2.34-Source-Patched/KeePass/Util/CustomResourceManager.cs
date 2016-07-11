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
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

using KeePass.UI;
using KeePass.Util.Archive;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Util
{
	public sealed class CrmEventArgs : EventArgs
	{
		private readonly string m_strName;
		public string Name
		{
			get { return m_strName; }
		}

		private readonly CultureInfo m_ci;
		public CultureInfo CultureInfo
		{
			get { return m_ci; }
		}

		private object m_obj;
		public object Object
		{
			get { return m_obj; }
			set { m_obj = value; }
		}

		public CrmEventArgs(string strName, CultureInfo ci, object o)
		{
			if(strName == null) throw new ArgumentNullException("strName");

			m_strName = strName;
			m_ci = ci;
			m_obj = o;
		}
	}

	public sealed class CustomResourceManager : ResourceManager
	{
		private static List<CustomResourceManager> m_lInsts =
			new List<CustomResourceManager>();
		public static ReadOnlyCollection<CustomResourceManager> Instances
		{
			get { return m_lInsts.AsReadOnly(); }
		}

		public event EventHandler<CrmEventArgs> GetObjectPre;

		private readonly ResourceManager m_rm;
		public ResourceManager BaseResourceManager
		{
			get { return m_rm; }
		}

		private Dictionary<string, object> m_dOverrides =
			new Dictionary<string, object>();

		private ImageArchive m_iaAppHighRes = new ImageArchive();

		public CustomResourceManager(ResourceManager rmBase)
		{
			if(rmBase == null) throw new ArgumentNullException("rmBase");

			m_rm = rmBase;

			if(m_lInsts.Count < 1000) m_lInsts.Add(this);
			else { Debug.Assert(false); }

			try { m_iaAppHighRes.Load(Properties.Resources.Images_App_HighRes); }
			catch(Exception) { Debug.Assert(false); }
		}

		public override object GetObject(string name)
		{
			return GetObject(name, null);
		}

		public override object GetObject(string name, CultureInfo culture)
		{
			if(name == null) throw new ArgumentNullException("name");

			if(this.GetObjectPre != null)
			{
				CrmEventArgs e = new CrmEventArgs(name, culture, null);
				this.GetObjectPre(this, e);
				if(e.Object != null) return e.Object;
			}

			object oOvr;
			if(m_dOverrides.TryGetValue(name, out oOvr)) return oOvr;

			object o = m_rm.GetObject(name, culture);
			if(o == null) { Debug.Assert(false); return null; }

			try
			{
				Image img = (o as Image);
				if(img != null)
				{
					Debug.Assert(!(o is Icon));

					Image imgOvr = m_iaAppHighRes.GetForObject(name);
					if(imgOvr != null)
					{
						int wOvr = imgOvr.Width;
						int hOvr = imgOvr.Height;
						int wBase = img.Width;
						int hBase = img.Height;
						int wReq = DpiUtil.ScaleIntX(wBase);
						int hReq = DpiUtil.ScaleIntY(hBase);

						if((wBase > wOvr) || (hBase > hOvr))
						{
							Debug.Assert(false); // Base has higher resolution
							imgOvr = img;
							wOvr = wBase;
							hOvr = hBase;
						}

						if((wReq != wOvr) || (hReq != hOvr))
							imgOvr = GfxUtil.ScaleImage(imgOvr, wReq, hReq,
								ScaleTransformFlags.UIIcon);
					}
					else imgOvr = DpiUtil.ScaleImage(img, false);

					m_dOverrides[name] = imgOvr;
					return imgOvr;
				}
			}
			catch(Exception) { Debug.Assert(false); }

			return o;
		}

		public override string GetString(string name)
		{
			return m_rm.GetString(name);
		}

		public override string GetString(string name, CultureInfo culture)
		{
			return m_rm.GetString(name, culture);
		}

		public static void Override(Type tResClass)
		{
			try { OverridePriv(tResClass); }
			catch(Exception) { Debug.Assert(false); }
		}

		private static void OverridePriv(Type tResClass)
		{
			if(tResClass == null) { Debug.Assert(false); return; }
			if(Program.DesignMode) return;
			if(!DpiUtil.ScalingRequired) return;

			// Ensure ResourceManager instance
			PropertyInfo pi = tResClass.GetProperty("ResourceManager",
				(BindingFlags.NonPublic | BindingFlags.Static));
			if(pi == null) { Debug.Assert(false); return; }
			pi.GetValue(null, null);

			FieldInfo fi = tResClass.GetField("resourceMan",
				(BindingFlags.NonPublic | BindingFlags.Static));
			if(fi == null) { Debug.Assert(false); return; }

			ResourceManager rm = (fi.GetValue(null) as ResourceManager);
			if(rm == null) { Debug.Assert(false); return; }
			Debug.Assert(!(rm is CustomResourceManager)); // Override only once

			CustomResourceManager crm = new CustomResourceManager(rm);
			fi.SetValue(null, crm);
		}
	}
}

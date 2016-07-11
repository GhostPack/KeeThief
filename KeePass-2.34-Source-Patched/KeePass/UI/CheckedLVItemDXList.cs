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
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

using KeePass.App;
using KeePass.App.Configuration;
using KeePass.Util;

using KeePassLib.Utility;

namespace KeePass.UI
{
	public enum CheckItemLinkType
	{
		None = 0,
		CheckedChecked,
		UncheckedUnchecked,
		CheckedUnchecked,
		UncheckedChecked
	}

	public sealed class CheckedLVItemDXList
	{
		private ListView m_lv;

		private List<ClviInfo> m_lItems = new List<ClviInfo>();
		private List<CheckItemLink> m_lLinks = new List<CheckItemLink>();

		private bool m_bUseEnforcedConfig;

		private sealed class ClviInfo
		{
			private object m_o; // Never null
			public object Object { get { return m_o; } }

			// private string m_strPropName; // Never null
			// public string PropertyName { get { return m_strPropName; } }

			private PropertyInfo m_pi; // Never null
			public PropertyInfo PropertyInfo { get { return m_pi; } }

			private ListViewItem m_lvi; // Never null
			public ListViewItem ListViewItem { get { return m_lvi; } }

			public bool PropertyValue
			{
				get { return (bool)m_pi.GetValue(m_o, null); }
				set { m_pi.SetValue(m_o, value, null); }
			}

			private bool m_bReadOnly = false;
			public bool ReadOnly
			{
				get { return m_bReadOnly; }
				set { m_bReadOnly = value; }
			}

			public ClviInfo(object pContainer, string strPropertyName,
				ListViewItem lvi)
			{
				if(pContainer == null) throw new ArgumentNullException("pContainer");
				if(strPropertyName == null) throw new ArgumentNullException("strPropertyName");
				if(strPropertyName.Length == 0) throw new ArgumentException("strPropertyName");
				// if(lvi == null) throw new ArgumentNullException("lvi");

				m_o = pContainer;
				// m_strPropName = strPropertyName;
				m_lvi = lvi;

				Type t = pContainer.GetType();
				m_pi = t.GetProperty(strPropertyName);
				if((m_pi == null) || (m_pi.PropertyType != typeof(bool)) ||
					!m_pi.CanRead || !m_pi.CanWrite)
					throw new ArgumentException("strPropertyName");
			}
		}

		private sealed class CheckItemLink
		{
			private ListViewItem m_lvSource;
			public ListViewItem Source { get { return m_lvSource; } }

			private ListViewItem m_lvTarget;
			public ListViewItem Target { get { return m_lvTarget; } }

			private CheckItemLinkType m_t;
			public CheckItemLinkType Type { get { return m_t; } }

			public CheckItemLink(ListViewItem lviSource, ListViewItem lviTarget,
				CheckItemLinkType cilType)
			{
				m_lvSource = lviSource;
				m_lvTarget = lviTarget;
				m_t = cilType;
			}
		}

		[Obsolete]
		public CheckedLVItemDXList(ListView lv)
		{
			Construct(lv, false);
		}

		public CheckedLVItemDXList(ListView lv, bool bUseEnforcedConfig)
		{
			Construct(lv, bUseEnforcedConfig);
		}

		private void Construct(ListView lv, bool bUseEnforcedConfig)
		{
			if(lv == null) throw new ArgumentNullException("lv");

			m_lv = lv;
			m_bUseEnforcedConfig = bUseEnforcedConfig;

			m_lv.ItemChecked += this.OnItemCheckedChanged;
		}

#if DEBUG
		~CheckedLVItemDXList()
		{
			Debug.Assert(m_lv == null); // Release should have been called
		}
#endif

		public void Release()
		{
			if(m_lv == null) { Debug.Assert(false); return; }

			m_lItems.Clear();
			m_lLinks.Clear();

			m_lv.ItemChecked -= this.OnItemCheckedChanged;
			m_lv = null;
		}

		public void UpdateData(bool bGuiToInternals)
		{
			if(m_lv == null) { Debug.Assert(false); return; }

			Color clr = SystemColors.ControlText;
			float fH, fS, fV;
			UIUtil.ColorToHsv(clr, out fH, out fS, out fV);
			if(fV >= 0.5f) // Text color is rather light
				clr = UIUtil.ColorFromHsv(fH, 0.0f, 0.40f);
			else // Text color is rather dark
				clr = UIUtil.ColorFromHsv(fH, 0.0f, 0.60f);

			foreach(ClviInfo clvi in m_lItems)
			{
				ListViewItem lvi = clvi.ListViewItem;

				Debug.Assert(lvi.Index >= 0);
				Debug.Assert(m_lv.Items.IndexOf(lvi) >= 0);
				if(bGuiToInternals)
				{
					bool bChecked = lvi.Checked;
					clvi.PropertyValue = bChecked;
				}
				else // Internals to GUI
				{
					bool bValue = clvi.PropertyValue;
					lvi.Checked = bValue;

					if(clvi.ReadOnly) lvi.ForeColor = clr;
				}
			}
		}

		public ListViewItem CreateItem(object pContainer, string strPropertyName,
			ListViewGroup lvgContainer, string strDisplayString)
		{
			return CreateItem(pContainer, strPropertyName, lvgContainer,
				strDisplayString, null);
		}

		public ListViewItem CreateItem(object pContainer, string strPropertyName,
			ListViewGroup lvgContainer, string strDisplayString, bool? obReadOnly)
		{
			if(pContainer == null) throw new ArgumentNullException("pContainer");
			if(strPropertyName == null) throw new ArgumentNullException("strPropertyName");
			if(strPropertyName.Length == 0) throw new ArgumentException("strPropertyName");
			if(strDisplayString == null) throw new ArgumentNullException("strDisplayString");

			if(m_lv == null) { Debug.Assert(false); return null; }

			ListViewItem lvi = new ListViewItem(strDisplayString);
			ClviInfo clvi = new ClviInfo(pContainer, strPropertyName, lvi);

			if(obReadOnly.HasValue) clvi.ReadOnly = obReadOnly.Value;
			else DetermineReadOnlyState(clvi);

			if(lvgContainer != null)
			{
				lvi.Group = lvgContainer;
				Debug.Assert(lvgContainer.Items.IndexOf(lvi) >= 0);
				Debug.Assert(m_lv.Groups.IndexOf(lvgContainer) >= 0);
			}

			m_lv.Items.Add(lvi);
			m_lItems.Add(clvi);

			return lvi;
		}

		public void AddLink(ListViewItem lviSource, ListViewItem lviTarget,
			CheckItemLinkType t)
		{
			if(lviSource == null) { Debug.Assert(false); return; }
			if(lviTarget == null) { Debug.Assert(false); return; }

			if(m_lv == null) { Debug.Assert(false); return; }

			Debug.Assert(GetItem(lviSource) != null);
			Debug.Assert(GetItem(lviTarget) != null);

			m_lLinks.Add(new CheckItemLink(lviSource, lviTarget, t));
		}

		private ClviInfo GetItem(ListViewItem lvi)
		{
			if(lvi == null) { Debug.Assert(false); return null; }

			foreach(ClviInfo clvi in m_lItems)
			{
				if(clvi.ListViewItem == lvi) return clvi;
			}

			return null;
		}

		private void OnItemCheckedChanged(object sender, ItemCheckedEventArgs e)
		{
			ListViewItem lvi = e.Item;
			if(lvi == null) { Debug.Assert(false); return; }

			bool bChecked = lvi.Checked;

			ClviInfo clvi = GetItem(lvi);
			if(clvi != null)
			{
				if(clvi.ReadOnly && (bChecked != clvi.PropertyValue))
				{
					lvi.Checked = clvi.PropertyValue;
					return;
				}
			}

			foreach(CheckItemLink cl in m_lLinks)
			{
				if(cl.Source == lvi)
				{
					if(cl.Target.Index < 0) continue;

					if((cl.Type == CheckItemLinkType.CheckedChecked) &&
						bChecked && !cl.Target.Checked)
						cl.Target.Checked = true;
					else if((cl.Type == CheckItemLinkType.UncheckedUnchecked) &&
						!bChecked && cl.Target.Checked)
						cl.Target.Checked = false;
					else if((cl.Type == CheckItemLinkType.CheckedUnchecked) &&
						bChecked && cl.Target.Checked)
						cl.Target.Checked = false;
					else if((cl.Type == CheckItemLinkType.UncheckedChecked) &&
						!bChecked && !cl.Target.Checked)
						cl.Target.Checked = true;
				}
			}
		}

		private void DetermineReadOnlyState(ClviInfo clvi)
		{
			if(clvi == null) { Debug.Assert(false); return; }

			if(!m_bUseEnforcedConfig) clvi.ReadOnly = false;
			else
				clvi.ReadOnly = AppConfigEx.IsOptionEnforced(clvi.Object,
					clvi.PropertyInfo);
		}
	}
}

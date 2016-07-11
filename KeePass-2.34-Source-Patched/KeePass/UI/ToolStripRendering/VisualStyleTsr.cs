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

/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing;
using System.Diagnostics;

// Parts and States:
//   http://msdn.microsoft.com/en-us/library/bb773210.aspx
// How to Implement a Custom ToolStripRenderer:
//   http://msdn.microsoft.com/en-us/library/ms229720.aspx

namespace KeePass.UI.ToolStripRendering
{
	public sealed class VisualStyleTsr : ProExtTsr
	{
		private const string VsClassMenu = "MENU";
		private const string VsClassToolBar = "TOOLBAR";
		private const string VsClassReBar = "REBAR";

		private enum VsMenuPart : int
		{
			MenuItemTmSchema = 1,
			MenuDropDownTmSchema = 2,
			MenuBarItemTmSchema = 3,
			MenuBarDropDownTmSchema = 4,
			ChevronTmSchema = 5,
			SeparatorTmSchema = 6,
			BarBackground = 7,
			BarItem = 8,
			PopupBackground = 9,
			PopupBorders = 10,
			PopupCheck = 11,
			PopupCheckBackground = 12,
			PopupGutter = 13,
			PopupItem = 14,
			PopupSeparator = 15,
			PopupSubMenu = 16,
			SystemClose = 17,
			SystemMaximize = 18,
			SystemMinimize = 19,
			SystemRestore = 20
		}

		private enum VsMenuState : int
		{
			Default = 0,

			MbActive = 1,
			MbInactive = 2,

			MbiNormal = 1,
			MbiHot = 2,
			MbiPushed = 3,
			MbiDisabled = 4,
			MbiDisabledHot = 5,
			MbiDisabledPushed = 6,

			McCheckMarkNormal = 1,
			McCheckMarkDisabled = 2,
			McBulletNormal = 3,
			McBulletDisabled = 4,

			McbDisabled = 1,
			McbNormal = 2,
			McbBitmap = 3,

			MpiNormal = 1,
			MpiHot = 2,
			MpiDisabled = 3,
			MpiDisabledHot = 4,

			MsmNormal = 1,
			MsmDisabled = 2
		};

		private enum VsReBarPart : int
		{
			Gripper = 1,
			GripperVert = 2,
			Band = 3,
			Chevron = 4,
			ChevronVert = 5,
			Background = 6,
			Splitter = 7,
			SplitterVert = 8
		}

		private VisualStyleRenderer m_r = null;

		private bool IsSupported
		{
			get { return (m_r != null); }
		}

		public VisualStyleTsr() : base()
		{
			try
			{
				if(VisualStyleRenderer.IsSupported)
					m_r = new VisualStyleRenderer(VsClassMenu,
						(int)VsMenuPart.BarBackground, (int)VsMenuState.MbActive);
			}
			catch(Exception) { }
		}

		private void DrawBackgroundEx(Graphics g, Rectangle rect, Control c,
			bool bCanPaintParent)
		{
			if(bCanPaintParent && m_r.IsBackgroundPartiallyTransparent() &&
				(c != null))
				m_r.DrawParentBackground(g, rect, c);

			m_r.DrawBackground(g, rect);

#if DEBUG
			Color clr = Color.Red;
			string str = Environment.StackTrace;
			if(str.IndexOf("OnRenderMenuItemBackground") >= 0) clr = Color.Green;
			if(str.IndexOf("OnRenderToolStripBackground") >= 0) clr = Color.Blue;
			if(str.IndexOf("OnRenderToolStripBorder") >= 0) clr = Color.Pink;
			using(SolidBrush br = new SolidBrush(clr))
			{
				using(Pen p = new Pen(UIUtil.LightenColor(clr, 0.5)))
				{
					g.FillRectangle(br, rect);
					g.DrawRectangle(p, rect);
				}
			}
#endif
		}

		private void ConfigureForItem(ToolStripItem tsi)
		{
			bool bEnabled = tsi.Enabled;
			bool bPressed = tsi.Pressed;
			bool bHot = tsi.Selected;

			string c = VsClassMenu;
			if(tsi.IsOnDropDown)
			{
				const int p = (int)VsMenuPart.PopupItem;

				if(bEnabled)
					m_r.SetParameters(c, p, (int)(bHot ? VsMenuState.MpiHot :
						VsMenuState.MpiNormal));
				else
					m_r.SetParameters(c, p, (int)(bHot ? VsMenuState.MpiDisabledHot :
						VsMenuState.MpiDisabled));
			}
			else
			{
				const int p = (int)VsMenuPart.BarItem;

				if(tsi.Pressed)
					m_r.SetParameters(c, p, (int)(bEnabled ? VsMenuState.MbiPushed :
						VsMenuState.MbiDisabledPushed));
				else if(bEnabled)
					m_r.SetParameters(c, p, (int)(bHot ? VsMenuState.MbiHot :
						VsMenuState.MbiNormal));
				else
					m_r.SetParameters(c, p, (int)(bHot ? VsMenuState.MbiDisabledHot :
						VsMenuState.MbiDisabled));
			}
		}

		private delegate bool TsrBoolDelegate();
		private delegate void TsrFuncDelegate<T>(T e);
		private void RenderOrBase<T>(T e, TsrBoolDelegate f, TsrFuncDelegate<T> fBase)
		{
			bool bBase = true;
			try
			{
				if(this.IsSupported) bBase = !f();
			}
			catch(Exception) { Debug.Assert(false); }

			if(bBase) fBase(e);
		}

		private Rectangle GetBackgroundRect(Graphics g, ToolStripItem tsi)
		{
			if(!tsi.IsOnDropDown)
				return new Rectangle(0, 0, tsi.Width, tsi.Height - 1);

			Rectangle rect = new Rectangle(Point.Empty, tsi.Size);
			Size szBorder = GetPopupBorderSize(g);
			rect.Inflate(-szBorder.Width, 0);
			rect.X += 1;
			rect.Width -= 1;

			return rect;
		}

		private Size GetPopupBorderSize(Graphics g)
		{
			Size sz = m_r.GetPartSize(g, ThemeSizeType.Minimum);
			if(sz.Width < 1) { Debug.Assert(false); sz.Width = 1; }
			if(sz.Height < 1) { Debug.Assert(false); sz.Height = 1; }
			return sz;
		}

		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			TsrBoolDelegate f = delegate()
			{
				ConfigureForItem(e.Item);
				DrawBackgroundEx(e.Graphics, GetBackgroundRect(e.Graphics,
					e.Item), e.ToolStrip, false);
				return true;
			};

			RenderOrBase(e, f, base.OnRenderMenuItemBackground);
		}

		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			TsrBoolDelegate f = delegate()
			{
				Debug.Assert(e.AffectedBounds == e.ToolStrip.ClientRectangle);

				VisualStyleElement vse = VisualStyleElement.CreateElement(VsClassMenu,
					(int)VsMenuPart.BarBackground, (int)VsMenuState.MbInactive);
				if(!VisualStyleRenderer.IsElementDefined(vse))
					vse = VisualStyleElement.CreateElement(VsClassMenu,
						(int)VsMenuPart.BarBackground, 0);
				m_r.SetParameters(vse);

				DrawBackgroundEx(e.Graphics, e.ToolStrip.ClientRectangle,
					e.ToolStrip, true);

				return true;
			};

			RenderOrBase(e, f, base.OnRenderToolStripBackground);
		}

		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			TsrBoolDelegate f = delegate()
			{
				Debug.Assert(e.AffectedBounds == e.ToolStrip.ClientRectangle);

				m_r.SetParameters(VsClassMenu, (int)VsMenuPart.PopupBorders, 0);

				Graphics g = e.Graphics;

				Rectangle rectClient = e.ToolStrip.ClientRectangle;
				Rectangle rect = rectClient;
				Size szBorder = GetPopupBorderSize(e.Graphics);
				rect.Inflate(-szBorder.Width, -szBorder.Height);

				Region rgOrgClip = g.Clip.Clone();
				g.ExcludeClip(rect);

				DrawBackgroundEx(g, rectClient, e.ToolStrip, false);

				g.Clip = rgOrgClip;
				return true;
			};

			RenderOrBase(e, f, base.OnRenderToolStripBorder);
		}
	}
}
*/

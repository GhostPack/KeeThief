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
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

using KeePassLib.Native;

namespace KeePass.UI
{
	public sealed class CustomSplitContainerEx : SplitContainer
	{
		private ControlCollection m_ccControls = null;
		private Control m_cDefault = null;

		private Control m_cFocused = null;
		private Control m_cLastKnown = null;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double SplitterDistanceFrac
		{
			get
			{
				bool bVert = (this.Orientation == Orientation.Vertical);

				int m = (bVert ? this.Width : this.Height);
				if(m <= 0) { Debug.Assert(false); return 0.0; }

				int d = this.SplitterDistance;
				if(d < 0) { Debug.Assert(false); return 0.0; }
				if(d == 0) return 0.0; // Avoid fExact infinity

				double f = (double)d / (double)m;

				try
				{
					FieldInfo fi = GetRatioField(bVert);
					if(fi != null)
					{
						double fExact = (double)fi.GetValue(this);
						if(fExact > double.Epsilon)
						{
							fExact = 1.0 / fExact;

							// Test whether fExact makes sense and if so,
							// use it instead of f; 1/m as boundary is
							// slightly too strict
							if(Math.Abs(fExact - f) <= (1.5 / (double)m))
								return fExact;
							else { Debug.Assert(false); }
						}
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}
				catch(Exception) { Debug.Assert(false); }

				return f;
			}

			set
			{
				if((value < 0.0) || (value > 1.0)) { Debug.Assert(false); return; }

				bool bVert = (this.Orientation == Orientation.Vertical);

				int m = (bVert ? this.Width : this.Height);
				if(m <= 0) { Debug.Assert(false); return; }

				int d = (int)Math.Round(value * (double)m);
				if(d < 0) { Debug.Assert(false); d = 0; }
				if(d > m) { Debug.Assert(false); d = m; }

				this.SplitterDistance = d;
				if(d == 0) return; // Avoid infinity / division by zero

				// If the position was auto-adjusted (e.g. due to
				// minimum size constraints), skip the rest
				if(this.SplitterDistance != d) return;

				try
				{
					FieldInfo fi = GetRatioField(bVert);
					if(fi != null)
					{
						double fEst = (double)fi.GetValue(this);
						if(fEst <= double.Epsilon) { Debug.Assert(false); return; }
						fEst = 1.0 / fEst; // m/d -> d/m

						// Test whether fEst makes sense and if so,
						// overwrite it with the exact value;
						// we must test for 1.5/m, not 1/m, because .NET
						// uses Math.Floor and we use Math.Round
						if(Math.Abs(fEst - value) <= (1.5 / (double)m))
							fi.SetValue(this, 1.0 / value); // d/m -> m/d
						else { Debug.Assert(false); }
					}
					else { Debug.Assert(false); }
				}
				catch(Exception) { Debug.Assert(false); }
			}
		}

		public CustomSplitContainerEx() : base()
		{
		}

		public void InitEx(ControlCollection cc, Control cDefault)
		{
			m_ccControls = cc;
			m_cDefault = m_cLastKnown = cDefault;
		}

		private static Control FindInputFocus(ControlCollection cc)
		{
			if(cc == null) { Debug.Assert(false); return null; }

			foreach(Control c in cc)
			{
				if(c.Focused)
					return c;
				else if(c.ContainsFocus)
					return FindInputFocus(c.Controls);
			}

			return null;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_cFocused = FindInputFocus(m_ccControls);
			if(m_cFocused == null) m_cFocused = m_cDefault;

			if(m_cFocused != null) m_cLastKnown = m_cFocused;

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if(m_cFocused != null)
			{
				UIUtil.SetFocus(m_cFocused, null);
				m_cFocused = null;
			}
			else { Debug.Assert(false); }
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);

			if(this.Focused && (m_cFocused == null))
			{
				if(m_cLastKnown != null) UIUtil.SetFocus(m_cLastKnown, null);
				else if(m_cDefault != null) UIUtil.SetFocus(m_cDefault, null);
			}
		}

		private static FieldInfo GetRatioField(bool bVert)
		{
			// Both .NET and Mono store 'max/pos', not 'pos/max'
			return typeof(SplitContainer).GetField(
				(NativeLib.IsUnix() ? "fixed_none_ratio" :
				(bVert ? "ratioWidth" : "ratioHeight")),
				(BindingFlags.Instance | BindingFlags.NonPublic));
		}
	}
}

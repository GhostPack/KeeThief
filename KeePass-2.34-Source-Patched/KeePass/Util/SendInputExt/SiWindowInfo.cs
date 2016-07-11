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

namespace KeePass.Util.SendInputExt
{
	internal enum SiSendMethod
	{
		Default = 0,
		KeyEvent,
		UnicodePacket // VK_PACKET via SendInput
	}

	internal sealed class SiWindowInfo
	{
		private readonly IntPtr m_hWnd;
		public IntPtr HWnd
		{
			get { return m_hWnd; }
		}

		private IntPtr m_hkl = IntPtr.Zero;
		public IntPtr KeyboardLayout
		{
			get { return m_hkl; }
			set { m_hkl = value; }
		}

		private SiSendMethod m_sm = SiSendMethod.Default;
		public SiSendMethod SendMethod
		{
			get { return m_sm; }
			set { m_sm = value; }
		}

		public SiWindowInfo(IntPtr hWnd)
		{
			m_hWnd = hWnd;
		}
	}
}

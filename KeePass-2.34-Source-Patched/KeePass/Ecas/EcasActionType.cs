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

using KeePassLib;

namespace KeePass.Ecas
{
	public delegate void EcasActionExecute(EcasAction a, EcasContext ctx);

	public sealed class EcasActionType : IEcasParameterized
	{
		private PwUuid m_type;
		public PwUuid Type
		{
			get { return m_type; }
		}

		private string m_strName;
		public string Name
		{
			get { return m_strName; }
		}

		private PwIcon m_pwIcon;
		public PwIcon Icon
		{
			get { return m_pwIcon; }
		}

		private EcasParameter[] m_vParams;
		public EcasParameter[] Parameters
		{
			get { return m_vParams; }
		}

		private EcasActionExecute m_fn;
		public EcasActionExecute ExecuteMethod
		{
			get { return m_fn; }
		}

		private static void EcasActionExecuteNull(EcasAction a, EcasContext ctx)
		{
		}

		public EcasActionType(PwUuid uuidType, string strName, PwIcon pwIcon,
			EcasParameter[] vParams, EcasActionExecute f)
		{
			if((uuidType == null) || PwUuid.Zero.Equals(uuidType))
				throw new ArgumentNullException("uuidType");
			if(strName == null) throw new ArgumentNullException("strName");
			// if(vParams == null) throw new ArgumentNullException("vParams");
			// if(f == null) throw new ArgumentNullException("f");

			m_type = uuidType;
			m_strName = strName;
			m_pwIcon = pwIcon;
			m_vParams = (vParams ?? EcasParameter.EmptyArray);
			m_fn = (f ?? EcasActionExecuteNull);
		}
	}
}

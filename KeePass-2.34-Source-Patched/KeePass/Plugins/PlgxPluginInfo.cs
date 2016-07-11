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
using System.Diagnostics;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Utility;

namespace KeePass.Plugins
{
	public enum PlgxProjectType
	{
		CSharp,
		VisualBasic
	}

	public sealed class PlgxPluginInfo
	{
		public bool Compiling { get; set; }
		public bool AllowCached { get; set; }
		public bool AllowCompile { get; set; }

		private TextWriter m_twLog = null;
		public TextWriter LogStream
		{
			get { return m_twLog; }
			set { m_twLog = value; }
		}

		private PlgxProjectType m_pt = PlgxProjectType.CSharp;
		public PlgxProjectType ProjectType
		{
			get { return m_pt; }
			set { m_pt = value; }
		}

		private PwUuid m_uuid = PwUuid.Zero;
		public PwUuid FileUuid
		{
			get { return m_uuid; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_uuid = value;
			}
		}

		private string m_strBaseFileName = string.Empty;
		public string BaseFileName
		{
			get { return m_strBaseFileName; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strBaseFileName = value;
			}
		}

		private string m_strCsproj = string.Empty;
		public string CsprojFilePath
		{
			get { return m_strCsproj; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_strCsproj = value;
			}
		}

		private CompilerParameters m_cp = new CompilerParameters();
		public CompilerParameters CompilerParameters
		{
			get { return m_cp; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_cp = value;
			}
		}

		private List<string> m_vFiles = new List<string>();
		public List<string> SourceFiles
		{
			get { return m_vFiles; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vFiles = value;
			}
		}

		private List<string> m_vIncRefAsms = new List<string>();
		public List<string> IncludedReferencedAssemblies
		{
			get { return m_vIncRefAsms; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vIncRefAsms = value;
			}
		}

		private List<string> m_vEmbeddedRes = new List<string>();
		public List<string> EmbeddedResourceSources
		{
			get { return m_vEmbeddedRes; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vEmbeddedRes = value;
			}
		}

		private List<string> m_vVbImports = new List<string>();
		public List<string> VbImports
		{
			get { return m_vVbImports; }
			set
			{
				if(value == null) throw new ArgumentNullException("value");
				m_vVbImports = value;
			}
		}

		public PlgxPluginInfo(bool bCompiling, bool bAllowCached, bool bAllowCompile)
		{
			this.Compiling = bCompiling;
			this.AllowCached = bAllowCached;
			this.AllowCompile = bAllowCompile;
		}

		public string GetAbsPath(string strRelPath)
		{
			Debug.Assert(!string.IsNullOrEmpty(this.CsprojFilePath));
			return UrlUtil.MakeAbsolutePath(this.CsprojFilePath,
				UrlUtil.ConvertSeparators(strRelPath));
		}
	}
}

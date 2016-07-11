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
using System.Xml;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;

using KeePass.Resources;

using KeePassLib;
using KeePassLib.Resources;
using KeePassLib.Utility;

namespace KeePass.Plugins
{
	public static class PlgxCsprojLoader
	{
		private const string XnnProject = "Project";
		private const string XnnPropertyGroup = "PropertyGroup";
		private const string XnnItemGroup = "ItemGroup";

		private const string XnnAssemblyName = "AssemblyName";
		private const string XnnEmbeddedRes = "EmbeddedResource";
		private const string XnnInclude = "Include";
		private const string XnnReference = "Reference";
		private const string XnnCompile = "Compile";
		private const string XnnHintPath = "HintPath";
		private const string XnnImport = "Import";

		public static void LoadDefault(string strDirPath, PlgxPluginInfo plgxOutInfo)
		{
			if(plgxOutInfo == null) throw new ArgumentNullException("plgxOutInfo");

			string[] vCsproj = UrlUtil.GetFilePaths(strDirPath, "*.csproj",
				SearchOption.AllDirectories).ToArray();
			if(vCsproj.Length == 1)
			{
				plgxOutInfo.ProjectType = PlgxProjectType.CSharp;
				PlgxCsprojLoader.Load(vCsproj[0], plgxOutInfo);
				return;
			}

			// string[] vVbproj = UrlUtil.GetFilePaths(strDirPath, "*.vbproj",
			//	SearchOption.AllDirectories).ToArray();
			// if(vVbproj.Length == 1)
			// {
			//	plgxOutInfo.ProjectType = PlgxProjectType.VisualBasic;
			//	PlgxCsprojLoader.Load(vVbproj[0], plgxOutInfo);
			//	return;
			// }

			throw new InvalidOperationException(KPRes.CsprojCountError);
		}

		private static void Load(string strFilePath, PlgxPluginInfo plgxOutInfo)
		{
			if(strFilePath == null) throw new ArgumentNullException("strFilePath");

			plgxOutInfo.CsprojFilePath = strFilePath;

			XmlDocument doc = new XmlDocument();
			doc.Load(strFilePath);

			ReadProject(doc.DocumentElement, plgxOutInfo);
		}

		private static void ReadProject(XmlNode xn, PlgxPluginInfo plgx)
		{
			if(xn.Name != XnnProject) throw new Exception(KLRes.FileCorrupted);

			foreach(XmlNode xnChild in xn.ChildNodes)
			{
				if(xnChild.Name == XnnPropertyGroup) ReadPropertyGroup(xnChild, plgx);
				else if(xnChild.Name == XnnItemGroup) ReadItemGroup(xnChild, plgx);
			}
		}

		private static void ReadPropertyGroup(XmlNode xn, PlgxPluginInfo plgx)
		{
			foreach(XmlNode xnChild in xn.ChildNodes)
			{
				if(xnChild.Name == XnnAssemblyName)
					plgx.BaseFileName = xnChild.InnerText;
			}
		}

		private static void ReadItemGroup(XmlNode xn, PlgxPluginInfo plgx)
		{
			foreach(XmlNode xnChild in xn.ChildNodes)
			{
				if(xnChild.Name == XnnEmbeddedRes) ReadEmbeddedRes(xnChild, plgx);
				else if(xnChild.Name == XnnReference) ReadReference(xnChild, plgx);
				else if(xnChild.Name == XnnCompile) ReadCompile(xnChild, plgx);
				else if(xnChild.Name == XnnImport) ReadImport(xnChild, plgx);
			}
		}

		private static void ReadEmbeddedRes(XmlNode xn, PlgxPluginInfo plgx)
		{
			XmlNode xnInc = xn.Attributes.GetNamedItem(XnnInclude);
			if((xnInc == null) || string.IsNullOrEmpty(xnInc.Value)) { Debug.Assert(false); return; }

			string strResSrc = plgx.GetAbsPath(xnInc.Value); // Converts separators
			plgx.EmbeddedResourceSources.Add(strResSrc);
		}

		private static void ReadReference(XmlNode xn, PlgxPluginInfo plgx)
		{
			XmlNode xnInc = xn.Attributes.GetNamedItem(XnnInclude);
			if((xnInc == null) || string.IsNullOrEmpty(xnInc.Value)) { Debug.Assert(false); return; }
			string str = xnInc.Value;

			if(UrlUtil.AssemblyEquals(str, PwDefs.ShortProductName))
				return; // Ignore KeePass references

			foreach(XmlNode xnSub in xn.ChildNodes)
			{
				if(xnSub.Name == XnnHintPath)
				{
					plgx.IncludedReferencedAssemblies.Add(
						UrlUtil.ConvertSeparators(xnSub.InnerText, '/'));
					return;
				}
			}

			if(!str.EndsWith(".dll", StrUtil.CaseIgnoreCmp)) str += ".dll";

			plgx.CompilerParameters.ReferencedAssemblies.Add(str);
		}

		private static void ReadCompile(XmlNode xn, PlgxPluginInfo plgx)
		{
			XmlNode xnInc = xn.Attributes.GetNamedItem(XnnInclude);
			if((xnInc == null) || string.IsNullOrEmpty(xnInc.Value)) { Debug.Assert(false); return; }

			plgx.SourceFiles.Add(plgx.GetAbsPath(xnInc.Value)); // Converts separators
		}

		private static void ReadImport(XmlNode xn, PlgxPluginInfo plgx)
		{
			if(plgx.ProjectType != PlgxProjectType.VisualBasic) { Debug.Assert(false); return; }

			XmlNode xnInc = xn.Attributes.GetNamedItem(XnnInclude);
			if((xnInc == null) || string.IsNullOrEmpty(xnInc.Value)) { Debug.Assert(false); return; }

			plgx.VbImports.Add(UrlUtil.ConvertSeparators(xnInc.Value));
		}
	}
}

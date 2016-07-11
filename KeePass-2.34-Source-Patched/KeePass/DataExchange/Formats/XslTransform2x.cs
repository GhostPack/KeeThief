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
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using KeePass.App;
using KeePass.Resources;
using KeePass.UI;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Utility;

namespace KeePass.DataExchange.Formats
{
	internal sealed class XslTransform2x : FileFormatProvider
	{
		private const string ParamXslFile = "XslFile";

		public override bool SupportsImport { get { return false; } }
		public override bool SupportsExport { get { return true; } }

		public override string FormatName { get { return KPRes.XslExporter; } }
		public override string ApplicationGroup { get { return KPRes.General; } }

		public override Image SmallIcon
		{
			get { return KeePass.Properties.Resources.B16x16_CompFile; }
		}

		public override bool Export(PwExportInfo pwExportInfo, Stream sOutput,
			IStatusLogger slLogger)
		{
			string strXslFile;
			pwExportInfo.Parameters.TryGetValue(ParamXslFile, out strXslFile);

			if(string.IsNullOrEmpty(strXslFile))
				strXslFile = UIGetXslFile();
			if(string.IsNullOrEmpty(strXslFile))
				return false;

			return ExportEx(pwExportInfo, sOutput, slLogger, strXslFile);
		}

		private static string UIGetXslFile()
		{
			string strFilter = UIUtil.CreateFileTypeFilter("xsl", KPRes.XslFileType, true);
			OpenFileDialogEx dlgXsl = UIUtil.CreateOpenFileDialog(KPRes.XslSelectFile,
				strFilter, 1, "xsl", false, AppDefs.FileDialogContext.Xsl);

			if(dlgXsl.ShowDialog() != DialogResult.OK) return null;

			return dlgXsl.FileName;
		}

		private bool ExportEx(PwExportInfo pwExportInfo, Stream sOutput,
			IStatusLogger slLogger, string strXslFile)
		{
			XslCompiledTransform xsl = new XslCompiledTransform();

			try { xsl.Load(strXslFile); }
			catch(Exception exXsl)
			{
				throw new NotSupportedException(strXslFile + MessageService.NewParagraph +
					KPRes.NoXslFile + MessageService.NewParagraph + exXsl.Message);
			}

			MemoryStream msDataXml = new MemoryStream();

			PwDatabase pd = (pwExportInfo.ContextDatabase ?? new PwDatabase());
			KdbxFile kdb = new KdbxFile(pd);
			kdb.Save(msDataXml, pwExportInfo.DataGroup, KdbxFormat.PlainXml, slLogger);

			byte[] pbData = msDataXml.ToArray();
			msDataXml.Close();
			MemoryStream msDataRead = new MemoryStream(pbData, false);
			XmlReader xmlDataReader = XmlReader.Create(msDataRead);

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.CheckCharacters = false;
			xws.Encoding = new UTF8Encoding(false);
			xws.NewLineChars = MessageService.NewLine;
			xws.NewLineHandling = NewLineHandling.None;
			xws.OmitXmlDeclaration = true;
			xws.ConformanceLevel = ConformanceLevel.Auto;

			XmlWriter xmlWriter = XmlWriter.Create(sOutput, xws);
			xsl.Transform(xmlDataReader, xmlWriter);
			xmlWriter.Close();
			xmlDataReader.Close();
			msDataRead.Close();

			Array.Clear(pbData, 0, pbData.Length);
			return true;
		}
	}
}

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
using System.Net;
using System.IO;
using System.IO.Compression;

using KeePassLib.Utility;

namespace KeePass.Util
{
	public static class NetUtil
	{
		/* public static string GZipUtf8ResultToString(DownloadDataCompletedEventArgs e)
		{
			if(e.Cancelled || (e.Error != null) || (e.Result == null))
				return null;

			MemoryStream msZipped = new MemoryStream(e.Result);
			GZipStream gz = new GZipStream(msZipped, CompressionMode.Decompress);
			BinaryReader br = new BinaryReader(gz);
			MemoryStream msUTF8 = new MemoryStream();

			while(true)
			{
				byte[] pb = null;

				try { pb = br.ReadBytes(4096); }
				catch(Exception) { }

				if((pb == null) || (pb.Length == 0)) break;

				msUTF8.Write(pb, 0, pb.Length);
			}

			br.Close();
			gz.Close();
			msZipped.Close();

			return StrUtil.Utf8.GetString(msUTF8.ToArray());
		} */

		public static string WebPageLogin(Uri url, string strPostData,
			out List<KeyValuePair<string, string>> vCookies)
		{
			if(url == null) throw new ArgumentNullException("url");

			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);

			byte[] pbPostData = Encoding.ASCII.GetBytes(strPostData);

			hwr.Method = "POST";
			hwr.ContentType = "application/x-www-form-urlencoded";
			hwr.ContentLength = pbPostData.Length;
			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0)";

			Stream s = hwr.GetRequestStream();
			s.Write(pbPostData, 0, pbPostData.Length);
			s.Close();

			WebResponse wr = hwr.GetResponse();

			StreamReader sr = new StreamReader(wr.GetResponseStream());
			string strResponse = sr.ReadToEnd();
			sr.Close();
			wr.Close();

			vCookies = new List<KeyValuePair<string, string>>();
			foreach(string strHeader in wr.Headers.AllKeys)
			{
				if(strHeader == "Set-Cookie")
				{
					string strCookie = wr.Headers.Get(strHeader);
					string[] vParts = strCookie.Split(new char[]{ ';' });
					if(vParts.Length < 1) continue;

					string[] vInfo = vParts[0].Split(new char[]{ '=' });
					if(vInfo.Length != 2) continue;

					vCookies.Add(new KeyValuePair<string, string>(
						vInfo[0], vInfo[1]));
				}
			}

			return strResponse;
		}

		public static string WebPageGetWithCookies(Uri url,
			List<KeyValuePair<string, string>> vCookies, string strDomain)
		{
			if(url == null) throw new ArgumentNullException("url");

			HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);

			hwr.Method = "GET";
			hwr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0)";

			if(vCookies != null)
			{
				hwr.CookieContainer = new CookieContainer();

				foreach(KeyValuePair<string, string> kvpCookie in vCookies)
				{
					Cookie ck = new Cookie(kvpCookie.Key, kvpCookie.Value,
						"/", strDomain);
					hwr.CookieContainer.Add(ck);
				}
			}

			WebResponse wr = hwr.GetResponse();

			StreamReader sr = new StreamReader(wr.GetResponseStream());
			string strResponse = sr.ReadToEnd();
			sr.Close();
			wr.Close();

			return strResponse;
		}
	}
}

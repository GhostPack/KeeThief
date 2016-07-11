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
using System.Diagnostics;

namespace KeePass.Util
{
	public static class TextSimilarity
	{
		public static int LevenshteinDistance(char[] s, char[] t)
		{
			Debug.Assert(s != null);
			if(s == null) throw new ArgumentNullException("s");
			Debug.Assert(t != null);
			if(t == null) throw new ArgumentNullException("t");

			int n = s.Length, m = t.Length;
			if(n <= 0) return m;
			if(m <= 0) return n;

			int[,] d = new int[n + 1, m + 1];

			for(int k = 0; k <= n; ++k) d[k, 0] = k;
			for(int l = 0; l <= m; ++l) d[0, l] = l;

			for(int i = 1; i <= n; ++i)
			{
				char s_i = s[i - 1];

				for(int j = 1; j <= m; ++j)
				{
					int nCost = ((s_i == t[j - 1]) ? 0 : 1);

					// Insertion, deletion and substitution
					d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(
						d[i, j - 1] + 1, d[i - 1, j - 1] + nCost));
				}
			}

			return d[n, m];
		}
	}
}
*/

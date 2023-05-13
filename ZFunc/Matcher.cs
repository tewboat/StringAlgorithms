namespace task_Zfunc
{
	class Matcher
	{
		public static int[] CountPrefixMatches<TChar>(IList<TChar> pattern, IList<TChar> text)
		{
			int m = pattern.Count, n = text.Count;
			var prefixMatchCount = new int[m];
			var z = Zfunction(pattern);

			int rightBound = -1000, offset = -1;

			for (var i = 0; i < n; i++)
			{
				var cnt = 0;
				if (offset < 0 || i + z[offset] > rightBound)
				{
					cnt = Math.Max(0, rightBound - i + 1);
					while (i + cnt < n && cnt < m && Equal(pattern[cnt], text[i + cnt]))
						cnt++;
				}
				else
				{
					cnt = z[offset++];
				}
				if (i + cnt - 1 >= rightBound)
				{
					offset = 1;
					rightBound = i + cnt - 1;
				}
				
				if (cnt > 0)
					prefixMatchCount[cnt - 1]++;
			}

			for (var i = prefixMatchCount.Length - 1; i > 0; i--)
				prefixMatchCount[i - 1] += prefixMatchCount[i];
			
			return prefixMatchCount;
		}

		// То же что CountPrefixMatches, но работает за O(|text| * |text|).
		public static int[] NaiveCountPrefixMatches<TChar>(IList<TChar> pattern, IList<TChar> text)
		{
			var prefixMatchCount = new int[pattern.Count];
			for (int i = 0; i < text.Count; i++)
			{
				int match = text.Skip(i)  //Skip работает за O(n) - очень медленно
					.TakeWhile((c, j) => j < pattern.Count && Equal(pattern[j], c)).Count();
				if (match > 0)
					prefixMatchCount[match - 1]++;
			}
			for (int i = pattern.Count - 2; i >= 0; i--)
				prefixMatchCount[i] += prefixMatchCount[i + 1];
			return prefixMatchCount;
		}
		private static bool Equal<TChar>(TChar a, TChar b)
		{
			return EqualityComparer<TChar>.Default.Equals(a, b);
		}
		private static int[] Zfunction<TChar>(IList<TChar> str)
		{
			var z = new int[str.Count];
			z[0] = str.Count;
			int rightBoundIdx = 1, rightBound = -1000;
			for (int i = 1; i < str.Count; i++) 
			{
				if (i + z[i - rightBoundIdx] > rightBound)
				{
					z[i] = Math.Max(0, rightBound - i + 1);
					while (i + z[i] < str.Count && Equal(str[z[i]], str[i + z[i]]))
						z[i]++;
				}
				else
				{
					z[i] = z[i - rightBoundIdx];
				}
				if (i + z[i] - 1 >= rightBound)
				{
					rightBoundIdx = i;
					rightBound = i + z[i] - 1;
				}
			}
			return z;
		}
	}
}

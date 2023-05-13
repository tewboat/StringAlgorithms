using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
	class Matcher2d
	{
		public static List<Tuple<int, int>> PatternMatches<TChar>(
				IList<IList<TChar>> pattern, IList<IList<TChar>> matrix, TChar delimeter) 
				where TChar : IComparable<TChar>
		{
			if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
				throw new ArgumentException();
			if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
				throw new ArgumentException();
			int p = pattern.Count, q = pattern[0].Count;
			int m = matrix.Count, n = matrix[0].Count;
			var result = new List<Tuple<int, int>>();

			var input = new List<TChar>(m * n + m - 1);
			input.AddRange(matrix[0]);
			for (var i = 1; i < m; i++)
			{
				input.Add(delimeter);
				input.AddRange(matrix[i]);
			}

			var data = new List<int[]>(n);
			for (var i = 0; i < n; i++)
			{
				data.Add(new int[m]);
				Array.Fill(data[i], -1);
			}
			
			var ahoCorasic = new AhoCorasick<TChar>(pattern, out var ids);
			ahoCorasic.ReportOccurrencesIds(input, (position, id) =>
			{
				data[position % (n + 1) - q + 1][position / (n + 1)] = id;
			});
			
			var s = new List<int>(m * n + m - 1);
			s.AddRange(data[0]);
			for (var i = 1; i < m; i++)
			{
				s.Add(-2);
				s.AddRange(data[i]);
			}

			
			var ac = new AhoCorasick<int>(new[] { ids }, out _);
			ac.ReportOccurrencesIds(s, (position, id) =>
			{
				result.Add(new Tuple<int, int>(position % (m + 1) - p + 1, position / (m + 1)));
			});

			return result;
		}

		public static List<Tuple<int, int>> NaivePatternMatches<TChar>(
				IList<IList<TChar>> pattern, IList<IList<TChar>> matrix) 
				where TChar : IComparable<TChar>
		{
			if (pattern[0].Count == 0 || pattern.Any(row => row.Count != pattern[0].Count))
				throw new ArgumentException();
			if (matrix[0].Count == 0 || matrix.Any(row => row.Count != matrix[0].Count))
				throw new ArgumentException();
			int p = pattern.Count, q = pattern[0].Count;
			int m = matrix.Count, n = matrix[0].Count;
			var result = new List<Tuple<int, int>>();
			for (int i = 0; i <= m - p; i++)
				for (int j = 0; j <= n - q; j++)
					if (pattern.Select((row, k) => Enumerable.SequenceEqual(
						matrix[i + k].Skip(j).Take(q), pattern[k])).All(b => b))
					{
						result.Add(Tuple.Create(i, j));
					}
			return result;
		}
	}
}

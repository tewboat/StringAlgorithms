using System;
using System.Linq;

namespace task_Match2d
{
	using System.Diagnostics;

	class Program
	{
		static void PrepareMatrixAndPattern(out int[][] matrix, out int[][] pattern, Random rand)
		{
			matrix = Enumerable.Range(0, 100).Select(_ => Enumerable.Range(0, 100)
						.Select(__ => rand.Next() % 3).ToArray()).ToArray();
			pattern = Enumerable.Range(0, 10).Select(_ => Enumerable.Range(0, 5)
						.Select(__ => rand.Next() % 3).ToArray()).ToArray();
			for (int occNum = rand.Next() % 20; occNum >= 0; occNum--)
			{
				int x = rand.Next() % (matrix.Length - pattern.Length + 1);
				int y = rand.Next() % (matrix[0].Length - pattern[0].Length + 1);
				for (int i = 0; i < pattern.Length; i++)
					for (int j = 0; j < pattern[0].Length; j++)
						matrix[x + i][y + j] = pattern[i][j];
			}
		}
		static void Main(string[] args)
		{
			int[][] aa = Enumerable.Repeat(Enumerable.Repeat(5, 1000).ToArray(), 1000).ToArray();
			int[][] a = Enumerable.Repeat(Enumerable.Repeat(5, 1000).ToArray(), 1000).ToArray();
			var time = Stopwatch.StartNew();
			var occsT = Matcher2d.PatternMatches(a, aa, int.MaxValue);
			time.Stop();
			Console.WriteLine(time.ElapsedMilliseconds);
			
			time.Restart();
			var occs_expectedT = Matcher2d.NaivePatternMatches(a, aa);
			time.Stop();
			Console.WriteLine(time.ElapsedMilliseconds);
			
			var rand = new Random();
			for (int i = 0; i < 1000; i++)
			{
				int[][] matrix, pattern;
				PrepareMatrixAndPattern(out matrix, out pattern, rand);
				var occs = Matcher2d.PatternMatches(pattern, matrix, int.MaxValue);
				var occs_expected = Matcher2d.NaivePatternMatches(pattern, matrix);
				if (!Enumerable.SequenceEqual(
						occs.OrderBy(t => t.Item1).ThenBy(t => t.Item2), 
						occs_expected.OrderBy(t => t.Item1).ThenBy(t => t.Item2)))
				{
					Console.WriteLine("TEST FAILED!");
					return;
				}
			}
			Console.WriteLine("All tests passed successfully");
		}
	}
}

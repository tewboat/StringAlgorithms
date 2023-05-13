namespace Compress;

using System;
using System.IO;
using System.Linq;

public static class Program
{
    public static void Main(string[] args)
    {
        foreach (var filename in new[] { "img.bmp", "program.txt", "lotr.txt", })
        {
            var data = File.ReadAllBytes(filename);
            var compressed = Compressor.Compress(data);
            File.WriteAllBytes(filename + ".compessed", compressed);
            Console.WriteLine($"'{filename}' {data.Length:n0} B to {compressed.Length:n0} B");
            if (!Compressor.Decompress(compressed).SequenceEqual(data))
                Console.WriteLine("Decompression error");
        }
    }
}
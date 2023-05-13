namespace Compress;

using System;
using System.Collections.Generic;
using System.Linq;

public static class Compressor
{
	private static int[] DataToIds(byte[] data)
	{
		var root = CompressionTrie.InitRoot();
		var currentNode = root;
		var code = 256;
		var ids = new List<int>();
		foreach (var b in data)
		{
			if (currentNode.Next.TryGetValue(b, out var nextNode))
			{
				currentNode = nextNode;
				continue;	
			}

			ids.Add(currentNode.Code);
			currentNode.Next[b] = CompressionTrie.InitTree(code++);
			currentNode = root.Next[b];
		}
		
		ids.Add(currentNode.Code);

		return ids.ToArray();
	}
		
	private static byte[] IdsToData(int[] ids)
	{
		var data = new List<byte>();
		var max = ids.Max();
		var root = DecompressionTrie.InitRoot();
		var dict = new DecompressionTrie[max + 1];

		foreach (var (b, node) in root.Next)
			dict[b] = node;
		
		var currentNode = root;
		var code = 256;
		foreach (var id in ids)
		{
			var node = dict[id];

			if (node == null!)
			{
				node = currentNode;
				var strr = new List<byte>();
				while (node != root)
				{
					strr.Add(node.Byte);
					node = node.Parent;
				}
				
				node = DecompressionTrie.InitTree(currentNode, strr[^1]);
				currentNode.Next[strr[^1]] = node;
				dict[code++] = node;
			}
			
			var str = new List<byte>();
			while (node != root)
			{
				str.Add(node!.Byte);
				node = node.Parent;
			}

			str.Reverse();
			data.AddRange(str);

			var b = str[0];
			if (currentNode.Next.TryGetValue(b, out var nextNode))
			{
				currentNode = nextNode;
				continue;
			}
			
			nextNode = DecompressionTrie.InitTree(currentNode, b);
			dict[code++] = nextNode;
			currentNode = root.Next[b];
		}

		return data.ToArray();
	}
		
	public static byte[] Compress(byte[] data)
	{
		var ids = DataToIds(data);
		var output = new List<byte>();
		int id_bits = 8, curr_bit = 0;
		for (int id = 0; id < ids.Length; id++)
		{
			int new_size = (curr_bit + id_bits + 7) / 8 + 8;
			output.AddRange(new byte[new_size - output.Count]);
			var bytes = BitConverter.GetBytes((ulong)ids[id] << curr_bit % 8);
			for (int k = 0; k < 2 + id_bits/8; k++)
				output[curr_bit/8 + k] |= bytes[k];
			curr_bit += id_bits;
			if (256 + id >= (1 << id_bits))
				id_bits++;
		}
		output.RemoveRange((curr_bit + 7) / 8, 8);
		return output.ToArray();
	}
		
	public static byte[] Decompress(byte[] data)
	{
		var ids = new List<int>();
		int id_bits = 8, curr_bit = 0, last_bit = data.Length * 8;
		data = data.Concat(new byte[8]).ToArray();
		for (int id = 0; curr_bit + id_bits <= last_bit; id++)
		{
			ulong x = BitConverter.ToUInt64(data, curr_bit / 8);
			ids.Add((int)(x >> curr_bit % 8) & (1 << id_bits) - 1);
			curr_bit += id_bits;
			if (256 + id >= (1 << id_bits))
				id_bits++;
		}
		return IdsToData(ids.ToArray());
	}
}
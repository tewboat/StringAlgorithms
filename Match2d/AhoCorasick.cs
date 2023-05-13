namespace task_Match2d
{
    class AhoCorasick<TChar> where TChar : IComparable<TChar>
    {
        private readonly Trie<TChar> root;

        public delegate void ReportAction(int endPosition, int id);

        public void ReportOccurrencesIds(IEnumerable<TChar> input, ReportAction report)
        {
            var j = 0;
            var currentNode = root;
            foreach (var currentChar in input)
            {
                while (currentNode != root && !currentNode.Next.ContainsKey(currentChar))
                    currentNode = currentNode.Link;
                currentNode = currentNode.Next.TryGetValue(currentChar, out var u) ? u : root;

                for (var v = currentNode; v != root; v = v.Report)
                    if (v.Terminal)
                        report(j, v.Id);
                j++;
            }
        }

        public AhoCorasick(IEnumerable<IList<TChar>> strings, out List<int> stringIds)
        {
            var lastId = 0;
            stringIds = new List<int>();

            root = new Trie<TChar>();

            foreach (var str in strings)
            {
                var currentNode = root;
                foreach (var t in str)
                {
                    if (currentNode.Next.TryGetValue(t, out var nextNode))
                    {
                        currentNode = nextNode;
                        continue;
                    }

                    nextNode = new Trie<TChar>();
                    currentNode.Next[t] = nextNode;
                    currentNode = nextNode;
                }

                if (currentNode.Terminal)
                {
                    stringIds.Add(currentNode.Id);
                    continue;
                }

                currentNode.Terminal = true;
                var id = lastId++;
                currentNode.Id = id;
                stringIds.Add(id);
            }
            
            SetUpLinksAndReports(root);
        }

        private static void SetUpLinksAndReports(Trie<TChar> root)
        {
            var queue = new Queue<Trie<TChar>>();
            root.Report = root;
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var (c, x) in currentNode.Next)
                {
                    queue.Enqueue(x);
                    x.Link = null!;
                    for (var p = currentNode; p != root && x.Link == null; p = p.Link)
                        x.Link = p.Link.Next.TryGetValue(c, out var nextNode) ? nextNode : null!;

                    if (x.Link == null!)
                        x.Link = root;

                    x.Report = x.Link;
                    if (!x.Report.Terminal)
                        x.Report = x.Link.Report;
                }
            }
        }
    }

    class Trie<TChar> where TChar : notnull
    {
        public readonly SortedDictionary<TChar, Trie<TChar>> Next = new SortedDictionary<TChar, Trie<TChar>>();
        public Trie<TChar> Report { get; set; }
        public Trie<TChar> Link { get; set; }
        public bool Terminal { get; set; }
        public int Id { get; set; } = -1;
    }
}
using System.Diagnostics.Contracts;

var lines = File.ReadAllLines(@"../../../input.txt");
DirectoryEntry root = new("/");
int linenum = 0;

if (lines[linenum++] == "$ cd /")
	buildTree(root);

// Part 1
//
Console.WriteLine(root.ComputeSizeMax(100000));

// Part 2
//
const int MAXSPACE = 70000000, NEEDED = 30000000;
int freespace = MAXSPACE - root.Size, neededspace = NEEDED - freespace;

Console.WriteLine($"FREE: {freespace}, TO FREE: {neededspace} MINSPACE: {root.FindClosestSize(neededspace)}");


void buildTree(DirectoryEntry de)
{
	while(linenum < lines.Length) {
		var l = lines[linenum++];

		if (l == "$ cd ..") // use recursion stack to move up and down file system tree
			return;

		if (l == "$ ls") {
			for (; linenum < lines.Length; linenum++) {
				var l2 = lines[linenum];
				if (l2.StartsWith("$"))
					break;

				if (l2.StartsWith("dir")) {
					de.Add(l2[4..], 0);
				}
				else {
					int space = l2.IndexOf(' ');
					int sz = int.Parse(l2[..space]);

					de.Add(l2[(space + 1)..], sz);
				}
			}
		}
		else if (l.StartsWith("$ cd")) {
			int last = l.LastIndexOf(' ');
			var sub = de.Find(l[(last+1)..]) as DirectoryEntry;

			buildTree(sub!);
		}
	}
}

record FSObject(string name) { public virtual int Size => 0; }

record DirectoryEntry(string name) : FSObject(name) {
	readonly List<FSObject> entries = new();

	public FSObject Add(string name, int size)
	{
		FSObject e = size > 0 ? new FileEntry(name, size) : new DirectoryEntry(name);
		entries.Add(e);

		return e;
	}

	public FSObject? Find(string name) => entries.Find(e => e.name == name);

	public int FindClosestSize(int lowerbound)
	{
		if(Size < lowerbound)
			return 0;

		int res = Size;
		foreach(var e in entries.OfType<DirectoryEntry>()) {
			var sz = e.FindClosestSize(lowerbound);
			if (sz >= lowerbound && sz < res) res = sz;
		}

		return res;
	}

	public override int Size => entries.Sum(e => e.Size);

	public int ComputeSizeMax(int maxsz)
	{
		int res = entries.OfType<DirectoryEntry>().Sum(e => e.ComputeSizeMax(maxsz));
		if (Size <= maxsz) res += Size;

		return res;
	}
}

record FileEntry(string name, int size) : FSObject(name) { public override int Size => size; }

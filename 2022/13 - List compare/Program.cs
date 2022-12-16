using System.Runtime.InteropServices;
using System.Text;

var lines = File.ReadAllLines(@"../../../input.txt");
var sum = 0;
List<ObjectList> packets = new();

for(int i=0; i<lines.Length; i += 3) {
	var left = ObjectList.Create(lines[i]);
	var right = ObjectList.Create(lines[i+1]);

	var cmp = left.CompareTo(right);
	if (cmp < 0) sum += 1+i/3;

	packets.Add(left);
	packets.Add(right);
}

packets.Sort();
var (sx, ex) = (packets.FindIndex(o => o.GetSingleItem() == 2), packets.FindIndex(o => o.GetSingleItem() == 6));

Console.WriteLine("left cmp right = {0}; dividers {1}-{2} *:{3}", sum, sx+1, ex+1, (sx+1)*(ex+1));


class ObjectList : IComparable<ObjectList> {
	ObjectList(ReadOnlySpan<char> val) => singleValue = int.Parse(val);

	ObjectList(List<ObjectList> list) => items = list;

	public int GetSingleItem()
	{
		return items == null
			? singleValue
			: items.Count == 1
				? items[0].GetSingleItem()
				: -1;
	}

	public static ObjectList Create(in ReadOnlySpan<char> text)
	{
		int pos = 0;
		return new(Parse(text[1..], ref pos));
	}

	static List<ObjectList> Parse(ReadOnlySpan<char> text, ref int at)
	{
		List<ObjectList> list = new();
		int token = at;

		while(at < text.Length) {
			switch (text[at]) {
			case ',':
				if (token < at) list.Add(new(text[token..at]));

				token = ++at;
				break;

			case ']':
				if (token < at) list.Add(new(text[token..at]));

				return list;

			case '[':
				at++;
				list.Add(new(Parse(text, ref at)));

				token = ++at;
				break;

			default:
				at++;
				break;
			}
		}

		return list;
	}

	public int CompareTo(ObjectList? other)
	{
		// double dispatch: do we have a list? Ask the other to compare a list, else a number
		//
		return items != null
			? -other!.CompareList(this)
			: -other!.CompareNumber(singleValue);
	}

	int CompareNumber(int other)
	{
		if(items == null) return singleValue.CompareTo(other);

		if(items.Count == 0) return -1;
		
		var cmp = items[0].CompareNumber(other);
		return cmp != 0 ? cmp : items.Count > 1 ? 1 : 0;
	}

	int CompareList(ObjectList list)
	{
		if(items == null) return -list.CompareNumber(singleValue);

		int maxcnt = Math.Min(items.Count, list.items!.Count);

		for (int i = 0; i < maxcnt; i++) {
			var cmp = items[i].CompareTo(list.items[i]);
			if (cmp!=0) return cmp;
		}

		return items.Count - list.items.Count;
	}

	readonly int singleValue;
	readonly List<ObjectList>? items;

	public string PrintFull()
	{
		if (items == null) return singleValue.ToString();

		var sb = new StringBuilder();
		
		sb.Append('[');
		int cnt = items.Count;
		foreach (var it in items) {
			sb.Append(it.PrintFull());
			if(--cnt>0) sb.Append(',');
		}
		sb.Append(']');

		return sb.ToString();
	}

	public override string ToString() => PrintFull();
}

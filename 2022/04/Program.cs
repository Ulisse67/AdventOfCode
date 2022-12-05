using System.Numerics;

var lines = File.ReadLines(@"../../../input.txt");
var count = 0;

foreach(var line in lines) {
	var sa = line.Split(',');
	Segment left = new(sa[0]), right = new(sa[1]);
	if (left | right) count++;
}

Console.WriteLine(count);

record Segment(int start, int end) {
	public Segment(string text) : this(default,default)
	{
		var args = text.Split('-');
		(start, end) = (int.Parse(args[0]), int.Parse(args[1]));
	}

	public bool IsIntersect(Segment rhs)
	{
		var ds = Math.Sign(start - rhs.start);
		var de = Math.Sign(rhs.end - end);

		return ds == de || ds==0 || de==0; // same sign or zero
	}

	public bool IsOverlap(Segment rhs)
	{
		return rhs.start <= end && rhs.end >= start;
	}

	public static bool operator &(Segment a, Segment b) => a.IsIntersect(b);
	public static bool operator |(Segment a, Segment b) => a.IsOverlap(b);
}

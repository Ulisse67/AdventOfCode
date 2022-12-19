using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"../../../input.txt");
var zones = lines.Select(l => new SensorZone(l)).ToArray();

int d = find_max_at(2000000);
Console.WriteLine("Max used positions {0}",d);

var tf = find_tuning_frequency(0,4000000);
Console.WriteLine("Tuning frequency = {0}",tf);

int find_max_at(int search_y)
{
	var maxrange = new Range(int.MaxValue,int.MinValue);
	int matching_zones = 0;

	foreach(var z in zones) {
		if(z.TryGetRow(search_y,out var range)) {
			maxrange = maxrange.Max(range);
			matching_zones++;
		}
	}

	return maxrange.Distance;
}

long find_tuning_frequency(int y_from, int y_to)
{
	List<Range> sl = new();

	for(int y = y_from; y < y_to; y++) {
		sl.Clear();

		foreach(var z in zones)
			if(z.TryGetRow(y,out var range)) sl.Add(range);
		
		sl.Sort();
		int lastx = -1;

		for(int i=0; i < sl.Count; i++) {
			if(sl[i].left - lastx > 1) {
				var x = lastx + 1;
				var tf = x * 4000000L + y;

				Console.WriteLine("At {0},{1} x={2}",x,y,tf);
				return tf;
			}

			lastx = Math.Max(lastx, sl[i].right);
		}
	}

	return 0;
}

class SensorZone {
	public SensorZone(string text)
	{
		var m = matcher.Match(text);
		
		position = new(parse(1),parse(2));
		beacon = new(parse(3),parse(4));
		distance = position.Distance(beacon);

		int parse(int ix) => int.Parse(m.Groups[ix].Value);
	}

	readonly MGPoint position, beacon;
	readonly int distance;

	public bool TryGetRow(int y, out Range row)
	{
		var ydist = Math.Abs(y - position.y) - distance;

		if(ydist <= 0) {
			row = new(position.x + ydist, position.x - ydist);
			return true;
		}

		row = default;
		return false;
	}

	static readonly Regex matcher = new(@"x=(.+), y=(.+):.+x=(.+), y=(.+)", RegexOptions.Compiled);
}

record struct MGPoint(int x, int y) {
	public int Distance(MGPoint rhs) => Math.Abs(x-rhs.x)+Math.Abs(y-rhs.y);
}

record struct Range(int left, int right) : IComparable<Range> {
	public Range Max(Range rhs) => new Range(Math.Min(left,rhs.left), Math.Max(right,rhs.right));
	public int Distance => Math.Abs(left-right);

	public int CompareTo(Range other)
	{
		return left > other.left
			? 1
			: left < other.left ? -1 : right.CompareTo(other.right);
	}
}

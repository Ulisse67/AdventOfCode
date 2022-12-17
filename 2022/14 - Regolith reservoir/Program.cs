#define PART1
//#define TEST

using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Extensions;

using static System.Math;

#if !TEST
var lines = File.ReadAllLines(@"../../../input.txt");
#else
var lines = new string[] {
	"498,4-> 498,6-> 496,6",
	"503,4-> 502,4-> 502,9-> 494,9",
};
#endif

var segments = lines.Select(l => new SegList(l)).ToList();

#if PART2
const int x_extra = 146, y_extra = 2;
#else
const int x_extra = 0, y_extra = 0;
#endif

int bottom_y = SegList.BottomRight.y + y_extra;
var (x0, x1) = (SegList.TopLeft.x-x_extra, SegList.BottomRight.x+x_extra);

var clip = new Point(x1 - x0 + 1, bottom_y+1);
var origin = new Point(x0,0);

Cave matrix = new(clip, origin);
var drawn = 0;

foreach(var seg in segments) {
	drawn += matrix.DrawPoly(seg.segments, '#');
}

// Part 2:
//
#if PART2
var bottomrock = new SegList(new Point[] { new(x0,bottom_y),new(x1,bottom_y) });
matrix.DrawPoly(bottomrock.segments,'#');
#endif

var grains = 0;

while(matrix.DrawSand(new(500, 0))) grains++;

Console.WriteLine(grains);



record SegList(IReadOnlyList<Point> segments) {
	public SegList(string text) : this(GetSegments(text)) {
		var (p0,p1) = (new Point(int.MaxValue, int.MaxValue), new Point(0, 0));

		foreach (var s in segments) {
			p0 = p0.Min(s);
			p1 = p1.Max(s);
		}

		TopLeft = TopLeft.Min(p0);
		BottomRight = BottomRight.Max(p1);
	}

	public static Point TopLeft { get; private set; } = new(int.MaxValue, int.MaxValue);
	public static Point BottomRight { get; private set; }

	static IReadOnlyList<Point> GetSegments(string text)
	{
		return matcher.Matches(text).Select(m => new Point(parse(m, 1), parse(m, 2))).ToImmutableList();

		static int parse(Match m, int ix) => int.Parse(m.Groups[ix].ValueSpan);
	}

	static readonly Regex matcher = new(@"(\d+),(\d+)", RegexOptions.Compiled);
}

class Cave {
	readonly char[,] grid;
	readonly Point origin, clip;

	public Cave(Point size, Point origin)
	{
		this.grid = new char[size.y, size.x];
		this.origin = origin;
		this.clip = size;

		grid.Fill('.');
	}

	public void DrawYourself() => grid.Print();

	public int DrawPoly(IReadOnlyList<Point> segments, char symbol)
	{
		if (segments.Count < 2) return 0;
		int drawn = 0;

		for (int i = 1; i < segments.Count; i++) {
			var (pt0, pt1) = (segments[i-1].Min(segments[i]), segments[i-1].Max(segments[i]));
			pt0 -= origin;
			pt1 -= origin;

			for(int y = pt0.y; y <= pt1.y; y++)
				for(int x = pt0.x; x <= pt1.x; x++)
					grid[y,x] = symbol;

			var d = pt0.Distance(pt1);
			drawn += d.x * d.y;
		}

		return drawn - (segments.Count - 2); // subtract segment joints
	}

	public bool DrawSand(Point drop)
	{
		drop -= origin;
		int dx = 0, dy = 1;

		for (; ; ) {
			Point next = new(drop.x + dx, drop.y + dy);

			if (next.y >= clip.y || next.x < 0 || next.x >= clip.x)
				return false;

			var ch = grid[next.y, next.x];

			switch(ch) {
			case '.':
				drop = next;
				dx = 0;

				continue;

			case '#':
			case 'o':
				if (dx == 0)
					dx = -1;
				else if (dx < 0)
					dx = 1;
				else {
					if (grid[drop.y, drop.x] != '.') return false; // full

					grid[drop.y, drop.x] = 'o';
					return true;
				}

				break;
			}
		}
	}
}

record struct Point(int x, int y) {
	public static Point operator -(Point a, Point b) => new(a.x - b.x, a.y - b.y);
	public Point Distance(Point b) => new(Abs(b.x - x) + 1, Abs(b.y - y) + 1);
	public Point Min(Point b) => new(Math.Min(x, b.x), Math.Min(y, b.y));
	public Point Max(Point b) => new(Math.Max(x, b.x), Math.Max(y, b.y));
}

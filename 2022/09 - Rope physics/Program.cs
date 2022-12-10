using System.Numerics;

var moves = File.ReadAllLines(@"../../../input.txt");
Rope theRope = new(0, 0, 10);

foreach (var m in moves) {
	var (dir, steps) = (m[0], int.Parse(m[2..]));

	theRope.MoveAll(dir, steps);
}

Console.WriteLine(theRope.TailSteps);

class Rope {
	public Rope(int xstart, int ystart)
	{
		// Part 1
		//
		head = tail = new(xstart, ystart);
		traces.Add(tail);
	}

	public Rope(int xstart, int ystart, int numknots)
	{
		// Part 2
		//
		knots = new Point[numknots];

		knots[0] = knots[^1] = new(xstart, ystart);
		traces.Add(knots[^1]);
	}

	static Point get_steps(char dir)
	{
		return dir switch {
			'U' => new(0, +1),
			'D' => new(0, -1),
			'L' => new(-1, 0),
			'R' => new(+1, 0),
			_ => new(0,0),
		};
	}

	HashSet<Point> traces = new();

	public int TailSteps => traces.Count;

	public void Move(char direction, int steps)
	{
		var dir = get_steps(direction);

		for (; steps > 0; steps--) {
			if (dir.x != 0) {
				if (head.x - tail.x == dir.x) tail = head;
			}
			else {
				if (head.y - tail.y == dir.y) tail = head;
			}

			head += dir;

			traces.Add(tail);
		}
	}

	Point head, tail;
	readonly Point[]? knots;

	public void MoveAll(char direction, int steps)
	{
		if(knots == null) { // safety check
			Move(direction, steps);
			return;
		}

		var dir = get_steps(direction);

		for (; steps > 0; steps--) {
			knots[0] += dir;

			for (int ix = 1; ix < knots.Length; ix++) {
				var distance = knots[ix-1] - knots[ix];
				var delta = distance.Abs();

				if (delta.x != 2 && delta.y != 2) break; // no change: stop propagation

				knots[ix] += distance.Sign();

				if (ix == knots.Length-1) traces.Add(knots[^1]);
			}
		}
	}

	record struct Point(int x, int y) : IAdditionOperators<Point,Point,Point>, ISubtractionOperators<Point,Point,Point>
	{
		public static Point operator+(Point a, Point b) => new(a.x+b.x, a.y+b.y);
		public static Point operator-(Point a, Point b) => new(a.x-b.x, a.y-b.y);
		public Point Abs() => new(Math.Abs(x), Math.Abs(y));
		public Point Sign() => new(Math.Sign(x), Math.Sign(y));
	}
}

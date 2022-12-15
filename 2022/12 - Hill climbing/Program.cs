using Extensions;

var lines = File.ReadAllLines(@"../../../input.txt");

var grid = lines.Make<char, string>();
var walker = new BfsWalker(grid);

const int part = 2;
var (start, move, ended) = part == 1 ? setup_1(grid) : setup_2(grid);
var lev = walker.Find(start, move, ended);

Console.WriteLine(lev);


(Node, Func<char, char, bool>, Func<Node, bool>) setup_1(char[,] grid)
{
	var (start_r, start_c) = grid.Find('S');
	var (end_r, end_c) = grid.Find('E');

	grid[start_r, start_c] = 'a';
	grid[end_r, end_c] = 'z';

	Node ns = new(start_r, start_c, 0), ne = new(end_r, end_c, 0);
	return (ns, (ch, from) => ch - from <= 1, n => n == ne);
}

(Node, Func<char, char, bool>, Func<Node, bool>) setup_2(char[,] grid)
{
	var (start_r, start_c) = grid.Find('E');
	grid[start_r, start_c] = 'z';

	return (new(start_r, start_c, 0), (ch, from) => from - ch <= 1, n => grid[n.row, n.col] == 'a');
}

class BfsWalker {
	public BfsWalker(char[,] matrix) => (grid, rows, cols) = (matrix, matrix.GetLength(0), matrix.GetLength(1));

	Func<char,char,bool>? is_adjacent;
	Func<Node,bool>? is_end;

	public int Find(Node start, Func<char, char, bool> move, Func<Node,bool> ended)
	{
		(is_adjacent, is_end) = (move, ended);

		neighbours.Clear();
		visited.Clear();

		return do_walk(start);
	}

	int do_walk(Node start)
	{
		neighbours.Enqueue(start);
		visited.Add(start);

		while (neighbours.TryDequeue(out var next)) {
			if (is_end!(next))
				return next.level;

			var (r, c, l) = next;
			var ch = grid[r, c];

			try_move(r, c - 1, ch, l);
			try_move(r, c + 1, ch, l);
			try_move(r - 1, c, ch, l);
			try_move(r + 1, c, ch, l);
		}

		return -1;
	}

	readonly char[,] grid;
	readonly int rows, cols;

	readonly HashSet<Node> visited = new();
	readonly Queue<Node> neighbours = new();

	void try_move(int r, int c, char from, int lev)
	{
		if (r < 0 || r >= rows || c < 0 || c >= cols)
			return;

		Node n = new(r, c, lev + 1);
		if (visited.Contains(n))
			return;

		var ch = grid[r, c];
		if (!is_adjacent!(ch, from))
			return;

		neighbours.Enqueue(n);
		visited.Add(n);
	}
}

record struct Node(int row, int col, int level) {
	public bool Equals(Node rhs) => row == rhs.row && col == rhs.col;
	public override int GetHashCode() => HashCode.Combine(row.GetHashCode(), col.GetHashCode());
}
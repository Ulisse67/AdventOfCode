// See https://aka.ms/new-console-template for more information
using System.Numerics;

const uint SEED = 1362;
const uint X_MAX = 32, Y_MAX = 40;
const uint X_DEST = 31, Y_DEST = 39;
//const uint SEED = 10;
//const uint X_MAX = 9, Y_MAX = 6;
//const uint X_DEST = 7, Y_DEST = 4;

char[][] matrix = new char[Y_MAX+1][];

for (uint y=0; y<=Y_MAX; y++) {
	Console.Write("{0} ",y);
	matrix[y] = new char[X_MAX + 1];

	for(uint x=0; x<=X_MAX; x++) {
		var px = getpos(x, y);
		matrix[y][x] = px;
		Console.Write(px);
	}

	Console.WriteLine();
}

int counter = 0, mincnt = 10000;
(int dx, int dy)[] dirs = { (1, 0), (-1, 0), (0, 1), (0, -1) };
HashSet<(int, int)> points = new();

walk(1, 1);

Console.WriteLine(mincnt);

void walk(int x, int y)
{
	if(x<0 || x>X_MAX || y<0 || y>Y_MAX) return;

	if (matrix[y][x] != '.') return;

	points.Add((x, y));
	if (counter == 50) return;

	if (x == X_DEST && y == Y_DEST) {
		if (mincnt > counter) mincnt = counter;
		return;
	}

	counter++;
	matrix[y][x] = '='; // flag to avoid walking that path again

	walk(x + 1, y);
	walk(x - 1, y);
	walk(x, y + 1);
	walk(x, y - 1);

	matrix[y][x] = '.';
	counter--;
}

static ulong compute_hash(uint x, uint y) => x * x + 3 * x + 2 * x * y + y + y * y  + SEED;
static char getpos(uint x, uint y) => (BitOperations.PopCount(compute_hash(x, y)) & 1) != 0 ? '#' : '.';

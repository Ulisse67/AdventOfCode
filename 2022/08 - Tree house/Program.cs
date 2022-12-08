using System.Xml.Schema;

using Extensions;

var lines = File.ReadAllLines(@"../../../input.txt");
var grid = lines.Make<char,string>();
var (maxrows,maxcols) = (grid.GetLength(0), grid.GetLength(1));

int visible_trees = 2 * (maxrows + maxcols) -4; // don't sum corners twice

for(int row=1; row<maxrows-1; row++) {
	for(int col=1; col<maxcols-1; col++) {
		var ch = grid[row, col];

		if(left() || right() || up() || down())
			visible_trees++;

		bool left()
		{
			for (int x = col - 1; x >= 0; x--) if (grid[row, x] >= ch) return false;
			return true;
		}

		bool right()
		{
			for (int x = col + 1; x < maxcols; x++) if (grid[row, x] >= ch) return false;
			return true;
		}

		bool up()
		{
			for (int y = row -1; y >=0; y--) if (grid[y, col] >= ch) return false;
			return true;
		}

		bool down()
		{
			for (int y = row + 1; y < maxrows; y++) if (grid[y, col] >= ch) return false;
			return true;
		}
	}
}

Console.WriteLine("Visible trees: {0}", visible_trees);

// Part 2
//
int max_score = 0;

for (int row = 1; row < maxrows-1; row++) {
	for (int col = 1; col < maxcols-1; col++) {
		var ch = grid[row, col];

		var score = left() * right() * up() * down();
		if(score>max_score) max_score = score;

		int left()
		{
			int x;
			for (x = col - 1; x >= 0; x--) if (grid[row, x] >= ch) return col-x;
			return col;
		}

		int right()
		{
			int x;
			for (x = col + 1; x < maxcols; x++) if (grid[row, x] >= ch) return x-col;
			return maxcols - (col+1);
		}

		int up()
		{
			int y;
			for (y = row - 1; y >= 0; y--) if (grid[y, col] >= ch) return row-y;
			return row;
		}

		int down()
		{
			int y;
			for (y = row + 1; y < maxrows; y++) if (grid[y, col] >= ch) return y-row;
			return maxrows - (row+1);
		}
	}
}

Console.WriteLine("Max score: {0}", max_score);

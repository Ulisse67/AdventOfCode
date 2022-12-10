using Extensions;

var lines = File.ReadAllLines(@"../../../input.txt");

var regx = 1;
var sig_strength = 0;

const int mincycle = 20, cyclestep = 40, maxcycle = 220;
int cycle_counter=0, c40=mincycle;

// Part 2
//
var crtscreen = new char[6, 40];

foreach (var line in lines) {
	switch (line[..4]) {
	case "noop":
		check_and_sum();
		break;

	case "addx":
		check_and_sum();
		check_and_sum();

		var arg = int.Parse(line[5..]);
		regx += arg;

		break;
	}

	void check_and_sum()
	{
		// Part 2
		//
		var (r, c) = getpixel(cycle_counter);
		var ch = regx-1 <= c && c <= regx+1 ? '#':'.';

		crtscreen[r, c] = ch;

		// Part 1
		//
		cycle_counter++;

		if (cycle_counter <= maxcycle && --c40 == 0) {
			c40 = cyclestep;
			sig_strength += cycle_counter * regx;
		}

		static (int r, int c) getpixel(int cycle) => (cycle / 40, cycle % 40);
	}
}

Console.WriteLine(sig_strength);
crtscreen.Print();

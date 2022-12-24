using System.Text;

var line = File.ReadAllText(@"../../../input.txt");

string[] rock_pixels = {
@"####",

@".#.
###
.#.",

@"..#
..#
###",

@"#
#
#
#",

@"##
##"
};

var rocks = rock_pixels.Select(r => new Rock(r)).ToArray();
var arena = new Arena(7,line);

const long MAX_THROWS =  1000000000000; // 2022
const int CACHE_FILL_ATTEMPTS = 3000;

int cycle_start = fill_cache(CACHE_FILL_ATTEMPTS, out List<ArenaState> weights);
long res = cycle_start >= 0 ? compute_throws(cycle_start, weights, MAX_THROWS) : arena.Top;

Console.WriteLine(res);


long compute_throws(int cycle_start, IReadOnlyList<ArenaState> weights, in long max_throws)
{
	long sum_init = weights.Take(cycle_start).Sum(e => e.increase);
	long sum_cycle = weights.Skip(cycle_start).Sum(e => e.increase);
	int cycle_len = weights.Count - cycle_start;

	long repeating = max_throws - cycle_start;
	var (q, r) = Math.DivRem(repeating,cycle_len);
	long sum_rem = weights.Skip(cycle_start).Take((int)r).Sum(e => e.increase);

	long res = q * sum_cycle + sum_rem + sum_init;

	return res;
}


int fill_cache(int attempts, out List<ArenaState> log)
{
	HashSet<ArenaState> memoize = new();
	log = new();

	for(var cnt = attempts; cnt > 0;) {
		foreach(var r in rocks) {
			var st = arena.Add(r);

			if(!memoize.Add(st))
				return log.FindIndex(e => e == st);

			log.Add(st);

			if(--cnt <= 0) return -1;
		}
	}

	return -1;
}


record ArenaState(int rock, int jetIndex, byte pixels, int increase);

class Rock {
	public Rock(string shape)
	{
		var sa = shape.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
		(height,width) = (sa.Length,sa[0].Length);

		pixels = new byte[height];

		for(int i=0; i<sa.Length; i++) {
			foreach(var ch in sa[i]) {
				pixels[i] <<= 1;
				pixels[i] |= ch switch { '.' => 0, '#' => 1, _=>0 };
			}
		}
	}

	static int rockNumber;
	public int Id { get; init; } = rockNumber++;

	public readonly byte[] pixels;
	public readonly int height, width;
	static readonly string[] b2s = {
		"....",
		"...#",
		"..#.",
		"..##",
		".#..",
		".#.#",
		".##.",
		".###",
		"#...",
		"#..#",
		"#.#.",
		"#.##",
		"##..",
		"##.#",
		"###.",
		"####"
	};

	public override string ToString()
	{
		StringBuilder sb = new();
		foreach(var b in pixels)
			sb.AppendLine(b2s[b]);
		
		return sb.ToString();
	}
}

class FallingRock {
	public FallingRock(Rock r, int top, int left, int width)
	{
		proto = r;
		(vpos, hpos) = (top, left);
		shift = width - proto.width;
	}

	public int Id => proto.Id;
	public int Height => proto.height;

	public int vpos, hpos;

	readonly Rock proto;
	readonly int shift;

	public bool can_move(int dir) => 0 <= hpos+dir && hpos+dir <= shift;

	public byte this[int y] => (byte)(proto.pixels[y] << (shift-hpos));
	public byte this[Index y] => this[y.GetOffset(proto.pixels.Length)];
}

class Arena {
	public Arena(int width, string jets)
	{
		gasJets = jets;
		arenaWidth= width;
	}

	public int Top => bottom;

	int bottom, nextJet;
	byte[] shaft = new byte[5000];

	readonly int arenaWidth;
	readonly string gasJets;

	public void Print(TextWriter output)
	{
		char[] line = new char[7];
		for(int y=bottom-1; y>=0; y--) {
			byte mask = 64, v = shaft[y];

			for(int b = 0; b < 7; b++) {
				line[b] = (v & mask) == 0 ? '.' : '#';
				mask >>= 1;
			}

			output.Write("{0,5}: ", y);
			output.WriteLine(line);
		}

		output.WriteLine();
	}

	public ArenaState Add(Rock next)
	{
		// 1. appear at top
		//
		FallingRock fr = new(next, bottom +3 +next.height -1, 2, arenaWidth);
		int currentLevel = bottom, currentJet = nextJet;

		for(; ; ) {
			// 2. blow
			//
			if(nextJet >= gasJets.Length) nextJet = 0;
			int dir = gasJets[nextJet++] switch { '>' => 1, '<' => -1, _ => 0 };

			if(fr.can_move(dir) && !hit_test(dir,0)) fr.hpos += dir;

			// 3. move down
			//
			int lasty = fr.vpos - fr.Height;

			if(lasty < 0 || hit_test(0,-1)) {
				or_add();
				bottom = Math.Max(bottom,fr.vpos + 1);

				return new(fr.Id,currentJet,shaft[bottom - 1],bottom - currentLevel);
			}

			fr.vpos--;

			bool hit_test(int hdir, int vdir)
			{
				for(int y = 0; y < fr.Height; y++) {
					var ab = shaft[fr.vpos - y + vdir];

					int map(byte b) => hdir switch { < 0 => b << 1, > 0 => b >> 1, 0 => b };

					if((map(fr[y]) & ab) != 0) return true;
				}

				return false;
			}

			void or_add()
			{
				for(int y = 0; y < fr.Height; y++)
					shaft[fr.vpos - y] |= fr[y];
			}
		}
	}
}

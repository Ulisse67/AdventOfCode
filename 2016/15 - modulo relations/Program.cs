using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"../../../input.txt");
Regex lexer = new(@"Disc #(\d+) has (\d+) positions; at time=(\d+), it is at position (\d+).");

var positions = lines.Select(l => {
	var m = lexer.Match(l);
	return new Disk(parse(1), parse(2), parse(4));

	int parse(int gn) => int.Parse(m.Groups[gn].ValueSpan);
}).ToArray();

int time, max_time = 10000000;

for(time=0; time < max_time; time++) {
	var sum = positions.Sum(d => d.new_pos(time));

	if (sum == 0) break;
}

Console.WriteLine(time);

record Disk(int id, int maxPosition, int position) {
	public int new_pos(int time) => (time + id + position) % maxPosition;
}

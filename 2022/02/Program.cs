// See https://aka.ms/new-console-template for more information
var moves = File.ReadLines(@"../../../input.txt");
//moves = new[] { "A Y","B X","C Z"};

Dictionary<char, Shapes> letters = new() {
	{ 'A', Shapes.Rock },
	{ 'B', Shapes.Paper },
	{ 'C', Shapes.Scissor },
	{ 'X', Shapes.Rock },
	{ 'Y', Shapes.Paper },
	{ 'Z', Shapes.Scissor },
};

Dictionary<string, int> outcomes = new() {
	{ "A X", 3 },
	{ "A Y", 6 },
	{ "A Z", 0 },
	{ "B X", 0 },
	{ "B Y", 3 },
	{ "B Z", 6 },
	{ "C X", 6 },
	{ "C Y", 0 },
	{ "C Z", 3 },
};

Dictionary<string, string> map_outcomes = new() {
	{ "A X", "A Z" },
	{ "A Y", "A X" },
	{ "A Z", "A Y" },
	{ "B X", "B X" },
	{ "B Y", "B Y" },
	{ "B Z", "B Z" },
	{ "C X", "C Y" },
	{ "C Y", "C Z" },
	{ "C Z", "C X" },
};

int[] letter_scores = { 1, 2, 3 };

int sum = 0;
foreach(var line in moves) {
	// part 1
	//if(outcomes.TryGetValue(line,out var v))
	//	sum += v + (int)letters[line[2]]+1;

	// part 2
	if(map_outcomes.TryGetValue(line, out var v)) {
		if(outcomes.TryGetValue(v, out var v2)) {
			sum += v2 + (int)letters[v[2]] + 1;
		}
	}
}

Console.WriteLine(sum);

enum Shapes { Rock, Paper, Scissor };

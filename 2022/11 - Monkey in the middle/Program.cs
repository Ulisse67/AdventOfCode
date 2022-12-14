using System.Text.RegularExpressions;

var lines = File.ReadLines(@"../../../input.txt");
var initsequence = lines.Chunk(7);
var monkeys = initsequence.Select(init => new Monkey(init)).ToList();

var (times, reduce) = (10000, 1);//(20, 3);

for(; times>0; times--)
	monkeys.ForEach(m => m.Execute(monkeys, reduce));

var counts = monkeys.Select(m => m.InspectionCount).Order().ToArray();
var monkeyBusinessLevel = counts[^2] * counts[^1];

Console.WriteLine(monkeyBusinessLevel);

class Monkey {
	public static int primeFactors=1;

	public Monkey(IEnumerable<string> description)
	{
		var it = description.GetEnumerator();
		it.MoveNext();
		if (!it.Current.StartsWith("Monkey ")) throw new ArgumentException("Not a monkey.");

		id = int.Parse(it.Current[7..^1]);

		it.MoveNext();
		var colon = it.Current.IndexOf(':') + 2;
		items = it.Current[colon..].Split(',').Select(s => int.Parse(s)).ToList();

		it.MoveNext();
		var expr = Regex.Match(it.Current, "Operation: new = (.+) ([+*]) (.+)");
		var arg1 = expr.Groups[1].Value;
		var op = expr.Groups[2].Value[0];
		var arg2 = expr.Groups[3].Value;

		if (arg1 != "old") throw new ArgumentException("First argument not old.");

		operation = op switch { '+' => do_plus, '*' => do_times, _ => a=>a };
		operand2 = arg2 == "old" ? -1 : int.Parse(arg2);

		it.MoveNext();
		expr = Regex.Match(it.Current, @"divisible by (\d+)");
		primeDivisor = int.Parse(expr.Groups[1].Value);
		primeFactors *= primeDivisor;

		it.MoveNext();
		expr = Regex.Match(it.Current, @"If true: throw to monkey (\d+)");
		ifTrue = int.Parse(expr.Groups[1].Value);

		it.MoveNext();
		expr = Regex.Match(it.Current, @"If false: throw to monkey (\d+)");
		ifFalse = int.Parse(expr.Groups[1].Value);
	}

	int do_plus(int a) => checked(a + (operand2 < 0 ? a : operand2));
	int do_times(int a) => (int)do_times((long)a);
	long do_times(long a) => checked(a * (operand2< 0 ? a : operand2)) % primeFactors;

	readonly int id;
	readonly List<int> items;
	readonly Func<int, int> operation;

	readonly int primeDivisor, operand2;
	readonly int ifTrue, ifFalse;

	public void Execute(List<Monkey> context, int reduce=1)
	{
		foreach (var item in items) {
			var v = operation(item) / reduce;
			var nx = v % primeDivisor == 0 ? ifTrue : ifFalse;

			context[nx].items.Add(v);
		}

		InspectionCount += items.Count;
		items.Clear();
	}

	public long InspectionCount { get; private set; }

	public override string ToString() => InspectionCount.ToString();
}

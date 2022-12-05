// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;

int[] regs = { 0, 0, 0, 0 };

var program = File.ReadAllLines(@"../../../input.txt").Select(l => new instr(l)).ToArray();
//program = @"
//cpy 41 a
//inc a
//inc a
//dec a
//jnz a 2
//dec a".Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(l => new instr(l)).ToArray();

for (int pc=0; pc<program.Length; ) {
	var ops = program[pc];

	switch(ops.op) {
	case 'c':
		set(ops.b) = get(ops.a);
		break;

	case 'i':
		set(ops.a)++;
		break;

	case 'd':
		set(ops.a)--;
		break;

	case 'j':
		if (get(ops.a) != 0) {
			pc += ops.b;
			continue;
		}

		break;
	}

	pc++;

	int get(int v) => v < 1000 ? v : regs[v - 1000];
	ref int set(int v) => ref regs[v - 1000];
}

Console.WriteLine(regs[0]);

record instr(char op, int a, int b) {
	public instr(string text) : this(default,0,0)
	{
		var sa = text.Split();

		op = sa[0][0];
		a = parse(sa[1]);
		b = sa.Length>2 ? parse(sa[2]) : 0;

		static int parse(string part) => char.IsLetter(part[0]) ? 1000 + part[0] - 'a' : int.Parse(part);
	}
}

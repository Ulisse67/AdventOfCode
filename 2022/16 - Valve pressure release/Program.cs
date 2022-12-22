#define FAST

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Extensions;


var lines = File.ReadAllLines(@"../../../input2.txt");

var valves = lines.Select(l => Valve.Parse(l)).ToDictionary(v => v.key);
var paths = compute_shortest_paths(valves);
var flows = valves.Values.Where(v => v.pressure > 0).ToArray();

const int MAXTIME = 30;

#if SLOW
int bitlen = 1 << flows.Length;
uint allbits = (uint)(bitlen - 1);

// Part1
//
var start = new ValveState(valves["AA"],visitTime: -1,openTime: 0,0,null);
int score = compute_pressure_slow(start,MAXTIME,flows,allbits);

Console.WriteLine("Max pressure from {0} valves: {1}", flows.Length, score);


// Part2
//
// This is not really optimum, because it searches 
// also combinations too small to produce a good score.
//
Dictionary<uint,int> scores_from_masks = new(bitlen);

// Try all bit combinations (
//
for(uint m=0; m<bitlen; m++) {
	var sc = compute_pressure_slow(start,MAXTIME-4,flows,m);
	scores_from_masks[m] = sc;
}

score = 0;

for(uint m = 0; m < bitlen - 1; m++) {
	uint m0 = ~m & allbits;

	var s0 = scores_from_masks[m];
	var s1 = scores_from_masks[m0];

	var sum = s0+s1;
	if(sum>score) score = sum;
}

Console.WriteLine("Max pressure from {0} valves operated in 2 groups: {1}",flows.Length,score);
#endif


// Learned a faster way to keep information using a mask
// and avoiding computing it again for different sets of valves

#if FAST
Dictionary<uint,int> answers = new();

compute_pressure(valves["AA"],MAXTIME,0,0);

var maxscore = answers.Values.Max();

Console.WriteLine(maxscore);

answers.Clear();
compute_pressure(valves["AA"],MAXTIME - 4,0,0);

var fn = from s0 in answers
				 from s1 in answers
				 where (s0.Key & s1.Key) == 0 // only sums of different valves
				 select s0.Value + s1.Value;

maxscore = fn.Max();

Console.WriteLine(maxscore);


void compute_pressure(Valve v, int maxtime, uint state, int totalOutput)
{
	answers[state] = Math.Max(answers.GetValueOrDefault(state,0), totalOutput);

	foreach(var vv in flows) {
		if((vv.BitMask & state) != 0) continue; // already visited

		var newbudget = maxtime - paths![v.Index, vv.Index] - 1;

		if(newbudget <= 0) continue;

		compute_pressure(vv, newbudget, state | vv.BitMask, totalOutput + newbudget * vv.pressure);
	}
}
#endif

#if SLOW
int compute_pressure_slow(ValveState start, int maxtime, IEnumerable<Valve> valveset, uint mask)
{
	Queue<ValveState> processing = new();
	int maxscore = 0;

	processing.Enqueue(start);

	while(processing.TryDequeue(out var st)) {
		int output = st.totalOutput + st.Output;
		maxscore = Math.Max(maxscore,output);

		foreach(var v in valveset) {
			if((mask & v.BitMask) == 0) continue;

			if(st.FindFor(v) != null) continue;

			var shortest = paths[st.valve.Index,v.Index];
			var visit_time = st.visitTime + 1 + shortest;

			if(visit_time + 1 < maxtime) {
				var vs = new ValveState(v,visitTime: visit_time,openTime: maxtime - (visit_time + 1),output,st);
				processing.Enqueue(vs);
			}
		}
	}

	return maxscore;
}
#endif


static int[,] compute_shortest_paths(IReadOnlyDictionary<string,Valve> dict)
{
	// Floyd-Warshall algorithm
	//
	int n = dict.Count;
	var paths = new int[n,n];

	paths.Fill(100000);

	// distance to self = 0
	//
	for(int i = 0; i < n; i++) paths[i,i] = 0;

	// distance to direct successors = 1	
	//
	int row = 0;
	foreach(var v in dict.Values) {
		foreach(var k in v.successors) {
			var vv = dict[k];
			paths[row,vv.Index] = 1;
		}

		row++;
	}

	// other distances are sums of path segments
	//
	for(int k = 0; k < n; k++)
		for(int i = 0; i < n; i++)
			for(int j = 0; j < n; j++)
				paths[i,j] = Math.Min(paths[k,i] + paths[j,k],paths[i,j]);

	return paths;
}


record class Valve(string key, int pressure, string[] successors) {
	public static Valve Parse(string text)
	{
		var m = matcher.Match(text);
		var vlist = m.Groups[4].Captures.Select(c=>c.Value).ToArray();

		var v = new Valve(m.Groups[1].Value, int.Parse(m.Groups[2].ValueSpan), vlist);
		if(v.pressure > 0) v.BitMask = 1U << maskIndex++;
		return v;
	}

	public readonly int Index = creationIndex++;
	public uint BitMask;

	static int creationIndex, maskIndex;

	static readonly Regex matcher = new(@"Valve (\w\w) has flow rate=(\d+);.+valve[s]? ((\w\w)(, )?)+");
}

#if SLOW
record class ValveState(Valve valve, int visitTime, int openTime, int totalOutput, ValveState? parent) {
	public int Output => valve.pressure * openTime;

	public ValveState? FindFor(Valve v)
	{
			for(var p = this; p != null; p = p.parent) if(p.valve == v) return p;
			return null;
	}
}
#endif

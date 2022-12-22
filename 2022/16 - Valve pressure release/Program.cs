using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

using Extensions;


var lines = File.ReadAllLines(@"../../../input.txt");

var valves = lines.Select(l => Valve.Parse(l)).ToDictionary(v => v.key);
var paths = compute_shortest_paths(valves);
var flows = valves.Values.Where(v => v.pressure > 0).ToArray();

int bitlen = 1 << flows.Length;
uint allbits = (uint)(bitlen - 1);

// Part1
//
const int MAXTIME = 30;

var start = new ValveState(valves["AA"],visitTime: -1,openTime: 0,0,null);
int score = compute_pressure(start,MAXTIME,flows,allbits);

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
	var sc = compute_pressure(start,MAXTIME-4,flows,m);
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


int compute_pressure(ValveState start, int maxtime, IEnumerable<Valve> valveset, uint mask)
{
	Queue<ValveState> processing = new();
	int maxscore = 0;

	processing.Enqueue(start);

	while(processing.TryDequeue(out var st)) {
		int output = st.totalOutput + st.Output;
		maxscore = Math.Max(maxscore,output);

		foreach(var v in valveset) {
			if((mask & 1 << v.BitIndex) == 0) continue;

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
		if(v.pressure > 0) v.BitIndex = maskIndex++;
		return v;
	}

	public readonly int Index = creationIndex++;
	public int BitIndex;

	static int creationIndex, maskIndex;

	static readonly Regex matcher = new(@"Valve (\w\w) has flow rate=(\d+);.+valve[s]? ((\w\w)(, )?)+");
}

record class ValveState(Valve valve, int visitTime, int openTime, int totalOutput, ValveState? parent) {
	public int Output => valve.pressure * openTime;

	public ValveState? FindFor(Valve v)
	{
			for(var p = this; p != null; p = p.parent) if(p.valve == v) return p;
			return null;
	}
}

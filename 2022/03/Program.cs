using System.Numerics;

var lines = File.ReadAllLines(@"../../../input.txt");
int[] freq = new int[26*2];
int sum = 0;

//foreach(var line in lines) {
//	Array.Fill(freq, 0);
//	var len = line.Length / 2;

//	foreach(var ch in line[..len]) {
//		int ix = ch < 'a' ? 26 + ch - 'A' : ch - 'a';
//		freq[ix]++;
//	}

//	foreach (var ch in line[len..]) {
//		int ix = ch < 'a' ? 26 + ch - 'A' : ch - 'a';
//		if (freq[ix] > 0) {
//			sum += 1+ix;
//			break;
//		}
//	}
//}


for(int pos=0; pos<lines.Length; pos+=3) {
	ulong mask = 0xfffffffffffff;

	foreach (var line in lines[pos..(pos+3)]) {
		Array.Fill(freq, 0);
		ulong fill = 0;

		foreach (var ch in line) {
			int ix = ch < 'a' ? 26 + ch - 'A' : ch - 'a';
			freq[ix]++;

			fill |= 1UL<<ix;
		}

		mask &= fill;
	}

	var bit = BitOperations.TrailingZeroCount(mask);
	sum += 1 + bit;
}

Console.WriteLine(sum);

var line = File.ReadAllText(@"../../../input.txt");

int length = 14; // part 1: 4; part 2: 14
var marker = new char[length];
int at = 0, ix = 0;

for(; ix<line.Length && at<length; ) {
	var ch = line[ix++];
	var pos = Array.IndexOf(marker, ch, 0, at);

	if (pos >= 0) {
		at -= pos + 1;
		Array.Copy(marker, pos+1, marker, 0, at);
	}

	marker[at++] = ch;
}

Console.WriteLine(ix);

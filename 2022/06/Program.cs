using System.Xml.Schema;

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

// second version: no copies, jusr scans on slices
//
for(int pos=0; pos <= line.Length - length; ) {
	var view = line[pos..(pos+length)];

	if (scan()) { pos += length; break; }

	bool scan()
	{
		for (int x = 0; x < view.Length-1; x++) {
			var ch = view[x];
			for (int y = x + 1; y < view.Length; y++) {
				if (ch == view[y]) {
					pos += x + 1;
					return false;
				}
			}
		}

		return true;
	}
}

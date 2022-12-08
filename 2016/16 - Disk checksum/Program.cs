using System.Text;

var input = "10011111011011001";
var length = 35651584;// 272;
var buffer = new StringBuilder(input, length);

while(buffer.Length < length) {
	Span<char> dup = new(buffer.ToString().ToCharArray());
	for (int i = 0; i <= dup.Length / 2; i++) {
		var ch = xor(dup[i]);
		dup[i] = xor(dup[^(1+i)]);
		dup[^(1+i)] = ch;

		static char xor(char ch) => (char)('0'+(1 - (ch - '0')));
	}

	buffer.Append('0').Append(dup);
}

var checksum = new char[buffer.Length / 2];
var digits = buffer.ToString(0, length);

for (; ; ) {
	int dst = 0;
	for (int at = 0; at < length; at += 2) {
		var ch = digits[at..(at + 2)] switch {
			"00" => '1',
			"01" => '0',
			"10" => '0',
			"11" => '1',
		};

		checksum[dst++] = ch;
	}

	if ((dst & 1) == 1) {
		ReadOnlySpan<char> res = new(checksum, 0, dst);
		Console.WriteLine(res.ToString());

		break;
	}

	length = dst;
	digits = new string(checksum, 0, length);
}

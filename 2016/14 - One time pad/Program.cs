using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

MD5 hasher = MD5.Create();
Regex triplematcher = new(@"(.)\1\1", RegexOptions.Compiled);
Dictionary<int,HashKey> candidates = new();
List<HashKey> confirmed = new();

var inp = Encoding.UTF8.GetBytes("cuanljph000000000");
var (seedsz,numsz) = (8,1);
var num = new Span<byte>(inp, seedsz, numsz);

for (int ix = 0; ; ix++) {
	candidates.Remove(ix - 1001);

	var hash = hasher.ComputeHash(inp, 0, seedsz + numsz);
	var txt = Convert.ToHexString(hash);

	txt = stretch_key(txt, 2016);

	if (retrieve_keys(txt, 64)) break;

	var found = triplematcher.Match(txt);
	if (found.Success) {
		candidates.Add(ix, new(found.ValueSpan[0], ix));
	}

	if(!bump(num)) {
		numsz++;
		num = new Span<byte>(inp, seedsz, numsz);
		num[0] = (byte)'1';
	}

	string stretch_key(string key, int loops)
	{
		for (int i = loops; i>0; i--) {
			var inp = Encoding.UTF8.GetBytes(key.ToLower());
			var hash = hasher.ComputeHash(inp);
			key = Convert.ToHexString(hash);
		}

		return key;
	}

	bool retrieve_keys(string match, int maxcnt)
	{
		var keys = candidates.Where(kv => kv.Value.IsMatch(match)).OrderBy(kv => kv.Key).ToArray();
		foreach (var kv in keys) {
			confirmed.Add(kv.Value);
			candidates.Remove(kv.Key);

			if (confirmed.Count == maxcnt) return true;
		}

		return false;
	}
}

Console.WriteLine(confirmed[63].hashNumber);

static bool bump(Span<byte> digits)
{
	int at = digits.Length - 1;
	while (at >= 0) {
		digits[at]++;
		if (digits[at] <= '9') break;

		digits[at] = (byte)'0';
		at--;
	}

	return at >= 0;
}

record HashKey(char key, int hashNumber) {
	readonly string five = new string(key, 5);

	public bool IsMatch(string text) => text.Contains(five);
}

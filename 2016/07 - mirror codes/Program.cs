// See https://aka.ms/new-console-template for more information

string[] testdata = { "aba[bab]xyz", "xyx[xyx]xyx", "aaa[kek]eke", "zazbz[bzb]cdb" };//"abba[mnop]qrst", "abcd[bddb]xyyx", "aaaa[qwer]tyui", "ioxxoj[asdfgh]zxcvbn" };

var input = File.ReadAllLines(@"..\..\..\input.txt");
var count = 0;
//input = testdata;
int c0 = 0, c1 = 0, c2 = 0;

foreach(var line in input) {
    //ReadOnlySpan<char> slice = line;
    //bool b0 = false;

    //for(; ; ) {
    //    var bracket = slice.IndexOf('[');

    //    if(bracket >= 0) {
    //        var before = slice[..bracket];
    //        b0 |= is_mirrored(before);

    //        slice = slice[(bracket + 1)..];

    //        bracket = slice.IndexOf(']');
    //        var between = slice[..bracket];
    //        var b1 = is_mirrored(between);

    //        if(b1) { b0 = false; break; } // at least one [] is mirrored

    //        slice = slice[(bracket + 1)..];
    //    }
    //    else {
    //        b0 |= is_mirrored(slice);
    //        break;
    //    }
    //}

    //if(b0) count++;

    var parts = line.Split(new[] { '[', ']' });

    if(is_aba_bab()) count++;

    bool is_aba_bab()
    {
        for(int i = 0; i < parts.Length; i += 2) {
            var sl = parts[i].AsSpan();

            while(is_aba(sl, out var pos)) {
                var bab = new string(new[] { sl[pos+1], sl[pos], sl[pos+1] });

                for(int j = 1; j < parts.Length; j += 2) {
                    if(parts[j].Contains(bab, StringComparison.Ordinal)) {
                        return true;
                    }
                }

                sl = sl[(pos+1)..];
            }
        }

        return false;
    }
    //var (b0, b1, b2) = (is_mirrored(parts[0]), is_mirrored(parts[1]), is_mirrored(parts[2]));

    //c0 += b0?1:0;
    //c1 += b1?1:0;
    //c2 += b2?1:0;

    //if((b0 || b2) && !b1) {
    //    Console.WriteLine("{0} - {1} - {2}", parts);
    //    count++;
    //}
}

Console.WriteLine(count);

bool is_aba(ReadOnlySpan<char> text, out int pos)
{
    for(int at = 0; at < text.Length - 2; at++) {
        if(text[at] == text[at+2] && text[at] != text[at+1]) {
            pos = at;
            return true;
        }
    }

    pos = -1;
    return false;
}

bool is_mirrored(ReadOnlySpan<char> text)
{
    // abba[mnop]qrst
    for(int i = 0; i < text.Length - 3; i++) {
        if(text[i] == text[i + 1]) continue;
        if(text[i] == text[i + 3] && text[i + 1] == text[i + 2]) {
            //Console.WriteLine(text[i..(i+4)]);
            return true;
        }
    }

    return false;
}

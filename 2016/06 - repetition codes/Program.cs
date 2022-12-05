// See https://aka.ms/new-console-template for more information

var input = File.ReadAllLines(@"..\..\..\input.txt");
var len = input[0].Length;

var message = new freq[len][];
for(int col = 0; col < message.Length; col++) {
    var m = new freq[26];
    var key = 'a';

    for(int row = 0; row < m.Length; row++) {
        m[row] = new(key++, 0);
    }

    message[col] = m;
}

foreach(var line in input) {
    for(int col = 0; col < message.Length; col++)
        message[col][line[col]-'a'].count++;
}

ReadOnlySpan<char> code = message.Select(m => m.Min()).Select(m => m.ch).ToArray();

Console.WriteLine(code.ToString());

record struct freq(char ch, int count) : IComparable<freq> {
    public int CompareTo(freq other) => count.CompareTo(other.count);
}

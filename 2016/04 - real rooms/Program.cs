// See https://aka.ms/new-console-template for more information
var lines = File.ReadAllLines(@"..\..\..\input.txt");
var counts = new (char ch, int freq)[26];
long id_sum = 0;
List<string> decoded_words = new();

foreach(var line in lines) {
    int postminus = line.LastIndexOf('-')+1;
    int bracket = line.IndexOf('[', postminus);

    var idpart = line[postminus..bracket];
    var id = int.Parse(idpart);

    var chk = line[(bracket+1)..^1];
    for(int i = 0; i < 26; i++) counts[i] = ((char)('a' + i), 0);

    foreach(var ch in line[..postminus]) {
        if(ch != '-') counts[ch-'a'].freq++;
    }

    Array.Sort(counts, (a,b) =>
        a.freq>b.freq
        ? -1
        : a.freq < b.freq
          ? 1
          : a.ch > b.ch ? 1 : -1
    );

    var ix = 0;
    var is_real = chk.All(ch => ch == counts[ix++].ch);

    if(is_real) {
        id_sum += id;
        Span<char> dec = line.ToCharArray(0,postminus);
        for(int i = 0; i < dec.Length; i++) { var ch = dec[i]; dec[i] = ch == '-' ? ' ' : (char)('a' + (ch - 'a' + id) % 26); }

        var decoded = new string(line[..postminus].Select(ch => ch == '-' ? ' ' : (char)('a' + (ch - 'a' + id) % 26)).ToArray());
        decoded_words.Add(decoded);
        if(decoded.StartsWith("north"))
            id = id;
        //if(line[5] == '-') { //}&& line[10] == '-') {
        //    var north = line[..5].Select(ch => (char)('a'+(ch - 'a' + id) % 26));
        //    var pole = line[6..10].Select(ch => (char)('a' + ((ch - 'a') + id) % 26));

        //    if(north.SequenceEqual("north"))  id = 0;
        //}
    }
}

Console.WriteLine(id_sum);

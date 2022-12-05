var lines = File.ReadAllLines(@"../../../input.txt");

int crates_num = (lines[0].Length + 1) / 4;
var crates = new Stack<char>[crates_num];
for (int i = 0; i < crates_num; crates[i++] = new()) ;

int ix = 0;
while (lines[ix][0..2] != " 1") ix++;

// fill the crates

for(int iy=ix; iy>0; )
{
    var line = lines[--iy];
    for(int cn=0, at=0; cn<crates_num; cn++, at += 4) {
        var ch = line[at + 1];
        if (ch != ' ') crates[cn].Push(ch);
    }
}

ix += 2; // instructi0ons start here

foreach(var line in lines[ix..]) {
    var sa = line.Split(' ');
    var (qt, from, to) = (int.Parse(sa[1]), int.Parse(sa[3])-1, int.Parse(sa[5])-1);
    var add = new Stack<char>(qt);

    for(int q=qt; q>0; q--) add.Push(crates[from].Pop());
    foreach (var ch in add) crates[to].Push(ch);
}

var res = new string(crates.Select(c=> c.Peek()).ToArray());

Console.WriteLine(res);

// See https://aka.ms/new-console-template for more information
var lines = File.ReadAllLines(@"..\..\..\input.txt")
    .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    .Select(s => int.Parse(s)).ToArray());

var sides = lines.Select(v => v.ToList());
var t3 = new int[3][];
t3[0] = new int[3];
t3[1] = new int[3];
t3[2] = new int[3];

var row = 0;
int count = 0;

foreach(var line in lines) {
    if(row < 3) {
        t3[0][row] = line[0];
        t3[1][row] = line[1];
        t3[2][row] = line[2];

        row++;
    }

    if(row > 2) {
        row = 0;

        for(int i = 0; i < 3; i++) {
            Array.Sort(t3[i]);
            if(t3[i][0] + t3[i][1] > t3[i][2]) count++;
        }
    }
}


foreach(var side in sides) {
    side.Sort();
    if(side[0] + side[1] > side[2]) count++;
}

Console.WriteLine(count);

using System.Text;

//var d = new Matrix(7, 3);
//d.Fill(7, 3, '.');
//d.Fill(3, 2, '#');
//d.RotateDown(1, 1);
//d.RotateRight(0, 4);
//d.RotateDown(1, 1);

var input = File.ReadAllLines(@"..\..\..\input.txt");
var display = new Matrix(50, 6);
display.Fill(50, 6, '.');

foreach(var line in input) {
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    switch(parts[0]) {
    case "rect":
        var argv = parts[1].Split('x').Select(d => int.Parse(d)).ToArray();
        display.Fill(argv[0], argv[1], '#');

        break;
    case "rotate":
        var arg0 = int.Parse(parts[2].Split('=')[1]);
        var arg1 = int.Parse(parts[4]);

        if(parts[1] == "row") display.RotateRight(arg0, arg1);
        else display.RotateDown(arg0, arg1);

        break;

    default: break;
    }
}

Console.WriteLine(display.Count('#'));
Console.WriteLine(display);

record Matrix(int width, int height) {
    char[,] buffer = new char[width, height];
    char[] rowbuf = new char[width], colbuf = new char[height];

    public void Fill(int wx, int wy, char ch)
    {
        for(int x=0; x<wx; x++) {
            for(int y=0; y<wy; y++) {
                buffer[x, y] = ch;
            }
        }
    }

    public void RotateRight(int row, int steps)
    {
        for(int src=width - steps, dst=0; src<width; ) rowbuf[dst++] = buffer[src++, row];
        for(int src = width - steps, dst = width; src > 0;) buffer[--dst, row] = buffer[--src, row];
        for(int src = 0; src < steps; src++) buffer[src, row] = rowbuf[src];
    }

    public void RotateDown(int col, int steps)
    {
        for(int src=height - steps, dst=0; src<height; ) colbuf[dst++] = buffer[col, src++];
        for(int src= height - steps, dst = height; src > 0;) buffer[col, --dst] = buffer[col, --src];
        for(int src = 0; src < steps; src++) buffer[col, src] = colbuf[src];
    }

    public int Count(char ch)
    {
        int res = 0;
        for(int row = 0; row < height; row++) {
            for(int col = 0; col < width; col++) {
                if(buffer[col, row] == ch) res++;
            }
        }

        return res;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for(int row = 0; row < height; row++) {
            for(int col = 0; col < width; col++) {
                sb.Append(buffer[col, row]);
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

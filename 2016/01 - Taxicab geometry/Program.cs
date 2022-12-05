// See https://aka.ms/new-console-template for more information
var directions = File.ReadAllText(@"..\..\..\input.txt")
                         .Split(", ", StringSplitOptions.RemoveEmptyEntries);
var (x, y) = (0, 0);
var dir = 'N'; // North

var states = new Dictionary<string, action> {
    { "NR", new('E',1,0) },
    { "NL", new('W',-1,0) },
    { "ER", new('S',0,-1) },
    { "EL", new('N',0,1) },
    { "SR", new('W',-1,0) },
    { "SL", new('E',1,0) },
    { "WR", new('N',0,1) },
    { "WL", new('S',0,-1) },
};

List<vec> vectors = new();

foreach (var direction in directions) {
    var len = int.Parse(direction[1..]);
    var key = $"{dir}{direction[0]}";

    if(!states.TryGetValue(key, out var st))
        continue;

    vec v = new(x, y, len, st.xd, st.yd);
    
    foreach(var vv in vectors) {
        if(vv.intersect(v)) {
            var (xsect, ysect) = (v.horiz ? vv.x0 : v.x0, v.horiz ? v.y0 : vv.y0);
            break;
        }
    }

    vectors.Add(v);

    dir = st.newdir;
    x += st.xd * len;
    y += st.yd * len;
}

Console.WriteLine($"{x}, {y}, {Math.Abs(x) + Math.Abs(y)}");

record action(char newdir, int xd, int yd);
record vec {
    static int sequence;

    public vec(int x, int y, int delta, int xmove, int ymove)
    {
        seq = sequence++;
        horiz = xmove != 0;

        if(horiz) {
            if(xmove < 0) (x0, x1) = (x + xmove * delta, x);
            else (x0, x1) = (x, x + xmove * delta);

            y0 = y1 = y;
        }
        else {
            if(ymove < 0) (y0,y1) = (y + ymove * delta, y);
            else (y0,y1) = (y, y + ymove * delta);

            x0 = x1 = x;
        }
    }

    readonly int seq;

    public readonly int x0,x1,y0,y1;
    public readonly bool horiz;

    public bool intersect(vec v)
    {
        if(horiz == v.horiz || seq == v.seq -1) return false;

        if(horiz) {
            return x0 <= v.x0 && v.x0 <= x1 && v.y0 <= y0 && y0 <= v.y1;
        }
        else {
            return v.x0 <= x0 && x0 <= v.x1 && y0 <= v.y0 && v.y0 <= y1;
        }
    }
}

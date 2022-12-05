// See https://aka.ms/new-console-template for more information

var lines = File.ReadAllLines(@"..\..\..\input.txt");
long run_length = 0;
int at = 0;

foreach(var line in lines) {
    run_length = compute(line);

    while(at < line.Length) {
        int next_marker = line.IndexOf('(', at);
        if(next_marker == -1) {
            run_length += line.Length - at;
            continue;
        }

        run_length += next_marker - at;
        next_marker++;

        int end = line.IndexOf(')', next_marker);
        var counts = line[next_marker .. end].Split('x').Select(n => int.Parse(n)).ToArray();

        run_length += counts[0] * counts[1];
        at = end+1 + counts[0];
    }

    long compute(ReadOnlySpan<char> segment)
    {
        long run = 0;

        for(; ; ) {
            if(segment.IsEmpty) return run;

            int next = segment.IndexOf('(');
            if(next == -1) return run + segment.Length;

            run += next;
            next++;
            int end = segment.IndexOf(')');
            int x = segment.IndexOf('x');
            var (len,times) = ( int.Parse(segment[next..x]), int.Parse(segment[(x+1)..end]) );

            end++;
            run += compute(segment[end .. (end + len)]) * times;
            segment = segment.Slice(end+len);
        }
    }
}

Console.WriteLine(run_length);

// See https://aka.ms/new-console-template for more information

const string path = @"..\..\..";
var lines = File.ReadAllLines($"{path}/input.txt");

Dictionary<string, int> outputs = new();
Dictionary<string, Bot> bots = new();

var tokenized = lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries));

//value 23 goes to bot 208
//bot 125 gives low to bot 58 and high to bot 57
//bot 131 gives low to output 6 and high to bot 151

foreach(var tokens in tokenized) {
    if(tokens.Length < 6 || tokens[0] != "bot") continue;

    if(bots.TryGetValue(tokens[1], out var bot)) continue;

    bots.Add(tokens[1], new Bot(tokens[1], tokens[6], tokens[11], tokens[5] == "output", tokens[10] == "output"));
}

Console.WriteLine($"Bots: {bots.Count}");
State machine = new(bots, outputs);

foreach(var tokens in tokenized) {
    if(tokens.Length < 6 || tokens[0] != "value") continue;

    var bot = bots[tokens[5]];

    bot.Add(machine, tokens[1]);
    //bot.Next(bots, outputs);
}

Console.WriteLine($"outputs: {outputs.Count}");

record State(Dictionary<string,Bot> bots, Dictionary<string,int> outputs);

record Bot(string key, string low, string high, bool l_out, bool h_out) {
    List<int> values = new();

    public bool Add(State st, string value) => Add(st, int.Parse(value));

    public bool Add(State st, int value)
    {
        values.Add(value);
        if(values.Count == 2) {
            var (v0, v1) = (values.Min(), values.Max());

            if(v1 == 61 && v0 == 17)
                v1 = v1;

            if(l_out) st.outputs[low] = v0; else st.bots[low].Add(st, v0);
            if(h_out) st.outputs[high] = v1; else st.bots[high].Add(st, v1);

            //values.Clear();
            return true;
        }

        return false;
    }
}

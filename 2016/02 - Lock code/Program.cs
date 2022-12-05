// See https://aka.ms/new-console-template for more information
var lines = File.ReadAllLines(@"..\..\..\input.txt");
var moves = new Dictionary<string, string> {
    { "1R", "2" },
    { "1D", "4" },
    { "2L", "1" },
    { "2R", "3" },
    { "2D", "5" },
    { "3L", "2" },
    { "3D", "6" },
    { "4U", "1" },
    { "4R", "5" },
    { "4D", "7" },
    { "5U", "2" },
    { "5D", "8" },
    { "5L", "4" },
    { "5R", "6" },
    { "6U", "3" },
    { "6L", "5" },
    { "6D", "9" },
    { "7U", "4" },
    { "7R", "8" },
    { "8L", "7" },
    { "8R", "9" },
    { "8U", "5" },
    { "9L", "8" },
    { "9U", "6" },
};

var moves_alpha = new Dictionary<string, string> {
    { "1D", "3" },
    { "2D", "6" },
    { "2R", "3" },
    { "3U", "1" },
    { "3D", "7" },
    { "3L", "2" },
    { "3R", "4" },
    { "4L", "3" },
    { "4D", "8" },
    { "5R", "6" },
    { "6U", "2" },
    { "6D", "A" },
    { "6L", "5" },
    { "6R", "7" },
    { "7U", "3" },
    { "7D", "B" },
    { "7L", "6" },
    { "7R", "8" },
    { "8U", "4" },
    { "8D", "C" },
    { "8L", "7" },
    { "8R", "9" },
    { "9L", "8" },
    { "AU", "6" },
    { "AR", "B" },
    { "BU", "7" },
    { "BD", "D" },
    { "BL", "A" },
    { "BR", "C" },
    { "CU", "8" },
    { "CL", "B" },
    { "DU", "B" },

};

var start = "5";
var code = "";

foreach(var line in lines) {
    foreach(var ch in line) {
        var key = start + ch;

        if(moves_alpha.TryGetValue(key, out var move)) {
            start = move;
        }
        else continue;
    }

    code += start;
}

Console.WriteLine(code);

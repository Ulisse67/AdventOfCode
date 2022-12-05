// See https://aka.ms/new-console-template for more information
var lines = File.ReadLines(@"../../../input.txt");
long calories = 0, sum = 0;
List<long> heap = new();

foreach(var line in lines) {
    if(line.Length == 0) {
        heap.Add(sum);
        if (sum > calories) calories = sum;
        sum = 0;
    }
    else sum += int.Parse(line);
}

heap.Sort();
var sum3 = heap.TakeLast(3).Sum();
Console.WriteLine(calories);

// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;

MD5 hasher = MD5.Create();

//var buffer = new byte[8+7];
//var inp = new Span<byte>(buffer);
var inp = Encoding.UTF8.GetBytes("wtnhxymk0000000");
var num = new Span<byte>(inp, 8, 7);

var pwd = new byte[8];
Array.Fill<byte>(pwd, 0xff);

for(var px=0; px<8 ; ) {
    var hash = hasher.ComputeHash(inp);
    if(hash[0] == 0 && hash[1] == 0 && (hash[2] & 0xf0) == 0) {
        var at = hash[2] & 0x0f;
        if(at < 8 && pwd[at] == 0xff) {
            pwd[at] = (byte)(hash[3] >>> 4);
            px++;
        }
        //pwd[px++] = (byte)(hash[2] & 0x0f);
    }

    if(!bump(num)) {
        inp = Encoding.UTF8.GetBytes("wtnhxymk10000000");
        num = new Span<byte>(inp, 8, 8);
    }
}

Console.WriteLine(Convert.ToHexString(pwd));

static bool bump(Span<byte> digits)
{
    int at=digits.Length-1;
    while(at >= 0) {
        digits[at]++;
        if(digits[at] <= '9') break;

        digits[at] = (byte)'0';
        at--;
    }

    return at >= 0;
}

<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 21

    /*  The input instructions translate to the following

        while (true)
        {
            r5 = 123;                // instr 0
            r5 = r5 & 456;           // 1
            r5 = r5 == 72 ? 1 : 0;   // 2
            if (r5 == 1)             // 3
                break;
        }

        r5 = 0;  // instr 5

        while (true)
        {
            r4 = r5 | 65536;             // 6
            r5 = 13284195;               // 7

            while (true)
            {
                r3 = r4 & 255;           // 8
                r5 = r5 + r3;            // 9
                r5 = r5 & 16777215;      // 10
                r5 = r5 * 65899;         // 11
                r5 = r5 & 16777215;      // 12
                r3 = 256 > r4 ? 1 : 0;   // 13
                if (r3 == 1)             // 14 -> goto 16 -> goto 28
                {
                    break;
                }
                else
                {
                    r3 = 0;              // 17
                    while (true)
                    {
                        r2 = r3 + 1;     // 18
                        r2 = r2 * 256;   // 19
                        r2 = r2 > r4 ? 1 : 0;  // 20
                        if (r2 == 1)     //21 -> goto 23
                        {
                                         // 23 -> goto 26
                            r4 = r3;     // 26
                            break;
                        }
                        else // 22 -> goto 24
                        {
                            r3 = r3 + 1; // 24
                                         // 25 -> goto 18
                        }
                    }
                }
            }

            r3 = r5 == r0 ? 1 : 0;     // 28
            if (r3 == 1)               // 29  
                break;
        }
    */
    // Which can then be rewritten and simplified as:

    int r4 = 0, r5 = 0;
    // Part 1: Run the program through with r0 = 0, set a breakpoint at instruction 28 "r3 = r5 == r0 ? 1 : 0;"
    // and inspect the value of r5. This is what we need to initialise r0 to in order to exit 
    // as quickly as possible. After one run we see that: r5 = 7224964; = part 1 answer

    // Part 2: We need to watch for when the r5 value starts to repeat. The r5 before this point
    // will be the one that has executed the most instructions. Any future values will already 
    // have been seen and, thus, have taken fewer instructions to get there.
    int count = 0;   
    var seen = new Dictionary<int, int>();
    
    int r0 = 0;
    while (true)
    {
        count++;
        r4 = r5 | 65536;             // 6
        r5 = 13284195;               // 7

        while (true)
        {
            r5 = r5 + (r4 & 255);
            r5 = ((r5 & 16777215) * 65899) & 16777215;      // 12
            
            if (r4 < 256)
                break;
            
            r4 = r4 / 256;
        }

        // Original code
        if (r5 == r0)
            break;
            
        // Part 2
        if (seen.ContainsKey(r5))
            break;
        seen.Add(r5, count);        
    }

    var seeninorder = seen.OrderBy(k => k.Value);
    // Part 1 is the first r5 value seen
    Console.WriteLine($"Part 1: Program will exit asap if register 0 is set to {seeninorder.First().Key}");

    // The part 2 result is the last r[5] we saw before it started to loop
    Console.WriteLine($"Part 2: {seeninorder.Last().Key}");

    //seeninorder.Skip(seen.Count() - 20).Dump("Last 20 items");
}


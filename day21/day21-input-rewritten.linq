<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 21

    int r0 = 0, r1 = 0, r2 = 0, r3 = 0, r4 = 0, r5 = 0, r6 = 0;

    // This is the input rewritten in a "higher level language" - even though it has more lines!
    // Run the program through with r0 = 0, set a breakpoint at instruction 28 "r3 = r5 == r0 ? 1 : 0;"
    // and inspect the value of r5. This is what we need to initialise r0 to in order to exit 
    // as quickly as possible.
    // After one run we see that:
    r0 = 7224964;
    
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
}

<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018
    // Day 19 
    // Refactored into a high level language
    
    var A = 0;
    var B = 1;
    //var F = 876;     // Part 1
    var F = 10551276;  // Part 2

    // Rewrite the original program as is
    //    while (B <= F)
    //    {
    //        var C = 1;
    //        while (C <= F)
    //        {
    //            D = B * C;
    //            if (D == F)
    //                A = A + B;
    //            C++;
    //        }
    //        
    //        B++;
    //    }

    // Turns out it's just a slow way of finding factors and then summing them

    // Optimised a bit
    while (B * B <= F)
    {
        if (F % B == 0)
            A = A + B + (F / B);
            
        B++;
    }

    Console.WriteLine($"Result: {A}");
}

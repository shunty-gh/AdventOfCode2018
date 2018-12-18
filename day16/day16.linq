<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationTypes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\System.Windows.Input.Manipulations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationProvider.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Namespace>System.Windows</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 16

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day16-input.txt");
    var input = File.ReadAllLines(inputname).ToList();
    var part2index = input.Select((s, i) => (s, i)).Where(s => s.s.StartsWith("After")).Select(s => s.i).Max(i => i) + 1;
    var opcodes = new string[] { "addr", "addi", "mulr", "muli", "banr", "bani", "borr", "bori", "setr", "seti", "gtir", "gtri", "gtrr", "eqir", "eqri", "eqrr" };

    // Part 1
    var index = 0;
    var matches = new Dictionary<int, List<string>>();
    var cannotmatchI = new Dictionary<int, List<(string, int)>>();
    var matchesThreeOrMore = new List<int>();
    while (index < part2index)
    {
        // Read 3 lines -> Before, Instruction, After
        var before = ParseSample(input[index]);
        (int Op, int A, int B, int C) instruction = ParseInstruction(input[index + 1]);
        var after = ParseSample(input[index + 2]);

        // Apply each opcode type to the 'before' and see if it gives the 'after' value
        var mcount = 0;
        foreach (var op in opcodes)
        {
            if (IsOpcodeMatch(op, before, instruction, after))
            {
                mcount++;
                if (matches.ContainsKey(instruction.Op))
                {
                    matches[instruction.Op].Add(op);
                }
                else
                {
                    matches[instruction.Op] = new List<string> { op };
                }
            }
            else
            {
                if (cannotmatchI.ContainsKey(instruction.Op))
                    cannotmatchI[instruction.Op].Add((op, index));
                else
                    cannotmatchI[instruction.Op] = new List<(string, int)> { (op, index) };
            }
        }
        if (mcount >= 3)
        {
            matchesThreeOrMore.Add(index);
        }

        // next (and skip a line)
        index += 4;
    }
    
    var part1 = matchesThreeOrMore.Count;
    Console.WriteLine($"Part 1: {part1}");

    // A visual inspection of the cannotMatchI output is quicker than programming it
//    cannotmatchI
//        .OrderBy(k => k.Key)
//        .Select(k => (k.Key, k.Value
//            .Select(v => v.Item1)
//            .Distinct()
//            .OrderBy(v => v))).Dump();
            
    var opmap = new Dictionary<int, string>();
    opmap.Add( 4, "eqir");
    opmap.Add( 8, "addr");
    opmap.Add(10, "muli");
    opmap.Add(12, "addi");
    opmap.Add(14, "mulr");
    opmap.Add( 5, "bori"); // "addi | bori"
    opmap.Add( 9, "borr"); // "addr | bori | borr"
    opmap.Add( 6, "seti"); // "addi | bori | borr | seti"
    opmap.Add( 2, "eqrr"); // "eqrr | seti"
    opmap.Add( 0, "gtri"); // "eqrr | gtri"
    opmap.Add(15, "gtrr"); // "eqir | gtri | gtrr | seti"
    opmap.Add(13, "eqri"); // "addr | borr | eqir | eqri | seti"
    opmap.Add( 3, "gtir"); // "eqir | eqri | eqrr | gtir | gtri | gtrr | seti"
    opmap.Add( 7, "setr"); // "addi | addr | bori | borr | gtir | gtri | gtrr | seti | setr"
    opmap.Add(11, "banr"); // "addi | banr | bori | borr | eqir | eqrr | gtri | mulr | seti | setr"
    opmap.Add( 1, "bani"); // "addi | addr | bani | banr | bori | borr | gtir | gtri | gtrr | muli | mulr | seti | setr"

    var registers = new int[] {0,0,0,0};
    for (index = part2index + 1; index < input.Count; index++)
    {
        var line = input[index];
        if (string.IsNullOrWhiteSpace(line))
            continue;
            
        var instruction = ParseInstruction(line);
        var op = opmap[instruction.Op];
        registers = ApplyInstruction(op, registers, instruction.A, instruction.B, instruction.C);
    }
    var part2 = registers[0];
    Console.WriteLine($"Part 2: {part2}");
}

public bool IsOpcodeMatch(string opcode, int[] before, (int Op, int A, int B, int C) instruction, int[] after)
{
    var result = ApplyInstruction(opcode, before, instruction.A, instruction.B, instruction.C);
    return (result[0] == after[0])
        && (result[1] == after[1])
        && (result[2] == after[2])
        && (result[3] == after[3]);
}

public int[] ApplyInstruction(string opcode, int[] registers, int A, int B, int C)
{
    if (registers.Length != 4)
        throw new Exception($"Invalid register input param {string.Join(",", registers)}");
        
    var result = new int[4] { registers[0], registers[1], registers[2], registers[3] };
    switch (opcode)
    {
        case "addr": // (add register) stores into register C the result of adding register A and register B
            result[C] = registers[A] + registers[B];
            break;
        case "addi": // (add immediate) stores into register C the result of adding register A and value B
            result[C] = registers[A] + B;
            break;
        case "mulr": // (multiply register) stores into register C the result of multiplying register A and register B.
            result[C] = registers[A] * registers[B];
            break;
        case "muli": // (multiply immediate) stores into register C the result of multiplying register A and value B.
            result[C] = registers[A] * B;
            break;
        case "banr": // (bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
            result[C] = (registers[A] & registers[B]);
            break;
        case "bani": // (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.        
            result[C] = (registers[A] & B);
            break;
        case "borr": // (bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
            result[C] = (registers[A] | registers[B]);
            break;
        case "bori": // (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
            result[C] = (registers[A] | B);
            break;
        case "setr": // (set register) copies the contents of register A into register C. (Input B is ignored.)
            result[C] = registers[A];
            break;
        case "seti": // (set immediate) stores value A into register C. (Input B is ignored.)
            result[C] = A;
            break;
        case "gtir": // (greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
            result[C] = A > registers[B] ? 1 : 0;
            break;
        case "gtri": // (greater - than register / immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
            result[C] = registers[A] > B ? 1 : 0;
            break;
        case "gtrr": // (greater - than register / register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
            result[C] = registers[A] > registers[B] ? 1 : 0;
            break;
        case "eqir": // (equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
            result[C] = A == registers[B] ? 1 : 0;
            break;
        case "eqri": // (equal register / immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
            result[C] = registers[A] == B ? 1 : 0;
            break;
        case "eqrr": // (equal register / register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
            result[C] = registers[A] == registers[B] ? 1 : 0;
            break;
        default:
            throw new Exception($"Unknown OpCode {opcode}");
    }
    
    return result;
}

public int[] ParseSample(string src)
{
    if (!(src.StartsWith("Before:") || src.StartsWith("After:")))
        throw new Exception($"Invalid sample line \"{src}\"");

    return src
        .Substring(8)
        .Trim(new char[] { '[', ']', ' ' })
        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => int.Parse(s.Trim()))
        .ToArray();
}

public (int Op, int A, int B, int C) ParseInstruction(string src)
{
    if (string.IsNullOrWhiteSpace(src))
        throw new Exception($"Invalid instruction line \"{src}\"");
        
    var nums = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    if (nums.Length != 4)
        throw new Exception($"Invalid instruction line \"{src}\"");
        
    return (int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]));
}

<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 21

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day21-input.txt");
    var input = File.ReadAllLines(inputname).ToList();
    //var input = TestData();

    var registers = new int[] { 0, 0, 0, 0, 0, 0 };
    var ipregister = int.Parse(input[0].Substring(3).Trim());
    var ip = 0;
    var instructions = input.Skip(1).Select(s => ParseInstruction(s)).ToList();
    var icount = instructions.Count;
    var counter = 0;
    int part1 = 0;
    
    while (ip < icount)
    {
        //Console.Write($"ip={ip} [{registers[0]}, {registers[1]}, {registers[2]}, {registers[3]}, {registers[4]}, {registers[5]}]");
        var instruction = instructions[ip];
        registers[ipregister] = ip;
        ApplyInstruction(registers, instruction);
        counter++;
        //Console.WriteLine($"  {instruction.Op} {instruction.A} {instruction.B} {instruction.C}  [{registers[0]}, {registers[1]}, {registers[2]}, {registers[3]}, {registers[4]}, {registers[5]}]");
        ip = registers[ipregister] + 1;
        
        // A review of the code shows that at instruction 28 we check r[0] against r[5]. If they're equal we stop.
        // This is the only time r[0] is involved.
        // Therefore we need to initialise r[0] to this value and the program will stop at the first opportunity.
        if (ip == 28)         
        {
            part1 = registers[5];
            break;
        }
    }
    Console.WriteLine($"Part 1: Program will exit after {counter} instructions if register 0 is set to {part1}");

}

public int[] ApplyInstruction(int[] registers, (string Op, int A, int B, int C) instruction)
{
    if (registers.Length != 6)
        throw new Exception($"Invalid register input param {string.Join(",", registers)}");

    var result = registers;
    var A = instruction.A;
    var B = instruction.B;
    var C = instruction.C;
    
    switch (instruction.Op)
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
            throw new Exception($"Unknown OpCode {instruction.Op}");
    }

    return result;
}

public (string Op, int A, int B, int C) ParseInstruction(string src)
{
    if (string.IsNullOrWhiteSpace(src))
        throw new Exception($"Invalid instruction line \"{src}\"");

    var nums = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    if (nums.Length != 4)
        throw new Exception($"Invalid instruction line \"{src}\"");

    return (nums[0], int.Parse(nums[1]), int.Parse(nums[2]), int.Parse(nums[3]));
}

public List<string> TestData()
{
    return new List<string>
    {
        "#ip 0",
        "seti 5 0 1",
        "seti 6 0 2",
        "addi 0 1 0",
        "addr 1 2 3",
        "setr 1 0 0",
        "seti 8 0 4",
        "seti 9 0 5",
    };
}
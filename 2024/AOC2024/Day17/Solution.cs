using NUnit.Framework;

namespace Day17;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo("4,6,3,5,6,3,5,2,1,0"));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt");
        Console.WriteLine(result);
    }

    [Test]
    public void Part1_InputTest1()
    {
        var result = SolvePart1("Inputs/input.txt", 2072305);
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example2()
    {
        var result = SolvePart2("Inputs/example2.txt");
        Assert.That(result, Is.EqualTo(117440));
    }

    [Test]
    public void Part2_Example3()
    {
        var result = SolvePart2("Inputs/example3.txt");
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    static string SolvePart1(string inputPath, long? regAOverride = null)
    {
        (var registers, var program) = ReadInput(inputPath);

        return ExecuteProgram(program, registers, regAOverride);
    }

    long SolvePart2(string inputPath)
    {
        (var registers, var program) = ReadInput(inputPath);

        var usedABitsPerRegister = new RegistersInABitsUsed();

        var instructionPointer = 0;
        var decrement = 0L;
        var numOfLoops = 1;
        List<int>[] usedABitsInOneLoop = [];

        while (instructionPointer < program.Count - 1)
        {
            if (program[instructionPointer] is 0)
            {
                //We assume that operand will always be literal here
                decrement += program[instructionPointer + 1];
            }
            else if (program[instructionPointer] is 3)
            {
                numOfLoops = program[instructionPointer + 1];
            }
            else if (program[instructionPointer] is 5)
            {
                usedABitsInOneLoop = program[instructionPointer + 1] switch
                {
                    4 => [[0], [1], [2]],
                    5 => usedABitsPerRegister.RegisterB[..^3],
                    6 => usedABitsPerRegister.RegisterC[..^3],
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                ExecuteOpAsBitsOfRegA(program[instructionPointer], program[instructionPointer + 1], usedABitsPerRegister);
            }

            instructionPointer += 2;
        }

        return 0;
    }

    static (Registers Registers, List<int> Program) ReadInput(string inputPath)
    {
        var rawInput = File.ReadAllLines(inputPath)
            .Where(x => x is not "")
            .Select(x => x.Split(':'))
            .Select(x => x[1].Trim())
            .ToList();

        var registers = new Registers(long.Parse(rawInput[0]), long.Parse(rawInput[1]), long.Parse(rawInput[2]));

        var programs = rawInput[3].Split(',')
            .Select(int.Parse)
            .ToList();

        return (registers, programs);
    }

    static string ExecuteProgram(List<int> program, Registers registers, long? regAOverride = null)
    {
        if (regAOverride is not null)
            registers.A = regAOverride.Value;

        var result = "";
        var instructionPointer = 0;
        while (instructionPointer < program.Count)
        {
            var currentOperation = program[instructionPointer];

            var currentOperand = program[instructionPointer + 1];

            switch (currentOperation)
            {
                case 5:
                    result += $"{ExecuteOp(currentOperation, currentOperand, registers)},";
                    break;
                case 3:
                    if (registers.A is not 0)
                    {
                        instructionPointer = currentOperand;
                        continue;
                    }
                    break;
                default:
                    ExecuteOp(currentOperation, currentOperand, registers);
                    break;
            }

            instructionPointer += 2;
        }

        return result[..^1];
    }

    static long? ExecuteOp(long opcode, int operand, Registers registers)
    {
        return opcode switch
        {
            0 => registers.A /= (long)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //adv
            1 => registers.B ^= ParseOperand(OperandType.Literal, operand, registers), //bxl
            2 => registers.B = ParseOperand(OperandType.Combo, operand, registers) % 8, //bst
            3 => 0, //jnz
            4 => registers.B ^= registers.C, //bxc
            5 => ParseOperand(OperandType.Combo, operand, registers) % 8, //out
            6 => registers.B = registers.A / (long)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //bdv
            7 => registers.C = registers.A / (long)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //cdv
            _ => throw new NotImplementedException()
        };
    }

    static void ExecuteOpAsBitsOfRegA(long opcode, int operand, RegistersInABitsUsed usedABits)
    {
        switch (opcode)
        {
            case 0 or 1 or 3 or 5:
                return;
            case 2:
                usedABits.RegisterB = ParseComboOpAsUsedABits(operand, usedABits)[^3..];
                break;
            case 4:
                usedABits.RegisterB = PerformXOROnUsedABits(usedABits.RegisterB, usedABits.RegisterC);
                break;
            case 6:
                usedABits.RegisterB = PerformDecrementOnUsedABits(usedABits.RegisterB, operand, usedABits);
                break;
            case 7:
                usedABits.RegisterC = PerformDecrementOnUsedABits(usedABits.RegisterC, operand, usedABits);
                break;
            default: throw new InvalidOperationException();
        };
    }

    static List<int>[] ParseComboOpAsUsedABits(int operand, RegistersInABitsUsed usedABits)
    {
        return operand switch
        {
            int n when n >= 0 && n <= 3 => [],
            4 => Enumerable.Range(0, 3).Select(x => new List<int>() { x }).ToArray(),
            5 => usedABits.RegisterB,
            6 => usedABits.RegisterC,
            7 => throw new InvalidDataException("7 is reserved and cannot be used as literal."),
            _ => throw new InvalidOperationException()
        };
    }

    static List<int>[] PerformXOROnUsedABits(List<int>[] reg1, List<int>[] reg2)
    {
        for (int i = 0; i < reg1.Length; i++)
        {
            if (reg1[i] is not null && reg2[i] is not null)
            {
                reg1[i].AddRange(reg2[i]);
                reg1[i] = reg1[i].Distinct().ToList();
            }
        }

        return reg1;
    }

    static List<int>[] PerformDecrementOnUsedABits(List<int>[] reg, int comboOperand, RegistersInABitsUsed usedABits)
    {
        if (comboOperand >= 0 && comboOperand <= 3)
            return Enumerable.Range(0, 64)
                .Select((_, i) => i < 64 - comboOperand ? new List<int>() { i } : [])
                .ToArray();

        return comboOperand switch
        {
            4 => [],
            5 => Enumerable.Range(0, 64)
                .Select((x, i) =>
                    x > Math.Pow(2, usedABits.RegisterB
                        .Select((x, i) => new { x, i })
                        .Where(y => y.x is not null)
                        .OrderBy(x => x.i)
                        .Last().i)
                    ? new List<int>() { i }
                    : [])
                .ToArray(),
            6 => Enumerable.Range(0, 64)
                .Select((x, i) =>
                    x > Math.Pow(2, usedABits.RegisterC
                        .Select((x, i) => new { x, i })
                        .Where(y => y.x is not null)
                        .OrderBy(x => x.i)
                        .Last().i)
                    ? new List<int>() { i }
                    : [])
                .ToArray(),
            _ => throw new InvalidOperationException()
        };
    }

    static long ParseOperand(OperandType operandType, int operand, Registers registers)
    {
        return operandType switch
        {
            OperandType.Literal => operand,
            OperandType.Combo => ParseComboOperand(operand, registers),
            _ => throw new NotImplementedException()
        };
    }

    static long ParseComboOperand(int operand, Registers registers)
    {
        return operand switch
        {
            int n when n >= 0 && n <= 3 => operand,
            4 => registers.A,
            5 => registers.B,
            6 => registers.C,
            7 => throw new InvalidDataException("7 is reserved and cannot be used as literal."),
            _ => throw new InvalidOperationException()
        };
    }

    class Registers(long regA, long regB, long regC)
    {
        public long A = regA;
        public long B = regB;
        public long C = regC;
    }

    class RegistersInABitsUsed
    {
        public List<int>[] RegisterB = new List<int>[64];
        public List<int>[] RegisterC = new List<int>[64];
    }

    enum OperandType
    {
        Literal,
        Combo
    }
}

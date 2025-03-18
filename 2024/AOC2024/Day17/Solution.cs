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

        return FindRegAValue(program, 0, program.Count - 1);
    }

    static long FindRegAValue(List<int> program, long currentRegAValue, int currentProgramIndex)
    {
        if (ExecuteProgram(program, new Registers(), currentRegAValue) == string.Join(',', program[..^currentProgramIndex]))
        {
            if (currentProgramIndex is 0)
                return currentRegAValue;
            else
                return FindRegAValue(program, currentRegAValue * 8, currentProgramIndex - 1);
        }

        if (currentProgramIndex < program.Count - 1
            && ExecuteProgram(program, new Registers(), currentRegAValue / 8) != string.Join(',', program[..^(currentProgramIndex + 1)]))
        {
            return long.MaxValue;
        }

        return Math.Min(
            FindRegAValue(program, currentRegAValue + 1, currentProgramIndex),
            FindRegAValue(program, currentRegAValue + 2, currentProgramIndex)
            );
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

    class Registers
    {
        public long A;
        public long B;
        public long C;

        public Registers(long regA, long regB, long regC)
        {
            A = regA;
            B = regB;
            C = regC;
        }

        public Registers()
        { }
    }

    class RegistersInABitsUsed
    {
        public List<int>?[] RegisterB = new List<int>[64];
        public List<int>?[] RegisterC = new List<int>[64];
    }

    enum OperandType
    {
        Literal,
        Combo
    }
}

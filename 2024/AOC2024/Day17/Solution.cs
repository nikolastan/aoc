using NUnit.Framework;

namespace Day17;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt");
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    int SolvePart1(string inputPath)
    {
        return 0;
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static int? Execute(int opcode, int operand, Registers registers)
    {
        return opcode switch
        {
            0 => registers.A /= (int)Math.Pow(ParseOperand(OperandType.Combo, operand, registers), 2), //adv
            1 => registers.B ^ ParseOperand(OperandType.Literal, operand, registers), //bxl
            2 => registers.B = ParseOperand(OperandType.Combo, operand, registers) % 8, //bst
            3 => registers.A is not 0 ? 1 : 0, //jnz
            4 => registers.B ^= registers.C, //bxc
            5 => ParseOperand(OperandType.Combo, operand, registers) % 8,
            6 => registers.B = registers.A / (int)Math.Pow(ParseOperand(OperandType.Combo, operand, registers), 2), //bdv
            7 => registers.C = registers.A / (int)Math.Pow(ParseOperand(OperandType.Combo, operand, registers), 2), //cdv
            _ => throw new NotImplementedException()
        };
    }

    static int ParseOperand(OperandType operandType, int operand, Registers registers)
    {
        return operandType switch
        {
            OperandType.Literal => operand,
            OperandType.Combo => ParseComboOperand(operand, registers),
            _ => throw new NotImplementedException()
        };
    }

    static int ParseComboOperand(int operand, Registers registers)
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

    class Registers(int regA, int regB, int regC)
    {
        public int A = regA;
        public int B = regB;
        public int C = regC;
    }

    enum OperandType
    {
        Literal,
        Combo
    }
}

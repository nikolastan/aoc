using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Win32;
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

    static string SolvePart1(string inputPath, int? regAOverride = null)
    {
        (var registers, var program) = ReadInput(inputPath);

        return ExecuteProgram(program, registers);
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static (Registers Registers, List<int> Program) ReadInput(string inputPath)
    {
        var rawInput = File.ReadAllLines(inputPath)
            .Where(x => x is not "")
            .Select(x => x.Split(':'))
            .Select(x => x[1].Trim())
            .ToList();


        var registers = new Registers(int.Parse(rawInput[0]), int.Parse(rawInput[1]), int.Parse(rawInput[2]));
        var programs = rawInput[3].Split(',')
            .Select(int.Parse)
            .ToList();

        return (registers, programs);
    }
	static string ExecuteProgram(List<int> program, Registers registers, int? regAOverride = null)
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

    static int? ExecuteOp(int opcode, int operand, Registers registers)
    {
        return opcode switch
        {
            0 => registers.A /= (int)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //adv
            1 => registers.B ^= ParseOperand(OperandType.Literal, operand, registers), //bxl
            2 => registers.B = ParseOperand(OperandType.Combo, operand, registers) % 8, //bst
            3 => registers.A is not 0 ? 1 : 0, //jnz
            4 => registers.B ^= registers.C, //bxc
            5 => ParseOperand(OperandType.Combo, operand, registers) % 8,
            6 => registers.B = registers.A / (int)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //bdv
            7 => registers.C = registers.A / (int)Math.Pow(2, ParseOperand(OperandType.Combo, operand, registers)), //cdv
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

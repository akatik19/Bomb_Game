using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Disarmable), typeof(ModuleIdentifier))]
public class WiresGame : MonoBehaviour
{
    public delegate void GeneratedInstructionsEvent(List<Instruction> instructions);

    public event GeneratedInstructionsEvent GeneratedInstructions;

    public GameObject Prefab;

    public Color[] WireColors;

    abstract class Condition
    {
        public abstract bool isValid(string toCheck);

        public abstract string generateOutput();
    }

    class And : Condition
    {
        public Condition A { private set; get; }
        public Condition B { private set; get; }

        public And(Condition a, Condition b)
        {
            A = a;
            B = b;
        }

        public override string generateOutput()
        {
            return string.Format("{0} and {1}", A.generateOutput(), B.generateOutput());
        }

        public override bool isValid(string toCheck)
        {
            return A.isValid(toCheck) && B.isValid(toCheck);
        }
    }

    class Or : Condition
    {
        public Condition A { private set; get; }
        public Condition B { private set; get; }

        public Or(Condition a, Condition b)
        {
            A = a;
            B = b;
        }

        public override string generateOutput()
        {
            return string.Format("{0} or {1}", A.generateOutput(), B.generateOutput());
        }

        public override bool isValid(string toCheck)
        {
            return A.isValid(toCheck) || B.isValid(toCheck);
        }
    }

    public enum Ordering { First, Last };

    class DigitInRange : Condition
    {
        public static DigitInRange CreateRandom()
        {
            int from = UnityEngine.Random.Range(0, 10);
            int to = UnityEngine.Random.Range(from, 10);
            int numberOfDigits = 0;
            Ordering[] orderings = (Ordering[])Enum.GetValues(typeof(Ordering));
            Ordering ordering = orderings[UnityEngine.Random.Range(0, orderings.Length)];
            return new DigitInRange(ordering, numberOfDigits, from, to);
        }

        public RangeInt Range { private set; get; }

        public int Count { private set; get; }

        public Ordering Position { private set; get; }

        public DigitInRange(Ordering p, int numberOfDigits, int from, int to)
        {
            if (numberOfDigits < 0)
                throw new ArgumentException("The number of digits should be positive.");
            if (to < from)
                throw new ArgumentException("The parameter 'to' have to be greater or equal the parameter 'from'.");

            Position = p;
            Count = numberOfDigits;
            Range = new RangeInt(from, to - from);
        }

        public override string generateOutput()
        {
            return string.Format("the {0} {1} in range from {2} to {3}", Position, Count > 1 ? Count.ToString() + " digits are" : "digit is", Range.start, Range.end);
        }

        public override bool isValid(string toCheck)
        {
            bool result = true;
            IEnumerable<char> characters = Position == Ordering.Last ? toCheck.Reverse() : toCheck.AsEnumerable();

            int digitCount = 0;
            foreach (char c in characters)
            {
                if (char.IsDigit(c))
                {
                    int digit = c - '0';
                    if (digitCount >= Count)
                        break;

                    if (digit < Range.start && digit > Range.end)
                    {
                        result = false;
                        break;
                    }
                    digitCount++;
                }
            }
            return result;
        }
    }

    class DigitEvenOrOdd : Condition
    {
        public bool Even { private set; get; }

        public Ordering Position { private set; get; }

        public DigitEvenOrOdd(Ordering position, bool even)
        {
            Position = position;
            Even = even;
        }

        public override bool isValid(string toCheck)
        {
            IEnumerable<char> characters = Position == Ordering.Last ? toCheck.Reverse() : toCheck.AsEnumerable();
            bool result = false;
            foreach (char c in characters)
            {
                if (char.IsDigit(c))
                {
                    int digit = c - '0';
                    result = digit % 2 == 0 ? Even : !Even;
                    break;
                }
            }
            return result;
        }

        public override string generateOutput()
        {
            return string.Format("the {0} digit is {1}", Position.ToString().ToLower(), Even ? "even" : "odd");
        }
    }

    class HasVowel : Condition
    {

        public Ordering Position { private set; get; }

        public HasVowel(Ordering position)
        {
            Position = position;
        }

        public override bool isValid(string toCheck)
        {
            IEnumerable<char> characters = Position == Ordering.Last ? toCheck.Reverse() : toCheck.AsEnumerable();
            bool result = false;
            foreach (char c in characters)
            {
                if (char.IsLetter(c))
                {
                    result = c.isVowel();
                    break;
                }
            }
            return result;
        }

        public override string generateOutput()
        {
            return string.Format("the {0} letter is a vowel", Position.ToString().ToLower());
        }
    }

    public abstract class Instruction
    {
        public abstract bool isValid(int wirePosition);

        public abstract string generateOutput();
    }

    class IfElse : Instruction
    {
        Timer _timer;
        Condition _condition;
        Instruction _then;
        Instruction _otherwise;

        public IfElse(Timer component, Condition condition, Instruction then, Instruction otherwise)
        {
            _timer = component;
            _condition = condition;
            _then = then;
            _otherwise = otherwise;
        }

        public override string generateOutput()
        {
            return string.Format("If {0} then {1} else {2}", _condition.generateOutput(), _then.generateOutput(), _otherwise.generateOutput());
        }

        public override bool isValid(int wirePosition)
        {
            return _condition.isValid(_timer.SerialNumber) ? _then.isValid(wirePosition) : _otherwise.isValid(wirePosition);
        }
    }

    class CutWire : Instruction
    {
        public int Position { private set; get; }

        public CutWire(int position)
        {
            Position = position;
        }

        public override bool isValid(int wirePosition)
        {
            return Position == wirePosition;
        }

        public override string generateOutput()
        {
            return string.Format("cut the {0} wire", Position);
        }
    }

    struct CutWireAction
    {
        public int Position;
    }

    List<Instruction> _instructions;

    void OnEnable()
    {
        Debug.Log("Enabled WiresGame");
        _instructions = new List<Instruction>();
    }

    void Start()
    {
        GenerateInstructions();
    }

    void GenerateInstructions()
    {
        Timer timer = GetComponentInParent<Timer>();
        if (!timer)
        {
            Debug.LogError("No timer component in parent game object found. Stop generating instructions.");
            return;
        }

        int from = UnityEngine.Random.Range(0, 9);
        int to = from + UnityEngine.Random.Range(1, 9 - from);
        Condition a = new DigitInRange((Ordering)UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(1, 3), from, to);
        Condition b = new DigitEvenOrOdd((Ordering)UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1) != 0);
        Condition c = new And(a, b);

        Instruction cutThird = new CutWire(UnityEngine.Random.Range(0, WireColors.Length));
        Instruction cutFifth = new CutWire(UnityEngine.Random.Range(0, WireColors.Length));
        Instruction first = new IfElse(timer, c, cutThird, cutFifth);
        Debug.Log(first.generateOutput());
        for (int i = 0; i < WireColors.Length; i++)
            Debug.LogFormat("{0} = isValid({1})", first.isValid(i), i);

        _instructions.Add(first);

        if (GeneratedInstructions != null)
        {
            GeneratedInstructions(_instructions);
        }
    }

    private void OnMessageReceived(Message message)
    {
        if (message.Module != GetComponent<ModuleIdentifier>().Name)
            return;

        try
        {
            int wirePosition = int.Parse(message.Text);

        }
        catch (Exception e)
        {
            throw new UnityException("Couldn't parse message.", e);
        }
    }
}
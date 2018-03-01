using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Tooltip("Factor by which the time should be increased for each strike.")]
    public float IncreasingFactor = 0.5f;

    [Tooltip("Total time of the timer in minutes.")]
    public float TotalTime = 10;

    [Tooltip("Largest number of strikes that can be achieved before the bomb explodes.")]
    public int MaxStrikes = 2;

    [Tooltip("These are the characters, that are used to generate the serial number.")]
    public string AllowedCharacters;

    public int MinDigits;

    public int MaxDigits;

    [Tooltip("The amount of characters a serial number should have.")]
    public int SerialNumberSize;

    public string SerialNumber { private set; get; }

    private int _strikes;
    public int Strikes
    {
        set
        {
            if (value > MaxStrikes)
            {
                SendMessageUpwards("ExplodeByStrikes");
                return;
            }
            _strikes = value;
            Debug.LogFormat(this, "Set Strikes to {0}", _strikes);
        }
        get { return _strikes; }
    }

    public TimeSpan CurrentTime { private set; get; }

    public bool SerialNumberHasVowel { private set; get; }

    public bool SerialNumbersLastDigitIsOdd { private set; get; }

    void OnEnable()
    {
        SerialNumber = GenerateSerialNumber();
        var serialNumberText = GameObject.FindGameObjectWithTag("SerialNumberText");
        if (serialNumberText)
            serialNumberText.GetComponent<Text>().text = SerialNumber;
        Debug.LogFormat(this, "Serial Number = {0}", SerialNumber);
        Strikes = 0;
        CurrentTime = TimeSpan.FromMinutes(TotalTime);
    }

    void Start()
    {
        if (MinDigits < 0)
            Debug.LogWarning("The minimal number of digits shouldn't be less than 0.");
        if (MaxDigits > SerialNumberSize)
            Debug.LogWarning("The maximal number of digits shouldn't exeed the size of the serial number.");
    }

    private string GenerateSerialNumber()
    {
        SerialNumberHasVowel = false;
        int digitsCount = UnityEngine.Random.Range(MinDigits, MaxDigits);
        IEnumerable<char> randomDigits = Enumerable.Range(0, 10).OrderBy(x => UnityEngine.Random.value).Take(digitsCount).Select(n => (char)('0' + n));
        IEnumerable<char> randomLetters = AllowedCharacters.OrderBy(x => UnityEngine.Random.value).Take(SerialNumberSize - digitsCount);
        IEnumerable<char> randomSerialNumber = randomDigits.Union(randomLetters).OrderBy(x => UnityEngine.Random.value);
        StringBuilder serialNumber = randomSerialNumber.Aggregate(new StringBuilder(),
            (sb, c) =>
            {
                if (!SerialNumberHasVowel && c.isVowel())
                    SerialNumberHasVowel = true;
                if (char.IsDigit(c))
                    SerialNumbersLastDigitIsOdd = (c - '0') % 2 != 0;
                return sb.Append(c);
            }
        );
        return serialNumber.ToString();
    }

    void Update()
    {
        if (CurrentTime > TimeSpan.Zero)
        {
            CurrentTime -= TimeSpan.FromSeconds(Time.deltaTime * (Strikes * IncreasingFactor + 1));
            if (CurrentTime <= TimeSpan.Zero)
            {
                CurrentTime = TimeSpan.Zero;
                SendMessageUpwards("ExplodeByTime");
            }
        }
    }
}

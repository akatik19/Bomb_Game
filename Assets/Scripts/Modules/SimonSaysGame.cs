using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Disarmable), typeof(ModuleIdentifier))]
public class SimonSaysGame : MonoBehaviour
{
    /// <summary>
    /// The colors of the simon says game
    /// </summary>
    [System.Serializable]
    public enum Color
    {
        Red, Green, Blue, Yellow
    }

    /// <summary>
    /// The number of sequence elements to disarm the Simon Says Module
    /// </summary>
    public int MaxSequenceSize = 5;

    /// <summary>
    /// The current generated sequence of colors
    /// </summary>
    private List<Color> _sequence;

    /// <summary>
    /// The current index indicates the position of the sequence the player has reached
    /// </summary>
    private int _currentIndex;

    private UnityEngine.Random.State _randomState;

    private Color[,] _randomColorMap;

    void Awake()
    {
        _sequence = new List<Color>();
        var timer = GetComponentInParent<Timer>();
        _randomColorMap = new Color[Enum.GetValues(typeof(Color)).Length, 2 * timer.MaxStrikes];
    }

    void OnEnable()
    {
        var timerModule = GetComponentInParent<Timer>();
        if (!timerModule)
            Debug.LogError("No timer component found!", this);

        // Use the serial number as a seed value for random number generation
        int seed = timerModule.SerialNumber.GetHashCode();
        UnityEngine.Random.InitState(seed);
        _randomState = UnityEngine.Random.state;
        _sequence.Clear();

        var connection = GetComponentInParent<Connection<Message>>();
        if (connection)
        {
            Debug.Log(string.Format("Got connection {0}", connection));
            connection.ReceiveMessage += OnReceiveMessage;
        }
        else
            Debug.LogWarning("There is no connection component.", this);

        GenerateRandomIndicesMap();

        // first random generated color
        AddRandomColor();
    }

    private void GenerateRandomIndicesMap()
    {
        var timer = GetComponentInParent<Timer>();
        Color[] randomColors = (Color[])Enum.GetValues(typeof(Color));
        int count = 2 * timer.MaxStrikes;
        for (int j = 0; j < count; j++)
        {
            randomColors = randomColors.OrderBy(index => UnityEngine.Random.value).ToArray();
            for (int i = 0; i < randomColors.Length; i++)
            {
                _randomColorMap[i, j] = randomColors[i];
            }
        }
    }

    void OnDisable()
    {
        SendMessageUpwards("Send", new Message { Module = GetComponent<ModuleIdentifier>().Name, Text = "reset" }, SendMessageOptions.DontRequireReceiver);
        var connection = GetComponentInParent<Connection<Message>>();
        if (connection)
            connection.ReceiveMessage -= OnReceiveMessage;
    }

    private void OnReceiveMessage(Message message)
    {
        Debug.LogFormat(this, "Received message: {0}", message);

        if (message.Module != GetComponent<ModuleIdentifier>().Name)
            return;

        try
        {
            Color color = (Color)Enum.Parse(typeof(Color), message.Text, true);
            Debug.LogFormat(this, "Parsed '{0}' to {1}", message.Text, color);
            OnButtonPressed(color);
        }
        catch (ArgumentException e)
        {
            throw new UnityException(string.Format("Cannot parse message text '{0}' to enum {1}", message.Text, typeof(Color)), e);
        }
    }

    void AddRandomColor()
    {
        // load random state
        UnityEngine.Random.state = _randomState;

        // generate random color
        Color[] colors = (Color[])Enum.GetValues(typeof(Color));
        Color color = colors[UnityEngine.Random.Range(0, colors.Length)];
        _sequence.Add(color);
        _currentIndex = 0;

        // store new random state
        _randomState = UnityEngine.Random.state;

        SendMessageUpwards("Send", new Message { Module = GetComponent<ModuleIdentifier>().Name, Text = color.ToString() });
#if (UNITY_EDITOR)
        Debug.LogFormat(this, "Sequence = {0}", string.Join(", ", _sequence.ConvertAll(c => c.ToString()).ToArray()));
        var timer = GetComponentInParent<Timer>();
        Converter<Color, string> converter = c => GetExpectedColor(c, timer.SerialNumberHasVowel, timer.Strikes).ToString();
        Debug.LogFormat(this, "Sequence to play = {0}", string.Join(", ", _sequence.ConvertAll(converter).ToArray()));
#endif
    }

    void OnButtonPressed(Color color)
    {
        var timer = GetComponentInParent<Timer>();
        Color expectedColor = GetExpectedColor(_sequence[_currentIndex], timer.SerialNumberHasVowel, timer.Strikes);

        // wrong color
        if (color != expectedColor)
        {
            _currentIndex = 0;
            Debug.Log("Wrong color");
            GetComponentInParent<Timer>().Strikes++;
#if (UNITY_EDITOR)
            Converter<Color, string> converter = c => GetExpectedColor(c, timer.SerialNumberHasVowel, timer.Strikes).ToString();
            Debug.LogFormat(this, "Sequence to play = {0}", string.Join(", ", _sequence.ConvertAll(converter).ToArray()));
#endif
            return;
        }

        _currentIndex++;
        if (_currentIndex >= _sequence.Count)
        {
            if (_sequence.Count < MaxSequenceSize)
                AddRandomColor();
            else
            {
                GetComponent<Disarmable>().Disarmed = true;
            }
        }
    }

    public Color GetExpectedColor(Color color, bool serialNumberHasVowel, int numStrikes)
    {
        var timerModule = GetComponentInParent<Timer>();
        return _randomColorMap[(int)color, serialNumberHasVowel ? numStrikes + timerModule.MaxStrikes : numStrikes];
    }
}

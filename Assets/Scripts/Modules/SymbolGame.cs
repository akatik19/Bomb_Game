
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Disarmable), typeof(ModuleIdentifier))]
public class SymbolGame : MonoBehaviour
{

    [Range(3, 10)]
    [Tooltip("Number of columns.")]
    public int ColumnCount = 5;

    [Range(5, 10)]
    [Tooltip("Number of symbols for each column.")]
    public int RowCount = 8;

    [Tooltip("Number of symbols of one column that should be entered in the right order.")]
    public int ShownSymbolsCount = 4;

    public Sprite[] Symbols;

    public delegate void ShownSymbolsChangedHandler(IEnumerable<Sprite> shownSymbols);
    public event ShownSymbolsChangedHandler ShownSymbolsChanged;

    /// <summary>
    /// The current index indicates the position of the sequence the player has reached
    /// </summary>
    private int _currentIndex;

    /// <summary>
    /// The column where the shown symbols are from
    /// </summary>
    private int _expectedColumn;

    /// <summary>
    /// The shown symbols
    /// </summary>
    private Sprite[] _shownSymbols;

    /// <summary>
    /// The indices of the shown symbols ordered by the row index
    /// </summary>
    private int[] _shownSymbolIndices;

    /// <summary>
    /// A table of symbols to identify the order of the shown symbols
    /// </summary>
    private Sprite[,] _symbolTable;

    private UnityEngine.Random.State _randomState;

    void OnEnable()
    {
        var timerModule = GetComponentInParent<Timer>();
        if (!timerModule)
            Debug.LogError("No timer component found!", this);

        // Use the serial number as a seed value for random number generation
        int seed = timerModule.SerialNumber.GetHashCode();
        UnityEngine.Random.InitState(seed);
        _randomState = UnityEngine.Random.state;

        var connection = GetComponentInParent<Connection<Message>>();
        if (connection)
        {
            Debug.Log(string.Format("Got connection {0}", connection));
            connection.ReceiveMessage += OnReceiveMessage;
        }
        else
            Debug.LogWarning("There is no connection component.", this);

        _symbolTable = new Sprite[ColumnCount, RowCount];
        GenerateColumns();
        GenerateShownSymbols();
    }

    void OnDisable()
    {
        SendMessageUpwards("Send", new Message { Module = GetComponent<ModuleIdentifier>().Name, Text = "reset" }, SendMessageOptions.DontRequireReceiver);
        var connection = GetComponentInParent<Connection<Message>>();
        if (connection)
            connection.ReceiveMessage -= OnReceiveMessage;
    }

    private void GenerateColumns()
    {
        for (int i = 0; i < ColumnCount; i++)
        {
            int j = 0;
            // Shuffle indices of the Symbols array and pick RowCount of them to guarantee that none is taken twice
            IEnumerable randomIndices = Enumerable.Range(0, Symbols.Length).OrderBy(x => UnityEngine.Random.value).Take(RowCount);
            foreach (int randomIndex in randomIndices)
            {
                _symbolTable[i, j] = Symbols[randomIndex];
                j++;
            }
        }
    }

    private void GenerateShownSymbols()
    {
        _expectedColumn = UnityEngine.Random.Range(0, ColumnCount);

        Message m = new Message();
        m.Module = GetComponent<ModuleIdentifier>().Name;
        m.Text = "Column:" + _expectedColumn;
        SendMessageUpwards("Send", m);

        int[] randomRowIndices = Enumerable.Range(0, RowCount).OrderBy(x => UnityEngine.Random.value).Take(ShownSymbolsCount).ToArray();
        _shownSymbols = randomRowIndices.Select((index) => _symbolTable[_expectedColumn, index]).ToArray();
        _shownSymbolIndices = Enumerable.Range(0, _shownSymbols.Length).OrderBy(symbolIndex => randomRowIndices[symbolIndex]).ToArray();

        if (ShownSymbolsChanged != null)
            ShownSymbolsChanged(_shownSymbols);

        m = new Message();
        m.Module = GetComponent<ModuleIdentifier>().Name;
        m.Text = "Images:" + String.Join(",", randomRowIndices.Select(x => x.ToString()).ToArray());
        SendMessageUpwards("Send", m);

        Debug.LogFormat("Symbol sequence = {0}", string.Join(", ", _shownSymbolIndices.Select(i => string.Format("{0}[{1}]", _shownSymbols[i], i)).ToArray()));
        Debug.LogFormat(this, "Sequence to play = {0}", string.Join(", ", _shownSymbolIndices.Select(i => i.ToString()).ToArray()));
    }

    public Sprite GetSymbol(int row, int column)
    {
        return _symbolTable[column, row];
    }

    public IEnumerable<Sprite> GetShownSymbols()
    {
        return _shownSymbols;
    }

    public void OnReceiveMessage(Message message)
    {
        Debug.LogFormat(this, "Received message: {0}", message);

        if (message.Module != GetComponent<ModuleIdentifier>().Name)
            return;

        try
        {
            int position = int.Parse(message.Text);
            Debug.LogFormat(this, "Parsed '{0}' to {1}", message.Text, position);
            OnSymbolChosen(position);
        }
        catch (ArgumentException e)
        {
            throw new UnityException(string.Format("Cannot parse message text '{0}' to {1}", message.Text, typeof(int)), e);
        }
    }

    private void OnSymbolChosen(int position)
    {
        var timer = GetComponentInParent<Timer>();

        int expectedPosition = _shownSymbolIndices[_currentIndex];
        // wrong color
        if (position != expectedPosition)
        {
            _currentIndex = 0;
            Debug.Log("Wrong order");
            GenerateShownSymbols();
            timer.Strikes++;
            return;
        }

        _currentIndex++;
        if (_currentIndex >= ShownSymbolsCount)
        {
            GetComponent<Disarmable>().Disarmed = true;
        }
    }
}

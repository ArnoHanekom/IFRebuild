namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class TrackingMatrix
{
    List<List<MatrixNumber>> _columns;
    int _numberOfColumns;
    int _matrixColumn;
    int _maxVerticalGapColumn;

    List<Counter> _columnGapCounters;

    public TrackingMatrix(int numberOfColumns)
    {
        _columnGapCounters = new List<Counter>();
        _numberOfColumns = numberOfColumns;

        //initialize the columns and their associated counters
        _columns = new List<List<MatrixNumber>>(numberOfColumns);
        Counter c;
        for (int i = 1; i <= _numberOfColumns; i++)
        {
            c = new Counter(i, 0);
            _columnGapCounters.Add(c);
            _columns.Add(new List<MatrixNumber>());
        }
        _maxVerticalGapColumn = 0;
    }

    /// <summary>
    /// The private constructor used for cloning,
    /// </summary>
    private TrackingMatrix(int numberOfColumns, List<Counter> columnGapCounters, List<List<MatrixNumber>> columns, int matrixColumn,
        int maxVerticalGapColumn)
    {
        _numberOfColumns = numberOfColumns;
        _columnGapCounters = columnGapCounters;
        _columns = columns;
        _matrixColumn = matrixColumn;
        _maxVerticalGapColumn = maxVerticalGapColumn;
    }

    public List<List<MatrixNumber>> Columns
    {
        get { return _columns; }
        set { _columns = value; }
    }

    /// <summary>
    /// returns the maximum vertical gap size through the max of the column counters
    /// </summary>
    public int MaxVerticalGapSize
    {
        get
        {
            Counter highCount = new Counter(0, 0);
            foreach (Counter c in _columnGapCounters)
            {
                if (c.Count > highCount.Count)
                    highCount = c;
            }
            _maxVerticalGapColumn = highCount.NumberValue;
            return highCount.Count;
        }
    }

    /// <summary>
    /// Returns the column in which the highest vertical gap exists in the matrices
    /// </summary>
    public int MaxVerticalGapColumn
    {
        get { return _maxVerticalGapColumn; }
    }
    /// <summary>
    /// The current column of the matrix, so the next number is entered in CurrentColumn + 1
    /// </summary>
    public int CurrentColumn
    {
        get { return _matrixColumn; }
    }

    /// <summary>
    /// Gets the column with the most numbers in order to render it correctly
    /// </summary>
    public int MaxColumnLength
    {
        get
        {
            int count = 0;
            foreach (List<MatrixNumber> numbers in _columns)
            {
                if (numbers.Count > count)
                    count = numbers.Count;
            }
            return count;
        }
    }

    /// <summary>
    /// Read-only Property specifying the number of columns
    /// </summary>
    public int NumberOfColumns
    {
        get { return _numberOfColumns; }
    }

    /// <summary>
    /// picks up an event from the owing board to add the spin
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnSpin(object sender, MatrixSpinEventArgs e)
    {
        EnterSpin(e.Number.Number, e.Number.Win);
    }
    /// <summary>
    /// Initialize the counter objects for gap sizes
    /// </summary>
    public void InitializeCounters()
    {
        Counter c;
        for (int i = 1; i <= _numberOfColumns; i++)
        {
            c = new Counter(i, 0);
            _columnGapCounters.Add(c);
        }
    }
    public void EnterSpin(int number, bool win)
    {
        //get the current column rows collection
        var columnNumbers = _columns[_matrixColumn];
        int rowCount = columnNumbers.Count;

        //now add the number to the column
        var matrixNumber = new MatrixNumber(number, _matrixColumn + 1, rowCount + 1);
        matrixNumber.Win = win;
        columnNumbers.Add(matrixNumber);

        //update the vertical gap counters
        var count = _columnGapCounters[_matrixColumn];
        if (win)
        {
            count.Count = 0;//reset counter
            DeleteColumn(_matrixColumn);//delete the numbers in the column where the win occured
        }
        else count.Count += 1; //increment counter

        //column index tracker
        if (_matrixColumn == _numberOfColumns - 1)
            _matrixColumn = 0;
        else
            _matrixColumn++;
    }
    /// <summary>
    /// Deletes a column in the tracking matrix
    /// </summary>
    /// <param name="columnIndex"></param>
    public void DeleteColumn(int columnIndex)
    {
        _columns[columnIndex] = new List<MatrixNumber>();
    }

    public TrackingMatrix Clone()
    {
        //clone the gap counters
        var newColumnGapCounters = new List<Counter>(_columnGapCounters.Count);
        Array.ForEach<Counter>(_columnGapCounters.ToArray(), x => newColumnGapCounters.Add(x.Clone()));

        //clone the columns
        var newColumns = new List<List<MatrixNumber>>(_columns.Count);
        foreach (List<MatrixNumber> column in _columns)
        {
            var newColumn = new List<MatrixNumber>(column.Count);
            Array.ForEach<MatrixNumber>(column.ToArray(), x => newColumn.Add(x.Clone()));
            newColumns.Add(newColumn);
        }

        var trackingMatrix = new TrackingMatrix(_numberOfColumns, newColumnGapCounters, newColumns, _matrixColumn, _maxVerticalGapColumn);

        return trackingMatrix;
    }
}
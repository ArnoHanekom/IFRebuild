namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class Matrix
{
    List<MatrixRow> _rows;
    MatrixRow _currentRow;
    int _maxGapSize = 0;
    short _numberOfColumns;

    public Matrix()
    {
        _rows = new List<MatrixRow>();
        _currentRow = new MatrixRow();
        _numberOfColumns = 6;//default 
    }
    public Matrix(short numberOfColumns) : this()
    {
        _numberOfColumns = numberOfColumns;
    }

    /// <summary>
    /// The private constructor used in cloning. 
    /// </summary>
    private Matrix(short numberOfColumns, List<MatrixRow> rows, MatrixRow currentRow, int maxGapSize)
    {
        _numberOfColumns = numberOfColumns;
        _rows = rows;
        _currentRow = currentRow;
        _maxGapSize = maxGapSize;
    }

    public List<MatrixRow> Rows
    {
        get { return _rows; }
        set { _rows = value; }
    }

    /// <summary>
    /// The enter spin method captures the number from the Roulette wheel into the matrix
    /// </summary>
    /// <param name="number">The number which the randmoized roulette board at the online casino has produced.</param>
    /// <param name="boardColumn">The board column of the spun number</param>
    /// <returns>The integer specifying the current column x1,x2,x3 etc</returns>
    public MatrixNumber EnterSpin(int number, int boardColumn)
    {
        //add the new row if first item
        if (_currentRow.Numbers.Count == 0)
            _rows.Add(_currentRow);

        //get the column of the matrix (we still need to add the number so incremenet it)
        int matrixColumn = _currentRow.Numbers.Count();
        matrixColumn += 1;

        //set the win property
        bool win = false;
        if (boardColumn == matrixColumn)
            win = true;

        //add the number to the current row
        MatrixNumber matrixNumber = new MatrixNumber(number, matrixColumn, _rows.Count());
        matrixNumber.Win = win;
        _currentRow.Numbers.Add(matrixNumber);

        //its the end of the row, so reset the current row
        if (_currentRow.Numbers.Count == _numberOfColumns)
            _currentRow = new MatrixRow();

        //returned the column where the number was just inserted
        return matrixNumber;
    }

    /// <summary>
    /// Gets a list of all the Matrix number occurences in the matrix matching this number
    /// </summary>
    /// <param name="number">the number to search for</param>
    /// <returns>A list of matrix numbers</returns>
    public List<MatrixNumber> GetAllOccurences(int number)
    {
        var list = new List<MatrixNumber>();

        //start at the top of the board matrix, and cycle forward
        for (int i = 0; i < _rows.Count(); i++)
        {
            list.AddRange(_rows[i].Numbers.FindAll(x => x.Number == number));

            //foreach (MatrixNumber matrixNumber in _rows[i].Numbers)
            //{
            //    //do the numbers match & is this number on a different column to the board code of the immovable number
            //    if (number == matrixNumber.Number)
            //    {
            //        list.Add(matrixNumber);
            //    }
            //}
        }
        return list;
    }

    /// <summary>
    /// Finds the first occurence of a number in the matrix, starting at the top moving downwards
    /// </summary>
    /// <param name="number">the number</param>
    /// <param name="boardCode">The board code of the number which is immovable, a number found in that column in the matrix will be ignored.</param>
    /// <returns>The matrix Number reference, containing the important row and column information</returns>
    public Nullable<MatrixNumber> FindFirstOccurence(int number, int boardCode)
    {
        var occurences = GetAllOccurences(number);

        foreach (MatrixNumber matrixNumber in occurences)
        {
            if (boardCode != matrixNumber.Column &&
                !IsInColumn(matrixNumber.Number, matrixNumber.Column, matrixNumber.Row + 1))
                return matrixNumber;
        }
        return null;
    }

    /// <summary>
    /// Deletes rows in the matrix up until a certain number, so it will delete all rows from the top down to the row including that number
    /// </summary>
    /// <param name="numberToDelete">The matrix number which effectively needs to be deleted</param>
    /// <returns>A collection of the matrix rows which were deleted</returns>
    public List<MatrixRow> DeleteRows(MatrixNumber numberToDelete)
    {
        List<MatrixRow> newRows = new List<MatrixRow>();
        MatrixRow newRow;

        //iterate the matrix and get the rows to delete
        for (int i = 0; i < numberToDelete.Row; i++)
        {
            newRow = new MatrixRow();
            newRow = _rows[i];
            newRows.Add(newRow);
        }
        //now delete the rows
        _rows.RemoveRange(0, numberToDelete.Row);

        //now renumber the matrix numbers row property
        RenumberNumberRows();

        return newRows;
    }

    /// <summary>
    /// This operation checks whether a number exists in a specific column of the current active matrix
    /// The collection of rows need not be specified, this operation will assume its the current matrix row collection
    /// and pass it to the overload
    /// </summary>
    /// <param name="number">The number to check</param>
    /// <param name="column">The colulmn to check</param>
    /// <returns>True/False</returns>
    public bool IsInColumn(int number, int column, int startRow)
    {
        return IsInColumn(number, column, startRow, _rows);
    }

    /// <summary>
    /// This operation checks whether a number exists in a specific column. The collection of rows is supplied so it may not be 
    /// the default row collection of this matrix instance. You may specify another collection such as the Deleted Rows collection
    /// </summary>
    /// <param name="number">The number to search for</param>
    /// <param name="column">The column to search down</param>
    /// <param name="rows">The list of MatrixRow to search</param>
    /// <returns>True/False</returns>
    public bool IsInColumn(int number, int column, int startRow, List<MatrixRow> rows)
    {
        //iterate the rows and check the number in each column
        MatrixNumber matrixNumber;
        for (int i = startRow - 1; i < rows.Count; i++)
        {
            //does the column even exist in the row?
            if (rows[i].Numbers.Count >= column)
            {
                matrixNumber = rows[i].Numbers[column - 1];
                if (matrixNumber.Number == number)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the largest Gap size in a matrix
    /// </summary>
    public int MaxGapSize
    {
        get
        {
            short counter = 0;
            _maxGapSize = 0;
            foreach (MatrixRow row in _rows)
            {
                foreach (MatrixNumber number in row.Numbers)
                {
                    if (number.Win)
                    {
                        if (counter > _maxGapSize)
                            _maxGapSize = counter;

                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
            return counter > _maxGapSize ? counter : _maxGapSize;
        }
    }

    /// <summary>
    /// Returns the most recent gap size in the matrix between the current number and last win
    /// </summary>
    public int GapSize
    {
        get
        {
            short counter = 0;
            //iterate matrix rows backwards
            for (int i = _rows.Count - 1; i >= 0; i--)
            {
                //iterate numbers backwards
                for (int j = _rows[i].Numbers.Count - 1; j >= 0; j--)
                {
                    if (_rows[i].Numbers[j].Win)
                        return counter;
                    else
                        counter++;
                }
            }
            return counter;
        }
    }

    /// <summary>
    /// Returns the current column of the matrix
    /// </summary>
    public int CurrentColumn
    {
        get { return _currentRow.Numbers.Count(); }
    }

    public Matrix Clone()
    {
        //current row
        var newCurrentRow = _currentRow.Clone();

        //matrix rows
        var newMatrixRows = new List<MatrixRow>(_rows.Count);
        Array.ForEach<MatrixRow>(_rows.ToArray(), x => newMatrixRows.Add(x.Clone()));

        var matrix = new Matrix(_numberOfColumns, newMatrixRows, newCurrentRow, _maxGapSize);

        return matrix;
    }

    /// <summary>
    /// Private method to renumber the matrix row numbers after a delete
    /// </summary>
    private void RenumberNumberRows()
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            //OLD way when using a class reference, i changed to value types in hopes of effeciency
            //foreach (MatrixNumber number in _rows[i].Numbers)
            //{

            //    number.Row = i+1;
            //}
            var newList = new List<MatrixNumber>(6);
            for (int j = 0; j < _rows[i].Numbers.Count; j++)
            {
                var currentNumber = _rows[i].Numbers[j];
                newList.Add(new MatrixNumber(currentNumber.Number, currentNumber.Column, i + 1));
            }
            _rows[i].Numbers = newList;
        }
    }
}
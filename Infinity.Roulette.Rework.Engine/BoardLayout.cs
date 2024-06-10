using System.Diagnostics;

namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class BoardLayout
{
    List<BoardColumn> _columns;
    Matrix _matrix;
    List<TrackingMatrix> _matrices;
    int _layoutCode = 0;
    int _bettingStategyGap;
    BettingStrategy _bettingStrategy;
    int _maxVerticalGapMatrix;

    public event MatrixSpinEventHandler MatrixSpin;

    /// <summary>
    /// Board layout constructor. Creates the board columns, the main matrix and the 50 tracking matrices.
    /// </summary>
    public BoardLayout()
    {
        //create 6 columns on the board
        _columns = new List<BoardColumn>(6);
        _matrix = new Matrix();

        //create the tracking matrices
        _matrices = new List<TrackingMatrix>(Settings.Default.MATRICES);
        TrackingMatrix matrix;
        for (short i = 2; i <= Settings.Default.MATRICES; i++)
        {
            matrix = new TrackingMatrix(i);
            this.MatrixSpin += new MatrixSpinEventHandler(matrix.OnSpin);
            _matrices.Add(matrix);
        }

        resetCodeWins();
        //set the defaults
        _maxVerticalGapMatrix = 0;
    }

    /// <summary>
    /// The private constructor used for cloning
    /// </summary>
    private BoardLayout(List<BoardColumn> columns, Matrix matrix, List<TrackingMatrix> matrices, int layoutCode, int bettingStrategyGap,
        BettingStrategy bettingStrategy, int maxVerticalGapMatrix)
    {
        _columns = columns;
        _matrix = matrix;
        _matrices = matrices;
        _layoutCode = layoutCode;
        _bettingStategyGap = bettingStrategyGap;
        _bettingStrategy = bettingStrategy;
        _maxVerticalGapMatrix = maxVerticalGapMatrix;
    }

    private Dictionary<int, int> _codeWins { get; set; } = new(6);
    public Dictionary<int, int> CodeWins => _codeWins;

    private void resetCodeWins()
    {
        _codeWins = new Dictionary<int, int>(6);
        for (int key = 0; key < 6; ++key)
            _codeWins.Add(key, 0);
    }

    /// <summary>
    /// Returns the layout code of the board. Valid numbers are 1-720
    /// </summary>
    public int LayoutCode
    {
        get { return _layoutCode; }
    }
    /// <summary>
    /// Returns the list of board columns 
    /// </summary>
    public List<BoardColumn> Columns
    {
        get { return _columns; }
        set { _columns = value; }
    }
    /// <summary>
    /// The main matrix 6x6 horizontally searched
    /// </summary>
    public Matrix Matrix
    {
        get { return _matrix; }
    }
    /// <summary>
    /// The list collection of the tracking matrices. These are layouts 1-50, 1 being 1 column and 50 having 50 columns.
    /// They listen to the board layouts events to track spins.
    /// </summary>
    public List<TrackingMatrix> TrackingMatrices
    {
        get { return _matrices; }
    }

    /// <summary>
    /// Returns the current betting strategy for the board
    /// </summary>
    public BettingStrategy BettingStrategy
    {
        get { return _bettingStrategy; }
        set { _bettingStrategy = value; }
    }

    /// <summary>
    /// The configured betting strategy gap size currently being used in betting
    /// </summary>
    public int BettingStrategyGapSize
    {
        get { return _bettingStategyGap; }
        set { _bettingStategyGap = value; }
    }

    /// <summary>
    /// The maximum gap size found either in the main matrix or that max vertical gap in the vertical matrices
    /// </summary>
    public int MaxGapSize
    {
        get
        {
            int maxVerticalGapSize = this.MaxVerticalGapSize;
            if (_matrix.GapSize > maxVerticalGapSize)
                return Matrix.GapSize;
            else
                return maxVerticalGapSize;

        }
    }
    /// <summary>
    /// Returns the highest vertical Gap size in all the vertical matrices for this board
    /// </summary>
    public int MaxVerticalGapSize
    {
        get
        {
            int ret = 0, matrixNo = 0;
            for (int i = 0; i < _matrices.Count; i++)
            {
                if (_matrices[i].MaxVerticalGapSize > ret)
                {
                    ret = _matrices[i].MaxVerticalGapSize;
                    matrixNo = i + 1;
                }
            }
            _maxVerticalGapMatrix = matrixNo;
            return ret;
        }
    }

    /// <summary>
    /// Returns the matrix number for this board layout where the highest vertical gap has been found
    /// </summary>
    public int MaxVerticalGapMatrix
    {
        get { return _maxVerticalGapMatrix; }
    }

    /// <summary>
    /// Gets the 6 board numbers matching the column which matches the next column in the matrix
    /// </summary>
    public List<BoardNumber> BetOnNumbers
    {
        get
        {
            //are we able to show the betting numbers? Have the appropriate gap sizes been found?
            if (this.BettingActive)
            {
                int column;
                column = _matrix.CurrentColumn == 6 ? 1 : _matrix.CurrentColumn + 1;
                var list = new List<BoardNumber>(6);
                list = this.GetBoardColumn(column).Numbers;
                return list;
            }
            return null;
        }
    }

    /// <summary>
    /// Is betting active? The matrix must contain a gap larger than the configured gap size. This can either be the main gap in a normal horizontal
    /// searched matrix or the vertically searched tracking matrices for optimal results.
    /// </summary>
    public bool BettingActive
    {
        get
        {
            bool ret = false;
            int maxVerticalGapSize = this.MaxVerticalGapSize;
            int horizontalGapSize = this._matrix.GapSize;

            //the vertical gap is larger than the horizontal main matrix gap AND is larger than the betting strategy gap
            if (maxVerticalGapSize >= horizontalGapSize && maxVerticalGapSize > _bettingStategyGap)
            {
                var trackingMatrix = _matrices[_maxVerticalGapMatrix - 1];
                //make sure that the current column matches the column where the gap exists
                if (trackingMatrix.CurrentColumn == trackingMatrix.MaxVerticalGapColumn)
                    ret = true;
            }
            //the vertical gap is smaller that the horizontal but the horizontal still larger than betting strategy gap
            else if (horizontalGapSize >= maxVerticalGapSize && horizontalGapSize >= _bettingStategyGap)
                ret = true;

            return ret;
        }
    }

    private Func<int, int, int> _calculatedRow = (boardCode, matrixColumn) =>
    {
        if (boardCode < matrixColumn)
            return 6 - (matrixColumn - boardCode);
        return boardCode > matrixColumn ? boardCode - matrixColumn : 0;
    };

    /// <summary>
    /// Initializes a board layout. 
    /// </summary>
    public void Initialize(int bettingStategyGap, int layoutCode)
    {
        //get the board code order for this layout code, we have 720 permuations
        List<int> codeOrder = Permutations.CurrentList[layoutCode - 1];
        _layoutCode = layoutCode;

        //add the columns to the board
        BoardColumn column;
        List<BoardNumber> boardNumbers;
        int boardNumber = 1;

        //column loop (based on the permutation) YOSHI!
        foreach (int i in codeOrder)
        {
            //now add the six board numbers per column
            boardNumbers = new List<BoardNumber>(6);
            for (int j = 1; j <= 6; j++)
            {
                boardNumbers.Add(new BoardNumber(boardNumber, i));
                boardNumber++;
            }
            column = new BoardColumn(i, boardNumbers);
            _columns.Add(column);
        }
        //now sort the columns into order
        _columns.Sort();

        _bettingStategyGap = bettingStategyGap;
        _bettingStrategy = new BettingStrategy(bettingStategyGap);
    }

    private bool _appliedImmovability { get; set; } = false;

    public bool AppliedImmovability
    {
        get
        {
            return _appliedImmovability;
        }
        set
        {
            _appliedImmovability = value;
        }
    }

    private void ResetColumnsWins() => _columns.ForEach(c => c.ResetWins());

    /// <summary>
    /// Searches for the first occurence of a number in the matrix which has no other occurences in the same column.
    /// It then deletes the numbers in the matrix from top down until that number's row. 
    /// Processing is then applied to the board layout by adding codes where applicable
    /// </summary>
    /// <param name="boardNumber">The board number which is immovable</param>
    /// <returns "BoardNumber">The same board number reference, but with an altered state in the form of a code edition</returns>
    public BoardNumber ApplyImmovabilty(BoardNumber boardNumber)
    {
        //find the first occurence of the matrix Number
        Nullable<MatrixNumber> matrixNumber = null;

        matrixNumber = _matrix.FindFirstOccurence(boardNumber.Number, boardNumber.BoardCode);

        //TRACE ISSUES! April 2010 : These issues have not occured in ages, I have removed these traces
        #region Old Tracing
        //if (matrixNumber == null)
        //    Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("BUG: The Number matching FindFirstOccurence condition was not found?!",matrixNumber.ToString()));
        //if (_matrix.Rows[matrixNumber.Value.Row - 1].Numbers[matrixNumber.Value.Column - 1].Number != matrixNumber.Value.Number)
        //    Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("BUG: The matrix number found is not in the delete matrix?!",matrixNumber.ToString()));
        #endregion

        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("Found First occurence of matrix number :{0} {1}:{2}"
            , matrixNumber.Value.Number, matrixNumber.Value.Row, matrixNumber.Value.Column));
        //now delete the rows until that number and get the deleted rows back for processing
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("Deleting {0} rows from Matrix. Current Rows {1}",
            matrixNumber.Value.Row, _matrix.Rows.Count));

        List<MatrixRow> deletedRows = _matrix.DeleteRows((MatrixNumber)matrixNumber.Value);

        #region MATRIX TRACING

        foreach (MatrixRow row in deletedRows)
        {
            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, row.ToString());
        }
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Search in remaining matrix as below");
        foreach (MatrixRow row in _matrix.Rows)
        {
            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, row.ToString());
        }
        #endregion

        //apply processing logic to deleted numbers including the immovable number, iterate column by column
        int number;
        BoardNumber boardNumberFound;
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("Number Of Deleted Matrix Row for Processing. {0}",
                deletedRows.Count));

        for (int i = 1; i <= 6; i++)
        {
            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("Processing Column. {0}", i));

            //now iterate the row of the column
            foreach (MatrixRow row in deletedRows)
            {
                //check if there is actually a number in that column
                if (i <= row.Numbers.Count)
                {
                    number = row.Numbers[i - 1].Number;

                    //nope, ok add the code back to the number
                    boardNumberFound = FindNumber(number);
                    Trace.WriteIf(Settings.Default.TRACE_ENABLED, string.Format("Processing Number {0}. Matching Board Number {1}.  ",
                        number, boardNumberFound.ToString()));

                    //is there another occurence of this number in the matrix column?
                    //always start a row 1 , as the matrix deletion rows have already been deleted
                    //Also check the deleted rows below where you are now for occurences
                    if (!_matrix.IsInColumn(number, i, 1) && !_matrix.IsInColumn(number, i, row.Numbers[i - 1].Row + 1, deletedRows))
                    {
                        if (boardNumber.Number == boardNumberFound.Number)
                            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("Attempting to re-add code {0} to WIN number {1}.",
                                i, boardNumberFound.ToString()));

                        //make sure the code attempting to add does not exist already
                        if (!boardNumberFound.Codes.Contains(i))
                        {
                            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("The code {0} was added to number {1}.",
                                i, boardNumberFound.ToString()));
                            boardNumberFound.Codes.Add(i);
                        }
                        else
                            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("BUG: The code {0} could not be added to number {1}.",
                                i, boardNumberFound.ToString()));
                    }
                    else
                    {
                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("The board number {0} has other instances in column {1}. Do Nothing.",
                            boardNumberFound.ToString(), i.ToString()));
                    }
                }
                else
                {
                    Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, string.Format("No number found in column {0}", i));
                }
            }
        }

        _appliedImmovability = true;

        resetCodeWins();
        ResetColumnsWins();

        return boardNumber;
    }

    /// <summary>
    /// Iterates the columns on the board and attempts the locate the number
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public BoardNumber FindNumber(int number)
    {
        //check in all columns until found
        BoardNumber boardNumber;
        foreach (BoardColumn column in _columns)
        {
            boardNumber = column.FindNumber(number);
            if (boardNumber != null)
                return boardNumber;
        }
        return null;
    }

    public void OnSpin(object sender, SpinEventArgs e)
    {
        CaptureSpin(e.Number);
    }

    /// <summary>
    /// Capture the result of the roulette wheel spin
    /// </summary>
    /// <param name="winningNumber"></param>
    public void CaptureSpin(int winningNumber)
    {
        //do nothing with 0 except increment spins & record history
        if (winningNumber != 0)
        {
            //Find the number on the board 
            BoardNumber number = this.FindNumber(winningNumber);

            //record the event in the board layouts matrix and get the current column back x1,x2,x3 etc
            MatrixNumber matrixNumber = _matrix.EnterSpin(winningNumber, number.BoardCode);
            int matrixColumn = matrixNumber.Column;

            //bug checking
            if (number != null)
            {
                _appliedImmovability = false;
                Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Before Code Removal :" + number.ToString());

                _codeWins[_calculatedRow(number.BoardCode, matrixColumn)]++;
                //Remove the code which matches the matrix column from the found number
                number.RemoveCode(matrixColumn);

                Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "After Code Removal :" + number.ToString());

                //is the matrix column the same as the board layout column? Then a Swap must be made
                //I beleive this is a win
                if (matrixColumn == number.BoardCode)
                {
                    //set the matrix Winning number property = true, this is used to calculate gap sizes on a board
                    matrixNumber.Win = true;

                    GetBoardColumn(number.BoardCode)!.AddWin();

                    Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "WIN --> Attempting swap of number " + number.ToString());

                    //check if number is immovable? If so, apply immovability logic to the boards matrix and layout
                    if (number.Codes.Count == 0)
                    {
                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Number is immovable! :" + number.ToString());

                        number = this.ApplyImmovabilty(number);

                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Immovability complete! :" + number.ToString());
                    }

                    //Was the number not swappable or double swappable"?
                    if (!this.Swap(number))
                    {
                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Number Not Swappable! :" + number.ToString());


                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Re-Applying Immovability on unswapable number:" + number.ToString());

                        //retry immovability 
                        number = this.ApplyImmovabilty(number);
                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Immovability complete! :" + number.ToString());

                        //now lets try the swap again!
                        if (!this.Swap(number))
                        {
                            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Number Not Swappable even after 2nd immovability!? :" + number.ToString());
                        }

                        //NOTE: this used to seem rite? but introduced bugs later with /triple/double swapping
                        //number.Codes.Add(matrixColumn);
                    }
                }
            }
            else  //basically the board number other than 0 is not on the board, its a definite bug!
            {
                throw new Exception("Computational Error: Number " + winningNumber.ToString() + " not found in board! ");
            }

            //now spin the betting strategy for the next cycle when betting is active
            if (BettingActive)
                _bettingStrategy.Spin(matrixNumber.Win);

            //now publish this to all other matrices
            MatrixSpin(this, new MatrixSpinEventArgs() { Number = matrixNumber });

            //clean
            number = null;

        }
        //number 0 condition, increment the 0 counter when betting is active, same numbers must be bet on
        else
        {
            //increment the zeroCounter if betting is active
            if (this.BettingActive)
                _bettingStrategy.Spin(false);

            Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "0 was recorded. Only Spin history affected");
        }


        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "------------------------------");
        Trace.Flush();
    }

    /// <summary>
    /// The Swap method attempts to swap numbers through single, double or triple swaps
    /// </summary>
    /// <param name="originalNumber">The original number</param>
    /// <returns>true/False whether the swop was possible or not</returns>
    public bool Swap(BoardNumber originalNumber)
    {
        //find all numbers on the board which have the code of the original
        var swapNumbers = SearchBoardForSwapNumbers(originalNumber.BoardCode);

        //single swap
        foreach (BoardNumber number in swapNumbers)
        {
            if (originalNumber.Codes.Contains(number.BoardCode) && number.Codes.Contains(originalNumber.BoardCode))
            {
                SwapNumbers(originalNumber, number);
                Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Single Swap Occured");
                return true;
            }
        }

        //double swap
        if (DoubleSwap(originalNumber))
            return true;

        //triple swap
        if (TripleSwap(originalNumber))
            return true;

        //no swap possible, return false
        return false;
    }

    /// <summary>
    /// The triple swap method tries to triple swap a number throught the 3 leg process
    /// </summary>
    /// <param name="originalNumber"></param>
    /// <returns></returns>
    private bool TripleSwap(BoardNumber originalNumber)
    {
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Attempting Triple Swap");

        //triple swap
        BoardColumn column, column2, column3;

        foreach (int code0 in originalNumber.Codes)
        {
            column = GetBoardColumn(code0);
            //first children
            foreach (BoardNumber number in column.Numbers)
            {
                foreach (int code2 in number.Codes)
                {
                    column2 = GetBoardColumn(code2);
                    //second children
                    foreach (BoardNumber secondNumber in column2.Numbers)
                    {
                        foreach (int code3 in secondNumber.Codes)
                        {
                            column3 = GetBoardColumn(code3);
                            //third children
                            foreach (BoardNumber thirdNumber in column3.Numbers)
                            {
                                //ensure the number has all board codes to do the full 3 way swap
                                if (thirdNumber.Codes.Contains(secondNumber.BoardCode)
                                    && thirdNumber.Codes.Contains(number.BoardCode)
                                    && thirdNumber.Codes.Contains(originalNumber.BoardCode)
                                    )
                                {
                                    SwapNumbers(thirdNumber, secondNumber);
                                    SwapNumbers(thirdNumber, number);
                                    SwapNumbers(originalNumber, thirdNumber);
                                    Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Triple Swap Occured");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// The double swap method tries to triple swap a number throught the 2 leg process
    /// </summary>
    /// <param name="originalNumber"></param>
    /// <returns></returns>
    private bool DoubleSwap(BoardNumber originalNumber)
    {
        //local vars
        BoardColumn swapColumn;
        BoardNumber swapNumber;

        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Attempting Double Swap");
        //Double Swap Scenario, search for number that can double swap to make the swap viable
        foreach (int code in originalNumber.Codes)
        {
            //get the current column and look for the number matching the swap code
            swapColumn = GetBoardColumn(code);

            //find the number on the board that contains the applicable swap codes
            List<BoardNumber> potentialSwaps = SearchBoardForSwapNumbers(code, originalNumber.BoardCode);

            //was a potential double swap number found for this column?
            if (potentialSwaps.Count > 0)
            {
                //now iterate the numbers in the column and find the swap number to swap with swapNumber2
                foreach (BoardNumber potentialSwap in potentialSwaps)
                {
                    //is there a corresponding swap number
                    swapNumber = swapColumn.FindNumberWithCode(potentialSwap.BoardCode);

                    //a swappable number was found! Now perform the double swap
                    if (swapNumber != null)
                    {
                        //swap the swap number and the potential swap
                        this.SwapNumbers(swapNumber, potentialSwap);

                        //now swap the potentialSwap with the originalNumber
                        this.SwapNumbers(originalNumber, potentialSwap);

                        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, "Double Swap Occured");
                        //return true the double swap succeeded
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Get a board column object based on the boardCode
    /// </summary>
    /// <param name="boardCode"></param>
    /// <returns></returns>
    public BoardColumn GetBoardColumn(int boardCode)
    {
        return _columns.Find(delegate (BoardColumn column) { return column.Code == boardCode; });
    }

    /// <summary>
    /// Swaps two numbers with each other on the board layout
    /// </summary>
    /// <param name="originalNumber">The original number</param>
    /// <param name="swapNumber">The number to swap the original with</param>
    private void SwapNumbers(BoardNumber originalNumber, BoardNumber swapNumber)
    {
        //get the columns for each of the numbers
        BoardColumn originalColumn = GetBoardColumn(originalNumber.BoardCode);
        BoardColumn swapColumn = GetBoardColumn(swapNumber.BoardCode);

        //remove the swap number / original number from their columns
        swapColumn.Numbers.Remove(swapNumber);
        originalColumn.Numbers.Remove(originalNumber);

        //add the swap number to its new location
        swapNumber.BoardCode = originalColumn.Code;
        originalColumn.Numbers.Add(swapNumber);

        //add the original number to its new location
        originalNumber.BoardCode = swapColumn.Code;
        swapColumn.Numbers.Add(originalNumber);

        //tracing 
        string traceMsg = string.Format("Number {0} moved board code from {1} to {2} and swopped with Number {3}",
            originalNumber.ToString(), originalColumn.Code, swapColumn.Code, swapNumber.ToString());
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, traceMsg);
        traceMsg = string.Format("Number {0} moved board code from {1} to {2} and swopped with Number {3}",
            swapNumber.ToString(), swapColumn.Code, originalColumn.Code, originalNumber.ToString());
        Trace.WriteLineIf(Settings.Default.TRACE_ENABLED, traceMsg);
    }

    /// <summary>
    /// Search the board for a number which has the two required swap codes, and not in the same column as the swap column
    /// </summary>
    /// <param name="swapCode">The column to swap too, cannot be the same as the search column</param>
    /// <param name="finalSwapCode">The final code to swap the number too </param>
    /// <returns>BoardNumber instance or null if not found</returns>
    private BoardNumber SearchBoardForSwapNumber(int swapCode, int finalSwapCode)
    {
        BoardNumber number = null;

        //you can only swap to a different column, never the same
        foreach (BoardColumn column in _columns)
        {
            if (column.Code != swapCode)
            {
                var containsCodes = new List<int>(2);
                containsCodes.Add(swapCode);
                containsCodes.Add(finalSwapCode);

                //look for the number that contains the required codes
                number = column.FindNumberWithCodes(containsCodes);
                if (number != null) return number;
            }
        }

        return null;
    }

    /// <summary>
    /// Searches the board for numbers that contain the swap code
    /// </summary>
    /// <param name="swapCode">the swap code to look for</param>
    /// <returns>returns a list of board numbers</returns>
    private List<BoardNumber> SearchBoardForSwapNumbers(int swapCode)
    {
        List<BoardNumber> numbers = new List<BoardNumber>();

        //get the board column 
        foreach (BoardColumn column in _columns)
        {
            //you can only swap to a different column, never the same
            if (column.Code != swapCode)
            {
                var containsCodes = new List<int>(1);
                containsCodes.Add(swapCode);

                //look for the number that contains the required codes
                var columnNumbers = column.FindNumbersWithCodes(containsCodes);
                if (columnNumbers.Count > 0) numbers.AddRange(columnNumbers);
            }
        }

        return numbers;
    }

    /// <summary>
    /// Returns a list of numbers on the board which contain the two specificed swap codes
    /// </summary>
    /// <param name="swapCode">The column to swap too, cannot be the same as the search column</param>
    /// <param name="finalSwapCode">The final code to swap the number too </param>
    /// <returns>BoardNumber collection</returns>
    private List<BoardNumber> SearchBoardForSwapNumbers(int swapCode, int finalSwapCode)
    {
        List<BoardNumber> numbers = new List<BoardNumber>();


        //get the board column 
        foreach (BoardColumn column in _columns)
        {
            //you can only swap to a different column, never the same
            if (column.Code != swapCode)
            {
                var containsCodes = new List<int>(2);
                containsCodes.Add(swapCode);
                containsCodes.Add(finalSwapCode);

                //look for the number that contains the required codes
                var columnNumbers = column.FindNumbersWithCodes(containsCodes);
                if (columnNumbers.Count > 0) numbers.AddRange(columnNumbers);
            }
        }

        return numbers;
    }

    public BoardLayout Clone()
    {
        //clone the board columns
        var boardColumns = new List<BoardColumn>(_columns.Count);
        Array.ForEach<BoardColumn>(_columns.ToArray(), x => boardColumns.Add(x.Clone()));

        //clone the matrix
        var matrix = _matrix.Clone();

        //tracking matrices
        var trackingMatrices = new List<TrackingMatrix>(_matrices.Count);
        Array.ForEach<TrackingMatrix>(_matrices.ToArray(), x => trackingMatrices.Add(x.Clone()));

        //betting strategy
        var bettingStrategy = _bettingStrategy.Clone();

        var boardLayout = new BoardLayout(boardColumns, matrix, trackingMatrices, _layoutCode, _bettingStategyGap, bettingStrategy, _maxVerticalGapMatrix);

        return boardLayout;
    }
}
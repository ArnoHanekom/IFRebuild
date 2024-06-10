namespace Infinity.Roulette.Rework.Engine;

[Serializable()]
public class MatrixRow
{
    List<MatrixNumber> _numbers;

    public MatrixRow()
    {
        _numbers = new List<MatrixNumber>(6);
    }

    public List<MatrixNumber> Numbers
    {
        get { return _numbers; }
        set { _numbers = value; }
    }

    public MatrixRow Clone()
    {
        var newNumbers = new List<MatrixNumber>(6);
        Array.ForEach<MatrixNumber>(_numbers.ToArray(), x => newNumbers.Add(x.Clone()));

        var matrixRow = new MatrixRow();
        matrixRow.Numbers = newNumbers;
        return matrixRow;
    }

    public override string ToString()
    {
        string s = string.Empty;

        Array.ForEach<MatrixNumber>(_numbers.ToArray(), x => s += x.Number.ToString() + '\t');

        return s;
    }
}
#nullable enable
namespace Infinity.Engine
{
    public class MatrixRow
    {
        private List<MatrixNumber> _numbers { get; set; } = new(6);

        public List<MatrixNumber> Numbers
        {
            get => _numbers;
            set => _numbers = value;
        }

        private MatrixNumber[] _colNumbers { get; set; } = new MatrixNumber[6];
        public MatrixNumber[] ColumnNumbers
        {
            get => _colNumbers;
            set => _colNumbers = value;
        }
        public override string ToString() => string.Join("\t", _numbers);
    }
}

namespace Infinity.Roulette.Rework.Engine;

public delegate void SpinEventHandler(object sender, SpinEventArgs e);
public delegate void MatrixSpinEventHandler(object sender, MatrixSpinEventArgs e);

public class SpinEventArgs : EventArgs
{
    public int Number { get; set; }
}
public class MatrixSpinEventArgs : EventArgs
{
    public MatrixNumber Number { get; set; }
}
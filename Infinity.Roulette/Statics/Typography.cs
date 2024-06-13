using System.Windows.Media;
using System.Windows;

namespace Infinity.Roulette.Statics
{
    public static class Typography
    {
        public static FontFamily CenturyGothic => new("Century Gothic");
        public static Style? NormalResultRow => Application.Current?.FindResource("NormalResultRow") as Style;
        public static Style? HighlightWinsMatch => Application.Current?.FindResource("HighlightWinsMatch") as Style;
    }
}

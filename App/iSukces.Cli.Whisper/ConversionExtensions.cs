using System.Globalization;

namespace iSukces.Cli.Whisper;

internal static class ConversionExtensions
{
    extension(double value)
    {
        public string ToInvariantString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
 
}

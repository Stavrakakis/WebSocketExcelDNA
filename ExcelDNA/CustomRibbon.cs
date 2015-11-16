using ExcelDna.Integration.CustomUI;
using System.Runtime.InteropServices;

namespace ExcelDNA
{
    [ComVisible(true)]
    public class MyRibbon : ExcelRibbon
    {
        public static void ConnectToSocket()
        {
            CalculationEngine.ConnectToSonnect();
        }
    }
}

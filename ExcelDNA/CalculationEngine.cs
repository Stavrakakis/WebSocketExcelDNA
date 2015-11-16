using System;
using ExcelDna.Integration;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SocketIOClient;
using System.Reflection;
using System.Windows.Forms;

namespace ExcelDNA
{
    public class CalculationEngine : IExcelAddIn
    {
        static Client socket;

        private static bool connected = false;

        public void AutoOpen()
        {
            ConnectToSonnect();
        }

        public void AutoClose()
        {
            
        }

        [ExcelFunction(Description = "Subscribe to a web socket feed")]
        public static object Subscribe(String feed)
        {
            Func<IObservable<double>> f2 = () => Observable.Create<double>(CalculationObservable);

            return RxExcel.Observe("Subscribe", feed, f2, socket);
        }

        public static void ConnectToSonnect()
        {
            if (connected)
            {
                return;
            }

            try
            {
                object app;
                object rtd;
                app = ExcelDnaUtil.Application;
                rtd = app.GetType().InvokeMember("RTD", BindingFlags.GetProperty, null, app, null);
                rtd.GetType().InvokeMember("ThrottleInterval", BindingFlags.SetProperty, null, rtd, new object[] { 100 });

                socket = new Client("http://localhost:8000");
                socket.Connect();
                socket.Opened += SocketOpened;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        private static Func<IObserver<double>, IDisposable> CalculationObservable = observer =>
        {
            return Disposable.Empty;                        
        };

        private static void SocketOpened(object sender, EventArgs e)
        {
            connected = true;
        }
    }
}

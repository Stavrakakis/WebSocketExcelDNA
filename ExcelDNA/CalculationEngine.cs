using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using SocketIOClient;
using System.Reflection;


namespace ExcelDNA
{
    public class CalculationEngine : IExcelAddIn
    {
        static Client socket;

        public void AutoOpen()
        {
            object app;
            object rtd;
            app = ExcelDnaUtil.Application;
            rtd = app.GetType().InvokeMember("RTD", BindingFlags.GetProperty, null, app, null);
            rtd.GetType().InvokeMember("ThrottleInterval", BindingFlags.SetProperty, null, rtd, new object[] { 100 });
            
            socket = new Client("http://localhost:8000");
            socket.Connect();
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

        static Func<IObserver<double>, IDisposable> CalculationObservable = observer =>
        {
            return Disposable.Empty;                        
        };

    }
}

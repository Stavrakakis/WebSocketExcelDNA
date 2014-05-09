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
        

        public static event TimeSentHandler OnTimeSent;
        public delegate void TimeSentHandler(TimeSentEventArgs args);        
        public class TimeSentEventArgs : EventArgs
        {
            public DateTime Time { get; private set; }

            public TimeSentEventArgs(DateTime time)
            {
                this.Time = time;
            }
        }

        public class ValueSentEventArgs : EventArgs
        {
            public double Calculation { get; private set; }

            public ValueSentEventArgs(double calculation)
            {
                this.Calculation = calculation;
            }
        }

        [ExcelFunction(Description = "Test Calculation")]
        public static object Calculate(String feed)
        {
            Func<IObservable<double>> f2 = () => Observable.Create<double>(CalculationObservable);

            return RxExcel.Observe("Calculate", feed, f2, socket, feed);
        }


        static Func<IObserver<double>, IDisposable> CalculationObservable = observer =>
        {
            return Disposable.Empty;                        
        };

        static Func<IObserver<DateTime>, IDisposable> TimeObservable = observer =>
        {
            OnTimeSent += d => observer.OnNext(d.Time);
            return Disposable.Empty;
        };

    }
}

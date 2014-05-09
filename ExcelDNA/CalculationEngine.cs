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
        

        // event boilerplate stuff
        public delegate void ValueSentHandler(ValueSentEventArgs args);
        public static event ValueSentHandler OnValueSent;
        public class ValueSentEventArgs : EventArgs
        {
            public double Calculation { get; private set; }
            
            public ValueSentEventArgs(double calculation)
            {
                this.Calculation = calculation;
            }
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


        [ExcelFunction(Description = "Test Calculation")]
        public static object Calculate(int input)
        {
            socket.On("calculation", (data) =>
            {
                ValueSentEventArgs value = data.Json.GetFirstArgAs<ValueSentEventArgs>();

                if (OnValueSent != null)
                    OnValueSent(value);
            });


            Func<IObservable<double>> f2 = () => Observable.Create<double>(CalculationObservable);


            return RxExcel.Observe("Calculate", null, f2);
        }

        [ExcelFunction(Description = "Test Calculation")]
        public static object GetTime()
        {
            socket.On("time", (data) =>
            {
                TimeSentEventArgs value = data.Json.GetFirstArgAs<TimeSentEventArgs>();

                if (OnTimeSent != null)
                    OnTimeSent(value);
            });


            Func<IObservable<DateTime>> f2 = () => Observable.Create<DateTime>(TimeObservable);


            return RxExcel.Observe("GetTime", null, f2);
        }


        static Func<IObserver<double>, IDisposable> CalculationObservable = observer =>
        {
            OnValueSent += d => observer.OnNext(d.Calculation);   
            return Disposable.Empty;                        
        };

        static Func<IObserver<DateTime>, IDisposable> TimeObservable = observer =>
        {
            OnTimeSent += d => observer.OnNext(d.Time);
            return Disposable.Empty;
        };

    }
}

using ExcelDna.Integration;
using SocketIOClient;
using System;

namespace ExcelDNA
{

    public static class RxExcel
    {

        public static IExcelObservable ToExcelObservable<T>(this IObservable<T> observable, Client socket, string feed)
        {
            return new ExcelObservable<T>(observable, socket, feed);
        }

        public static object Observe<T>(string functionName, object parameters, Func<IObservable<T>> observableSource, Client socket, string feed)
        {
            return ExcelAsyncUtil.Observe(functionName, parameters, () => observableSource().ToExcelObservable(socket, feed));
        }
    }

    public class ExcelObservable<T> : IExcelObservable
    {

        readonly IObservable<T> _observable;
        readonly string _feed;
        readonly Client _socket;

        public ExcelObservable(IObservable<T> observable, Client socket, string feed)
        {
            _feed = feed;
            _socket = socket;
            _observable = observable;
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

        public IDisposable Subscribe(IExcelObserver observer)
        {

            _socket.On(_feed, (data) =>
            {
                ValueSentEventArgs value = data.Json.GetFirstArgAs<ValueSentEventArgs>();
                
                observer.OnNext(value.Calculation);

            });

            return _observable.Subscribe(value => observer.OnNext(value), observer.OnError, observer.OnCompleted);
        }
    }
}

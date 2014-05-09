using System;

namespace ExcelDna.Integration.RxExcel {
    
    public static class RxExcel {

        public static IExcelObservable ToExcelObservable<T>(this IObservable<T> observable) {
            return new ExcelObservable<T>(observable);
        }

        public static object Observe<T>(string functionName, object parameters, Func<IObservable<T>> observableSource) {
            return ExcelAsyncUtil.Observe(functionName, parameters, () => observableSource().ToExcelObservable());
        }
    }

    public class ExcelObservable<T> : IExcelObservable {

        readonly IObservable<T> _observable;

        public ExcelObservable(IObservable<T> observable) {
            _observable = observable;
        }

        public IDisposable Subscribe(IExcelObserver observer) {
            return _observable.Subscribe(value => observer.OnNext(value), observer.OnError, observer.OnCompleted);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public sealed class OData<TEnclodedType>
    {
        internal OData(TEnclodedType enclosedElement)
        {
            //// TODO make this a struct so that you can't create default instances?
            this.EnclosedElement = enclosedElement;
        }

        public TEnclodedType EnclosedElement { get; }
    }

    public static class ODataExtensions
    {
        public static ODataResult<TResult> Evaluate<TEnclosedType, TResult>(this IV2Queryable<OData<TEnclosedType>> queryable, Func<IEnumerable<TEnclosedType>, TResult> projection)
        {
            if (queryable is IODataEvaluator<TEnclosedType> odataEvaluator)
            {
                return odataEvaluator.Evaluate(projection);
            }

            try
            {
                var result = projection(queryable.AsEnumerable().Select(element => element.EnclosedElement));
                return new ODataResult<TResult>(new RavudetNullable<TResult>(result), null);
            }
            catch (Exception e)
            {
                return new ODataResult<TResult>(new RavudetNullable<TResult>(), e);
            }
        }
    }

    public struct RavudetNullable<T>
    {
        public RavudetNullable(T value)
        {
            this.Value = value; 
        }

        public T Value { get; }
    }

    public sealed class ODataResult<TResult>
    {
        public ODataResult(RavudetNullable<TResult> result, Exception? exception)
        {
            this.Result = result;
            this.Exception = exception;
        }

        public RavudetNullable<TResult> Result { get; }

        public Exception? Exception { get; }
    }

    public interface IODataEvaluator<TEnclosedType>
    {
        ODataResult<TResult> Evaluate<TResult>(Func<IEnumerable<TEnclosedType>, TResult> projection);
    }

    public sealed class ODataGraphCalendar : IV2Queryable<OData<GraphCalendarEvent>>, IODataEvaluator<GraphCalendarEvent>, IWhereQueryable<OData<GraphCalendarEvent>>
    {
        public ODataResult<TResult> Evaluate<TResult>(Func<IEnumerable<GraphCalendarEvent>, TResult> projection)
        {
            var resultBuilder = new ODataResultBuilder();
            var enumerable = GetEnumerable(resultBuilder);
            var result = projection(enumerable);
            return new ODataResult<TResult>(new RavudetNullable<TResult>(result), resultBuilder.Exception);
        }

        private sealed class ODataResultBuilder
        {
            public Exception? Exception { get; set; }
        }

        public IEnumerator<OData<GraphCalendarEvent>> GetEnumerator()
        {
            return GetEnumerable(new ODataResultBuilder()).Select(calendarEvent => new OData<GraphCalendarEvent>(calendarEvent)).GetEnumerator();
        }

        private static IEnumerable<GraphCalendarEvent> GetEnumerable(ODataResultBuilder builder)
        {
            int page;
            try
            {
                page = GetNextPage();
            }
            catch (Exception e)
            {
                builder.Exception = e;
                yield break;
            }

            yield return new GraphCalendarEvent();
        }

        private static int GetNextPage()
        {
            return 0;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

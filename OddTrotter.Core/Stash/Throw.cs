using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stash
{

    /// <summary>
    /// TODO can you use this anywhere?
    /// </summary>
    public static class ExceptionExtensions
    {
        public static Nothing Throw<TException>(this TException exception) where TException : Exception
        {
            throw exception;
        }
    }

    public struct Throw<T>
    {
        public static implicit operator Nothing(Throw<T> @throw)
        {
            return new Nothing();
        }

        public static implicit operator T(Throw<T> @throw)
        {
            //// TODO probably not safe, but we are wanting it here for type inference and lambdas
            return default(T)!;
        }
    }
}

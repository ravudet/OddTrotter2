namespace CalendarV2.System.Linq
{
    using global::System.Collections.Generic;

    using CalendarV2.Fx.Either;
    using global::System;
    using CalendarV2.Fx.Try;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq;

    public static class EnumerableExtensions
    {
        public sealed class EitherFirstOrDefaultResult<TElement, TDefault> : IEither<TElement, TDefault>
        {
            private readonly IEither<TElement, TDefault> either;

            public EitherFirstOrDefaultResult(IEither<TElement, TDefault> either)
            {
                //// TODO any type that looks like this can actually end up implementing their own either and visitor and whatnot if the performance of the eitherdelegatevisitor ends up being bad...
                this.either = either;
            }

            public TResult Visit<TResult, TContext>(
                global::System.Func<TElement, TContext, TResult> leftAccept, 
                global::System.Func<TDefault, TContext, TResult> rightAccept,
                TContext context)
            {
                return either.Visit(leftAccept, rightAccept, context);
            }
        }

        public static EitherFirstOrDefaultResult<TElement, TDefault> EitherFirstOrDefault<TElement, TDefault>(
            this IEnumerable<TElement> source,
            TDefault @default)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new EitherFirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Right(
                        @default));
                }

                return new EitherFirstOrDefaultResult<TElement, TDefault>(new Either<TElement, TDefault>.Left(
                    enumerator.Current));
            }
        }

        public static IEnumerable<TResult> TrySelect1<TElement, TResult>(
            this IEnumerable<TElement> source,
            Try1<TElement, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try.Try(element, out var selected))
                {
                    yield return selected;
                }
            }
        }

        public static IEnumerable<TResult> TrySelect2<TElement, TResult>(
            this IEnumerable<TElement> source,
            Try2<TElement, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try(element, out var selected))
                {
                    yield return selected;
                }
            }
        }

        public static Try2<TInput, TOutput> ToTry<TInput, TOutput>(this Func<TInput, TOutput> func)
        {
            return (TInput input, [MaybeNullWhen(false)] out TOutput output) =>
            {
                try
                {
                    output = func(input);
                    return true;
                }
                catch
                {
                    output = default;
                    return false;
                }
            };
        }

        public static bool TryParse(string input, out int result)
        {
            return int.TryParse(input, out result);
        }
        
        public class Shape
        {
        }

        public class Rectangle : Shape
        {
        }

        public class Square : Rectangle
        {
        }

        public class Circle : Shape
        { 
        }

        public class Animal
        {
        }

        public class Dog : Animal
        {
        }

        public class Husky : Dog
        {
        }

        public class Cat : Animal
        {
        }

        public static bool Adapt21(Shape input, [MaybeNullWhen(false)] out Animal output)
        {
            throw new Exception("TODO");
        }

        public static bool Adapt22(Shape input, [MaybeNullWhen(false)] out Dog output)
        {
            throw new Exception("TODO");
        }

        public static Animal Adapt11(Shape input, out bool success)
        {
            throw new Exception("TODO");
        }

        public static Dog Adapt12(Shape input, out bool success)
        {
            throw new Exception("TODO");
        }

        public static void CovarianceUseCase1(Try1<Shape, Animal> @try)
        {
        }

        public static void CovarianceUseCase2(Try2<Shape, Animal> @try)
        {
        }

        public static void TrySelectUseCase(IEnumerable<Shape> input)
        {
            IEnumerable<Animal> animals;
            animals = input.TrySelect2<Shape, Animal>(Adapt21);
            animals = input.TrySelect1(Adapt11);
            animals = input.TrySelect2<Shape, Dog>(Adapt22);
            animals = input.TrySelect1(Adapt12);

            CovarianceUseCase1(Adapt11);
            CovarianceUseCase1(Adapt12);
            CovarianceUseCase2(Adapt21);
            CovarianceUseCase2(Adapt22);


            var data = new[] { "Asfd" };

            data.TrySelect2((Try2<string, int>)int.TryParse);

            data.TrySelect2(EnumerableExtensions.TryParse);


            ((Func<string, int>)int.Parse).ToTry()

            var data = new[] { "Asfd" };
            data.TrySelect((input => ((Try<string, int>)int.TryParse).ToEither(input)));

            //// TODO maybe try to overload && and || to somehow deal with "tries" on eithers?
            data.TrySelect(TryParse);

            data.TrySelect((Try<string, int>)int.TryParse);
        }

        public static IEither<int, CalendarV2.System.Void> TryParse(string input)
        {
            if (int.TryParse(input, out var result))
            {
                return Either.Left(result).Right<CalendarV2.System.Void>();
            }
            else
            {
                return Either.Left<int>().Right(new CalendarV2.System.Void());
            }
        }

        public static Either<int, Exception> ToTry(Func<string, int> func, string input)
        {
            try
            {
                return Either.Left(func(input)).Right<Exception>();
            }
            catch (Exception e)
            {
                return Either.Left<int>().Right(e);
            }
            //// TODO propbably throw this method away until you hvae a real use case
        }

        public static IEnumerable<TResult> TrySelect<TElement, TResult>(
            this IEnumerable<TElement> source,
            Try<TElement, TResult> @try)
        {
            foreach (var element in source)
            {
                if (@try.Try(element, out var result))
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<TResult> TrySelect<TElement, TResult>(
            this IEnumerable<TElement> source,
            Func<TElement, IEither<TResult, CalendarV2.System.Void>> selector)
        {
            foreach (var element in source)
            {
                if (selector(element).Try(out var left))
                {
                    yield return left;
                }
            }
        }
    }
}

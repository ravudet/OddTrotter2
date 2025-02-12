namespace CalendarV2.Fx.Try
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq;

    using global::Fx.Either;

    public delegate bool Try<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput output);

    //// TODO maybe this is called something that's not try? or maybe the above is called something that's not try; one of them is definitely try, and the other is probably more verbose
    /*public delegate bool Try4<TOutput>([MaybeNullWhen(false)] out TOutput output);

    public static class TryPlayground
    {
        public static void DoWork(Either<string, Nothing> either)
        {
            UseTry<string>(either.TryGet);
        }

        private static bool TryGetValue(Either<string, Nothing> either, out string value)
        {
            value = string.Empty;
            return true;
        }

        private static void UseTry<TOutput>(Try4<TOutput> @try)
        {

        }
    }*/

    //// TODO TOPIC everything below here

    public delegate TOutput? Try1<in TInput, out TOutput>(TInput input, out bool success);
    public delegate bool Try2<in TInput, TOutput>(TInput input, [MaybeNullWhen(false)] out TOutput success);
    public delegate IEither<TOutput, Nothing> Try3<in TInput, out TOutput>(TInput input);
    
    public static class Try1Extensions
    {
        public static bool Try<TInput, TOutput>(this Try1<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            output = @try(input, out var success);
            return success;
        }

        public static Try2<TInput, TOutput> ToTry2<TInput, TOutput>(this Try1<TInput, TOutput> @try)
        {
            return @try.Try;
        }
    }

    public static class Try2Extensions
    {
        public static bool Try<TInput, TOutput>(this Try2<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            return @try(input, out output);
        }

        /*public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this OldTry<TInput, TOutput> oldTry)
        {
            return (TInput input, out bool success) =>
            {
                success = oldTry(input, out var output);
                return output!; //// TODO can you actually do this if `output` ends up being `null`?
            };
        }*/
    }

    public static class Try3Extensions
    {
        public static bool Try<TInput, TOutput>(this Try3<TInput, TOutput> @try, TInput input, [MaybeNullWhen(false)] out TOutput output)
        {
            return @try(input).TryGetLeft(out output);
        }

        public static Try2<TInput, TOutput> ToTry2<TInput, TOutput>(this Try3<TInput, TOutput> @try)
        {
            return @try.Try;
        }

        /*public static bool Try<TInput, TOutput>(this EitherTry<TInput, TOutput> eitherTry, TInput input, out TOutput output)
        {
            eitherTry(input).TryLeft()
        }*/

        /*public static Try<TInput, TOutput> ToTry<TInput, TOutput>(this Func<TInput, TOutput> func)
        {
            int? foo = 0;

            foo.ToEither();

            return (TInput input, out bool output) =>
            {
                try
                {
                    output = true;
                    return func(input);
                }
                catch
                {
                    output = false;
                    return default!;
                }
            };
        }*/

        /*public static IEither<T, CalendarV2.System.Void> ToEither<T>(this Nullable<T> nullable) where T : struct
        {
            //// TODO do you need this variant? `int? foo = 0; foo.ToEither();` works without it....
            if (nullable.HasValue)
            {
                return Either.Left(nullable.Value).Right<CalendarV2.System.Void>();
            }
            else
            {
                return Either.Left<T>().Right(new CalendarV2.System.Void());
            }
        }*/

        public static IEither<T, Nothing> ToEither<T>(this T? nullable)
        {
            if (nullable == null)
            {
                return Either.Left<T>().Right(new Nothing());
            }
            else
            {
                return Either.Left(nullable).Right<Nothing>();
            }
        }
    }

    public static class MoreEnumerableExtensions
    {
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
            //// Either<int, Exception> int.Parse(string) <- maybe call this extension "try"?
            //// Either<TLeft, Void> ______<TLeft, TRight>(Either<TLeft, TRight>)
            //// bool TryLeft<TLeft, TRight>(Either<TLeft, TRight>, out TLeft)
            //// Parse.TryLeft

            /*IEnumerable<Animal> animals;
            animals = input.TrySelect2<Shape, Animal>(Adapt21);
            animals = input.TrySelect1(Adapt11);
            animals = input.TrySelect2<Shape, Dog>(Adapt22);
            animals = input.TrySelect1(Adapt12);

            if (Adapt22(new Shape(), out var dog))
            {
                animals = animals.Append(dog);
            }
            

            CovarianceUseCase1(Adapt11);
            CovarianceUseCase1(Adapt12);
            CovarianceUseCase2(Adapt21);
            CovarianceUseCase2(Adapt22);

            var tries1 = Enumerable
                .Empty<Try1<Shape, Animal>>()
                .Append(Adapt11)
                .Append(Adapt12);

            var tires2 = Enumerable
                .Empty<Try2<Shape, Animal>>()
                .Append(Adapt21)
                .Append(Adapt22);

            var data = new[] { "Asfd" };

            data.TrySelect2((Try2<string, int>)int.TryParse);

            data.TrySelect2(MoreEnumerableExtensions.TryParse);


            ((Func<string, int>)int.Parse).ToTry();

            var data2 = new[] { "Asfd" };
            data2.TrySelect((input => ((Try<string, int>)int.TryParse).ToEither(input)));

            //// TODO maybe try to overload && and || to somehow deal with "tries" on eithers?
            data2.TrySelect(TryParse);

            data2.TrySelect((Try<string, int>)int.TryParse);*/
        }

        public static IEither<int, Nothing> TryParse(string input)
        {
            if (int.TryParse(input, out var result))
            {
                return Either.Left(result).Right<Nothing>();
            }
            else
            {
                return Either.Left<int>().Right(new Nothing());
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
            Func<TElement, IEither<TResult, Nothing>> selector)
        {
            foreach (var element in source)
            {
                if (selector(element).TryGet(out var left))
                {
                    yield return left;
                }
            }
        }
    }
}

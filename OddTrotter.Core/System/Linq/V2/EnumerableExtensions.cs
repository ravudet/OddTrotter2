namespace System.Linq.V2
{
    public static partial class EnumerableExtensions
    {
        /*public static void DoWork()
        {
            var data = new[] { "asdf" }.ToV2Enumerable();
            var aggregated = data.ApplyAggregation(0, (aggregation, element) => Math.Max(aggregation, element.Length));

            var secondAggregated = aggregated.ApplyAggregation(string.Empty, (aggregation, element) => aggregation + element);

            var thirdAggregated = secondAggregated.ApplyAggregation('\0', (aggregation, element) => (char)Math.Max(aggregation, element[0]));



            IV2Enumerable<Application> applications = null; //// TODO would this whole thing work for the groupby implementation?

            var aggregatedApplications = applications
                .ApplyAggregation(
                    Enumerable.Empty<Application>(),
                    (singlePageApplications, application) => 
                        application.SinglePageApplicationSettings == null ? singlePageApplications : singlePageApplications.Append(application))
                .ApplyAggregation(
                    Enumerable.Empty<Application>(), 
                    (clientApplications, application) => application.ClientApplicationSettings == null ? clientApplications : clientApplications.Append(application))
                .ApplyAggregation(
                    Enumerable.Empty<Application>(), 
                    (publicApiApplications, application) => application.PublicApiApplicationSettings == null ? publicApiApplications : publicApiApplications.Append(application));

            var publicapi = aggregatedApplications.Aggregation; //// TODO this won't be lazily evaluated; is that ok?
            var client = aggregatedApplications.OnTopOf.Aggregation;
            var spa = aggregatedApplications.OnTopOf.OnTopOf.Aggregation;
        }

        public sealed class Application
        {
            public SinglePageApplicationSettings? SinglePageApplicationSettings { get; set; }

            public WebApplicationSettings? WebApplicationSettings { get; set; }

            public ClientApplicationSettings? ClientApplicationSettings { get; set; }

            public PublicApiApplicationSettings? PublicApiApplicationSettings { get; set; }
        }

        public sealed class SinglePageApplicationSettings
        {
            public string SpaProp { get; set; }
        }

        public sealed class WebApplicationSettings
        {
            public string WebAppProp { get; set; }
        }

        public sealed class ClientApplicationSettings
        {
            public string ClientAppProp { get; set; }
        }

        public sealed class PublicApiApplicationSettings
        {
            public string PublicApiProp { get; set; }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="self"></param>
        /// <param name="seed"></param>
        /// <param name="aggregator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="aggregator"/> is <see langword="null"/></exception>
        public static IAggregatedEnumerable<TAggregate, TElement, IV2Enumerable<TElement>> ApplyAggregation<TAggregate, TElement>(
            this IV2Enumerable<TElement> self,
            TAggregate seed, 
            Func<TAggregate, TElement, TAggregate> aggregator)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (aggregator == null)
            {
                throw new ArgumentNullException(nameof(aggregator));
            }

            if (self is IApplyAggregationEnumerable<TElement> applyAggregation)
            {
                return applyAggregation.ApplyAggregation(seed, aggregator);
            }

            return self.ApplyAggregationDefault(seed, aggregator);
        }
    }
}

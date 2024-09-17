////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public sealed class ComposedRequestEvaluator : IRequestEvaluator
    {
        private readonly Dictionary<SelectProperty.NavigationProperty, IRequestEvaluator> navigationPropertyEvaluators;

        private readonly Dictionary<SelectProperty.FullSelectPath, IRequestEvaluator> fullSelectPathEvaluators;

        public ComposedRequestEvaluator(
            Dictionary<SelectProperty.NavigationProperty, IRequestEvaluator> navigationPropertyEvaluators,
            Dictionary<SelectProperty.FullSelectPath, IRequestEvaluator> fullSelectPathEvaluators)
        {
            this.navigationPropertyEvaluators = navigationPropertyEvaluators;
            this.fullSelectPathEvaluators = fullSelectPathEvaluators;
        }

        public async Task<OdataResponse.Instance> Evaluate(OdataRequest.GetInstance request)
        {
            if (request.Select != null)
            {
                var navigationPropertyResponses = new Dictionary<
                    SelectProperty.NavigationProperty, 
                    OdataResponse.Instance>();
                var fullSelectPathResponses = new Dictionary<
                    SelectProperty.FullSelectPath, 
                    OdataResponse.Instance>();
                foreach (var selectItem in request.Select.SelectItems)
                {
                    if (selectItem is SelectItem.PropertyPath propertyPath)
                    {
                        if (propertyPath is SelectItem.PropertyPath.Second second)
                        {
                            if (second.SelectProperty is SelectProperty.NavigationProperty navigationProperty)
                            {
                                if (this.navigationPropertyEvaluators.TryGetValue(navigationProperty, out var evaluator))
                                {
                                    //// TODO actually create the correct subrequest
                                    var subRequest = new OdataRequest.GetInstance(request.Uri, null, null);
                                    var subResponse = await evaluator.Evaluate(subRequest).ConfigureAwait(false);

                                    navigationPropertyResponses.Add(navigationProperty, subResponse);
                                }
                                else
                                {
                                    throw new Exception("TODO");
                                }
                            }
                            else if (second.SelectProperty is SelectProperty.FullSelectPath fullSelectPath)
                            {
                                if (this.fullSelectPathEvaluators.TryGetValue(fullSelectPath, out var evaluator))
                                {
                                    //// TODO actually create the correct subrequest
                                    var subRequest = new OdataRequest.GetInstance(request.Uri, null, null);
                                    var subResponse = await evaluator.Evaluate(subRequest).ConfigureAwait(false);

                                    fullSelectPathResponses.Add(fullSelectPath, subResponse);
                                }
                                else
                                {
                                    throw new Exception("TODO");
                                }
                            }
                            else
                            {
                                throw new Exception("TODO");
                            }
                        }
                        else
                        {
                            throw new Exception("TODO");
                        }
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }
                }

                //// TODO stitch torgether navigationPropertyResponses and fullSelectPathResponses
                return new OdataResponse.Instance(System.IO.Stream.Null);
            }

            throw new Exception("TODO");
        }

        public Task<OdataResponse.Collection> Evaluate(OdataRequest.GetCollection request)
        {
            throw new System.NotImplementedException();
        }
    }
}

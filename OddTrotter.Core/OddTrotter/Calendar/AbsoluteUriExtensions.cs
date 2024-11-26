namespace OddTrotter.Calendar
{
    using System;

    public static class AbsoluteUriExtensions
    {
        public static RelativeUri GetRelativeUri(this AbsoluteUri absoluteUri)
        {
            var relativeUri = absoluteUri.GetComponents(
                UriComponents.Path | UriComponents.Query | UriComponents.Fragment, 
                UriFormat.Unescaped); //// TODO is this the correct uriformat?
            return new Uri(relativeUri, UriKind.Relative).ToRelativeUri();
        }
    }
}

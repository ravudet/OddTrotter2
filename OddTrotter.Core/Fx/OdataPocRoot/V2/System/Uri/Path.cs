namespace Fx.OdataPocRoot.V2.System.Uri
{
    using global::System;
    
    public sealed class Path
    {
        private readonly string path;

        public Path(string path)
        {
            var uri = new Uri(path, UriKind.Relative).ToRelativeUri();

            var nonPathComponents = uri.GetComponents(RelativeUriComponents.Query | RelativeUriComponents.Fragment, UriFormat.Unescaped);
            if (!string.IsNullOrEmpty(nonPathComponents))
            {
                throw new Exception("TODO");
            }

            this.path = uri.GetComponents(RelativeUriComponents.Path, UriFormat.Unescaped);
        }

        /// <summary>
        /// TODO should this be the segments instead?
        /// </summary>
        public string Value
        {
            get
            {
                return this.path;
            }
        }
    }
}

namespace System
{
    /// <summary>
    /// The individual parts of a relative URI, corresponding to the absolute URI parts defined by <see cref="UriComponents"/>
    /// </summary>
    [Flags]
    public enum RelativeUriComponents
    {
        Path = 1 << 0,
        Query = 1 << 1,
        Fragment = 1 << 2,
        KeepDelimiter = (int.MaxValue >> 1) + 1, // this is the value of System.UriComponents.KeepDelimiter
    }
}

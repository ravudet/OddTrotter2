namespace OddTrotter
{
    using System;
    using System.Collections.Generic;

    public sealed class MissingFormDataException : Exception
    {
        public MissingFormDataException(IEnumerable<string> missingFormFieldNames)
            : base()
        {
            this.MissingFormFieldNames = missingFormFieldNames;
        }

        public IEnumerable<string> MissingFormFieldNames { get; }
    }
}

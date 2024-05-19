namespace OddTrotter
{
    using System;

    public sealed class InvalidFormDataException : Exception
    {
        public InvalidFormDataException(string formFieldName)
            : base()
        {
            FormFieldName = formFieldName;
        }

        public string FormFieldName { get; }
    }
}

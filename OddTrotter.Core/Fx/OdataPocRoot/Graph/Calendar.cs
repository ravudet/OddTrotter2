namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json.Serialization;

    public sealed class Calendar
    {
        public Calendar(OdataInstanceProperty<string> id, OdataCollectionProperty<Event> events, Foo foo)
        {
            this.Id = id;
            this.Events = events;
            this.Foo = foo;
        }

        [PropertyName("id")]
        public OdataInstanceProperty<string> Id { get; }

        public Foo Foo { get; }

        [PropertyName("events")]
        public OdataCollectionProperty<Event> Events { get; } //// TODO it'd be good if you didn't need to specify ienumerable
    }

    public sealed class Foo
    {
        public Foo(Bar bar)
        {
            this.Bar = bar;
        }

        public Bar Bar { get; }
    }

    public sealed class Bar
    {
        public Bar(string test)
        {
            Test = test;
        }

        public string Test { get; }
    }

    public sealed class OdataInstanceProperty<T>
    {
        public OdataInstanceProperty(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }

    public sealed class OdataCollectionProperty<T>
    {
        public OdataCollectionProperty(IEnumerable<T> value)
        {
            this.Value = value;
        }

        public IEnumerable<T> Value { get; }
    }
}

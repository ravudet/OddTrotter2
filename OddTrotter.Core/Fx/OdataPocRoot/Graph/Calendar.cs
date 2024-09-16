namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json.Serialization;

    public sealed class Calendar
    {
        /*public Calendar(string id, IEnumerable<Event> events)
            : this(new OdataProperty<string>(id), new OdataProperty<IEnumerable<Event>>(events), new Foo(new Bar("sasdf")))
        {
        }*/

        public Calendar(OdataProperty<string> id, OdataProperty<IEnumerable<Event>> events, Foo foo)
        {
            this.Id = id;
            this.Events = events;
            this.Foo = foo;
        }

        ////[JsonPropertyName("id")] //// TODO this should be an odata attribute, not a json one jsonSerializerOptions.PropertyNamingPolicy
        [PropertyName("id")]
        public OdataProperty<string> Id { get; }

        public Foo Foo { get; }

        ////[JsonPropertyName("events")] //// TODO this should be an odata attribute, not a json one jsonSerializerOptions.PropertyNamingPolicy/
        [PropertyName("events")]
        public OdataProperty<IEnumerable<Event>> Events { get; }
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

    public sealed class OdataProperty<T>
    {
        public OdataProperty(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }
    }
}

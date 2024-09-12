namespace Fx.OdataPocRoot.Graph
{
    using System.Collections.Generic;

    public sealed class Calendar
    {
        public Calendar(string id, IEnumerable<Event> events)
        {
            this.Id = id;
            this.Events = events;

            this.Foo = new Foo(new Bar("sasdf"));
        }
        
        public string Id { get; }

        public OdataProperty<string> OdataProp { get; } = new OdataProperty<string>();

        public Foo Foo { get; }

        public IEnumerable<Event> Events { get; }
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
    }
}

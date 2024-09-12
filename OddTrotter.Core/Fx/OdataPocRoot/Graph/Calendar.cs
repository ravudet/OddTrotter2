namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;
    using System.Collections.Generic;

    public sealed class Calendar
    {
        public Calendar(string id, IEnumerable<Event> events)
        {
            this.Id = new OdataProperty<string>(id);
            this.Events = new OdataProperty<IEnumerable<Event>>(events);

            this.Foo = new Foo(new Bar("sasdf"));
        }
        
        public OdataProperty<string> Id { get; }

        public Foo Foo { get; }

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

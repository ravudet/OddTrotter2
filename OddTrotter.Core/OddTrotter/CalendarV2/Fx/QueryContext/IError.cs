namespace Fx.QueryContext
{
    public interface IError<out TError>
    {
        TError Value { get; }
    }
}

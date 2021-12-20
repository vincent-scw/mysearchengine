namespace MySearchEngine.Core.Utilities
{
    public interface IIdGenerator<TId>
    {
        TId Next(string parameter);
    }
}

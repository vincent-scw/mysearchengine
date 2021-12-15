namespace MySearchEngine.Core
{
    public interface IIdGenerator<TId>
    {
        TId Next(string seed);
    }
}

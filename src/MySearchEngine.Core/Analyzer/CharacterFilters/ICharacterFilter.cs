namespace MySearchEngine.Core.Analyzer.CharacterFilters
{
    public interface ICharacterFilter
    {
        string Filter(string originText);
    }
}

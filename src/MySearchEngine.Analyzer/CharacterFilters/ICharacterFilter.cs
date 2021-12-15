namespace MySearchEngine.Analyzer.CharacterFilters
{
    public interface ICharacterFilter
    {
        public string Filter(string originText);
    }
}

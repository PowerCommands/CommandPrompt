using PainKiller.ReadLine.Contracts;

namespace PainKiller.ReadLine.Handlers
{
    public class AutoCompleteHandler(IEnumerable<string> suggestions, Func<string, string[]> suggestionProvider) : IAutoCompleteHandler
    {
        public char[] Separators { get; set; } = { ' ', '\\' };

        public string[] GetSuggestions(string input, int index)
        {
            var providerSuggestions = suggestionProvider.Invoke(input);
            return providerSuggestions;
        }
    }
}
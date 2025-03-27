using System.Text;

namespace PainKiller.CommandPrompt.CoreLib.Core.Presentation;

public static class InteractiveFilter<T>
{
    /// <summary>
    /// Runs an interactive filtering loop for a collection of items.
    /// </summary>
    /// <param name="items">The collection of items to filter.</param>
    /// <param name="filterFunc">
    /// A function that takes an item and a filter string, returning true if the item matches.
    /// </param>
    /// <param name="displayFunc">
    /// A delegate that is responsible for displaying a collection of items.
    /// </param>
    /// <param name="prompt">The prompt displayed to the user.</param>
    public static void Run(IEnumerable<T> items, Func<T, string, bool> filterFunc, Action<IEnumerable<T>> displayFunc, string prompt = "Enter filter term (Esc to exit, Enter for all): ")
    {
        var filterItems = items.ToArray();
        Console.Clear();
        displayFunc(filterItems);

        while (true)
        {
            string? filterTerm = ReadFilterCriteria(prompt);
            if (filterTerm == null)
                break;
            var filteredItems = string.IsNullOrEmpty(filterTerm) ? filterItems : filterItems.Where(item => filterFunc(item, filterTerm));
            Console.Clear();
            displayFunc(filteredItems);
        }
    }
    private static string? ReadFilterCriteria(string prompt)
    {
        Console.Write(prompt);
        var sb = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return null;
            }
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            sb.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }
        return sb.ToString();
    }
}

using Spectre.Console;

namespace PainKiller.CommandPrompt.CoreLib.Core.Presentation;
public static class DialogService
{
    /// <summary>
    /// Prompts the user with a Yes/No question.
    /// </summary>
    /// <param name="question">The question to ask the user.</param>
    /// <param name="choices">Optional list of choices for the user to select from (default is "Yes" and "No").</param>
    /// <returns>True if the user selects "Yes", otherwise False.</returns>
    public static bool YesNoDialog(string question, List<string>? choices = null)
    {
        // Default choices if none provided
        choices ??= ["Yes", "No"];

        // Create the prompt with the given choices
        var prompt = new SelectionPrompt<string>()
            .Title(question)
            .AddChoices(choices);

        var result = AnsiConsole.Prompt(prompt);
        return result == "Yes";
    }
    public static string GetSecret(string artifact = "password")
    {
        string password;
        while (true)
        {
            password = AnsiConsole.Prompt(
                new TextPrompt<string>($"Enter your {artifact}:")
                    .PromptStyle("yellow")
                    .Secret());

            var confirmPassword = AnsiConsole.Prompt(
                new TextPrompt<string>($"Confirm your {artifact}:")
                    .PromptStyle("yellow")
                    .Secret());

            if (password == confirmPassword)
            {
                AnsiConsole.MarkupLine($"[green]{artifact} confirmed![/]");
                break;
            }
            AnsiConsole.MarkupLine($"[red]{artifact} do not match. Please try again.[/]");
        }
        return password;
    }
}

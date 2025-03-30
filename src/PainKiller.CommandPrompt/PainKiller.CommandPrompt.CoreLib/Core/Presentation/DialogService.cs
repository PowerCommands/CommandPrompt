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
                    .PromptStyle("Magenta")
                    .Secret());

            var confirmPassword = AnsiConsole.Prompt(
                new TextPrompt<string>($"Confirm your {artifact}:")
                    .PromptStyle("Magenta")
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
    public static string QuestionAnswerDialog(string question)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(question)
                .PromptStyle("Magenta")
                .ValidationErrorMessage("[red]Invalid input[/]")
                .Validate(answer => 
                    !string.IsNullOrWhiteSpace(answer) ? ValidationResult.Success() : ValidationResult.Error("[red]Input cannot be empty[/]")));
    }
    public static string PathDialog(string prompt = "Enter output path:")
    {
        while (true)
        {
            AnsiConsole.MarkupLine($"[blue]Current working directory: [magenta]{Environment.CurrentDirectory}[/][/]");
        
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>(prompt)
                    .PromptStyle("Magenta")
                    .ValidationErrorMessage("[red]Invalid path[/]")
                    .Validate(input =>
                    {
                        try
                        {
                            var fullPath = Path.GetFullPath(input, Environment.CurrentDirectory);
                            return Directory.Exists(Path.GetDirectoryName(fullPath))
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Directory does not exist[/]");
                        }
                        catch
                        {
                            return ValidationResult.Error("[red]Invalid path format[/]");
                        }
                    }));

            // Om relativ sökväg, omvandla till absolut baserat på den aktuella arbetskatalogen
            var resolvedPath = Path.GetFullPath(input, Environment.CurrentDirectory);

            if (AnsiConsole.Confirm($"You entered: [magenta]{resolvedPath}[/]. Is this correct?"))
                return resolvedPath;
        }
    }
}

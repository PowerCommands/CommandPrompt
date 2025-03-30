using PainKiller.PromptKit.DomainObjects;

namespace PainKiller.PromptKit.Managers;
//public class ConfigurationTemplateManager(string rootDirectory, string outputProjectDirectory)
public class ConfigurationTemplateManager(TemplatePaths paths)
{
    public void CreateYamlConfigurationFile(string sourceFileName1, List<string> selectedModules)
    {
        var sourceFilePath = Path.Combine(paths.ApplicationRoot.Source, sourceFileName1);
        var lines = File.ReadAllLines(sourceFilePath).ToList();
        var outputLines = new List<string>();

        var i = 0;
        var inModulesSection = false;
        var modulesIndent = 0;

        while (i < lines.Count)
        {
            string line = lines[i];

            // Kolla om vi precis har nått "modules:"-nyckeln
            if (!inModulesSection && line.TrimStart().StartsWith("modules:"))
            {
                inModulesSection = true;
                modulesIndent = line.TakeWhile(Char.IsWhiteSpace).Count();
                outputLines.Add(line); // Lägg med själva nyckeln "modules:"
                i++;

                // Processa innehållet under modules:
                while (i < lines.Count)
                {
                    string currentLine = lines[i];
                    int currentIndent = currentLine.TakeWhile(Char.IsWhiteSpace).Count();

                    // Om vi når en rad med lägre eller lika indentering som "modules:" så är vi ute ur modules-blocket.
                    if (currentIndent <= modulesIndent)
                    {
                        inModulesSection = false;
                        break;
                    }

                    // Om raden innehåller ett modulnamn (t.ex. "security:"), utan att börja med "-".
                    string trimmed = currentLine.Trim();
                    if (!trimmed.StartsWith("-") && trimmed.Contains(":"))
                    {
                        int colonIndex = trimmed.IndexOf(':');
                        string moduleName = trimmed.Substring(0, colonIndex).Trim();

                        // Läs in hela modulblocket
                        int moduleIndent = currentIndent;
                        var moduleBlock = new List<string> { currentLine };
                        i++;
                        while (i < lines.Count)
                        {
                            string nextLine = lines[i];
                            int nextIndent = nextLine.TakeWhile(Char.IsWhiteSpace).Count();
                            if (nextIndent > moduleIndent)
                            {
                                moduleBlock.Add(nextLine);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        // Om modulen är vald, lägg med den
                        if (selectedModules.Any(m => m.Equals(moduleName, StringComparison.OrdinalIgnoreCase)))
                        {
                            outputLines.AddRange(moduleBlock);
                        }
                        // Annars så hoppar vi över detta block (dvs. vi lägger inte till det)
                    }
                    else
                    {
                        // Om raden inte matchar ett moduldefinition, bara gå vidare.
                        i++;
                    }
                }
                continue; // Fortsätt med nästa rad (som nu utanför modules-blocket)
            }

            // Utanför modules-sektionen – kopiera alla rader som de är.
            outputLines.Add(line);
            i++;
        }
        var outputFilePath = Path.Combine(paths.ApplicationRoot.Target, sourceFileName1);
        File.WriteAllLines(outputFilePath, outputLines);
    }


    /// <summary>
    /// Exempelmetod för att göra samma operation på den C#-baserade konfigurationen.
    /// Här kan du läsa in en klassfil eller ett objekt, filtrera bort ovällda moduler och sedan skriva ut resultatet.
    /// Implementation beror på hur du har tänkt hantera dina C#-konfigurationsobjekt.
    /// </summary>
    public void ProcessCsConfiguration(string sourceCsFilePath, string targetCsFilePath, List<string> selectedModules)
    {
        // Exempelsteg:
        // 1. Läs in C#-konfigurationen (detta kan vara via reflektion eller genom att skapa instansen direkt)
        // 2. Utför liknande filtrering: t.ex. sätt Modules.Security = null om "Security" inte finns i selectedModules.
        // 3. Skriv ut (eller kompilera om) konfigurationsklassen till målfilen.
        //
        // Eftersom hanteringen av C#-klassfilen kan bli mer komplicerad, lämnas denna metod som ett exempel för vidare utveckling.
    }
}

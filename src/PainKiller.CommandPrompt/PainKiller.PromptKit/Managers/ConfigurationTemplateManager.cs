using PainKiller.PromptKit.DomainObjects;
namespace PainKiller.PromptKit.Managers;
public class ConfigurationTemplateManager(TemplatePaths paths)
{
    public void CreateYamlConfigurationFile(List<string> selectedModules)
    {
        var lines = File.ReadAllLines(paths.ConfigurationYamlPath.Source).ToList();
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
        var outputFilePath = Path.Combine(paths.ApplicationRoot.Target, paths.ConfigurationYamlPath.Target);
        Directory.CreateDirectory(paths.ApplicationRoot.Target);
        File.WriteAllLines(outputFilePath, outputLines);
    }
    
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

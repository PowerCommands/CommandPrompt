using PainKiller.PromptKit.DomainObjects;
namespace PainKiller.PromptKit.Managers;
public class ConfigurationTemplateManager(TemplatePaths paths, string projectName)
{
    public void CreateYamlConfigurationFile(List<string> selectedModules)
    {
        var lines = File.ReadAllLines(paths.ConfigurationYamlPath.Source).ToList();
        var outputLines = new List<string>();
        var i = 0;
        var inModulesSection = false;
        while (i < lines.Count)
        {
            var line = lines[i];
            if (!inModulesSection && line.TrimStart().StartsWith("modules:"))
            {
                inModulesSection = true;
                var modulesIndent = line.TakeWhile(Char.IsWhiteSpace).Count();
                outputLines.Add(line); // Lägg med själva nyckeln "modules:"
                i++;

                // Processa innehållet under modules:
                while (i < lines.Count)
                {
                    var currentLine = lines[i];
                    var currentIndent = currentLine.TakeWhile(char.IsWhiteSpace).Count();
                    if (currentIndent <= modulesIndent)
                    {
                        inModulesSection = false;
                        break;
                    }
                    var trimmed = currentLine.Trim();
                    if (!trimmed.StartsWith($"-") && trimmed.Contains($":"))
                    {
                        var colonIndex = trimmed.IndexOf(':');
                        var moduleName = trimmed[..colonIndex].Trim();
                        var moduleBlock = new List<string> { currentLine };
                        i++;
                        while (i < lines.Count)
                        {
                            var nextLine = lines[i];
                            var nextIndent = nextLine.TakeWhile(char.IsWhiteSpace).Count();
                            if (nextIndent > currentIndent)
                            {
                                moduleBlock.Add(nextLine);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (selectedModules.Any(m => m.Replace("Module","").ToLower().Equals(moduleName, StringComparison.OrdinalIgnoreCase))) outputLines.AddRange(moduleBlock);
                    }
                    else
                    {
                        i++;
                    }
                }
                continue;
            }
            outputLines.Add(line);
            i++;
        }
        var yamlContent = string.Join(Environment.NewLine, outputLines);
        yamlContent = yamlContent.Replace("$APPLICATION_NAME$", projectName);

        var outputFilePath = Path.Combine(paths.ApplicationRoot.Target, paths.ConfigurationYamlPath.Target);
        Directory.CreateDirectory(paths.ApplicationRoot.Target);
        File.WriteAllText(outputFilePath, yamlContent);
    }
    
    public void ProcessCsConfiguration(string sourceCsFilePath, string targetCsFilePath, List<string> selectedModules)
    {
        var lines = File.ReadAllLines(sourceCsFilePath).ToList();
        var outputLines = new List<string>();

        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith("using "))
            {
                var usingLine = line.Trim();
                var moduleName = usingLine.Replace(";", "").Split('.').FirstOrDefault(part => selectedModules.Any(m => part.StartsWith(m.Replace("Module", ""), StringComparison.OrdinalIgnoreCase)));

                if (moduleName != null) outputLines.Add(line);
            }
            else if (line.Contains("get; set;"))
            {
                var trimmed = line.Trim();
                var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (words.Length < 4) continue;
                var propertyName = words[2].Replace("{", "").Trim();
                if (selectedModules.Any(m => m.Replace("Module", "").Equals(propertyName, StringComparison.OrdinalIgnoreCase)))
                {
                    outputLines.Add(line);
                }
            }
            else
            {
                outputLines.Add(line);
            }
        }
        File.WriteAllLines(targetCsFilePath, outputLines);
    }
}
using PainKiller.CommandPrompt.CoreLib.Core.Contracts;

namespace PainKiller.PromptKit.Managers;
public class DependencyManager(string projectFilePath, string sourceCoreRoot,IEnumerable<string> selectedModules, IEnumerable<string> allModules, IConsoleWriter writer)
{
    public void CleanDependencies()
    {
        var allDependencies = GetAllDependencies();
        var selectedDependencies = GetSelectedDependencies();

        var unusedDependencies = allDependencies.Except(selectedDependencies).ToList();

        if (!unusedDependencies.Any())
        {
            writer.WriteLine("All dependencies are covered by selected modules. No cleanup needed.");
            return;
        }

        writer.WriteLine($"Unused dependencies detected: {string.Join(", ", unusedDependencies)}");
        RemoveDependenciesFromCsproj(unusedDependencies);
    }

    private HashSet<string> GetAllDependencies()
    {
        var dependencies = new HashSet<string>();
        foreach (var module in allModules)
        {
            var moduleDependencies = GetModuleDependencies(module);
            foreach (var dep in moduleDependencies)
            {
                dependencies.Add(dep);
            }
        }
        return dependencies;
    }

    private HashSet<string> GetSelectedDependencies()
    {
        var dependencies = new HashSet<string>();
        foreach (var module in selectedModules)
        {
            var moduleDependencies = GetModuleDependencies(module);
            foreach (var dep in moduleDependencies)
            {
                dependencies.Add(dep);
            }
        }
        return dependencies;
    }

    private IEnumerable<string> GetModuleDependencies(string module)
    {
        var dependencies = new HashSet<string>();
        var readmePath = Path.Combine(sourceCoreRoot, "Modules", module, "readme.md");
        if (!File.Exists(readmePath)) return dependencies;

        var lines = File.ReadAllLines(readmePath);
        foreach (var line in lines)
        {
            if (line.StartsWith("DEPENDENCY:", StringComparison.OrdinalIgnoreCase))
            {
                var dependency = line.Substring("DEPENDENCY:".Length).Trim();
                dependencies.Add(dependency);
            }
        }
        return dependencies;
    }

    private void RemoveDependenciesFromCsproj(IEnumerable<string> unusedDependencies)
    {
        var lines = File.ReadAllLines(projectFilePath);
        var cleanedLines = new List<string>();

        foreach (var line in lines)
        {
            if (IsPackageReference(line, unusedDependencies))
            {
                var packageName = ExtractPackageName(line);
                writer.WriteLine($"Removing dependency: {packageName}");
                continue;
            }
            cleanedLines.Add(line);
        }

        File.WriteAllLines(projectFilePath, cleanedLines);
        writer.WriteLine("Dependency cleanup completed.");
    }

    private bool IsPackageReference(string line, IEnumerable<string> dependencies)
    {
        return line.Contains("<PackageReference Include=") && dependencies.Any(dep => line.Contains($"Include=\"{dep}\""));
    }

    private string ExtractPackageName(string line)
    {
        var startIndex = line.IndexOf("\"") + 1;
        var endIndex = line.IndexOf("\"", startIndex);
        return line[startIndex..endIndex];
    }
}

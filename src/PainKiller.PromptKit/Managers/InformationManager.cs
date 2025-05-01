using System.Xml.Linq;

namespace PainKiller.PromptKit.Managers;

public class InformationManager(string projectRoot)
{
    public int GetTotalNumberOfFiles()
    {
        return Directory.GetFiles(projectRoot, "*.*", SearchOption.AllDirectories).Count(f => !f.Contains("Modules"));
    }

    public int GetTotalNumberOfCodeLines() {
        int totalLines = 0;
        var codeFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories).Where(f => !f.Contains("Modules"));
        foreach (var file in codeFiles) {
            var lines = File
                .ReadLines(file)
                .Count(line => !string.IsNullOrWhiteSpace(line));
            totalLines += lines;
        }
        return totalLines;
    }

    public List<(string Name, string Version)> GetThirdPartyComponents()
    {
        var components = new List<(string Name, string Version)>();
        var csprojFiles = Directory.GetFiles(projectRoot, "*.csproj", SearchOption.AllDirectories);

        foreach (var csprojFile in csprojFiles)
        {
            var doc = XDocument.Load(csprojFile);
            var packageReferences = doc.Descendants("PackageReference");

            foreach (var reference in packageReferences)
            {
                var name = reference.Attribute("Include")?.Value;
                var version = reference.Attribute("Version")?.Value;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(version))
                {
                    components.Add((name, version));
                }
            }
        }
        return components;
    }
}
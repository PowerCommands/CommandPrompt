using System.Xml.Linq;
namespace PainKiller.PromptKit.Managers;
public static class CsprojCleaner
{
    public static void RemoveDeadReferencesAndRebuildProjectFile(string projectDirectory, string projectName = "")
    {
        var rootDir = new DirectoryInfo(projectDirectory);
        projectName = string.IsNullOrEmpty(projectName)
            ? Path.Combine(rootDir.FullName, $"{rootDir.Name}.csproj")
            : projectName;
        var projectFilePath = Path.Combine(projectDirectory, projectName);
        if (!File.Exists(projectFilePath))
            throw new FileNotFoundException($"The project file '{projectFilePath}' does not exist.");

        var document = XDocument.Load(projectFilePath);
        var ns = document.Root?.GetDefaultNamespace();

        var itemGroups = document.Descendants(ns + "ItemGroup").ToList();
        foreach (var itemGroup in itemGroups)
        {
            var compileItems = itemGroup.Elements(ns + "Compile").ToList();
            bool hasChanges = false;

            foreach (var compileItem in compileItems)
            {
                var includeAttribute = compileItem.Attribute("Include");
                if (includeAttribute is null)
                    continue;
                var includedPath = includeAttribute.Value.Trim().Replace('\\', Path.DirectorySeparatorChar);
                var fullIncludedPath = Path.Combine(projectDirectory, includedPath);
                if (!File.Exists(fullIncludedPath) || !Directory.Exists(Path.GetDirectoryName(fullIncludedPath)))
                {
                    compileItem.Remove();
                    hasChanges = true;
                }
            }
            if (hasChanges && !itemGroup.Elements().Any())
                itemGroup.Remove();
        }
        document.Save(projectFilePath);
    }
}
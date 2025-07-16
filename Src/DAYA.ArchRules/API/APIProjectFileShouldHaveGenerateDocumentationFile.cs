using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DAYA.ArchRules.API
{
    internal class APIProjectFileShouldHaveGenerateDocumentationFile : ArchRule
    {
        private DirectoryInfo FindSolutionDirectory(DirectoryInfo startDirectory)
        {
            var currentDirectory = startDirectory;
            while (currentDirectory != null)
            {
                if (Directory.GetFiles(currentDirectory.FullName, "*.sln").Any())
                {
                    return currentDirectory;
                }
                currentDirectory = currentDirectory.Parent;
            }
            return null;
        }

        private string FindProjectFile(DirectoryInfo solutionDirectory, string projectName)
        {
            // First try to find exact match for project name
            var projectFiles = Directory.GetFiles(solutionDirectory.FullName, $"{projectName}.csproj", SearchOption.AllDirectories);
            if (projectFiles.Any())
            {
                return projectFiles.First();
            }

            // If not found, look for API project files
            projectFiles = Directory.GetFiles(solutionDirectory.FullName, "*API.csproj", SearchOption.AllDirectories);
            if (projectFiles.Any())
            {
                return projectFiles.First();
            }

            return null;
        }

        internal override void Check()
        {
            if (Data.ApiAssembly == null)
            {
                throw new ArchitectureException("API assembly not found. Make sure Data.ApiAssembly is properly initialized.");
            }

            // Get the API assembly name without extension (typically matches the project name)
            var apiAssemblyName = Data.ApiAssembly.GetName().Name;

            // Find solution directory by traversing up from the current directory
            var currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var solutionDirectory = FindSolutionDirectory(currentDirectory);

            if (solutionDirectory == null)
            {
                throw new ArchitectureException("Could not find solution directory");
            }

            // Find the project file that corresponds to the API assembly
            var projectFile = FindProjectFile(solutionDirectory, apiAssemblyName);

            if (projectFile == null)
            {
                throw new ArchitectureException($"Could not find the API project file for {apiAssemblyName}");
            }

            using var reader = new StreamReader(projectFile);
            var serializer = new XmlSerializer(typeof(Project));
            var project = (Project)serializer.Deserialize(reader);
            var packages = project
                .PropertyGroup
                .Where(x => x.Condition == null)
                .Select(x => x.GenerateDocumentationFile);

            if (!packages.Any(x => x == true))
            {
                throw new ArchitectureException($"API Project should have GenerateDocumentationFile");
            }
        }
    }

    [XmlRoot(ElementName = "PropertyGroup")]
    public class PropertyGroup
    {
        [XmlElement(ElementName = "TargetFramework")]
        public string TargetFramework { get; set; }

        [XmlElement(ElementName = "GenerateDocumentationFile")]
        public bool? GenerateDocumentationFile { get; set; }

        [XmlElement(ElementName = "DocumentationFile")]
        public string DocumentationFile { get; set; }

        [XmlElement(ElementName = "NoWarn")]
        public string NoWarn { get; set; }

        [XmlAttribute(AttributeName = "Condition")]
        public string Condition { get; set; }
    }

    [XmlRoot(ElementName = "PackageReference")]
    public class PackageReference
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Include { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "ItemGroup")]
    public class ItemGroup
    {
        [XmlElement(ElementName = "PackageReference")]
        public List<PackageReference> PackageReference { get; set; }

        [XmlElement(ElementName = "ProjectReference")]
        public List<ProjectReference> ProjectReference { get; set; }
    }

    [XmlRoot(ElementName = "ProjectReference")]
    public class ProjectReference
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Include { get; set; }
    }

    [XmlRoot(ElementName = "Project")]
    public class Project
    {
        [XmlElement(ElementName = "PropertyGroup")]
        public List<PropertyGroup> PropertyGroup { get; set; }

        [XmlElement(ElementName = "ItemGroup")]
        public List<ItemGroup> ItemGroup { get; set; }

        [XmlAttribute(AttributeName = "Sdk")]
        public string Sdk { get; set; }
    }
}
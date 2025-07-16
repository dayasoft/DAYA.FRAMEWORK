using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetArchTest.Rules;

namespace Daya.CodeGenerator;

public class Generator
{
    public static string TestHelperPath { get; set; }
    public static Assembly ApplicationAssembly { get; set; }
    public static Assembly DomainAssembly { get; set; }
    public static Type StronglyTypedId { get; set; }
    public static Type AggregateType { get; set; }
    public static Type EntityType { get; set; }
    public static Type ValueObjectType { get; set; }
    private static readonly string Tab = new(' ', 4);

    public static async Task<IEnumerable<string>> GenerateApplicationBuildersAsync(params Type[] types)
    {
        DeleteFiles();
        var typePredicateQuery = Types.InAssembly(ApplicationAssembly).That();

        foreach (var t in types)
        {
            if (t == types.Last())
            {
                break;
            }

            if (t.IsInterface)
            {
                typePredicateQuery = typePredicateQuery.ImplementInterface(t).Or();
            }
            else
            {
                typePredicateQuery = typePredicateQuery.Inherit(t).Or();
            }
        }

        Type[] commandsQueriesTypes;

        if (types.Last().IsInterface)
        {
            commandsQueriesTypes = typePredicateQuery.ImplementInterface(types.Last()).GetTypes().ToArray();
        }
        else
        {
            commandsQueriesTypes = typePredicateQuery.Inherit(types.Last()).GetTypes().ToArray();
        }

        var updatedFiles = new List<string>();
        foreach (var requestType in commandsQueriesTypes)
        {
            await GenerateBuildersForProperties(updatedFiles, requestType);
            await CheckAndGenerateFileBuilder(updatedFiles, requestType);
        }
        return updatedFiles;
    }

    private static async Task GenerateBuildersForProperties(List<string> updatedFiles, Type type)
    {
        foreach (var propertyType in type.GetProperties().Select(x => x.PropertyType.GetUnderlyingType()))
        {
            if (!propertyType.GetUnderlyingType().IsPrimitive())
            {
                await GenerateBuildersForProperties(updatedFiles, propertyType);
            }
        }

        if (!type.IsPrimitive())
        {
            await CheckAndGenerateFileBuilder(updatedFiles, type);
        }
    }

    private static async Task CheckAndGenerateFileBuilder(List<string> updatedFiles, Type applicationRequest)
    {
        Console.WriteLine("Checking " + applicationRequest.Name + "...");
        var aggregate = applicationRequest.FullName[(applicationRequest.FullName.IndexOf("Application.") + "Application.".Length)..];
        if (aggregate.IndexOf('.') < 0)
        {
            aggregate = "SeedWork";
        }
        else
        {
            aggregate = aggregate.Substring(0, aggregate.IndexOf('.'));
        }
        var content = "";
        if (applicationRequest.IsAssignableTo(ValueObjectType))
        {
            content = string.Empty;
        }
        else if (applicationRequest.IsAssignableTo(StronglyTypedId))
        {
            content = string.Empty;
        }
        else
        {
            content = GenerateApplicationBuilderContent(applicationRequest, aggregate);
        }
        string applicationFile = GetApplicationBuilderFilePath(applicationRequest, aggregate);

        if (await ShouldOverwrite(applicationFile, content))
        {
            await WriteToFile(applicationFile, content);
            updatedFiles.Add(applicationFile);
        }
    }

    public static async Task<IEnumerable<string>> GenerateDomainBuildersAsync()
    {
        var types = Types.InAssembly(DomainAssembly)
                        .That()
                        .Inherit(EntityType)
                        .And()
                        .Inherit(AggregateType)
                        .GetTypes()
                        .ToArray();

        types = types.Union(Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(ValueObjectType)
            .GetTypes())
            .Distinct()
            .ToArray();
        var vos = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(StronglyTypedId)
            .GetTypes()
            .Distinct()
            .ToArray();

        var updatedFiles = new List<string>();
        foreach (var aggregate in types)
        {
            Console.WriteLine("Checking " + aggregate.Name + "...");
            var content = GenerateDomainBuilderContent(aggregate);
            string domainFile = GetDomainBuilderFilePath(aggregate);

            if (await ShouldOverwrite(domainFile, content))
            {
                await WriteToFile(domainFile, content);
                updatedFiles.Add(domainFile);
            }
        }
        foreach (var vo in vos)
        {
            Console.WriteLine("Checking " + vo.Name + "...");
            var content = GenerateVOContent(vo);
            string domainFile = GetDomainBuilderFilePath(vo);

            if (await ShouldOverwrite(domainFile, content))
            {
                await WriteToFile(domainFile, content);
                updatedFiles.Add(domainFile);
            }
        }
        return updatedFiles;
    }

    /// <summary>
    /// Returns the resolved path to the TestHelper project by searching for it in the solution
    /// </summary>
    /// <returns>The absolute path to the TestHelper project directory</returns>
    private static string GetTestHelperProjectPath()
    {
        try
        {
            // Start with the current execution directory
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;

            // Look for the solution directory by searching upwards
            var directory = new DirectoryInfo(currentDir);
            while (directory != null && !Directory.GetFiles(directory.FullName, "*.sln").Any())
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                // No solution found, try the current working directory
                Console.WriteLine("Warning: No solution file found in parent directories. Falling back to working directory.");
                directory = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (directory != null && !Directory.GetFiles(directory.FullName, "*.sln").Any())
                {
                    directory = directory.Parent;
                }
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException("Could not find solution directory");
            }

            // Search for the project directory
            var solutionDir = directory.FullName;

            // Look for a directory with the TestHelperPath name (with or without .csproj extension)
            var projectDirs = Directory.GetDirectories(solutionDir, TestHelperPath, SearchOption.AllDirectories);
            if (projectDirs.Length > 0)
            {
                return projectDirs[0];
            }

            // If no directory with exact name, try to find by csproj name
            var projectFiles = Directory.GetFiles(solutionDir, $"{TestHelperPath}.csproj", SearchOption.AllDirectories);
            if (projectFiles.Length > 0)
            {
                var projectDir = Path.GetDirectoryName(projectFiles[0]);
                return projectDir;
            }

            // If still not found, create the directory in the solution root
            Console.WriteLine($"Project '{TestHelperPath}' not found. Creating directory in solution root.");
            var newProjectDir = Path.Combine(solutionDir, TestHelperPath);
            Directory.CreateDirectory(newProjectDir);
            return newProjectDir;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving TestHelper path: {ex.Message}");
            throw;
        }
    }

    private static void DeleteFiles()
    {
        var testHelperPath = GetTestHelperProjectPath();
        Console.WriteLine($"Looking for files in: {testHelperPath}");

        var files = Directory.GetFiles(testHelperPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (IsLocked(file))
            {
                Console.WriteLine($"{Path.GetFileName(file)} is locked, skipping...");
                continue;
            }
            Console.WriteLine($"Deleting {Path.GetFileName(file)}...");
            File.Delete(file);
        }
    }

    private static bool IsLocked(string file)
    {
        using var reader = new StreamReader(file);
        var all = reader.ReadToEnd();
        return all.Contains("[Lock]") ||
            all.Contains("[LockAttribute]") ||
            all.Contains("[Daya.CodeGenerator.Lock]") ||
            all.Contains("[Daya.CodeGenerator.LockAttribute]");
    }

    private static string GetApplicationBuilderFilePath(Type request, string aggregate)
    {
        var testHelperPath = GetTestHelperProjectPath();
        var applicationDirectory = Path.Combine(testHelperPath, "Application");
        Directory.CreateDirectory(applicationDirectory);
        applicationDirectory = Path.Combine(applicationDirectory, aggregate);
        Directory.CreateDirectory(applicationDirectory);
        var applicationBuilderPath = Path.Combine(applicationDirectory, $"{request.Name}Builder.cs");
        return applicationBuilderPath;
    }

    private static string GetDomainBuilderFilePath(Type aggregate)
    {
        var testHelperPath = GetTestHelperProjectPath();
        var domainDirectory = Path.Combine(testHelperPath, "Domain");
        Directory.CreateDirectory(domainDirectory);
        var domainFile = Path.Combine(domainDirectory, $"{aggregate.Name}Builder.cs");
        return domainFile;
    }

    private static string GenerateVOContent(Type request)
    {
        var builder = new StringBuilder();
        ConstructorInfo constructor = request.GetConstructors().Single();
        var parameters = constructor.GetParameters();

        var paramNames = GetFieldNameTypes(parameters);

        var namespaces = paramNames
            .SelectMany(x => x.Namespaces)
            .Union(new List<string>() { request.Namespace, $"{TestHelperPath}.Domain" })
            .Distinct()
            .OrderBy(x => x)
            .ToList();
        foreach (var @namespace in namespaces)
        {
            builder.AppendLine($"using {@namespace};");
        }
        builder.AppendLine();

        int tab = 0;
        builder.AppendLine($"namespace {TestHelperPath}.Domain");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        builder.AppendLine($"{Tabs(tab)}public class {request.Name}Builder");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;

        foreach (var field in paramNames)
        {
            builder.AppendLine($"{Tabs(tab)}{field.GetDefinition()}");
            builder.AppendLine($"{Tabs(tab)}private bool _{field.Name}IsSet = false;");
        }

        builder.AppendLine();
        builder.AppendLine($"{Tabs(tab)}public {request.Name} Build()");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}return new {request.Name}(");

        var last = paramNames.LastOrDefault();
        tab++;
        foreach (var x in paramNames)
        {
            if (x != last)
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")},");
            }
            else
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")});");
            }
        }
        if (last == null)
        {
            builder.AppendLine($"{Tabs(tab)});");
        }
        tab--;
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        builder.AppendLine();

        foreach (var parameter in paramNames)
        {
            if (parameter.IsArray)
            {
                builder.Append($"{Tabs(tab)}public {request.Name}Builder AddTo{parameter.PascalName()}");
                builder.AppendLine($"({parameter.GetUnderLyingTypeName()} {parameter.Name})");
                builder.AppendLine(Tabs(tab) + "{");
                tab++;
                builder.AppendLine($"{Tabs(tab)}_{parameter.Name}.Add({parameter.Name});");
                builder.AppendLine($"{Tabs(tab)}return this;");
                tab--;
                builder.AppendLine(Tabs(tab) + "}");
                builder.AppendLine();
            }
            builder.Append($"{Tabs(tab)}public {request.Name}Builder Set{parameter.PascalName()}");
            builder.AppendLine($"({parameter.GetTypeName()} {parameter.Name})");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine($"{Tabs(tab)}if (_{parameter.Name}IsSet)");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine(@$"{Tabs(tab)}throw new System.InvalidOperationException(nameof(_{parameter.Name}) + "" already initialized"");");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name}IsSet = true;");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name} = {parameter.Name};");
            builder.AppendLine($"{Tabs(tab)}return this;");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            if (parameter != paramNames.Last())
            {
                builder.AppendLine();
            }
        }
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        tab--;
        builder.AppendLine(Tabs(tab) + "}");

        return builder.ToString();
    }

    private static string GenerateApplicationBuilderContent(Type request, string aggregate)
    {
        var builder = new StringBuilder();
        ConstructorInfo constructor = request.GetConstructors().SingleOrDefault();
        if (constructor == null)
        {
            return string.Empty;
        }
        var parameters = constructor.GetParameters();

        var paramNames = GetFieldNameTypes(parameters);

        var namespaces = paramNames
            .SelectMany(x => x.Namespaces)
            .Union(new List<string>() { request.Namespace, $"{TestHelperPath}.Domain" })
            .Distinct()
            .OrderBy(x => x)
            .ToList();
        foreach (var @namespace in namespaces)
        {
            builder.AppendLine($"using {@namespace};");
        }
        builder.AppendLine();

        int tab = 0;
        builder.AppendLine($"namespace {TestHelperPath}.Application.{aggregate}");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        builder.AppendLine($"{Tabs(tab)}public class {request.Name}Builder");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;

        foreach (var field in paramNames)
        {
            builder.AppendLine($"{Tabs(tab)}{field.GetDefinition()}");
            builder.AppendLine($"{Tabs(tab)}private bool _{field.Name}IsSet = false;");
        }

        builder.AppendLine();
        builder.AppendLine($"{Tabs(tab)}public {request.Name} Build()");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}return new {request.Name}(");

        var last = paramNames.LastOrDefault();
        tab++;
        foreach (var x in paramNames)
        {
            if (x != last)
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")},");
            }
            else
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")});");
            }
        }
        if (last == null)
        {
            builder.AppendLine($"{Tabs(tab)});");
        }
        tab--;
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        builder.AppendLine();

        foreach (var parameter in paramNames)
        {
            if (parameter.IsArray)
            {
                builder.Append($"{Tabs(tab)}public {request.Name}Builder AddTo{parameter.PascalName()}");
                builder.AppendLine($"({parameter.GetUnderLyingTypeName()} {parameter.Name})");
                builder.AppendLine(Tabs(tab) + "{");
                tab++;
                builder.AppendLine($"{Tabs(tab)}_{parameter.Name}.Add({parameter.Name});");
                builder.AppendLine($"{Tabs(tab)}return this;");
                tab--;
                builder.AppendLine(Tabs(tab) + "}");
                builder.AppendLine();
            }
            builder.Append($"{Tabs(tab)}public {request.Name}Builder Set{parameter.PascalName()}");
            builder.AppendLine($"({parameter.GetTypeName()} {parameter.Name})");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine($"{Tabs(tab)}if (_{parameter.Name}IsSet)");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine(@$"{Tabs(tab)}throw new System.InvalidOperationException(nameof(_{parameter.Name}) + "" already initialized"");");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name}IsSet = true;");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name} = {parameter.Name};");
            builder.AppendLine($"{Tabs(tab)}return this;");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            if (parameter != paramNames.Last())
            {
                builder.AppendLine();
            }
        }
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        tab--;
        builder.AppendLine(Tabs(tab) + "}");

        return builder.ToString();
    }

    private static string GenerateDomainBuilderContent(Type aggregate)
    {
        var builder = new StringBuilder();
        MethodInfo factoryMethod;
        try
        {
            factoryMethod = aggregate.GetMethods(BindingFlags.Public | BindingFlags.Static).Single();
        }
        catch (InvalidOperationException)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{aggregate.Name} does not have a static creator method");
            Console.ForegroundColor = color;
            throw;
        }
        var parameters = factoryMethod.GetParameters();

        var isAsync = factoryMethod.CustomAttributes.Any(x => x.AttributeType == typeof(AsyncStateMachineAttribute));

        var paramNames = GetFieldNameTypes(parameters);
        var namespaces = paramNames.SelectMany(x => x.Namespaces).Distinct().OrderBy(x => x).ToList();
        namespaces.Add(aggregate.Namespace);
        if (isAsync)
        {
            namespaces.Add("System.Threading.Tasks");
        }
        namespaces = namespaces.Distinct().OrderBy(x => x).ToList();

        foreach (var @namespace in namespaces)
        {
            builder.AppendLine($"using {@namespace};");
        }
        builder.AppendLine();

        int tab = 0;
        builder.AppendLine($"namespace {TestHelperPath}.Domain");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        builder.AppendLine($"{Tabs(tab)}public class {aggregate.Name}Builder");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;

        foreach (var field in paramNames)
        {
            builder.AppendLine($"{Tabs(tab)}{field.GetDefinition()}");
            builder.AppendLine($"{Tabs(tab)}private bool _{field.Name}IsSet = false;");
        }

        builder.AppendLine();
        builder.AppendLine($"{Tabs(tab)}public {(isAsync ? "Task<" + aggregate.Name + ">" : aggregate.Name)} Build{(isAsync ? "Async" : "")}()");
        builder.AppendLine(Tabs(tab) + "{");
        tab++;
        builder.AppendLine($"{Tabs(tab)}return {aggregate.Name}.{factoryMethod.Name}(");

        var last = paramNames.LastOrDefault();
        tab++;
        foreach (var x in paramNames)
        {
            if (x != last)
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")},");
            }
            else
            {
                builder.AppendLine($"{Tabs(tab)}" +
                    $"{x.GetParameterDefinition()}" +
                    $"{(x.IsArray ? ".ToArray()" : "")});");
            }
        }
        if (last == null)
        {
            builder.AppendLine($"{Tabs(tab)});");
        }
        tab--;
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        builder.AppendLine();

        foreach (var parameter in paramNames)
        {
            if (parameter.IsArray)
            {
                builder.Append($"{Tabs(tab)}public {aggregate.Name}Builder AddTo{parameter.PascalName()}");
                builder.AppendLine($"({parameter.GetUnderLyingTypeName()} {parameter.Name})");
                builder.AppendLine(Tabs(tab) + "{");
                tab++;
                builder.AppendLine($"{Tabs(tab)}_{parameter.Name}.Add({parameter.Name});");
                builder.AppendLine($"{Tabs(tab)}return this;");
                tab--;
                builder.AppendLine(Tabs(tab) + "}");
                builder.AppendLine();
            }
            builder.Append($"{Tabs(tab)}public {aggregate.Name}Builder Set{parameter.PascalName()}");
            builder.AppendLine($"({parameter.GetTypeName()} {parameter.Name})");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine($"{Tabs(tab)}if (_{parameter.Name}IsSet)");
            builder.AppendLine(Tabs(tab) + "{");
            tab++;
            builder.AppendLine(@$"{Tabs(tab)}throw new System.InvalidOperationException(nameof(_{parameter.Name}) + "" already initialized"");");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name}IsSet = true;");
            builder.AppendLine($"{Tabs(tab)}_{parameter.Name} = {parameter.Name};");
            builder.AppendLine($"{Tabs(tab)}return this;");
            tab--;
            builder.AppendLine(Tabs(tab) + "}");
            if (parameter != paramNames.Last())
            {
                builder.AppendLine();
            }
        }
        tab--;
        builder.AppendLine(Tabs(tab) + "}");
        tab--;
        builder.AppendLine(Tabs(tab) + "}");

        return builder.ToString();
    }

    private static async Task<bool> ShouldOverwrite(string path, string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
        {
            return await Task.FromResult(false);
        }
        if (!File.Exists(path))
        {
            return await Task.FromResult(true);
        }
        if (IsLocked(path))
        {
            return await Task.FromResult(false);
        }
        string oldContent;
        using var reader = new StreamReader(path);
        oldContent = await reader.ReadToEndAsync();

        return newContent != oldContent;
    }

    private static async Task WriteToFile(string path, string newContent)
    {
        using var reader = new StreamWriter(path);
        await reader.WriteAsync(newContent);
    }

    private static string Tabs(int tab)
    {
        return string.Concat(Enumerable.Repeat(Tab, tab));
    }

    /// <summary>
    /// </summary>
    /// <param name="parameters">A Map of (FieldName, FieldType)</param>
    /// <returns></returns>

    internal static IEnumerable<FieldInfo> GetFieldNameTypes(ParameterInfo[] parameters)
    {
        var fields = new List<FieldInfo>();
        foreach (var parameter in parameters)
        {
            var paramType = parameter.ParameterType;
            string fieldName = parameter.Name;
            string fieldType;
            bool isTypedId = false;
            bool isArray = false;
            bool isList = false;
            bool isNullable = false;
            string typedIdName = "";
            var namespaces = new List<string>() { paramType.Namespace };
            if (paramType.IsInterface)
            {
                fieldType = paramType.Name;
                namespaces.Add(paramType.Namespace);
            }
            else if (paramType.BaseType.IsGenericType)
            {
                isTypedId = paramType.BaseType.IsAssignableTo(StronglyTypedId);
                if (isTypedId)
                {
                    typedIdName = paramType.Name;
                }

                var type = paramType.BaseType.GetGenericArguments().First();
                fieldType = type.Name;
                namespaces.Add(type.Namespace);
            }
            else if (paramType.IsGenericType && typeof(ICollection).IsAssignableFrom(paramType.GetGenericTypeDefinition()))
            {
                var type = paramType.GetUnderlyingType();
                fieldType = type.Name;
                namespaces.Add(type.Namespace);
                namespaces.Add("System.Collections.Generic");
                isList = true;
            }
            else if (paramType.IsGenericType)
            {
                isNullable = paramType.GetGenericTypeDefinition() == typeof(Nullable<>);
                var type = paramType.GetGenericArguments().First();
                fieldType = type.Name;
                namespaces.Add(type.Namespace);
            }
            else if (paramType.IsArray)
            {
                var type = paramType.GetUnderlyingType();
                fieldType = type.Name;
                namespaces.Add(type.Namespace);
                namespaces.Add("System.Collections.Generic");
                isArray = true;
            }
            else
            {
                fieldType = paramType.Name;
                namespaces.Add(paramType.Namespace);
            }
            isNullable = NullableHelper.IsNullable(parameter);

            fields.Add(new FieldInfo()
            {
                IsTypedId = isTypedId,
                IsArray = isArray,
                IsList = isList,
                IsNullable = isNullable,
                Name = fieldName,
                Type = fieldType,
                TypedIdName = typedIdName,
                Namespaces = namespaces,
            });
        }

        return fields;
    }
}
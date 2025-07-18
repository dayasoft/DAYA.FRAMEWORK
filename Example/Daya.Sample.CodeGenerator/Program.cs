using System.Reflection;
using Daya.CodeGenerator;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.CodeGenerator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var defaultColor = Setup();

            var applicationBuilders = await Generator.GenerateApplicationBuildersAsync(
                typeof(Query<>),
                typeof(IQuery<>),
                typeof(Command<>),
                typeof(Command),
                typeof(DirectCommand),
                typeof(DirectCommand<>)).ConfigureAwait(false);
            var domainBuilders = await Generator.GenerateDomainBuildersAsync().ConfigureAwait(false);

            Report(defaultColor, applicationBuilders, domainBuilders);
        }

        private static void Report(
            ConsoleColor defaultColor,
            IEnumerable<string> applicationBuilders,
            IEnumerable<string> domainBuilders)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("The following files has been updated:");
            Console.WriteLine(string.Join(Environment.NewLine, domainBuilders
                .Union(applicationBuilders)
                .Select(x => Path.GetFileName(x))));
            Console.ForegroundColor = defaultColor;
        }

        private static ConsoleColor Setup()
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;

            Generator.AggregateType = typeof(CosmosEntity);
            Generator.ApplicationAssembly = Assembly.Load("Daya.Sample.Application");
            Generator.DomainAssembly = Assembly.Load("Daya.Sample.Domain");
            Generator.EntityType = typeof(Entity);
            Generator.ValueObjectType = typeof(ValueObject);
            Generator.StronglyTypedId = typeof(TypedId);
            Generator.TestHelperPath = "Daya.Sample.TestHelpers";
            return defaultColor;
        }
    }
}
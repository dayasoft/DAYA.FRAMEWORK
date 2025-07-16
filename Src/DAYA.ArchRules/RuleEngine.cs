using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAYA.ArchRules
{
    public class RuleEngine
    {
        public static List<(string ClassType, string RuleType)> IgnoredRules = new List<(string ClassType, string RuleType)>();

        public RuleEngine(
            Assembly domainAssembly,
            Assembly applicationAssembly,
            Assembly infrastructureAssembly,
            Assembly apiAssembly,
            Assembly integrationEventsAssembly)
        {
            Data.DomainAssembly = domainAssembly;
            Data.ApplicationAssembly = applicationAssembly;
            Data.InfrastructureAssembly = infrastructureAssembly;
            Data.ApiAssembly = apiAssembly;
            Data.IntegrationEventsAssembly = integrationEventsAssembly;
        }

        public void Check()
        {
            var rules = GetRules();
            var errorsBuilder = new StringBuilder();
            var failedRules = CheckAllRules(rules, errorsBuilder);

            if (failedRules > 0)
            {
                throw new Exception($"{Environment.NewLine}FailedRules: {failedRules}{Environment.NewLine}{Environment.NewLine}" + errorsBuilder.ToString());
            }
        }

        private static int CheckAllRules(IEnumerable<ArchRule> rules, StringBuilder builder)
        {
            return rules
                .Where(rule => RuleIsNotMeet(builder, rule))
                .Count();
        }

        private static bool RuleIsNotMeet(StringBuilder stringBuilder, ArchRule rule)
        {
            try
            {
                rule.Check();
            }
            catch (ArchitectureException ex)
            {
                string errorMessage = string.IsNullOrWhiteSpace(rule.Message) ? rule.GetType().Name : rule.Message;
                stringBuilder.AppendLine(errorMessage + ": " + Environment.NewLine + ex.Message + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine("Unhandled Exception in " + rule.GetType().Name + " Rule: " + Environment.NewLine + ex.Message + Environment.NewLine);
                return true;
            }

            return false;
        }

        private static IEnumerable<ArchRule> GetRules()
        {
            return
                   from assemblyType in Assembly.GetExecutingAssembly().GetTypes()
                   where typeof(ArchRule).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                   select (ArchRule)Activator.CreateInstance(assemblyType);
        }
    }
}

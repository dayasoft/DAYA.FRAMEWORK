using DAYA.ArchRules.Utilities.Rules;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.ArchRules.Utilities
{
    static internal class ConditionsExtender
    {
        public static ConditionList BeInitOnly(this Conditions condition)
        {
            var initOnlyRule = new InitOnlyRule();
            return condition.MeetCustomRule(initOnlyRule);
        }

        public static ConditionList HaveAPublicConstructor(this Conditions condition)
        {
            var rule = new HaveAPublicConstructorRule();
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveOnlyOneConstructor(this Conditions condition)
        {
            var rule = new HaveOneConstructorConstructorRule();
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveUserDefinedMethod(this Conditions condition)
        {
            var rule = new HaveUserDefinedMethodRule();
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HavePropertyWithName(this Conditions condition, string name)
        {
            var rule = new HavePropertyWithNameRule(name);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList UsedAsBaseClass(this Conditions condition, IEnumerable<Type> types)
        {
            var rule = new UsedAsBaseClassRule(types);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveOnlyPrimitiveProperties(this Conditions condition)
        {
            var rule = new HaveOnlyPrimitivePropertiesRule();
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveOnlyPublicProperties(this Conditions condition)
        {
            var rule = new HaveOnlyPublicPropertiesRule();
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveOnlySimpleOrDefinedProperties(this Conditions condition, HashSet<Type> definedTypes)
        {
            var rule = new HaveOnlySimpleOrDefinedPropertiesRule(definedTypes);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveAConstructorMatchesWithFieldsAndPropsNames(this Conditions condition, BindingFlags bindingFlags = BindingFlags.Default)
        {
            var rule = new HaveAConstructorMatchesWithFieldsAndPropsNamesRule(bindingFlags);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveComplexPropertiesAndFieldsThatInheritDefiendTypes(
            this Conditions condition,
            List<Type> definedTypes,
            BindingFlags bindingFlags)
        {
            var rule = new HaveComplexPropertiesAndFieldsThatInheritDefiendTypesRule(definedTypes, bindingFlags);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveListProperty(
            this Conditions condition,
            BindingFlags bindingFlags)
        {
            var rule = new HaveListProperty(bindingFlags);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HavePropertyMoreThan(
            this Conditions condition,
            BindingFlags bindingFlags,
            int max)
        {
            var rule = new HavePropertyMoreThan(bindingFlags, max);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HavePropertyWithName(
            this Conditions condition,
            BindingFlags bindingFlags,
            string name)
        {
            var rule = new HavePropertyWithName(bindingFlags, name);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HavePropertyMatchCondition(
            this Conditions condition,
            BindingFlags bindingFlags,
            Func<PropertyInfo, bool> conditionFunc)
        {
            var rule = new HavePropertyWithCondition(bindingFlags, conditionFunc);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveDuplicateFieldType(
            this Conditions condition,
            BindingFlags bindingFlags)
        {
            var rule = new HaveDuplicateFieldType(bindingFlags);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveField(
            this Conditions condition,
            BindingFlags bindingFlags)
        {
            var rule = new HaveField(bindingFlags);
            return condition.MeetCustomRule(rule);
        }

        public static ConditionList HaveOnlyReadOnlyField(
            this Conditions condition,
            BindingFlags bindingFlags)
        {
            var rule = new HaveOnlyReadOnlyFieldRule(bindingFlags);
            return condition.MeetCustomRule(rule);
        }
    }
}
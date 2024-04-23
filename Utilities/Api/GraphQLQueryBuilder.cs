using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class GraphQLQueryBuilder
    {
        private StringBuilder queryBuilder;
        private Dictionary<string, object> variables;
        private string operationType;
        private Dictionary<string, object> arguments;

        public GraphQLQueryBuilder(string operationType = "query")
        {
            queryBuilder = new StringBuilder();
            variables = new Dictionary<string, object>();
            arguments = new Dictionary<string, object>();
            this.operationType = operationType;
        }

        public GraphQLQueryBuilder SetOperationType(string operationType)
        {
            this.operationType = operationType;
            return this;
        }

        public GraphQLQueryBuilder AddVariable(string variableName, object variableValue)
        {
            variables[variableName] = variableValue;
            return this;
        }

        public GraphQLQueryBuilder AddArgumentVariable(string argumentName, object argumentValue)
        {
            if (arguments == null)
            {
                arguments = new Dictionary<string, object>();
            }

            arguments[argumentName] = argumentValue;
            return this;
        }

        public GraphQLQueryBuilder AddField(string fieldName, string alias = null)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                queryBuilder.Append($"{fieldName} : {alias} ");
            }
            else
            {
                queryBuilder.Append($"{fieldName} ");
            }

            return this;
        }

        public GraphQLQueryBuilder AddArguments(Dictionary<string, object> arguments)
        {
            if (arguments != null && arguments.Count > 0)
            {
                queryBuilder.Append("(");

                foreach (var arg in arguments)
                {
                    if (arg.Value is IEnumerable<object> enumerable && enumerable.Any())
                    {
                        var formattedValues = string.Join(", ", enumerable.Select(value => $"{value}"));
                        queryBuilder.Append($"{arg.Key}: [{formattedValues}], ");
                    }
                    else
                    {
                        queryBuilder.Append($"{arg.Key}: {FormatArgumentValue(arg.Value)}, ");
                    }
                }

                queryBuilder.Remove(queryBuilder.Length - 2, 2); //Remove trailing comma and space
                queryBuilder.Append(") ");
            }

            return this;
        }

        public GraphQLQueryBuilder AddNestedField(string fieldName, Action<GraphQLQueryBuilder> nestedQueryAction, string alias = null)
        {
            return AddNestedField(fieldName, null, nestedQueryAction, alias);
        }

        public GraphQLQueryBuilder AddNestedField(string fieldName, Dictionary<string, object> arguments, Action<GraphQLQueryBuilder> nestedQueryAction, string alias = null)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                queryBuilder.Append($"{fieldName} : {alias} ");
            }
            else
            {
                queryBuilder.Append($"{fieldName} ");
            }

            if (arguments != null && arguments.Count > 0)
            {
                var argumentsString = BuildArgumentsString(arguments);
                queryBuilder.Append($"{argumentsString} ");
            }

            queryBuilder.Append("{ ");
            nestedQueryAction.Invoke(this);
            queryBuilder.Append("} ");

            return this;
        }

        private string BuildArgumentsString(Dictionary<string, object> arguments, bool isNested = false)
        {
            var argumentsBuilder = new StringBuilder();
            if (!isNested)
            {
                argumentsBuilder.Append("(");
            }

            foreach (var arg in arguments)
            {
                argumentsBuilder.Append(BuildArgumentString(arg.Key, arg.Value));
            }

            if (argumentsBuilder.Length > 2 && argumentsBuilder[argumentsBuilder.Length - 1] == ' ' && argumentsBuilder[argumentsBuilder.Length - 2] == ',')
            {
                argumentsBuilder.Remove(argumentsBuilder.Length - 2, 2); // Remove trailing comma and space
            }

            if (!isNested)
            {
                argumentsBuilder.Append(") ");
            }

            return argumentsBuilder.ToString();
        }

        private string BuildArgumentString(string key, object value)
        {
            if (value is IEnumerable<object> enumerable && enumerable.Any())
            {
                return BuildEnumerableArgumentString(key, enumerable);
            }
            else if (value is Dictionary<string, object> dict && dict.Any())
            {
                return $"{key} : {{{BuildArgumentsString(dict, true)}}}";
            }
            else
            {
                return $"{key}: {FormatArgumentValue(value)}, ";
            }
        }

        private string BuildEnumerableArgumentString(string key, IEnumerable<object> enumerable)
        {
            var formattedValues = string.Join(", ", enumerable.Select(value =>
            {
                if (value is Dictionary<string, object>)
                {
                    return $"{{{BuildArgumentsString((Dictionary<string, object>)value, true)}}}";
                }
                else
                {
                    return $"{value}";
                }
            }));

            return $"{key}: [{formattedValues}], ";
        }

        public string Build()
        {
            var variablesString = BuildVariablesString();

            if (!string.IsNullOrEmpty(variablesString))
            {
                return $"{operationType} {variablesString} {{{queryBuilder}}}";
            }

            return $"{operationType} {{{queryBuilder}}}";
        }

        private string BuildVariablesString()
        {
            if (variables.Count == 0)
            {
                return "";
            }

            var variablesBuilder = new StringBuilder();
            variablesBuilder.Append("(");

            foreach (var variable in variables)
            {
                if (variable.Value is IEnumerable<object> enumerable && enumerable.Any())
                {
                    var formattedValues = string.Join(", ", enumerable.Select(value => $"{value}"));
                    variablesBuilder.Append($"[{formattedValues}]");
                }
                else
                {
                    variablesBuilder.Append($"{variable.Key}: {FormatArgumentValue(variable.Value)}, ");
                }
            }

            variablesBuilder.Remove(variablesBuilder.Length - 2, 2); // Remove trailing comma and space
            variablesBuilder.Append(") ");

            return variablesBuilder.ToString();
        }


        private string FormatArgumentValue(object value)
        {
            return value.ToString();
        }

    }
}

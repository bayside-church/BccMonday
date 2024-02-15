using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class GraphQLQueryBuilder
    {
        private StringBuilder queryBuilder;
        private Dictionary<string, object> variables;
        private string operationType;

        public GraphQLQueryBuilder(string operationType = "query")
        {
            queryBuilder = new StringBuilder();
            variables = new Dictionary<string, object>();
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

        public GraphQLQueryBuilder AddNestedField(string fieldName, Action<GraphQLQueryBuilder> nestedQuery, Dictionary<string, object> arguments = null, string alias = null)
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
            nestedQuery.Invoke(this);
            queryBuilder.Append("} ");

            return this;
        }

        private string BuildArgumentsString(Dictionary<string, object> arguments)
        {
            var argumentsBuilder = new StringBuilder("(");

            foreach (var arg in arguments)
            {
                if (arg.Value is IEnumerable<object> enumerable && enumerable.Any())
                {
                    var formattedValues = string.Join(", ", enumerable.Select(value => $"{value}"));
                    argumentsBuilder.Append($"{arg.Key}: [{formattedValues}], ");
                }
                else
                {
                    argumentsBuilder.Append($"{arg.Key}: {FormatArgumentValue(arg.Value)}, ");
                }
            }

            argumentsBuilder.Remove(argumentsBuilder.Length - 2, 2); // Remove trailing comma and space
            argumentsBuilder.Append(") ");

            return argumentsBuilder.ToString();
        }

        public string Build()
        {
            var variablesString = BuildVariablesString();

            if (!string.IsNullOrEmpty(variablesString))
            {
                return $"{operationType} {variablesString} {{{queryBuilder.ToString()}}}";
            }

            return $"{operationType} {{{queryBuilder.ToString()}}}";
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

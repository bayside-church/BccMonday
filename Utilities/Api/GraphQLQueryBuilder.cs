using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.baysideonline.BccMonday.Utilities.Api
{
    public class GraphQLQueryBuilder
    {
        private readonly StringBuilder _queryBuilder;
        private readonly Dictionary<string, object> _variables;
        private readonly string _operationType;
        private readonly Dictionary<string, object> _arguments;

        public GraphQLQueryBuilder(string operationType = "query")
        {
            _queryBuilder = new StringBuilder();
            _variables = new Dictionary<string, object>();
            _arguments = new Dictionary<string, object>();
            _operationType = operationType;
        }

        public GraphQLQueryBuilder AddVariable(string variableName, object variableValue)
        {
            _variables[variableName] = variableValue;
            return this;
        }

        public GraphQLQueryBuilder AddArgumentVariable(string argumentName, object argumentValue)
        {
            if (!_arguments.ContainsKey(argumentName))
            {
                _arguments[argumentName] = argumentValue;
            }
            return this;
        }

        public GraphQLQueryBuilder AddField(string fieldName, string alias = null)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                _queryBuilder.Append($"{fieldName} : {alias} ");
            }
            else
            {
                _queryBuilder.Append($"{fieldName} ");
            }

            return this;
        }

        public GraphQLQueryBuilder AddArguments(Dictionary<string, object> arguments)
        {
            if (arguments != null && arguments.Count > 0)
            {
                _queryBuilder.Append("(");

                foreach (var arg in arguments)
                {
                    _queryBuilder.Append(BuildArgumentString(arg.Key, arg.Value));
                }

                _queryBuilder.Remove(_queryBuilder.Length - 2, 2); //Remove trailing comma and space
                _queryBuilder.Append(") ");
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
                _queryBuilder.Append($"{fieldName} : {alias} ");
            }
            else
            {
                _queryBuilder.Append($"{fieldName} ");
            }

            if (arguments?.Any() == true)
            {
                var argumentsString = BuildArgumentsString(arguments);
                _queryBuilder.Append($"{argumentsString} ");
            }

            _queryBuilder.Append("{ ");
            nestedQueryAction.Invoke(this);
            _queryBuilder.Append("} ");

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
                if (value is Dictionary<string, object> dictionary)
                {
                    return $"{{{BuildArgumentsString(dictionary, true)}}}";
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
                return $"{_operationType} {variablesString} {{{_queryBuilder}}}";
            }

            return $"{_operationType} {{{_queryBuilder}}}";
        }

        private string BuildVariablesString()
        {
            if (_variables.Count == 0)
            {
                return "";
            }

            var variablesBuilder = new StringBuilder();
            variablesBuilder.Append("(");

            foreach (var variable in _variables)
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

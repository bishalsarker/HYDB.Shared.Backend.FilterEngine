using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HYDB.FilterEngine
{
    public class Tokenizr
    {
        public static IEnumerable<Clause> ParseClauses(string filterString)
        {
            var clauses = new List<Clause>();
            string pattern = @"AND|OR";

            var conjuctions = Regex.Matches(filterString, pattern);
            var conditions = Regex.Split(filterString, pattern);

            if(conjuctions.Count == 0)
            {
                clauses.Add(new Clause() 
                { 
                    Logic = ParseCondition(filterString),
                    RightConjuction = "" 
                });
            }
            else
            {
                clauses = GenerateClauseList(conjuctions, conditions).ToList();
            }

            if(clauses.Count < 1)
            {
                return null;
            }
            else
            {
                return clauses;
            }
        }

        private static Variable DefineVariable(string leftOrRight)
        {
            if (TypeExtractor.GetDataType(leftOrRight) == null)
            {
                return new Variable()
                {
                    Name = leftOrRight
                };
            }
            else
            {
                Type valueType = TypeExtractor.GetDataType(leftOrRight);
                var value = Convert.ChangeType(leftOrRight, valueType);
                if (valueType == typeof(string))
                {
                    value = TypeExtractor.GetStringValue(leftOrRight).ToString();
                }

                return new Variable()
                {
                    Name = "const",
                    Type = valueType,
                    Value = value
                };
            }
        }

        private static IEnumerable<Clause> GenerateClauseList(MatchCollection conjuctions, string[] conditions)
        {
            var clauses = new List<Clause>();
            int i = 0;
            while (i < conjuctions.Count)
            {
                var parsedClause = ParseClause(conjuctions[i].Value, conditions[i]);
                if (parsedClause == null)
                {
                    break;
                }
                else
                {
                    clauses.Add(parsedClause);
                    i++;
                }
            }

            var parsedLastClause = ParseClause("", conditions[conditions.Length - 1]);
            if (parsedLastClause != null)
            {
                clauses.Add(parsedLastClause);
            }

            return clauses;
        }

        private static Clause ParseClause(string conjuction, string condition)
        {
            var parsedCondition = ParseCondition(condition);

            if (parsedCondition == null)
            {
                return null;
            }
            else
            {
                return new Clause()
                {
                    Logic = parsedCondition,
                    RightConjuction = conjuction
                };
            }
        }

        public static Condition ParseCondition(string conditionString)
        {
            string pattern = @">|<|==|!=|<=|>=";

            var opratr = Regex.Matches(conditionString, pattern);
            var operands = Regex.Split(conditionString, pattern);

            if (operands.Length != 2 && opratr.Count != 1)
            {
                return null;
            }
            else
            {
                var condition = new Condition()
                {
                    Left = operands[0].Trim(),
                    Right = operands[1].Trim(),
                    Operator = opratr.FirstOrDefault().Value.Trim()
                };

                var variables = new List<Variable>();

                variables.Add(DefineVariable(condition.Left));
                variables.Add(DefineVariable(condition.Right));

                condition.Definitions = variables;

                return condition;
            }
        }
    }
}

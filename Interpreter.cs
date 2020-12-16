using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HYDB.FilterEngine
{
    public class Interpreter
    {
        public static bool Execute(List<Clause> clauseList)
        {
            var totalClause = clauseList.Count;
            if(totalClause < 2)
            {
                var functionExecutionResult = InterpreteFunction(clauseList.FirstOrDefault().Logic);
                if(functionExecutionResult != null)
                {
                    return functionExecutionResult();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return InterpreteClauseList(clauseList);
            }
        }

        private static bool InterpreteClauseList(List<Clause> clauseList)
        {
            int i = 1;
            var prevClause = clauseList.FirstOrDefault();
            bool result = false;
            while (i < clauseList.Count)
            {
                var left = Expression.Constant(InterpreteFunction(prevClause.Logic)(), typeof(bool));
                var right = Expression.Constant(InterpreteFunction(clauseList[i].Logic)(), typeof(bool));
                var body = GetConjunction(prevClause.RightConjuction, left, right);
                result = Expression.Lambda<Func<bool>>(body).Compile()();
                prevClause = clauseList[i];
                i++;
            }
            return result;
        }

        private static BinaryExpression GetConjunction(string operatr, ConstantExpression left, ConstantExpression right)
        {
            if (operatr.Equals("AND"))
            {
                return Expression.And(left, right);
            }
            else if (operatr.Equals("OR"))
            {
                return Expression.Or(left, right);
            }
            else
            {
                return null;
            }
        }

        private static Func<bool> InterpreteFunction(Condition condition)
        {
            var leftVar = condition.Definitions[0];
            var rightVar = condition.Definitions[1];

            if(leftVar.Type != rightVar.Type)
            {
                return null;
            }
            else
            {
                ConstantExpression left = Expression.Constant(ResolveZeroIfDouble(leftVar.Value, leftVar.Type), leftVar.Type);
                ConstantExpression right = Expression.Constant(ResolveZeroIfDouble(rightVar.Value, rightVar.Type), rightVar.Type);
                BinaryExpression body = GetConditionalOperator(condition.Operator, left, right);
                return Expression.Lambda<Func<bool>>(body).Compile();
            }
            
        }

        private static object ResolveZeroIfDouble(dynamic val, Type type)
        {
            object newObj;
            if (type == typeof(double) && val.GetType() == typeof(int)) {
                newObj = (double)val;
            }
            else
            {
                newObj = val;
            }

            return newObj;
        }

        private static BinaryExpression GetConditionalOperator(string operatr, ConstantExpression left, ConstantExpression right)
        {
            if(operatr.Equals("=="))
            {
                return Expression.Equal(SetType(left, right.Value.GetType()), right);
            }
            else if (operatr.Equals("!="))
            {
                return Expression.NotEqual(SetType(left, right.Value.GetType()), right);
            }
            else if(operatr.Equals(">"))
            {
                return Expression.GreaterThan(SetTypeDouble(left), right);
            }
            else if (operatr.Equals("<"))
            {
                return Expression.LessThan(SetTypeDouble(left), right);
            }
            else if (operatr.Equals("<="))
            {
                return Expression.LessThanOrEqual(SetTypeDouble(left), right);
            }
            else if(operatr.Equals(">="))
            {
                return Expression.GreaterThanOrEqual(SetTypeDouble(left), right);
            }
            else
            {
                return null;
            }
        }

        private static Expression SetTypeDouble(Expression exp)
        {
            return Expression.Convert(exp, typeof(double));
        }

        private static Expression SetType(Expression left, Type type)
        {
            return Expression.Convert(left, type);
        }
    }
}

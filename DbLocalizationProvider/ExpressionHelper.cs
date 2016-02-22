using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DbLocalizationProvider
{
    internal class ExpressionHelper
    {
        internal static string GetMemberName(Expression memberSelector)
        {
            var memberStack = WalkExpression(memberSelector as LambdaExpression);
            return memberStack.Pop().Name;
        }

        internal static string GetMemberName(Expression<Func<object>> memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);
            return memberStack.Pop().Name;
        }

        internal static string GetFullMemberName(Expression<Func<object>> memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);
            var result = memberStack.Pop().GetUnderlyingType().FullName;
            return memberStack.Aggregate(result, (current, memberInfo) => current + $".{memberInfo.Name}");
        }

        internal static Stack<MemberInfo> WalkExpression(LambdaExpression expression)
        {
            var stack = new Stack<MemberInfo>();
            var e = expression.Body;
            while (e != null)
            {
                switch (e.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpr = (MemberExpression) e;
                        stack.Push(memberExpr.Member);

                        if (memberExpr.Member.MemberType == MemberTypes.Property)
                        {
                            var propertyInfo = (PropertyInfo) memberExpr.Member;
                            if (propertyInfo.GetGetMethod().IsStatic)
                            {
                                // property is static -> so expression is null afterwards
                                // we need to push delcaring type to stack as well
                                stack.Push(propertyInfo.DeclaringType);
                            }
                        }

                        e = memberExpr.Expression;
                        break;
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        e = ((UnaryExpression) e).Operand;
                        break;
                    case ExpressionType.Constant:
                        e = null;
                        break;
                    case ExpressionType.Parameter:
                        //var parameterExpr = (ParameterExpression) e;
                        //stack.Push(parameterExpr.Type);
                        e = null;
                        break;
                    default:
                        throw new NotSupportedException("Not supported");
                }
            }

            return stack;
        }
    }
}

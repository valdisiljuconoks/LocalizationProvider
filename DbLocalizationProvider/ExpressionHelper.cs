using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DbLocalizationProvider
{
    internal class ExpressionHelper
    {
        internal static string GetMemberName(Expression memberSelector)
        {
            var memberStack = WalkExpression(memberSelector as LambdaExpression);
            return memberStack.Pop();
        }

        internal static string GetMemberName(Expression<Func<object>> memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);
            return memberStack.Pop();
        }

        internal static string GetFullMemberName(Expression<Func<object>> memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);
            return ResourceKeyBuilder.BuildResourceKey(memberStack.Pop(), memberStack);
        }

        internal static Stack<string> WalkExpression(LambdaExpression expression)
        {
            var stack = new Stack<string>();
            var e = expression.Body;
            while (e != null)
            {
                switch (e.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpr = (MemberExpression) e;

                        // if we are on the field level - we need to push full name of the underlying type
                        stack.Push(memberExpr.Member.MemberType == MemberTypes.Field
                                       ? memberExpr.Member.GetUnderlyingType().FullName
                                       : memberExpr.Member.Name);

                        if(memberExpr.Member.MemberType == MemberTypes.Property)
                        {
                            var propertyInfo = (PropertyInfo) memberExpr.Member;
                            if(propertyInfo.GetGetMethod().IsStatic)
                            {
                                // property is static -> so expression is null afterwards
                                // we need to push declaring type to stack as well
                                if(propertyInfo.DeclaringType != null)
                                {
                                    stack.Push(propertyInfo.DeclaringType.GetUnderlyingType().FullName);
                                }
                            }
                        }

                        e = memberExpr.Expression;
                        break;
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        // usually enums are coming in here
                        e = ((UnaryExpression) e).Operand;

                        var item = e as ConstantExpression;
                        if(item != null)
                        {
                            stack.Push(item.Value.ToString());
                            stack.Push(item.Type.FullName);
                        }
                        break;
                    case ExpressionType.Constant:
                        e = null;
                        break;
                    case ExpressionType.Parameter:
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

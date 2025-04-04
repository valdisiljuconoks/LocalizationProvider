// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DbLocalizationProvider.Internal;

/// <summary>
/// Helper class to get along with expressions
/// </summary>
public class ExpressionHelper
{
    private readonly ResourceKeyBuilder _keyBuilder;

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="keyBuilder">Builder of the keys.</param>
    public ExpressionHelper(ResourceKeyBuilder keyBuilder)
    {
        _keyBuilder = keyBuilder;
    }

    internal string GetMemberName(Expression memberSelector)
    {
        var memberStack = WalkExpression((LambdaExpression)memberSelector);

        return memberStack.Item2.Pop();
    }

    internal string GetMemberName(Expression<Func<object>> memberSelector)
    {
        var memberStack = WalkExpression(memberSelector);

        return memberStack.Item2.Pop();
    }

    internal string GetFullMemberName(Expression<Func<object>> memberSelector)
    {
        return GetFullMemberName((LambdaExpression)memberSelector);
    }

    internal string GetFullMemberName(LambdaExpression memberSelector)
    {
        var memberStack = WalkExpression(memberSelector);
        memberStack.Item2.Pop();

        return _keyBuilder.BuildResourceKey(memberStack.Item1, memberStack.Item2);
    }

    internal Tuple<Type, Stack<string>> WalkExpression(LambdaExpression expression)
    {
        // TODO: more I look at this, more it turns into nasty code that becomes hard to reason about
        // need to find a way to refactor to cleaner code
        var stack = new Stack<string>();
        Type? containerType = null;

        var e = expression.Body;
        while (e != null)
        {
            switch (e.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)e;

                    switch (memberExpr.Member.MemberType)
                    {
                        case MemberTypes.Field:
                            var fieldInfo = (FieldInfo)memberExpr.Member;
                            if (fieldInfo.IsStatic)
                            {
                                stack.Push(fieldInfo.Name);
                                containerType = fieldInfo.DeclaringType;

                                if (!string.IsNullOrEmpty(fieldInfo?.DeclaringType?.FullName))
                                {
                                    stack.Push(fieldInfo?.DeclaringType?.FullName!);
                                }
                            }
                            else if (memberExpr.Expression?.NodeType != ExpressionType.Constant)
                            {
                                /* we need to push current field name if next node in the tree is not constant
                                 * usually this means that we are at "ThisIsField" level in following expression
                                 *
                                 * () => t.ThisIsField
                                 *             ^
                                 */
                                stack.Push(fieldInfo.Name);
                            }
                            else
                            {
                                /* if we are on the field level - we need to push full name of the underlying type
                                 * this is usually last node in the tree - level "t"
                                 *
                                 * () => t.ThisIsField
                                 *       ^
                                 */
                                containerType = fieldInfo.GetUnderlyingType();
                                stack.Push(containerType.FullName!);
                            }

                            break;

                        case MemberTypes.Property:
                            stack.Push(memberExpr.Member.Name);

                            var propertyInfo = (PropertyInfo)memberExpr.Member;
                            if ((propertyInfo.GetGetMethod()?.IsStatic ?? false) && propertyInfo.DeclaringType != null)
                            {
                                // property is static -> so expression is null afterward
                                // we need to push declaring type to stack as well
                                containerType = propertyInfo.DeclaringType;
                                stack.Push(propertyInfo.DeclaringType.FullName!);
                            }

                            break;
                    }

                    e = memberExpr.Expression;
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    // usually System.Enum comes here
                    e = ((UnaryExpression)e).Operand;

                    if (e is ConstantExpression item)
                    {
                        if (item.Value is not null)
                        {
                            stack.Push(item.Value.ToString()!);
                        }

                        stack.Push(item.Type.FullName!);
                        containerType = item.Type;
                    }

                    break;

                case ExpressionType.Constant:
                    e = null;
                    break;

                case ExpressionType.Parameter:
                    stack.Push(e.Type.FullName!);
                    containerType = e.Type;
                    e = null;
                    break;

                default:
                    throw new NotSupportedException("Not supported");
            }
        }

        return new Tuple<Type, Stack<string>>(containerType!, stack);
    }
}

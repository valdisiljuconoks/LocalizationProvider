// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DbLocalizationProvider.Internal
{
    internal class ExpressionHelper
    {
        internal static string GetMemberName(Expression memberSelector)
        {
            var memberStack = WalkExpression((LambdaExpression) memberSelector);

            return memberStack.Item2.Pop();
        }

        internal static string GetMemberName(Expression<Func<object>> memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);

            return memberStack.Item2.Pop();
        }

        internal static string GetFullMemberName(Expression<Func<object>> memberSelector)
        {
            return GetFullMemberName((LambdaExpression) memberSelector);
        }

        internal static string GetFullMemberName(LambdaExpression memberSelector)
        {
            var memberStack = WalkExpression(memberSelector);
            memberStack.Item2.Pop();

            return ResourceKeyBuilder.BuildResourceKey(memberStack.Item1, memberStack.Item2);
        }

        internal static Tuple<Type, Stack<string>> WalkExpression(LambdaExpression expression)
        {
            // TODO: more I look at this, more it turns into nasty code that becomes hard to reason about
            // need to find a way to refactor to cleaner code
            var stack = new Stack<string>();
            Type containerType = null;

            var e = expression.Body;
            while (e != null)
            {
                switch (e.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpr = (MemberExpression) e;

                        switch (memberExpr.Member.MemberType)
                        {
                            case MemberTypes.Field:
                                var fieldInfo = (FieldInfo) memberExpr.Member;
                                if(fieldInfo.IsStatic)
                                {
                                    stack.Push(fieldInfo.Name);
                                    containerType = fieldInfo.DeclaringType;
                                    stack.Push(fieldInfo.DeclaringType.FullName);
                                }
                                else if(memberExpr.Expression.NodeType != ExpressionType.Constant)
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
                                    stack.Push(containerType.FullName);
                                }
                                break;

                            case MemberTypes.Property:
                                stack.Push(memberExpr.Member.Name);

                                var propertyInfo = (PropertyInfo) memberExpr.Member;
                                if(propertyInfo.GetGetMethod().IsStatic)
                                {
                                    // property is static -> so expression is null afterwards
                                    // we need to push declaring type to stack as well
                                    if(propertyInfo.DeclaringType != null)
                                    {
                                        containerType = propertyInfo.DeclaringType;
                                        stack.Push(propertyInfo.DeclaringType.FullName);
                                    }
                                }
                                break;
                        }

                        e = memberExpr.Expression;
                        break;

                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        // usually System.Enum comes here
                        e = ((UnaryExpression) e).Operand;

                        if(e is ConstantExpression item)
                        {
                            stack.Push(item.Value.ToString());
                            stack.Push(item.Type.FullName);
                            containerType = item.Type;
                        }
                        break;

                    case ExpressionType.Constant:
                        e = null;
                        break;

                    case ExpressionType.Parameter:
                        stack.Push(e.Type.FullName);
                        containerType = e.Type;
                        e = null;
                        break;

                    default:
                        throw new NotSupportedException("Not supported");
                }
            }

            return new Tuple<Type, Stack<string>>(containerType, stack);
        }
    }
}

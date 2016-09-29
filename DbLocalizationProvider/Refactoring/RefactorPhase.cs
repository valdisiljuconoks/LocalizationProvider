using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.Refactoring
{
    public class RefactorPhase<T>
    {
        private readonly Type _targetType;
        private readonly ICollection<RefactorStep> _list;

        internal RefactorPhase(Type targetType, ICollection<RefactorStep> list)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            _targetType = targetType;
            _list = list;
        }


        public RefactorStep Property(Expression<Func<T, object>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return Property(ExpressionHelper.GetMemberName(property));
        }

        public RefactorStep Property(string newResourceKey)
        {
            return new RefactorStep(_targetType, _list, newResourceKey);
        }
    }
}
using System;
using System.Collections.Generic;

namespace DbLocalizationProvider.Refactoring
{
    public class RefactorStep
    {
        private readonly ICollection<RefactorStep> _list;
        private readonly string _newResourceKey;

        public RefactorStep(Type targetType, ICollection<RefactorStep> list, string newResourceKey)
        {
            TargetType = targetType;
            _list = list;
            _newResourceKey = newResourceKey;
        }

        public Type TargetType { get; private set; }

        public string OldResourceKey { get; private set; }

        public string NewResourceKey { get; private set; }

        public void Was(string oldResourceKey)
        {
            if (oldResourceKey == null)
            {
                throw new ArgumentNullException(nameof(oldResourceKey));
            }

            NewResourceKey = _newResourceKey;
            OldResourceKey = oldResourceKey;
            _list.Add(this);
        }
    }
}

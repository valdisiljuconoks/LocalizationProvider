using System;
using System.Collections.Generic;
using System.Reflection;

namespace DbLocalizationProvider.Refactoring
{
    public abstract class RefactoredModelAttribute
    {
        private readonly List<RefactorStep> _list = new List<RefactorStep>();

        internal ICollection<RefactorStep> Steps => _list;

        public RefactorPhase<T> For<T>()
        {
            var t = typeof (RefactorPhase<>).MakeGenericType(typeof (T));
            return Activator.CreateInstance(t, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { typeof (T), _list }, null) as RefactorPhase<T>;
        }

        public abstract void DefineChanges();
    }
}

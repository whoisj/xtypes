using System;
using System.Collections;
using System.Collections.Generic;

namespace XTypes.Options
{
    abstract class OptionSet : ICollection<OptionSet>, IEnumerable<OptionSet>
    {
        public OptionSet()
        { }

        public abstract int Count { get; }

        private volatile bool _isReadOnly;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            protected set { _isReadOnly = value; }
        }

        public abstract void Add(OptionSet item);

        public abstract void Clear();

        public abstract bool Contains(OptionSet item);

        public abstract void CopyTo(OptionSet[] array, int arrayIndex);

        public abstract IEnumerator<OptionSet> GetEnumerator();

        public abstract bool Remove(OptionSet item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

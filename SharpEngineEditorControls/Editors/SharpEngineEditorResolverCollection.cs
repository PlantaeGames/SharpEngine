using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpEngineEditorControls.Editors
{
    public sealed class SharpEngineEditorResolverCollection : IEnumerable<SharpEngineEditorResolver>
    {
        public sealed class Enumerator : IEnumerator<SharpEngineEditorResolver>
        {
            private readonly SharpEngineEditorResolver[] _resolvers;

            public SharpEngineEditorResolver Current => _resolvers[_current];

            object IEnumerator.Current => Current;
            private int _current = -1;
            private bool _disposed;

            public Enumerator(SharpEngineEditorResolver[] resolvers)
            {
                _resolvers = resolvers;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
            }

            public bool MoveNext()
            {
                _current++;
                return _current < _resolvers.Length;
            }

            public void Reset()
            {
                _current = -1;
            }
        }

        private List<SharpEngineEditorResolver> _resolvers = new();

        public SharpEngineEditorResolver this[int index]
        {
            get
            {
                Debug.Assert(index < _resolvers.Count);

                return _resolvers[index];
            }
        }

        public void Add(SharpEngineEditorResolver resolver) => _resolvers.Add(resolver);
        public void AddRange(SharpEngineEditorResolver[] resolvers) => _resolvers.AddRange(resolvers);
        public void Remove(SharpEngineEditorResolver resolver) => _resolvers.Remove(resolver);

        public void Clear() => _resolvers.Clear();

        public IEnumerator<SharpEngineEditorResolver> GetEnumerator()
        {
            return new Enumerator([.. _resolvers]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator([.. _resolvers]);
        }
    }
}
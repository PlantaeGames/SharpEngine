using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpEngineEditorControls.Editors
{
    public sealed class SharpEngineEditorCollection : IEnumerable<SharpEngineEditorCollection.Pair>
    {
        public readonly struct Pair
        {
            public readonly SharpEngineEditor Editor;
            public readonly Type TargetType;

            public Pair(SharpEngineEditor editor, Type targetType)
            {
                Editor = editor;
                TargetType = targetType;
            }
        }

        public sealed class Enumerator : IEnumerator<Pair>
        {
            private readonly Pair[] _pairs;

            public Pair Current => _pairs[_currentIndex];
            object System.Collections.IEnumerator.Current => Current;

            private int _currentIndex = -1;
            private bool _disposed;

            public Enumerator(Pair[] pairs)
            {
                _pairs = pairs;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
            }

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _pairs.Length;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }
        }

        private List<Pair> _pairs = new();

        public void Add(Pair pair)
        {
            _pairs.Add(pair);
        }

        public void Remove(Pair pair)
        {
            _pairs.Remove(pair);
        }

        public void AddRange(Pair[] pairs)
        {
            _pairs.AddRange(pairs);
        }

        public Pair this[int index]
        {
            get
            {
                Debug.Assert(index < _pairs.Count);

                return _pairs[index];
            }
        }

        public void Clear() => _pairs.Clear();

        public System.Collections.IEnumerator GetEnumerator()
        {
            return new Enumerator([.. _pairs]);
        }

        IEnumerator<Pair> IEnumerable<Pair>.GetEnumerator()
        {
            return new Enumerator([.. _pairs]);
        }
    }
}
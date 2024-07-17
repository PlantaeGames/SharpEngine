using SharpEngineEditorControls.Attributes;
using SharpEngineEditorControls.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SharpEngineEditorControls.Editors
{
    public class SharpEngineEditorResolver
    {
        protected SharpEngineEditorCollection _editors;

        public SharpEngineEditor Resolve(Type type)
        {
            var editor = _editors.Where(x => x.TargetType == type).FirstOrDefault().Editor;
            editor ??= new DefaultSharpEngineEditor();

            return editor;
        }

        public void Clean()
        {
            foreach(SharpEngineEditorCollection.Pair pair in _editors)
            {
                pair.Editor.Clear();
            }
        }

        public SharpEngineEditorResolver()
        {
            Initialize([Assembly.GetExecutingAssembly()]);
        }

        public SharpEngineEditorResolver(Assembly[] assemblies)
        {
            Debug.Assert(assemblies != null);
            Debug.Assert(assemblies.Length > 0);

            Initialize(assemblies);
        }

        private void Initialize(Assembly[] assemblies)
        {
            var collection = new SharpEngineEditorCollection();

            foreach (var assembly in assemblies)
            {
                var typeAttributePairs = assembly.GetTypesOfCustomAttribute<SharpEngineEditorAttribute>();

                foreach (var typeAttributePair in typeAttributePairs)
                {
                    if (typeAttributePair.type.GetConstructor(Type.EmptyTypes) == null)
                        continue;

                    SharpEngineEditor editor = null;
                    try
                    {
                        editor = Activator.CreateInstance(typeAttributePair.type) as SharpEngineEditor;
                    }
                    catch (Exception e)
                    {
                        Debug.Assert(false, $"{e}");
                    }

                    Debug.Assert(editor != null);

                    var pair = new SharpEngineEditorCollection.Pair(editor, typeAttributePair.attribute.TargetType);
                    collection.Add(pair);
                }
            }

            _editors = collection;
        }
    }
}
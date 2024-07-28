using SharpEngineEditorControls.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace SharpEngineEditorControls.Editors
{
    public abstract class SharpEngineEditor
    {
        protected Stack<PrimitiveBinding> _record = new();

        public void Clear()
        {
            _record.Clear();
        }

        public virtual UICollection CreateUI(SharpEngineEditorResolver resolver,
            PrimitiveBinding binding)
        {
            if (binding.Parent == null)
                return UICollection.Empty();

            var collection = new UICollection();

            var isCollection = binding.Valid == false;
            var parent = isCollection ? binding.Parent : binding.Value;

            var instanceType = parent.GetType();

            foreach (var field in instanceType.GetFields(
                BindingFlags.Public | BindingFlags.Instance))
            {
                var fieldType = field.FieldType;

                var editor = resolver.Resolve(fieldType);

                var more = editor.CreateUI(resolver, new(parent, field));
                more.Margin = new(0, 10, 0, 0);
                collection += more;
            }

            return collection;
        }
    }
}
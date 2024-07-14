using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using SharpEngineBuildSystem.Exceptions;
using System.Runtime.InteropServices;
using TerraFX.Interop.DirectX;

namespace SharpEngineBuildSystem.Utilities;

public class CodeGenerator
{
    public interface ICodeBuilder;

    public class Builder : ICodeBuilder
    {
        public interface IFieldBuilder : ICodeBuilder
        {
            ITypeBuilder FeildEnd();
        }

        public interface ITypeBuilder : ICodeBuilder
        {
            Builder TypeEnd();

            IConstructorBuilder ConstructorStart();

            ITypeBuilder Attributes(TypeAttributes attributes);
            ITypeBuilder Name(string name);
        }

        public interface IConstructorBuilder : ICodeBuilder
        {
            IConstructorBuilder Default(MethodAttributes attributes);
            ITypeBuilder ConstructorEnd();
        }

        public interface IPropertyBuilder : ICodeBuilder
        {
            IMethodBuilder<IPropertyBuilder> SetterStart();
            IMethodBuilder<IPropertyBuilder> GetterStart();

            IPropertyBuilder Define<T>(string name, PropertyAttributes attributes);
            ITypeBuilder PropertyEnd();
        }

        public interface IMethodBuilder<T> : ICodeBuilder
            where T : ICodeBuilder
        {
#nullable enable
            IMethodBuilder<T> Define(string name, MethodAttributes attributes, System.Type[]? @params);
#nullable disable
            T MethodEnd();
        }

        private class Type : ITypeBuilder
        {
            private class Feild : IFieldBuilder
            {
                private TypeBuilder _typeBuilder;
                private Type _type;

                public ITypeBuilder FeildEnd()
                {
                    return _type;
                }

                public Feild(Type type, TypeBuilder typeBuilder)
                {
                    _type = type;
                    _typeBuilder = typeBuilder;
                }
            }

            private class PropertyMethod : IMethodBuilder<IPropertyBuilder>
            {
                private readonly Property _property;
                private readonly TypeBuilder _typeBuilder;

#nullable enable
                public IMethodBuilder<IPropertyBuilder> Define(string name, MethodAttributes attributes, System.Type[]? @params)
                {
                    Debug.Assert(name != null);
                    Debug.Assert(name.Length > 0);

                    return this;
                }
#nullable disable

                IPropertyBuilder IMethodBuilder<IPropertyBuilder>.MethodEnd()
                {
                    return _property;
                }

                public PropertyMethod(Property property, TypeBuilder typeBuilder)
                {
                    _property = property;
                    _typeBuilder = typeBuilder;
                }
            }

            private class Constructor : IConstructorBuilder
            {
                private readonly TypeBuilder _typeBuilder;
                private readonly Type _type;

                private ConstructorBuilder _constructorBuilder;
                private ILGenerator _ilGenerator;

                IConstructorBuilder IConstructorBuilder.Default(MethodAttributes attributes)
                {
                    ConstructorBuilder constructor = null;

                    try
                    {
                        constructor = _typeBuilder.DefineDefaultConstructor(attributes);

                        _ilGenerator = constructor.GetILGenerator();
                        var baseConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);

                        _ilGenerator.Emit(OpCodes.Ldarg_0);
                        _ilGenerator.Emit(OpCodes.Call, baseConstructor);
                    }
                    catch (Exception e)
                    {
                        throw new FailedToGenerateCode(
                            $"Failed to create default constructor, Attributes: {attributes}", e);
                    }
                    return this;
                }

                public ITypeBuilder ConstructorEnd()
                {
                    return _type;
                }

                public Constructor(TypeBuilder typeBuilder, Type type)
                {
                    _typeBuilder = typeBuilder;
                    _type = type;
                }
            }

            private class Property : IPropertyBuilder
            {
                private readonly TypeBuilder _typeBuilder;
                private readonly Type _type;

                private PropertyBuilder _propertyBuilder;
                private PropertyMethod _setterMethod;
                private PropertyMethod _getterMethod;

                private string _providedName;

                public IMethodBuilder<IPropertyBuilder> SetterStart()
                {
                    var builder = new PropertyMethod(this, _typeBuilder);
                    _setterMethod = builder;

                    return _setterMethod;
                }

                public IMethodBuilder<IPropertyBuilder> GetterStart()
                {
                    var builder = new PropertyMethod(this, _typeBuilder);
                    _getterMethod = builder;

                    return _setterMethod;
                }

                IPropertyBuilder IPropertyBuilder.Define<T>(string name, PropertyAttributes attributes)
                {
                    Debug.Assert(name != null);
                    Debug.Assert(name.Length > 0);

                    _providedName = name;

                    try
                    {
                        _propertyBuilder = _typeBuilder.DefineProperty(_providedName, attributes, typeof(T), System.Type.EmptyTypes);
                    }
                    catch(Exception e)
                    {
                        throw new FailedToGenerateCode(
                            $"Failed to define property, Name: {name}", e);
                    }

                    return this;
                }

                public ITypeBuilder PropertyEnd()
                {
                    return _type;
                }

                public Property(TypeBuilder typeBuilder, Type type)
                {
                    _typeBuilder = typeBuilder;
                    _type = type;
                }
            }

            private string _providedName;
            private TypeBuilder _typeBuilder;

            private readonly ModuleBuilder _moduleBuilder;
            private readonly Builder _builder;

            private List<IConstructorBuilder> _constructors = new();
            private List<IPropertyBuilder> _properties = new();

            public Builder TypeEnd()
            {
                return _builder;
            }

            public IConstructorBuilder ConstructorStart()
            {
                Debug.Assert(_typeBuilder != null);

                var constructor = new Constructor(_typeBuilder, this);
                _constructors.Add(constructor);

                return constructor;
            }

            /*          [OBSOLETE("Constructors Builders in Place")]
             * 
             * 
                        ITypeBuilder ITypeBuilder.AddConstructor(MethodAttributes attributes)
                        {
                            ConstructorBuilder constructor = null;

                            try
                            {
                                constructor = _typeBuilder.DefineDefaultConstructor(attributes);

                                var il = constructor.GetILGenerator();
                                var baseConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);

                                il.Emit(OpCodes.Ldarg_0);
                                il.Emit(OpCodes.Call, baseConstructor);
                            }
                            catch(Exception e)
                            {
                                throw new FailedToGenerateCode(
                                    $"Failed to create default constructor, Name: {_providedName}, Attributes: {attributes}", e);
                            }
                            _constructors.Add(constructor);

                            return this;
                        }*/

            /*#nullable enable
                        ITypeBuilder ITypeBuilder.AddConstructor(MethodAttributes attributes, CallingConventions convention, System.Type[]? @params)
                        {
                            ConstructorBuilder? constructor = null;

                            try
                            {
                                constructor = _typeBuilder.DefineConstructor(attributes, convention, @params);
                            }
                            catch (Exception e)
                            {
                                throw new FailedToGenerateCode(
                                    $"Failed to create default constructor, Name: {_providedName}, Attributes: {attributes}, Convention: {convention}, Params: {@params}", e);
                            }
                            _constructors.Add(constructor);

                            return this;
                        }
            #nullable disable*/

            ITypeBuilder ITypeBuilder.Attributes(System.Reflection.TypeAttributes attributes)
            {
                try
                {
                    _typeBuilder = _moduleBuilder.DefineType(_providedName, attributes);
                }
                catch(Exception e)
                {
                    throw new FailedToGenerateCode(
                        $"Failed to create type builder, Name: {_providedName}, Attributes: {attributes}", e);
                }

                return this;
            }

            ITypeBuilder ITypeBuilder.Name(string name)
            {
                Debug.Assert(name != null);
                Debug.Assert(name.Length > 0);

                _providedName = name;

                return this;
            }

            public Type(Builder builder, ModuleBuilder moduleBuilder) 
            {
                _builder = builder;
                _moduleBuilder = moduleBuilder;
            }
        }

        private string _providedName;
        private string _providedModuleName;

        private AssemblyName _assenbmyName;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        private List<ITypeBuilder> _types = new();

        public ITypeBuilder TypeStart()
        {
            Debug.Assert(_moduleBuilder != null,
                            $"Assembly muset be defined first.");

            var typeBuilder = new Type(this, _moduleBuilder);
            _types.Add(typeBuilder);

            return typeBuilder;
        }

        public Builder Module(string name)
        {
            Debug.Assert(name != null);
            Debug.Assert(name.Length > 0);

            try
            {
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(name);
            }
            catch(Exception e)
            {
                throw new FailedToGenerateCode(
                    $"Failed to define dynamic module, {name}", e);
            }
            return this;
        }

        public Builder AccessMode(AssemblyBuilderAccess mode)
        {
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assenbmyName, mode);

            return this;
        }

        public Builder AssemblyName(string name)
        {
            Debug.Assert(name != null);
            Debug.Assert(name.Length > 0);

            _providedName = name;

            try
            {
                _assenbmyName = new AssemblyName(_providedName);
            }
            catch(Exception e)
            {
                throw new FailedToGenerateCode(
                    $"Failed to define assembly name, {_providedName}", e);
            }

            return this;
        }

        public static Builder Create() => new();

        private Builder() { }
    }

    private CodeGenerator() { }
}

﻿namespace Stash
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;

    public static class Util
    {
        public static Type<T> TypeOf<T>()
        {
            return new Type<T>();
        }
    }

    public sealed class Type<T> : Type //// TODO needs a factory for covariance //// TODO should it be covariant instead of contravariant? maybe have three class, one for covariant, one for contravariant, and one for exact
    {
        private readonly Type type;

        public Type()
        {
            type = typeof(T);
        }

        public override Assembly Assembly => type.Assembly;

        public override string? AssemblyQualifiedName => type.AssemblyQualifiedName;

        public override Type? BaseType => type.BaseType;

        public override string? FullName => type.FullName;

        public override Guid GUID => type.GUID;

        public override Module Module => type.Module;

        public override string? Namespace => type.Namespace;

        public override Type UnderlyingSystemType => type.UnderlyingSystemType;

        public override string Name => type.Name;

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return type.GetConstructors(bindingAttr);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return type.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit);
        }

        public override Type? GetElementType()
        {
            return type.GetElementType();
        }

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            return type.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return type.GetEvents(bindingAttr);
        }

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            return type.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return type.GetFields(bindingAttr);
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            return type.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return type.GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return type.GetMembers(bindingAttr);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return type.GetMethods(bindingAttr);
        }

        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            return type.GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return type.GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return type.GetProperties(bindingAttr);
        }

        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            return type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return type.IsDefined(attributeType, inherit);
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return type.Attributes;
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            return type.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            //// TODO i'm not clear if `array.empty` is actually semantically correct
            return type.GetMethod(name, bindingAttr, binder, callConvention, types ?? Array.Empty<Type>(), modifiers);
        }

        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            //// TODO i'm not clear if `array.empty` is actually semantically correct
            return type.GetProperty(name, bindingAttr, binder, returnType, types ?? Array.Empty<Type>(), modifiers);
        }

        protected override bool HasElementTypeImpl()
        {
            return type.HasElementType;
        }

        protected override bool IsArrayImpl()
        {
            return type.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return type.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return type.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return type.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return type.IsPrimitive;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Appointer.Utility
{
    public static class ReflectionHelper
    {
        public static T MapTo<T>(this object value, bool chechName = false) where T : class, new()
        {
            var valueType = value.GetType();
            if (chechName && valueType.Name != typeof(T).Name)
                return null;

            var result = new T();
            foreach (var prop in typeof(T).GetProperties())
            {
                var propInfo = valueType.GetProperty(prop.Name);
                if (propInfo != null)
                {
                    var propValue = propInfo.GetValue(value, null);
                    prop.SetValue(result, propValue, null);
                }
            }
            return result;
        }

        public static bool HasAttribute<T>(this MemberInfo type, bool inherit = false) where T : Attribute
        {
            return HasAttribute(type, typeof(T), inherit);
        }

        public static bool HasAttribute(this MemberInfo type, Type attribute, bool inherit = false)
        {
            return Attribute.IsDefined(type, attribute, inherit);
        }

        public static string ToDisplay<T, TProp>(Expression<Func<T, TProp>> propertyExpression, DisplayProperty property = DisplayProperty.Name)
        {
            var propInfo = propertyExpression.GetPropInfo();
            var attr = propInfo.GetAttribute<DisplayNameAttribute>(false);
            return attr.GetType().GetProperty(property.ToString()).GetValue(attr, null) as string ?? propInfo.Name;
        }

        public static PropertyInfo GetPropInfo<T, TPropType>(this Expression<Func<T, TPropType>> keySelector)
        {
            //var propInfo = (keySelector.Body as MemberExpression)?.Member as PropertyInfo;
            var member = keySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");
            return propInfo;
        }

        public static MemberInfo GetPropertyInformation(this Expression propertyExpression)
        {
            var memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                var unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }
            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }
            return null;
        }

        public static T GetAttribute<T>(this MemberInfo member, bool isRequired = false) where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();
            if (attribute == null && isRequired)
                throw new ArgumentException($"The {typeof(T).Name} attribute must be defined on member {member.Name}");
            return (T)attribute;
        }

        public static bool IsInheritFrom<T>(this Type type)
        {
            return IsInheritFrom(type, typeof(T));
        }

        public static bool IsInheritFrom(this Type type, Type parentType)
        {
            //the 'is' keyword do this too for values (new ChildClass() is ParentClass)
            return parentType.IsAssignableFrom(type);
        }

        public static bool BaseTypeIsGeneric(this Type type, Type genericType)
        {
            return type.BaseType?.IsGenericType == true && type.BaseType.GetGenericTypeDefinition() == genericType;
        }

        public static IEnumerable<Type> GetTypesAssignableFrom<T>(params Assembly[] assemblies)
        {
            return typeof(T).GetTypesAssignableFrom(assemblies);
        }

        public static IEnumerable<Type> GetTypesAssignableFrom(this Type type, params Assembly[] assemblies)
        {
            return assemblies.SelectMany(p => p.GetTypes()).Where(p => p.IsInheritFrom(type));
        }

        public static IEnumerable<Type> GetTypesHasAttribute<T>(params Assembly[] assemblies) where T : Attribute
        {
            return typeof(T).GetTypesHasAttribute(assemblies);
        }

        public static IEnumerable<Type> GetTypesHasAttribute(this Type type, params Assembly[] assemblies)
        {
            return assemblies.SelectMany(p => p.GetTypes()).Where(p => p.HasAttribute(type));
        }

        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && type.IsInheritFrom<IEnumerable>();
        }

        public static bool IsEnumerable<T>(this Type type)
        {
            return type != typeof(string) && type.IsInheritFrom<IEnumerable<T>>() && type.IsGenericType;
        }

        public static Type GetBaseType<T>()
        {
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType ?? type;

            //if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            //    return typeof(T).GetGenericArguments()[0];
            //return type;
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if ((type == null) || (type.BaseType == null))
                yield break;

            foreach (var i in type.GetInterfaces())
                yield return i;

            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        public static bool IsCustomType(this Type type)
        {
            //return type.Assembly.GetName().Name != "mscorlib";
            return type.IsCustomReferenceType() || type.IsCustomReferenceType();
        }

        public static bool IsCustomValueType(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System", StringComparison.Ordinal);
        }

        public static bool IsCustomReferenceType(this Type type)
        {
            return !type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System", StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivatePropertyValue<T>(object obj, string propName)
        {
            if (obj != null)
            {
                PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi == null)
                    throw new ArgumentOutOfRangeException(nameof(propName), $"Property {propName} was not found in Type {obj.GetType().FullName}");
                return (T)pi.GetValue(obj, null);
            }
            return default(T);
        }

        /// <summary>
        /// Returns a private Field Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Field</typeparam>
        /// <param name="obj">Object from where the Field Value is returned</param>
        /// <param name="propName">Field Name as string.</param>
        /// <returns>FieldValue</returns>
        public static T GetPrivateFieldValue<T>(object obj, string propName)
        {
            if (obj != null)
            {
                Type t = obj.GetType();
                FieldInfo fi = null;
                while (fi == null && t != null)
                {
                    fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    t = t.BaseType;
                }
                if (fi == null)
                    throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
                return (T)fi.GetValue(obj);
            }

            return default(T);
        }

        /// <summary>
        /// Sets a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">Value to set.</param>
        /// <returns>PropertyValue</returns>
        public static void SetPrivatePropertyValue<T>(object obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                throw new ArgumentOutOfRangeException(nameof(propName), $"Property {propName} was not found in Type {obj.GetType().FullName}");
            t.InvokeMember(propName,
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty
                           | BindingFlags.Instance, null, obj, new object[] { val });
        }

        /// <summary>
        /// Set a private Field Value on a given Object. Uses Reflection.
        /// </summary>
        /// <typeparam name="T">Type of the Field</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Field name as string.</param>
        /// <param name="val">the value to set</param>
        /// <exception cref="ArgumentOutOfRangeException">if the Property is not found</exception>
        public static void SetPrivateFieldValue<T>(object obj, string propName, T val)
        {
            if (obj != null)
            {
                Type t = obj.GetType();
                FieldInfo fi = null;
                while (fi == null && t != null)
                {
                    fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    t = t.BaseType;
                }
                if (fi == null)
                    throw new ArgumentOutOfRangeException(nameof(propName), $"Field {propName} was not found in Type {obj.GetType().FullName}");
                fi.SetValue(obj, val);
            }
        }
    }
}

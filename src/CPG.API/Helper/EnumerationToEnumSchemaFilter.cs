using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using static System.Diagnostics.Activity;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace CPG.API.Helper
{
    public class EnumerationToEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (IsSubclassOf(typeof(Enumeration<>), context.Type))
            {
                FieldInfo[] fields = context.Type.GetFields(BindingFlags.Static | BindingFlags.Public);
                schema.Enum = fields.Select((FieldInfo field) => new OpenApiString(field.Name)).Cast<IOpenApiAny>().ToList();
                schema.Type = "string";
                schema.Properties = null;
                schema.AllOf = null;
            }
        }

        private static bool IsSubclassOf(Type? generic, Type? toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                Type type = (toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck);
                if (generic == type)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }


    public abstract class Enumeration<T> : IComparable, IEquatable<Enumeration<T>>, IComparable<Enumeration<T>> where T : IEquatable<T>, IComparable<T>
    {
        public string? Name { get; set; }

        public T Value { get; set; }

        [ExcludeFromCodeCoverage]
        protected Enumeration()
        {
        }

        protected Enumeration(string name, T value)
        {
            Value = value;
            Name = name;
        }

        public static bool operator ==(Enumeration<T> left, Enumeration<T> right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(Enumeration<T> left, Enumeration<T> right)
        {
            return !object.Equals(left, right);
        }

        public override string? ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Enumeration<T>)
            {
                return Equals(obj as Enumeration<T>);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(Enumeration<T>? other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(Enumeration<T>? other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if ((object)this == other)
            {
                return true;
            }

            return Value.Equals(other.Value);
        }

        public int CompareTo(object? other)
        {
            return Value.CompareTo((other as Enumeration<T>).Value);
        }
    }



}

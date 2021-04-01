using System;
using Micro.Net.Abstractions;
using Micro.Net.Exceptions;

namespace Micro.Net.Serializing
{
    public static class SerializerExtensions
    {
        public static object Materialize(this ISerializer serializer, Type type, string value)
        {
            return typeof(ISerializer)
                       .GetMethod(nameof(ISerializer.Materialize))?
                       .MakeGenericMethod(type)
                       .Invoke(serializer, new object[]{value}) 
                   ?? throw new MicroException("An error occurred while doing something dangerous with a serializer!", 0);
        }

        public static string Serialize(this ISerializer serializer, object obj)
        {
            return (string)typeof(ISerializer)
                       .GetMethod(nameof(ISerializer.Serialize))?
                       .MakeGenericMethod(obj.GetType())
                       .Invoke(serializer, new object[] { obj })
                   ?? throw new MicroException("An error occurred while doing something dangerous with a serializer!", 0);
        }
    }
}
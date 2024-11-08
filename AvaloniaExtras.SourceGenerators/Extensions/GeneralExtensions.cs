using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AvaloniaExtras.SourceGenerators.Extensions;

public static class GeneralExtensions
{
    public static bool IsDefault<T>(this T source)
        where T : struct => source.Equals(default(T));

    public static T MapToAttributeType<T>(this AttributeData attributeData)
        where T : Attribute
    {
        T attribute;
        if (attributeData is { AttributeConstructor: not null, ConstructorArguments.Length: > 0 })
        {
            attribute = (T)
                Activator.CreateInstance(
                    typeof(T),
                    attributeData.ConstructorArguments.Select(x => x.Value).ToArray()
                );
        }
        else
        {
            attribute = (T)Activator.CreateInstance(typeof(T));
        }

        foreach (var p in attributeData.NamedArguments)
        {
            var type = typeof(T);
            var field = type.GetField(p.Key);
            if (field != null)
                field.SetValue(attribute, p.Value.Value);
            else
                type.GetProperty(p.Key)?.SetValue(attribute, p.Value.Value);
        }

        return attribute;
    }
}

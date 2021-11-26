
namespace Simulation.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class TypeAttribute : Attribute
{
    public TypeAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}

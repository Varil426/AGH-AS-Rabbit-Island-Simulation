
namespace Simulation.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal class TypeAttribute : Attribute
{
    public TypeAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}

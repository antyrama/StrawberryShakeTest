using StrawberryShakeTest.Pim;

namespace StrawberryShakeTest;

public class Component
{
    public Config Config { get; init; } = default!;
    public string Id { get; init; } = default!;
    public ComponentType Type { get; init; } = default!;
}

public class Config
{
    public List<Component> Components { get; init; } = default!;
}

public class Data
{
    public Shape Shape { get; init; } = default!;
}

public class GetMany
{
    public List<Component> Components { get; init; } = default!;
    public string Identifier { get; init; } = default!;
    public List<VariantComponent> VariantComponents { get; init; } = default!;
    public string Id { get; init; } = default!;
}

public class Root
{
    public Data Data { get; init; } = default!;
}

public class Shape
{
    public List<GetMany> GetMany { get; init; } = default!;
}

public class VariantComponent
{
    public string Id { get; init; } = default!;
    public ComponentType Type { get; init; } = default!;
}
﻿namespace Pocoman;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PocoAttribute : Attribute
{
    public PocoAttribute()
    {
    }

    public PocoAttribute(Type builderType)
    {
    }
}

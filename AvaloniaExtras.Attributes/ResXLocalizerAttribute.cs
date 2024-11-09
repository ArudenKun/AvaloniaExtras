using System;

namespace AvaloniaExtras.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ResXLocalizerAttribute(Type resourceType) : Attribute;

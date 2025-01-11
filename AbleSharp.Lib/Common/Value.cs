using System.Xml.Serialization;

namespace AbleSharp.Lib;

public class Value<T>
{
    [XmlAttribute("Value")]
    public T Val { get; set; }

    public static implicit operator T(Value<T> value) => value.Val;
    public static implicit operator Value<T>(T value) => new Value<T> { Val = value };
}
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
    public string ComplexMethod(int id, string name) => string.Empty;
}

[Serializable] 
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ComplexMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
        Assert.Contains("PublicField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueIfAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        var hasAttr = analyzer.HasAttribute<SerializableAttribute>();

        Assert.True(hasAttr);
    }

    [Fact]
    public void GetMethodParams_ReturnsReturnTypeAndParameters()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("ComplexMethod").ToList();

        Assert.Equal("Return: String", result[0]);
        Assert.Equal("Int32 id", result[1]);
        Assert.Equal("String name", result[2]);
    }
}

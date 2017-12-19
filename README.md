# DataMocks
[![Build status](https://ci.appveyor.com/api/projects/status/lbdbfyux2xt5jx4k?svg=true)](https://ci.appveyor.com/project/sadedil/datamocks-ekfm1)
[![Test results](https://img.shields.io/appveyor/tests/sadedil/DataMocks-ekfm1.svg)](https://ci.appveyor.com/project/sadedil/datamocks-ekfm1/build/tests)
[![Nuget](https://img.shields.io/nuget/v/DataMocks.svg)](https://www.nuget.org/packages/DataMocks/)

## Information
This library contains mocks for System.Data classes (Currently IDataReader is supported)

## Installation
You can install DataMocks by copying and pasting the following command into your Package Manager Console within Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

```bash
Install-Package DataMocks
```

## Examples

We have two MockDataReaderBuilder for now

### 1) SimpleMockDataReaderBuilder

You can create data reader with manually adding columns and the data

```C#
[TestMethod]
public void CanReadAllDataProperly()
{
    var builder = new SimpleMockDataReaderBuilder().
        SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
        AddColumn("ID_PRODUCT", typeof(long)).
        AddColumn("PRODUCT_NAME", typeof(string)).
        AddColumn("IS_ACTIVE", typeof(bool)).
        AddColumn("BARCODE", typeof(string)).
        AddData(1L, "Product 1", true, null);

    using (var reader = new MockDataReader(builder))
    {
        reader.Read().ShouldBe<bool>(true);
        reader.IsDBNull(0).ShouldBe<bool>(false);
        reader.GetString(1).ShouldBe<string>("Product 1");
        reader["IS_ACTIVE"].ShouldBe<object>(true);
        reader["BARCODE"].ShouldBe<object>(DBNull.Value);
    }
}
```

### 2) ObjectBasedMockDataReaderBuilder

You can create MockDataReader with `List<TEntity>`

```C#
private class TestObjectClass
{
    public int IntProperty { get; set; }
    public string StringProperty { get; set; }
}
```

```C#
[TestMethod]
public void CanReadAllDataProperly()
{
    var dataList = new List<TestObjectClass>();
    dataList.Add(new TestObjectClass { IntProperty = 10, StringProperty = "ten" });
    dataList.Add(new TestObjectClass { IntProperty = 20, StringProperty = "twenty" });
    dataList.Add(new TestObjectClass { IntProperty = 30, StringProperty = "thirty" });

    var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
        AddData(dataList);

    using (var reader = new MockDataReader(builder))
    {
        reader.Read(); // Seek the first item
        reader.Read(); // Seek the second item

        reader.GetInt32(0).ShouldBe<int>(20);
        reader.GetString(1).ShouldBe<string>("twenty");
    }
}
```
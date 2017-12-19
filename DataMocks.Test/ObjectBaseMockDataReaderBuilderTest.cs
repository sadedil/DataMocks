using DataMocks.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Collections.Generic;

namespace DataMocks.Test
{
    [TestClass]
    public class ObjectBaseMockDataReaderBuilderTest
    {
        private class TestObjectClass
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }

        [TestMethod]
        public void CanSuccessfullyAddColumns()
        {
            var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull);

            builder.GetColumns.Count.ShouldBe<int>(2);
        }

        [TestMethod]
        public void CanSuccessfullyAddData()
        {
            var dataList = new List<TestObjectClass>();
            dataList.Add(new TestObjectClass { IntProperty = 1, StringProperty = "one" });
            dataList.Add(new TestObjectClass { IntProperty = 2, StringProperty = "two" });
            dataList.Add(new TestObjectClass { IntProperty = 3, StringProperty = "three" });

            var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddData(dataList);

            builder.GetDataList.Count.ShouldBe<int>(3);
        }

        [TestMethod]
        public void CanReadAllDataProperly()
        {
            var dataList = new List<TestObjectClass>();
            dataList.Add(new TestObjectClass { IntProperty = 10, StringProperty = "ten" });
            dataList.Add(new TestObjectClass { IntProperty = 20, StringProperty = "twenty" });
            dataList.Add(new TestObjectClass { IntProperty = 30, StringProperty = "thirty" });

            var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddData(dataList);

            using (var reader = new MockDataReader(builder))
            {
                reader.Read().ShouldBe<bool>(true);
                reader.Read().ShouldBe<bool>(true);

                reader.IsDBNull(0).ShouldBe(false);
                reader.GetInt32(0).ShouldBe<int>(20);
                reader.GetValue(0).ShouldBe<object>(20);

                reader.IsDBNull(1).ShouldBe<bool>(false);
                reader.GetString(1).ShouldBe<string>("twenty");
                reader.GetValue(1).ShouldBe<object>("twenty");
            }
        }
    }
}
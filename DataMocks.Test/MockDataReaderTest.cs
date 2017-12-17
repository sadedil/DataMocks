using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataMocks;
using DataMocks.Builders;
using Shouldly;
using System;
using System.Collections.Generic;

namespace DataReaderToEntity.Test
{
    [TestClass]
    public class MockDataReaderTest
    {
        private class TestObjectClass
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }

        [TestMethod]
        public void ObjectBaseMockDataReaderBuilder_CanSuccessfullyAddColumns()
        {
            var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull);

            builder.Columns.Count.ShouldBe<int>(2);
        }

        [TestMethod]
        public void ObjectBaseMockDataReaderBuilder_CanSuccessfullyAddData()
        {
            var dataList = new List<TestObjectClass>();
            dataList.Add(new TestObjectClass { IntProperty = 1, StringProperty = "one" });
            dataList.Add(new TestObjectClass { IntProperty = 2, StringProperty = "two" });
            dataList.Add(new TestObjectClass { IntProperty = 3, StringProperty = "three" });

            var builder = new ObjectBasedMockDataReaderBuilder<TestObjectClass>().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddData(dataList);

            builder.DataList.Count.ShouldBe<int>(3);
        }

        [TestMethod]
        public void ObjectBaseMockDataReaderBuilder_CanReadAllDataProperly()
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimpleMockDataReaderBuilder_ShouldThrowError()
        {
            var builder = new SimpleMockDataReaderBuilder().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddColumn("Column1", typeof(long)).
                AddData("Value for column 1", "The value that can not corresponding to any column");
        }

        [TestMethod]
        public void SimpleMockDataReaderBuilder_CanReadAllDataProperly()
        {
            var builder = new SimpleMockDataReaderBuilder().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddColumn("ID_PRODUCT", typeof(long)).
                AddColumn("DS_PRODUCT", typeof(string)).
                AddColumn("FL_ACTIVE", typeof(bool)).
                AddColumn("DS_BARCODE", typeof(string)).
                AddData(1L, "Product 1", true, null);

            using (var reader = new MockDataReader(builder))
            {
                reader.Read();

                reader.IsDBNull(0).ShouldBe<bool>(false);
                reader.GetInt64(0).ShouldBe<long>(1);
                reader.GetValue(0).ShouldBe<object>(1);
                reader["ID_PRODUCT"].ShouldBe<object>(1);

                reader.IsDBNull(1).ShouldBe<bool>(false);
                reader.GetString(1).ShouldBe<string>("Product 1");
                reader.GetValue(1).ShouldBe<object>("Product 1");
                reader["DS_PRODUCT"].ShouldBe<object>("Product 1");

                reader.IsDBNull(2).ShouldBe<bool>(false);
                reader.GetBoolean(2).ShouldBe<bool>(true);
                reader.GetValue(2).ShouldBe<object>(true);
                reader["FL_ACTIVE"].ShouldBe<object>(true);

                reader.IsDBNull(3).ShouldBe<bool>(true);
                reader.GetValue(3).ShouldBe<object>(DBNull.Value);
                reader["DS_BARCODE"].ShouldBe<object>(DBNull.Value);
            }
        }
    }
}
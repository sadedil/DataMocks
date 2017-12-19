using DataMocks.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;

namespace DataMocks.Test
{
    [TestClass]
    public class SimpleMockDataReaderBuilderTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowErrorOnInconsistentColumnDataMapping()
        {
            var builder = new SimpleMockDataReaderBuilder().
                SetNullValueHandling(NullValueHandling.AssumeAsDbNull).
                AddColumn("Column1", typeof(long)).
                AddData("Value for column 1", "The value that can not corresponding to any column");
        }

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
                reader.Read();

                reader.IsDBNull(0).ShouldBe<bool>(false);
                reader.GetInt64(0).ShouldBe<long>(1);
                reader.GetValue(0).ShouldBe<object>(1);
                reader["ID_PRODUCT"].ShouldBe<object>(1);

                reader.IsDBNull(1).ShouldBe<bool>(false);
                reader.GetString(1).ShouldBe<string>("Product 1");
                reader.GetValue(1).ShouldBe<object>("Product 1");
                reader["PRODUCT_NAME"].ShouldBe<object>("Product 1");

                reader.IsDBNull(2).ShouldBe<bool>(false);
                reader.GetBoolean(2).ShouldBe<bool>(true);
                reader.GetValue(2).ShouldBe<object>(true);
                reader["IS_ACTIVE"].ShouldBe<object>(true);

                reader.IsDBNull(3).ShouldBe<bool>(true);
                reader.GetValue(3).ShouldBe<object>(DBNull.Value);
                reader["BARCODE"].ShouldBe<object>(DBNull.Value);
            }
        }
    }
}
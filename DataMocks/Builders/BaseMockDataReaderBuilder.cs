using System;
using System.Collections.Generic;
using System.Linq;

namespace DataMocks.Builders
{
    public abstract class BaseMockDataReaderBuilder<TBuilder> : IMockDataReaderBuilder
       where TBuilder : BaseMockDataReaderBuilder<TBuilder>
    {
        protected abstract TBuilder BuilderInstance { get; }

        public Dictionary<string, Type> Columns { get; private set; }
        public List<object[]> DataList { get; private set; }
        public NullValueHandling NullValueHandling { get; private set; } = NullValueHandling.AssumeAsIs;

        public BaseMockDataReaderBuilder()
        {
            this.Columns = new Dictionary<string, Type>();
            this.DataList = new List<object[]>();
        }

        private bool IsColumnsAdded => this.Columns.Any();
        private bool IsDataAdded => this.DataList.Any();

        public TBuilder SetNullValueHandling(NullValueHandling nullValueHandling)
        {
            this.NullValueHandling = nullValueHandling;
            return this.BuilderInstance;
        }

        public TBuilder AddColumn(string columnName, Type type)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (this.Columns.ContainsKey(columnName)) throw new ArgumentException("This column is already added", nameof(columnName));
            if (this.IsDataAdded) throw new InvalidOperationException("You cannot change columns after the data added");

            this.Columns.Add(columnName, type);
            return this.BuilderInstance;
        }

        public TBuilder AddData(params object[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!this.IsColumnsAdded) throw new InvalidOperationException("You cannot add data before the adding columns");
            if (this.Columns.Count != data.Length) throw new InvalidOperationException("Data size must be equal to columns size");

            this.DataList.Add(data);
            return this.BuilderInstance;
        }
    }
}
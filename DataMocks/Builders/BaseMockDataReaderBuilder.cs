using System;
using System.Collections.Generic;
using System.Linq;

namespace DataMocks.Builders
{
    public abstract class BaseMockDataReaderBuilder<TBuilder> : IMockDataReaderBuilder
       where TBuilder : BaseMockDataReaderBuilder<TBuilder>
    {
        protected abstract TBuilder BuilderInstance { get; }

        private Dictionary<string, Type> _columns;
        public IReadOnlyDictionary<string, Type> GetColumns => this._columns;

        private List<object[]> _dataList;
        public IReadOnlyList<object[]> GetDataList => this._dataList;

        public NullValueHandling NullValueHandling { get; private set; } = NullValueHandling.AssumeAsIs;

        public BaseMockDataReaderBuilder()
        {
            this._columns = new Dictionary<string, Type>();
            this._dataList = new List<object[]>();
        }

        private bool IsColumnsAdded => this._columns.Any();
        private bool IsDataAdded => this._dataList.Any();

        public TBuilder SetNullValueHandling(NullValueHandling nullValueHandling)
        {
            this.NullValueHandling = nullValueHandling;
            return this.BuilderInstance;
        }

        public TBuilder AddColumn(string columnName, Type type)
        {
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentNullException(nameof(columnName));
            if (this._columns.ContainsKey(columnName)) throw new ArgumentException("This column is already added", nameof(columnName));
            if (this.IsDataAdded) throw new InvalidOperationException("You cannot change columns after the data added");

            this._columns.Add(columnName, type);
            return this.BuilderInstance;
        }

        public TBuilder AddData(params object[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!this.IsColumnsAdded) throw new InvalidOperationException("You cannot add data before the adding columns");
            if (this._columns.Count != data.Length) throw new InvalidOperationException("Data size must be equal to columns size");

            this._dataList.Add(data);
            return this.BuilderInstance;
        }
    }
}
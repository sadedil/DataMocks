using DataMocks.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataMocks
{
    public class MockDataReader : IDataReader
    {
        private int currentPosition = -1;
        private object[] currentData = null;

        private bool isClosed = false;
        private bool isDisposed = false;

        private Dictionary<string, int> columnNameToOrdinalMapping = null;
        private Dictionary<int, string> ordinalToColumnNameMapping = null;

        #region Builder Parameters

        public Dictionary<string, Type> Columns { get; private set; }
        public List<object[]> DataList { get; private set; }
        public NullValueHandling NullValueHandling { get; private set; } = NullValueHandling.AssumeAsIs;

        #endregion Builder Parameters

        public MockDataReader(IMockDataReaderBuilder builder)
        {
            this.columnNameToOrdinalMapping = new Dictionary<string, int>();
            this.ordinalToColumnNameMapping = new Dictionary<int, string>();

            this.NullValueHandling = builder.NullValueHandling;
            this.Columns = builder.Columns;
            this.DataList = builder.DataList;

            this.Columns.Keys.
                Select((k, i) => new { ColumnName = k, Ordinal = i }).ToList().
                ForEach(item =>
                {
                    this.columnNameToOrdinalMapping.Add(item.ColumnName, item.Ordinal);
                    this.ordinalToColumnNameMapping.Add(item.Ordinal, item.ColumnName);
                });
        }

        private void ThrowExceptionIfDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(MockDataReader), "Reader is disposed. This operation is not supported");
        }

        public object this[int i]
        {
            get
            {
                this.ThrowExceptionIfDisposed();
                var result = this.currentData[i];

                if (result == null && this.NullValueHandling == NullValueHandling.AssumeAsDbNull)
                    result = DBNull.Value;

                return result;
            }
        }

        public object this[string name] => this[this.columnNameToOrdinalMapping[name]];

        public int Depth => 0;

        public bool IsClosed => this.isClosed;

        public int RecordsAffected => -1;

        public int FieldCount => this.Columns.Count;

        public void Close()
        {
            this.isClosed = true;
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        public bool GetBoolean(int i)
        {
            return (bool)this[i];
        }

        public byte GetByte(int i)
        {
            return (byte)this[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            return (char)this[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            return this.Columns[this.ordinalToColumnNameMapping[i]].ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)this[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)this[i];
        }

        public double GetDouble(int i)
        {
            return (double)this[i];
        }

        public Type GetFieldType(int i)
        {
            return this[i].GetType();
        }

        public float GetFloat(int i)
        {
            return (float)this[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)this[i];
        }

        public short GetInt16(int i)
        {
            return (Int16)this[i];
        }

        public int GetInt32(int i)
        {
            return (Int32)this[i];
        }

        public long GetInt64(int i)
        {
            return (Int64)this[i];
        }

        public string GetName(int i)
        {
            return this.ordinalToColumnNameMapping[i];
        }

        public int GetOrdinal(string name)
        {
            return this.columnNameToOrdinalMapping[name];
        }

        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public string GetString(int i)
        {
            return (string)this[i];
        }

        public object GetValue(int i)
        {
            return this[i];
        }

        public int GetValues(object[] values)
        {
            for (int i = 0; i < this.currentData.Length; i++)
            {
                values[i] = this.currentData[i];
            }
            return this.currentData.Length;
        }

        public bool IsDBNull(int i)
        {
            return DBNull.Value.Equals(this[i]);
        }

        /// <summary>
        /// We are not supporting MARS
        /// This method always returns false
        /// </summary>
        /// <returns></returns>
        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            this.ThrowExceptionIfDisposed();

            if (!this.DataList.Any())
                return false;

            if (this.currentPosition >= this.DataList.Count - 1)
                return false;

            this.currentPosition++;
            this.currentData = this.DataList[this.currentPosition];

            return true;
        }
    }
}
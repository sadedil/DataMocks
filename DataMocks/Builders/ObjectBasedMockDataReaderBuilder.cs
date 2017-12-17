using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataMocks.Builders
{
    public class ObjectBasedMockDataReaderBuilder<T> : BaseMockDataReaderBuilder<ObjectBasedMockDataReaderBuilder<T>>
    {
        protected override ObjectBasedMockDataReaderBuilder<T> BuilderInstance => this;

        public Type Type { get; private set; }

        public PropertyInfo[] PropertyInformations { get; private set; }

        public ObjectBasedMockDataReaderBuilder() : base()
        {
            this.Type = typeof(T);
            this.PropertyInformations = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in this.PropertyInformations)
            {
                this.AddColumn(pi.Name, pi.GetType());
            }
        }

        public ObjectBasedMockDataReaderBuilder<T> AddData(IEnumerable<T> dataList)
        {
            var type = typeof(T);
            foreach (var data in dataList)
            {
                List<object> objectData = new List<object>();
                foreach (var pi in this.PropertyInformations)
                {
                    objectData.Add(pi.GetValue(data));
                }
                this.AddData(objectData.ToArray());
            }
            return this.BuilderInstance;
        }
    }
}
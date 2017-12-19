using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataMocks.Builders
{
    public class ObjectBasedMockDataReaderBuilder<TObject> : BaseMockDataReaderBuilder<ObjectBasedMockDataReaderBuilder<TObject>>
    {
        protected override ObjectBasedMockDataReaderBuilder<TObject> BuilderInstance => this;

        private Type TypeOfGenericObject { get; set; }

        private PropertyInfo[] PropertyInformations { get; set; }

        public ObjectBasedMockDataReaderBuilder() : base()
        {
            this.TypeOfGenericObject = typeof(TObject);
            this.PropertyInformations = this.TypeOfGenericObject.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in this.PropertyInformations)
            {
                this.AddColumn(pi.Name, pi.GetType());
            }
        }

        public ObjectBasedMockDataReaderBuilder<TObject> AddData(IEnumerable<TObject> dataList)
        {
            var type = typeof(TObject);
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
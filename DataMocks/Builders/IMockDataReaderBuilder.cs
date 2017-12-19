using System;
using System.Collections.Generic;

namespace DataMocks.Builders
{
    public interface IMockDataReaderBuilder
    {
        IReadOnlyDictionary<string, Type> GetColumns { get; }
        IReadOnlyList<object[]> GetDataList { get; }
        NullValueHandling NullValueHandling { get; }
    }
}
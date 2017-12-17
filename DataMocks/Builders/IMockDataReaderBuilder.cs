using System;
using System.Collections.Generic;

namespace DataMocks.Builders
{
    public interface IMockDataReaderBuilder
    {
        Dictionary<string, Type> Columns { get; }
        List<object[]> DataList { get; }
        NullValueHandling NullValueHandling { get; }
    }
}
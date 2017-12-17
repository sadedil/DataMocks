namespace DataMocks
{
    public enum NullValueHandling
    {
        /// <summary>
        /// Assume all null values as "null"
        /// This is the default behaviour
        /// </summary>
        AssumeAsIs,

        /// <summary>
        /// Assume all null values as "DbNull.Value"
        /// </summary>
        AssumeAsDbNull,
    }
}
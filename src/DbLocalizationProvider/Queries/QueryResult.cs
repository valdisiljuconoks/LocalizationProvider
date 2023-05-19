namespace DbLocalizationProvider.Queries
{
    public class QueryResult<TResult> 
    {
        public QueryResult()
        {
        }

        public QueryResult(TResult result, long rowCount)
        {
            Result = result;
            RowCount = rowCount;
        }


        public long? RowCount { get; }

        public TResult Result { get; }
    }
}

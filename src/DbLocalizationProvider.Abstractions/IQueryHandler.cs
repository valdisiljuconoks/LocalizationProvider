namespace DbLocalizationProvider
{
    public interface IQueryHandler<in TQuery, out TResult>
    {
        TResult Execute(TQuery query);
    }
}

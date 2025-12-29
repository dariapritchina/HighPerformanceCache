namespace Cache.Domain.Interfaces;

public interface IStatisticStore
{
    public (long setCount, long getCount, long deleteCount) GetStatistic();
}
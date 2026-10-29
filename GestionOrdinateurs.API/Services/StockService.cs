namespace GestionOrdinateurs.Services;

public class StockService
{
    private object _lock = new object();
    public int StockSSD = 10;
    public int getStockSSD
    {
        get
        {
            lock (_lock)
            {
                return StockSSD;
            }
        }
    }
    public Task ConsommerSSD()
    {
        Task.Delay(10).Wait();
        //wait avant le lock
        lock(_lock)
        {
            if (StockSSD > 0)
            {

                StockSSD--;
            }
        }
        return Task.CompletedTask;
    }
}
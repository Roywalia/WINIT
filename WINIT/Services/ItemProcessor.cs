using Microsoft.EntityFrameworkCore;
using WINIT.Models;

namespace WINIT.Services;
public interface IItemProcessor
{
    Task ProcessAsync();
    Task ProcessSingleAsync(Guid logId);
}

public class ItemProcessor:IItemProcessor
{
    private readonly AppDbContext _db;

    public ItemProcessor(AppDbContext db) => _db = db;

    public async Task ProcessAsync()
    {
        var pending = await _db.LogItemIngestions.AsNoTracking().Where(x => x.Status == "SUCCESS" && x.ProcessStatus == "PENDING").ToListAsync();

        foreach (var log in pending)
            await ProcessSingleAsync(log.LogId);
    }

    public async Task ProcessSingleAsync(Guid logId)
    {
        var log = await _db.LogItemIngestions.FindAsync(logId);
        if (log == null) return;

        try
        {
            log.ProcessError = "PROCESSED";
        }
        catch (Exception ex)
        {
            log.ProcessStatus = "ERROR";
            log.ProcessError = ex.Message;
        }
        await _db.SaveChangesAsync();
    }
}

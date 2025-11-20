using Microsoft.EntityFrameworkCore;
using WINIT.Models;

namespace WINIT.Services;

public interface ICustomerProcessor
{
    Task ProcessAsync();
    Task ProcessSingleAsync(Guid logId);
}
public class CustomerProcessor:ICustomerProcessor
{
    private readonly AppDbContext _db;

    public CustomerProcessor(AppDbContext db)=> _db=db;

    public async Task ProcessAsync()
    {
        var pending = await _db.LogCustomerIngestions.AsNoTracking().Where(x => x.Status == "SUCCESS" && x.ProcessStatus == "PENDING").ToListAsync();

        foreach(var log in pending)        
            await ProcessSingleAsync(log.LogId);        
    }

    public async Task ProcessSingleAsync(Guid logId)
    {
        var log = await _db.LogCustomerIngestions.FindAsync(logId);
        if (log == null) return;

        try
        {
            log.ProcessError = "PROCESSED";
        }
        catch(Exception ex)
        {
            log.ProcessStatus = "ERROR";
            log.ProcessError = ex.Message;
        }
        await _db.SaveChangesAsync();
    }
}

using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WINIT.Models;
using WINIT.Services;
using static WINIT.Models.AppDbContext;

namespace WINIT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class IngestController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IIngestionService _validator;
        private readonly IBackgroundJobClient _backgroundJob;

        public IngestController(AppDbContext db, IIngestionService validator, IBackgroundJobClient backgroundJob)
        {
            _db = db;
            _validator = validator;
            _backgroundJob = backgroundJob;
        }
        [HttpPost("customers/ingest")]
        public async Task<IActionResult> IngestCustomer([FromBody] IncomingCustomer customer)
        {
            var log = new CustomerIngestionLog
            {
                RequestTime=DateTime.UtcNow,
                RawPayload=JsonConvert.SerializeObject(customer),
            };

            var validation= _validator.ValidateCustomer(customer);

            if (!validation.isValid)
            {
                log.HttpStatus = 400;
                log.Status = "VALIDATION_FAILED";
                log.ValidatonDetails = JsonConvert.SerializeObject(validation.Errors);

                _db.LogCustomerIngestions.Add(log);
                await _db.SaveChangesAsync();

                return BadRequest(new
                {
                    status = "Failure",
                    message = "Validaton failed for one of momre fields.",
                    validationErrors = validation.Errors
                });
            }

            log.HttpStatus = 202;
            log.Status = "SUCCESS";
            _db.LogCustomerIngestions.Add(log);
            await _db.SaveChangesAsync();

            var refId = _validator.GenerateReferenceId("INGEST-C");

            _backgroundJob.Enqueue<ICustomerProcessor>(p=> p.ProcessSingleAsync(log.LogId));

            return Accepted(new
            {
                status="SUCCESS",
                message="Customer data received and queued for processing",
                referenceId=$"{refId}-{log.LogId}",
            });
        }
        [HttpPost("items/ingest")]
        public async Task<IActionResult> IngestItem([FromBody] MaterialWrapper wrapper)
        {
            var material = wrapper.material;
            var log = new ItemIngestionLog
            {
                RequestTime = DateTime.UtcNow,
                RawPayload = JsonConvert.SerializeObject(wrapper)
            };

            var validation = _validator.ValidateItem(material);

            if (!validation.isValid)
            {
                log.HttpStatus = 400;
                log.Status = "VALIDATION_FAILED";
                log.ValidatonDetails = JsonConvert.SerializeObject(validation.Errors);

                _db.LogItemIngestions.Add(log);
                await _db.SaveChangesAsync();

                return BadRequest(new
                {
                    status = "Failure",
                    message = "Validaton failed for one of momre fields.",
                    validationErrors = validation.Errors
                });
            }

            log.HttpStatus = 202;
            log.Status = "SUCCESS";
            _db.LogItemIngestions.Add(log);
            await _db.SaveChangesAsync();

            var refId = _validator.GenerateReferenceId("INGEST-I");

            _backgroundJob.Enqueue<IItemProcessor>(p => p.ProcessSingleAsync(log.LogId));

            return Accepted(new
            {
                status = "SUCCESS",
                message = "Item data received and queued for processing",
                referenceId = $"{refId}-{log.LogId}",
            });
        }
    }
}

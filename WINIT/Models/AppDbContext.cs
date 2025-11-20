using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace WINIT.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CustomerIngestionLog> LogCustomerIngestions { get; set; } = null!;
    public DbSet<ItemIngestionLog> LogItemIngestions { get; set; } = null!;
    public DbSet<MRegion> MRegions { get; set; } = null!;
    public DbSet<MCity> MCities { get; set; } = null!;
    public DbSet<MPaymentTerm> MPaymentTerms { get; set; } = null!;
    public DbSet<MChannel> MChannels { get; set; } = null!;
    public DbSet<TCustomer> TCustomers { get; set; } = null!;
    public DbSet<MBrand> MBrands { get; set; } = null!;
    public DbSet<MCategory> MCategories { get; set; } = null!;
    public DbSet<MUom> MUoms { get; set; } = null!;
    public DbSet<TItem> TItems { get; set; } = null!;
    public DbSet<TItemUomConversion> TItemUomConversions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelbuilder)
    {
        modelbuilder.Entity<CustomerIngestionLog>()
        .HasKey(c => c.LogId);
        modelbuilder.Entity<ItemIngestionLog>()
        .HasKey(c => c.LogId);
        base.OnModelCreating(modelbuilder);
    }

    public class CustomerIngestionLog
    {
        public Guid LogId { get; set; } = Guid.NewGuid();
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public string RawPayload { get; set; } = null!;
        public int HttpStatus { get; set; }
        public string Status { get; set; } = null!;

        public string? ValidatonDetails { get; set; }
        public string ProcessStatus { get; set; } = "PENDING";
        public string? ProcessError { get; set; }
    }
    public class ItemIngestionLog
    {
        public Guid LogId { get; set; } = Guid.NewGuid();
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public string RawPayload { get; set; } = null!;
        public int HttpStatus { get; set; }
        public string Status { get; set; } = null!;

        public string? ValidatonDetails { get; set; }
        public string ProcessStatus { get; set; } = "PENDING";
        public string? ProcessError { get; set; }
    }
    public class MRegion
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class MCity
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class MPaymentTerm
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class MChannel
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class TCustomer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = null!;
        public string CustomerName { get; set; } = null!;

        public string ArabicDescription { get; set; } = null!;
        public string ParentCustomerCode { get; set; } = null!;
        public string ParentCustomerName { get; set; } = null!;
        public string ContactNo { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address1 { get; set; } = null!;
        public string Address2 { get; set; } = null!;
        public string Address3 { get; set; } = null!;
        public string Address4 { get; set; } = null!;
        public bool isActive { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string ContactPersonName { get; set; } = null!;
        public string CityCode { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string CityNameArabic { get; set; } = null!;
        public string RegionCode { get; set; } = null!;
        public string RegionName { get; set; } = null!;
        public string PriceListCode { get; set; } = null!;
        public string CustomerGroupCode { get; set; } = null!;
        public string CustomerGroupName { get; set; } = null!;
        public decimal CreditLimit { get; set; }
        public int CreditDays { get; set; } = 0;
        public string PaymentTermName { get; set; } = null!;
        public string PaymentTermCode { get; set; } = null!;
        public string CustomerType { get; set; } = null!;
        public string ChannelCode { get; set; } = null!;
        public string ChannelName { get; set; } = null!;
        public string SubChannelCode { get; set; } = null!;
        public string SubChannelName { get; set; } = null!;
        public bool isBlocked { get; set; }
    }
    public class MBrand
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class MCategory
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    public class MUom
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
    }
    public class TItem
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string ItemName { get; set; } = null!;
    }
    public class TItemUomConversion
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UomId { get; set; }
        public decimal ConversionFactor { get; set; }
    }

}


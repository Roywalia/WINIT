using WINIT.Models;

namespace WINIT.Services;

public class IngestionService:IIngestionService
{
    public ValidationResult ValidateCustomer(IncomingCustomer c)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(c.customerCode))
        {
            errors.Add(new("customerCode", "Required"));
        }
        if (string.IsNullOrWhiteSpace(c.customerName))
        {
            errors.Add(new("customerName", "Required"));
        }
        if (string.IsNullOrWhiteSpace(c.contactNo))
        {
            errors.Add(new("customerNo", "Required"));
        }
        if (string.IsNullOrWhiteSpace(c.email) || !c.email.Contains("@"))
        {
            errors.Add(new("email", "Invalid email"));
        }
        if (string.IsNullOrWhiteSpace(c.cityCode))
        {
            errors.Add(new("cityCode", "Required"));
        }
        if (string.IsNullOrWhiteSpace(c.regionCode))
        {
            errors.Add(new("regionCode", "Required"));
        }
        if (string.IsNullOrWhiteSpace(c.customerType))
        {
            errors.Add(new("customerType", "Required"));
        }
        if (c.customerType!="CASH" && (c.creditLimit<=0 || c.creditDays<=0))
        {
            errors.Add(new("creditLimit/creditDays", "Mumst be > 0 for non-CASH customers"));
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    public ValidationResult ValidateItem(IncomingMaterial i)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(i.itemCode))
        {
            errors.Add(new("itemCode", "Required"));
        }
        if (string.IsNullOrWhiteSpace(i.itemName))
        {
            errors.Add(new("itemName", "Required"));
        }
        if (string.IsNullOrWhiteSpace(i.baseUOM))
        {
            errors.Add(new("baseUOM", "Required"));
        }
        if(i.uomList==null || i.uomList.Count==0)
        {
            errors.Add(new("uomList", "At least one UOM entry is required"));
        }
        foreach(var u in i.uomList ?? new())
        {
            if (string.IsNullOrWhiteSpace(u.uom))
            {
                errors.Add(new("uomList.uom", "UOM code Required"));
            }
            if (u.conversionFactor<=0)
            {
                errors.Add(new("uomList.conversonFactor", "Must be > 0"));
            }
        }
        return new ValidationResult(errors.Count == 0, errors);
    }

    public string GenerateReferenceId(string prefix) => $"{prefix}-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 30);
    
}

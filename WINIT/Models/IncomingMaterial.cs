namespace WINIT.Models;

public record UomEntry
(
    string uom,
    decimal conversionFactor
);

public record IncomingMaterial
(
    string itemCode,
    string itemName,
    string arabicDescription,
    string salesOrgCode,
    string baseUOM,
    string brandCode,
    string brandName,
    string categoryCode,
    string categoryName,
    bool isActive,
    bool isBatchEnabled,
    string businessType,
    string businessTypeDescription,
    List<UomEntry> uomList
);

public record MaterialWrapper(
    IncomingMaterial material    
);
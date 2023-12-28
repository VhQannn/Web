using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class SupporterInsurancePackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public int Duration { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<UserSupporterInsurance> UserSupporterInsurances { get; set; } = new List<UserSupporterInsurance>();
}

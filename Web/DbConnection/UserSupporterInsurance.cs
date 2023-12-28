using System;
using System.Collections.Generic;

namespace Web.DbConnection;

public partial class UserSupporterInsurance
{
    public int UserSupporterInsuranceId { get; set; }

    public int UserId { get; set; }

    public int PackageId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual SupporterInsurancePackage Package { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

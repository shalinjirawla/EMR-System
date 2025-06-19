using Abp.Authorization.Users;
using Abp.Extensions;
using EMRSystem.Billings;
using EMRSystem.Doctors;
using EMRSystem.LabReports;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Pharmacists;
using System;
using System.Collections.Generic;

namespace EMRSystem.Authorization.Users;

public class User : AbpUser<User>
{
    public const string DefaultPassword = "123qwe";

    public static string CreateRandomPassword()
    {
        return Guid.NewGuid().ToString("N").Truncate(16);
    }

    public static User CreateTenantAdminUser(int tenantId, string emailAddress)
    {
        var user = new User
        {
            TenantId = tenantId,
            UserName = AdminUserName,
            Name = AdminUserName,
            Surname = AdminUserName,
            EmailAddress = emailAddress,
            Roles = new List<UserRole>()
        };

        user.SetNormalizedNames();

        return user;
    }
    public ICollection<Patient> Patients { get; set; }
    public ICollection<Bill> Bills { get; set; }
    public ICollection<Doctor> Doctors { get; set; }
    public ICollection<Nurse> Nurses { get; set; }
    public ICollection<Pharmacist> Pharmacists{ get; set; }

    public ICollection<LabTechnician> LabTechnicians { get; set; }
}

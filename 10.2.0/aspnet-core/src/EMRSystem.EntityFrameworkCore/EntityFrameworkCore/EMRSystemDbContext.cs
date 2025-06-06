using Abp.Zero.EntityFrameworkCore;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Billings;
using EMRSystem.Doctors;
using EMRSystem.LabReports;
using EMRSystem.MultiTenancy;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using EMRSystem.Vitals;
using Microsoft.EntityFrameworkCore;

namespace EMRSystem.EntityFrameworkCore;

public class EMRSystemDbContext : AbpZeroDbContext<Tenant, Role, User, EMRSystemDbContext>
{
    /* Define a DbSet for each entity of the application */
    public DbSet<Bill> Billing { get; set; }
    public DbSet<BillItem> BillItem { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<LabReport> Lab { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Vital> Vitals { get; set; }
    public EMRSystemDbContext(DbContextOptions<EMRSystemDbContext> options)
        : base(options)
    {}    
}

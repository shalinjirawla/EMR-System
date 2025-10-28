using Abp.Domain.Entities;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BirthRecord
{
    public class BirthRecord : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        // 🔹 Mother Reference
        public long MotherId { get; set; } // PatientId reference
        public Patient Mother { get; set; }


        // 🔹 Baby Details
        public GenderType Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime BirthTime { get; set; } // e.g. "03:25 PM"
        public decimal? BirthWeight { get; set; }
        public BirthType BirthType { get; set; }
        public DeliveryType DeliveryType { get; set; }

        // 🔹 Medical Staff
        public long? DoctorId { get; set; } // DoctorMaster reference
        public Doctor Doctor { get; set; }
        public long? NurseId { get; set; } // NurseMaster reference
        public Nurse Nurse { get; set; }

        // 🔹 Condition
        public bool IsStillBirth { get; set; }

        // 🔹 Father Information
        public string? FatherName { get; set; }

        // 🔹 Additional Info
        public string? Notes { get; set; } // for remarks like IVF, unknown father, etc.
    }

    // 🔸 Enums
    public enum GenderType
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum BirthType
    {
        Single = 1,
        Twins = 2,
        Triplets = 3,
        Multiple = 4
    }

    public enum DeliveryType
    {
        Normal = 1,
        Caesarean = 2
    }
}

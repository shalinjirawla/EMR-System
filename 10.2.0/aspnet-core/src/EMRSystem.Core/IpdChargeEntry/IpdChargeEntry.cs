﻿using Abp.Domain.Entities;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.IpdChargeEntry
{
    public class IpdChargeEntry : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long AdmissionId { get; set; }
        public long PatientId { get; set; }
        public ChargeType ChargeType { get; set; }
        public string Description { get; set; } 
        public decimal Amount { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsProcessed { get; set; } = false;
        public long? ReferenceId { get; set; }
        public virtual Admission.Admission Admission { get; set; }
        public virtual Patient Patient { get; set; }
    }

    public enum ChargeType
    {
        Appointment,
        LabTest,
        Medicine,
        Procedure,
        Other
    }
}

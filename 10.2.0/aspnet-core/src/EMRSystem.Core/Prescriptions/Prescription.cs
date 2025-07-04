﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Doctors;
using EMRSystem.LabReports;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions
{
    public class Prescription : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public bool IsFollowUpRequired { get; set; }
        public long AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public ICollection<PrescriptionItem> Items { get; set; }
        public virtual ICollection<PrescriptionLabTest> LabTests { get; set; }
    }
}

﻿using Abp.Application.Services.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor.Dto;
using EMRSystem.Invoices;
using EMRSystem.Nurse.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Room;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        //public long? RoomId { get; set; }
        //public string? RoomNumber { get; set; }
        //public int? Floor { get; set; }
        //public RoomStatus? RoomStatus { get; set; }
        //public string? RoomTypeName { get; set; }

        //public long? AssignedNurseId { get; set; }
        //public BillingMethod BillingMethod { get; set; }
        //public PaymentMethod? PaymentMethod { get; set; }
        //public long? DepositAmount { get; set; }

        public bool IsAdmitted { get; set; }
        //public DateTime? AdmissionDate { get; set; }
        //public DateTime? DischargeDate { get; set; }
        //public string? InsuranceProvider { get; set; }
        //public string? InsurancePolicyNumber { get; set; }
        public UserDto AbpUser { get; set; }
        public DoctorDto Doctors { get; set; }
        public NurseDto Nurses { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }
        //public List<VitalDto> Vitals { get; set; }
    }
}

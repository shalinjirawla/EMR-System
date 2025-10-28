using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BirthRecord.Dto
{
    public class CreateUpdateBirthRecordDto : EntityDto<long>
    {
        public int? TenantId { get; set; }
        public long MotherId { get; set; }
        public GenderType Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime BirthTime { get; set; }
        public decimal? BirthWeight { get; set; }
        public BirthType BirthType { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public long? DoctorId { get; set; }
        public long? NurseId { get; set; }
        public bool IsStillBirth { get; set; }
        public string? FatherName { get; set; }
        public string? Notes { get; set; }
    }
}

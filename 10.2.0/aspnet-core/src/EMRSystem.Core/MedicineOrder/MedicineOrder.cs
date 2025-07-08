using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder
{
    public class MedicineOrder : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required]
        public long NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }

        public long? PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public OrderPriority Priority { get; set; }

        public virtual ICollection<MedicineOrderItem> Items { get; set; }

        public MedicineOrder()
        {
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
            Items = new List<MedicineOrderItem>();
        }
    }

    public enum OrderStatus
    {
        Pending,
        Dispensed
    }

    public enum OrderPriority
    {
        Low,
        Medium,
        High
    }
}

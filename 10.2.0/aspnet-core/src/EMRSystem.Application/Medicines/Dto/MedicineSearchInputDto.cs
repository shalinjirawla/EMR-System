using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineSearchInputDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

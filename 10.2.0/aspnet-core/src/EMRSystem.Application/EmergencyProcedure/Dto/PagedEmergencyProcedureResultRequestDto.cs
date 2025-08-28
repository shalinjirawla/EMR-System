using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class PagedEmergencyProcedureResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public ProcedureCategory? Category { get; set; }   // Nullable so "All" works
    }
}

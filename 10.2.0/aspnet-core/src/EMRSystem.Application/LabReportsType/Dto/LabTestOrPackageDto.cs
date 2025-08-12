using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class LabTestOrPackageDto
    {
        public long Id { get; set; }
        public string Name { get; set; }  // Test name or Package name
        public decimal Price { get; set; }
        public string Type { get; set; } // "Test" or "Package"
        public List<long> PackageTestIds { get; set; }
    }

}

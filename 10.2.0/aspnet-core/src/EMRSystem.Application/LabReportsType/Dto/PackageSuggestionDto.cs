using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class PackageSuggestionDto
    {
        public long PackageId { get; set; }
        public string PackageName { get; set; }

        // List of all test IDs in the package
        public List<long> IncludedTests { get; set; } = new List<long>();

        // Total price of the selected tests that match this package
        public decimal SelectedTestsPrice { get; set; }

        // Price of the whole package
        public decimal PackagePrice { get; set; }

        // ✅ New: how many selected tests are included in this package
        public int MatchCount { get; set; }

        // ✅ New: how many tests the user selected in total
        public int TotalSelectedCount { get; set; }

        // ✅ Extra tests included in the package that the user didn’t select
        public List<long> ExtraTests { get; set; } = new List<long>();
    }


}

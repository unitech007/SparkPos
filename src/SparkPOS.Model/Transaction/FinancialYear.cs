using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SparkPOS.Model.Transaction
{
    [Table("FinancialYear")]
    public partial class FinancialYear 
    {
        public string financial_year_id { get; set; }

        [Required]
        [StringLength(10)]
        public string FiscalYearCode { get; set; }

        [Required]
        [StringLength(100)]
        public string FiscalYearName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}

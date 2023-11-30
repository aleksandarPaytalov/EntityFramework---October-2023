using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class ImportEmployeesDto
    {
        [JsonProperty("Username")]
        [JsonRequired]
        [RegularExpression(@"^[A-Za-z0-9]{3,40}$")]
        public string Username { get; set; } = null!;

        [JsonProperty("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [JsonProperty("Phone")]
        [Required]
        [RegularExpression(@"^\d{3}\-\d{3}\-\d{4}$")]
        public string Phone { get; set; } = null!;

        [JsonProperty("Tasks")]
        public int[] Tasks { get; set; } = null!;
    }
}

//Employee
//    •	Id – integer, Primary Key
//    •	Username – text with length [3, 40]. Should contain only lower or upper case letters and/or digits. (required)
//•	Email – text (required). Validate it! There is attribute for this job.
//•	Phone – text. Consists only of three groups (separated by '-'), the first two consist of three digits and the last one – of 4 digits. (required)
//•	EmployeesTasks – collection of type EmployeeTask

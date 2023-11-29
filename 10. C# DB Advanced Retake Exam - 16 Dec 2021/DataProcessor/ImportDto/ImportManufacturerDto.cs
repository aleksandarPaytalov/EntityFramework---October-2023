using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Manufacturer")]
    public class ImportManufacturerDto
    {
        [Required]
        [XmlElement("ManufacturerName")]
        [MinLength(4)]
        [MaxLength(40)]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [XmlElement("Founded")]
        [MinLength(10)]
        [MaxLength(100)]
        public string Founded { get; set; } = null!;

    }
}


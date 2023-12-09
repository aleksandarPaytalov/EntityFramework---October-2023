﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ImportDto
{
    [XmlType("Purchase")]
    public class ImportPurchaseDto
    {
        [XmlAttribute("title")]
        [Required]
        public string title { get; set; }

        [XmlElement("Type")]
        [Required]
        public string Type { get; set; }

        [XmlElement("Key")]
        [Required]
        [RegularExpression("^[A-Z0-9]{4}\\-[A-Z0-9]{4}\\-[A-Z0-9]{4}$")]
        public string Key { get; set; }

        [XmlElement("Card")]
        [Required]
        [RegularExpression("^(\\d{4})\\s(\\d{4})\\s(\\d{4})\\s(\\d{4})$")]
        public string Card { get; set; }

        [XmlElement("Date")]
        [Required]
        public string Date { get; set; }
    }
}

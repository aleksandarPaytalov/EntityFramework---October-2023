using System.Xml.Serialization;
using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ExportDto;

[XmlType("Invoice")]
public class ExportInvoiceDto
{
    [XmlElement("InvoiceNumber")]
    public int InvoiceNumber { get; set; }

    [XmlElement("InvoiceAmount")]
    public decimal InvoiceAmount { get; set; }

    [XmlElement("DueDate")]
    public string DueDate { get; set; }

    [XmlElement("Currency")]
    public CurrencyType Currency { get; set; }
}


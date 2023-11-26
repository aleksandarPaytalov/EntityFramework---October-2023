using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto;

[XmlType("Client")]
public class ExportClientDto
{
    [XmlElement("ClientName")]
    public string ClientName { get; set; }

    [XmlElement("VatNumber")]
    public string VatNumber { get; set; }

    [XmlAttribute("InvoicesCount")]
    public int InvoicesCount { get; set; }

    [XmlArray("Invoices")]
    public ExportInvoiceDto[] Invoices { get; set; }
}


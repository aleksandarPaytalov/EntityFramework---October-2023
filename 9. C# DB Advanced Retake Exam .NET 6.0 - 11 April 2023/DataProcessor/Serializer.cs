using Footballers.Utilities;
using Invoices.Data.Models.Enums;
using Invoices.DataProcessor.ExportDto;
using Newtonsoft.Json;

namespace Invoices.DataProcessor;

using Data;
using Microsoft.VisualBasic;

public class Serializer
{
    public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
    {
        XmlHelper xmlHelper = new XmlHelper();

        var clients = context.Clients
            .Where(c => c.Invoices.Any(i => i.IssueDate > date))
            .Select(c => new ExportClientDto
            {
                InvoicesCount = c.Invoices.Count,
                ClientName = c.Name,
                VatNumber = c.NumberVat,
                Invoices = c.Invoices
                    .OrderBy(i => i.IssueDate)
                    .ThenByDescending(i => i.DueDate)
                    .Select(i => new ExportInvoiceDto
                    {
                        InvoiceNumber = i.Number,
                        InvoiceAmount = i.Amount,
                        DueDate = i.DueDate.ToString("MM/dd/yyyy"),
                        Currency = i.CurrencyType
                    })
                    .ToArray()
            })
            .OrderByDescending(c => c.Invoices.Length)
            .ThenBy(c => c.ClientName)
            .ToArray();

        return xmlHelper.Serialize(clients, "Clients");
    }

    public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var productsWithMostClients = context.Products
                .Where(p => p.ProductsClients.Any(c => c.Client.Name.Length >= nameLength))
                .ToArray()
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                        .Where(c => c.Client.Name.Length >= nameLength)
                        .Select(c => new
                        {
                            Name = c.Client.Name,
                            NumberVat = c.Client.NumberVat
                        })
                        .OrderBy(c => c.Name)
                        .ToArray()
                })
                .OrderByDescending(p => p.Clients.Length)
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(productsWithMostClients, Formatting.Indented);
        }
}

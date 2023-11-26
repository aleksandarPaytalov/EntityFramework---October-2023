using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor;

using System.Text;
using Newtonsoft.Json;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

using Data;
using Data.Models;
using ImportDto;
using Footballers.Utilities;

public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfullyImportedClients
        = "Successfully imported client {0}.";

    private const string SuccessfullyImportedInvoices
        = "Successfully imported invoice with number {0}.";

    private const string SuccessfullyImportedProducts
        = "Successfully imported product - {0} with {1} clients.";


    public static string ImportClients(InvoicesContext context, string xmlString)
    {
        XmlHelper xmlHelper = new XmlHelper();
        var clientsDto = xmlHelper.Deserialize<ImportClientDto[]>(xmlString, "Clients");

        StringBuilder sb = new StringBuilder();
        HashSet<Client> validClients = new HashSet<Client>();
        foreach (var client in clientsDto)
        {
            if (!IsValid(client))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Client validClient = new Client()
            {
                Name = client.Name,
                NumberVat = client.NumberVat
            };
            validClients.Add(validClient);

            foreach (var address in client.Addresses)
            {
                if (!IsValid(address))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(address.StreetName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Address validAddress = new Address()
                {
                    StreetName = address.StreetName,
                    StreetNumber = address.StreetNumber,
                    PostCode = address.PostCode,
                    City = address.City,
                    Country = address.Country
                };

                validClient.Addresses.Add(validAddress);
            }
            
            sb.AppendLine(string.Format(SuccessfullyImportedClients, validClient.Name));
        }

        context.AddRange(validClients);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static string ImportInvoices(InvoicesContext context, string jsonString)
    {
        StringBuilder sb = new StringBuilder();
        var invoicesJson = JsonConvert.DeserializeObject<ImportInvoicesDto[]>(jsonString);
        
        int[] clientsIds = context.Clients.Select(c => c.Id).ToArray();
        
        HashSet<Invoice> validInvoices = new HashSet<Invoice>();
        foreach (var invoice in invoicesJson)
        {
            if (!IsValid(invoice) || !clientsIds.Contains(invoice.ClientId))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
        
            DateTime dueDate = DateTime.ParseExact(invoice.DueDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            DateTime issueDate = DateTime.ParseExact(invoice.IssueDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        
            if (dueDate < issueDate)
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }
        
            Invoice validInvoice = new Invoice()
            {
                Number = invoice.Number,
                IssueDate = issueDate,
                DueDate = dueDate,
                Amount = invoice.Amount,
                CurrencyType = (CurrencyType)invoice.CurrencyType,
                ClientId = invoice.ClientId
            };
        
            validInvoices.Add(validInvoice);
            sb.AppendLine(string.Format(SuccessfullyImportedInvoices, validInvoice.Number));
        }
        
        context.AddRange(validInvoices);
        context.SaveChanges();
        
        return sb.ToString().TrimEnd();
    }

    public static string ImportProducts(InvoicesContext context, string jsonString)
    {
        var productJson = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

        StringBuilder sb = new StringBuilder();
        var validProducts = new HashSet<Product>();

        int[] existedClientsIds = context.Clients.Select(c => c.Id).ToArray();
        foreach (var product in productJson!)
        {
            if (!IsValid(product))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Product validProduct = new Product()
            {
                Name = product.Name,
                Price = product.Price,
                CategoryType = (CategoryType)product.CategoryType,
            };
            

            foreach (var clientId in product.Clients.Distinct())
            {
                if (!existedClientsIds.Contains(clientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ProductClient validProductClient = new ProductClient()
                {
                    ClientId = clientId
                };

                validProduct.ProductsClients.Add(validProductClient);
            }

            validProducts.Add(validProduct);
            sb.AppendLine(string.Format(SuccessfullyImportedProducts, validProduct.Name,
                validProduct.ProductsClients.Count));
        }

        context.AddRange(validProducts);
        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }

    public static bool IsValid(object dto)
    {
        var validationContext = new ValidationContext(dto);
        var validationResult = new List<ValidationResult>();

        return Validator.TryValidateObject(dto, validationContext, validationResult, true);
    }
} 


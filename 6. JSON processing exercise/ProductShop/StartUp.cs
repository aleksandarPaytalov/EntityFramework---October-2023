using Newtonsoft.Json;
using ProductShop.Models;

namespace ProductShop;

using Data;

public class StartUp
{
    public static void Main()
    {
        ProductShopContext context = new ProductShopContext();

        //1. Import Data
        //string userJson = File.ReadAllText("../../../Datasets/users.json");
        //Console.WriteLine(ImportUsers(context, userJson));

        //02. Import Products
        //string productJson = File.ReadAllText("../../../Datasets/products.json");
        //Console.WriteLine(ImportProducts(context, productJson));

        //03. Import Categories
        //string categoryJson = File.ReadAllText("../../../Datasets/categories.json");
        //Console.WriteLine(ImportCategories(context, productJson));

        //04. Import Categories and Products
        //string categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
        //Console.WriteLine(ImportCategoryProducts(context, categoriesProductsJson));

        //05. Export Products In Range
        //Console.WriteLine(GetProductsInRange(context));

        //06. Export Sold Products
        //Console.WriteLine(GetSoldProducts(context));

        //07. Export Categories by Products Count
        //Console.WriteLine(GetCategoriesByProductsCount(context));
    }

    //01. Import Data
    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        var users = JsonConvert.DeserializeObject<List<User>>(inputJson);

        context.Users.AddRange(users);
        context.SaveChanges();

        return $"Successfully imported {users.Count}";
    }

    //02. Import Products
    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
        context.Products.AddRange(products);
        context.SaveChanges();

        return $"Successfully imported {products.Length}";
    }

    //03. Import Categories
    public static string ImportCategories(ProductShopContext context, string inputJson)
    {
        var categories = JsonConvert.DeserializeObject<Category[]>(inputJson);
        var validCategories = categories?.Where(c => c.Name is not null).ToArray();

        if (validCategories != null)
        {
            context.Categories.AddRange(validCategories);
            context.SaveChanges();
            return $"Successfully imported {validCategories.Length}";
        }

        return $"Successfully imported 0";
    }                                                                                                        

    //04. Import Categories and Products
    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
        if (categoriesProducts != null)
        {
            context.CategoriesProducts.AddRange(categoriesProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducts.Length}";
        }

        return $"Successfully imported 0";
    }

    //05. Export Products In Range
    public static string GetProductsInRange(ProductShopContext context)
    {
        var products = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .Select(p => new
            {
                name = p.Name,
                price = p.Price,
                seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
            })
            .OrderBy(p => p.price)
            .ToArray();

        var productJson = JsonConvert.SerializeObject(products, Formatting.Indented);

        return productJson;
    }

    //06. Export Sold Products
    public static string GetSoldProducts(ProductShopContext context)
    {
        var usersWithSoldProducts = context.Users
            .Where(p => p.ProductsSold.Any(b => b.BuyerId != null))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                soldProducts = u.ProductsSold
                    .Where(b  => b.BuyerId != null)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer!.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                    .ToArray()
            })
            .ToArray();

        var usersJsonWithSoldProduct = JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented);

        return usersJsonWithSoldProduct;
    }

    //07. Export Categories by Products Count
    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var categories = context.Categories
            .Select(c => new
            {
                category = c.Name,
                productsCount = c.CategoriesProducts.Count,
                averagePrice = c.CategoriesProducts.Average(p => p.Product.Price).ToString("f2"),
                totalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price).ToString("f2")
            })
            .OrderByDescending(c => c.productsCount)
            .ToArray();
            

        var categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented);

        return categoriesJson;
    }
}

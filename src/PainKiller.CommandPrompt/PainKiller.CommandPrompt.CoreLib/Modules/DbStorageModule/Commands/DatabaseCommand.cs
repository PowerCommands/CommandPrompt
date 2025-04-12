using PainKiller.CommandPrompt.CoreLib.Core.BaseClasses;
using PainKiller.CommandPrompt.CoreLib.Core.Extensions;
using PainKiller.CommandPrompt.CoreLib.Metadata.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Attributes;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Contracts;
using PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Services;

namespace PainKiller.CommandPrompt.CoreLib.Modules.DbStorageModule.Commands;

[CommandDesign(description:"Try out the DbStorage module",
                   options: ["insert", "delete"],
                  examples: ["//Show database info","database","//Insert some products","database --insert"])]
public class DatabaseCommand(string identifier) : ConsoleCommandBase<ApplicationConfiguration>(identifier)
{
    public override RunResult Run(ICommandLineInput input)
    {
        var storage = DbStorageService<Product>.Initialize(Configuration.Core.Modules.DbStorage);
        
        if (input.HasOption("insert")) Insert(storage);
        if (input.HasOption("delete")) Delete(storage);

        var products = storage?.GetAll() ?? [];
        Writer.WriteTable(products);
        return Ok();
    }

    private void Insert(IDbStorageService<Product> storage)
    {
        var rnd = new Random();
        var productNames = new[] { "Wireless Mouse", "Coffee Mug", "Notebook", "Bluetooth Speaker", "Desk Lamp" };

        for (int i = 0; i < 5; i++)
        {
            var product = new Product
            {
                Name = productNames[i],
                Price = (decimal)Math.Round(rnd.NextDouble() * 100, 2), // 0.00 - 100.00
                CreatedAt = DateTime.Now.AddDays(-rnd.Next(1, 10)),
                Orders = [new Order() { Id = 1, Quantity = 100 }, new Order() { Id = 2, Quantity = 32 + i }, new Order() { Id = 3 + i + 3, Quantity = 110 + i + 5 }]
            };
            storage.InsertObject<int>(product);
        }
    }
    private void Delete(IDbStorageService<Product> storage)
    {
        var product = storage.GetAll().FirstOrDefault();
        if (product != null)
        {
            storage.DeleteObject(x => x.Id == product.Id);
            Writer.WriteSuccessLine($"Deleted product with ID: {product.Id}");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonColumn]
        public List<Order> Orders { get; set; } = [];
    }

    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
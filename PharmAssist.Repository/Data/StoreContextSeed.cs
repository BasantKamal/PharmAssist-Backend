
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Order_Aggregation;
using System.Text.Json;


namespace PharmAssist.Repository.Data
{
	public static class StoreContextSeed
	{
		public static async Task SeedAsync(StoreContext dbContext)
		{

			if(!dbContext.Products.Any())
			{
				var productsData = File.ReadAllText("../PharmAssist.Repository/Data/DataSeed/products.json");
				var products = JsonSerializer.Deserialize<List<Product>>(productsData);
				if (products?.Count > 0)
				{
					foreach (var product in products)
						await dbContext.Set<Product>().AddAsync(product);
				}
			}

			if (!dbContext.DeliveryMethod.Any())
			{
				var DeliveryMethodsData = File.ReadAllText("../PharmAssist.Repository/Data/DataSeed/delivery.json");
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
				if (DeliveryMethods?.Count > 0)
				{
					foreach (var DeliveryMethod in DeliveryMethods)
						await dbContext.Set<DeliveryMethod>().AddAsync(DeliveryMethod);
				}
			}

			await dbContext.SaveChangesAsync();

		}
	}
}

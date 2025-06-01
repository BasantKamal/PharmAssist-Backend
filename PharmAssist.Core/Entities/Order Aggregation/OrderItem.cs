using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities.Order_Aggregation
{
	public class OrderItem:BaseEntity
	{
		public OrderItem() { }
		public OrderItem(ProductItemOrdered product, int quantity, decimal price)
		{
			Product = product;
			Price = price;
			Quantity = quantity;
		}

		public ProductItemOrdered Product { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }

	}
}

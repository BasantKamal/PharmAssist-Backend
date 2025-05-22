
using PharmAssist.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Core.Entities
{
	public class Product:BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string PictureUrl { get; set; }
		public decimal Price { get; set; }
		public string ActiveIngredient { get; set; }
    }
}

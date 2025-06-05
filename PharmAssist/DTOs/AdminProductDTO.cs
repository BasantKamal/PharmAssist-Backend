namespace PharmAssist.DTOs
{
    public class AdminProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public string ActiveIngredient { get; set; }
        public string Conflicts { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public string ActiveIngredient { get; set; }
        public string Conflicts { get; set; }
    }

    public class UpdateProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public string ActiveIngredient { get; set; }
        public string Conflicts { get; set; }
    }
} 
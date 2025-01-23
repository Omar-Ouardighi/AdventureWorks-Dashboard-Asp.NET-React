namespace Server.Dtos
{
    public class ProductCategorySalesDto
    {
        public required string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
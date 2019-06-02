namespace ConsulService.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; } 
        public bool IsCredit { get; set; }
    }
}
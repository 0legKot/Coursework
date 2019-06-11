namespace ConsulService.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public decimal Value { get; set; }
        public Currency Currency { get; set; }
        public string Comment { get; set; }
    }
}
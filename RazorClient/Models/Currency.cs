﻿namespace ConsulService.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public override string ToString()
        {
            return $"{{\"Id\":\"{Id}\",\"Name\":\"{Name}\",\"Rate\":\"{Rate}\"}}";
        }
    }
}
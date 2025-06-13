using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OhBau.Model.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand {  get; set; }
        public double Price { get; set; }
        public int Quantity {  get; set; }
        public string Color {  get; set; }
        public string Size {  get; set; }
        public string AgeRange {  get; set; }
        public string Image {  get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get;set; }
        public string Status {  get; set; }
        public bool Active {  get; set; }
        public Guid CategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}

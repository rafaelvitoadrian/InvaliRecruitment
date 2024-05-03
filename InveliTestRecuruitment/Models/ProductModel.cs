using System.ComponentModel.DataAnnotations;

namespace InveliTestRecuruitment.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Esercizio_DTO.Entities
{
    public class Prodotto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public String Name { get; set; }

        [Required]
        [StringLength(100)]
        public String Category { get; set; }

        [StringLength(100)]
        public String? Description { get; set; }

        [Required]
        [Precision(10, 2)]
        [Range(0.01, double.MaxValue)]
        public double Price { get; set; }
    }
}

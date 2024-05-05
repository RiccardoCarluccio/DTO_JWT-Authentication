using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Esercizio_DTO.Models
{
    public class ProdottoDto
    {
        public String Name { get; set; }
        public String Category { get; set; }
        public String? Description { get; set; }
        public double Price { get; set; }
    }
}

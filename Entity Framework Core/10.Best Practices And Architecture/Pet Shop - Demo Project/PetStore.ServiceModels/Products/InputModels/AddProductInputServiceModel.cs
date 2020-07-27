using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.ServiceModels.Products.InputModels
{
    public class AddProductInputServiceModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Range(0, Double.MaxValue)]
        public decimal Price { get; set; }

        public string ProductType { get; set; }
    }
}

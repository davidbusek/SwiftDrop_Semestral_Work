using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model used by the admin to create a new menu item under an existing category.
    /// <see cref="Categories"/> is populated by the controller and used to render the category drop-down.
    /// </summary>
    public class CreateMenuItemViewModel
    {
        /// <summary>Foreign key of the category this item belongs to.</summary>
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        /// <summary>Display name of the menu item.</summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Optional description shown on the restaurant detail page.</summary>
        public string? Description { get; set; }

        /// <summary>Price in CZK.</summary>
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        /// <summary>Optional comma-separated allergen codes (e.g. "1,3,7").</summary>
        [MaxLength(255)]
        public string? Allergens { get; set; }

        /// <summary>Optional weight or volume label shown on the item card (e.g. "300 g", "500 ml").</summary>
        [MaxLength(50)]
        public string? WeightOrVolume { get; set; }

        /// <summary>Optional URL of the item image.</summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// All available categories, grouped by restaurant, used to populate the category drop-down.
        /// Populated by the controller; not submitted as part of the form.
        /// </summary>
        public List<Category> Categories { get; set; } = new();
    }
}

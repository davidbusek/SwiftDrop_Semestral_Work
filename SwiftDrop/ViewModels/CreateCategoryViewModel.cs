using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SwiftDrop.Models;

namespace SwiftDrop.ViewModels
{
    /// <summary>
    /// Form model used by both admins and restaurant managers to create a new menu category
    /// under an existing restaurant.
    /// <para>
    /// For managers the <see cref="AvailableRestaurants"/> list is pre-filtered to only the
    /// restaurants linked to that manager (via <c>Address.UserId</c>).
    /// For admins it contains all restaurants.
    /// </para>
    /// </summary>
    public class CreateCategoryViewModel
    {
        /// <summary>Foreign key of the restaurant this category belongs to.</summary>
        [Required(ErrorMessage = "Restaurant is required.")]
        public int RestaurantId { get; set; }

        /// <summary>Display name of the category (e.g. "Burgers", "Drinks").</summary>
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Optional sort order; lower numbers appear first on the menu.</summary>
        [Range(0, 9999)]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Restaurants available for the drop-down, populated by the controller.
        /// Not submitted as part of the form.
        /// </summary>
        public List<Restaurant> AvailableRestaurants { get; set; } = new();
    }
}

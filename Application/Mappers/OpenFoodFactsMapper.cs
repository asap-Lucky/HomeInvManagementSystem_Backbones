using Application.DTOs.Outbound;
using Application.DTOs.Outbound.API;

namespace HomeInvManagementAPI.Mappers
{
    public static class OpenFoodFactsMapper
    {
        public static ProductAggregateDTO MapToProductAggregateDTO(OpenFoodFactsResponseApiDto openFoodFactsResponse)
        {
            var product = openFoodFactsResponse.Product;

            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product data is null in the OpenFoodFacts response.");

            return new ProductAggregateDTO
            {
                EanCode = openFoodFactsResponse.EanCode,
                ProductName = product.ProductName ?? "Unknown Product",
                //EanCode = openFoodFactsResponse.EanCode,
                //ProductName = product.ProductName,
                //Category = product.c,
                //Brand = product.Brand,
                //ImageBLOB = products.Image != null ? Encoding.UTF8.GetString(products.Image.Data) : null,

                //Locations = products.ProductLocations.Select(pl => new ProductLocationDTO
                //{
                //    LocationId = pl.LocationId,
                //    LocationName = _context.Locations
                //                          .Where(l => l.Id == pl.LocationId)
                //                          .Select(l => l.Name)
                //                          .FirstOrDefault() ?? string.Empty,
                //    Ammount = pl.Amount
                //}).ToList(),
                //CountriesOfOrigin = products.Countries.Select(cuntryOri => new ProductCountryDTO
                //{
                //    CountryId = cuntryOri.Id,
                //    CountryName = cuntryOri.Name
                //}).ToList(),
                //Tags = products.Tags.Select(tag => new ProductTagDTO
                //{
                //    TagId = tag.Id,
                //    TagName = tag.Name
                //}).ToList()

            };
        }
    }
}

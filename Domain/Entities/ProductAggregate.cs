namespace HomeInvManagementAPI.Models
{
    public class ProductAggregate
    {
        public string EANCode { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string CountryOfOrigin { get; set; }
        public List<string> Tags { get; set; }
    }
}

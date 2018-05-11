namespace SAIS.Model
{
    public class CompanyModel
    {
        public int? Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }

        public PersonModel[] Representatives { get; set; }

        public AddressModel ManagementAddress { get; set; }
        public AddressModel ContactAddress { get; set; }
        public string WebSiteAddress { get; set; }
    }
}
namespace api_egc.Models
{
    public class Uniform
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public required byte State { get; set; }
    }
}

namespace api_egc.Models
{
    public class UniformOrder
    {
        public required long Id { get; set; }

        public required decimal GrandTotal { get; set; }

        public required decimal AmountPaid { get; set; }

        public required decimal AmountPending { get; set; }

        public required long MemberId { get; set; }

        public required DateTime CreationDate { get; set; }

        public required DateTime FullDeliveryDate { get; set; }

        public required long InstructorId { get; set; }

        public required int FinancialState { get; set; }

        public required int DeliveryState { get; set; }
    }
}

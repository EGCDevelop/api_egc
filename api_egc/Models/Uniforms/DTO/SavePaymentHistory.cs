namespace api_egc.Models.Uniforms.DTO
{
    public class SavePaymentHistory
    {
        public required PaymentHistory Payment { get; set; }

        public required PaymentHistoryDetails Inflow { get; set; }

        public required PaymentHistoryDetails? Outflow { get; set; }

        public required UniformOrder Order { get; set; }
    }
}

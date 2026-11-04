namespace api_egc.Models.Uniforms.DTO
{
    public class SavePaymentHistory
    {
        public required PaymentHistory Payment { get; set; }

        public required PaymentHistoryDetails Details { get; set; }
    }
}

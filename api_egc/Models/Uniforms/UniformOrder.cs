using System.Text.Json.Serialization;

namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// table egc_pedido_uniforme
    /// </summary>
    public class UniformOrder
    {
        [JsonPropertyName("id")]
        public required long Id { get; set; }

        [JsonPropertyName("grandTotal")]
        public required decimal GrandTotal { get; set; }

        [JsonPropertyName("amountPaid")]
        public required decimal AmountPaid { get; set; }

        [JsonPropertyName("amountPending")]
        public required decimal AmountPending { get; set; }

        [JsonPropertyName("memberId")]
        public required long MemberId { get; set; }

        [JsonPropertyName("creationDate")]
        public required DateTime CreationDate { get; set; }

        [JsonPropertyName("fullDeliveryDate")]
        public DateTime? FullDeliveryDate { get; set; }

        [JsonPropertyName("instructorId")]
        public required long InstructorId { get; set; }

        /// <summary>
        /// 1 - awaiting payment | 2 - partial payment | 3 - canceled
        /// </summary>
        [JsonPropertyName("financialState")]
        public required int FinancialState { get; set; }

        /// <summary>
        /// 1 - pending | 2 - partial delivered | 3 - complete
        /// </summary>
        [JsonPropertyName("deliveryState")]
        public required int DeliveryState { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(GrandTotal)}={GrandTotal.ToString()}, {nameof(AmountPaid)}={AmountPaid.ToString()}, {nameof(AmountPending)}={AmountPending.ToString()}, {nameof(MemberId)}={MemberId.ToString()}, {nameof(CreationDate)}={CreationDate.ToString()}, {nameof(FullDeliveryDate)}={FullDeliveryDate.ToString()}, {nameof(InstructorId)}={InstructorId.ToString()}, {nameof(FinancialState)}={FinancialState.ToString()}, {nameof(DeliveryState)}={DeliveryState.ToString()}}}";
        }
    }
}

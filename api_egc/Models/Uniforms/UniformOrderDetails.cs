using System.Text.Json.Serialization;

namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// Table egc_pedido_uniforme_detalle
    /// </summary>
    public class UniformOrderDetails
    {
        [JsonPropertyName("id")]
        public required long Id { get; set; }

        [JsonPropertyName("orderId")]
        public required long OrderId { get; set; }

        [JsonPropertyName("uniformId")]
        public required int UniformId { get; set; }

        /// <summary>
        /// 1 - pending | 2 - delivered
        /// </summary>
        [JsonPropertyName("deliveryState")]
        public required byte DeliveryState { get; set; }

        [JsonPropertyName("userDelivery")]
        public required long UserDelivery { get; set; }

        [JsonPropertyName("deliveryDate")]
        public DateTime? DeliveryDate { get; set; }

        [JsonPropertyName("deliveryReversalComments")]
        public string? DeliveryReversalComments { get; set; }

        /// <summary>
        /// 1 - active | 2 - inactive
        /// </summary>
        [JsonPropertyName("stateActive")]
        public required byte StateActive { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, " +
                $"\n{nameof(OrderId)}={OrderId.ToString()}, " +
                $"\n{nameof(UniformId)}={UniformId.ToString()}, " +
                $"\n{nameof(DeliveryState)}={DeliveryState.ToString()}, " +
                $"\n{nameof(UserDelivery)}={UserDelivery.ToString()}, " +
                $"\n{nameof(DeliveryDate)}={DeliveryDate.ToString()}, " +
                $"\n{nameof(DeliveryReversalComments)}={DeliveryReversalComments}, " +
                $"\n{nameof(StateActive)}={StateActive.ToString()}}}";
        }
    }
}

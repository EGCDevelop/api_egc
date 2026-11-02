namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// Table egc_pedido_uniforme_detalle
    /// </summary>
    public class UniformOrderDetails
    {
        public required long Id { get; set; }

        public required long OrderId { get; set; }

        public required int UniformId { get; set; }

        /// <summary>
        /// 1 - pending | 2 - delivered
        /// </summary>
        public required byte DeliveryState { get; set; }

        public required long UserDelivery { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string? DeliveryReversalComments { get; set; }

        /// <summary>
        /// 1 - active | 2 - inactive
        /// </summary>
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

namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// table egc_pedido_uniforme
    /// </summary>
    public class UniformOrder
    {
        public required long Id { get; set; }

        public required decimal GrandTotal { get; set; }

        public required decimal AmountPaid { get; set; }

        public required decimal AmountPending { get; set; }

        public required long MemberId { get; set; }

        public required DateTime CreationDate { get; set; }

        public DateTime? FullDeliveryDate { get; set; }

        public required long InstructorId { get; set; }

        /// <summary>
        /// 1 - awaiting payment | 2 - partial payment | 3 - canceled
        /// </summary>
        public required int FinancialState { get; set; }

        /// <summary>
        /// 1 - pending | 2 - partial delivered | 3 - complete
        /// </summary>
        public required int DeliveryState { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, " +
                $"\n{nameof(GrandTotal)}={GrandTotal.ToString()}, " +
                $"\n{nameof(AmountPaid)}={AmountPaid.ToString()}, " +
                $"\n{nameof(AmountPending)}={AmountPending.ToString()}, " +
                $"\n{nameof(MemberId)}={MemberId.ToString()}, " +
                $"\n{nameof(CreationDate)}={CreationDate.ToString()}, " +
                $"\n{nameof(FullDeliveryDate)}={FullDeliveryDate.ToString()}, " +
                $"\n{nameof(InstructorId)}={InstructorId.ToString()}, " +
                $"\n{nameof(FinancialState)}={FinancialState.ToString()}, " +
                $"\n{nameof(DeliveryState)}={DeliveryState.ToString()}}}";
        }
    }
}

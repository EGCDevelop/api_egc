namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// table egc_detalle_historial_pagos
    /// </summary>
    public class PaymentHistoryDetails
    {
        public required long Id { get; set; }

        public required long PaymentHistoryId { get; set; }

        /// <summary>
        /// 1 - entradas | 2 - salidas
        /// </summary>
        public required byte TypeMovement { get; set; } 

        public required int NumberOneBills { get; set; }

        public required int NumberFiveBills { get; set; }

        public required int NumberTenBills { get; set; }

        public required int NumberTwentyBills { get; set; }

        public required int NumberFiftyBills { get; set; }

        public required int NumberOneHundredBills { get; set; }

        public required int TwoHundredBills { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(PaymentHistoryId)}={PaymentHistoryId.ToString()}, {nameof(TypeMovement)}={TypeMovement.ToString()}, {nameof(NumberOneBills)}={NumberOneBills.ToString()}, {nameof(NumberFiveBills)}={NumberFiveBills.ToString()}, {nameof(NumberTenBills)}={NumberTenBills.ToString()}, {nameof(NumberTwentyBills)}={NumberTwentyBills.ToString()}, {nameof(NumberFiftyBills)}={NumberFiftyBills.ToString()}, {nameof(NumberOneHundredBills)}={NumberOneHundredBills.ToString()}, {nameof(TwoHundredBills)}={TwoHundredBills.ToString()}}}";
        }
    }
}

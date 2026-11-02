namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// table egc_detalle_historial_pagos
    /// </summary>
    public class PaymentHistoryDetails
    {
        public required long Id { get; set; }

        /// <summary>
        /// 1 - entradas | 2 - salidas
        /// </summary>
        public required long PaymentHistoryId { get; set; }

        public required byte TypeMovement { get; set; } 

        public required int NumberOneBills { get; set; }

        public required int NumberFiveBills { get; set; }

        public required int NumberTenBills { get; set; }

        public required int NumberTwentyBills { get; set; }

        public required int NumberFiftyBills { get; set; }

        public required int NumberOneHundredBills { get; set; }

        public required int TwoHundredBills { get; set; }
    }
}

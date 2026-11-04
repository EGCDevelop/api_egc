namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// Table egc_historial_pagos
    /// </summary>
    public class PaymentHistory
    {
        public required long Id { get; set; }

        public required decimal PaidAmount { get; set; }

        public required decimal CashReceived { get; set; }

        public required decimal ChancheMoney { get; set; }

        public required DateTime PaidDate { get; set; } 

        public required int PaymentNumber { get; set; }

        public string? Observations{ get; set; }

        public required long OrderId { get; set; }

        public required long UserReceived { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(PaidAmount)}={PaidAmount.ToString()}, {nameof(CashReceived)}={CashReceived.ToString()}, {nameof(ChancheMoney)}={ChancheMoney.ToString()}, {nameof(PaidDate)}={PaidDate.ToString()}, {nameof(PaymentNumber)}={PaymentNumber.ToString()}, {nameof(Observations)}={Observations}, {nameof(OrderId)}={OrderId.ToString()}, {nameof(UserReceived)}={UserReceived.ToString()}}}";
        }
    }
}

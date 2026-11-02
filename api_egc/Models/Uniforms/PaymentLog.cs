namespace api_egc.Models.Uniforms
{
    /// <summary>
    /// table egc_bitacora_pagos
    /// </summary>
    public class PaymentLog
    {
        public required long Id { get; set; }

        public required string Action { get; set; }

        public required long ActionUserId { get; set; }

        public required DateTime ActionDate { get; set; }
    }
}

using System;

namespace CPG.Application.UseCases.DirectDebit.ViewModels
{
    public class GrantDataViewModel
    {
        public DateTime CraetionDateTime { get; set; }
        public decimal AmountLimitPerTransaction { get; set; }
        public string AccountNumber { get; set; }
        public double RemainingDays { get; set; }
    }
}

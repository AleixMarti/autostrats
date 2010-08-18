#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Failed low reversal long with stop loss and trailing stop.
    /// </summary>
    [Description("Failed low reversal long with stop loss and trailing stop.")]
    public class V2BLong230510 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int contractAmount = 1; // Default setting for ContractAmount
        private int signalBarPlusTicks = 1; // Default setting for SignalBarPlusTicks
        private int stopLoss = 5; // Default setting for StopLoss
        private int trailStop = 8; // Default setting for TrailStop
        private int profitTarget = 48; // Default setting for ProfitTarget
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(NBarsDown(1, true, true, true));
            SetStopLoss("", CalculationMode.Ticks, StopLoss, true);
            SetProfitTarget("", CalculationMode.Ticks, ProfitTarget);
            SetTrailStop("2B0", CalculationMode.Ticks, TrailStop, true);
            SetTrailStop("2B1", CalculationMode.Ticks, TrailStop, true);

            CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if  ((Close [3] <= Open [3])	
				&& (Close [2] >= Open [2])
				&& (Close [1] >= Open [1]))	
				
				
				
            {
                EnterLongStop(ContractAmount, High[0] + SignalBarPlusTicks * TickSize, "2B0");
            }

            // Condition set 2
            //if (Position.MarketPosition == MarketPosition.Flat
            //    && NBarsDown(1, true, true, true)[2] > 0
            //    && Close[1] > High[2])
            //{
            //    EnterLongStop(ContractAmount, High[1] + SignalBarPlusTicks * TickSize, "2B1");
            //}
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int ContractAmount
        {
            get { return contractAmount; }
            set { contractAmount = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int SignalBarPlusTicks
        {
            get { return signalBarPlusTicks; }
            set { signalBarPlusTicks = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(3, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailStop
        {
            get { return trailStop; }
            set { trailStop = Math.Max(6, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(5, value); }
        }
        #endregion
    }
}

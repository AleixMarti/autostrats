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
    /// Long 3BR ES with ARM after 3BR.
    /// </summary>
    [Description("Long 3BR ES with ARM after 3BR.")]
    public class LONG3BRATMES270510 : Strategy
    {
        #region Variables
		
		
		private int signalBarPlusTicks = 1; // Default setting for SignalBarPlusTicks
		
		private string	atmStrategyId		= string.Empty;
		private string	orderId				= string.Empty;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// After our strategy has a PnL greater than $1000 or less than -$400 we will stop our strategy
			//if (Performance.AllTrades.TradesPerformance.Currency.CumProfit > 1000 
			if	( Performance.AllTrades.TradesPerformance.Currency.CumProfit < -4000)
				
			// Checks to see if the day of the week is Saturday or Sunday. Only allows trading if the day of the week is not Saturday or Sunday.
			if (Time[0].DayOfWeek != DayOfWeek.Saturday && Time[0].DayOfWeek != DayOfWeek.Sunday)
			//{
				/* Checks to see if the time is during the following hours (format is HHMMSS or HMMSS). 
				The timezone used here is (GMT-05:00) EST. */
				//if ((ToTime(Time[0]) >= 101500 && ToTime(Time[0]) <= 151500)) //|| (ToTime(Time[0]) >= 140000 && ToTime(Time[0]) < 154500))
				if ((ToTime(Time[0]) >= 141500 && ToTime(Time[0]) <= 201500))
				//{	
					if (Historical) return;
			


			// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
            // **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
			if (orderId.Length == 0 && atmStrategyId.Length == 0 && (NBarsDown(3, true, true, true)[0] > 0))
								
			{
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(Cbi.OrderAction.Buy, OrderType.StopLimit, High[0] + SignalBarPlusTicks * TickSize, 0, TimeInForce.Day, orderId, "ES_3BR_ATM", atmStrategyId);
			}


			// Check for a pending entry order
			if (orderId.Length > 0)
			{
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);
				 
                
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
				if (status.GetLength(0) > 0)
				{
					// Print out some information about the order to the output window
					Print("The entry order average fill price is: " + status[0]);
					Print("The entry order filled amount is: " + status[1]);
					Print("The entry order order state is: " + status[2]);

					// If the order state is terminal, reset the order id value
					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
						orderId = string.Empty;
				}
			} // If the strategy has terminated reset the strategy id
			else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
				atmStrategyId = string.Empty;


			if (atmStrategyId.Length > 0)
			{
				// You can change the stop price
				if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
					AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP1", atmStrategyId);

				// Print some information about the strategy to the output window
				//Print("The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
				//Print("The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
				//Print("The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
				//Print("The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));
			}
        }

        #region Properties
		
		[Description("")]
        [GridCategory("Parameters")]
        public int SignalBarPlusTicks
        {
            get { return signalBarPlusTicks; }
            set { signalBarPlusTicks = Math.Max(1, value); }
        }
		
        #endregion
    }
}

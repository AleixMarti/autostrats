// 
// Copyright (C) 2006, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//

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
using NinjaTrader.Strategy;
#endregion


// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public class TFcancel3BR_3W : Strategy
    {
        #region Variables
		private string	atmStrategyId		= string.Empty;
		private string	orderId				= string.Empty;
		
		private int prevBarsPlusTicks = 1; // Default setting for PrevBarsPlusTicks
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            EntriesPerDirection = 1;
			CalculateOnBarClose = false;
			TraceOrders = true;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// HELP DOCUMENTATION REFERENCE: Please see the Help Guide section "Using ATM Strategies"

			// Make sure this strategy does not execute against historical data
			if (Historical)
				return;


			// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
            // **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
			if (Position.MarketPosition == MarketPosition.Flat && orderId.Length == 0 && atmStrategyId.Length == 0 
				//&& (NBarsDown(3, true, true, true)[1] > 0))
				&& Close [3] <= Open [3]
				&& Close [2] <= Open [2]
				&& Close [1] <= Open [1]
				&& High  [1] + PrevBarsPlusTicks * TickSize > GetCurrentBid())
			{
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(Cbi.OrderAction.Buy, OrderType.Stop, 0, High[1] + PrevBarsPlusTicks * TickSize, TimeInForce.Day, orderId, "TF_long_500T", atmStrategyId);	
				Print("70 TFcancel3BR_3W ATM create order");
			}


			// Check for a pending entry order
			if (orderId.Length > 0)
			{
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);
                
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
				if (status.GetLength(0) > 0)
				{
					

					// If the order state is terminal, reset the order id value
					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
						orderId = string.Empty;
					
					
				}
			} // If the strategy has terminated reset the strategy id
			else if 
				(atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
				atmStrategyId = string.Empty;
		     	
			
			// Check for a pending entry order
		//	else if (orderId.Length > 0)
		//	{
		//		string[] status = GetAtmStrategyEntryOrderStatus(orderId);
        //      
		//		// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
		//		if (status.GetLength(0) > 0)
		//		
		//			status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending"
         //          && Close [0] - 10 * TickSize < High[1]
		//			
		//			AtmStrategyCancelEntryOrder(orderId);
		//			Print("107 TFcancel3BR_3W Cancelled Order less than 5 ticks");
		//			atmStrategyId = string.Empty;
		//			orderId = string.Empty;
		//				
		//							
		//	}

			if (atmStrategyId.Length > 0)
			{
				// You can change the stop price
				//if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
				//	AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP1", atmStrategyId);

				// Print some information about the strategy to the output window
				Print("The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
				Print("The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
				Print("The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
				Print("The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));
				Print("The current ATM Strategy Realized PnL is: " + GetAtmStrategyRealizedProfitLoss(atmStrategyId));
			}
        }

        #region Properties
		
		[Description("")]
        [GridCategory("Parameters")]
        public int PrevBarsPlusTicks
        {
            get { return prevBarsPlusTicks; }
            set { prevBarsPlusTicks = Math.Max(1, value); }
        }
		
		
        #endregion
    }
}

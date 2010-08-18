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
    public class FDAX3BRjthom : Strategy
    {
        #region Variables
		
		private string	atmStrategyId		= string.Empty;
		private string	orderId				= string.Empty;
		
		private int prevBarsPlusTicks = 1; // Default setting for PrevBarsPlusTicks
		
		private int barNumberOfOrder = 0; 
		
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Print("43 Initialise FDAX3BRjthom");
			EntriesPerDirection = 1;
			EntryHandling = EntryHandling.AllEntries; 
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
			if (Position.MarketPosition == MarketPosition.Flat 
				&& orderId.Length == 0 
				&& atmStrategyId.Length == 0 
				&& FirstTickOfBar
				&& Close [3] <= Open [3]
				&& Close [2] <= Open [2]
				&& Close [1] <= Open [1]
				&& Open  [1] + PrevBarsPlusTicks * TickSize > GetCurrentBid())
			
			
			{
				
				
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(Cbi.OrderAction.Buy, OrderType.Stop, 0, Open[1] + PrevBarsPlusTicks * TickSize, TimeInForce.Day, orderId, "TF_long_500T", atmStrategyId);	
			    barNumberOfOrder = CurrentBar;
				Print("81 AtmStrategyCreate FDAX3BRjthom");
			}


			// Check for a pending entry order
			if (  orderId.Length > 0)
			
			
			{
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);
				
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements

				if (status.GetLength(0) > 0)
					
				
				if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
				
				{
					Print("100 Reset the strategy id if filled/cancelled or rejected");	
      				orderId = string.Empty;
				    					
				}
                
				//if (Close [0] - 5 * TickSize > High[1] &&(status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending"))
				if ((status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending") && High[1] > Close [0] - 5 * TickSize   )
				
				{
				Print("108 ATM cancel FDAX3BRjthom order");	
				AtmStrategyCancelEntryOrder(orderId);
				}
			} 
			
						
			if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Flat)
				{
				Print("117 reset the strategy id FDAX3BRjthom");
				atmStrategyId = string.Empty;
				orderId = string.Empty;
				}


			if (atmStrategyId.Length > 0)
			{
				
				// Print some information about the strategy to the output window
				Print("126 At the end of FDAX3BRjthom");
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

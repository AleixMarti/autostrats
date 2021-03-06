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
    public class TFSV2Bjthom : Strategy
    {
        #region Variables
		
		private string	atmStrategyId		= string.Empty;
		private string	orderId				= string.Empty;
		
		private int barNumberOfOrder = 0;
		
		private int prevBarsMinusTicks = 1; // Default setting for PrevBarsPlusTicks
		
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Print("43 Initialise TFSV2Bjthom");
			CalculateOnBarClose = false;
			TraceOrders = true;
        }
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
			
        {
			// Make sure this strategy does not execute against historical data
			if (Historical)
				return;
			

			// Submits an entry limit order to initiate an ATM Strategy if both order id and strategy id are in a reset state
            
			if (Position.MarketPosition == MarketPosition.Flat 
				&& orderId.Length == 0 
				&& atmStrategyId.Length == 0 
				&& FirstTickOfBar
				&& Close [3] >= Open [3]	
                && Close [2] <= Open [2]
				&& Close [1] <= Open [1]
				&& Low[1] - PrevBarsMinusTicks * TickSize < GetCurrentAsk())
			
			
			{
				
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(Cbi.OrderAction.Sell, OrderType.Stop, 0, Low[1] - PrevBarsMinusTicks * TickSize , TimeInForce.Day, orderId, "TF_short_500T", atmStrategyId);	
                barNumberOfOrder = CurrentBar;
				Print("74 AtmStrategyCreate TFSV2Bjthom ");
			}


			// Check for a pending entry order
			if (orderId.Length > 0)
			{
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements

				if (status.GetLength(0) > 0)
			
					
				if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
					
				{
					Print("92 Reset the strategy id if filled/cancelled or rejected");	
      				orderId = string.Empty;				
				}	
				
				if (Close [0] + 4 * TickSize > Low[1] &&(status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending"))
				
				{
				Print("99 ATM cancel TFSV2Bjthom order");	
				AtmStrategyCancelEntryOrder(orderId);
				}
				
			}
			
			    if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Flat)
				{
				Print("108 reset the strategy id");
				atmStrategyId = string.Empty;
				orderId = string.Empty;
				}
				
				
				if (atmStrategyId.Length > 0)
			{
				
				// Print some information about the strategy to the output window
				Print("126 At the end of  TFL3BR");
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
        public int PrevBarsMinusTicks
        {
            get { return prevBarsMinusTicks; }
            set { prevBarsMinusTicks = Math.Max(1, value); }
        }
		
		
        #endregion
    }
}

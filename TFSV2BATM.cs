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
    public class TFSV2BATM : Strategy
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
            EntriesPerDirection = 1;
			EntryHandling = EntryHandling.UniqueEntries; 
			CalculateOnBarClose = false;
			TraceOrders = true;
			TimeInForce = Cbi.TimeInForce.Day; 
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
				// && GetAtmStrategyMarketPosition == Cbi.MarketPosition.Flat 
				// && atmStrategyId.Length < 0
				&& orderId.Length == 0
				&& atmStrategyId.Length == 0
				&& FirstTickOfBar
				//&& BarsSinceEntry() > 2
				&& Close [3] >= Open [3]	
                && Close [2] <= Open [2]
				&& Close [1] <= Open [1]
				&& Low[1] - PrevBarsMinusTicks * TickSize < GetCurrentAsk())
			
			
			{
				
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				
				AtmStrategyCreate(Cbi.OrderAction.Sell, OrderType.Stop, 0, Low[1] - PrevBarsMinusTicks * TickSize , TimeInForce.Day, orderId, "TF_short_3W_V2B", atmStrategyId);	
                barNumberOfOrder = CurrentBar;
			}


			// Check for a pending entry order
			if (orderId.Length > 0)
			{
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);
				if (status.GetLength(0) > 0)
					
				{
					
					if
						
					((status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending")&& (CurrentBar > barNumberOfOrder + 2)) 
					AtmStrategyCancelEntryOrder(orderId);
				    Print("TFSV2BATM cancelled order");
					atmStrategyId = string.Empty;
					orderId = string.Empty;
				}
                
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
				 else if (status.GetLength(0) > 0)
				{
					// Print out some information about the order to the output window
					Print("The entry order average fill price is: " + status[0]);
					Print("The entry order filled amount is: " + status[1]);
					Print("The entry order order state is: " + status[2]);

					// If the order state is terminal, reset the order id value
					if (status[0] == "Filled" || status[0] == "Cancelled" || status[0] == "Rejected")
						orderId = string.Empty;
				}
			} 
			
			else 
			if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Flat)
				{
				Print("FSV2BATM 117 reset the strategy id");
				atmStrategyId = string.Empty;
				orderId = string.Empty;
				}


			if (atmStrategyId.Length > 0)
			{
				

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
        public int PrevBarsMinusTicks
        {
            get { return prevBarsMinusTicks; }
            set { prevBarsMinusTicks = Math.Max(1, value); }
        }
		
		
        #endregion
    }
}

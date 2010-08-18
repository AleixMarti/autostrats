// 
// Copyright (C) 2009, NinjaTrader LLC <www.ninjatrader.com>.
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
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Sample Reference Strategy demonstrating use of CancelOrder()
    /// </summary>
    [Description("Reference Sample demonstrating use of CancelOrder()")]
    public class SampleCancelOrder : Strategy
    {
        #region Variables
		private IOrder 	entryOrder 			= null; // This variable holds an object representing our entry order.
		private IOrder 	stopOrder 			= null; // This variable holds an object representing our stop loss order.
		private IOrder 	targetOrder 		= null; // This variable holds an object representing our profit target order.
		private IOrder	marketOrder			= null; // This variable holds an object representing our market EnterLong() order.
		private int 	barNumberOfOrder 	= 0;	// This variable is used to store the entry bar
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose 	= true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Return if there aren't enough loaded bars
			if (CurrentBar < 1)
				return;
			
			// First, we need a simple entry. Then entryOrder == null checks to make sure entryOrder does not contain an order yet.
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				// Check IOrder objects for null to ensure there are no working entry orders before submitting new entry order
				if (entryOrder == null && marketOrder == null && Close[0] > Close[1])
				{
					/* Our IOrder object, entryOrder, is assigned an entry order.
					It is offset 5 ticks below the low to try and make it not execute to demonstrate the CancelOrder() method. */
					entryOrder = EnterLongLimit(0, true, 1, Low[0] - 5 * TickSize, "long limit entry");
					
					// Here, we assign barNumberOfOrder the CurrentBar, so we can check how many bars pass after our order is placed.
					barNumberOfOrder = CurrentBar;
				}				
				
				// If entryOrder has not been filled within 3 bars, cancel the order.
				else if (entryOrder != null && CurrentBar > barNumberOfOrder + 3)
				{
					// When entryOrder gets cancelled below in OnOrderUpdate(), it gets replaced with a Market Order via EnterLong()
					CancelOrder(entryOrder);
				}
			}
        }
		
		protected override void OnOrderUpdate(IOrder order)
		{
			// Checks for all updates to entryOrder.
			if (entryOrder != null && entryOrder.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled)
				{
					// Reset entryOrder back to null
					entryOrder = null;
					
					// Replace entry limit order with a market order.
					marketOrder = EnterLong(1, "market order");
				}
			}
		}
		
		protected override void OnExecution(IExecution execution)
        {
			/* We advise monitoring OnExecution() to trigger submission of stop/target orders instead of OnOrderUpdate() since OnExecution() is called after OnOrderUpdate()
			which ensures your strategy has received the execution which is used for internal signal tracking.
			
			This first if-statement is in place to deal only with the long limit entry. */
			if (entryOrder != null && entryOrder.Token == execution.Order.Token)
			{
				// This second if-statement is meant to only let fills and cancellations filter through.
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
				{
					// Simple stop and target
					stopOrder = ExitLongStop(0, true, 1, execution.Price - 20 * TickSize, "stop", "long limit entry");
					targetOrder = ExitLongLimit(0, true, 1, execution.Price + 40 * TickSize, "target", "long limit entry");
					
					// Resets the entryOrder object to null after the order has been filled
					if (execution.Order.OrderState != OrderState.PartFilled)
					{
						entryOrder 	= null;
					}
				}
			}
			
			// This if-statments lets execution details for the market order filter through.
			else if (marketOrder != null && marketOrder.Token == execution.Order.Token)
			{
				// Check only for fills and cancellations.
				if (execution.Order.OrderState == OrderState.Filled || execution.Order.OrderState == OrderState.PartFilled || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
				{
					stopOrder = ExitLongStop(0, true, 1, execution.Price - 15 * TickSize, "stop", "market order");
					targetOrder = ExitLongLimit(0, true, 1, execution.Price + 30 * TickSize, "target", "market order");
					
					// Resets the marketOrder object to null after the order has been filled
					if (execution.Order.OrderState != OrderState.PartFilled)
					{
						marketOrder = null;
					}
				}
			}
		}
        #region Properties
   			// Nothing to see here.
        #endregion
    }
}

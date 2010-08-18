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

//Version 1.0 (2/9/10) - original

//Version 1.1 (2/23/10)
//Updates: CancelOrder() fixed, plots arrows and chart name on main chart.

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enters long limit order when 20 EMA crosses above 50 EMA and a short limit order for the opposite.
    /// </summary>
    [Description("Enters long limit order when 20 EMA crosses above 50 EMA and a short limit order for the opposite.")]
    public class SampleAdvancedAutomatedStratv1point1 : Strategy
    {
        #region Variables
		
		private int		breakEvenTicks		= 4;
		private int		plusBreakEven		= 0;
		private int 	trailProfitTrigger 	= 4;
		private int		trailStepTicks		= 1;
		
		private double	initialBreakEven	= 0;
		private double 	previousPrice 		= 0;
		private double 	newPrice 			= 0;
		
		private IOrder 	entryOrder1 		= null;
		private IOrder 	entryOrder2 		= null;
		private IOrder 	entryOrder3 		= null;
		private IOrder 	entryOrder4 		= null;
		private IOrder 	entryOrder5 		= null;
		private IOrder 	entryOrder6 		= null;
		private IOrder	limitPrice			= null;
		
		#endregion
		
        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
			Add(PeriodType.Volume, 20000);
			Add(PeriodType.Volume, 30000);
			EntriesPerDirection = 2;
			EntryHandling = EntryHandling.UniqueEntries;
									
            CalculateOnBarClose = false; 
			TraceOrders = true;
			
			SetProfitTarget("Long 1a", CalculationMode.Ticks, 4);
			SetProfitTarget("Long 1b", CalculationMode.Ticks, 8);
			SetProfitTarget("Short 1a", CalculationMode.Ticks, 4);
			SetProfitTarget("Short 1b", CalculationMode.Ticks, 8);
			
		}
		
        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
		
		
		protected override void OnBarUpdate()
		{	
			// After our strategy has a PnL greater than $1000 or less than -$400 we will stop our strategy
			if (Performance.AllTrades.TradesPerformance.Currency.CumProfit > 1000 
				|| Performance.AllTrades.TradesPerformance.Currency.CumProfit < -400)
			{
				/* A custom method designed to close all open positions and cancel all working orders will be called.
				This will ensure we do not have an unmanaged position left after we halt our strategy. */
				StopStrategy();
			
				// Halt further processing of our strategy
				return;
			}	
			// Checks to see if the day of the week is Saturday or Sunday. Only allows trading if the day of the week is not Saturday or Sunday.
			if (Time[0].DayOfWeek != DayOfWeek.Saturday && Time[0].DayOfWeek != DayOfWeek.Sunday)
			{
				/* Checks to see if the time is during the following hours (format is HHMMSS or HMMSS). 
				The timezone used here is (GMT-05:00) EST. */
				if ((ToTime(Time[0]) >= 101500 && ToTime(Time[0]) <= 151500)) //|| (ToTime(Time[0]) >= 140000 && ToTime(Time[0]) < 154500))	
				{	
					if (Historical) return;
				
					#region Trail Stop
					// Only allow entries if we have no current positions open
						switch (Position.MarketPosition)
					{
						case MarketPosition.Flat:
							SetStopLoss("Long 1a", CalculationMode.Ticks, 12, false);
							SetStopLoss("Long 1b", CalculationMode.Ticks, 12, false);
							SetStopLoss("Short 1a", CalculationMode.Ticks, 12, false);
							SetStopLoss("Short 1b", CalculationMode.Ticks, 12, false);
							previousPrice = 0;
							break;
						case MarketPosition.Long:
							// Once the price is greater than entry price+ breakEvenTicks ticks, set stop loss to breakeven
							if (Close[0] == Position.AvgPrice + breakEvenTicks * TickSize && previousPrice == 0)
							{
								initialBreakEven = Position.AvgPrice + plusBreakEven * TickSize; 
								SetStopLoss("Long 1a", CalculationMode.Price, initialBreakEven, false);
								SetStopLoss("Long 1b", CalculationMode.Price, initialBreakEven, false);
								previousPrice = Position.AvgPrice;
								PlaySound(@"C:\Program Files\NinjaTrader 6.5\sounds\AutoTrail.wav");
								PrintWithTimeStamp("previousPrice = "+previousPrice);
								PrintWithTimeStamp("newPrice = "+previousPrice);
							}
							// Once at breakeven wait till trailProfitTrigger is reached before advancing stoploss by trailStepTicks size step
							else if (previousPrice	!= 0 ////StopLoss is at breakeven
									&& Close[0] > previousPrice + trailProfitTrigger * TickSize)
								
							{
								newPrice = previousPrice + trailStepTicks * TickSize;
								SetStopLoss("Long 1b", CalculationMode.Price, newPrice, false);
								previousPrice = newPrice;
								PrintWithTimeStamp("previousPrice = "+previousPrice);
								PrintWithTimeStamp("newPrice = "+previousPrice);
								PlaySound(@"C:\Program Files\NinjaTrader 6.5\sounds\AutoTrail.wav");
							}
							break;
						case MarketPosition.Short:
							// Once the price is Less than entry price - breakEvenTicks ticks, set stop loss to breakeven
							if (Close[0] == Position.AvgPrice - breakEvenTicks * TickSize && previousPrice == 0)
							{
								initialBreakEven = Position.AvgPrice - plusBreakEven * TickSize;
							SetStopLoss("Short 1a", CalculationMode.Price, initialBreakEven, false);
								SetStopLoss("Short 1b", CalculationMode.Price, initialBreakEven, false);
								previousPrice = Position.AvgPrice;
								PlaySound(@"C:\Program Files\NinjaTrader 6.5\sounds\AutoTrail.wav");
								PrintWithTimeStamp("previousPrice = "+previousPrice);
								PrintWithTimeStamp("newPrice = "+previousPrice);
							}
							// Once at breakeven wait till trailProfitTrigger is reached before advancing stoploss by trailStepTicks size step
							else if (previousPrice	!= 0 ////StopLoss is at breakeven
									&& Close[0] < previousPrice - trailProfitTrigger * TickSize)
								
							{
								newPrice = previousPrice - trailStepTicks * TickSize;
								SetStopLoss("Short 1b", CalculationMode.Price, newPrice, false);
								previousPrice = newPrice;
								PrintWithTimeStamp("previousPrice = "+previousPrice);
								PrintWithTimeStamp("newPrice = "+previousPrice);
								PlaySound(@"C:\Program Files\NinjaTrader 6.5\sounds\AutoTrail.wav");
							}
							break;	
							
						
					}
					#endregion
			
					#region Entry Signal and Cancel Order
				
					if (Position.MarketPosition == MarketPosition.Flat && BarsInProgress == 0)
					{	
						// Entry Condition: Enters long limit order when 20 EMA crosses above 50 EMA and a short limit order for the opposite.
					
							if (entryOrder1 == null && entryOrder2 == null && FirstTickOfBar && CrossAbove(EMA(20), EMA(50), 1))
						{	
							entryOrder1 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1a");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "10K", 0, Low[0] - 8 * TickSize, Color.Blue);
							entryOrder2 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1b");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "10K", 0, Low[0] - 8 * TickSize, Color.Blue);
							
						}
							//Cancel order if price moves four ticks away without filling
							else if (entryOrder1 != null && entryOrder1.LimitPrice != null && Close[0] == entryOrder1.LimitPrice + 4 * TickSize
							&& entryOrder2 != null && entryOrder2.LimitPrice != null && Close[0] == entryOrder2.LimitPrice + 4 * TickSize)
							{		
							CancelOrder(entryOrder1);
							CancelOrder(entryOrder2);
							}
				
						else if (entryOrder1 == null && entryOrder2 == null && FirstTickOfBar && CrossBelow(EMA(20), EMA(50), 1))
						{	
							entryOrder1 = EnterShortLimit(0, true, DefaultQuantity,  Open[0] + .25, "Short 1a");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "10K", 0, High[0] + 8 * TickSize, Color.Red);
							entryOrder2 = EnterShortLimit(0, true, DefaultQuantity,  Open[0] + .25, "Short 1b");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "10K", 0, High[0] + 8 * TickSize, Color.Red);
							
						}
							//Cancel order if price moves four ticks away without filling
							else if (entryOrder1 != null && entryOrder1.LimitPrice != null && Close[0] == entryOrder1.LimitPrice - 4 * TickSize
							&& entryOrder2 != null && entryOrder2.LimitPrice != null && Close[0] == entryOrder2.LimitPrice - 4 * TickSize)
							{	
							CancelOrder(entryOrder1);
							CancelOrder(entryOrder2);
							}	
					}	
				
					if (Position.MarketPosition == MarketPosition.Flat && BarsInProgress == 1)
					{	
						// Entry Condition: Enters long limit order when 20 EMA crosses above 50 EMA and a short limit order for the opposite.
						if (entryOrder3 == null && entryOrder4 == null && FirstTickOfBar && CrossAbove(EMA(20), EMA(50), 1))
						{
							entryOrder3 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1a");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "20K", 0, Low[0] - 8 * TickSize, Color.Blue);
							entryOrder4 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1b");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "20K", 0, Low[0] - 8 * TickSize, Color.Blue);
						}
						//Cancel order if price moves four ticks away without filling
						else if (entryOrder3 != null && entryOrder3.LimitPrice != null && Close[0] == entryOrder3.LimitPrice + 4 * TickSize
						&& entryOrder4 != null && entryOrder4.LimitPrice != null && Close[0] == entryOrder4.LimitPrice + 4 * TickSize)
						{		
						CancelOrder(entryOrder3);
						CancelOrder(entryOrder4);
						}
						
						if (entryOrder3 == null && entryOrder4 == null && FirstTickOfBar && CrossBelow(EMA(20), EMA(50), 1))
						{	
							entryOrder3 = EnterShortLimit(0, true, DefaultQuantity,  Open[0], "Short 1a");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "20K", 0, High[0] + 8 * TickSize, Color.Red);
							entryOrder4 = EnterShortLimit(0, true, DefaultQuantity,  Open[0], "Short 1b");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "20K", 0, High[0] + 8 * TickSize, Color.Red);
						}
						//Cancel order if price moves four ticks away without filling
						else if (entryOrder3 != null && entryOrder3.LimitPrice != null && Close[0] == entryOrder3.LimitPrice - 4 * TickSize
						&& entryOrder4 != null && entryOrder4.LimitPrice != null && Close[0] == entryOrder4.LimitPrice - 4 * TickSize)
						{	
						CancelOrder(entryOrder3);
						CancelOrder(entryOrder4);
						}	
					}	
					
					if (Position.MarketPosition == MarketPosition.Flat && BarsInProgress == 2)
					{	
						// Entry Condition: Enters long limit order when 20 EMA crosses above 50 EMA and a short limit order for the opposite.
						if (entryOrder5 == null && entryOrder6 == null && FirstTickOfBar && CrossAbove(EMA(20), EMA(50), 1))
						{
							entryOrder5 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1a");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "30K", 0, Low[0] - 8 * TickSize, Color.Blue);
							entryOrder6 = EnterLongLimit(0, true, DefaultQuantity,  Open[0], "Long 1b");
							DrawArrowUp("LONG", true, 0, Low[0] - 12 * TickSize, Color.Green);
							DrawText("My text" + CurrentBar, "30K", 0, Low[0] - 8 * TickSize, Color.Blue);
						}
						//Cancel order if price moves four ticks away without filling
						else if (entryOrder5 != null && entryOrder5.LimitPrice != null && Close[0] == entryOrder5.LimitPrice + 4 * TickSize
						&& entryOrder6 != null && entryOrder6.LimitPrice != null && Close[0] == entryOrder6.LimitPrice + 4 * TickSize)
						{		
						CancelOrder(entryOrder5);
						CancelOrder(entryOrder6);
						}
					
						if (entryOrder5 == null && entryOrder6 == null && FirstTickOfBar && CrossBelow(EMA(20), EMA(50), 1))
						{	
							entryOrder5 = EnterShortLimit(0, true, DefaultQuantity,  Open[0], "Short 1a");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "30K", 0, High[0] + 8 * TickSize, Color.Red);
							entryOrder6 = EnterShortLimit(0, true, DefaultQuantity,  Close[0], "Short 1b");
							DrawArrowDown("SHORT", true, 0, High[0] + 12 * TickSize, Color.Red);
							DrawText("My text" + CurrentBar, "30K", 0, High[0] + 8 * TickSize, Color.Red);
						}
						//Cancel order if price moves four ticks away without filling
						else if (entryOrder5 != null && entryOrder5.LimitPrice != null && Close[0] == entryOrder5.LimitPrice - 4 * TickSize
						&& entryOrder6 != null && entryOrder6.LimitPrice != null && Close[0] == entryOrder6.LimitPrice - 4 * TickSize)
						{	
						CancelOrder(entryOrder5);
						CancelOrder(entryOrder6);
						}
						#endregion
					}	
				}
			}
		}	
			protected override void OnOrderUpdate(IOrder order)
		{
			// Checks for all updates to entryOrder.
			if (entryOrder1 != null && entryOrder1.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder1 = null;	
				}	
					
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder2 != null && entryOrder2.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder2 = null;	
				}	
				
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder3 != null && entryOrder3.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder3 = null;	
				}	
				
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder4 != null && entryOrder4.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder4 = null;	
				}	
				
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder5 != null && entryOrder5.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder5 = null;	
				}	
			
			}	
			// Checks for all updates to entryOrder.
			if (entryOrder6 != null && entryOrder6.Token == order.Token)
			{	
				// Check if entryOrder is cancelled.
				if (order.OrderState == OrderState.Cancelled | order.OrderState == OrderState.Filled)
				{
					// Reset entryOrder back to null
					entryOrder6 = null;	
				}	
				
			}
		}	
				
		private void StopStrategy()
		{
			// If our Long Limit order is still active we will need to cancel it.
			CancelOrder(entryOrder1);
			CancelOrder(entryOrder2);
			CancelOrder(entryOrder3);
			CancelOrder(entryOrder4);
			CancelOrder(entryOrder5);
			CancelOrder(entryOrder6);
		
			// If we have a position we will need to close the position
			if (Position.MarketPosition == MarketPosition.Long)
				ExitLong();			
			else if (Position.MarketPosition == MarketPosition.Short)
				ExitShort();
		}	
		
	}
}

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
    /// Sample long/short after n bars up/down with buy stop at low/high previous bar plus/minus n ticks.
    /// </summary>
    [Description("Sample long/short after n bars up/down with buy stop at low/high previous bar plus/minus n ticks.Calculate on bar close set to false")]
    public class FDAX_ATM_cancel_2W : Strategy
    {
        #region Variables
                private string  atmStrategyIdL          = string.Empty;
                private string  atmStrategyIdS          = string.Empty;
               
                private int prevBarsPlusTicks = 1; // Default setting for PrevBarsPlusTicks
                private int prevBarsMinusTicks = 1; // Default setting for PrevBarsPlusTicks
               
                private string  orderIdL                                = string.Empty;
        		private string  orderIdS                                = string.Empty;
               
                private int orderBarL                           = 0;
                private int orderBarS                           = 0;
                private int entryBarL                           = 0;
                private int entryBarS                           = 0;
               
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            EntriesPerDirection = 1;
			EntryHandling = EntryHandling.AllEntries; 
			CalculateOnBarClose = false;
            TraceOrders = true;
            TimeInForce = Cbi.TimeInForce.Day;
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
            // **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'TF_long_3W_3BR' and 'TF_short_3W_3BR' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK  you can create your own****
                        if (orderIdL.Length == 0  
                                && atmStrategyIdL.Length == 0
								&& FirstTickOfBar
                                && Close [3] <= Open [3]
								&& Close [2] <= Open [2]
								&& Close [1] <= Open [1]
							    && High  [1] + PrevBarsPlusTicks * TickSize > GetCurrentBid())
                               
                        {
                                atmStrategyIdL = GetAtmStrategyUniqueId();
                                orderIdL = GetAtmStrategyUniqueId();
                                orderBarL = CurrentBar;
                                AtmStrategyCreate(Cbi.OrderAction.Buy, OrderType.Stop, 0, High[1] + PrevBarsPlusTicks * TickSize, TimeInForce.Day, orderIdL, "FDAX_long_5R_3BR", atmStrategyIdL);	
			    
						}
                        if (orderIdS.Length == 0  
                                && atmStrategyIdS.Length == 0  
                                && FirstTickOfBar
                				&& Close [3] >= Open [3]
								&& Close [2] >= Open [2]
								&& Close [1] >= Open [1]
                				&& Low   [1] - PrevBarsMinusTicks * TickSize < GetCurrentAsk())
                               
                        {
                                atmStrategyIdS = GetAtmStrategyUniqueId();
                                orderIdS = GetAtmStrategyUniqueId();
                                orderBarS = CurrentBar;
                                AtmStrategyCreate(Cbi.OrderAction.Sell, OrderType.Stop, 0, Low[1] - prevBarsMinusTicks * TickSize, TimeInForce.Day, orderIdS, "FDAX_short_5R_3BR", atmStrategyIdS);
                 
						}
                       
////////////////////////////////////////////////////////////////////////////////
                        // Check for a pending entry order
                        if (orderIdL.Length > 0)
                        {
							string[] statusL = GetAtmStrategyEntryOrderStatus(orderIdL);
							 
							// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
                            if (statusL.GetLength(0) > 0)
								
                                //add in by kz
                                if ((CurrentBar - orderBarL) >= 1)
									
                                
								{
                                        AtmStrategyCancelEntryOrder(orderIdL);
                                       
                                        orderIdL = string.Empty;
                                }
                                
                                {
                                        // Print out some information about the order to the output window
                                        Print("The entry order average fill price is:\t" + statusL[0]);
                                        Print("The entry order filled amount is:\t" + statusL[1]);
                                        Print("The entry order order state is:\t" + statusL[2]);

                                        // If the order state is terminal, reset the order id value
                                        if (statusL[2] == "Filled" || statusL[2] == "Cancelled" || statusL[2] == "Rejected")
                                                orderIdL = string.Empty;
                                }
                        } // If the strategy has terminated reset the strategy id
                        else if (atmStrategyIdL.Length > 0 &&
                                GetAtmStrategyMarketPosition(atmStrategyIdL) == Cbi.MarketPosition.Flat)
                                atmStrategyIdL = string.Empty;
////////////////////////////////////////////////////////////////////////////////
						// Check for a pending entry order
                        if (orderIdS.Length > 0)
							
                        {
							string[] statusS = GetAtmStrategyEntryOrderStatus(orderIdS);
							
							// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
                                if (statusS.GetLength(0) > 0)
									
                                //add in by kz
                                if ((CurrentBar - orderBarS) >= 1)
                                
								{
                                        AtmStrategyCancelEntryOrder(orderIdS);
                                       
                                        orderIdS = string.Empty;
                                }
                                
                                {
                                        // Print out some information about the order to the output window
                                        Print("The entry order average fill price is:\t" + statusS[0]);
                                        Print("The entry order filled amount is:\t" + statusS[1]);
                                        Print("The entry order order state is:\t\t" + statusS[2]);

                                        // If the order state is terminal, reset the order id value
                                        if (statusS[2] == "Filled" || statusS[2] == "Cancelled" || statusS[2] == "Rejected")
                                                orderIdS = string.Empty;
                                }
                        } // If the strategy has terminated reset the strategy id
                        else if (atmStrategyIdS.Length > 0 &&
                                GetAtmStrategyMarketPosition(atmStrategyIdS) == Cbi.MarketPosition.Flat)
                                atmStrategyIdS = string.Empty;
////////////////////////////////////////////////////////////////////////////////

                        if (atmStrategyIdL.Length > 0 &&
                                (CurrentBar - orderBarL) >= 8//added in
                                )
                        {
                                
                                // Print some information about the strategy to the output window
                                Print("The current ATM Strategy market position is:\t" + GetAtmStrategyMarketPosition(atmStrategyIdL));
                                Print("The current ATM Strategy position quantity is:\t" + GetAtmStrategyPositionQuantity(atmStrategyIdL));
                                Print("The current ATM Strategy average price is:\t" + GetAtmStrategyPositionAveragePrice(atmStrategyIdL));
                                Print("The current ATM Strategy Unrealized PnL is:\t$" + FormatPrice(GetAtmStrategyUnrealizedProfitLoss(atmStrategyIdL)));
                        }
////////////////////////////////////////////////////////////////////////////////

                        if (atmStrategyIdS.Length > 0 &&
                                (CurrentBar - orderBarS) >= 8//added in
                                )
                        {
                                
                                // Print some information about the strategy to the output window
                                Print("The current ATM Strategy market position is:\t" + GetAtmStrategyMarketPosition(atmStrategyIdS));
                                Print("The current ATM Strategy position quantity is:\t" + GetAtmStrategyPositionQuantity(atmStrategyIdS));
                                Print("The current ATM Strategy average price is:\t" + GetAtmStrategyPositionAveragePrice(atmStrategyIdS));
                                Print("The current ATM Strategy Unrealized PnL is:\t$" + FormatPrice(GetAtmStrategyUnrealizedProfitLoss(atmStrategyIdS)));
                        }
                       
        }//end of OnBarUpdate
               
                #region FormatPrice
                //this piece of code courtesy of eDanny?
                private string FormatPrice(double iVal)
        {
            return Bars.Instrument.MasterInstrument.FormatPrice(iVal);
        }
                #endregion
               

        #region Properties
                [Description("")]
        [GridCategory("Parameters")]
        public int PrevBarsPlusTicks
        {
            get { return prevBarsPlusTicks; }
            set { prevBarsPlusTicks = Math.Max(1, value); }
        }
               
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



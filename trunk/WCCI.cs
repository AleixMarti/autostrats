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
    [Description("A strategy to trade CCI_Forecaster signals using an ATM Strategy")]
    public class WCCI_AT : Strategy
    {
        #region Variables
		private string	atmStrategyId		= string.Empty;
		private string	orderId				= string.Empty;
		private string aTMStrategy			= string.Empty;
		private string aTMAltStrategy		= string.Empty;
		private OrderType typeEntry = OrderType.Limit;
		private OrderType snREntry = OrderType.Market;
		private int limitOffset = 1;
		private int stopOffset	= 1;
 		private int entrybar;
		private string from_time = "9:30 AM"; //Time to begin trading
		private string to_time = "3:30 PM"; //Time to end trading
		private int start_time; //Conversion variable for time to begin trading
		private int end_time; //Conversion variable for time to end trading
		private DateTime end_time1;
		private DateTime end_time2;
		private string from_time_1 = string.Empty; //Time to begin trading
		private string to_time_1 = "3:30 PM"; //Time to end trading
		private int start_time_1; //Conversion variable for time to begin trading
		private int end_time_1; //Conversion variable for time to end trading
		private DateTime end_time3;
		private DateTime end_time4;
		private int tradeExitCross = 100;
		private bool exitCandle = false;
		private bool stopReverse = false;
		private bool SnR;
		private int SnRBar;
		private bool offEndCandle = true;
		
		//CCI Forecaster 
		private string	dummy			= "===================================";
		private double 	extremes		= 185;
        private double	rejectLevel		= 100; // Default setting for RejectLevel
		private int		trendLength		= 12;
		private int 	periods			= 14;
		private double	minZLRPts   	= 15; //Default minimum ZLR change
		private double	maxSignalValue	= 120;
		private double	minZLRValue		= 0;
		private double	famirLimits		= 50;
		private double	famirHookMin	= 5;
		private double	vTHFEPenetrationLevel = 150;// Level that the CCI must penetrate through to reach the swing high/low
		private double	vMaxSwingPoint	= 150; // Max high or low for swing back to trend after swing high/low
		private int 	vMaxSwingBars	= 10;
		private	string	usePoints		= "Points";
		private double	usePointsAmount	= 10;
		private int		chopCrosses		= 4;
		private double	minCCIPts   	= 0; //Default minimum CCI change for all non-ZLR signals
		private double	gstColor		= 2.5;
		private bool 	useJavierSideways = false;
		private int 	sidewaysboundary = 100;

		CCI_Forecaster_DE ccif;
		
		private bool trade_long = true; //Take long trades
		private bool trade_short = true; //Take short trades
		private bool trade_sideways = false;// Trade in sideways markets?
		private bool trade_zlr = true; // Trade ZLR pattern
		private bool trade_famir = true; // Trade Famir pattern
		private bool trade_vegas = true; // Trade Vegas pattern
		private bool trade_gb = true; // Trade GB pattern
		private bool trade_tt = true; // Trade TT pattern
		private bool trade_ghost = true; // Trade Ghost pattern
		
		private string[] status;
		private int trade = 0;
		private int closeat = 0;
		private bool printStats = true;
		
		private double projectedhigh;
		private double projectedlow;
		private double oldprojectedhigh = 0;
		private double oldprojectedlow = 0;
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
	    }
		
        protected override void OnBarUpdate()
        {
			// Make sure this strategy does not execute against historical data
			if (Historical) return;
			
			projectedhigh = Low[0]+Bars.Period.Value*TickSize;
			projectedlow = High[0]-Bars.Period.Value*TickSize;
			
Print("================   Begin   ================");
 			start_time = ToTime(Convert.ToDateTime(from_time));//Convert String to Time for start time
			end_time = ToTime(Convert.ToDateTime(to_time).AddSeconds(-1));//ToTime(end_time2);//Finally convert end time to Time
			//Second trading session
			if(!string.IsNullOrEmpty(from_time_1))
			{	start_time_1 = ToTime(Convert.ToDateTime(from_time_1));
				end_time_1 = ToTime(Convert.ToDateTime(to_time_1).AddSeconds(-1));//ToTime(end_time2);//Finally convert end time to Time
			}
			CCI_Forecaster_DE ccif = CCI_Forecaster_DE(chopCrosses,extremes,famirHookMin,famirLimits,gstColor,maxSignalValue,minCCIPts,minZLRPts,minZLRValue,periods,rejectLevel,sidewaysboundary,trendLength,useJavierSideways,usePoints,usePointsAmount,vMaxSwingBars,vMaxSwingPoint,vTHFEPenetrationLevel);

			Print(Time[0] + ",  " + Instrument.FullName + ",  " + "Bar #" + CurrentBar.ToString() +",  " + orderId.Length + ",  " + atmStrategyId.Length + "-" + (atmStrategyId.Length > 0 ? GetAtmStrategyMarketPosition(atmStrategyId).ToString() : "Flat") + ",  Trade Signal = " + trade + ",  Sideways Market?: " + ccif.SidewaysMkt[0] + ",  Trade in Sideways Market?: " + SidewaysTrades + 
			",  Start Time: " + start_time + ",  End Time: " + end_time + ",  Start Time 2: " + start_time_1 + ",  End Time 2: " + end_time_1);
			
			
#region Check to see if there has been a 100 line cross or if our position has been closed
			if (atmStrategyId.Length > 0)// Were we in a trade, and if so, are we still in it?  If not, reset orderId and atmStrategyId to String.Empty
			{
				if(orderId.Length >0) status = GetAtmStrategyEntryOrderStatus(orderId);
	/////////////////  100 Line Cross  //////////////////////////
				if((GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Long && CCI(14)[1] > TradeExitCross && CCI(14)[0] < TradeExitCross)
					||(GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Short && CCI(14)[1] < -TradeExitCross && CCI(14)[0] > -TradeExitCross))// Close the position on a 100 line cross
				{
//Print("122   100 X");
					if(GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Long && projectedlow != oldprojectedlow)
						{	
							AtmStrategyChangeStopTarget(0, projectedlow-TickSize, "STOP1", atmStrategyId);
							AtmStrategyChangeStopTarget(0, projectedlow-TickSize, "STOP2", atmStrategyId);
							AtmStrategyChangeStopTarget(0, projectedlow-TickSize, "STOP3", atmStrategyId);
							oldprojectedlow = projectedlow;
						}
					else 
					if(GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Short && projectedhigh != oldprojectedhigh)
//Print("141   100 X Close Short");
						{	AtmStrategyChangeStopTarget(0, projectedhigh+TickSize, "STOP1", atmStrategyId);
							AtmStrategyChangeStopTarget(0, projectedhigh+TickSize, "STOP2", atmStrategyId);
							AtmStrategyChangeStopTarget(0, projectedhigh+TickSize, "STOP3", atmStrategyId);
							oldprojectedhigh = projectedhigh;
						}
				}
				Print("The current ATM Strategy Market Position for " + Instrument.FullName + " is: " + GetAtmStrategyMarketPosition(atmStrategyId));
				Print("The current ATM Strategy Position Quantity for " + Instrument.FullName + " is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
				Print("The current ATM Strategy Average Price for " + Instrument.FullName + " is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId).ToString("f4"));
				Print("The current ATM Strategy Unrealized PnL for " + Instrument.FullName + " is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId).ToString("f4"));
				Print("The current ATM Strategy Realized PnL for " + Instrument.FullName + " is: " + GetAtmStrategyRealizedProfitLoss(atmStrategyId).ToString("f4")); 
				
				//Are we flat following a filled, cancelled or rejected order (don't do this if order is still pending)?
				if(GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Flat && (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected"))
					{	
//Print("162   Position:  Flat,     Order: " + status[2]);
						atmStrategyId = string.Empty;
						orderId = string.Empty;
						oldprojectedlow = 0;
						oldprojectedhigh = 0;
					}
			}
			#endregion
			
  			#region Set Signal Flag
			trade = 0;
			trade = ccif.WCCIPattern[0] * ccif.WCCIDirection[0];
			closeat = ccif.WCCICloseAt[0];
			switch(trade)
			{
				case 6:
					if(!Trade_Ghost || !TakeLongTrades)
						trade = 0;
					break;
				case 5:
					if(!Trade_TT || !TakeLongTrades)
						trade = 0;
					break;
				case 4:
					if(!Trade_GB || !TakeLongTrades)
						trade = 0;
					break;
				case 3:
					if(!Trade_Vegas || !TakeLongTrades)
						trade = 0;
					break;
				case 2:
					if(!Trade_Famir || !TakeLongTrades)
						trade = 0;
					break;
				case 1:
					if(!Trade_ZLR || !TakeLongTrades)
						trade = 0;
					break;
				case -1:
					if(!Trade_ZLR || !TakeShortTrades)
						trade = 0;
					break;
				case -2:
					if(!Trade_Famir || !TakeShortTrades)
						trade = 0;
					break;
				case -3:
					if(!Trade_Vegas || !TakeShortTrades)
						trade = 0;
					break;
				case -4:
					if(!Trade_GB || !TakeShortTrades)
						trade = 0;
					break;
				case -5:
					if(!Trade_TT || !TakeShortTrades)
						trade = 0;
					break;
				case -6:
					if(!Trade_Ghost || !TakeShortTrades)
						trade = 0;
					break;
				default:
					break;
			}
			#endregion
			
			//Stop and Reverse on a losing trade with opposing signal
/*			if(atmStrategyId.Length > 0 && StopReverse && GetAtmStrategyUnrealizedProfitLoss(atmStrategyId) < 0 
			&& 	(GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Long ? trade < 0 : trade > 0)
			&& (ccif.SidewaysMkt[0] ? SidewaysTrades : 1==1)//Do not allow if in sideways mkt or outside trading hours
			&& ( !string.IsNullOrEmpty(from_time_1) ? ((ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time) || (ToTime(Time[0]) >= start_time_1 && ToTime(Time[0]) <= end_time_1)) : (ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time))
				)
			{
//Print("236   SnR Check 1");
				SnR = true;
				SnRBar = CurrentBar;
				AtmStrategyClose(atmStrategyId);
			}
			else 
			{	SnR = false;
			}
*/				
			#region Order Entry
			if (orderId.Length == 0 && atmStrategyId.Length == 0 && trade > 0 && (ccif.SidewaysMkt[0] ? SidewaysTrades : 1==1)
				&& ( !string.IsNullOrEmpty(from_time_1) ? ((ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time) || (ToTime(Time[0]) >= start_time_1 && ToTime(Time[0]) <= end_time_1)) : (ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time))
				&& (!OffEndCandle ? Close[0]>Low[0] : 1==1)
				)
			{
				int entrybar = CurrentBar;
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(OrderAction.Buy, TypeEntry, (TypeEntry != OrderType.Market ? Close[0]+LimitOffset*TickSize : 0),
					(TypeEntry != OrderType.StopLimit ? 0 : (Close[0]+StopOffset*TickSize)), TimeInForce.Day, orderId,  
					(closeat == 1 ? ATMStrategy : ATMAltStrategy), atmStrategyId);
				if(PrintStats)
				{
					Print(Instrument.FullName +","+ToDay(Time[0])+","+ToTime(Time[0])+","+(trade>1?"L":"S")+","+trade+","+Close[0]+","+(Close[0]==Low[0]?-1:1)+","+CCI(Periods)[0].ToString("f0")+","+(CCI(Periods)[0]-CCI(Periods)[1]).ToString("f0"));
				}
				else
				{
					Print(Time[0] + ",  " + Instrument.FullName + ",  " + orderId.Length + ",  " + atmStrategyId.Length + ",  Buy, " + TypeEntry + ",  " + (TypeEntry != OrderType.Market ? (Close[0]+LimitOffset*TickSize).ToString() : "0") 
	+ ",  " + (TypeEntry != OrderType.StopLimit ? "0" : (Close[0]+StopOffset*TickSize).ToString()) + ",   " + (closeat == 1 ? ATMStrategy : ATMAltStrategy));

				}
			}
			else
			if (orderId.Length == 0 && atmStrategyId.Length == 0 && trade < 0 && (ccif.SidewaysMkt[0] ? SidewaysTrades : 1==1)
				&& (!string.IsNullOrEmpty(from_time_1) ? ((ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time) || (ToTime(Time[0]) >= start_time_1 && ToTime(Time[0]) <= end_time_1)) : (ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time))
				&& (!OffEndCandle ? Close[0]<High[0] : 1==1)
				)
			{	
				entrybar = CurrentBar;
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(OrderAction.SellShort, TypeEntry, (TypeEntry != OrderType.Market ? Close[0]-LimitOffset*TickSize : 0),
					(TypeEntry != OrderType.StopLimit ? 0 : (Close[0]-StopOffset*TickSize)), TimeInForce.Day, orderId, 
					(closeat == -1 ? ATMStrategy : ATMAltStrategy), atmStrategyId);
				if(PrintStats)
				{
					Print(Instrument.FullName +","+ToDay(Time[0])+","+ToTime(Time[0])+","+(trade>1?"L":"S")+","+trade+","+Close[0]+","+(Close[0]==Low[0]?-1:1)+","+CCI(Periods)[0].ToString("f0")+","+(CCI(Periods)[0]-CCI(Periods)[1]).ToString("f0"));
				}
				else
				{
					Print(Time[0] + ",  " + Instrument.FullName + ",  " + orderId.Length + ",  " + atmStrategyId.Length + ",  SellShort, " + TypeEntry + ",  " + (TypeEntry != OrderType.Market ? (Close[0]-LimitOffset*TickSize).ToString() : "0") 
	+ ",  " + (TypeEntry != OrderType.StopLimit ? "0" : (Close[0]-StopOffset*TickSize).ToString()) + ",   " + (closeat == -1 ? ATMStrategy : ATMAltStrategy));

				}
			}
			#endregion

			//Cancel unfilled orders on new bar
			// Check for a pending entry order
			if (orderId.Length > 0)
			{
				status = GetAtmStrategyEntryOrderStatus(orderId);
Print("294   Check 1,   " + orderId + ",   Order Status:   " + status[2]);
				if(CurrentBar==entrybar+1 && (status[2] == "Accepted" || status[2] == "Working" || status[2] == "Pending"))
				{
Print("292   Cancelled Order");
					AtmStrategyCancelEntryOrder(orderId);
					atmStrategyId = string.Empty;
					orderId = string.Empty;
				}
                
			} // If the strategy has terminated reset the strategy id
			else 
			if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == MarketPosition.Flat)
				{
Print("302   reset the strategy id");
				atmStrategyId = string.Empty;
				orderId = string.Empty;
				}
				// You can change the stop price
//				if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
//				AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP1", atmStrategyId);
//			if(SnR) StopnReverse();
Print("================   End   ================");			
        }
/*		
		#region Stop and Reverse
		protected void StopnReverse()
		{//Check for a Stop and Reverse situation, else ignore
			int n=0;
			while(GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat && n<1000)
			{
				++n;
			}
			Print(Time[0]+",   while loops: "+n);
			if(GetAtmStrategyMarketPosition(atmStrategyId)==MarketPosition.Flat)
			{
				if (orderId.Length == 0 && trade > 0 && (ccif.SidewaysMkt[0] ? SidewaysTrades : 1==1)
					&& ( !string.IsNullOrEmpty(from_time_1) ? ((ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time) || (ToTime(Time[0]) >= start_time_1 && ToTime(Time[0]) <= end_time_1)) 
						: (ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time))
					&& (!OffEndCandle ? Close[0]>Low[0] : 1==1)
					)
				{
					Print("SnR: " + Time[0] + ",  " + Instrument.FullName + ",  " + orderId.Length + ",  " + atmStrategyId.Length + ",  Buy, Market, 0, 0,   " + (closeat == -1 ? ATMStrategy : ATMAltStrategy));
					int entrybar = CurrentBar;
					atmStrategyId = ATMStrategy+"-"+GetAtmStrategyUniqueId();
					orderId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(Action.Buy, SnREntry, (SnREntry != OrderType.Market ? Close[0]+LimitOffset*TickSize : 0),
					(SnREntry != OrderType.StopLimit ? 0 : (Close[0]+StopOffset*TickSize)), TimeInForce.Day, orderId,  
					(closeat == 1 ? ATMStrategy : ATMAltStrategy), atmStrategyId);
					
				}
				else
				if (orderId.Length == 0 && trade < 0 && (ccif.SidewaysMkt[0] ? SidewaysTrades : 1==1)
					&& (!string.IsNullOrEmpty(from_time_1) ? ((ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time) || (ToTime(Time[0]) >= start_time_1 && ToTime(Time[0]) <= end_time_1)) 
						: (ToTime(Time[0]) >= start_time && ToTime(Time[0]) <= end_time))
					&& (!OffEndCandle ? Close[0]<High[0] : 1==1)
					)
				{	
					Print("SnR: " + Time[0] + ",  " + Instrument.FullName + ",  " + orderId.Length + ",  " + atmStrategyId.Length + ",  SellShort, , 0, 0,   " + (closeat == -1 ? ATMStrategy : ATMAltStrategy));
					entrybar = CurrentBar;
					atmStrategyId = ATMStrategy + "-" + GetAtmStrategyUniqueId();
					orderId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(Action.SellShort, SnREntry, (SnREntry != OrderType.Market ? Close[0]-LimitOffset*TickSize : 0),
					(SnREntry != OrderType.StopLimit ? 0 : (Close[0]-StopOffset*TickSize)), TimeInForce.Day, orderId, 
					(closeat == -1 ? ATMStrategy : ATMAltStrategy), atmStrategyId);
				}
			}
		}
		#endregion
*/
        #region Properties
		[Description("ATM Strategy name")]
 		[Gui.Design.DisplayName("\t\t\tATM Strategy")]
		[Category("Parameters")]
        public string ATMStrategy
        {
            get { return aTMStrategy; }
            set { aTMStrategy = value; }
        }
		
		[Description("ATM Strategy name for trades when the candle closes at the off end (Low for Longs and Highs for Shorts)")]
 		[Gui.Design.DisplayName("\t\t\tATM Strategy, Off Candle ")]
		[Category("Parameters")]
        public string ATMAltStrategy
        {
            get { return aTMAltStrategy; }
            set { aTMAltStrategy = value; }
        }
		
 		[Description("Type of order to be placed")]
		[Gui.Design.DisplayName("\t\tATM Strategy, Order Type")]
		[Category("Parameters")]
        public OrderType TypeEntry
        {
         get { return typeEntry ; }
		 set{typeEntry = (value == OrderType.Market ? OrderType.Market : (value == OrderType.StopLimit ? OrderType.StopLimit : OrderType.Limit));}		
		}
		
 		[Description("Type of order to be placed on Stop and Reverse")]
		[Gui.Design.DisplayName("\t\tATM Strategy, SAR Order Type")]
		[Category("Parameters")]
        public OrderType SnREntry
        {
         get { return snREntry ; }
		 set{snREntry = (value == OrderType.Market ? OrderType.Market : (value == OrderType.StopLimit ? OrderType.StopLimit : OrderType.Limit));}		
		}
		
 		[Description("Number of ticks for Limit Order from the trigger bar CLOSE (1 tick should equate to next bar open).")]
		[Gui.Design.DisplayName("\t\tOrder Offset, Limit")]
		[Category("Parameters")]
        public int LimitOffset
        {
         get { return limitOffset ; }
		 set{limitOffset = value;}		
		}
		
 		[Description("Number of CCI points needed between the trend line and the right shoulder for it to have 'Color').")]
		[Gui.Design.DisplayName("\t\tGhost Color")]
		[Category("Parameters")]
        public double GstColor
        {
         get { return gstColor ; }
		 set{gstColor = value;}		
		}
				
 		[Description("Number of ticks for the StopLimit Order Stop price from the trigger bar projected High or Low (1 tick should equate to next bar open).")]
		[Gui.Design.DisplayName("\t\tOrder Stop Offset, StopLimit")]
		[Category("Parameters")]
        public int StopOffset
        {
         get { return stopOffset ; }
		 set{stopOffset = value;}		
		}
		
		[Description("Exit the trade when the CCI crosses this line.")]
 		[Gui.Design.DisplayName("\t\tXit Cross Line")]
		[Category("Parameters")]
        public int TradeExitCross
        {
            get { return tradeExitCross; }
            set { tradeExitCross = Math.Max(0, Math.Min(1000, value)); }
        }
		
		[Description("Exit the trade when the CCI crosses the Exit Cross Line on any candle, both with and against the trade direction.  If True, any cross of 100 will exit the trade. If False, Only crosses on a candle against the trade direction will exit the trade.  In this case the stops will be moved to the high/low of the crossing candle.")]
 		[Gui.Design.DisplayName("\t\tXit on cross, all candles")]
		[Category("Parameters")]
        public bool ExitCandle
        {
            get { return exitCandle; }
            set { exitCandle = value; }
        }
		
		[Description("Start Time for trading session 1.")]
 		[Gui.Design.DisplayName("\t\tSession 1 Begin Time")]
        [Category("Parameters")]
        public string Trade_Begin_Time1
        {
            get { return from_time; }
            set { from_time =  value; }
        } 
		
		[Description("End Time for trading session 1.")]
 		[Gui.Design.DisplayName("\t\tSession 1 End Time")]
        [Category("Parameters")]
        public string Trade_End_Time1
        {
            get { return to_time; }
            set { to_time =  value; }
        } 
		
		[Description("Start Time for trading session 2.  Leave blank if there is no 2nd session.")]
 		[Gui.Design.DisplayName("\t\tSession 2 Begin Time")]
        [Category("Parameters")]
        public string Trade_Begin_Time2
        {
            get { return from_time_1; }
            set { from_time_1 =  value; }
        } 
		
		[Description("End Time for trading session 2.")]
 		[Gui.Design.DisplayName("\t\tSession 2 End Time")]
        [Category("Parameters")]
        public string Trade_End_Time2
        {
            get { return to_time_1; }
            set { to_time_1 =  value; }
        } 
		
 		[Description("Trade ZLR Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade ZLRs?")]
		public bool Trade_ZLR
		{
		get { return trade_zlr; }
		set { trade_zlr = value; }
		}
		
		[Description("Trade Famir Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade Famirs?")]
		public bool Trade_Famir
		{
		get { return trade_famir; }
		set { trade_famir = value; }
		}
		
		[Description("Trade Vegas Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade Vegas'?")]
		public bool Trade_Vegas
		{
		get { return trade_vegas; }
		set { trade_vegas = value; }
		}
		
		[Description("Trade GB Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade GB 100s?")]
		public bool Trade_GB
		{
		get { return trade_gb; }
		set { trade_gb = value; }
		}
		
		[Description("Trade TT Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade Tonys?")]
		public bool Trade_TT
		{
		get { return trade_tt; }
		set { trade_tt = value; }
		}
		
		[Description("Trade Ghost Pattern")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTrade Ghosts?")]
		public bool Trade_Ghost
		{
		get { return trade_ghost; }
		set { trade_ghost = value; }
		}
		
		[Description("Trade Long Trades")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTake Long Trades?")]
		public bool TakeLongTrades
		{
		get { return trade_long; }
		set { trade_long = value; }
		}
		
		[Description("Trade Short Trades")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTake Short Trades?")]
		public bool TakeShortTrades
		{
		get { return trade_short; }
		set { trade_short = value; }
		}
		
		[Description("Trade on signals with close opposite to trade direction? (Longs on a low close or shorts on a high close)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTake Off Candle Trades?")]
		public bool OffEndCandle
		{
		get { return offEndCandle; }
		set { offEndCandle = value; }
		}
		
		[Description("Trade during sideways markets?")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tTake Trades in sideways mkts?")]
		public bool SidewaysTrades
		{
		get { return trade_sideways; }
		set { trade_sideways = value; }
		}
		
		[Description("Stop and Reverse on losing positions with new signal in opposite direction?  The reversal will be order type 'SAR Order Type'.")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t\tStop and Reverse?")]
		public bool StopReverse
		{
		get { return stopReverse; }
		set { stopReverse = value; }
		}

		[Description("Print Entry and Exit CCI data?")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Print Stats?")]
		public bool PrintStats
		{
		get { return printStats; }
		set { printStats = value; }
		}
		
//CCI Forecaster

		[Description("Just a separator between Trade and CCI parameters")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("\t===================================")]
		public string Dummy
		{
			get { return dummy; }
			set { dummy = "==================================="; }
		}
		
		[Description("CCI must go below this value before a ZLR or Vegas can be signaled")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("CCI Trend Level")]
		public double RejectLevel
		{
			get { return rejectLevel; }
			set { rejectLevel = Math.Max(0, Math.Min(100, value)); }
		}
		
		[Description("Number of bars for a trend to be in place for GB100 trades to trigger")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Trend Length for GB100")]
		public int TrendLength
		{
			get { return trendLength; }
			set { trendLength = value; }
		}
		
		[Description("CCI Periods")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("CCI Periods")]
		public int Periods
		{
			get { return periods; }
			set { periods = Math.Max(2,value); }
		}
		
		[Description("Minimum CCI point change between ghost peaks and valleys")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Use Ghost Points/Percent Amount")]
		public double UsePointsAmount
		{
			get { return usePointsAmount; }
			set { usePointsAmount = Math.Max(0, Math.Max(1, value)); }
		}
		
		[Description("Type of ZigZag deviation (Points or Percent)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Use Ghost Points or Percent")]
		public string UsePoints
		{
			get { return usePoints; }
			set { if((value == "Points") || (value == "Percent"))
					{ usePoints = value;  }
				}
		}

		[Description("CCI must close above this value before before it is considered to have 'gone to extremes'.  This will also be the level at which a sideways market is reset.")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Extremes CCI Level")]
		public double Extremes
		{
			get { return extremes; }
			set { extremes = Math.Max(175, Math.Min(250, value)); }
		}

		[Description("Famir minimum CCI hook points (default = 3)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Famir, CCI Points for Hook")]
		public double FamirHookMin
		{
			get { return famirHookMin; }
			set { famirHookMin = Math.Max(0, Math.Min(10, value)); }
		}

		[Description("Famir CCI boundaries (default = 50)")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Famir, Max CCI Level")]
		public double FamirLimits
		{
			get { return famirLimits; }
			set { famirLimits = Math.Max(0, Math.Min(100, value)); }
		}

		[Description("Level that the CCI must penetrate through to reach the swing high/low.")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Vegas, Penetration level before swing high/low")]
		public double VTHFEPenetrationLevel
		{
			get { return vTHFEPenetrationLevel; }
			set { vTHFEPenetrationLevel = Math.Max(0, Math.Min(200, value)); }
		}
		
		[Description("Max high or low for swing back to trend after swing high/low (default = 100).  NOT SWING HIGH/LOW.")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Vegas, Max Swing after swing high/low")]
		public double VMaxSwingPoint
		{
			get { return vMaxSwingPoint; }
			set { vMaxSwingPoint = Math.Max(0, Math.Min(200, value)); }
		}
		
		[Description("Max # of bars between swing high/low and trigger (default = 10).")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Vegas, Max Swing bars")]
		public int VMaxSwingBars
		{
			get { return vMaxSwingBars; }
			set { vMaxSwingBars = Math.Max(0, Math.Min(200, value)); }
		}
		
		[Description("Minimum CCI point change before a ZLR can be signaled")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("ZLR, Minimum Point Change")]
		public double MinZLRPts
		{
			get { return minZLRPts; }
			set { minZLRPts = Math.Max(0, Math.Min(75, value)); }
		}

		[Description("Minimum CCI point change before a non ZLR can be signaled")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Minimum CCI Point Change")]
		public double MinCCIPts
		{
			get { return minCCIPts; }
			set { minCCIPts = Math.Max(0, Math.Min(100, value)); }
		}

		[Description("Maximum CCI value for a Pattern to be signaled")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Maximum Signal Level")]
		public double MaxSignalValue
		{
			get { return maxSignalValue; }
			set { maxSignalValue = Math.Max(75, Math.Min(200, value)); }
		}

		[Description("Minimum CCI value for a ZLR to be signaled")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("ZLR, Minimum Level")]
		public double MinZLRValue
		{
			get { return minZLRValue; }
			set { minZLRValue = Math.Max(0, Math.Min(100, value)); }
		}

		[Description("Numbers of 0 line crosses needed to declare a sideways market")]
		[Category("Parameters")]
		[Gui.Design.DisplayName("Sideways mkt crosses")]
		public int ChopCrosses
		{
			get { return chopCrosses; }
			set { chopCrosses = Math.Max(1, value); }
		}

			[Description("Use Javier's alternate sideways market indicator?")]
			[Category("Parameters")]
			[Gui.Design.DisplayName("Sideways mkt, Use Javier's alternate?")]
			public bool UseJavierSideways
			{
				get { return useJavierSideways; }
				set { useJavierSideways = value; }
			}
			
			[Description("Boundary limits between which sideways market crosses must remain.")]
			[Category("Parameters")]
			[Gui.Design.DisplayName("Sideways Market Boundary")]
			public int Sidewaysboundary
			{
				get { return sidewaysboundary; }
				set { sidewaysboundary  = Math.Max(1, value); }
			}

		#endregion
    }
}

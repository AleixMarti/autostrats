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
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Stoc test")]
    public class Stocastic8020 : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int myInput0 = 1; // Default setting for MyInput0
        private int lastbarcount = 0;
        private double _myAccountValue=0;
        private double currentAccountValue=0;
        private double minDev =0;
		private double bollingerDeviations =0;
        private int StdDevCount;
        private int trailingDevCount;
        private StdDev stddev;
         private SMA sma;
        // User defined variables (add any user defined variables below)
        //System.Diagnostics.Stopwatch stopWatch;
        private int closeAfter = 0;
        private int didLongAt = 0;
        private int didShortAt = 0;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
            closeAfter = 4;
            myAccountValue = 2500;
            int SmaCount = 6;
            StdDevCount = 14;
            trailingDevCount=12;
            minDev = 0.03;
            bollingerDeviations = 2.1;
            sma = this.SMA(SmaCount);
            
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            stddev = this.StdDev(Bars,StdDevCount);
            
            Stochastics sto = Stochastics(Bars,9,10,3);
		
			
            double stdDev1 = stddev.Value[0] * 1;
            double stdDev2 =  stddev.Value[0] * 2;
            double stdDev3 = stddev.Value[0] * 3;
            double currentSMA = sma.Value[0];
            double lastSMA = sma.Value[1];
            string shortName =  "BuyShort1";
            string longName =  "BuyLong1";
            if( lastbarcount < Bars.Count )
            {
                 
						
				if(sto.D[0] < 80 &&  sto.D[1]> 80)
				{			
					
					if(Position.MarketPosition == MarketPosition.Long) // get out of a long.
					{
						SetStopLoss(longName,CalculationMode.Price,GetCurrentBid(),false);
					}
					
					// Get in a Short
					SetStopLoss(shortName,CalculationMode.Price,Bollinger(bollingerDeviations,StdDevCount).Upper[0] + stdDev2,false);
					EnterShort(0,GetQuantityToPurchase(), shortName);
				}
				
				if( sto.D[0] > 20 && sto.D[1] < 20)
				{			
					
					if(Position.MarketPosition == MarketPosition.Short) // get out of a Short.
					{
						SetStopLoss(shortName,CalculationMode.Price,GetCurrentAsk(),false);
					}
					
					// Get in a Long
					SetStopLoss(longName,CalculationMode.Price,Bollinger(bollingerDeviations,StdDevCount).Lower[0] - stdDev2,false);
                    EnterLong(0,GetQuantityToPurchase(),longName);
				}					
					
                
            
			}
            
            lastbarcount = Bars.Count;
        }
        protected int GetQuantityToPurchase()
        {
            currentAccountValue = myAccountValue
                    + Performance.RealtimeTrades.TradesPerformance.GrossProfit
                    + Performance.RealtimeTrades.TradesPerformance.GrossLoss
                    - Performance.RealtimeTrades.TradesPerformance.Commission;
            return Convert.ToInt32(  ((currentAccountValue) *  .015) /  GetCurrentBid()  );
        }
        
        

        #region Properties
		[Description("")]
        [Category("Parameters")]
        public double myAccountValue
        {
            get {return _myAccountValue;}
            set {_myAccountValue = value;}
        }
        #endregion
    }
}

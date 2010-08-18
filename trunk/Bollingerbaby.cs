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
    [Description("Enter the description of your strategy here")]
    public class BollingerBaby : Strategy
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
               
               
                private int GetInShort =0;
                private int GetInLong = 0;
                private int BollingerIndicatorDuration = 0;
                private IOrder shortOrder = null;
                private IOrder longOrder = null;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = false;
            closeAfter = 4;
            myAccountValue = 40000;
            int SmaCount = 6;
            StdDevCount = 14;
            trailingDevCount=14;
            minDev = 0.03;
            bollingerDeviations = 2.0;
                        BollingerIndicatorDuration = 3;
            sma = this.SMA(SmaCount);
           
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
                {
            EaseOfMovement EMV = this.EaseOfMovement(Typical,12,10000);

            string shortName =  "BuyShort1";
            string longName =  "BuyLong1";
               
            if( lastbarcount < Bars.Count)
            {                          
                                GetInLong --;// decrement our duration for this indicator
                                GetInShort --;// decrement our duration for this indicator
               
                if(High[2] > Bollinger(Typical, bollingerDeviations,StdDevCount).Upper[2])
                {
                    if(  High[1] < Bollinger(Typical,bollingerDeviations,StdDevCount).Upper[1]) // SHORT POSITION
                    {                                                          
                                                GetInShort = BollingerIndicatorDuration;
                                               
                                                DrawArrowLine("shortLine"+lastbarcount.ToString(),true
                                                                                                                                                        ,1
                                                                                                                                                ,Bollinger(Typical,bollingerDeviations,StdDevCount).Upper[1]
                                                                                                                                                ,0
                                                                                                                                                ,Bollinger(Typical,bollingerDeviations,StdDevCount).Upper[1]
                                                                                                                                                ,Color.Aqua,DashStyle.Solid,1);
                    }
                }
                else if(Low[2] < Bollinger(Typical,bollingerDeviations,StdDevCount).Lower[2])
                {
                    if (  Low[1] > Bollinger(Typical,bollingerDeviations,StdDevCount).Lower[1]) // LONG POSITION
                    {
                                                GetInLong = BollingerIndicatorDuration;
                                               
                                                DrawArrowLine("longLine"+lastbarcount.ToString(),true
                                                                                                                                                        ,1
                                                                                                                                                ,Bollinger(Typical,bollingerDeviations,StdDevCount).Lower[1]
                                                                                                                                                ,0
                                                                                                                                                ,Bollinger(Typical,bollingerDeviations,StdDevCount).Lower[1]
                                                                                                                                                ,Color.Aqua,DashStyle.Solid,1);
                    }
                }
                               
                                // update stop loss for multibar trading  -- and profit target        
                                if(Position.MarketPosition == MarketPosition.Long) // if LONG
                                {
                                       
                                        if(Low[1] > EMA(Typical,14).Value[1] &&
                                                Low[2] < EMA(Typical,14).Value[1]  ) // If Low Crossed the Middle
                                        {
                                                SetStopLoss(longName,CalculationMode.Price,Math.Min(Low[0]-.02,Math.Min(Low[1]-.02, EMA(Typical,14).Value[1])) ,false);
                                                DrawDiamond("GetLong"+lastbarcount.ToString(),true,0,Math.Min(Low[1]-0.02,EMA(Typical,14).Value[1]),Color.Gold);
                                        }
                                        // try to get out.
                                        if(Low[3] > EMA(Typical,14).Value[3]
                                                && Low[2] <EMA(Typical,14).Value[2]
                                                && Low[1] <EMA(Typical,14).Value[1]
                                                && Low[0] <EMA(Typical,14).Value[0])
                                        {
                                                ExitLongLimit(0,true,Position.Quantity,Typical[1],"ExitLong"+Bars.Count.ToString(),longName);
                                                DrawDiamond("GetLong"+lastbarcount.ToString(),true,0,Typical[1],Color.Gold);
                                        }
                                                                               
                                }
                                // update stop loss  -- and profit target  
                                if(Position.MarketPosition == MarketPosition.Short) // IF SHORT
                                {
                                        if(High[1] <EMA(Typical,14).Value[1] &&
                                                High[2] > EMA(Typical,14).Value[1] ) // If High Crossed the Middle
                                        {
                                                SetStopLoss(shortName,CalculationMode.Price,Math.Max(High[0]+.02,Math.Max(High[1]+.02, EMA(Typical,14).Value[1])),false);                                      
                                                DrawDiamond("GetShort"+lastbarcount.ToString(),true,0,Math.Max(High[1]+0.02,EMA(Typical,14).Value[1]),Color.YellowGreen);
                                        }
                                        // try to get out.
                                        if(High[3] <EMA(Typical,14).Value[3]
                                                && High[2] >EMA(Typical,14).Value[2]
                                                && High[1] >EMA(Typical,14).Value[1]
                                                && High[0] >EMA(Typical,14).Value[0])
                                        {
                                                ExitShortLimit(0,true,Position.Quantity,Typical[1],"ExitShort"+Bars.Count.ToString(),shortName);
                                                DrawDiamond("GetShort"+lastbarcount.ToString(),true,0,Typical[1],Color.YellowGreen);
                                        }
                                       
                                }
                               
                       
                       
                       
//                      
//                      bool emvGetInShortOK = false;
//                      bool emvGetInLongOK = false;
//                      
//                      if(EMV[0] > 0 )
//                              emvGetInLongOK = true;
//            if(EMV[0] < 0 )
//                              emvGetInShortOK = true;
                                if(Position.MarketPosition == MarketPosition.Flat) // IF Flat
                                {
                                        // prevent stupid stuff.
                                        SetStopLoss(shortName,CalculationMode.Price,Bollinger(Typical,bollingerDeviations+2,StdDevCount).Upper[1],false);
                                        SetStopLoss(longName,CalculationMode.Price, Bollinger(Typical,bollingerDeviations+2,StdDevCount).Lower[1] ,false);
                                }
                                 
                        // DO OUR GET INS
                                if( Stochastics(Typical, 14,28,3).D[1] >= Stochastics(Typical, 14,28,3).K[1] && Stochastics(Typical, 14,28,3).D[1] > 79 && Stochastics(Typical, 14,28,3).K[1] > 79)
                                if(GetInShort >0  && Position.MarketPosition == MarketPosition.Flat )
                                {
                                        EnterShortLimit(0,true,GetQuantityToPurchase(),Math.Max(Bollinger(Typical,bollingerDeviations,StdDevCount).Middle[1],GetCurrentBid()-0.01), shortName);
                                        SetStopLoss(shortName,CalculationMode.Price,Bollinger(Typical,bollingerDeviations+2,StdDevCount).Upper[1],false);                                              
                                        DrawDiamond("GetShort-Initial"+lastbarcount.ToString(),true,0,Bollinger(Typical,bollingerDeviations+1,StdDevCount).Upper[1],Color.Chocolate);
                                        GetInShort = 0;
                                        //emvGetInShortOK= false;
                                }
                               
                                if( Stochastics(Typical, 14,28,3).D[1] <= Stochastics(Typical, 14,28,3).K[1] && Stochastics(Typical, 14,28,3).D[1] < 21 && Stochastics(Typical, 14,28,3).K[1] < 21 )
                                if(GetInLong >0  && Position.MarketPosition == MarketPosition.Flat )
                                {
                                        EnterLongLimit(0,true,GetQuantityToPurchase(),Math.Min( Bollinger(Typical,bollingerDeviations,StdDevCount).Middle[1],GetCurrentAsk()+0.01),longName);
                                        SetStopLoss(longName,CalculationMode.Price, Bollinger(Typical,bollingerDeviations+2,StdDevCount).Lower[1] ,false);
                                        DrawDiamond("GetLong-Initial"+lastbarcount.ToString(),true,0,Bollinger(Typical,bollingerDeviations+1,StdDevCount).Lower[1],Color.Chocolate);    
                                        GetInLong = 0;
                                        //emvGetInLongOK = false;
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
            return Convert.ToInt32(  ((currentAccountValue) *  .90) /  GetCurrentBid()  );
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

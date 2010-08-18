#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
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
    public class MoneyMakerTest : Strategy
    {
        #region Variables
        private int lastbarcount = 0; // BH:Used to make sure that new bar has been added
        int quantityToPurchase = 0; // BH:Used to calculate # of shares to place buy orders for
        double _myAccountValue = 0; // BH:Variable behind "myAccountValue" parameter
		List<OrderGroupTestTest> EntryOrders = new List<OrderGroupTestTest>();
		List<OrderGroupTestTest> ProfitOrders = new List<OrderGroupTestTest>();
		List<OrderGroupTestTest> StopLossOrders = new List<OrderGroupTestTest>();
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            SetProfitTarget("BuyStuff1", CalculationMode.Ticks, 1);
            SetStopLoss("BuyStuff1", CalculationMode.Ticks, 1, false);
			SetProfitTarget("BuyStuff2", CalculationMode.Ticks, 1);
            SetStopLoss("BuyStuff2", CalculationMode.Ticks, 1, false);
			SetProfitTarget("BuyStuff3", CalculationMode.Ticks, 1);
            SetStopLoss("BuyStuff3", CalculationMode.Ticks, 1, false);
			
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            if( lastbarcount < Bars.Count ) // BH:Checks to see if Current Bar is same as last bar processed
            {
				// BH:Calculates Current Account Value
                double currentAccountValue = myAccountValue // BH:parameter that holds starting account value
                    + Performance.RealtimeTrades.TradesPerformance.GrossProfit
                    + Performance.RealtimeTrades.TradesPerformance.GrossLoss
                    - Performance.RealtimeTrades.TradesPerformance.Commission;
				
				// BH:Calculates desired quantity to buy based off 1.5% of Current Account Value
                quantityToPurchase = Convert.ToInt32((currentAccountValue * .015) / GetCurrentBid());
				
				// Calculate the position we want to be in.
               // double desiredLongPosition = Math.Min(Low[0], GetCurrentBid());
				
                // BH:Remove EntryOrder if in a Final State
                for (int i = 0; i < EntryOrders.Count; i++)
                {
                    OrderGroupTestTest orderToCheck = EntryOrders[i];
					if (orderToCheck != null)
					{
						if (orderToCheck.MyOrder != null)
						{
							if (orderToCheck.MyOrder.OrderState == OrderState.Cancelled
								|| orderToCheck.MyOrder.OrderState == OrderState.Rejected
								|| orderToCheck.MyOrder.OrderState == OrderState.Unknown
								|| (orderToCheck.MyOrder.OrderState == OrderState.Filled
									&& Position.MarketPosition != MarketPosition.Long))
							{
								EntryOrders.Remove(orderToCheck);						
							}
						}
						else
						{
							EntryOrders.Remove(orderToCheck);
						}
					}
					else
					{
						EntryOrders.Remove(orderToCheck);
					}
                }		
				
				//Cancel Entry Orders Not Being Filled
				if (Position.MarketPosition == MarketPosition.Long)
				{
					for (int i = 0; i < EntryOrders.Count; i++)
					{
						OrderGroupTestTest orderToCheck = EntryOrders[i];
						if (orderToCheck != null)
						{
							if (orderToCheck.MyOrder != null)
							{
								if (orderToCheck.MyOrder.OrderState != OrderState.PartFilled
									&& orderToCheck.MyOrder.OrderState != OrderState.Filled)
								{
									CancelOrder(orderToCheck.MyOrder);
								}
							}
							else
							{
								EntryOrders.Remove(orderToCheck);
							}
						}
						else
						{
							EntryOrders.Remove(orderToCheck);
						}
					}
				}
				
				// Add new EntryOrders
				if (Low[0] < Low[1] // just went down
					&& EntryOrders.Count == 0
					&& Position.MarketPosition != MarketPosition.Long)
                {
                    AddOrUpdateBuyOrders(GetCurrentBid());
                }
				
				// BH:Chase Price
				UpdateCurrentOrders(GetCurrentBid());
            }
            lastbarcount = Bars.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddOrUpdateBuyOrders(double desiredLongPosition)
        {
            OrderGroupTestTest nextOrders1 = new OrderGroupTestTest();
            OrderGroupTestTest nextOrders2 = new OrderGroupTestTest();
            OrderGroupTestTest nextOrders3 = new OrderGroupTestTest();
            nextOrders1.MyOrder = EnterLongLimit(0, true, quantityToPurchase, desiredLongPosition - 0.00, "BuyStuff1");
            nextOrders2.MyOrder = EnterLongLimit(0, true, quantityToPurchase, desiredLongPosition - 0.01, "BuyStuff2");
            nextOrders3.MyOrder = EnterLongLimit(0, true, quantityToPurchase, desiredLongPosition - 0.02, "BuyStuff3");
			EntryOrders.Add(nextOrders1);
            EntryOrders.Add(nextOrders2);
            EntryOrders.Add(nextOrders3);
        }

		/// <summary>
        /// Updates current open orders to have the desired Price BH:Chase price
        /// </summary>
        private void UpdateCurrentOrders(double desiredLongPosition)
        {
            for (int i = 0; i < EntryOrders.Count; i++)
            {
                OrderGroupTestTest orderToCheck = EntryOrders[i];
                if (orderToCheck != null
					&& orderToCheck.MyOrder != null)
                {
                    // update the order to have our desired position.
                    switch (orderToCheck.MyOrder.Name)
                    {
                        case "BuyStuff1":
                            if (desiredLongPosition - orderToCheck.MyOrder.LimitPrice > 0.001 && orderToCheck.MyOrder.Filled != orderToCheck.MyOrder.Quantity)
                            {
                                EnterLongLimit(0, true, orderToCheck.MyOrder.Quantity, desiredLongPosition - 0.00, orderToCheck.MyOrder.Name);
                            } break;
                        case "BuyStuff2":
                            if (desiredLongPosition - orderToCheck.MyOrder.LimitPrice > 0.011 && orderToCheck.MyOrder.Filled != orderToCheck.MyOrder.Quantity)
                            {
                                EnterLongLimit(0, true, orderToCheck.MyOrder.Quantity, desiredLongPosition - 0.01, orderToCheck.MyOrder.Name);
                            } break;
                        case "BuyStuff3":
                            if (desiredLongPosition - orderToCheck.MyOrder.LimitPrice > 0.021 && orderToCheck.MyOrder.Filled != orderToCheck.MyOrder.Quantity)
                            {
                                EnterLongLimit(0, true, orderToCheck.MyOrder.Quantity, desiredLongPosition - 0.02, orderToCheck.MyOrder.Name);
                            } break;
                        default:
                            break;
                    }
                }
            }
        }

		/// <summary>
        /// 
        /// </summary>		
				
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

	
	
	
	
	
	public class OrderGroupTestTest
    {
        private IOrder _MyOrder;
        public IOrder MyOrder 
        {
            get{ return _MyOrder;}
            set{ _MyOrder = value;}
        }
    }
}

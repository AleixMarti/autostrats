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
    /// Long/short cross above/below daily pivots. 5 point stop loss 3 point trial and 18 pont profit target.
    /// </summary>
    [Description("Long/short cross above/below daily pivots. 5 point stop loss 3 point trial and 18 pont profit target.")]
    public class ANAPivotCrossLongES : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int barPlusTicks = 1; // Default setting for BarPlusTicks
        private int barMinusTicks = 1; // Default setting for BarMinusTicks
        private int contractAmount = 1; // Default setting for ContractAmount
        private int trailStopTicks = 3; // Default setting for TrailStopTicks
        private int stopLossTicks = 5; // Default setting for StopLossTicks
        private int profitTarget = 18; // Default setting for ProfitTarget
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            SetStopLoss("", CalculationMode.Ticks, StopLossTicks, true);
            SetTrailStop("ANALongES", CalculationMode.Ticks, TrailStopTicks, true);
            SetTrailStop("ANAShortES", CalculationMode.Ticks, TrailStopTicks, true);
            SetProfitTarget("", CalculationMode.Ticks, ProfitTarget);

            CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (Position.MarketPosition == MarketPosition.Flat
                && CrossAbove(Median, anaPivotsDailyV16(10, anaPivotStylesPD16.Floor, anaPlotAlignPD16.Left, HLCCalculationMode.CalcFromIntradayData, true, 15).PP, 1))
            {
                EnterLong(ContractAmount, "ANALongES");
            }

            // Condition set 2
            if (Position.MarketPosition == MarketPosition.Flat
                && CrossBelow(Median, anaPivotsDailyV16(10, anaPivotStylesPD16.Floor, anaPlotAlignPD16.Left, HLCCalculationMode.CalcFromIntradayData, true, 15).PP, 1))
            {
                EnterShort(ContractAmount, "ANAShortES");
            }
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int BarPlusTicks
        {
            get { return barPlusTicks; }
            set { barPlusTicks = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int BarMinusTicks
        {
            get { return barMinusTicks; }
            set { barMinusTicks = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ContractAmount
        {
            get { return contractAmount; }
            set { contractAmount = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailStopTicks
        {
            get { return trailStopTicks; }
            set { trailStopTicks = Math.Max(3, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLossTicks
        {
            get { return stopLossTicks; }
            set { stopLossTicks = Math.Max(5, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTarget
        {
            get { return profitTarget; }
            set { profitTarget = Math.Max(18, value); }
        }
        #endregion
    }
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0" encoding="utf-16"?>
<NinjaTrader>
  <Name>ANAPivotCrossLongES</Name>
  <CalculateOnBarClose>False</CalculateOnBarClose>
  <Description>Long/short cross above/below daily pivots. 5 point stop loss 3 point trial and 18 pont profit target.</Description>
  <Parameters>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>1</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>1</Minimum>
      <Name>BarPlusTicks</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>1</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>1</Minimum>
      <Name>BarMinusTicks</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>1</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>1</Minimum>
      <Name>ContractAmount</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>3</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>3</Minimum>
      <Name>TrailStopTicks</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>5</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>5</Minimum>
      <Name>StopLossTicks</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>18</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>18</Minimum>
      <Name>ProfitTarget</Name>
      <Type>int</Type>
    </Parameter>
  </Parameters>
  <State>
    <CurrentState>
      <StrategyWizardState xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        <Name>Flat</Name>
        <Sets>
          <StrategyWizardStateSet>
            <Actions>
              <StrategyWizardAction>
                <DisplayName>Enter long position</DisplayName>
                <Help />
                <MemberName>EnterLong</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>"ANALongES"</string>
                </Values>
                <WizardItems>
                  <StrategyWizardItem>
                    <DisplayName>ContractAmount</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>true</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>ContractAmount</MemberName>
                    <Parameters />
                    <Values />
                    <WizardItems />
                  </StrategyWizardItem>
                  <StrategyWizardItem>
                    <DisplayName />
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName />
                    <Parameters />
                    <Values />
                    <WizardItems />
                  </StrategyWizardItem>
                </WizardItems>
              </StrategyWizardAction>
            </Actions>
            <Conditions>
              <StrategyWizardCondition>
                <AndOr>And</AndOr>
                <Left>
                  <DisplayName>Current market position</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>Position.MarketPosition</MemberName>
                  <Parameters />
                  <Values />
                  <WizardItems />
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>==</Operator>
                <Right>
                  <DisplayName>Flat</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>MarketPosition.Flat</MemberName>
                  <Parameters />
                  <Values />
                  <WizardItems />
                </Right>
              </StrategyWizardCondition>
              <StrategyWizardCondition>
                <AndOr>And</AndOr>
                <Left>
                  <DisplayName>Median</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>Median</MemberName>
                  <Parameters>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                  </Values>
                  <WizardItems>
                    <StrategyWizardItem>
                      <DisplayName>	barsAgo</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	offset</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                  </WizardItems>
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>CrossAbove</Operator>
                <Right>
                  <DisplayName>anaPivotsDailyV16</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>anaPivotsDailyV16</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>LabelPosition</string>
                    <string>PivotFormula</string>
                    <string>PlotLabels</string>
                    <string>PriorDayHLC</string>
                    <string>ShowMidpivots</string>
                    <string>Width</string>
                    <string>	plot</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>10</string>
                    <string>anaPivotStylesPD16.Floor</string>
                    <string>anaPlotAlignPD16.Left</string>
                    <string>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</string>
                    <string>True</string>
                    <string>15</string>
                    <string>"PP"</string>
                    <string>0</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>False</string>
                  </Values>
                  <WizardItems>
                    <StrategyWizardItem>
                      <DisplayName>DefaultInput</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>DefaultInput</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>10</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>10</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>anaPivotStylesPD16.Floor</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>anaPivotStylesPD16.Floor</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>anaPlotAlignPD16.Left</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>anaPlotAlignPD16.Left</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>True</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>True</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>15</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>15</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	barsAgo</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	offset</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
          <StrategyWizardStateSet>
            <Actions>
              <StrategyWizardAction>
                <DisplayName>Enter short position</DisplayName>
                <Help />
                <MemberName>EnterShort</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>"ANAShortES"</string>
                </Values>
                <WizardItems>
                  <StrategyWizardItem>
                    <DisplayName>ContractAmount</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>true</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>ContractAmount</MemberName>
                    <Parameters />
                    <Values />
                    <WizardItems />
                  </StrategyWizardItem>
                  <StrategyWizardItem>
                    <DisplayName />
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName />
                    <Parameters />
                    <Values />
                    <WizardItems />
                  </StrategyWizardItem>
                </WizardItems>
              </StrategyWizardAction>
            </Actions>
            <Conditions>
              <StrategyWizardCondition>
                <AndOr>And</AndOr>
                <Left>
                  <DisplayName>Current market position</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>Position.MarketPosition</MemberName>
                  <Parameters />
                  <Values />
                  <WizardItems />
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>==</Operator>
                <Right>
                  <DisplayName>Flat</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>MarketPosition.Flat</MemberName>
                  <Parameters />
                  <Values />
                  <WizardItems />
                </Right>
              </StrategyWizardCondition>
              <StrategyWizardCondition>
                <AndOr>And</AndOr>
                <Left>
                  <DisplayName>Median</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>Median</MemberName>
                  <Parameters>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                  </Values>
                  <WizardItems>
                    <StrategyWizardItem>
                      <DisplayName>	barsAgo</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	offset</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                  </WizardItems>
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>CrossBelow</Operator>
                <Right>
                  <DisplayName>anaPivotsDailyV16</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>anaPivotsDailyV16</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>LabelPosition</string>
                    <string>PivotFormula</string>
                    <string>PlotLabels</string>
                    <string>PriorDayHLC</string>
                    <string>ShowMidpivots</string>
                    <string>Width</string>
                    <string>	plot</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>10</string>
                    <string>anaPivotStylesPD16.Floor</string>
                    <string>anaPlotAlignPD16.Left</string>
                    <string>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</string>
                    <string>True</string>
                    <string>15</string>
                    <string>"PP"</string>
                    <string>0</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>False</string>
                  </Values>
                  <WizardItems>
                    <StrategyWizardItem>
                      <DisplayName>DefaultInput</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>DefaultInput</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>10</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>10</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>anaPivotStylesPD16.Floor</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>anaPivotStylesPD16.Floor</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>anaPlotAlignPD16.Left</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>anaPlotAlignPD16.Left</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>NinjaTrader.Data.HLCCalculationMode.CalcFromIntradayData</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>True</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>True</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>15</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>15</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	barsAgo</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName>	offset</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>false</IsSet>
                      <MemberName>0</MemberName>
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                    <StrategyWizardItem>
                      <DisplayName />
                      <IsIndicator>false</IsIndicator>
                      <IsInt>false</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName />
                      <Parameters />
                      <Values />
                      <WizardItems />
                    </StrategyWizardItem>
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
        </Sets>
        <StopTargets>
          <StrategyWizardAction>
            <DisplayName>Stop loss</DisplayName>
            <Help />
            <MemberName>SetStopLoss</MemberName>
            <Parameters>
              <string>fromEntrySignal</string>
              <string>mode</string>
              <string>value</string>
              <string>simulated</string>
            </Parameters>
            <Values>
              <string />
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>StopLossTicks</string>
              <string>True</string>
            </Values>
            <WizardItems>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName>StopLossTicks</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>StopLossTicks</MemberName>
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
            </WizardItems>
          </StrategyWizardAction>
          <StrategyWizardAction>
            <DisplayName>Trailing stop</DisplayName>
            <Help />
            <MemberName>SetTrailStop</MemberName>
            <Parameters>
              <string>fromEntrySignal</string>
              <string>mode</string>
              <string>value</string>
              <string>simulated</string>
            </Parameters>
            <Values>
              <string>"ANALongES"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStopTicks</string>
              <string>True</string>
            </Values>
            <WizardItems>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName>TrailStopTicks</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStopTicks</MemberName>
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
            </WizardItems>
          </StrategyWizardAction>
          <StrategyWizardAction>
            <DisplayName>Trailing stop</DisplayName>
            <Help />
            <MemberName>SetTrailStop</MemberName>
            <Parameters>
              <string>fromEntrySignal</string>
              <string>mode</string>
              <string>value</string>
              <string>simulated</string>
            </Parameters>
            <Values>
              <string>"ANAShortES"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStopTicks</string>
              <string>True</string>
            </Values>
            <WizardItems>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName>TrailStopTicks</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStopTicks</MemberName>
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
            </WizardItems>
          </StrategyWizardAction>
          <StrategyWizardAction>
            <DisplayName>Profit target</DisplayName>
            <Help />
            <MemberName>SetProfitTarget</MemberName>
            <Parameters>
              <string>fromEntrySignal</string>
              <string>mode</string>
              <string>value</string>
            </Parameters>
            <Values>
              <string />
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>ProfitTarget</string>
            </Values>
            <WizardItems>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName />
                <IsIndicator>false</IsIndicator>
                <IsInt>false</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName />
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
              <StrategyWizardItem>
                <DisplayName>ProfitTarget</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>ProfitTarget</MemberName>
                <Parameters />
                <Values />
                <WizardItems />
              </StrategyWizardItem>
            </WizardItems>
          </StrategyWizardAction>
        </StopTargets>
      </StrategyWizardState>
    </CurrentState>
  </State>
</NinjaTrader>
@*/
#endregion

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
    /// 3BR with trail stop and stop loss
    /// </summary>
    [Description("3BR with trail stop and stop loss")]
    public class Long3BR230510TrailStop : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int contractAmount = 1; // Default setting for ContractAmount
        private int prevBarPlusTicks = 1; // Default setting for PrevBarPlusTicks
        private int stopLoss = 11; // Default setting for StopLoss
        private int trailStop = 8; // Default setting for TrailStop
        private int profitTraget = 12; // Default setting for ProfitTraget
        // User defined variables (add any user defined variables below)
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            Add(NBarsDown(3, true, true, true));
            Add(NBarsDown(3, true, true, true));
            Add(NBarsDown(3, true, true, true));
            Add(NBarsDown(3, true, true, true));
            SetStopLoss("", CalculationMode.Ticks, StopLoss, false);
            SetTrailStop("3BRLONG0", CalculationMode.Ticks, TrailStop, false);
            SetTrailStop("3BRLONG1", CalculationMode.Ticks, TrailStop, false);
            SetTrailStop("3BRLONG2", CalculationMode.Ticks, TrailStop, false);
            SetTrailStop("3BRLONG3", CalculationMode.Ticks, TrailStop, false);
            SetProfitTarget("", CalculationMode.Ticks, ProfitTraget);

            CalculateOnBarClose = false;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
            // Condition set 1
            if (NBarsDown(3, true, true, true)[0] > 0)
            {
                EnterLongStop(ContractAmount, High[0] + PrevBarPlusTicks * TickSize, "3BRLONG0");
            }

            // Condition set 2
            if (Position.MarketPosition == MarketPosition.Flat
                && NBarsDown(3, true, true, true)[1] > 0)
            {
                EnterLongStop(ContractAmount, High[1] + PrevBarPlusTicks * TickSize, "3BRLONG1");
            }

            // Condition set 3
            if (Position.MarketPosition == MarketPosition.Flat
                && NBarsDown(3, true, true, true)[2] > 0)
            {
                EnterLongStop(ContractAmount, High[2] + PrevBarPlusTicks * TickSize, "3BRLONG2");
            }

            // Condition set 4
            if (Position.MarketPosition == MarketPosition.Flat
                && NBarsDown(3, true, true, true)[3] > 0)
            {
                EnterLongStop(ContractAmount, High[3] + PrevBarPlusTicks * TickSize, "3BRLONG3");
            }

        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int ContractAmount
        {
            get { return contractAmount; }
            set { contractAmount = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int PrevBarPlusTicks
        {
            get { return prevBarPlusTicks; }
            set { prevBarPlusTicks = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int StopLoss
        {
            get { return stopLoss; }
            set { stopLoss = Math.Max(6, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int TrailStop
        {
            get { return trailStop; }
            set { trailStop = Math.Max(4, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int ProfitTraget
        {
            get { return profitTraget; }
            set { profitTraget = Math.Max(4, value); }
        }
        #endregion
    }
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0" encoding="utf-16"?>
<NinjaTrader>
  <Name>Long3BR230510TrailStop</Name>
  <CalculateOnBarClose>False</CalculateOnBarClose>
  <Description>3BR with trail stop and stop loss</Description>
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
      <Name>ContractAmount</Name>
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
      <Name>PrevBarPlusTicks</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>11</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>6</Minimum>
      <Name>StopLoss</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>8</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>4</Minimum>
      <Name>TrailStop</Name>
      <Type>int</Type>
    </Parameter>
    <Parameter>
      <Default1>
      </Default1>
      <Default2>12</Default2>
      <Default3>
      </Default3>
      <Description>
      </Description>
      <Minimum>4</Minimum>
      <Name>ProfitTraget</Name>
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
                <DisplayName>Enter long position by a stop order</DisplayName>
                <Help />
                <MemberName>EnterLongStop</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>stopPrice</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>High[0] + PrevBarPlusTicks * TickSize</string>
                  <string>"3BRLONG0"</string>
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
                    <DisplayName>High</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>High</MemberName>
                    <Parameters>
                      <string>	barsAgo</string>
                      <string>	offsetType</string>
                      <string>	offset</string>
                    </Parameters>
                    <Values>
                      <string>0</string>
                      <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                      <string>PrevBarPlusTicks</string>
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
                        <DisplayName>PrevBarPlusTicks</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>PrevBarPlusTicks</MemberName>
                        <Parameters />
                        <Values />
                        <WizardItems />
                      </StrategyWizardItem>
                    </WizardItems>
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
                  <DisplayName>NBarsDown</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>NBarsDown</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>BarCount</string>
                    <string>BarDown</string>
                    <string>LowerHigh</string>
                    <string>LowerLow</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>3</string>
                    <string>True</string>
                    <string>True</string>
                    <string>True</string>
                    <string>0</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>True</string>
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
                      <DisplayName>3</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>3</MemberName>
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
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>&gt;</Operator>
                <Right>
                  <DisplayName>Numeric value</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName />
                  <Parameters>
                    <string>Value</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
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
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
          <StrategyWizardStateSet>
            <Actions>
              <StrategyWizardAction>
                <DisplayName>Enter long position by a stop order</DisplayName>
                <Help />
                <MemberName>EnterLongStop</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>stopPrice</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>High[1] + PrevBarPlusTicks * TickSize</string>
                  <string>"3BRLONG1"</string>
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
                    <DisplayName>High</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>High</MemberName>
                    <Parameters>
                      <string>	barsAgo</string>
                      <string>	offsetType</string>
                      <string>	offset</string>
                    </Parameters>
                    <Values>
                      <string>1</string>
                      <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                      <string>PrevBarPlusTicks</string>
                    </Values>
                    <WizardItems>
                      <StrategyWizardItem>
                        <DisplayName>Numeric value</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>1</MemberName>
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
                        <DisplayName>PrevBarPlusTicks</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>PrevBarPlusTicks</MemberName>
                        <Parameters />
                        <Values />
                        <WizardItems />
                      </StrategyWizardItem>
                    </WizardItems>
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
                  <DisplayName>NBarsDown</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>NBarsDown</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>BarCount</string>
                    <string>BarDown</string>
                    <string>LowerHigh</string>
                    <string>LowerLow</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>3</string>
                    <string>True</string>
                    <string>True</string>
                    <string>True</string>
                    <string>1</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>True</string>
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
                      <DisplayName>3</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>3</MemberName>
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
                      <DisplayName>Numeric value</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>1</MemberName>
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
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>&gt;</Operator>
                <Right>
                  <DisplayName>Numeric value</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName />
                  <Parameters>
                    <string>Value</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
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
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
          <StrategyWizardStateSet>
            <Actions>
              <StrategyWizardAction>
                <DisplayName>Enter long position by a stop order</DisplayName>
                <Help />
                <MemberName>EnterLongStop</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>stopPrice</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>High[2] + PrevBarPlusTicks * TickSize</string>
                  <string>"3BRLONG2"</string>
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
                    <DisplayName>High</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>High</MemberName>
                    <Parameters>
                      <string>	barsAgo</string>
                      <string>	offsetType</string>
                      <string>	offset</string>
                    </Parameters>
                    <Values>
                      <string>2</string>
                      <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                      <string>PrevBarPlusTicks</string>
                    </Values>
                    <WizardItems>
                      <StrategyWizardItem>
                        <DisplayName>Numeric value</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>2</MemberName>
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
                        <DisplayName>PrevBarPlusTicks</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>PrevBarPlusTicks</MemberName>
                        <Parameters />
                        <Values />
                        <WizardItems />
                      </StrategyWizardItem>
                    </WizardItems>
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
                  <DisplayName>NBarsDown</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>NBarsDown</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>BarCount</string>
                    <string>BarDown</string>
                    <string>LowerHigh</string>
                    <string>LowerLow</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>3</string>
                    <string>True</string>
                    <string>True</string>
                    <string>True</string>
                    <string>2</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>True</string>
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
                      <DisplayName>3</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>3</MemberName>
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
                      <DisplayName>Numeric value</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>2</MemberName>
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
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>&gt;</Operator>
                <Right>
                  <DisplayName>Numeric value</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName />
                  <Parameters>
                    <string>Value</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
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
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
          <StrategyWizardStateSet>
            <Actions>
              <StrategyWizardAction>
                <DisplayName>Enter long position by a stop order</DisplayName>
                <Help />
                <MemberName>EnterLongStop</MemberName>
                <Parameters>
                  <string>quantity</string>
                  <string>stopPrice</string>
                  <string>signalName</string>
                </Parameters>
                <Values>
                  <string>ContractAmount</string>
                  <string>High[3] + PrevBarPlusTicks * TickSize</string>
                  <string>"3BRLONG3"</string>
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
                    <DisplayName>High</DisplayName>
                    <IsIndicator>false</IsIndicator>
                    <IsInt>false</IsInt>
                    <IsMethod>false</IsMethod>
                    <IsSet>true</IsSet>
                    <MemberName>High</MemberName>
                    <Parameters>
                      <string>	barsAgo</string>
                      <string>	offsetType</string>
                      <string>	offset</string>
                    </Parameters>
                    <Values>
                      <string>3</string>
                      <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                      <string>PrevBarPlusTicks</string>
                    </Values>
                    <WizardItems>
                      <StrategyWizardItem>
                        <DisplayName>Numeric value</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>3</MemberName>
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
                        <DisplayName>PrevBarPlusTicks</DisplayName>
                        <IsIndicator>false</IsIndicator>
                        <IsInt>true</IsInt>
                        <IsMethod>false</IsMethod>
                        <IsSet>true</IsSet>
                        <MemberName>PrevBarPlusTicks</MemberName>
                        <Parameters />
                        <Values />
                        <WizardItems />
                      </StrategyWizardItem>
                    </WizardItems>
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
                  <DisplayName>NBarsDown</DisplayName>
                  <IsIndicator>true</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>true</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName>NBarsDown</MemberName>
                  <Parameters>
                    <string>	inputSeries</string>
                    <string>BarCount</string>
                    <string>BarDown</string>
                    <string>LowerHigh</string>
                    <string>LowerLow</string>
                    <string>	barsAgo</string>
                    <string>	offsetType</string>
                    <string>	offset</string>
                    <string>	plotOnChart</string>
                  </Parameters>
                  <Values>
                    <string>DefaultInput</string>
                    <string>3</string>
                    <string>True</string>
                    <string>True</string>
                    <string>True</string>
                    <string>3</string>
                    <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
                    <string>0</string>
                    <string>True</string>
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
                      <DisplayName>3</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>3</MemberName>
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
                      <DisplayName>Numeric value</DisplayName>
                      <IsIndicator>false</IsIndicator>
                      <IsInt>true</IsInt>
                      <IsMethod>false</IsMethod>
                      <IsSet>true</IsSet>
                      <MemberName>3</MemberName>
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
                </Left>
                <LookBackPeriod>1</LookBackPeriod>
                <Operator>&gt;</Operator>
                <Right>
                  <DisplayName>Numeric value</DisplayName>
                  <IsIndicator>false</IsIndicator>
                  <IsInt>false</IsInt>
                  <IsMethod>false</IsMethod>
                  <IsSet>true</IsSet>
                  <MemberName />
                  <Parameters>
                    <string>Value</string>
                  </Parameters>
                  <Values>
                    <string>0</string>
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
                  </WizardItems>
                </Right>
              </StrategyWizardCondition>
            </Conditions>
          </StrategyWizardStateSet>
          <StrategyWizardStateSet>
            <Actions />
            <Conditions />
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
              <string>StopLoss</string>
              <string>False</string>
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
                <DisplayName>StopLoss</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>StopLoss</MemberName>
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
              <string>"3BRLONG0"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStop</string>
              <string>False</string>
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
                <DisplayName>TrailStop</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStop</MemberName>
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
              <string>"3BRLONG1"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStop</string>
              <string>False</string>
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
                <DisplayName>TrailStop</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStop</MemberName>
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
              <string>"3BRLONG2"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStop</string>
              <string>False</string>
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
                <DisplayName>TrailStop</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStop</MemberName>
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
              <string>"3BRLONG3"</string>
              <string>NinjaTrader.Strategy.CalculationMode.Ticks</string>
              <string>TrailStop</string>
              <string>False</string>
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
                <DisplayName>TrailStop</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>TrailStop</MemberName>
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
              <string>ProfitTraget</string>
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
                <DisplayName>ProfitTraget</DisplayName>
                <IsIndicator>false</IsIndicator>
                <IsInt>true</IsInt>
                <IsMethod>false</IsMethod>
                <IsSet>true</IsSet>
                <MemberName>ProfitTraget</MemberName>
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

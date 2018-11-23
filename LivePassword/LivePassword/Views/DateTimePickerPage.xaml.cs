using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls.Primitives;
using System.Globalization;

namespace EnteAutonomoVolturno.Views
{
    public partial class DateTimePickerPage : DateTimePickerPageBase
    {
        public DateTimePickerPage()
        {
            InitializeComponent();

            // Hook up the data sources
            PrimarySelector.DataSource = new YearDataSource();
            SecondarySelector.DataSource = new MonthDataSource();
            TertiarySelector.DataSource = new DayDataSource();

            InitializeDateTimePickerPage(PrimarySelector, SecondarySelector, TertiarySelector);
        }

        /// <summary>
        /// Gets a sequence of LoopingSelector parts ordered according to culture string for date/time formatting.
        /// </summary>
        /// <returns>LoopingSelectors ordered by culture-specific priority.</returns>
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
            return new LoopingSelector[] { TertiarySelector, SecondarySelector, PrimarySelector };
        }

        /// <summary>
        /// Handles changes to the page's Orientation property.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (null == e)
            {
                throw new ArgumentNullException("e");
            }

            base.OnOrientationChanged(e);
        }

        public override void SetFlowDirection(FlowDirection flowDirection)
        {
            PrimarySelector.FlowDirection = flowDirection;
            SecondarySelector.FlowDirection = flowDirection;
            TertiarySelector.FlowDirection = flowDirection;
        }
    }

    public class DateDataSource : Microsoft.Phone.Controls.DataSource
    {
        protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
        {
            return relativeDate.AddDays(delta);
        }
    }
}
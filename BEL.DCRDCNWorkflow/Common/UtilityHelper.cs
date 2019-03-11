namespace BEL.DCRDCNWorkflow.Common
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Utility Helper
    /// </summary>
    public static class UtilityHelper
    {
        /// <summary>
        /// Gets the year period list.
        /// </summary>
        /// <param name="noOfFutureyear">The no of futureyear.</param>
        /// <param name="isShowPrevYear">show prev year</param>
        /// <returns>returnn year list</returns>
        public static List<NameValueData> GetYearPeriodList(int noOfFutureyear, bool isShowPrevYear = false)
        {
            List<NameValueData> yearList = new List<NameValueData>();
            int startYear = 0;
            int endYear = 0;

            if (isShowPrevYear)
            {
                startYear = DateTime.Now.Year - 10;
                endYear = DateTime.Now.Year;
            }
            else
            {
                startYear = DateTime.Now.Year;
                endYear = startYear + noOfFutureyear;
            }
            for (int i = startYear; i <= endYear; i++)
            {
                yearList.Add(new NameValueData() { Name = i.ToString(), Value = i.ToString() });
            }

            return yearList.ToList();
        }

        /// <summary>
        /// Gets the month period list.
        /// </summary>
        /// <returns>return month list</returns>
        public static List<NameValueData> GetMonthPeriodList()
        {
            return DateTimeFormatInfo
               .InvariantInfo
               .MonthNames.Where(x => !string.IsNullOrEmpty(x))
               .Select((monthName, index) => new NameValueData
               {
                   Value = (index + 1).ToString(),
                   Name = monthName
               }).ToList();
        }
    }
}
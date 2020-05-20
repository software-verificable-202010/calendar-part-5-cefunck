using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public static class Utilities
    {
        #region Constants
        public const string mondayName = "Lunes";
        public const string tuesdayName = "Martes";
        public const string wednesdayName = "Miércoles";
        public const string thursdayName = "Jueves";
        public const string fridayName = "Viernes";
        public const string saturdayName = "Sábado";
        public const string sundayName = "Domingo";
        public const int mondayNumberInweek = 1;
        public const int tuesdayNumberInweek = 2;
        public const int wednesdayNumberInweek = 3;
        public const int thursdayNumberInweek = 4;
        public const int fridayNumberInweek = 5;
        public const int saturdayNumberInweek = 6;
        public const int sundayNumberInweek = 7;
        public const string blankSpace = " ";
        public const int negativeMultiplier = -1;
        public const int daysInWeek = 7;
        public const int systemEnumSundayNumber = 0;
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static int GetDayNumberInWeek(DateTime date)
        {
            int dayNumber = (int)date.DayOfWeek;
            if (dayNumber == systemEnumSundayNumber)
            {
                dayNumber = sundayNumberInweek;
            }
            return dayNumber;
        }

        public static string GetNameOfDayInSpanish(DateTime date)
        {
            string spanishDayName;
            switch ((int)date.DayOfWeek)
            {
                case mondayNumberInweek:
                    spanishDayName = mondayName;
                    break;
                case tuesdayNumberInweek:
                    spanishDayName = tuesdayName;
                    break;
                case wednesdayNumberInweek:
                    spanishDayName = wednesdayName;
                    break;
                case Utilities.thursdayNumberInweek:
                    spanishDayName = thursdayName;
                    break;
                case fridayNumberInweek:
                    spanishDayName = fridayName;
                    break;
                case saturdayNumberInweek:
                    spanishDayName = saturdayName;
                    break;
                default:
                    spanishDayName = sundayName;
                    break;
            }
            return spanishDayName;
        }
        #endregion



    }
}

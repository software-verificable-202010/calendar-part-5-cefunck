using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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
        private const string displayedDateResourceName = "displayedDate";
        private const string appointmentsDataFilePath = "applicationAppointmentData";
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
        public static List<string> GetDayNames() 
        {
            List<string> dayNames = new List<string> 
            {
            mondayName,tuesdayName,wednesdayName,thursdayName,fridayName,saturdayName,sundayName
            };
            return dayNames;
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
                case thursdayNumberInweek:
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
        public static void SetDisplayedDate(DateTime dateToDisplay)
        {
            App.Current.Resources[displayedDateResourceName] = dateToDisplay;
        }
        public static void SetDisplayedDateToNow()
        {
            SetDisplayedDate(DateTime.Now);
        }
        public static DateTime GetDisplayedDate()
        {
            return (DateTime)App.Current.Resources[displayedDateResourceName];
        }
        public static void SaveAppointments(List<Appointment> appointments)
        {
            using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, appointments);
                file.Flush();
            }
        }
        public static List<Appointment> LoadAppointments()
        {
            List<Appointment> appointments;
            try
            {
                using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object obj = bf.Deserialize(file);
                    appointments = obj as List<Appointment>;
                }
            }
            catch (FileNotFoundException e)
            {
                appointments = new List<Appointment>();
            }
            
            return appointments;
        }
        #endregion
    }
}

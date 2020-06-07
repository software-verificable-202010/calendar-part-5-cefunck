using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calendar
{
    public static class Utilities
    {
        #region Constants
        public const int MondayNumberInweek = 1;
        public const int TuesdayNumberInweek = 2;
        public const int WednesdayNumberInweek = 3;
        public const int ThursdayNumberInweek = 4;
        public const int FridayNumberInweek = 5;
        public const int SaturdayNumberInweek = 6;
        public const int SundayNumberInweek = 7;
        public const int NegativeMultiplier = -1;
        public const int DaysInWeek = 7;
        public const int SystemEnumSundayNumber = 0;
        public const string MondayName = "Lunes";
        public const string TuesdayName = "Martes";
        public const string WednesdayName = "Miércoles";
        public const string ThursdayName = "Jueves";
        public const string FridayName = "Viernes";
        public const string SaturdayName = "Sábado";
        public const string SundayName = "Domingo";
        private const string appointmentsDataFilePath = "applicationAppointmentsData";
        #endregion


        #region Fields
        private static DateTime displayedDate;
        private static List<Appointment> calendarAppointments = new List<Appointment>();
        #endregion


        #region Properties
        public static DateTime DisplayedDate 
        { 
            get => displayedDate; 
            set => displayedDate = value; 
        }

        public static List<Appointment> CalendarAppointments 
        { 
            get => calendarAppointments;
        }

        public static Collection<string> DayNames
        {
            get
            {
                Collection<string> dayNames = new Collection<string>
                {
                    MondayName,
                    TuesdayName,
                    WednesdayName,
                    ThursdayName,
                    FridayName,
                    SaturdayName,
                    SundayName
                };
                return dayNames;
            }
        }

        #endregion


        #region Methods
        public static void SetDisplayedDateToNow()
        {
            DisplayedDate = DateTime.Now;
        }

        public static void SavePersistentAppointments()
        {
            List<Appointment> calendarAppointments = CalendarAppointments; 
            using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, calendarAppointments);
                file.Flush();
            }
        }

        public static void LoadPersistentAppointments()
        {
            bool isAppointmentsDataFileFound = File.Exists(appointmentsDataFilePath);

            if (isAppointmentsDataFileFound)
            {
                using (FileStream file = new FileStream(appointmentsDataFilePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object deserealizedAppointments = bf.Deserialize(file);
                    calendarAppointments = deserealizedAppointments as List<Appointment>;
                }
            }
            else 
            {
                calendarAppointments = new List<Appointment>();
            }
        }

        public static void AssignCalendarAppointments(List<Appointment> appointments)
        {
            calendarAppointments = appointments;
        }

        public static int GetDayNumberInWeek(DateTime date)
        {
            int dayNumber = (int)date.DayOfWeek;
            bool isSunday = dayNumber == SystemEnumSundayNumber;

            if (isSunday)
            {
                dayNumber = SundayNumberInweek;
            }
            return dayNumber;
        }

        public static string GetNameOfDayInSpanish(DateTime date)
        {
            string spanishDayName;
            switch ((int)date.DayOfWeek)
            {
                case MondayNumberInweek:
                    spanishDayName = MondayName;
                    break;
                case TuesdayNumberInweek:
                    spanishDayName = TuesdayName;
                    break;
                case WednesdayNumberInweek:
                    spanishDayName = WednesdayName;
                    break;
                case ThursdayNumberInweek:
                    spanishDayName = ThursdayName;
                    break;
                case FridayNumberInweek:
                    spanishDayName = FridayName;
                    break;
                case SaturdayNumberInweek:
                    spanishDayName = SaturdayName;
                    break;
                default:
                    spanishDayName = SundayName;
                    break;
            }
            return spanishDayName;
        }

        #endregion
    }
}

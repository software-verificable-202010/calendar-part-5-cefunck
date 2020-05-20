using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Lógica de interacción para CalendarLayout.xaml
    /// </summary>
    public partial class CalendarLayout : UserControl
    {
        #region Constants
        private const int bodyGridRow = 1;
        private const int bodyGridColumn = 0;
        private const string dayNumberResourceKey = "bodyContentResourceKey";
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public CalendarLayout()
        {
            InitializeComponent();
            ContentControl calendarBody = CreateBodyContentControl();
            InsertBodyContentControlToGrid(calendarBody);
        }

        public ContentControl CreateBodyContentControl()
        {

            ContentControl bodyContentControl = new ContentControl();
            bodyContentControl.SetValue(Grid.ColumnProperty, bodyGridColumn);
            bodyContentControl.SetValue(Grid.RowProperty, bodyGridRow);
            bodyContentControl.SetResourceReference(ContentControl.ContentProperty, dayNumberResourceKey);
            return bodyContentControl;
        }

        public void InsertBodyContentControlToGrid(ContentControl calendarBody)
        {
            LayoutGrid.Children.Add(calendarBody);
        }
        #endregion


    }
}

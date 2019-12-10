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

namespace UniversityTimetabling
{
    /// <summary>

    /// Interaction logic for UcPanel.xaml

    /// </summary>

    public partial class UControl : UserControl
    {
        const string red = "#FFFF0000";
        const string green = "#FF008000";
        const string yellow = "#FFFFFF00";
        const string orange = "#FFFFA500";
        const string gray = "#FFA9A9A9";
        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }

        public string Row
        {
            get { return lblRow.Text; }
            set { lblRow.Text = value; }
        }

        public string Column
        {
            get { return lblColumn.Text; }
            set { lblColumn.Text = value; }
        }

        public string Clicked
        {
            get { return lblClicked.Text; }
            set { lblClicked.Text = value; }
        }

        public string ControlAssigned
        {
            get { return lblControlAssigned.Text; }
            set { lblControlAssigned.Text = value; }
        }

        public string BlockedAssigned
        {
            get { return lblBlockedAssign.Text; }
            set { lblBlockedAssign.Text = value; }
        }

        public string PanelBackground
        {
            get { return pnlBackground.Background.ToString(); }
            set { pnlBackground.Background = (new BrushConverter()).ConvertFromString(value) as Brush; }
        }

        public string BackgroundOpacity
        {
            get { return pnlBackground.Opacity.ToString(); }
            set { pnlBackground.Opacity = double.Parse(value); }
        }

        public UControl(bool addRows, string row = null, string column = null)
        {
			try
			{
				InitializeComponent();
				lblClicked.Text = "false";
				lblControlAssigned.Text = "false";
				if (addRows == true)
				{
					for (int i = 0; i < 4; i++)
					{

						gridPanel.RowDefinitions.Add(new RowDefinition());
						UControl panel = new UControl(false);
						panel.Row = (((Convert.ToInt32(row) - 1) * 4) + i + 1).ToString();
						panel.Column = column;
						Grid.SetColumn(panel, 0);
						Grid.SetRow(panel, i);
						gridPanel.Children.Add(panel);
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
        }
        private void gridPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Grid gridMain = this.Parent as Grid;

                TimetableWindow main = (((gridMain.Parent as UControl).Parent as Grid).Parent as Grid).Parent as TimetableWindow;

                pnlBackground.Opacity -= 0.5;
                int lectures = int.Parse(main.txtNumberofLectures.Text);
                switch (pnlBackground.Background.ToString())
                {
                    case red: break;
                    case green:
                    case yellow:
                    case orange:
                        getInfluencedChilds(lectures, main);
                        break;
                    case gray: break;
                }

                pnlBackground.BorderThickness = new Thickness(1);
            }
            catch(Exception)
            {}
        }

        private void gridPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Grid gridMain = this.Parent as Grid;
                TimetableWindow main = (((gridMain.Parent as UControl).Parent as Grid).Parent as Grid).Parent as TimetableWindow;

                pnlBackground.Opacity += 0.5;
                int lectures = int.Parse(main.txtNumberofLectures.Text);
                switch (pnlBackground.Background.ToString())
                {
                    case red: break;
                    case green:
                    case yellow:
                    case orange:
                        leaveInfluencedChilds(lectures, main);
                        break;
                    case gray: break;
                }
                pnlBackground.BorderThickness = new Thickness(0);
            }
            catch (Exception)
            { }
        }

        private void pnlBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (Column != "")
                {
                    lblClicked.Text = "true";
                }
            }
           
        }

        private void getInfluencedChilds(int lectures, TimetableWindow main)
        {
            int breaks = 0;
            switch (lectures % 2)
            {
                case 0:
                    breaks = (lectures - 2) / 2;
                    break;
                case 1:
                    breaks = (lectures - 1) / 2;
                    break;
            }

            int lecturePeriods = 3 * lectures;
            int endPeriod = int.Parse(Row) + (3 * lectures) + breaks;
            List<UControl> childs = main.getChilds().FindAll(x => x.Column == Column && int.Parse(x.Row) > int.Parse(Row) && int.Parse(x.Row) < endPeriod);
            foreach(UControl child in childs)
            {
                child.Opacity -= 0.5;
            }
        }

        private void leaveInfluencedChilds(int lectures, TimetableWindow main)
        {
            int breaks = 0;
            switch (lectures % 2)
            {
                case 0:
                    breaks = (lectures - 2) / 2;
                    break;
                case 1:
                    breaks = (lectures - 1) / 2;
                    break;
            }

            int lecturePeriods = 3 * lectures;
            int endPeriod = int.Parse(Row) + (3 * lectures) + breaks;
            List<UControl> childs = main.getChilds().FindAll(x => x.Column == Column && int.Parse(x.Row) > int.Parse(Row) && int.Parse(x.Row) < endPeriod);
            foreach (UControl child in childs)
            {
                child.Opacity += 0.5;
            }
        }
    }
}
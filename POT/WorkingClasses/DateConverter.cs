using System;
using System.Linq;

namespace POT.WorkingClasses
{
    class DateConverter
    {
        public String ConvertDDMMYY(String mDate)
        {
            String day;
            String month;
            String year;
            String wrstr;

            if (mDate.Equals(""))
            {
                day = DateTime.Now.Day.ToString("dd");
                month = DateTime.Now.Month.ToString("MM");
                year = DateTime.Now.Year.ToString("yy");
            }
            else
            {
                try
                {
                    wrstr = mDate.Replace(" ", "/");
                    wrstr = wrstr.Replace(",", "/");
                    wrstr = wrstr.Replace(".", "/");
                    wrstr = wrstr.Replace("\\", "/");
                    wrstr = wrstr.Replace("-", "/");
                    wrstr = wrstr.Replace("_", "/");
                    wrstr = wrstr.Replace(":", "/");

                    var arr = wrstr.Split('/');

                    switch (arr.Count())
                    {
                        case 1:
                            day = string.Format("{0:00}", int.Parse(arr[0]));
                            month = string.Format("{0:00}", DateTime.Now.ToString("MM"));
                            year = DateTime.Now.ToString("yy");
                            break;
                        case 2:
                            day = string.Format("{0:00}", int.Parse(arr[0]));
                            month = string.Format("{0:00}", int.Parse(arr[1]));
                            year = DateTime.Now.ToString("yy");
                            break;
                        default:
                            day = string.Format("{0:00}", int.Parse(arr[0]));
                            month = string.Format("{0:00}", int.Parse(arr[1]));
                            if (arr[2].Length > 2)
                                year = string.Format("{0:00}", arr[2].Substring(arr[2].Length - 2, 2));
                            else
                                year = string.Format("{0:00}", int.Parse(arr[2]));
                            break;
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    return mDate;
                }
            }

            return day + "." + month + "." + year + ".";
        }

        public String ConvertTimeHHMM(String mTime)
        {
            String hh;
            String mm;
            String wrstr;

            if (mTime.Equals(""))
            {
                hh = DateTime.Now.ToString("HH");
                mm = DateTime.Now.ToString("mm");
            }
            else
            {
                try
                {
                    if(mTime.Length < 5)
                    {
                        int lng = mTime.Length;

                        if (lng < 4)
                        {
                            hh = string.Format("{0:00}", int.Parse(mTime.Substring(0, 1)));
                            mm = string.Format("{0:00}", int.Parse(mTime.Substring(lng - 2, lng - 1)));
                        }
                        else
                        {
                            hh = string.Format("{0:00}", int.Parse(mTime.Substring(0, 2)));
                            mm = string.Format("{0:00}", int.Parse(mTime.Substring(2, 2)));
                        }
                    }
                    else
                    {
                        wrstr = mTime.Replace(" ", "/");
                        wrstr = wrstr.Replace(",", "/");
                        wrstr = wrstr.Replace(".", "/");
                        wrstr = wrstr.Replace("\\", "/");
                        wrstr = wrstr.Replace("-", "/");
                        wrstr = wrstr.Replace("_", "/");
                        wrstr = wrstr.Replace(":", "/");

                        var arr = wrstr.Split('/');

                        switch (arr.Count())
                        {
                            case 1:
                                hh = string.Format("{0:00}", int.Parse(arr[0]));
                                mm = DateTime.Now.ToString("mm");
                                break;
                            default:
                                hh = string.Format("{0:00}", int.Parse(arr[0]));
                                mm = string.Format("{0:00}", int.Parse(arr[1]));
                                break;
                        }
                    }
                }
                catch (Exception e1)
                {
                    new LogWriter(e1);
                    return mTime;
                }
            }

            return hh + ":" + mm;
        }

        public String CalculatePrioDate_NIJE_TESTIRANO(String Prio, String datPrijave, String VriPrijave)
        {
            int _Prio = int.Parse(Prio);
            DateTime nesto;
            DateTime PlusVrijeme;

            switch (_Prio)
            {
                case 1:
                    _Prio = 2;
                    break;
                case 2:
                    _Prio = 4;
                    break;
                case 3:
                    _Prio = 8;
                    break;
                case 4:
                    _Prio = 24;
                    break;
                default:
                    _Prio = 48;
                    break;

            }

            if (!datPrijave.Equals("") && !VriPrijave.Equals(""))
            {
                DateTime calTime = new DateTime();
                PlusVrijeme = DateTime.Parse(VriPrijave);
                DateTime ifNextWorkingDay = new DateTime(1, 1, 1, PlusVrijeme.Hour, PlusVrijeme.Minute, 0);

                if ((_Prio == 2 || _Prio == 4 || _Prio == 8) && ifNextWorkingDay.AddHours(_Prio) > calTime.AddHours(18))
                {
                    PlusVrijeme = PlusVrijeme.AddHours(14);
                }

                nesto = DateTime.Parse(datPrijave).AddHours(PlusVrijeme.Hour);
                return string.Format("{0:dd.MM.yy.}", nesto.AddHours(_Prio));
            }
            else if (datPrijave.Equals(""))
            {
                return string.Format("{0:dd.MM.yy.}", DateTime.Now);
            }
            else
            {
                return string.Format("{0:dd.MM.yy.}", datPrijave);
            }
        }

        public DateTime CalculatePrio(String Prio, String datPrijave, String VriPrijave)
        {
            int _Prio = int.Parse(Prio);
            int _day = 0;
            DateTime slaTime;

            switch (_Prio)
            {
                case 1:
                    _Prio = 2;
                    break;
                case 2:
                    _Prio = 4;
                    break;
                case 3:
                    _Prio = 8;
                    break;
                case 4:
                    _Prio = 24;
                    _day = 1;
                    break;
                case 5:
                    _Prio = 48;
                    _day = 2;
                    break;
                default:
                    _Prio = 0;
                    break;

            }

            if (int.Parse(Prio) == 6)
            {
                return DateTime.Parse(datPrijave + " " + VriPrijave);
            }

            DateTime limitDown;
            DateTime limitUp;
            DateTime prijava;

            if (!VriPrijave.Equals("") || !datPrijave.Equals(""))
            {
                limitDown = new DateTime(
                    DateTime.Parse(datPrijave).Year,
                    DateTime.Parse(datPrijave).Month,
                    DateTime.Parse(datPrijave).Day,
                    8, 0, 0);
                limitUp = new DateTime(
                    DateTime.Parse(datPrijave).Year,
                    DateTime.Parse(datPrijave).Month,
                    DateTime.Parse(datPrijave).Day,
                    18, 0, 0);
                prijava = new DateTime(
                    DateTime.Parse(datPrijave).Year,
                    DateTime.Parse(datPrijave).Month,
                    DateTime.Parse(datPrijave).Day,
                    DateTime.Parse(VriPrijave).Hour,
                    DateTime.Parse(VriPrijave).Minute,
                    0);
            }
            else
            {
                limitDown = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    8, 0, 0);
                limitUp = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    18, 0, 0);
                prijava = new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    0);
            }

            if(_Prio == 24 || _Prio == 48)
            {
                if (prijava > limitDown && prijava < limitUp)
                    slaTime = prijava.AddDays(_day);
                else
                    slaTime = limitDown.AddDays(_day + 1);
            }
            else
            {
                if (prijava > limitDown && prijava < limitUp)
                {
                    if (prijava.AddHours(_Prio) > limitUp && prijava.AddHours(_Prio) < limitDown.AddDays(1))
                    {
                        slaTime = limitDown.AddDays(_day + 1).AddHours((prijava.AddHours(_Prio) - limitUp).Hours).AddMinutes(prijava.Minute);
                    }
                    else if (prijava.AddHours(_Prio) < limitUp)
                    {
                        slaTime = limitDown.AddDays(_day).AddHours((prijava.AddHours(_Prio) - limitDown).Hours).AddMinutes(prijava.Minute);
                    }
                    else
                    {
                        slaTime = limitDown.AddDays(_day + 1).AddHours(_Prio);
                    }
                }
                else
                {
                    if (prijava.AddHours(_Prio) > limitUp && prijava.AddHours(_Prio) < limitDown.AddDays(1))
                    {
                        slaTime = limitDown.AddDays(_day).AddHours(_Prio);
                    }
                    else
                    {
                        slaTime = limitDown.AddDays(_day + 1).AddHours(_Prio);
                    }
                }
            }

            return slaTime;
        }
    }
}

using POT.MyTypes;
using System;
using System.Collections.Generic;

namespace POT.WorkingClasses
{
    static class Decoder
    {
        public static String GetOwnerCode(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            if (retValue.Length < 12)
                return "00";

            return retValue.Length > 12 ? retValue.Substring(0, 2) : "0" + retValue.Substring(0, 1);
        }
        public static String GetOwnerCode(String mCode)
        {
            if (mCode.Length < 12)
                return "00";

            return mCode.Length > 12 ? mCode.Substring(0, 2) : "0" + mCode.Substring(0, 1);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static long GetFullPartCodeLng(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            switch (retValue.Length)
            {
                case int n when (n > 6 && n < 10):
                    return long.Parse(retValue.Substring(0));
                case int n when (n > 9):
                    return retValue.Length > 12 ? long.Parse(retValue.Substring(4, 9)) : long.Parse(retValue.Substring(3, 9));
                default:
                    return 0;
            }
        }
        public static long GetFullPartCodeLng(String mCode)
        {
            switch (mCode.Length)
            {
                case int n when (n > 6 && n < 10):
                    return long.Parse(mCode.Substring(0));
                case int n when (n > 9):
                    return mCode.Length > 12 ? long.Parse(mCode.Substring(4, 9)) : long.Parse(mCode.Substring(3, 9));
                default:
                    return 0;
            }
        }

        public static String GetFullPartCodeStr(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            switch (retValue.Length)
            {
                case 7:
                    return retValue.Substring(0);
                case 8:
                    return retValue.Substring(0);
                case 9:
                    return retValue.Substring(0);
                case int n when (n > 9):
                    return retValue.Length > 12 ? (long.Parse(retValue.Substring(4, 9))).ToString() : (long.Parse(retValue.Substring(3, 9))).ToString();
                default:
                    return "000000000";
            }
        }
        public static String GetFullPartCodeStr(String mCode)
        {
            switch (mCode.Length)
            {
                case 7:
                    return mCode.Substring(0);
                case 8:
                    return mCode.Substring(0);
                case 9:
                    return mCode.Substring(0);
                case int n when (n > 9):
                    return mCode.Length > 12 ? (long.Parse(mCode.Substring(4, 9))).ToString() : (long.Parse(mCode.Substring(3, 9))).ToString();
                default:
                    return "000000000";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static String GetCustomerCode(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            if (retValue.Length < 12)
                return "00";

            return retValue.Length > 12 ? retValue.Substring(2, 2) : retValue.Substring(1, 2);
        }
        public static String GetCustomerCode(String mCode)
        {
            if (mCode.Length < 12)
                return "00";

            return mCode.Length > 12 ? mCode.Substring(2, 2) : mCode.Substring(1, 2);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public static long GetCategoryCode(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            switch (retValue.Length)
            {
                case 7:
                    return long.Parse(retValue.Substring(0, 1));
                case 8:
                    return long.Parse(retValue.Substring(0, 2));
                case 9:
                    return long.Parse(retValue.Substring(0, 3));
                case int n when (n > 9):
                    return retValue.Length > 12 ? long.Parse(retValue.Substring(4, 3)) : long.Parse(retValue.Substring(3, 3));
                default:
                    return 0;
            }   
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public static long GetPartCode(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            switch (retValue.Length)
            {
                case 7:
                    return long.Parse(retValue.Substring(1, 3));
                case 8:
                    return long.Parse(retValue.Substring(2, 3));
                case 9:
                    return long.Parse(retValue.Substring(3, 3));
                case int n when (n > 9):
                    return retValue.Length > 12 ? long.Parse(retValue.Substring(7, 3)) : long.Parse(retValue.Substring(6, 3));
                default:
                    return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public static long GetSubPartCode(long mCode)
        {
            String retValue;
            retValue = mCode.ToString();

            switch (retValue.Length)
            {
                case 7:
                    return long.Parse(retValue.Substring(4, 3));
                case 8:
                    return long.Parse(retValue.Substring(5, 3));
                case 9:
                    return long.Parse(retValue.Substring(6, 3));
                case int n when (n > 9):
                    return retValue.Length > 12 ? long.Parse(retValue.Substring(10, 3)) : long.Parse(retValue.Substring(9, 3));
                default:
                    return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static String ConnectCodeName(List<String> mSifrarnik, Part mPrt)
        {
            String name = "";
            for (int i = 0; i < mSifrarnik.Count; i = i + 2)
            {
                if (long.Parse(mSifrarnik[i + 1]) == mPrt.PartialCode)
                {
                    name = mSifrarnik[i];
                    break;
                }
            }

            return name;
        }
    }
}

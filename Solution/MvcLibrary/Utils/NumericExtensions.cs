﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.Utils
{
    public static class NumericExtensions
    {
        public static string GetOrdinal(this int value)
        {
            string valueString = value.ToString();

            if (valueString.EndsWith("11") || valueString.EndsWith("12") || valueString.EndsWith("13"))
            {
                return "th";
            }
            else if (valueString.EndsWith("1"))
            {
                return "st";
            }
            else if (valueString.EndsWith("2"))
            {
                return "nd";
            }
            else if (valueString.EndsWith("3"))
            {
                return "rd";
            }
            else
            {
                return "th";
            }
        }

        public static string PadZeros(this int value, int lengthOfString)
        {
            int valueLength = value.ToString().Length;
            int zerosToPad = lengthOfString - valueLength;

            if (zerosToPad > 0)
            {
                StringBuilder output = new StringBuilder();

                for (int i = 0; i < zerosToPad; i++)
                {
                    output.Append("0");
                }

                output.Append(value);

                return output.ToString();
            }
            else
            {
                return value.ToString();
            }
        }

        public static float BullyIntoRange(this float value, float minValue, float maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static double BullyIntoRange(this double value, double minValue, double maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static decimal BullyIntoRange(this decimal value, decimal minValue, decimal maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static byte BullyIntoRange(this byte value, byte minValue, byte maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static short BullyIntoRange(this short value, short minValue, short maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static int BullyIntoRange(this int value, int minValue, int maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static int AtLeast(this int value, int minValue)
        {
            return value.BullyIntoRange(minValue, Int32.MaxValue);
        }

        public static int AtMost(this int value, int maxValue)
        {
            return value.BullyIntoRange(Int32.MinValue, maxValue);
        }

        public static long BullyIntoRange(this long value, long minValue, long maxValue)
        {
            return value < minValue ? minValue : (value > maxValue ? maxValue : value);
        }

        public static long AtLeast(this long value, long minValue)
        {
            return value.BullyIntoRange(minValue, Int64.MaxValue);
        }

        public static long AtMost(this long value, long maxValue)
        {
            return value.BullyIntoRange(Int64.MinValue, maxValue);
        }
    }
}
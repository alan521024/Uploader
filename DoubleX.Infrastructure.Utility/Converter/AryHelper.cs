using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoubleX.Infrastructure.Utility
{
    /// <summary>
    /// 进制转换辅助类
    /// </summary>
    public class AryHelper
    {
        //二进制 Bin 八进制 Oct 十进制 Dec 十六进制 Hex 三十二进制：Hex32

        #region 2进制转为其他进制

        /// <summary>
        /// 二进制转十进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为46
        ///         int result = AryHelper.Bin2Dec("101110");
        ///     </code>
        /// </example>
        /// <param name="bin">二进制字符串</param>
        /// <returns>返回2进制字符串对应的10进制值</returns>
        public static int Bin2Dec(string bin)
        {
            return Convert.ToInt32(bin, 2);
        }

        /// <summary>
        /// 二进制转十六进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为2E
        ///         string result = AryHelper.Bin2Hex("101110");
        ///     </code>
        /// </example>
        /// <param name="bin">二进制字符串</param>
        /// <returns>返回2进制字符串对应的16进制字符串</returns>
        public static string Bin2Hex(string bin)
        {
            return Dec2Hex(Bin2Dec(bin));
        }

        #endregion

        #region 10进制转其他进制

        /// <summary>
        /// 十进制转二进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为101110
        ///         string result = AryHelper.Dec2Bin("46");
        ///     </code>
        /// </example>
        /// <param name="value">十进制数值</param>
        /// <returns>返回10进制数值对应的2进制字符串</returns>
        public static string Dec2Bin(int value)
        {
            return Convert.ToString(value, 2);
        }

        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为2E
        ///         string result = AryHelper.Dec2Bin("46");
        ///     </code>
        /// </example>
        /// <param name="value">十进制数值</param>
        /// <returns>返回10进制数值对应的16进制字符串</returns>
        public static string Dec2Hex(int value)
        {
            return Convert.ToString(value, 16).ToUpper();
        }

        /// <summary>
        /// 十进制转十六进制:格式化十六进制为指定位数，不足位数左边补0
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为002E
        ///         string result = AryHelper.Dec2Bin("46",4);
        ///     </code>
        /// </example>
        /// <param name="value">十进制数值</param>
        /// <param name="formatLength">十六进制结果的总长度</param>
        /// <returns>返回10进制数值对应的指定长度的16进制字符串</returns>
        public static string Dec2Hex(int value, int formatLength)
        {
            string hex = Dec2Hex(value);
            if (hex.Length >= formatLength) return hex;
            return hex.PadLeft(formatLength, '0');
        }
        
        #endregion

        #region  16进制转其他进制

        /// <summary>
        /// 十六进制转十进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为46
        ///         int result = AryHelper.Hex2Dec("2E");
        ///     </code>
        /// </example>
        /// <param name="hex">16进制字符串</param>
        /// <returns>返回16进制对应的十进制数值</returns>
        public static int Hex2Dec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        /// <summary>
        /// 十六进制转二进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为101110
        ///         string result = AryHelper.Hex2Bin("2E");
        ///     </code>
        /// </example>
        /// <param name="hex">16进制字符串</param>
        /// <returns>返回16进制对应的2进制字符串</returns>
        public static string Hex2Bin(string hex)
        {
            return Dec2Bin(Hex2Dec(hex));
        }

        /// <summary>
        /// 十六进制转字节数组
        /// </summary>
        /// <example>
        ///     <code>
        ///         //result的结果为一个长度为2的字节数组
        ///         //其中result[0]=46,result[1]=61
        ///         byte[] result = AryHelper.Hex2Bin("2E3D");
        ///     </code>
        /// </example>
        /// <param name="hex">16进制字符串</param>
        /// <returns>返回16进制对应的字节数组</returns>
        public static byte[] Hex2Bytes(string hex)
        {
            MatchCollection mc = Regex.Matches(hex, @"(?i)[\da-f]{2}");
            return (from Match m in mc select Convert.ToByte(m.Value, 16)).ToArray();

            //hexString = hexString.Replace(" ", "");
            //if ((hexString.Length % 2) != 0)
            //    hexString += " ";
            //byte[] returnBytes = new byte[hexString.Length / 2];
            //for (int i = 0; i < returnBytes.Length; i++)
            //    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            //return returnBytes;   
        }

        #endregion

        #region 自定义进制转换

        //编码,默认(暂时只62),可加一些字符也可以实现72,96等任意进制转换，但是有符号数据不直观，会影响阅读。
        //"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //private static String keys = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        //private static int exponent = keys.Length;//幂数=formatLength

        public static string DecToAry(string keys,decimal value, int formatLength)
        {
            string result = string.Empty;
            if (formatLength > 62) {
                formatLength = 62;
            }
            string currentKey = keys.Substring(0, formatLength);
            do
            {
                decimal index = value % formatLength;
                result = currentKey[(int)index] + result;
                value = (value - index) / formatLength;
            }
            while (value > 0);

            return result;
        }

        public static decimal AryToDec(string keys, string value, int formatLength)
        {
            if (formatLength > 62)
            {
                formatLength = 62;
            }
            string currentKey = keys.Substring(0, formatLength);

            decimal result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int x = value.Length - i - 1;
                result += currentKey.IndexOf(value[i]) * Pow(formatLength, x);// Math.Pow(exponent, x);
            }
            return result;
        }

        /// <summary>
        /// 一个数据的N次方
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static decimal Pow(decimal baseNo, decimal x)
        {
            decimal value = 1;////1 will be the result for any number's power 0.任何数的0次方，结果都等于1
            while (x > 0)
            {
                value = value * baseNo;
                x--;
            }
            return value;
        }

        #endregion

        #region 单字节转为其他进制

        /// <summary>
        /// Byte转16进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte b = 128;
        ///         string hex = AryHelper.Byte2Hex(b);
        ///         Console.WriteLine(hex);//输出结果为80
        ///     </code>
        /// </example>
        /// <param name="b">一个字节</param>
        /// <returns>返回对应的16进制字符串</returns>
        public static string Byte2Hex(byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// 单字节转换为2进制数值
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte b = 128;
        ///         string bin = AryHelper.Byte2Bin(b);
        ///         Console.WriteLine(bin);//输出结果为10000000
        ///     </code>
        /// </example>
        /// <param name="b">一个字节</param>
        /// <returns>返回对应的2进制字符串</returns>
        public static string Byte2Bin(byte b)
        {
            return Dec2Bin(b);
        }

        #endregion

        #region 多个字节转为其他进制

        /// <summary>
        /// 字节数组转ASCII
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte[] buffer = new byte[] {65,66,67};
        ///         string result = AryHelper.Bytes2ASCII(buffer);
        ///         Console.WriteLine(result);//结果输出：ABC
        ///     </code>
        /// </example>
        /// <param name="bytes">字节数组</param>
        /// <returns>返回该字节数组对应的ASCII码字符串</returns>
        public static string Bytes2ASCII(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 字节数组转十六进制
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte[] buffer = new byte[] { 65, 66, 67 };
        ///         string result = AryHelper.Bytes2Hex(buffer);
        ///         Console.WriteLine(result);//结果输出：414243
        ///     </code>
        /// </example>
        /// <param name="bytes">字节数组</param>
        /// <returns>返回该字节数组对应的16进制字符串</returns>
        public static string Bytes2Hex(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 将两个字节转换为十六进制数
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte[] buffer = new byte[] { 65, 66 };
        ///         
        ///         //高位在前，低位在后
        ///         string result = AryHelper.Bytes2Hex(buffer[0],buffer[1]);
        ///         Console.WriteLine(result);//结果输出：4142
        ///         
        ///         //低位在前，高位在后
        ///         result = AryHelper.Bytes2Hex(buffer[1], buffer[0]);
        ///         Console.WriteLine(result);//结果输出：4241
        ///     </code>
        /// </example>
        /// <param name="hByte">高字节</param>
        /// <param name="lByte">低字节</param>
        /// <returns>返回该两个字节对应的16进制数结果</returns>
        public static string Bytes2Hex(byte hByte, byte lByte)
        {
            return Byte2Hex(hByte) + Byte2Hex(lByte);
        }

        /// <summary>
        /// 将两个字节转换为十进制数
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte[] buffer = new byte[] { 65, 66 };
        ///         
        ///         //高位在前，低位在后
        ///         int result = AryHelper.Bytes2Dec(buffer[0], buffer[1]);
        ///         Console.WriteLine(result);//结果输出：16706
        ///         
        ///         //低位在前，高位在后
        ///         result = AryHelper.Bytes2Dec(buffer[1], buffer[0]);
        ///         Console.WriteLine(result);//结果输出：16961
        ///     </code>
        /// </example>
        /// <param name="hByte">高字节</param>
        /// <param name="lByte">低字节</param>
        /// <returns></returns>
        public static int Bytes2Dec(byte hByte, byte lByte)
        {
            return hByte << 8 | lByte;
        }

        /// <summary>
        /// 将两个字节(补码表示)转换为十进制数，如果是补码，则第一个bit为1则表示负数
        /// </summary>
        /// <example>
        ///     <code>
        ///         byte[] buffer = new byte[] { 255, 66 };
        ///         
        ///         //高位在前，低位在后
        ///         int result = AryHelper.Bytes2Dec(buffer[0], buffer[1],false);
        ///         Console.WriteLine(result);//结果输出：65346
        ///         
        ///         //高位在前，低位在后
        ///         result = AryHelper.Bytes2Dec(buffer[0], buffer[1], true);
        ///         Console.WriteLine(result);//结果输出：-190
        ///     </code>
        /// </example>
        /// <param name="hByte">高位字节</param>
        /// <param name="lByte">低位字节</param>
        /// <param name="isRadix">是否是采用补码表示形式</param>
        /// <returns>返回对应的10进制数值</returns>
        public static int Bytes2Dec(byte hByte, byte lByte, bool isRadix)
        {
            var v = (ushort)(hByte << 8 | lByte);//合并高地位为16进制
            if (isRadix)
            {
                if (hByte > 127)
                {
                    v = (ushort)~v; //按位取反
                    v = (ushort)(v + 1); //得到补码
                    return -1 * v;
                }
            }
            return v;
        }

        #endregion
    }
}

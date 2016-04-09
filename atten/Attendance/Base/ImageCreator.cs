using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
namespace Attendance.Base
{
	public static class ImageCreator
	{
		static Color[] colors;
		static Font[] fonts;
		static Random rd = new Random((int)DateTime.Now.Ticks);
		private static void ColorRandom()
		{
			int cr, cg, cb;
			colors = new Color[128];
			for (int i = 0; i < 128; i++)
			{
				cr = rd.Next(0, 255);
				cg = (cr > 128) ? rd.Next(0, 128) : rd.Next(0, 200);
				cb = (cr + cg > 256) ? rd.Next(0, 128) : cb = rd.Next(0, 255);
				colors[i] = Color.FromArgb(cr, cg, cb);
			}
		}
		private static void FontRandom()
		{
			fonts = new Font[3];
			fonts[0] = new Font("Arial Black", 14.0F, FontStyle.Italic);
			fonts[1] = new Font("Verdana", 12.0F, FontStyle.Bold);
			fonts[2] = new Font("Sans-Serif", 12.0F, FontStyle.Regular);
		}
		public static List<SizeF> CalStringSize(List<string> arrStr, List<Font> arrFont, int iFirstWidth = 50)
		{
			List<bool> listBool = new List<bool>();
			List<SizeF> listSize = new List<SizeF>();
			for (int i = 0; i < arrStr.Count; i++)
			{
				listBool.Add(string.IsNullOrEmpty(arrStr[i]));
				listSize.Add(new SizeF(0, 0));
			}
			while (true)
			{
				Bitmap imageTemp = new Bitmap(iFirstWidth, 1);
				Graphics evTemp = Graphics.FromImage(imageTemp);
				int i = 0;
				for (; i < arrStr.Count; i++)
				{
					if (!listBool[i])
					{//当前没有计算
						SizeF size = evTemp.MeasureString(arrStr[i], arrFont[i]);
						if (size.Width < iFirstWidth)
						{
							listBool[i] = true;
							listSize[i] = size;
						}
						else
						{
							iFirstWidth += iFirstWidth;
							break;
						}
					}
				}
				if (i == arrStr.Count)
					break;
			}
			return listSize;
		}
		public static List<Rectangle> CalStringSizeReal(List<string> arrStr, List<Font> arrFont, int iFirstSize = 50)
		{
			int iFirstWidth = iFirstSize;
			int iFirstHeight = iFirstSize;
			List<bool> listBool = new List<bool>();
			List<Rectangle> listSize = new List<Rectangle>();
			for (int i = 0; i < arrStr.Count; i++)
			{
				listBool.Add(string.IsNullOrEmpty(arrStr[i]));
				listSize.Add(Rectangle.Empty);
			}
			while (true)
			{
				Bitmap imageTemp = new Bitmap(iFirstWidth, iFirstHeight);
				Graphics evTemp = Graphics.FromImage(imageTemp);
				Brush brush = new SolidBrush(Color.Black);
				int i = 0;
				for (; i < arrStr.Count; i++)
				{
					if (!listBool[i])
					{//当前没有计算
						evTemp.Clear(Color.White);
						SizeF size = evTemp.MeasureString(arrStr[i], arrFont[i]);
						if (size.Width < iFirstWidth && size.Height < iFirstHeight)
						{
							listBool[i] = true;
							evTemp.DrawString(arrStr[i], arrFont[i], brush, 0, 0);
							listSize[i] = GetImageMinSize(imageTemp);
						}
						else if (size.Width >= iFirstWidth)
						{
							iFirstWidth += iFirstWidth;
							break;
						}
						else
						{
							iFirstHeight += iFirstHeight;
							break;
						}
					}
				}
				if (i == arrStr.Count)
					break;
			}
			return listSize;
		}
		private static Rectangle GetImageMinSize(Bitmap image)
		{
			int iLeft = 0, iTop = 0, iWidth = 0, iHeight = 0;
			int rgb_bg = (Color.White.R + Color.White.G + Color.White.B) / 3;
			for (int i = 0; i < image.Width; i++)
			{
				int j = 0;
				for (; j < image.Height; j++)
				{
					Color color = image.GetPixel(i, j);
					int rgb = (color.R + color.G + color.B) / 3;
					if (rgb != rgb_bg)
					{
						iLeft = i;
						break;
					}
				}
				if (j != image.Height)
					break;
			}
			for (int j = 0; j < image.Height; j++)
			{
				int i = iLeft;
				for (; i < image.Width; i++)
				{
					Color color = image.GetPixel(i, j);
					int rgb = (color.R + color.G + color.B) / 3;
					if (rgb != rgb_bg)
					{
						iTop = j;
						break;
					}
				}
				if (i != image.Width)
					break;
			}
			for (int i = image.Width - 1; i >= iLeft; i--)
			{
				int j = image.Height - 1;
				for (; j >= iTop; j--)
				{
					Color color = image.GetPixel(i, j);
					int rgb = (color.R + color.G + color.B) / 3;
					if (rgb != rgb_bg)
					{
						iWidth = i - iLeft + 1;
						break;
					}
				}
				if (j != iTop - 1)
					break;
			}
			for (int j = image.Height - 1; j >= iTop; j--)
			{
				int i = image.Width - 1;
				for (; i >= iLeft; i--)
				{
					Color color = image.GetPixel(i, j);
					int rgb = (color.R + color.G + color.B) / 3;
					if (rgb != rgb_bg)
					{
						iHeight = j - iTop + 1;
						break;
					}
				}
				if (i != iLeft - 1)
					break;
			}
			return new Rectangle(iLeft, iTop, iWidth, iHeight);
		}
		public static Bitmap CreateAddCodeImage(int numLength = 4, bool onlyNum = false, bool bColor = true, int intWidth = 70, int intHeight = 22, int maxAngle = 30)
		{
			if (numLength <= 0)
				numLength = 1;
			string RdCode = null;
			if (onlyNum)
				RdCode = RndNum(numLength);
			else
				RdCode = RndCode(numLength);
			return CreateAddCodeImage(RdCode, bColor, intWidth, intHeight);
		}
		public static Bitmap CreateAddCodeImage(string RdCode, bool bColor = true, int intWidth = 70, int intHeight = 22, int maxAngle = 30)
		{
			Bitmap image = new Bitmap(intWidth, intHeight);
			Graphics gp = Graphics.FromImage(image);
			gp.Clear(Color.White);
			gp.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, intWidth - 1, intHeight - 1);
			gp.SmoothingMode = SmoothingMode.AntiAlias;
			if (!string.IsNullOrEmpty(RdCode))
			{
				FontRandom();
				ColorRandom();
				List<string> arrStr = new List<string>();
				List<Font> arrFont = new List<Font>();
				for (int i = 0; i < RdCode.Length; i++)
				{
					arrStr.Add(RdCode.Substring(i, 1));
					arrFont.Add(fonts[rd.Next(2)]);
				}
				List<Rectangle> arrSize = CalStringSizeReal(arrStr, arrFont);

				SolidBrush brush;
				int perMargin = 2;
				gp.TranslateTransform(rd.Next(0, perMargin), 0);
				for (int i = 0; i < arrStr.Count; i++)
				{
					brush = new SolidBrush(colors[rd.Next(128)]);
					Font f = arrFont[i];
					float x = (arrSize[i].IsEmpty ? 0 : arrSize[i].Left);
					float y = intHeight - 2 - (arrSize[i].IsEmpty ? 0 : arrSize[i].Height - 1);
					if (y > 0)
						y = (float)(rd.Next((int)y));
					else
						y = 0;

					int angle = rd.Next(-maxAngle, maxAngle);
					float addX = 0;
					float addY = 0;
					double dAnagle = 2 * Math.PI * angle / 360;
					if (angle >= 0)
					{
						addX = (arrSize[i].IsEmpty ? 0 : -arrSize[i].Left) + (float)((arrSize[i].IsEmpty ? 0 : arrSize[i].Height) * Math.Cos(dAnagle) * Math.Sin(dAnagle));
						addY = (arrSize[i].IsEmpty ? 0 : -arrSize[i].Top - arrSize[i].Height) + (float)((arrSize[i].IsEmpty ? 0 : arrSize[i].Height) * Math.Cos(dAnagle) * Math.Cos(dAnagle));
					}
					else
					{
						dAnagle = -dAnagle;
						addX = (arrSize[i].IsEmpty ? 0 : -arrSize[i].Left) - (float)((arrSize[i].IsEmpty ? 0 : arrSize[i].Width) * Math.Sin(dAnagle) * Math.Sin(dAnagle));
						addY = (arrSize[i].IsEmpty ? 0 : -arrSize[i].Top) + (float)((arrSize[i].IsEmpty ? 0 : arrSize[i].Width) * Math.Sin(dAnagle) * Math.Cos(dAnagle));
					}
					gp.TranslateTransform((i > 0 ? (arrSize[i - 1].IsEmpty ? 0 : arrSize[i - 1].Width / (float)Math.Cos(dAnagle)) + rd.Next(0, perMargin) : 0), y);
					gp.RotateTransform(angle);
					gp.DrawString(arrStr[i], f, brush, addX, addY);
					gp.RotateTransform(-angle);
					gp.TranslateTransform(0, -y);
				}
				gp.ResetTransform();
			}
			if (bColor)
			{
				int intNoiseX;
				int intNoiseY;
				Point point1;
				Point point2;
				for (int i = 0; i < image.Height; i = i + 2)
				{
					for (int j = 0; j < image.Width; j = j + 2)
					{
						SolidBrush brushNoise = new SolidBrush(colors[rd.Next(128)]);
						int intShow = rd.Next(0, 90);
						if (intShow % 10 == 1)
						{
							intNoiseX = rd.Next(image.Width);
							intNoiseY = rd.Next(image.Height);
							point1 = new Point(j, i);
							point2 = new Point(j + rd.Next(0, 2), i + rd.Next(0, 4));
							gp.DrawLine(new Pen(brushNoise, 1.5f), point1, point2);
						}
					}
				}
			}
			return image;
		}
		public static string CreateAddCodeImageBase64(int numLength = 4, bool onlyNum = false, bool bColor = true, int intWidth = 70, int intHeight = 22, int maxAngle = 30)
		{
			if (numLength <= 0)
				numLength = 1;
			string RdCode = null;
			if (onlyNum)
				RdCode = RndNum(numLength);
			else
				RdCode = RndCode(numLength);
			return CreateAddCodeImageBase64(RdCode, bColor, intWidth, intHeight, maxAngle);
		}
		public static string CreateAddCodeImageBase64(string RdCode, bool bColor = true, int intWidth = 70, int intHeight = 22, int maxAngle = 30)
		{
			Bitmap image = CreateAddCodeImage(RdCode, bColor, intWidth, intHeight, maxAngle);
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			image.Save(ms, ImageFormat.Gif);
			byte[] buf = ms.ToArray();
			image.Dispose();
			return Convert.ToBase64String(buf);
		}

		#region 生成验证码
		private static string _rndNum(int numLength)
		{
			byte[] bArray = new byte[numLength];
			new Random().NextBytes(bArray);
			string resultStr = string.Empty;
			for (int i = 0; i < bArray.Length; i++)
			{
				resultStr += (bArray[i] % 10).ToString();
			}

			return resultStr;
		}

		public static string RndNum(int numLength)
		{
			string _lastRndNum = string.Empty;
			string newNum = _rndNum(numLength);
			while (newNum == _lastRndNum)
			{
				newNum = _rndNum(numLength);
			}
			_lastRndNum = newNum;
			return newNum;
		}

		public static string RndCode(int numLength)
		{
			int number;
			char code;
			string checkCode = String.Empty;
			System.Random random = new Random();
			for (int i = 0; i < numLength; i++)
			{
				int iTemp = random.Next();
				number = random.Next();
				if (iTemp % 2 == 0)
					code = (char)('0' + (char)(number % 10));
				else
					code = (char)('A' + (char)(number % 26));
				if (code == (char)('I') || code == (char)('L') || code == (char)('0') || code == (char)('1'))
				{
					i--;
					continue;
				}
				checkCode += code.ToString();
			}
			return checkCode;
		}
		#endregion

		#region 基本转换
		public static byte[] StreamToBuffer(Stream stream)
		{
			int iOldPos = (int)stream.Position;
			byte[] bytes = new byte[stream.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(bytes, 0, bytes.Length);
			stream.Position = iOldPos;
			return bytes;
		}
		public static Stream BufferToStream(byte[] bytes)
		{
			MemoryStream stream = new MemoryStream(bytes);
			return stream;
		}
		public static string StreamToString(Stream stream)
		{
			int iOldPos = (int)stream.Position;
			stream.Seek(0, SeekOrigin.Begin);
			StreamReader reader = new StreamReader(stream);
			string str = reader.ReadToEnd();
			stream.Position = iOldPos;//modify by zyf 2011-12-07
			return str;
		}
		public static Stream CloneToStream(this Stream obj)
		{
			var bytes = new byte[obj.Length];
			obj.Read(bytes, 0, bytes.Length);
			obj.Seek(0, SeekOrigin.Begin);
			return new MemoryStream(bytes);
		}
		public static Stream StringToStream(string str)
		{
			return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str));
		}
		public static byte[] StringToBuffer(string str)
		{
			return System.Text.Encoding.UTF8.GetBytes(str);
		}
		public static string BufferToString(byte[] buffer)
		{
			return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
		}
		public static string BufferToBase64String(byte[] buffer)
		{
			return Convert.ToBase64String(buffer);
		}
		public static byte[] StringToBase64Buffer(string str)
		{
			return Convert.FromBase64String(str);
		}
		#endregion
	}
}
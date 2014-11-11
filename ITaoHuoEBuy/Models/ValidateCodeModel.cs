using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EBuy.Models
{
    /// <summary>
    /// 验证码模型
    /// </summary>
    public static class ValidateCodeModel
    {
        public static bool ValidateCode(string inputStr, string strInSession)
        {
            if (inputStr == strInSession)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="length">指定验证码的长度</param>
        /// <returns></returns>
        public static string CreateValidateCode(int length)
        {
            //新建一个长度为length的数组来存放生成的随机数数字
            int[] validateNums = new int[length];
            //使用当前时间作为随机数种子来生成真随机数
            Random rand = new Random((int)DateTime.Now.ToFileTime());
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                validateNums[i] = rand.Next(0, 9);
            }
            //生成验证码
            string validateNumberStr = "";
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 根据验证码创建图片
        /// </summary>
        /// <param name="randomcode">随机码</param>
        /// <returns>验证码图片比特流</returns>
        public static byte[] CreateGraphic(string randomcode)
        {
            //定义颜色  
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //定义字体  
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            //字体旋转的最大角度
            int randAngle = 45;
            //计算验证码图片宽度
            int mapwidth = (int)(randomcode.Length * 23);
            //创建图片
            Bitmap image = new Bitmap(mapwidth, 28); 
            Graphics graph = Graphics.FromImage(image);

            try
            {
                //清除画面，使用淡蓝色填充背景
                graph.Clear(Color.AliceBlue);
                //随机数生成器
                Random random = new Random();
                //拆散字符串成单字符数组
                char[] chars = randomcode.ToCharArray();
                //设置文字居中  
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                //纵向居中
                format.Alignment = StringAlignment.Center;
                //横向居中
                format.LineAlignment = StringAlignment.Center;

                //开始把验证码字母一个个画进图片
                for (int i = 0; i < chars.Length; i++)
                {
                    //随机使用一种字体来绘制当前字母
                    int findex = random.Next(font.Length);
                    Font f = new System.Drawing.Font(font[findex], 13, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)  
                    //随机使用一种颜色来绘制当前字母
                    int cindex = random.Next(c.Length);
                    Brush b = new System.Drawing.SolidBrush(c[cindex]);

                    //生成一个随机转动的角度
                    float angle = random.Next(-randAngle, randAngle); 

                    Point dot = new Point(16, 16);
                    //移动光标到指定位置  
                    graph.TranslateTransform(dot.X, dot.Y);
                    graph.RotateTransform(angle);
                    graph.DrawString(chars[i] + "", f, b, 1, 1, format);
                    //转回去  
                    graph.RotateTransform(-angle);
                    //移动光标到指定位置  
                    graph.TranslateTransform(2, -dot.Y);
                }
                //生成jpeg图片
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                //结束后释放资源
                graph.Dispose();
                image.Dispose();
            }
        }
        
    }
}
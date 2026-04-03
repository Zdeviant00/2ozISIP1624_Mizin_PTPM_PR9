using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Mizin_WPF_PR9.Pages
{
    public partial class CaptchaPage : Page
    {
        private string _captchaCode;
        private AuthPage _authPage;
        private string _userLogin;  

        public CaptchaPage(AuthPage authPage)
        {
            InitializeComponent();
            _authPage = authPage;
            _userLogin = authPage.CurrentLogin;  
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            // Генерация случайного кода CAPTCHA (6 символов)
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Без похожих символов
            Random rand = new Random();
            _captchaCode = "";
            for (int i = 0; i < 6; i++)
                _captchaCode += chars[rand.Next(chars.Length)];

            // Создание изображения CAPTCHA с шумом
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(200, 70))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // Фон с градиентом
                    using (System.Drawing.Drawing2D.LinearGradientBrush brush =
                        new System.Drawing.Drawing2D.LinearGradientBrush(
                            new System.Drawing.Rectangle(0, 0, 200, 70),
                            System.Drawing.Color.LightBlue,
                            System.Drawing.Color.LightYellow, 45F))
                    {
                        g.FillRectangle(brush, 0, 0, 200, 70);
                    }

                    // Добавление шума (линии)
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.LightGray, 1))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int x1 = rand.Next(200);
                            int y1 = rand.Next(70);
                            int x2 = rand.Next(200);
                            int y2 = rand.Next(70);
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                    }

                    // Добавление шума (точки)
                    for (int i = 0; i < 100; i++)
                    {
                        int x = rand.Next(200);
                        int y = rand.Next(70);
                        bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(
                            rand.Next(256), rand.Next(256), rand.Next(256)));
                    }

                    // Текст CAPTCHA с искажением
                    using (System.Drawing.Font font = new System.Drawing.Font(
                        "Arial", 20, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))
                    {
                        using (System.Drawing.Brush brush = new System.Drawing.SolidBrush(
                            System.Drawing.Color.DarkBlue))
                        {
                            for (int i = 0; i < _captchaCode.Length; i++)
                            {
                                float x = 20 + i * 28;
                                float y = 15 + rand.Next(-5, 5);
                                g.DrawString(_captchaCode[i].ToString(), font, brush, x, y);
                            }
                        }
                    }

                    // Волнистые линии поверх текста
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(
                        System.Drawing.Color.FromArgb(100, System.Drawing.Color.Gray), 2))
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            System.Drawing.PointF[] points = new System.Drawing.PointF[5];
                            for (int i = 0; i < 5; i++)
                            {
                                points[i] = new System.Drawing.PointF(i * 50, 35 + rand.Next(-10, 10));
                            }
                            g.DrawCurve(pen, points);
                        }
                    }
                }

                // Конвертация в BitmapImage для WPF
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    CaptchaImage.Source = bitmapImage;
                }
            }
        }

        private void ButtonVerify_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxCaptcha.Text.ToUpper() == _captchaCode)
            {
                // CAPTCHA верна - сбросить счётчик для ЭТОГО логина
                AttemptCounter.ResetAttempts(_userLogin); 
                MessageBox.Show("CAPTCHA пройдена! Теперь вы можете войти.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(_authPage);
            }
            else
            {
                TextBlockError.Text = "Неверная CAPTCHA! Попробуйте ещё раз.";
                TextBlockError.Visibility = Visibility.Visible;
                GenerateCaptcha();
                TextBoxCaptcha.Clear();
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            TextBoxCaptcha.Clear();
            TextBlockError.Visibility = Visibility.Collapsed;
        }
    }
}
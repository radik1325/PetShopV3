using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PetShop.Pages
{
    public partial class LoginPage : Page
    {
        private string captchaText;
        private readonly Random random = new Random();
        private bool isCaptchaVisible = false; // Поле для отслеживания отображения капчи

        public LoginPage()
        {
            InitializeComponent();
            GenerateCaptcha();
            CaptchaContainer.Visibility = Visibility.Collapsed; // Изначально скрываем капчу
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(LoginTextBox.Text))
                {
                    errors.AppendLine("Заполните логин");
                }
                if (string.IsNullOrEmpty(LoginPassBox.Password))
                {
                    errors.AppendLine("Заполните пароль");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Errors!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool isValidUser = Data.Pet_shopEntities.GetContext().User
                    .Any(d => d.UserLogin == LoginTextBox.Text
                    && d.UserPassword == LoginPassBox.Password);

                if (isValidUser)
                {
                    var user = Data.Pet_shopEntities.GetContext().User
                        .Where(d => d.UserLogin == LoginTextBox.Text
                        && d.UserPassword == LoginPassBox.Password).FirstOrDefault();

                    switch (user.Role.RoleName)
                    {
                        case "Администратор":
                            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
                            break;
                        case "Клиент":
                            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
                            break;
                        case "Менеджер":
                            Classes.Manager.MainFrame.Navigate(new Pages.ViewProductPage());
                            break;
                    }
                    MessageBox.Show("Успех!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    isCaptchaVisible = false; // Сброс состояния капчи
                    CaptchaContainer.Visibility = Visibility.Collapsed; // Скрыть капчу
                }
                else
                {
                    MessageBox.Show("Некоректные данные", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (!isCaptchaVisible)
                    {
                        ResultText.Text = "При неудачом вводе следующая попытка через 10 секунд";
                        CaptchaInput.Text = "";
                        isCaptchaVisible = true; // Отметить, что капча должна быть показана
                        CaptchaContainer.Visibility = Visibility.Visible; // Показать капчу
                        GenerateCaptcha(); // Генерация новой капчи
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализация для кнопки гостя
        }

       
        private void GenerateCaptcha()
        {
            
            captchaText = GenerateRandomText(4);

            int width = 150;  
            int height = 40;  

            
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                
                DrawDistortedText(dc, width, height, 24);

                
                AddNoise(dc, width, height);
            }

            
            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);

            // Установка источника изображения для элемента управления
            CaptchaImage.Source = bitmap;
        }

        // Метод для рисования искаженного текста капчи
        private void DrawDistortedText(DrawingContext dc, int width, int height, int fontSize)
        {
            Typeface typeface = new Typeface("Arial");  // Шрифт для текста капчи
            double x = 10;  // Начальная позиция X для текста
            double y = 15;  // Начальная позиция Y для текста

            // Перебор каждого символа текста капчи
            foreach (char c in captchaText)
            {
                // Создание отформатированного текста для символа
                FormattedText text = new FormattedText(
                    c.ToString(),
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    Brushes.Black);

                // Случайное смещение текста по X и Y
                double xOffset = random.Next(-5, 5);
                double yOffset = random.Next(-5, 5);
                dc.PushTransform(new TranslateTransform(x + xOffset, y + yOffset));
                dc.DrawText(text, new Point(0, 0));
                dc.Pop();

                // Перемещение позиции для следующего символа
                x += text.WidthIncludingTrailingWhitespace + 10;

                // Переход на новую строку, если достигнут край изображения
                if (x > width - 50)
                {
                    x = 10;
                    y += fontSize + 10;
                }
            }
        }

        // Метод для добавления шума к изображению капчи
        private void AddNoise(DrawingContext dc, int width, int height)
        {
            Pen pen = new Pen(Brushes.Gray, 1);

            // Рисование случайных линий
            for (int i = 0; i < 5; i++)
            {
                dc.DrawLine(pen, new Point(random.Next(width), random.Next(height)), new Point(random.Next(width), random.Next(height)));
            }

            // Рисование случайных эллипсов
            for (int i = 0; i < 20; i++)
            {
                dc.DrawEllipse(Brushes.LightGray, null, new Point(random.Next(width), random.Next(height)), 2, 2);
            }
        }

        // Метод для генерации случайного текста капчи
        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";  // Доступные символы для капчи
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];  // Выбор случайного символа
            }
            return new string(stringChars);
        }

        // Обработчик клика на кнопку "Submit"
        public void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка введенного текста капчи
            if (CaptchaInput.Text.Equals(captchaText, StringComparison.OrdinalIgnoreCase))
            {
                ResultText.Text = "Captcha is correct!";  // Сообщение о правильности капчи
                isCaptchaVisible = false;  // Сброс состояния видимости капчи
                CaptchaContainer.Visibility = Visibility.Collapsed;  // Скрытие контейнера капчи
            }
            else
            {
                CaptchaInput.Text = "";  // Очистка введенного текста капчи

                // Задержка перед генерацией новой капчи
                Thread.Sleep(10000);

                // Генерация новой капчи
                GenerateCaptcha();
            }
        }
    }
}

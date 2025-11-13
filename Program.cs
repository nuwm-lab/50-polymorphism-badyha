using System;
using System.Collections.ObjectModel;
using System.Text;

namespace LabWork
{
    // Даний проект є шаблоном для виконання лабораторних робіт
    // з курсу "Об'єктно-орієнтоване програмування та патерни проектування"
    // Необхідно змінювати і дописувати код лише в цьому проекті
    // Відео-інструкції щодо роботи з github можна переглянути 
    // за посиланням https://www.youtube.com/@ViktorZhukovskyy/videos 
    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування консолі для коректного відображення українських символів
            // Встановлюємо UTF-8 для введення/виведення
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Якщо в PowerShell або консолі досі проблеми з кодуванням, можна також виконати у терміналі: chcp 65001

            // Демонстрація роботи класів Vector4 та Matrix4x4
            var vec = new Vector4();
            vec.SetElements(new double[] { 1.5, -2.0, 3.25, 0.75 });
            Console.WriteLine("Vector (size 4):");
            vec.Print();
            Console.WriteLine($"Max element in vector: {vec.MaxElement():F2}\n");

            var mat = new Matrix4x4();
            // Заповнимо матрицю прикладними значеннями
            mat.SetElements(new double[] {
                1, 2, 3, 4,
                -1.5, 0, 7, 2.25,
                3.14, -9, 8, 0,
                0.5, 0.5, 0.5, 0.5
            });
            Console.WriteLine("Matrix (4x4):");
            mat.Print();
            Console.WriteLine($"Max element in matrix: {mat.MaxElement():F2}");

            // ---- Доповнення: демонстрація поліморфізму та перевірки граничних випадків ----
            Console.WriteLine("\n-- Поліморфізм: Vector4 reference -> Matrix4x4 instance --");
            Vector4 poly = new Matrix4x4();
            try
            {
                // Передаємо 16 елементів через посилання базового типу — викличеться перевизначений метод Matrix4x4.SetElements
                poly.SetElements(new double[] {
                    1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16
                });
                Console.WriteLine("Called SetElements(16) on Vector4 reference to Matrix4x4 -> succeeded and used overridden implementation.");
                poly.Print();
                Console.WriteLine($"Max via base reference: {poly.MaxElement():F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Polymorphism demo error: {ex.Message}");
            }

            // Негативні перевірки: неправильна довжина та null
            Console.WriteLine("\n-- Перевірка граничних випадків --");
            try
            {
                vec.SetElements(new double[] { 1.0, 2.0 });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Expected error for wrong-length vector: {ex.Message}");
            }

            try
            {
                mat.SetElements(null);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Expected error for null matrix input: param={ex.ParamName}");
            }

            // ---- Інтерактивна демонстрація поліморфізму ----
            // Користувач обирає, з яким об'єктом працювати, і виклик віртуальних методів
            // здійснюється через посилання на базовий клас (Vector4).
            Console.WriteLine("\n========== ІНТЕРАКТИВНА ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ ==========");
            Console.WriteLine("Оберіть режим роботи:");
            Console.WriteLine("  1 — Працювати з Vector4 (вектор розмірності 4)");
            Console.WriteLine("  2 — Працювати з Matrix4x4 (матриця 4x4)");
            Console.Write("Ваш вибір (1 або 2): ");
            
            string userInput = Console.ReadLine() ?? "";
            char userChoose = userInput.Length > 0 ? userInput[0] : '0';

            // Основна змінна типу базового класу — буде посилання на Vector4 або Matrix4x4
            Vector4 obj = null;

            if (userChoose == '1')
            {
                // Користувач обирає Vector4
                obj = new Vector4();
                Console.WriteLine("\n[Вибрано: Vector4]");
                Console.WriteLine("Введіть 4 числа через пробіл (наприклад: 1.5 -2 3.25 0.75):");
                string input = Console.ReadLine() ?? "";
                try
                {
                    string[] parts = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] values = new double[4];
                    for (int i = 0; i < Math.Min(4, parts.Length); i++)
                    {
                        if (double.TryParse(parts[i], System.Globalization.CultureInfo.InvariantCulture, out double val))
                            values[i] = val;
                    }
                    obj.SetElements(values);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час введення: {ex.Message}");
                    return;
                }
            }
            else if (userChoose == '2')
            {
                // Користувач обирає Matrix4x4
                obj = new Matrix4x4();
                Console.WriteLine("\n[Вибрано: Matrix4x4]");
                Console.WriteLine("Введіть 16 чисел через пробіл (наприклад: 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16):");
                string input = Console.ReadLine() ?? "";
                try
                {
                    string[] parts = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    double[] values = new double[16];
                    for (int i = 0; i < Math.Min(16, parts.Length); i++)
                    {
                        if (double.TryParse(parts[i], System.Globalization.CultureInfo.InvariantCulture, out double val))
                            values[i] = val;
                    }
                    obj.SetElements(values);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка під час введення: {ex.Message}");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Невідомий вибір. Завершення.");
                return;
            }

            // На цьому етапі компіляції ми знаємо тільки те, що obj є Vector4 (базовий клас).
            // Проте на етапі виконання програми об'єкт може бути Vector4 або Matrix4x4.
            // Виклик віртуальних методів (Print, MaxElement) здійснюється через динамічну диспетчеризацію.

            Console.WriteLine("\n-- Вивід об'єкта (через базовий тип Vector4) --");
            obj.Print("F2");  // Виклик віртуального методу Print

            Console.WriteLine("\n-- Максимальний елемент (через базовий тип Vector4) --");
            double maxVal = obj.MaxElement();  // Виклик віртуального методу MaxElement
            Console.WriteLine($"Макс: {maxVal:F2}");

            Console.WriteLine("\n-- Інформація про об'єкт --");
            Console.WriteLine($"Тип об'єкта під час виконання: {obj.GetType().Name}");
            Console.WriteLine("✓ Приклад поліморфізму: метод викликається на об'єкті базового типу,");
            Console.WriteLine("  але виконується його перевизначена версія з похідного класу (якщо такий вибраний)!");
        }
    }

    /// <summary>
    /// Одновимірний вектор розмірності 4.
    /// </summary>
    
    public class Vector4
    {
        // Приватне поле, захищений доступ через властивість для похідних класів
        private double[] _elements = new double[4];

        /// <summary>
        /// Надає захищений доступ до елементів як незмінного списку.
        /// Повертається ReadOnlyCollection, щоб уникнути випадкового зовнішнього модифікування внутрішнього масиву.
        /// Похідні класи можуть перевизначити цю властивість при потребі.
        /// </summary>
        protected virtual ReadOnlyCollection<double> Elements => Array.AsReadOnly(_elements);

        /// <summary>
        /// Індексатор для зручного доступу до елементів вектора (0..3).
        /// Підтримує отримання та встановлення значень з базовою валідацією індексу.
        /// </summary>
        public virtual double this[int index]
        {
            get
            {
                if (index < 0 || index >= _elements.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _elements[index];
            }
            set
            {
                if (index < 0 || index >= _elements.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _elements[index] = value;
            }
        }

        /// <summary>
        /// Конструктор: ініціалізує вектор нулями.
        /// </summary>
        public Vector4()
        {
        }

        /// <summary>
        /// Конструктор: ініціалізує вектор значеннями з масиву (довжина 4).
        /// </summary>
        public Vector4(double[] values)
        {
            SetElements(values);
        }

        /// <summary>
        /// Задати елементи вектора (масив повинен мати довжину 4).
        /// Це віртуальний метод: похідні класи можуть перевизначити поведінку.
        /// </summary>
        public virtual void SetElements(double[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 4)
            {
                throw new ArgumentException($"Expected {nameof(values)} to contain exactly 4 elements.", nameof(values));
            }

            for (int i = 0; i < 4; i++)
            {
                _elements[i] = values[i];
            }
        }

        /// <summary>
        /// Вивести вектор на екран. Параметр формат визначає формат кожного числа.
        /// </summary>
        public virtual void Print(string format = "G")
        {
            for (int i = 0; i < _elements.Length; i++)
            {
                if (i > 0) Console.Write(", ");
                Console.Write(this[i].ToString(format));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Знайти максимальний елемент вектора.
        /// </summary>
        public virtual double MaxElement()
        {
            double max = this[0];
            for (int i = 1; i < _elements.Length; i++)
            {
                if (this[i] > max)
                {
                    max = this[i];
                }
            }

            return max;
        }
    }

    /// <summary>
    /// Матриця 4x4 як похідний клас від Vector4.
    /// Примітка: наслідування від Vector4 демонстраційне для лабораторної.
    /// У реальному застосуванні композиція (Matrix має Vector[4]) зазвичай краща.
    /// Тут ми робимо контракт явним: SetElements перевизначається і очікує 16 елементів.
    /// Якщо хтось викликає SetElements на екземплярі Matrix4x4 (навіть через посилання Vector4),
    /// викличеться перевизначення і буде перевірено довжину масиву.
    /// </summary>
    public class Matrix4x4 : Vector4
    {
        // Приватне поле для зберігання елементів матриці (рядково)
        private double[] _data = new double[16];

        /// <summary>
        /// Конструктор: ініціалізує матрицю нулями.
        /// </summary>
        public Matrix4x4()
        {
        }

        /// <summary>
        /// Конструктор: ініціалізує матрицю значеннями з масиву довжини 16.
        /// </summary>
        public Matrix4x4(double[] values)
        {
            SetElements(values);
        }

        /// <summary>
        /// Задати елементи матриці з масиву довжини 16 (4x4).
        /// Перевизначає метод бази; контракт для матриці - 16 елементів.
        /// </summary>
        public override void SetElements(double[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 16)
            {
                throw new ArgumentException($"Expected {nameof(values)} to contain exactly 16 elements (4x4).", nameof(values));
            }

            for (int i = 0; i < 16; i++)
            {
                _data[i] = values[i];
            }
        }

        /// <summary>
        /// Вивести матрицю у вигляді 4 рядків по 4 елементи.
        /// </summary>
        public override void Print(string format = "G")
        {
            for (int r = 0; r < 4; r++)
            {
                Console.Write("[ ");
                for (int c = 0; c < 4; c++)
                {
                    if (c > 0) Console.Write(", ");
                    Console.Write(_data[r * 4 + c].ToString(format));
                }

                Console.WriteLine(" ]");
            }
        }

        /// <summary>
        /// Знайти максимальний елемент матриці.
        /// </summary>
        public override double MaxElement()
        {
            double max = _data[0];
            for (int i = 1; i < _data.Length; i++)
            {
                if (_data[i] > max)
                {
                    max = _data[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Індексатор для лінійного доступу до елементів матриці (0..15).
        /// Це перевизначення індексатора в базі Vector4.
        /// </summary>
        public override double this[int index]
        {
            get
            {
                if (index < 0 || index >= _data.Length) throw new ArgumentOutOfRangeException(nameof(index));
                return _data[index];
            }
            set
            {
                if (index < 0 || index >= _data.Length) throw new ArgumentOutOfRangeException(nameof(index));
                _data[index] = value;
            }
        }
    }
}

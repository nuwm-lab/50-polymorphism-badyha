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
        }
    }

    /// <summary>
    /// Одновимірний вектор розмірності 4.
    /// </summary>
    
    public class Vector4
    {
    // Розміри як константи, щоб уникнути магічних чисел
    protected const int VectorSize = 4;
    protected const int MatrixSize = 16;

        // Приватне поле для збереження елементів вектора
        private double[] _elements = new double[VectorSize];

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
        /// <summary>
        /// Встановити елементи. Для сумісності (LSP) метод приймає масив довжини 4 або 16.
        /// - Якщо довжина 4 — заповнюються елементи вектора.
        /// - Якщо довжина 16 — за замовчуванням делегується віртуальному обробнику, який
        ///   може бути перевизначений у похідних класах (наприклад, Matrix4x4).
        /// </summary>
        public virtual void SetElements(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length == VectorSize)
            {
                for (int i = 0; i < VectorSize; i++)
                    _elements[i] = values[i];
                return;
            }

            if (values.Length == MatrixSize)
            {
                // Делегуємо обробку масиву довжини 16 похідному типу, якщо він її підтримує.
                // За замовчуванням цей метод в базі кидає ArgumentException.
                HandleMatrixLengthElements(values);
                return;
            }

            throw new ArgumentException($"Expected {nameof(values)} to contain exactly {VectorSize} or {MatrixSize} elements.", nameof(values));
        }

        /// <summary>
        /// Обробник для масивів довжини 16. За замовчуванням кидає ArgumentException.
        /// Matrix4x4 перевизначає цей метод, щоб заповнити свою внутрішню структуру.
        /// </summary>
        protected virtual void HandleMatrixLengthElements(double[] values)
        {
            throw new ArgumentException($"This instance does not support arrays of length {MatrixSize}.", nameof(values));
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
        private double[] _data = new double[MatrixSize];

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
        /// <summary>
        /// Обробляє масив довжини 16 і заповнює внутрішній масив матриці.
        /// Це перевизначення обробника з базового класу, який дозволяє LSP-совісну поведінку.
        /// </summary>
        protected override void HandleMatrixLengthElements(double[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (values.Length != MatrixSize) throw new ArgumentException($"Expected {nameof(values)} to contain exactly {MatrixSize} elements (4x4).", nameof(values));

            for (int i = 0; i < MatrixSize; i++)
                _data[i] = values[i];
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
        /// Повертає представлення елементів матриці як IReadOnlyList.
        /// Перевизначає Elements базового класу, щоб відображати 16 значень.
        /// </summary>
        protected override ReadOnlyCollection<double> Elements => Array.AsReadOnly(_data);

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

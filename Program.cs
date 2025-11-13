using System;

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
        /// Захищений доступ до внутрішнього масиву (тільки для читання посилання).
        /// Похідні класи можуть використовувати це для оптимізації, але змінювати
        /// вміст слід обережно через встановлені контракти методів.
        /// </summary>
        protected double[] Elements => _elements;

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
                throw new ArgumentException("Vector must have exactly 4 elements.");
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
                Console.Write(_elements[i].ToString(format));
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Знайти максимальний елемент вектора.
        /// </summary>
        public virtual double MaxElement()
        {
            double max = _elements[0];
            for (int i = 1; i < _elements.Length; i++)
            {
                if (_elements[i] > max)
                {
                    max = _elements[i];
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
                throw new ArgumentException("Matrix must have exactly 16 elements (4x4).");
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
    }
}

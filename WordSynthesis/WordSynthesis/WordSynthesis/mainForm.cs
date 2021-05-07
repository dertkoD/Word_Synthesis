using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordSynthesis
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        string alphabet; // Алфавит (набор генов)
        string target;   // Целевое слово   
        string[] population;
        int[] fitness;
        int m;// Кол-во особей (слов)
        int n; // Кол-во генов (символов в слове)
        int pokolenie = 0;//номер поколения

        static void GetPopulation(int m, int n, string alphavit, out string[] population)
        {
            Random random = new Random();
            population = new string[m];

            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    population[i] += Convert.ToString(alphavit[random.Next(0, alphavit.Length - 1)]);
                }
            }
        }
        static void OutputInForm(string[] population, int[] fitness, ListBox lBox, ListBox lBox2)
        {
            int x;
            x = population.Length;
            lBox.Items.Clear();
            lBox2.Items.Clear();

            for (var i = 0; i < x; i++)
            {
                lBox.Items.Add(population[i]);
                lBox2.Items.Add(Convert.ToString(fitness[i]));
            }
        }

        //Расчет функции приспособленности для каждой особи (массив Fitness)
        static void GetFitness(string[] population, int n, string target, out int[] fitness)
        {
            int x = population.Length;
            int col = 0;
            string y;
            fitness = new int[x];

            for (var i = 0; i < x; i++)
            {
                y = population[i];
                for (var j = 0; j < n; j++)
                {
                    if (y[j] == target[j]) col++;
                }
                fitness[i] = col;
                col = 0;
            }
        }

        //Сортировка массивов population и fitness по убыванию приспособленности
        static void Sort(ref string[] population, ref int[] fitness, int n)
        {
            int x = population.Length;
            int buf;
            string bufstring;

            for (int j = x - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    if (fitness[i] < fitness[i + 1])
                    {
                        buf = fitness[i];
                        bufstring = population[i];
                        fitness[i] = fitness[i + 1];
                        population[i] = population[i + 1];
                        fitness[i + 1] = buf;
                        population[i + 1] = bufstring;
                    }
                }
            }
        }

        // Скрещивание (одноточечное)
        // parent0, parent1 - родители
        // child0, child1 - потомки
        static void Crossover(string parent0, string parent1,
                              out string child0, out string child1)
        {
            Random random = new Random();
            int k = random.Next(1, parent0.Length - 2); // Следующее число для точки деления хромосомы (строки)
            child0 = ""; child1 = "";

            for (int i = 0; i <= k - 1; i++)
                child0 = child0 + parent0[i];
            for (int i = k; i <= parent0.Length - 1; i++)
                child0 = child0 + parent1[i];
            for (int i = 0; i <= k - 1; i++)
                child1 = child1 + parent1[i];
            for (int i = k; i <= parent0.Length - 1; i++)
                child1 = child1 + parent0[i];
        }

        // Мутация: в случайной особи (слове) заменяется случайный ген (буква).
        // Новый ген (буква) случайно выбирается из набора генов (алфавита).
        static void Mutation(ref string[] population, string alphabet)
        {
            string b, b1, b2;
            Random random = new Random();
            int x, y, z;
            int a = population.Length;
            x = random.Next(alphabet.Length - 1); // Случайный номер буквы алфавита
            y = random.Next(a - 1);              // Случайный номер особи в популяции
            b = population[y];
            z = random.Next(b.Length - 1); // Случайный номер гена в особи (буквы в слове)
            b1 = b.Remove(z, 1);
            b2 = b1.Insert(z, Convert.ToString(alphabet[x]));
            population[y] = b2;
        }

        // Формирование новой популяции 
        // population – исходный и результирующий массив
        static void NewPopulation(int Cross, int Mut, ref string[] population, string alphabet)
        {
            // Из первой половины отсортированной популяции случайно выбираются две особи, которые будут родителями: parent0 и parent1.
            // В результате скрещивания (п/п Crossover) получаются потомки: child0 и child1.
            // Эти потомки заменяют две cлучайные особи особи во второй половине популяции.
            // Процесс повторяется k раз.
            // Затем происходит мутация (подпрограмма Mutation).
            string parent0, parent1, child0, child1;
            int x1, x2, x3, x4;
            int y = population.Length;
            Random random = new Random();

            for (int i = 0; i < Cross; i++)
            {
                x1 = random.Next(0, y / 2);   // Сл. число  - номер 1-го родителя                              
                x2 = random.Next(0, y / 2);   // Сл. число - номер 2-го родителя                              
                parent0 = population[x1];
                parent1 = population[x2];
                Crossover(parent0, parent1, out child0, out child1);

                x3 = random.Next(y / 2, y);  // Сл. числа  - номера заменяемых особей
                x4 = random.Next(y / 2, y);
                population[x3] = child0;
                population[x4] = child1;
            }

            for (int i = 0; i < Mut; i++)
                Mutation(ref population, alphabet);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pokolenie = 1;
            label6.Text = "Номер поколения:" + pokolenie.ToString();
            alphabet = textBox1.Text;
            target = textBox2.Text;
            m = Convert.ToInt32(textBox5.Text);
            n = target.Length;
            GetPopulation(m, n, alphabet, out population);
            GetFitness(population, n, target, out fitness);
            Sort(ref population, ref fitness, n);
            OutputInForm(population, fitness, listBox1, listBox2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            n = target.Length;
            pokolenie += 1;
            label6.Text = "Номер поколения:" + pokolenie.ToString();
            int Cross = Convert.ToInt32(textBox4.Text);
            int Mut = Convert.ToInt32(textBox3.Text);
            NewPopulation(Cross, Mut, ref population, alphabet);
            GetFitness(population, n, target, out fitness);
            Sort(ref population, ref fitness, n);
            OutputInForm(population, fitness, listBox1, listBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                button2_Click(sender, e);
            }
        }
    }
}

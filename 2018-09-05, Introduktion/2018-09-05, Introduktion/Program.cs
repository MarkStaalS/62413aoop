using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace _2018_09_05__Introduktion
{
    class Program
    {
        static void Main(string[] args)
        {
            p04();
            p05();
            
        }
        static void p04()
        {
            Console.WriteLine("Hello world");
        }
        static void p05()
        {
            /*
             *Prompts user for input and reads the user input 
             */
            Console.WriteLine("Please enter first number: ");
            string inp = Console.ReadLine();
            double num0 = Convert.ToDouble(inp);
            Console.WriteLine("Please enter action: ");
            string act = Console.ReadLine();
            Console.WriteLine("Please enter second number: ");
            inp = Console.ReadLine();
            double num1 = Convert.ToDouble(inp);
            /*
             *Creates object to handel calculations 
             */
            calc o = new calc();
            double result = o.calcLogic(num0, act, num1);
            /*
             * Displays the final result
             */
            Console.WriteLine("{0:n} {1} {2:n} = {3:n}",num0, act, num1, result);
        }
    }
    public class calc{
        public double calcLogic(double x, string act, double y)
        {
            double result = 0;
            switch (act)
            {
                case "+":
                    result = add(x, y);
                    break;
                case "-":
                    result = sub(x, y);
                    break;
                case "*":
                    result = multi(x, y);
                    break;
                case "/":
                    result = div(x, y);
                    break;
            }
            return result;
        }

        private double add(double x, double y)
        {
            return x + y;
        }
        private double sub(double x, double y)
        {
            return x - y;
        }
        private double multi(double x, double y)
        {
            return x * y;
        }
        private double div(double x, double y)
        {
            return x / y;
        }
    }
}

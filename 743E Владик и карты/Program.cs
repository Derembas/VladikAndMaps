using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace _743E_Владик_и_карты
{
    class Program
    {

        private static int n;
        private static int[] CardsCount = new int[8];
        private static int MaxLen = 0;
        public static byte[] Stack = new byte[8];
        public static List<int>[] EachCards = new List<int>[8];

        static void Main(string[] args)
        {
            //Stopwatch Counter = new Stopwatch();
            //Stopwatch StepCount = new Stopwatch();
            n =Convert.ToInt32( Console.ReadLine());
            //n = 9;
            //System.Random Rnd = new System.Random();
            //string Start = "1 2 3 4 5 6 7 8 8";
            //string Start = "5 2 2 7 5 2 6 4 3 8 1 8 4 2 7";
            //Случайное заполнение массива
            //string Start = Rnd.Next(1, 9).ToString();
            //for (int i = 1; i < n; i++)
            //{
            //    Start += " " + Rnd.Next(1, 9);
            //}
            //Console.WriteLine(Start);
            string[] Mass = Console.ReadLine().Split(' ');
            //string[] Mass = Start.Split(' ');
            //Counter.Start();
            // Считываем данные
            byte[] AllCards = new byte[n];
            for (byte i=0; i < 8; i++) { EachCards[i] = new List<int>(); }
            for (int i=0; i< n; i++)
            {
                byte CurPoint= Convert.ToByte(Mass[i]);
                AllCards[i] = CurPoint;
                EachCards[CurPoint-1].Add(i);
                CardsCount[CurPoint-1] += 1;
            }
            //GlobalVars.Stack = new byte[8];
            MaxLen = 0;

            // Находим решение для случая, когда еоличество всех цифр одинаково
            int LeftBorder = 0;
            int RightBorder = CardsCount.Min();
            //StepCount.Start();
            int CurMaxlen;
            int CurMass = (int)Math.Ceiling((double) RightBorder/ 2);
            bool[] Done = CreateDone(); // Проверенные цыфры. Если цифры нет, то она помечается как проверенная
            while (LeftBorder != RightBorder)
            {
                //StepCount.Restart();
                CurMaxlen = 0;
                Solve(0, 0, CurMass, Done, ref CurMaxlen, false);
                if (CurMaxlen == 0) { RightBorder = Math.Max(LeftBorder, CurMass - 1); }
                else
                {
                    MaxLen = Math.Max(MaxLen, CurMaxlen);
                    LeftBorder = Math.Min(RightBorder, CurMass);
                }
                //Console.WriteLine("Kol=" + CurMass + " - MaxLen=" + CurMaxlen);
                CurMass = LeftBorder + (int)Math.Ceiling((double)(RightBorder - LeftBorder) / 2);
            }
            //Console.WriteLine("Ans=" + CurMass);
            // Находим полное решение
            //StepCount.Restart();
            CurMaxlen = 0;
            Solve(0, 0, CurMass, Done,ref CurMaxlen, true);
            Console.WriteLine(CurMaxlen);
            //StepCount.Stop();
            //Console.WriteLine("Решение за " + Counter.ElapsedMilliseconds + " мс.");
            //Console.ReadKey();
        }

        // Функция добавления новой пары в массив
        static void Add(PairLine[] Arr, Pair CheckPair, byte Step, out bool Out, out PairLine[] NewList)
        {
            Out = true;
            NewList = new PairLine[8];
            for (byte i=(byte)(Step+1); i<8; i++)
            {
                NewList[i].Line = new List<Pair>();
                NewList[i].MinLen = n;
                foreach(Pair CurPair in Arr[i].Line)
                {
                    if (CurPair.EndPoz < CheckPair.StartPoz || CurPair.StartPoz > CheckPair.EndPoz)
                    {
                        NewList[i].Line.Add(CurPair);
                        NewList[i].MinLen = Math.Min(NewList[i].MinLen, CurPair.Lenth);
                    }
                }
                //string RezText = "";
                //for (int k=0; k<8; k++)
                //{
                //    if (NewList[k].Line != null) { RezText += NewList[k].Line.Count + " "; }
                //    else { RezText += "0 "; }
                //}
                //Console.WriteLine(RezText);
                if (NewList[i].Line.Count == 0)
                {
                    //Console.WriteLine("Step: " + Step + " Was: " + Arr[i].Line.Count);
                    Out = false;
                    return;
                }
            }
        }

        // Рекурсивная функция поиска решения
        static void Solve(int CurEnd, int CurLen, int CurMass, bool[] Done, ref int CurMaxLen, bool Final )
        {
            // CurEnd - номер в массиве AllCards
            // CurLen - Текущая общая длина длина уже выбранных пар
            // CurMass - Длиня каждой пары
            // Done - Какие цыфры уже обработаны
            // CurMaxLen - Найденная на данный момент максимальная длина
            
            // Если обработыны все цыфры возвращаем результат
            if (AllDone(Done))
            {
                CurMaxLen = Math.Max(CurMaxLen, CurLen);
                return;
            }
            // Если CurMaxLen=8*(CurMas+1) - то больше уже не найдём
            if (CurMaxLen == 8 * (CurMass + 1)) { return; }
            // Выбираем из массива всех пар самые левые для каждого значения карты
            List<Pair> ThisRoundMass = new List<Pair>();
            bool[] AllNumHavePair = new bool[8];
            for (byte i=0; i<8;i++)
            {
                if (!Done[i])
                {
                    Pair CurPair;
                    if (Final)
                    {
                        CurPair = LeftPair(i, CurEnd, CurMass + 1);
                        if (CurPair.Lenth != 0)
                        {
                            ThisRoundMass.Add(CurPair);
                            AllNumHavePair[i] = true;
                        }
                    }
                    
                    CurPair = LeftPair(i, CurEnd, CurMass);
                    if (CurPair.Lenth != 0)
                    {
                        ThisRoundMass.Add(CurPair);
                        AllNumHavePair[i] = true;
                    }
                }
                else { AllNumHavePair[i] = true; }
            }
            // Если для какого то числа нет пар выходим из цикла
            if (AllDone(AllNumHavePair))
            {
                foreach (Pair ThisRoundPair in ThisRoundMass)
                {
                    CurLen += ThisRoundPair.Mass;
                    Done[ThisRoundPair.Point] = true;
                    Solve(ThisRoundPair.EndPoz, CurLen, CurMass, Done,ref CurMaxLen, Final);
                    CurLen -= ThisRoundPair.Mass;
                    Done[ThisRoundPair.Point] = false;
                }
            }
        }
        
        // Функция выбора самой левой пары со значением больше Min
        static Pair LeftPair(byte Point, int Min, int Mass)
        {
            Pair Rezult = new Pair();
            if (Mass==0)
            {
                Rezult.EndPoz = Min;
                Rezult.Mass = 0;
                Rezult.Point = Point;
                Rezult.Lenth = 1;
            }
            else
            {
                int StartPoz = Next(Point, Min);
                if (EachCards[Point].Count > StartPoz + Mass-1)
                {
                    Rezult.StartPoz = EachCards[Point][StartPoz];
                    Rezult.EndPoz = EachCards[Point][StartPoz+ Mass-1];
                    Rezult.Mass = Mass;
                    Rezult.Point = Point;
                    Rezult.Lenth = Rezult.EndPoz-Rezult.StartPoz+1;
                }
            }
            
            return Rezult;
        } 

        // Определение самого левого символа в оставшемся массиве со значением больше мин
        static int Next(byte Point, int Min)
        {
            int LeftBorder = 0;
            int RightBorder = EachCards[Point].Count;
            int Answer = RightBorder/2;
            while (LeftBorder!=RightBorder)
            {
                if (EachCards[Point][Answer] < Min) { LeftBorder= Math.Min(RightBorder, Answer + 1); }
                else { RightBorder = Math.Max(LeftBorder, Answer); }
                Answer = LeftBorder + (RightBorder - LeftBorder) / 2;
            }
            return Answer;
        }

        // Функция проверки все ли цыфр обработаны
        static bool AllDone(bool[] Done)
        {
            for (byte i=0; i<8; i++)
            {
                if (!Done[i]) { return false; }
            }
            return true;
        }

        // Функция проверки всех не нулевых массивов
        static bool[] CreateDone()
        {
            bool[] Answer = new bool[8];
            for (int i =0; i<8;i++)
            {
                if (EachCards[i].Count == 0) { Answer[i] = true; }
            }
            return Answer;
        }
    }

    public struct Pair
    {
        public int StartPoz; // Номер первого элемента пары в массиве AllCards
        public int EndPoz; // Номер последнего элемента пары в массиве AllCards
        public int Mass; // Масса пары (количество одинаковых цифр, которые покрывает пара
        public int Lenth; // Длина пары (количество цифр, которое покрывает пара в массива AllCards
        public byte Point; // Цифра
    }
    public struct PairLine
    {
        public List<Pair> Line;
        public int MinLen;
    }
}

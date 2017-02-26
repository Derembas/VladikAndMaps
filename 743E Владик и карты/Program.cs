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
            Stopwatch Counter = new Stopwatch();
            Stopwatch StepCount = new Stopwatch();
            //GlobalVars.n =Convert.ToInt32( Console.ReadLine());
            n = 1000;
            System.Random Rnd = new System.Random();
            //string Start = "8 8 8 7 8 7 6 8 6 5 8 5 4 8 4 3 8 3 2 8 2 1 8 1";
            //Случайное заполнение массива
            string Start = Rnd.Next(1, 9).ToString();
            for (int i = 1; i < n; i++)
            {
                Start += " " + Rnd.Next(1, 9);
            }
            //Console.WriteLine(Start);
            //string[] Mass = Console.ReadLine().Split(' ');
            string[] Mass = Start.Split(' ');
            Counter.Start();
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

            int LeftBorder = 0;
            int RightBorder = CardsCount.Min();
            StepCount.Start();
            int CurMass = RightBorder/ 2;
            while (LeftBorder != RightBorder)
            {
                StepCount.Restart();
                int CurMaxlen = 0;
                bool[] Done = new bool[8];
                Solve(0, 0, CurMass, Done, ref CurMaxlen);
                if (CurMaxlen == 0) { RightBorder = Math.Max(LeftBorder, CurMass - 1); }
                else
                {
                    MaxLen = Math.Max(MaxLen, CurMaxlen);
                    //if (CurMaxlen < 8 * (CurMass + 1)) { GoForver = false; }
                    LeftBorder = Math.Min(RightBorder, CurMass);
                }
                Console.WriteLine("Kol=" + CurMass + " - MaxLen=" + CurMaxlen + " - за " + StepCount.ElapsedMilliseconds + " мс.");
                CurMass = LeftBorder + (RightBorder - LeftBorder) / 2;
            }
            Console.WriteLine("Ans=" + CurMass);
            StepCount.Stop();
            Console.WriteLine("MaxLen: " + MaxLen);
            Console.WriteLine("Решение за " + Counter.ElapsedMilliseconds + " мс.");
            Console.ReadKey();
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
        static void Solve(int CurEnd, int CurLen, int CurMass, bool[] Done, ref int CurMaxLen )
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
                    Pair CurPair = LeftPair(i, CurEnd, CurMass + 1);
                    //if (CurPair.Lenth != 0)
                    //{
                    //    ThisRoundMass.Add(CurPair);
                    //    AllNumHavePair[i] = true;
                    //}
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
                    Solve(ThisRoundPair.EndPoz, CurLen, CurMass, Done,ref CurMaxLen);
                    CurLen -= ThisRoundPair.Mass;
                    Done[ThisRoundPair.Point] = false;
                }
            }
        }
        
        // Функция выбора самой левой пары со значением больше Min
        static Pair LeftPair(byte Point, int Min, int Mass)
        {
            Pair Rezult = new Pair();
            int StartPoz = Next(Point, Min);
            if (EachCards[Point].Count > StartPoz + Mass-1)
            {
                Rezult.StartPoz = EachCards[Point][StartPoz];
                Rezult.EndPoz = EachCards[Point][StartPoz+ Mass-1];
                Rezult.Mass = Mass;
                Rezult.Point = Point;
                Rezult.Lenth = Rezult.EndPoz-Rezult.StartPoz;
            }
            return Rezult;
        } 

        // Определение самого левого символа в оставшемся массиве со значением больше мин
        static int Next(byte Point, int Min)
        {
            int LeftBorder = 0;
            int RightBorder = EachCards[Point].Count-1;
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

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
            n = 100;
            System.Random Rnd = new System.Random();
            //string Start = "1 8 1 2 8 2 3 8 3 4 8 4 5 8 5 6 8 6 7 8 7 8 8 8";
            //Случайное заполнение массива
            string Start = "1";
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
            for (byte i = 0; i < 8; i++)
            {
                //Console.WriteLine((i + 1) + " - " + GlobalVars.CardsCount[i]);
                Stack[i] = i;
                for (int k = i-1; k >= 0; k--)
                {
                    if (CardsCount[Stack[k]] > CardsCount[i])
                    {
                        Stack[k + 1] = Stack[k];
                        Stack[k] = i;
                    }
                }
            }
            for (byte i = 0; i < 8; i++) { Console.WriteLine((i + 1) + " - " + Stack[i] + " - " + CardsCount[Stack[i]]); }
            MaxLen = 0;

            int LeftBorder = 0;
            int RightBorder = CardsCount.Min();
            bool GoForver = true;
            StepCount.Start();
            while (GoForver)
            {
                StepCount.Restart();
                if (LeftBorder == RightBorder) { GoForver = false; }
                int k = LeftBorder+ (RightBorder - LeftBorder) / 2;
                List<Pair> AllPairs = FindPairs(k);
                int CurMaxlen = 0;
                Solve(AllPairs,k,ref CurMaxlen, 0);
                if (CurMaxlen == 0) { RightBorder = Math.Max(LeftBorder, k - 1); }
                else
                {
                    MaxLen = Math.Max(MaxLen, CurMaxlen);
                    if (CurMaxlen < 8 * (k + 1)) { GoForver = false; }
                    LeftBorder = Math.Min(RightBorder, k + 1);
                }
                Console.WriteLine("Kol=" + k + " - MaxLen=" + CurMaxlen + " - за " + StepCount.ElapsedMilliseconds + " мс.");
            }
            StepCount.Stop();
            Console.WriteLine("MaxLen: " + MaxLen);
            Console.WriteLine("Решение за " + Counter.ElapsedMilliseconds + " мс.");
            Console.ReadKey();
        }

        // Функция поиска всех нужных пар
        static List<Pair> FindPairs(int Kol)
        {
            List<Pair> AllPairs = new List<Pair>();
            for (byte i = 0; i < 8; i++)
            {
                AllPairs.AddRange(MakeList(EachCards[Stack[i]], (Kol+1), i));
                AllPairs.AddRange(MakeList(EachCards[Stack[i]], Kol, i));
            }
            return AllPairs;
        }

        // Функция выбора всех пар чисел
        static List <Pair> MakeList(List<int> Arr, int Kol, byte CurPoint)
        {
            List<Pair> OutList = new List<Pair>();
            if (Arr.Count>=Kol && Kol>0)
            {
                for (int i=0; i<=Arr.Count-Kol; i++)
                {
                    Pair CurPair = new Pair();
                    CurPair.StartPoz = Arr[i];
                    CurPair.EndPoz = Arr[i + Kol-1];
                    CurPair.Mass = Kol;
                    CurPair.Point = CurPoint;
                    CurPair.Lenth = (CurPair.EndPoz - CurPair.StartPoz + 1);
                    OutList.Add(CurPair);
                }
            }
            return OutList;
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
        static void Solve(List<Pair> Arr, int CurLen, ref int CurMass, byte Step )
        {
            // Если дошли до 7го уровня вычисляем максимальную длинну
            if (Step == 7)
            {
                MaxLen = Math.Max(MaxLen, CurMass);
                return;
            }
            // Выбираем из массива всех пар самые левые для каждого значения карты
            List<Pair> ThisRoundMass = new List<Pair>();
            for (byte i=0; i<8;i++)
            {
                Pair CurPair = LeftPair(i, CurLen + 1, Arr);
                if (CurPair.Lenth != 0) { ThisRoundMass.Add(CurPair); }
                CurPair = LeftPair(i, CurLen , Arr);
                if (CurPair.Lenth != 0) { ThisRoundMass.Add(CurPair); }
            }
            foreach (Pair ThisRoundPair in ThisRoundMass)
            {
                CurMass += ThisRoundPair.Mass;
                Step++;
                List<Pair> NextStep = LeftPairs(ThisRoundPair, Arr);
                if (NextStep != null) { Solve(NextStep, CurLen, ref CurMass, Step); }
                CurMass -= ThisRoundPair.Mass;
                Step--;
            }
        }
        
        // Функция выбора оставшихся пар
        static List<Pair> LeftPairs(Pair SelectedPair, List<Pair> AllPairs)
        {
            List<Pair> Result = new List<Pair>();
            var LeftP = from Pair CurPair in AllPairs
                        where CurPair.Point != SelectedPair.Point && CurPair.StartPoz > SelectedPair.EndPoz
                        select CurPair;

            if (LeftP!=null && LeftP.Any()) { Result.AddRange(LeftP); }
            return Result;
        }

        // Функция выбора самой левой пары
        static Pair LeftPair(byte Point, int Mass, List<Pair> AllPairs)
        {
            Pair Rezult = new Pair();
            var LP = from Pair CurPair in AllPairs
                     where CurPair.Point == Point && CurPair.Mass==Mass
                     orderby CurPair.StartPoz
                     select CurPair;

            if (LP != null && LP.Any()) { Rezult= LP.First(); }
            return Rezult;
        } 
    }

    public struct Pair
    {
        public int StartPoz;
        public int EndPoz;
        public int Mass;
        public int Lenth;
        public byte Point;
    }
    public struct PairLine
    {
        public List<Pair> Line;
        public int MinLen;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doudizhu.Class
{
    public class MyRandom
    {
        private int Count;  //产生多少个数字
        private int Amount; //产生几份
        private int Max;    //最大数(不包含)
        private int Min;    //最小数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">产生多少个数字</param>
        /// <param name="amount">产生几份(须为count的约数)</param>
        /// <param name="min">最小数</param>
        /// <param name="max">最大数(不包含)</param>
        public MyRandom(int count, int amount, int min, int max)
        {
            if (count % amount != 0)
                throw new Exception("count须为amount的整数倍");
            if (min > max)
                throw new Exception("min必须小于等于max");
            Count = count;
            Amount = amount;
            Max = max;
            Min = min;
        }
        public List<int> Get()
        {
            Random rand = new Random();
            List<int> ran = new List<int>();
            for (int n = 0; n < Count / Amount; n++)
                ran.Add(rand.Next(Min, Max));
            for (int i = 1; i < Amount; i++)
            {
                List<int> temp = new List<int>();
                temp.AddRange(ran);
                for (int n = 0; n < Count / Amount; n++)
                {
                    int r = rand.Next(0, temp.Count);
                    ran.Insert(rand.Next(0, ran.Count), temp[r]);
                    temp.RemoveAt(r);
                }
            }
            return ran;
        }
    }
}

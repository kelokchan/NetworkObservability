using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObservability
{
    class PriorityQueue
    {
        public Arc[] arcList;

        public PriorityQueue(Arc[] list)
        {
            arcList = list;
        }

        public Arc Pop()
        {
            int smallestCost = 99999;
            int indexOfTheSmallest = -1;
            for (int i = 1; i < arcList.Length; i++)
            {
                if (arcList[i] == null)
                {
                    arcList[i] = new Arc
                    {
                        //Tail = a,
                        //Head = this,
                        Weigth = 99999,
                        //Distance = int.MaxValue
                    };
                    return null;
                }
                if (arcList[i].Weigth < smallestCost && !arcList[i].flagged)
                {
                    smallestCost = arcList[i].Weigth;
                    indexOfTheSmallest = i;
                }
                else if (arcList[i].Weigth == smallestCost) indexOfTheSmallest = 1;
            }
            Arc temp = arcList[indexOfTheSmallest];
            temp.flagged = true;
            return temp;
        }

        public void Add(int index, int value)
        {
            arcList[index].Weigth = value;
        }

    }
}

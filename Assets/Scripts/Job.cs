using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    abstract class Job
    {

        protected float techProgress;

        abstract internal void Tick();
    }

    class Farm : Job
    {
        protected float stock;
        float efficiency = 1;
        float production { get { return People.PeopleAt(People.Community.farm) * efficiency * (1 - People.unrest[People.Community.farm]); } }
        internal override void Tick()
        {
            stock += production;
            if (stock > production * 2)
            {
                stock = production * 2;
                God.TheOne.Console("Food stock overflow. Stop wasting food!");
            }
        }
    }

    class Quarry : Job
    {
        protected float stock;
        float efficiency = 0.05f;   // you need 20 people to make 1 stone
        float production { get { return People.PeopleAt(People.Community.quarry) * efficiency * (1 - People.unrest[People.Community.quarry]); } }
        internal override void Tick()
        {
            stock += production;
        }

        internal int HasStones()
        {
            return (int)stock;
        }
    }

    class Road : Job
    {
        int teamsize = 10;
        int transportTime = 5;      // time to bring a stone to the docks
        List<Team> teamsOnTheWay = new List<Team>();

        internal override void Tick()
        {
            int availablePeople = People.PeopleAt(People.Community.transportRoad) - teamsOnTheWay.Sum(t => t.people);
            if (availablePeople < 0) God.TheOne.Console("Road has negative people!");
            int availableStones = God.TheOne.quarry.HasStones();
            int stonesToBeMoved = Math.Max(availablePeople / teamsize, availableStones);
            if (stonesToBeMoved > 0)
            {
                teamsOnTheWay.Add(new Team(stonesToBeMoved * teamsize, stonesToBeMoved));
            }
            for (int i = teamsOnTheWay.Count - 1; i >= 0; i--)
            {
                teamsOnTheWay[i].Time++;
                if(teamsOnTheWay[i].Time > transportTime)
                {
                    God.TheOne.river.newStonesArrive(teamsOnTheWay[i].stones);
                    teamsOnTheWay.RemoveAt(i);
                }
            }

        }

        class Team
        {
            readonly public int people;
            readonly public int stones;
            public int Time;
            public Team(int people, int stones) { this.people = people; this.stones = stones; Time = 0; }
        }
    }

    class River : Job
    {
        int dockStock;
        List<Boat> boats = new List<Boat>() { new Boat(), new Boat() };
        public enum Priority { smallest, fastest};
        public Priority priority;
        internal override void Tick()
        {
            while (dockStock > 0)
            {
                Boat next = boats.FindAll(b => b.isInDock && b.IsActive).
                    OrderBy(b => { switch (priority) { case Priority.smallest: return b.capacity; case Priority.fastest: return b.GetTimeRequired(); } throw new Exception("invalid priority"); }).
                    First();
                if (dockStock > next.capacity)
                {
                    next.Load(next.capacity);
                    dockStock -= next.capacity;
                }
                else break;
            }
        }

        internal void newStonesArrive(int stones) { dockStock += stones; }

        class Boat
        {
            public int crew { get; private set; }
            public int stones { get; private set; }
            public int timeTillArrival { get; private set; }
            public bool isInDock { get; private set; }
            readonly public int capacity;
            readonly public int minCrew;
            readonly public int maxCrew;
            readonly public int minTravelTime; // How many ticks this boat needs to take the trip on minimum crew
            readonly public int maxTravelTime; // How many ticks this boat needs to take the trip on maximum crew
            public bool IsActive { get { return crew >= minCrew; } }

            public Boat(int capacity = 5, int minCrew = 10, int maxCrew = 10, int minSpeed = 5, int maxSpeed = 5)
            {
                this.capacity = capacity; this.minCrew = minCrew; this.maxCrew = maxCrew; this.minTravelTime = minSpeed; this.maxTravelTime = maxSpeed;
                crew = 0; stones = 0; timeTillArrival = 0;
            }

            public void Tick()
            {
                if (IsActive)
                {
                    if (timeTillArrival > 0)
                    {
                        timeTillArrival--;
                        if(timeTillArrival == 0)    // Boat just arrived
                        {
                            if(stones> 0)
                            {
                                God.TheOne.construction.stock += stones;
                                timeTillArrival = GetTimeRequired();
                            }
                            else { isInDock = true; }
                        }
                    }
                }
            }

            public int GetTimeRequired()
            {
                int maxCrewDiff = maxCrew - minCrew;
                int maxTimeDiff = maxTravelTime - minTravelTime;
                int crewDiff = crew - minCrew;
                return minTravelTime + (int)((float)maxTimeDiff / maxCrewDiff * crewDiff);
            }

            public void Man(int extraCrew)
            {
                crew += extraCrew;
                if (crew > maxCrew) throw new ArgumentException("to many crew for this boat");
            }

            public void Load(int stones)
            {
                if (this.stones != 0) throw new Exception("Trying to load an already filled boat");
                this.stones = stones;
                timeTillArrival = GetTimeRequired() + 1;        // One tick for loading
            }
        }
    }

    class Construction : Job
    {
        internal float stock;
        float constructionSpeed = 0.05f;   // you need 20 people to place 1 stone
        Queue<Task> tasks = new Queue<Task>();
        float progress = 0;
        internal override void Tick()
        {
            float totalWork = People.PeopleAt(People.Community.construction) * constructionSpeed;
            while (totalWork > 0)
            {
                if (tasks.Peek().work < totalWork)
                {
                    totalWork -= tasks.Peek().work;
                    tasks.Dequeue().onCompletion();
                }
                else
                {
                    tasks.Peek().work -= totalWork;
                    break;
                }
            }
            float workOnPyramid = Math.Min(totalWork, stock);
            stock -= workOnPyramid;
            totalWork -= workOnPyramid;
            progress += workOnPyramid;
            if (totalWork > 0) God.TheOne.Console("" + totalWork.ToString("d2") + " tasks worth of work is being wasted by your construction crew.");
        }

        public void AddTask(float work, Action onCompletion) { tasks.Enqueue(new Task(work, onCompletion)); }

        class Task
        {
            public float work;
            public Action onCompletion { get; private set; }
            public Task(float work, Action onCompletion) { this.work = work; this.onCompletion = onCompletion; }
        }
    }

    class Military : Job
    {
        internal override void Tick()
        {
            throw new NotImplementedException();
        }
    }
}

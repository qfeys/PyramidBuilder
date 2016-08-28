using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    abstract class Job
    {
        public float techProgress = 0;

        abstract internal void Tick();
    }

    class Farm : Job
    {
        public  float stock { get; private set; }
        float efficiency = 1;
        public float production { get { return People.PeopleAt(People.Community.farm) * efficiency * People.Productivity(People.Community.farm); } }
        public float storageCapacity { get; private set; }

        public Farm() { storageCapacity = 50000; }
        internal override void Tick()
        {
            stock += production;
            if (stock > storageCapacity)
            {
                stock = storageCapacity;
                God.TheOne.Console("Food stock overflow. Stop wasting food!");
            }
        }

        internal void TakeFood(float requestedFood)
        {
            stock -= requestedFood;
        }
    }

    class Quarry : Job
    {
        public float stock { get; private set; }
        float efficiency = 0.05f;   // you need 20 people to make 1 stone
        public float production { get { return People.PeopleAt(People.Community.quarry) * efficiency * People.Productivity(People.Community.quarry); } }
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
        public List<Team> teamsOnTheWay { get; private set; }
        public int peopleBusy { get { return teamsOnTheWay.Sum(t => t.people); } }
        public int stonesInTransit { get { return teamsOnTheWay.Sum(t => t.stones); } }

        public Road() { teamsOnTheWay = new List<Team>(); }
        internal override void Tick()
        {
            int availablePeople = People.PeopleAt(People.Community.transportRoad) - teamsOnTheWay.Sum(t => t.people);
            if (availablePeople < 0) God.TheOne.Console("Road has negative people!");
            int availableStones = God.TheOne.quarry.HasStones();
            int stonesToBeMoved = Math.Min(availablePeople / teamsize, availableStones);
            if (stonesToBeMoved > 0)
            {
                teamsOnTheWay.Add(new Team(stonesToBeMoved * teamsize, stonesToBeMoved, transportTime));
            }
            for (int i = teamsOnTheWay.Count - 1; i >= 0; i--)
            {
                if(God.random.NextDouble()<People.Productivity(People.Community.transportRoad))
                    teamsOnTheWay[i].TimeLeft--;        // If the productivity is to low, somtimes they will not move.
                if(teamsOnTheWay[i].TimeLeft <= 0)
                {
                    God.TheOne.river.newStonesArrive(teamsOnTheWay[i].stones);
                    teamsOnTheWay.RemoveAt(i);
                }
            }

        }

        public class Team
        {
            readonly public int people;
            readonly public int stones;
            public int TimeLeft;
            public Team(int people, int stones, int transportTime) { this.people = people; this.stones = stones; TimeLeft = 0; }
        }
    }

    class River : Job
    {
        int dockStock;
        public List<Boat> boats { get; private set; }
        public enum Priority { smallest, fastest};
        public Priority priority = Priority.smallest;

        public float peopleBusy { get { return boats.Sum(b => b.crew); } }

        public River() { boats = new List<Boat>() { new Boat("Elise") }; }
        internal override void Tick()
        {
            while (dockStock > 0)
            {
                Boat next = boats.FindAll(b => b.isInDock && b.IsActive).
                    OrderBy(b => { switch (priority) { case Priority.smallest: return b.capacity; case Priority.fastest: return b.GetTimeRequired(); } throw new Exception("invalid priority"); }).
                    FirstOrDefault();
                if (next != null && dockStock > next.capacity)
                {
                    next.Load(next.capacity);
                    dockStock -= next.capacity;
                }
                else break;
            }
            boats.ForEach(b => b.Tick());
        }

        internal void newStonesArrive(int stones) { dockStock += stones; }

        public class Boat
        {
            public string name;
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

            public Boat(string name, int capacity = 5, int minCrew = 10, int maxCrew = 10, int minSpeed = 5, int maxSpeed = 5)
            {
                this.name = name; this.capacity = capacity; this.minCrew = minCrew; this.maxCrew = maxCrew; this.minTravelTime = minSpeed; this.maxTravelTime = maxSpeed;
                crew = 0; stones = 0; timeTillArrival = 0;
            }

            public void Tick()
            {
                if (IsActive)
                {
                    if (timeTillArrival > 0)
                    {
                        if (God.random.NextDouble() < People.Productivity(People.Community.transportRiver))
                            timeTillArrival--;      // Sometimes they will not move if productivity is to low
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
        public float stock { get; set; }
        float constructionSpeed = 0.05f;   // you need 20 people to place 1 stone
        public float workSpeed { get { return People.PeopleAt(People.Community.construction) * constructionSpeed * People.Productivity(People.Community.construction); } }
        public Queue<Task> tasks { get; private set; }
        float progress = 0;
        public Construction() { tasks = new Queue<Task>(); }
        internal override void Tick()
        {
            float totalWork = People.PeopleAt(People.Community.construction) * constructionSpeed;
            while (totalWork > 0 && tasks.Count != 0)
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
            //if (totalWork > 0) God.TheOne.Console("" + totalWork.ToString("n2") + " tasks worth of work is being wasted by your construction crew.");
        }

        public void AddTask(string name, float work, Action onCompletion) { tasks.Enqueue(new Task(name, work, onCompletion)); }

        public class Task
        {
            public string name;
            public float work;
            public Action onCompletion { get; private set; }
            public Task(string name, float work, Action onCompletion) { this.name = name; this.work = work; this.onCompletion = onCompletion; }
        }
    }

    class Military : Job
    {
        float suppressionEfficiancy = 1.0f;
        public float totalSuppression { get { return People.PeopleAt(People.Community.military) * suppressionEfficiancy * People.Productivity(People.Community.military); } }
        internal override void Tick()
        {
            // Do nthing, I guess
        }

        
    }
}
